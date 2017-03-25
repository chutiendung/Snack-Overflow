﻿using DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicLayer
{
    public interface IOrderLineManager
    {
        int CreateOrderLine(OrderLine orderLine);
        List<OrderLine> RetrieveOrderLineListByProductOrderId(int ProductOrderId, Decimal OrderAmount);
    }
}
