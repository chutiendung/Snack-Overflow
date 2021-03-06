﻿using DataAccessLayer;
using DataObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicLayer
{
    /// <summary>
    /// Aaron Usher
    /// Created: 2017/02/17
    /// 
    /// Class to represent a delivery manager.
    /// </summary>
    public class DeliveryManager : IDeliveryManager
    {
        /// <summary>
        /// Aaron Usher
        /// Created: 2017/02/17
        /// 
        /// Retrieves every delivery from the database.
        /// </summary>
        /// <remarks>
        /// Robert Forbes
        /// 
        /// Updated:
        /// 2017/05/05
        /// 
        /// Now pulls address for each delivery
        /// </remarks>
        /// <returns>The deliveries.</returns>
        public List<Delivery> RetrieveDeliveries()
        {
            var list = new List<Delivery>();

            try
            {
                list = DeliveryAccessor.RetrieveDeliveries();
                foreach(var d in list){
                    d.Address = DeliveryAccessor.RetrieveUserAddressForDelivery(d.DeliveryId);
                }
            }
            catch (Exception)
            {

                throw;
            }

            return list;
        }

        /// <summary>
        /// Retrieves a vehicle based on a deliveryID.
        /// </summary>
        /// <param name="deliveryID">The deliveryID.</param>
        /// <returns>The vehicle.</returns>
        public Vehicle RetrieveVehicleByDelivery(int deliveryID)
        {
            Vehicle result = null;
            try
            {
                result = VehicleAccessor.RetrieveVehicleByDelivery(deliveryID);
            }
            catch (Exception)
            {

                throw;
            }
            return result;
        }


        /// <summary>
        /// Robert Forbes
        /// Created: 2017/03/09
        /// 
        /// Creates a new delivery
        /// </summary>
        /// 
        /// <remarks>
        /// Aaron Usher
        /// Updated: 2017/04/07
        /// 
        /// Method signature changed from taking delivery pieces to a Delivery itself.
        /// </remarks>
        /// <param name="delivery">The delivery to add to the database.</param>
        /// <returns>bool representing if the creation was successful</returns>
        public bool CreateDelivery(Delivery delivery)
        {
            bool result = false;
            try
            {
                if (DeliveryAccessor.CreateDelivery(delivery) == 1)
                {
                    result = true;
                    
                }
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }

        /// <summary>
        /// Robert Forbes
        /// Created: 2017/03/09
        /// 
        /// Creates a new delivery
        /// </summary>
        /// <remarks>
        /// Aaron Usher
        /// Updated: 2017/04/07
        /// 
        /// Standardized method. Changed signature to take delivery object instead of individual parameters.
        /// </remarks>
        /// <remarks>
        /// Robert Forbes
        /// 
        /// Created:
        /// 2017/05/04
        /// 
        /// Removed random assigning of a route.
        /// </remarks>
        /// <param name="delivery">The delivery to create.</param>
        /// <returns>The delivery id of the newly created delivery</returns>
        public int CreateDeliveryAndRetrieveDeliveryId(Delivery delivery)
        {
            int result = 0;
            try
            {
                result = DeliveryAccessor.CreateDeliveryAndRetrieveDeliveryId(delivery);
                /* Removed because deliveries are acutally being assigned to routes when a route is created. And setting a random route doesn't make sense
                if (result != 0)
                {
                    bool success = AssignDeliveryToRoute(result);
                    if (!success)
                    {
                        throw new ApplicationException("Delivery could not be assigned to a route!");
                    }
                }
                */
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }

        /// <summary>
        /// Aaron Usher
        /// Created: 2017/05/04
        /// 
        /// Assigns a delivery to a random valid route.
        /// so a random one is picked.
        /// </summary>
        /// <param name="deliveryId">The id of the delivery to be updated.</param>
        /// <returns>Whether or not the delivery could be assigned.</returns>
        public bool AssignDeliveryToRoute(int deliveryId)
        {
            bool result = false;
            try
            {
                List<Route> routes = RouteAccessor.RetrieveRoutes();
                if (routes.Count == 0)
                {
                    throw new ApplicationException("Couldn't assign delivery to a route, as no routes exist!");
                }
                Random rand = new Random();
                int i = rand.Next(0, routes.Count);
                result = 1==DeliveryAccessor.AssignDeliveryToRoute(deliveryId, routes[i].RouteId.Value);
            }
            catch (Exception)
            {

                throw;
            }
            return result;
        }

        /// <summary>
        /// Aaron Usher
        /// Created: 2017/03/24
        /// 
        /// Updates a delivery.
        /// </summary>
        /// <param name="oldDelivery"></param>
        /// <param name="newDelivery"></param>
        /// <returns></returns>
        public bool UpdateDelivery(Delivery oldDelivery, Delivery newDelivery)
        {
            bool result = false;

            try
            {
                result = (1 == DeliveryAccessor.UpdateDelivery(oldDelivery, newDelivery));
            }
            catch (Exception)
            {

                throw;
            }

            return result;
        }


        /// <summary>
        /// Robert Forbes
        /// Created: 2017/04/19
        /// </summary>
        /// <param name="deliveryId"></param>
        /// <returns></returns>
        public Delivery RetrieveDeliveryById(int deliveryId)
        {
            Delivery delivery = null;

            try
            {
                delivery = DeliveryAccessor.RetrieveDeliveryById(deliveryId);
            }
            catch
            {
                throw;
            }

            return delivery;
        }


        /// <summary>
        /// Robert Forbes
        /// Creaded: 2017/04/23
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public List<Delivery> RetrieveDeliveriesByOrderId(int orderId)
        {
            List<Delivery> deliveries = new List<Delivery>();
            try
            {
                deliveries = DeliveryAccessor.RetrieveDeliveriesForOrder(orderId);
            }
            catch
            {
                throw;
            }

            return deliveries;
        }


        /// <summary>
        /// Robert Forbes
        /// 
        /// Created:
        /// 2017/05/04
        /// </summary>
        /// <param name="deliveryId"></param>
        /// <param name="routeId"></param>
        /// <returns></returns>
        public bool AssignRouteToDelivery(int deliveryId, int routeId)
        {
            var result = false;

            try
            {
                result = (DeliveryAccessor.AssignRouteToDelivery(deliveryId, routeId) == 1);
            }
            catch
            {
                throw;
            }

            return result;
        }
    }
}
