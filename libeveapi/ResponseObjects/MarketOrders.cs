using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace libeveapi
{
    /// <summary>
    /// Represents a character or corporation AccountBalance response from the eve api
    /// http://wiki.eve-dev.net/APIv2_Corp_MarketOrders_XML
    /// </summary>
    class MarketOrder : ApiResponse
    {
        public MarketOrderItem[] MarketOrderItems = new MarketOrderItem[0];

        /// <summary>
        /// Create an MarketOrderList by parsing an XmlDocument response from the eveapi
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <returns></returns>
        public static MarketOrder FromXmlDocument(XmlDocument xmlDoc)
        {
            MarketOrder MarketOrderList = new MarketOrder();
            MarketOrderList.ParseCommonElements(xmlDoc);

            List<MarketOrderItem> orders = new List<MarketOrderItem>();
            foreach (XmlNode node in xmlDoc.SelectNodes("//rowset[@name='orders']/row"))
            {
                orders.Add(ParseTransactionRow(node));
            }
            MarketOrderList.MarketOrderItems = orders.ToArray();

            return MarketOrderList;
        }

        /// <summary>
        /// Create an MarketOrderItem by parsing a single row
        /// </summary>
        /// <param name="marketOrderRow"></param>
        /// <returns></returns>
        protected static MarketOrderItem ParseTransactionRow(XmlNode marketOrderRow)
        {
            MarketOrderItem marketItem = new MarketOrderItem();

            marketItem.orderID = Convert.ToInt32(walletTransactionRow.Attributes["orderID"].InnerText);
            marketItem.charID = Convert.ToInt64(walletTransactionRow.Attributes["charID"].InnerText);
            marketItem.stationID = Convert.ToInt64(walletTransactionRow.Attributes["stationID"].InnerText);
            marketItem.volEntered = Convert.ToInt64(walletTransactionRow.Attributes["volEntered"].InnerText);
            marketItem.volRemaining = Convert.ToInt64(walletTransactionRow.Attributes["volRemaining"].InnerText);
            marketItem.minVolume = Convert.ToInt64(walletTransactionRow.Attributes["minVolume"].InnerText);

            switch (Convert.ToInt32(starbaseNode.Attributes["orderState"].InnerText))
            {
                case 0:
                    marketItem.orderState = marketOrderState.OpenActive;
                    break;
                case 1:
                    marketItem.orderState = marketOrderState.Closed;
                    break;
                case 2:
                    marketItem.orderState = marketOrderState.ExpiredFulfilled;
                    break;
                case 3:
                    marketItem.orderState = marketOrderState.Cancelled;
                    break;
                case 4:
                    marketItem.orderState = marketOrderState.Pending;
                    break;
                case 5:
                    marketItem.orderState = marketOrderState.CharacterDeleted;
                    break;
                default:
                    break;
            }

            marketItem.typeID = Convert.ToInt64(walletTransactionRow.Attributes["typeID"].InnerText);
            marketItem.range = Convert.ToInt32(walletTransactionRow.Attributes["range"].InnerText);
            marketItem.accountKey = Convert.ToInt32(walletTransactionRow.Attributes["accountKey"].InnerText);
            marketItem.duration = Convert.ToInt32(walletTransactionRow.Attributes["duration"].InnerText);
            marketItem.escrow = (float)Convert.ToDouble(walletTransactionRow.Attributes["escrow"].InnerText);
            marketItem.price = (float)Convert.ToDouble(walletTransactionRow.Attributes["price"].InnerText);
            marketItem.bid = Convert.ToBoolean(walletTransactionRow.Attributes["bid"].InnerText);
            marketItem.issued = Convert.ToDateTime(walletTransactionRow.Attributes["issued"].InnerText);

            return marketItem;
        }
    }

    public class MarketOrderItem
    {
        
        /// <summary>
        /// Order id, not forever unique but for this pull they will be unique
        /// </summary>
        public int orderID;

        /// <summary>
        /// Character ID of the character who placed the market order
        /// </summary>
        public long charID;

        /// <summary>
        /// The ID of the station that the order was placed in
        /// </summary>
        public long stationID;

        /// <summary>
        /// The quantity of the items required/offered when the order was placed
        /// </summary>
        public long volEntered;

        /// <summary>
        /// The quantitiy of items that are still for sale/ still required
        /// </summary>
        public long volRemaining;

        /// <summary>
        /// For bids (buy orders) the minimum quantity that must be sold in one
        /// sale so that the order is accepted.
        /// </summary>
        public long minVolume;

        /// <summary>
        /// See <see cref="marketOrderState"/> for full descriptions of each order state
        /// </summary>
        public marketOrderState orderState;

        /// <summary>
        /// This is the typeId of the item that is being bought/sold
        /// </summary>
        public long typeID;

        /// <summary>
        /// This is the range of the order
        /// For sell orders it is always 32767
        /// For buy orders it is either -1 = station, 0 = solar system
        /// Any number above 1 is number of jumps in region
        /// And 32767 means region
        /// </summary>
        public int range;

        /// <summary>
        /// This is which wallet the order is using, for a personal order
        /// this will always be 1000, for corporation orders it can be 1000-1006
        /// depending on which wallet is being used
        /// </summary>
        public int accountKey;

        /// <summary>
        /// How many days this order is good for. Expiration is issued + duration in days
        /// </summary>
        public int duration;

        /// <summary>
        /// How much ISK is in escrow. Valid for buy orders only (I believe).
        /// </summary>
        public float escrow;

        /// <summary>
        /// The cost per unit for this order
        /// </summary>
        public float price;

        /// <summary>
        /// If true this is a bid or buy order, else it is a sell order
        /// </summary>
        public bool bid;

        /// <summary>
        /// This is when the order was issued
        /// </summary>
        public DateTime issued;
    }

    public enum marketOrderState
    {
        /// <summary>
        /// If the market order is still active and up on the market
        /// </summary>
        OpenActive = 0,

        /// <summary>
        /// The order has been closed
        /// </summary>
        Closed = 1,

        /// <summary>
        /// The order has expired, or has been fufilled so it is no longer active
        /// </summary>
        ExpiredFulfilled = 2,

        /// <summary>
        /// The order was canceled
        /// </summary>
        Canceled = 3,

        /// <summary>
        /// The order is currently pending, and not on the market
        /// </summary>
        Pending = 4,

        /// <summary>
        /// The character that this order was associated with has been deleted
        /// </summary>
        CharacterDeleted = 5
    }

    public enum MarketOrderType
    {
        Corporation,
        Character
    }
}