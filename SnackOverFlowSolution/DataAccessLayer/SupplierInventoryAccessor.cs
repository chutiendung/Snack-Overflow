﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataObjects;
using System.Data.SqlClient;
using System.Data;

namespace DataAccessLayer
{
    /// <summary>
    /// Created 2017-03-29 by William Flood
    /// Manages database access for supplier inventory
    /// </summary>
    public class SupplierInventoryAccessor
    {
        /// <summary>
        /// Created 2017-03-29 by William Flood
        /// Adds a quantity of stock to a supplier's inventory
        /// </summary>
        /// <param name="toAdd"></param>
        /// <returns></returns>
        public static int CreateSupplierInventory (SupplierInventory toAdd)
        {
            var rowsAffected = 0;
            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_create_supplier_inventory",conn);
            cmd.Parameters.AddWithValue("@AGREEMENT_ID", toAdd.AgreementID);
            cmd.Parameters.AddWithValue("@DATE_ADDED", toAdd.DateAdded);
            cmd.Parameters.AddWithValue("@QUANTITY", toAdd.Quantity);
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                conn.Open();
                rowsAffected = cmd.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close(); // good housekeeping approved!
            }
            return rowsAffected;
        }
    }
}
