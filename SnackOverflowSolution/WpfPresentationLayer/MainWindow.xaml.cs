﻿using DataObjects;
using LogicLayer;
using System;
using System.Collections.Generic;
using System.Dynamic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfPresentationLayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ICustomerManager _customerManager = new CustomerManager();
        private EmployeeManager _employeeManager = new EmployeeManager();
        private IProductOrderManager _orderManager = new ProductOrderManager();
        private IVehicleManager _vehicleManager = new VehicleManager();
        List<ProductOrder> _currentOpenOrders;
        List<Employee> employeeList;
        List<Charity> charityList;
        private List<ProductLot> _productLotList;
        private List<CommercialCustomer> _commercialCustomers;
        private List<Vehicle> _vehicleList;
        private List<Supplier> _supplierList;
        private IUserManager _userManager = new UserManager();
        private ISupplierManager _supplierManager = new SupplierManager();
        private IProductLotManager _productLotManager = new ProductLotManager();
        private IProductManager _productManager = new ProductManager();
        private IDeliveryManager _deliveryManager;
        private IWarehouseManager _warehouseManager = new WarehouseManager();
        private IAgreementManager _agreementManager = new AgreementManager();
        private List<Delivery> _deliveries;
        private List<Warehouse> _warehouseList;
        private ProductLotSearchCriteria _productLotSearchCriteria;
        private ICharityManager _charityManager;


        Employee _employee = null;
        Supplier _supplier = null;
        CommercialCustomer _commercialCustomer = null;
        Charity _charity = null;
        User _user = null;

        private IPackageManager _packageManager = new PackageManager();
        List<Package> _packageList = null;
        private IOrderStatusManager _orderStatusManager = new OrderStatusManager();
        List<string> _orderStatusList = null;
        ISupplierInvoiceManager _supplierInvoiceManager = new SupplierInvoiceManager();
        List<SupplierInvoice> _supplierInvoiceList;

        public MainWindow()
        {
            InitializeComponent();
            _userManager = new UserManager();
            _charityManager = new CharityManager();
            _employeeManager = new EmployeeManager();
            _deliveryManager = new DeliveryManager();
            DisposeFiles();
            _productLotSearchCriteria = new ProductLotSearchCriteria() { Expired = false };
        }

        /// <summary>
        /// Eric Walton
        /// 2017/06/02
        /// 
        /// Button to load Create Commercial Customer Window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_Create_CommercialCustomer(object sender, RoutedEventArgs e)
        {
            _employee = _employeeManager.RetrieveEmployeeByUserName(_user.UserName);
            if (cboCustomerType.SelectedItem as String == "Commercial")
            {
                try
                {
                    frmCreateCommercialCustomer cCCW = new frmCreateCommercialCustomer((int)_employee.EmployeeId);
                    if (cCCW.ShowDialog() == true)
                    {
                        _commercialCustomers = _customerManager.RetrieveCommercialCustomers();
                        dgCustomer.ItemsSource = _commercialCustomers;
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Error: An employee must be logged in to create a commercial customer.");
                }
            }
            else if (cboCustomerType.SelectedItem as String == "Residential")
            {
                // If creating a residential customer is added to desktop code will go here to create one.
            }
        }

        /// <summary>
        /// Eric Walton
        /// 2017/03/03
        /// Invoked when the customer type combo box is initialized
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboCustomerTypeInitialized(object sender, EventArgs e)
        {
            cboCustomerType.Items.Add("Commercial");
            cboCustomerType.Items.Add("Residential");
            cboCustomerType.SelectedItem = "Commercial";
            refreshCustomerList();
        }

        /// <summary>
        /// Eric Walton
        /// 2017/03/03
        /// Invoked when the customer type combo box is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboCustomerTypeSelected(object sender, EventArgs e)
        {
            refreshCustomerList();
        }

        /// <summary>
        /// Eric Walton
        /// 2017/03/03
        /// Refreshes the customer list from the database
        /// </summary>
        private void refreshCustomerList()
        {
            if (cboCustomerType.SelectedItem as String == "Commercial")
            {
                try
                {
                    _commercialCustomers = _customerManager.RetrieveCommercialCustomers();
                    dgCustomer.ItemsSource = _commercialCustomers;
                }
                catch (Exception)
                {
                    ErrorAlert.ShowDatabaseError();
                }
            }
            else if (cboCustomerType.SelectedItem as String == "Residential")
            {
                dgCustomer.ItemsSource = null;
                // When functionality to retrieve list of residential customers the code will go here.
            }
        }
        /// <summary>
        /// Eric Walton
        /// 2017/05/03
        /// Invoked when the create order button is clicked on the customers tab.
        /// Loads the create order window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void createOrderClick(object sender, RoutedEventArgs e)
        {
            frmCreateOrder createOrderWindow = new frmCreateOrder((int)_employee.EmployeeId, (CommercialCustomer)dgCustomer.SelectedItem);
            if (createOrderWindow.ShowDialog() == true)
            {

            }
        }

        /// <summary>
        /// Eric Walton
        /// 2017/10/3
        /// Enables the create order button when a customer is selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CustomerSelected(object sender, SelectionChangedEventArgs e)
        {
            btnCreateOrder.IsEnabled = true;
        }

        /// <summary>
        /// Ariel Sigo
        /// Created 2017/10/02
        /// 
        /// Button that leads to update employee form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_Update_Employee(object sender, RoutedEventArgs e)
        {
            try
            {
                frmUpdateEmployee fUE = new frmUpdateEmployee(_employeeManager, employeeList[dgrdEmployee.SelectedIndex]);
                fUE.ShowDialog();
            }
            catch
            {
                MessageBox.Show("Please Select an Employee to Edit.");
            }
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            hideTabs();
            tabSetMain.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Bobby Thorne
        /// 2/11/17
        /// 
        /// When this button is pushed it first checks to see if there is a user logged in
        /// If there is not it will use the username and password text field and check if
        /// it matches with any user if so it then recieves the user's info
        /// 
        /// Needs work on returning employee info so tabs can be 
        /// filtered and not just show all
        /// 
        /// UPDATE
        /// Bobby Thorne
        /// 3/24/2017
        /// 
        /// Reset buttons and nulled supplier, charity, and customer variables
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (_user == null)
            {

                try
                {
                    if (_userManager.AuthenticateUser(tfUsername.Text, tfPassword.Password))
                    {

                        lblPassword.Visibility = Visibility.Collapsed;
                        lblUsername.Visibility = Visibility.Collapsed;
                        tfUsername.Visibility = Visibility.Collapsed;
                        tfPassword.Visibility = Visibility.Collapsed;
                        mnuRequestUsername.Visibility = Visibility.Collapsed;
                        tfPassword.Password = "";
                        btnLogin.Content = "Logout";
                        btnLogin.IsDefault = false;
                        tfPassword.Background = Brushes.White;
                        try
                        {
                            _user = _userManager.userInstance;

                            if ("ADMIN" == _user.UserName)
                            {
                                btnResetPassword.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                btnResetPassword.Visibility = Visibility.Collapsed;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Failed to find user.");
                            btnLogin_Click(sender, e);
                        }
                        try
                        {
                            _employee = _employeeManager.RetrieveEmployeeByUserName(_user.UserName);
                        }
                        catch (Exception ex)
                        {
                            // Enters here if user that access this is not an employee.
                            // For now it does nothing. 
                            MessageBox.Show("Employee table is empty or DB connection error.");
                        }
                        statusMessage.Content = "Welcome " + _user.UserName;
                        showTabs(); // This needs to be updated so it will show just one that is 
                        // assigned to the employe
                        mnuChangePassword.Visibility = Visibility.Visible;

                    }
                    else
                    {
                        statusMessage.Content = "Username and Password did not match.";
                        tfPassword.Password = "";
                        tfPassword.Background = Brushes.Red;
                    }
                }
                catch (System.Data.SqlClient.SqlException ex)
                {
                    ErrorAlert.ShowDatabaseError();
                }
            }
            else
            {
                _user = null;
                btnLogin.Content = "Login";
                btnLogin.IsDefault = true;
                _supplier = null;
                _charity = null;
                _commercialCustomer = null;
                btnSupplierApplicationStatusCheck.Content = "Check Supplier Status";
                btnCharityApplicationStatusCheck.Content = "Check Charity Status";
                btnCommercialCustomerApplicationStatusCheck.Content = "Check Commerical Status";
                btnSupplierApplicationStatusCheck.IsEnabled = true;
                btnCharityApplicationStatusCheck.IsEnabled = true;
                btnCommercialCustomerApplicationStatusCheck.IsEnabled = true;
                statusMessage.Content = "Please Log in to continue...";
                hideTabs();
                lblPassword.Visibility = Visibility.Visible;
                lblUsername.Visibility = Visibility.Visible;
                tfUsername.Visibility = Visibility.Visible;
                tfPassword.Visibility = Visibility.Visible;
                mnuRequestUsername.Visibility = Visibility.Visible;
                mnuChangePassword.Visibility = Visibility.Collapsed;
                btnResetPassword.Visibility = Visibility.Collapsed;
            }

        }
        private void showTabs()
        {
            tabSetMain.Visibility = Visibility.Visible;
            tabCommercialCustomer.Visibility = Visibility.Visible;
            tabEmployee.Visibility = Visibility.Visible;
            tabUser.Visibility = Visibility.Visible;

        }

        private void hideTabs()
        {
            tabSetMain.Visibility = Visibility.Hidden;
            tabCommercialCustomer.Visibility = Visibility.Collapsed;
            tabEmployee.Visibility = Visibility.Collapsed;
            tabUser.Visibility = Visibility.Collapsed;
        }

        private void btnCreateNewUser_Click(object sender, RoutedEventArgs e)
        {
            CreateNewUser fCU = new CreateNewUser();
            fCU.ShowDialog();
        }

        private void tabCharity_Selected(object sender, RoutedEventArgs e)
        {
            try
            {
                charityList = _charityManager.RetrieveCharityList();
                dgrdCharity.DataContext = charityList;
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                ErrorAlert.ShowDatabaseError();
            }
        }

        private void btnViewCharity_Click(object sender, RoutedEventArgs e)
        {
            if (dgrdCharity.SelectedIndex >= 0)
            {
                var CharityViewInstance = new CharityView(_charityManager, charityList[dgrdCharity.SelectedIndex]);
                CharityViewInstance.ShowDialog();
                tabCharity_Selected(sender, e);
            }

        }

        private void btnAddCharity_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<Employee> employeeSearchList = _employeeManager.SearchEmployees(new Employee() { UserId = _user.UserId });
                int employeeId;
                if (employeeSearchList.Count > 0)
                {
                    employeeId = (int)employeeSearchList[0].EmployeeId;
                    var CharityViewInstance = new CharityView(_charityManager, employeeId);
                    CharityViewInstance.SetEditable();
                    CharityViewInstance.ShowDialog();
                    tabCharity_Selected(sender, e);
                }
                else
                {
                    var applyForCharityFrm = new CharityView(_user, _charityManager);
                    var result = applyForCharityFrm.ShowDialog();
                    if (result == true)
                    {
                        MessageBox.Show("Application submitted");
                        tabCharity_Selected(sender, e);
                    }

                }
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                ErrorAlert.ShowDatabaseError();
            }
        }

        private void tabEmployee_Selected(object sender, RoutedEventArgs e)
        {
            try
            {
                employeeList = _employeeManager.RetrieveEmployeeList();
                dgrdEmployee.DataContext = employeeList;
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                ErrorAlert.ShowDatabaseError();
            }
        }

        private void btnAddEmployee_Click(object sender, RoutedEventArgs e)
        {
            var EmployeeViewInstance = new EmployeeView(_employeeManager);
            EmployeeViewInstance.SetEditable();
            EmployeeViewInstance.ShowDialog();
            tabEmployee_Selected(sender, e);
        }

        private void tabOrder_Selected(object sender, RoutedEventArgs e)
        {

        }

        private void btnViewEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (dgrdEmployee.SelectedIndex >= 0)
            {
                var EmployeeViewInstance = new EmployeeView(_employeeManager, employeeList[dgrdEmployee.SelectedIndex]);
                EmployeeViewInstance.ShowDialog();
                tabEmployee_Selected(sender, e);
            }
        }

        private void tabProductLot_Selected(object sender, RoutedEventArgs e)
        {
            try
            {
                List<ProductLot> productLotList;
                if (_productLotSearchCriteria.Expired)
                {
                    productLotList = _productLotManager.RetrieveExpiredProductLots();
                }
                else
                {
                    productLotList = _productLotManager.RetrieveProductLots();
                }
                dgProductLots.ItemsSource = productLotList;
            }
            catch (Exception ex)
            {

                MessageBox.Show("There was an error: " + ex.Message);
            }
        }

        private void AddProductLot_Click(object sender, RoutedEventArgs e)
        {
            var productLotView = new frmAddProductLot();
            productLotView.SetEditable();
            var addResult = productLotView.ShowDialog();
            if (addResult == true)
            {
                try
                {
                    var addInspectionFrm = new frmAddInspection(_productLotManager.RetrieveNewestProductLotBySupplier(_supplierManager.RetrieveSupplierBySupplierID(productLotView.supplierId)),
                        new GradeManager(), _employee, new ProductManager(), _supplierManager, new InspectionManager());
                    var addInspectionResult = addInspectionFrm.ShowDialog();
                    if (addInspectionResult == true)
                    {
                        MessageBox.Show("Inspection Added");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            tabProductLot_Selected(sender, e);
        }

        /// <summary>
        /// Christian Lopez
        /// Created on 2017/01/31
        /// 
        /// Open a frmAddSupplier
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>Last modified by Christian Lopez on 2017/02/02</remarks>
        private void btnCreateSupplier_Click(object sender, RoutedEventArgs e)
        {
            var addSupplierFrm = new frmAddSupplier(_user, _userManager, _supplierManager, _productManager, _agreementManager);
            var addSupplierResult = addSupplierFrm.ShowDialog();
            if (addSupplierResult == true)
            {
                MessageBox.Show("Supplier added!");
                tabSupplier_Selected(sender, e);
            }
        }


        /// <summary>
        /// Christian Lopez
        /// Created on 2017/02/15
        /// 
        /// Open a frmAddInspection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCreateInspection_Click(object sender, RoutedEventArgs e)
        {
            if (dgProductLots.SelectedIndex != -1)
            {
                try
                {
                    // Will need to redo method call when linked with either datagrid of ProductLots or immediately aftermaking a productLot
                    //_productLotManager.RetrieveNewestProductLotBySupplier(_supplierManager.RetrieveSupplierByUserId(_user.UserId))
                    var addInspectionFrm = new frmAddInspection((ProductLot)dgProductLots.SelectedItem,
                        new GradeManager(), _employee, new ProductManager(), _supplierManager, new InspectionManager());
                    var addInspectionResult = addInspectionFrm.ShowDialog();
                    if (addInspectionResult == true)
                    {
                        MessageBox.Show("Inspection Added");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void tfPassword_KeyDown(object sender, KeyEventArgs e)
        {
            tfPassword.Background = Brushes.White;
        }

        /// <summary>
        /// Laura Simmonds
        /// 2017/27/02
        /// Button links tab to View Product window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void viewProductBtn(object sender, RoutedEventArgs e)
        {
            frmViewProduct viewProduct = new frmViewProduct();
            viewProduct.ShowDialog();

        }

        /// <summary>
        ///     Mason Allen
        ///     ListView that displays current open orders.  Double clicking an item will display the order details.
        /// </summary>
        /// <remarks>
        /// Robert Forbes
        /// 2017/03/01
        /// 
        /// Modified to work with drop down to select status
        /// </remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabOpenOrders_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _orderStatusList = _orderStatusManager.RetrieveAllOrderStatus();
                cboOrderStatus.ItemsSource = _orderStatusList;
                cboOrderStatus.SelectedIndex = 0;
                if (cboOrderStatus.SelectedItem != null)
                {

                    _currentOpenOrders = _orderManager.RetrieveProductOrdersByStatus((string)cboOrderStatus.SelectedItem);
                    lvOpenOrders.Items.Clear();

                    for (int i = 0; i < _currentOpenOrders.Count; i++)
                    {
                        this.lvOpenOrders.Items.Add(_currentOpenOrders[i]);
                    }
                    lblStatus.Content = "Status: Success";
                }
            }
            catch (Exception ex)
            {
                lblStatus.Content += ex.ToString();
            }
        }



        private void lvOpenOrders_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                ProductOrder selectedItem = (ProductOrder)lvOpenOrders.SelectedItem;
                int index = selectedItem.OrderId;

                var detailsWindow = new frmProductOrderDetails(index);
                detailsWindow.Show();
            }
            catch (Exception)
            {
                lblStatus.Content = "Nothing selected";
            }

        }

        /// <summary>
        /// Robert Forbes
        /// 2017/03/01
        /// 
        /// Updates the list of orders to match the new combo box selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboOrderStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cboOrderStatus.SelectedItem != null)
                {
                    _currentOpenOrders = _orderManager.RetrieveProductOrdersByStatus((string)cboOrderStatus.SelectedItem);
                    lvOpenOrders.Items.Clear();

                    for (int i = 0; i < _currentOpenOrders.Count; i++)
                    {
                        this.lvOpenOrders.Items.Add(_currentOpenOrders[i]);
                    }
                    lblStatus.Content = "Status: Success";
                }
            }
            catch (Exception ex)
            {
                lblStatus.Content += ex.ToString();
            }
        }


        private void BtnAddProduct_OnClick(object sender, RoutedEventArgs e)
        {
            var frmAddProduct = new frmAddProduct(_user, _productManager);
            frmAddProduct.ShowDialog();
        }

        /// <summary>
        /// Created by Natacha Ilunga
        /// Created on 02-28-2017
        /// 
        /// Navigate to Browse Products Page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBrowseProduct_Click(object sender, RoutedEventArgs e)
        {
            var frmBrowseProducts = new frmBrowseProducts(_user, _productManager);
            frmBrowseProducts.Show();
        }


        /// <summary>
        /// Created by Michael Takrama, Natacha Ilunga
        /// Creatd on 02-28-2017
        /// 
        /// Method to cleanup cached files
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void DisposeFiles()
        {
            try
            {
                this.DisposeImages();
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }

        }




        /// <summary>
        /// Robert Forbes
        /// 2017/03/09
        /// 
        /// Tab that shows all packages in the database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabPackages_Selected(object sender, RoutedEventArgs e)
        {
            RefreshPackageList();
            dgPackages.ItemsSource = _packageList;
            dgPackages.Items.Refresh();
        }

        /// <summary>
        /// Robert Forbes
        /// 2017/03/01
        /// 
        /// Updates the locally stored list of packages
        /// </summary>
        private void RefreshPackageList()
        {
            try
            {
                _packageList = _packageManager.RetrieveAllPackages();
            }
            catch
            {
                MessageBox.Show("Unable to retrieve packages from database");
            }
        }

        /// <summary>
        /// Mason Allen
        /// 03/01/2017
        /// Opens new window to create a new vehicle record
        /// </summary>
        private void btnAddNewVehicle_Click(object sender, RoutedEventArgs e)
        {
            var addNewVehicleWindow = new frmAddEditVehicle();
            addNewVehicleWindow.Show();
        }

        private void BtnManageStock_OnClick(object sender, RoutedEventArgs e)
        {
            frmManageStock fms = new frmManageStock();
            fms.ShowDialog();
        }

        /// <summary>
        /// Eric Walton 
        /// 2017/02/03
        /// Triggers when vehicle management tab is selected
        /// Will populate a list of vehicles once complete
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabVehicle_Selected(object sender, RoutedEventArgs e)
        {
            try
            {
                _vehicleList = _vehicleManager.RetrieveAllVehicles();
                dgVehicle.ItemsSource = _vehicleList;
            }
            catch (Exception)
            {
                ErrorAlert.ShowDatabaseError();
            }

        }

        /// <summary>
        /// Christian Lopez
        /// Created 2017/03/02
        /// 
        /// Uses frmAddSupplier, but this calling does not automatically set the approved status that the other one does.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnApplyForSupplierAct_Click(object sender, RoutedEventArgs e)
        {
            var addSupplierFrm = new frmAddSupplier(_user, _userManager, _supplierManager, _productManager, _agreementManager, "Applying");
            var addSupplierResult = addSupplierFrm.ShowDialog();
            if (addSupplierResult == true)
            {
                MessageBox.Show("Application Submitted!");
                tabSupplier_Selected(sender, e);
            }
        }

        /// <summary>
        /// Opens a window to allow a user to change one's password
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangePassword(object sender, RoutedEventArgs e)
        {
            if (null != _user)
            {
                var updateScreen = new PasswordChangeView(_user.UserName);
                updateScreen.Show();
            }
        }

        private void editVehicleClick(object sender, RoutedEventArgs e)
        {
            if (dgVehicle.SelectedItem != null)
            {
                var addNewVehicleWindow = new frmAddEditVehicle((Vehicle)dgVehicle.SelectedItem);
                if (addNewVehicleWindow.ShowDialog() == true)
                {
                    try
                    {
                        _vehicleList = _vehicleManager.RetrieveAllVehicles();
                        dgVehicle.ItemsSource = _vehicleList;
                    }
                    catch (Exception)
                    {
                        ErrorAlert.ShowDatabaseError();
                    }
                }
            }
        }


        private void tabUser_Selected(object sender, RoutedEventArgs e)
        {
            if ("ADMIN" == _user.UserName)
            {
                btnResetPassword.Visibility = Visibility.Visible;
            }
            else
            {
                btnResetPassword.Visibility = Visibility.Collapsed;
            }
        }

        private void btnResetPassword_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<User> userList = _userManager.RetrieveFullUserList();
                var passwordResetWindow = new ResetPassword(_userManager, userList);
                passwordResetWindow.Show();
            }
            catch
            {
                ErrorAlert.ShowDatabaseError();
            }
        }



        private void tabSupplier_Selected(object sender, RoutedEventArgs e)
        {
            try
            {
                _supplierList = _supplierManager.ListSuppliers();
                dgSuppliers.ItemsSource = _supplierList;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        /// <summary>
        /// Christian Lopez
        /// Created 2017/03/03
        /// 
        /// Handles logic of what happens when the warehouse tab is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabWarehouse_Selected(object sender, RoutedEventArgs e)
        {
            try
            {
                _warehouseList = _warehouseManager.ListWarehouses();
                dgWarehouses.ItemsSource = _warehouseList;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        /// <summary>
        /// Ryan Spurgetis
        /// 03/02/2017
        /// 
        /// Loads the create a new product category window
        /// </summary>
        private void btnAddCategory_Click(object sender, RoutedEventArgs e)
        {
            frmProductCategory prodCategoryWindow = new frmProductCategory();
            prodCategoryWindow.Show();
        }

        private void RequestUsername_Click(object sender, RoutedEventArgs e)
        {
            if (_user == null)
            {
                frmRequestUsername requestUsername = new frmRequestUsername();
                requestUsername.ShowDialog();
            }
            else { MessageBox.Show("Must not be signed in to use this feature"); }
        }

        private void btnApproveDeny_Click(object sender, RoutedEventArgs e)
        {
            if (dgrdCharity.SelectedIndex >= 0)
            {
                var frmCharityApproval = new frmCharityApproval(_charityManager, charityList[dgrdCharity.SelectedIndex]);
                frmCharityApproval.ShowDialog();
                tabCharity_Selected(sender, e);
            }
        }

        /// <summary>
        /// Christian Lopez
        /// Created 2017/03/09
        /// 
        /// Launch form to edit existing supplier
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEditSupplier_Click(object sender, RoutedEventArgs e)
        {
            if (0 > dgSuppliers.SelectedIndex)
            {
                MessageBox.Show("Select a supplier to edit.");
            }
            else
            {
                var frmEditSupplier = new frmAddSupplier(_user, _userManager, _supplierManager, _productManager,
                    _agreementManager, "Editing", (Supplier)dgSuppliers.SelectedItem);
                var result = frmEditSupplier.ShowDialog();
                if (result == true)
                {
                    MessageBox.Show("Supplier Edited.");
                    tabSupplier_Selected(sender, e);
                }
            }
        }

        /// <summary>
        /// William Flood
        /// Created 2017/03/09
        /// 
        /// Retrieve criteria to filter product lots
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FilterProductLots_Click(object sender, RoutedEventArgs e)
        {
            var filterWindow = new ProductLotSearchView(_productLotSearchCriteria);
            filterWindow.ShowDialog();
            try
            {
                List<ProductLot> productLotList;
                if (_productLotSearchCriteria.Expired)
                {
                    productLotList = _productLotManager.RetrieveExpiredProductLots();
                }
                else
                {
                    productLotList = _productLotManager.RetrieveProductLots();
                }
                dgProductLots.ItemsSource = productLotList;
            }
            catch
            {
                ErrorAlert.ShowDatabaseError();
            }
        }
        /// <summary>
        /// Robert Forbes
        /// 2017/03/09
        /// 
        /// Button click event to open a delivery management window for the selected order
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCreateDeliveries_Click(object sender, RoutedEventArgs e)
        {
            if (lvOpenOrders.SelectedItem != null)
            {
                if (((ProductOrder)lvOpenOrders.SelectedItem).OrderStatusId.Equals("Ready For Shipment"))
                {
                    frmCreateDeliveryForOrder deliveryWindow = new frmCreateDeliveryForOrder(((ProductOrder)lvOpenOrders.SelectedItem).OrderId);
                    deliveryWindow.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Please select a delivery that is ready for shipment");
                }

            }
            else
            {
                MessageBox.Show("Please select a delivery that is ready for shipment");
            }
        }

        /// Created by Natacha Ilunga
        /// 03/09/2017
        /// 
        /// Supplier Tab Select Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabSupplierCatlog_Selected(object sender, RoutedEventArgs e)
        {
            //Load Supplier Data
            try
            {
                var suppliersData = _supplierManager.ListSuppliers();
                DgSupplierCatalogue.ItemsSource = parseIntoSupplierCatalogue(suppliersData);
            }
            catch (Exception ex)
            {

                MessageBox.Show("Problem Loading Supplier Catalogue Data " + ex);
            }

        }


        /// <summary>
        /// Created by Natacha Ilunga
        /// 03/09/2017
        /// 
        /// Parase Supplier Object into SupplierCatalogue view model.
        /// </summary>
        /// <param name="suppliersList"></param>
        /// <returns></returns>
        private List<SupplierCatalogueViewModel> parseIntoSupplierCatalogue(List<Supplier> suppliersList)
        {
            List<SupplierCatalogueViewModel> viewModelList = new List<SupplierCatalogueViewModel>();
            SupplierCatalogueViewModel viewModel;

            foreach (var k in suppliersList)
            {
                viewModel = new SupplierCatalogueViewModel()
                {
                    SupplierID = k.SupplierID,
                    FarmName = k.FarmName,
                    FarmAddress = k.FarmAddress,
                    FarmCity = k.FarmCity,
                    FarmState = k.FarmState,
                    FarmTaxID = k.FarmTaxID,
                    IsApproved = k.IsApproved,
                    UserId = k.UserId,
                    UserData = _userManager.RetrieveUser(k.UserId)
                };
                viewModelList.Add(viewModel);
            }





            return viewModelList;
        }

        private void btnAddMaintenance_Click(object sender, RoutedEventArgs e)
        {
            Vehicle selectedVehicle;
            if (dgVehicle.SelectedItem != null)
            {
                selectedVehicle = (Vehicle)dgVehicle.SelectedItem;
                frmAddMaintenanceRecord addMaint = new frmAddMaintenanceRecord(selectedVehicle.VehicleID);
                addMaint.Show();
            }

        }

        //private void btnCheckSupplierStatus_Click(object sender, RoutedEventArgs e)
        //{
        //    if (_user != null)
        //    {

        //    }
        //}

        /// <summary>
        /// Bobby Thorne
        /// 3/10/2017
        /// 
        /// When clicked it will bring up Applications that they have submitted
        /// that have been approved and also pending
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCheckApplicationStatus_Click(object sender, RoutedEventArgs e)
        {
            bool isSupplierApproved = false;
            bool isCommercialCustomerApproved = false;
            bool isCharityApproved = false;
            if (_user != null)
            {
                //btnCheckStatusDone.Visibility = Visibility.Visible;
                //btnCancelApplication.Visibility = Visibility.Visible;
                //dgMyAccount.Visibility = Visibility.Visible;

                try
                {
                    _supplier = _supplierManager.RetrieveSupplierByUserId(_user.UserId);
                    isSupplierApproved = _supplier.IsApproved;
                }
                catch
                {
                    //frmCheckSupplierStatus checkSupplierStatus = new frmCheckSupplierStatus(_user,_userManager,_supplierManager,_productManager,_agreementManager);
                    //checkSupplierStatus.Show();
                    //throw;
                }
                try
                {
                    _commercialCustomer = _customerManager.RetrieveCommercialCustomerByUserId(_user.UserId);
                    isCommercialCustomerApproved = _commercialCustomer.IsApproved;
                }
                catch
                {
                    //throw;
                }


                if (_supplier == null && _commercialCustomer == null)
                {
                    frmCheckSupplierStatus checkSupplierStatus = new frmCheckSupplierStatus(_user, _userManager, _supplierManager, _productManager, _agreementManager);
                    checkSupplierStatus.Show();
                }
            }
        }

        /// <summary>
        /// Bobby Thorne
        /// 3/10/20174
        /// 
        /// Button that reverts back to the original status of the tab
        ///
        /// Removed
        /// Bobby Thorne 
        /// 3/24/2017
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCheckApplicationStatusDone_Click(object sender, RoutedEventArgs e)
        {
            //btnCheckStatus.Visibility = Visibility.Visible;
            //btnCancelApplication.Visibility = Visibility.Collapsed;
            //btnCheckStatusDone.Visibility = Visibility.Collapsed;

        }

        /// <summary>
        /// Bobby Thorne
        /// 3/10/2017
        /// Removed
        /// 3/10/2017
        /// 
        /// This will cancel applications that have been submitted
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancelApplication_Click(object sender, RoutedEventArgs e)
        {
            //btnCheckStatus.Visibility = Visibility.Visible;
            //btnCancelApplication.Visibility = Visibility.Collapsed;
            //btnCheckStatusDone.Visibility = Visibility.Collapsed;
        }



        private void dgVehicle_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgVehicle.SelectedItem != null)
            {
                Vehicle vehicle = (Vehicle)dgVehicle.SelectedItem;
                frmViewVehicle vehicleWindow = new frmViewVehicle(vehicle.VehicleID);
                vehicleWindow.ShowDialog();
            }
        }

        /// <summary>
        /// Christian Lopez
        /// 2017/03/22
        /// 
        /// Handles what happens when the supplier invoice tab is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabSupplierInvoice_Selected(object sender, RoutedEventArgs e)
        {
            try
            {
                _supplierInvoiceList = _supplierInvoiceManager.RetrieveAllSupplierInvoices();
                dgSupplierInvoices.ItemsSource = _supplierInvoiceList;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        /// <summary>
        /// Christian Lopez
        /// 2017/03/22
        /// 
        /// Handles what happens if the datagrid is double clicked. Launches the detail window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>Last modified by Christian Lopez 2017/03/23</remarks>
        private void dgSupplierInvoices_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!(dgSupplierInvoices.SelectedIndex < 0))
            {
                var supplierInvoiceDetail = new frmSupplierInvoiceDetails((SupplierInvoice)dgSupplierInvoices.SelectedItem, _supplierInvoiceManager, _supplierManager);
                var result = supplierInvoiceDetail.ShowDialog();
                if (result == true)
                {
                    tabSupplierInvoice_Selected(sender, e);
                }
            }
            else
            {
                MessageBox.Show("Please select an invoice to view.");
            }

        }

        /// <summary>
        /// Christian Lopez
        /// 2017/03/23
        /// 
        /// Logic to approve the selected invoice
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnApproveInvoice_Click(object sender, RoutedEventArgs e)
        {
            if (!(dgSupplierInvoices.SelectedIndex < 0))
            {
                try
                {
                    if (_supplierInvoiceManager.ApproveSupplierInvoice(((SupplierInvoice)dgSupplierInvoices.SelectedItem).SupplierInvoiceId))
                    {
                        tabSupplierInvoice_Selected(sender, e);
                    }
                    else
                    {
                        MessageBox.Show("Unable to approve the invoice.");
                    }
                }
                catch (Exception ex)
                {

                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Bobby Thorne
        /// 3/24/2017
        /// 
        /// Checks user's Commercial Customer application status 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCommercialCustomerApplicationStatusCheck_Click(object sender, RoutedEventArgs e)
        {
            if (_user != null)
            {

                try
                {
                    _commercialCustomer = _customerManager.RetrieveCommercialCustomerByUserId(_user.UserId);

                    if (_commercialCustomer.IsApproved && _commercialCustomer.Active)
                    {
                        btnCommercialCustomerApplicationStatusCheck.Content = btnCommercialCustomerApplicationStatusCheck.Content + "\nAPPROVED";
                        btnCommercialCustomerApplicationStatusCheck.IsEnabled = false;
                        //btnCommercialCustomerApplicationStatusCheck.Background = Brushes.Green;
                    }
                    else if (!_commercialCustomer.IsApproved && _commercialCustomer.Active)
                    {
                        btnCommercialCustomerApplicationStatusCheck.Content = btnCommercialCustomerApplicationStatusCheck.Content + "\nPENDING";
                        btnCommercialCustomerApplicationStatusCheck.IsEnabled = false;
                    }
                    else if (!_commercialCustomer.IsApproved && _commercialCustomer.Active)
                    {
                        btnCommercialCustomerApplicationStatusCheck.Content = btnCommercialCustomerApplicationStatusCheck.Content + "\nDENIED";
                        btnCommercialCustomerApplicationStatusCheck.IsEnabled = false;
                    }
                }
                catch
                {
                    frmApplicationAskUser askAboutApplication = new frmApplicationAskUser(_user, _userManager, _customerManager);
                    askAboutApplication.Show();
                }
            }

        }

        /// <summary>
        /// Bobby Thorne
        /// 3/24/2017
        /// 
        /// Checks user's supplier application status 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSupplierApplicationStatusCheck_Click(object sender, RoutedEventArgs e)
        {
            if (_user != null)
            {

                try
                {
                    _supplier = _supplierManager.RetrieveSupplierByUserId(_user.UserId);

                    if (_supplier.IsApproved && _supplier.Active)
                    {
                        btnSupplierApplicationStatusCheck.Content = btnSupplierApplicationStatusCheck.Content + "\nAPPROVED";
                        btnSupplierApplicationStatusCheck.IsEnabled = false;
                        //btnSupplierApplicationStatusCheck.Background = Brushes.Green;
                    }
                    else if (!_supplier.IsApproved && _supplier.Active)
                    {
                        btnSupplierApplicationStatusCheck.Content = btnSupplierApplicationStatusCheck.Content + "\nPENDING";

                        btnSupplierApplicationStatusCheck.IsEnabled = false;
                    }
                    else if (!_supplier.IsApproved && !_supplier.Active)
                    {
                        btnSupplierApplicationStatusCheck.Content = btnSupplierApplicationStatusCheck.Content + "\nDENIED";
                        btnSupplierApplicationStatusCheck.IsEnabled = false;
                    }
                }
                catch
                {
                    frmApplicationAskUser askAboutApplication = new frmApplicationAskUser(_user, _userManager, _supplierManager, _productManager, _agreementManager);
                    askAboutApplication.Show();
                }
            }
        }

        /// <summary>
        /// Bobby Thorne
        /// 3/24/2017
        /// 
        /// checks user's charity application status 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCharityApplicationStatusCheck_Click(object sender, RoutedEventArgs e)
        {
            if (_user != null)
            {
                try
                {
                    _charity = _charityManager.RetrieveCharityByUserId(_user.UserId);

                    if (_charity.Status == "Approved")
                    {
                        btnCharityApplicationStatusCheck.Content = btnCharityApplicationStatusCheck.Content + "\nAPPROVED";
                        btnCharityApplicationStatusCheck.IsEnabled = false;
                        //btnCommercialCustomerApplicationStatusCheck.Background = Brushes.Green;
                    }
                    else if (_charity.Status == "Pending")
                    {
                        btnCharityApplicationStatusCheck.Content = btnCharityApplicationStatusCheck.Content + "\nPENDING";
                        btnCharityApplicationStatusCheck.IsEnabled = false;
                    }
                    else if (_charity.Status == "Denied")
                    {
                        btnCharityApplicationStatusCheck.Content = btnCharityApplicationStatusCheck.Content + "\nDENIED";
                        btnCharityApplicationStatusCheck.IsEnabled = false;
                    }
                }
                catch
                {
                    frmApplicationAskUser askAboutApplication = new frmApplicationAskUser(_user, _userManager, _charityManager);
                    askAboutApplication.Show();
                }
            }
        }

        /// <summary>
        /// Ethan Jorgensen
        /// 2017/03/20
        /// 
        /// Allows splitting a product lot into two smaller lots
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSplitLot_Click(object sender, RoutedEventArgs e)
        {
            if (!(dgProductLots.SelectedIndex < 0))
            {
                try
                {
                    // Call showDialog so that we know we have a result to use after this

                    var selectedItem = (ProductLot)dgProductLots.SelectedItem;
                    var frm = new frmSplitProductLot(selectedItem);
                    var result = frm.ShowDialog();

                    if (result ?? false)
                    {
                        _productLotManager.AddProductLot(new ProductLot()
                        {
                            AvailableQuantity = frm.OldQty - (selectedItem.Quantity - selectedItem.AvailableQuantity),
                            DateReceived = selectedItem.DateReceived,
                            ExpirationDate = selectedItem.ExpirationDate,
                            LocationId = selectedItem.LocationId,
                            ProductId = selectedItem.ProductId,
                            ProductName = selectedItem.ProductName,
                            Quantity = frm.OldQty,
                            SupplierId = selectedItem.SupplierId,
                            SupplyManagerId = selectedItem.SupplyManagerId,
                            WarehouseId = selectedItem.WarehouseId
                        });
                        _productLotManager.AddProductLot(new ProductLot()
                        {
                            AvailableQuantity = frm.NewQty,
                            DateReceived = selectedItem.DateReceived,
                            ExpirationDate = selectedItem.ExpirationDate,
                            LocationId = selectedItem.LocationId,
                            ProductId = selectedItem.ProductId,
                            ProductName = selectedItem.ProductName,
                            Quantity = frm.NewQty,
                            SupplierId = selectedItem.SupplierId,
                            SupplyManagerId = selectedItem.SupplyManagerId,
                            WarehouseId = selectedItem.WarehouseId
                        });
                        _productLotManager.DeleteProductLot(selectedItem);
                        tabProductLot_Selected(sender, e);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }

        }

        /// <summary>
        /// Robert Forbes
        /// 2017/03/24
        /// 
        /// Opens a window to view maintenance records
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnViewMaintenance_Click(object sender, RoutedEventArgs e)
        {
            if((Vehicle)dgVehicle.SelectedItem != null){
                frmViewMaintenanceRecords viewMaintenanceRecordsWindow = new frmViewMaintenanceRecords(((Vehicle)dgVehicle.SelectedItem).RepairList);
                viewMaintenanceRecordsWindow.ShowDialog();
            }
        }

        /// <summary>
        /// Ryan Spurgetis
        /// 2017/3/23
        /// 
        /// Brings up the Product Lot detail window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgProductLots_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var productLot = (ProductLot)dgProductLots.SelectedItem;
            var productLotMgr = new ProductLotManager();
            var productLotDetail = new frmAddProductLot(productLotMgr, productLot);
            productLotDetail.ShowDialog();
        }
        private void mnuQuit_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to quit?", "Confirmation:", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }


        private void tabDeliveries_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _deliveries = _deliveryManager.RetrieveDeliveries();
                var deliveriesWithVehicleId = new List<ExpandoObject>();
                foreach (var item in _deliveries)
                {
                    dynamic newItem = new ExpandoObject();
                    newItem.DeliveryDate = item.DeliveryDate;
                    newItem.StatusId = item.StatusId;
                    newItem.DeliveryTypeId = item.DeliveryTypeId;
                    newItem.VehicleId = _deliveryManager.RetrieveVehicleByDelivery(item.DeliveryId.Value).VehicleID;
                    deliveriesWithVehicleId.Add(newItem);
                }
                lvDeliveries.Items.Clear();

                for (int i = 0; i < _deliveries.Count; i++)
                {
                    lvDeliveries.Items.Add(deliveriesWithVehicleId[i]);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }

        private void lvDeliveries_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Delivery delivery;
            if (lvDeliveries.SelectedItem != null)
            {
                delivery = _deliveries[lvDeliveries.SelectedIndex];
            }
            else
            {
                MessageBox.Show("Please select a delivery to edit.");
                return;
            }
            var editForm = new frmAddEditDelivery(delivery, _deliveryManager, true);
            var result = editForm.ShowDialog();
            _deliveries = _deliveryManager.RetrieveDeliveries();
            lvDeliveries.Items.Refresh();
        }
    } // end of class
} // end of namespace 
