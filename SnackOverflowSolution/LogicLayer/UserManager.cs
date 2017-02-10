﻿using DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicLayer
{
    public class UserManager : IUserManager
    {

        public User RetrieveUserByUserName(string userName)
        {
            User user = null;
            if (userName.Equals("BLPlatinum@aol.com"))
            {
                user = new User();
                user.UserId = 10000;
                user.FirstName = "Bud";
                user.LastName = "Platinum";
                user.UserName = "BLPlatinum@aol.com";
                user.Phone = "555-555-5555";
                user.Active = true;
                user.EmailAddress = "BLPlatinum@aol.com";
                user.EmailPreferences = true;
                user.PreferredAddressId = 10000;
            }
            return user;
        }
    }
}