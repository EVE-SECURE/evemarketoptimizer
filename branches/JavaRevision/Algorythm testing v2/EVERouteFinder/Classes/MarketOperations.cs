using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EVERouteFinder.Classes
{
    class MarketOperations
    {
        List<EVEOrder> sales;
        List<EVEOrder> bids;
        double cargohold;
        double maxCap;

        private EVEOrder searchOrderByID(List<EVEOrder> orderList, int id)
        {
            return orderList.Find(delegate(EVEOrder o) { return o.OrderID == id; });
        }

        private List<EVEOrder> searchOrdersByItem(List<EVEOrder> orderList, int typeid)
        {
            return orderList.FindAll(delegate(EVEOrder o) { return o.TypeID == typeid; });
        }

        private List<EVEOrder> searchOrders(List<EVEOrder> orderList, int bid)
        {
            return orderList.FindAll(delegate(EVEOrder o) { return o.Bid == bid; });
        }

        public MarketOperations(List<EVEOrder> orderlist, double cargoholdSize, double maxCapital)
        {
            this.sales = searchOrders(orderlist, 1);
            this.bids = searchOrders(orderlist, 0);
        }
    }
}
