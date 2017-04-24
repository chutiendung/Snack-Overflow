﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DataObjects;
using LogicLayer;

namespace WpfPresentationLayer
{
    /// <summary>
    /// Interaction logic for frmAddMaintenanceRecord.xaml
    /// </summary>
    public partial class frmAddMaintenanceRecord : Window
    {
        MaintenanceScheduleManager _myMaintenanceScheduleManager = new MaintenanceScheduleManager();
        MaintenanceScheduleLineManager _myMaintenanceScheduleLineManager = new MaintenanceScheduleLineManager();
        int vehicleId;

        /// <summary>
        /// Alissa Duffy
        /// Updated: 2017/04/24
        /// 
        /// Initialize the Add Maintenance Record  form.
        /// Standaridized method.
        /// </summary>
        /// <param name="vehicleId"></param>
        public frmAddMaintenanceRecord(int vehicleId)
        {
            InitializeComponent();
            this.vehicleId = vehicleId;
        }

        /// <summary>
        /// Alissa Duffy
        /// Updated: 2017/04/24
        /// 
        /// Cancels and Closes the Add Maintenance Record form.
        /// Standaridized method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Alissa Duffy
        /// Updated: 2017/04/24
        /// 
        /// CSaves Changes made to the Add Maintenance Record form.
        /// Standaridized method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MaintenanceSchedule currentMaintenanceSchedule = _myMaintenanceScheduleManager.retrieveMaintenanceScheduleByVehicleId(vehicleId);
                _myMaintenanceScheduleLineManager.createMaintenanceScheduleLine(new MaintenanceScheduleLine
                {
                    MaintenanceScheduleId = currentMaintenanceSchedule.MaintenanceScheduleId,
                    Description = txtMaintenanceDescription.Text,
                    MaintenanceDate = DateTime.Today
                });
                MessageBox.Show("Maintenance record saved successfully.");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was an error saving the maintenance record." + ex);
            }
        }
    } // End of class
} // End of namespace
