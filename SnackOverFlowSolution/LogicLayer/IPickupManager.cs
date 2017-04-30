﻿using DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicLayer
{
    public interface IPickupManager
    {
        List<Pickup> RetrievePickupsForDriver(int? driverId);
        List<Pickup> RetrieveUnpickedupPickupsForDriver(int? driverId);

        bool UpdatePickupLine(PickupLine oldLine, PickupLine newLine);

        PickupLine RetrievePickupLineById(int? pickupLineId);

        List<PickupLine> RetrievePickupLinesReceived();

        Pickup RetrievePickupById(int? pickupId);

        bool DeletePickupLine(PickupLine pickupLine);
    }
}
