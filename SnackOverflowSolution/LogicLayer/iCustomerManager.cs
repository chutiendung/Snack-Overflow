﻿using DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicLayer
{
    interface ICustomerManager
    {
        bool CreateCommercialAccount(CommercialCustomer cc);
    }
}