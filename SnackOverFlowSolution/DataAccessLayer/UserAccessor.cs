﻿using DataObjects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class UserAccessor
    {
        
        
        

        public static List<User> RetrieveList()
        {
            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_retrieve_app_user_list", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                conn.Open();
                var reader = cmd.ExecuteReader();
                var resultList = new List<User>();
                while (reader.Read())
                {
                    resultList.Add(new User()
                    {
                        UserId = reader.GetInt32(0),
                        FirstName = reader.GetString(1),
                        //Test if the query returned a null value.
                        LastName = (reader.IsDBNull(2) ? null : reader.GetString(2)),
                        Phone = reader.GetString(3),
                        //Test if the query returned a null value.
                        PreferredAddressId = (reader.IsDBNull(4) ? null : (int?)reader.GetInt32(4)),
                        EmailAddress = reader.GetString(5),
                        EmailPreferences = reader.GetBoolean(6),
                        UserName = reader.GetString(9),
                        Active = reader.GetBoolean(10)
                    });
                }
                return resultList;
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close(); // good housekeeping approved!
            }
        }

        

        /// <summary>
        /// Christian Lopez
        /// Created on 2017/02/01
        /// 
        /// Access DB to get User by given username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public static User RetrieveUserByUsername(string username)
        {
            User user = null;

            var conn = DBConnection.GetConnection();
            var cmdText = @"sp_retrieve_app_user_by_username";
            var cmd = new SqlCommand(cmdText, conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@USERNAME", SqlDbType.NVarChar, 50);
            cmd.Parameters["@USERNAME"].Value = username;

            try
            {
                conn.Open();
                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    user = new User()
                    {
                        UserId = reader.GetInt32(0),
                        FirstName = reader.GetString(1),
                        LastName = reader.GetString(2),
                        Phone = reader.GetString(3),
                        //PreferredAddressId = reader.GetInt32(4),
                        EmailAddress = reader.GetString(5),
                        EmailPreferences = reader.GetBoolean(6),
                        UserName = reader.GetString(7),
                        Active = reader.GetBoolean(8)
                    };
                    if (!reader.IsDBNull(4))
                    {
                        user.PreferredAddressId = reader.GetInt32(4);
                    }
                    else
                    {
                        user.PreferredAddressId = null;
                    }
                }
                reader.Close();
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                conn.Close();
            }

            return user;
        }

        public static int Create(User toCreate)
        {
            var results = 0;
            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_create_app_user", conn);
            cmd.Parameters.Add("@FIRST_NAME", SqlDbType.NVarChar, 150);
            cmd.Parameters.Add("@LAST_NAME", SqlDbType.NVarChar, 100);
            cmd.Parameters.Add("@PHONE", SqlDbType.NVarChar, 15);
            cmd.Parameters.Add("@PREFERRED_ADDRESS_ID", SqlDbType.Int);
            cmd.Parameters.Add("@E_MAIL_ADDRESS", SqlDbType.NVarChar, 50);
            cmd.Parameters.Add("@E_MAIL_PREFERENCES", SqlDbType.Bit);
            cmd.Parameters.Add("@PASSWORD_HASH", SqlDbType.NVarChar, 64);
            cmd.Parameters.Add("@PASSWORD_SALT", SqlDbType.NVarChar, 64);
            cmd.Parameters.Add("@USER_NAME", SqlDbType.NVarChar, 50);
            cmd.Parameters.Add("@ACTIVE", SqlDbType.Bit);
            cmd.Parameters["@FIRST_NAME"].Value = toCreate.FirstName;
            cmd.Parameters["@LAST_NAME"].Value = toCreate.LastName;
            cmd.Parameters["@PHONE"].Value = toCreate.Phone;
            cmd.Parameters["@PREFERRED_ADDRESS_ID"].Value = toCreate.PreferredAddressId;
            cmd.Parameters["@E_MAIL_ADDRESS"].Value = toCreate.EmailAddress;
            cmd.Parameters["@E_MAIL_PREFERENCES"].Value = toCreate.EmailPreferences;
            cmd.Parameters["@PASSWORD_HASH"].Value = toCreate.PasswordHash;
            cmd.Parameters["@PASSWORD_SALT"].Value = toCreate.PasswordSalt;
            cmd.Parameters["@USER_NAME"].Value = toCreate.UserName;
            cmd.Parameters["@ACTIVE"].Value = toCreate.Active;
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                conn.Open();
                results = cmd.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close(); // good housekeeping approved!
            }
            return results;
        }


        public String RetrieveUserSalt (String userName)
        {

            var results = "";
            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_retrieve_user_salt", conn);
            cmd.Parameters.Add("@Username", SqlDbType.NVarChar,50);
            cmd.Parameters["@Username"].Value = userName;
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                conn.Open();
                var reader = cmd.ExecuteReader();
                if(reader.Read())
                {
                    results = reader.GetString(0);
                }
            } catch 
            {
                throw;
            }
            return results;
        }

        /// <summary>
        /// William Flood 
        /// Created on 2017-04-12
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static String RetrieveUserSaltByEmail(String email)
        {

            var results = "";
            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_retrieve_user_salt_by_email", conn);
            cmd.Parameters.Add("@EmailAddress", SqlDbType.NVarChar, 50);
            cmd.Parameters["@EmailAddress"].Value = email;
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                conn.Open();
                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    results = reader.GetString(0);
                }
            }
            catch
            {
                throw;
            }
            return results;
        }

        public static User Login(String userName, String hash)
        {
            User userFound = null;
            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_login", conn);
            cmd.Parameters.Add("@Username", SqlDbType.NVarChar, 50);
            cmd.Parameters.Add("@Password_Hash", SqlDbType.NVarChar, 64);
            cmd.Parameters["@Username"].Value = userName;
            cmd.Parameters["@Password_Hash"].Value = hash;
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                conn.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        userFound = new User()
                        {
                            UserId = reader.GetInt32(0),
                            FirstName = reader.GetString(1),
                            LastName = reader.GetString(2),
                            Phone = reader.GetString(3),
                            //PreferredAddressId = reader.GetInt32(4),
                            EmailAddress = reader.GetString(5),
                            EmailPreferences = reader.GetBoolean(6),
                            UserName = reader.GetString(9),
                            Active = reader.GetBoolean(10)
                        };
                    }
                }
                return userFound;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// William Flood
        /// Created on 2017/04/12
        /// Uses a user's email address and hashed password to log in
        /// </summary>
        /// <param name="email"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public static User WebLogin(String email, String hash)
        {
            User userFound = null;
            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_web_login", conn);
            cmd.Parameters.Add("@EmailAddress", SqlDbType.NVarChar, 50);
            cmd.Parameters.Add("@Password_Hash", SqlDbType.NVarChar, 64);
            cmd.Parameters["@EmailAddress"].Value = email;
            cmd.Parameters["@Password_Hash"].Value = hash;
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                conn.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        userFound = new User()
                        {
                            UserId = reader.GetInt32(0),
                            FirstName = reader.GetString(1),
                            LastName = reader.GetString(2),
                            Phone = reader.GetString(3),
                            //PreferredAddressId = reader.GetInt32(4),
                            EmailAddress = reader.GetString(5),
                            EmailPreferences = reader.GetBoolean(6),
                            UserName = reader.GetString(9),
                            Active = reader.GetBoolean(10)
                        };
                    }
                }
                return userFound;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Christian Lopez
        /// Created on 2017/02/01
        /// 
        /// Access DB to get UserAddress by given address ID
        /// </summary>
        /// <param name="preferredAddressId">The ID for the address</param>
        /// <returns>The associated UserAddress</returns>
        public static UserAddress RetrieveUserAddress(int? preferredAddressId)
        {

            UserAddress userAddress = null;

            if (null == preferredAddressId)
            {
                return userAddress;
            }

            var conn = DBConnection.GetConnection();
            var cmdText = @"sp_retrieve_user_address";
            var cmd = new SqlCommand(cmdText, conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@USER_ADDRESS_ID", SqlDbType.Int);
            cmd.Parameters["@USER_ADDRESS_ID"].Value = preferredAddressId;

            try
            {
                conn.Open();
                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    userAddress = new UserAddress()
                    {
                        UserAddressId = reader.GetInt32(0),
                        UserId = reader.GetInt32(1),
                        AddressLineOne = reader.GetString(2),
                        AddressLineTwo = reader.GetString(3),
                        City = reader.GetString(4),
                        State = reader.GetString(5),
                        Zip = reader.GetString(6)
                    };
                }
                reader.Close();
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                conn.Close();
            }

            return userAddress;
        }

        /// <summary>
        /// Bobby Thorne
        /// 3/4/2017
        /// 
        /// returns username of the email that is provided
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public string RetrieveUsernameByEmail(string email)
        {
            string username="";
            var conn = DBConnection.GetConnection();
            var cmdText = @"sp_retrieve_app_username_by_email";
            var cmd = new SqlCommand(cmdText, conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@E_MAIL_ADDRESS", SqlDbType.NVarChar,50);
            cmd.Parameters["@E_MAIL_ADDRESS"].Value = email;
            try
            {
                conn.Open();
                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    username = reader.GetString(0);

                }
                reader.Close();
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                conn.Close();
            }

            return username;
        }

        /// <summary>
        /// William Flood
        /// Created on 2017/02/28
        /// 
        /// Allows a user to update one's password
        /// </summary>
        /// <param name="username"></param>
        /// <param name="oldSalt"></param>
        /// <param name="oldHash"></param>
        /// <param name="newSalt"></param>
        /// <param name="newHash"></param>
        /// <returns></returns>
        public int UpdatePassword(String username, String oldSalt, String oldHash, String newSalt, String newHash)
        {

            var results = 0;
            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand(@"sp_update_user_password", conn);
            cmd.Parameters.Add("@USERNAME", SqlDbType.NVarChar);
            cmd.Parameters.Add("@OLD_SALT", SqlDbType.NVarChar);
            cmd.Parameters.Add("@OLD_HASH", SqlDbType.NVarChar);
            cmd.Parameters.Add("@NEW_SALT", SqlDbType.NVarChar);
            cmd.Parameters.Add("@NEW_HASH", SqlDbType.NVarChar);
            cmd.Parameters["@USERNAME"].Value = username;
            cmd.Parameters["@OLD_SALT"].Value = oldSalt;
            cmd.Parameters["@OLD_HASH"].Value = oldHash;
            cmd.Parameters["@NEW_SALT"].Value = newSalt;
            cmd.Parameters["@NEW_HASH"].Value = newHash;
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                conn.Open();
                results = cmd.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }
            return results;
        }

        public int ResetPassword(String username, String salt, String passwordHash)
        {
            var results = 0;
            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand(@"sp_reset_user_password", conn);
            cmd.Parameters.Add("@USERNAME", SqlDbType.NVarChar);
            cmd.Parameters.Add("@PASSWORD_SALT", SqlDbType.NVarChar);
            cmd.Parameters.Add("@PASSWORD_HASH", SqlDbType.NVarChar);
            cmd.Parameters["@USERNAME"].Value = username;
            cmd.Parameters["@PASSWORD_SALT"].Value = salt;
            cmd.Parameters["@PASSWORD_HASH"].Value = passwordHash;
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                conn.Open();
                results = cmd.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }
            return results;
        }

        /// <summary>
        /// Christian Lopez
        /// 2017/03/09
        /// 
        /// Attempts to retrieve a user from the DB associated with the userId
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static User RetrieveUserByUserId(int userId)
        {
            User user = null;

            var conn = DBConnection.GetConnection();
            var cmdText = @"sp_retrieve_app_user";
            var cmd = new SqlCommand(cmdText, conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@USER_ID", SqlDbType.Int);
            
            cmd.Parameters["@USER_ID"].Value = userId;

            try
            {
                conn.Open();
                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    user = new User()
                    {
                        UserId = reader.GetInt32(0),
                        FirstName = reader.GetString(1),
                        LastName = reader.GetString(2),
                        Phone = reader.GetString(3),
                        EmailAddress = reader.GetString(5),
                        EmailPreferences = reader.GetBoolean(6),
                        UserName = reader.GetString(9),
                        Active = reader.GetBoolean(10)
                    };
                    if (!reader.IsDBNull(4))
                    {
                        user.PreferredAddressId = reader.GetInt32(4);
                    }
                }
            }
            catch (Exception)
            {
                
                throw;
            }
            finally
            {
                conn.Close();
            }

            return user;
        }
    }
}