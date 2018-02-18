
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using TraderPerformanceComparer.Assets;
using TraderPerformanceComparer.Commands;
using TraderPerformanceComparer.CustomControls;
using TraderPerformanceComparer.Model;
using TraderPerformanceComparer.ViewModel;
using System.Windows.Forms;

namespace TraderPerformanceComparer.ViewModel
{
    public class CompareViewModel : ViewModelBase
    {

        #region #Private Field Variables
        private ObservableCollection<SpreadLogModel> resultSetObsrvColl;
        private AppCompInitializer appInitializer;
        private DataComparer dataComparer;
        private bool canExecute = true;
        private string fileLoc1;
        private string fileLoc2;
        private string selectedItemUser1CmbBox;
        private int selectedIndexUser1CmbBox;
        private int selectedIndexUser2CmbBox;
        private string selectedItemUser2CmbBox;
        private List<String> ListOfUsers;
        private bool IsReadyToCompare;
        private int diffThresholdValue;
        private IDialogCoordinator dialogCoordinator;
        private string percentageThroughPutUser1;
        private string percentageThroughPutUser2;
        bool loadDataForUser2;
        private bool IsdatafromDBSelected;
        private bool IsdatafromDBAndFileSelected;
        private bool IsdatafromFileSelected;
        private string selectedDateTo;
        private string selecteddateFrom;
        private bool datePickerFromIsEnabled;
        private bool datePickerToIsEnabled;
        private bool isCacheSet;
        private bool isBuildCacheBtnEnbaled;

        private bool isLive;
        private bool isHist;

        #endregion

        #region #Properties

        public bool IsBuildCacheBtnEnabled
        {
            get { return isBuildCacheBtnEnbaled; }
            set { isBuildCacheBtnEnbaled = value;OnPropertyChanged("IsBuildCacheBtnEnabled");

                }
        }
        public bool IsLive
        {
            get { return isLive; }
            set { isLive = value;OnPropertyChanged("IsLive");
                if(isLive)
                {
                    IsHist = false;
                }
                }
        }

        public bool IsHist
        {
            get { return isHist; }
            set { isHist = value; OnPropertyChanged("IsHist");

                if(IsHist)
                {
                    IsLive =false;
                }
                 }
        }
        public bool IsCacheSet
        {
            get { return isCacheSet; }
            set { isCacheSet = value; OnPropertyChanged("IsCacheSet"); }
        }
        public bool  DatePickerFromIsEnabled
        {
            get { return datePickerFromIsEnabled; }
            set { datePickerFromIsEnabled = value; OnPropertyChanged("DatePickerFromIsEnabled"); }
        }

        public bool DatePickerToIsEnabled
        {
            get { return datePickerToIsEnabled; }
            set { datePickerToIsEnabled = value; OnPropertyChanged("DatePickerToIsEnabled"); }
        }


        public String SelectedDateTo
        {
            get { return selectedDateTo; }
            set { selectedDateTo = value; OnPropertyChanged("SelectedDateTo");

                if (selectedDateTo != null && selectedDateTo != "")
                {

                    ////  selectedDateTo = ((Convert.ToDateTime(selectedDateTo)).ToString("dd/MM/yyyy")).Substring(0, 10) + " 11:59:00 PM";
                    //string elem = selectedDateTo.Substring(0, 10);
                    //selectedDateTo = elem + " 12:00:00 AM";
                    selectedDateTo = Convert.ToDateTime(selectedDateTo).ToString("MM/dd/yyyy");
                    selectedDateTo = selectedDateTo.Substring(0, 10) + " 11:59:00 PM";
                }

            }
        }

        public String SelectedDateFrom 
        {
            get { return selecteddateFrom; }
            set
            {
                selecteddateFrom = value; OnPropertyChanged("SelectedDateFrom");
                if (selecteddateFrom != null && selecteddateFrom != "")
                {
                    //selecteddateFrom = ((Convert.ToDateTime(selecteddateFrom)).ToString("dd/MM/yyyy")).Substring(0, 10) + " 11:59:00 PM";
                    selecteddateFrom = Convert.ToDateTime(selecteddateFrom).ToString("MM/dd/yyyy");
                    //string elem = selecteddateFrom.Substring(0, 10);
                    //selectedDateTo = elem + " 12:00:00 AM";
                    selecteddateFrom = selecteddateFrom.Substring(0, 10) + " 12:00:00 AM";
                }

            }
        }

        public bool IsDataFromDbSelectedRadioBtnFlag
        {
            get { return IsdatafromDBSelected; }
            set { IsdatafromDBSelected = value;OnPropertyChanged("IsDataFromDbSelectedRadioBtnFlag");
                if(IsdatafromDBSelected==true)
                {
                    DatePickerToIsEnabled = true;
                    DatePickerFromIsEnabled = true;
                    IsBuildCacheBtnEnabled = true;
                }
               
            }
        }

        public bool IsDataFromDbAndFileRadioBtnFlag
        {
            get { return IsdatafromDBAndFileSelected; }
            set
            {
                IsdatafromDBAndFileSelected = value; OnPropertyChanged("IsDataFromDbAndFileRadioBtnFlag");
                if (IsdatafromDBAndFileSelected == true)
                {
                    DatePickerToIsEnabled = true;
                    DatePickerFromIsEnabled = true;
                    IsBuildCacheBtnEnabled = true;
                    
                }
               
            }
        }

        public bool IsDataFromFileSelectedRadioBtnFlag
        {
            get { return IsdatafromFileSelected; }
            set { IsdatafromFileSelected = value; OnPropertyChanged("IsDataFromFileSelectedRadioBtnFlag");
                if (IsdatafromFileSelected == true)
                {
                    DatePickerToIsEnabled = false;
                    DatePickerFromIsEnabled = false;
                    IsBuildCacheBtnEnabled = false;

                }
                

            }
        }
        public String PercentageThroughPutUser1
        {
            get { return percentageThroughPutUser1; }
            set { percentageThroughPutUser1 = value; OnPropertyChanged("PercentageThroughPutUser1"); }
        }

        public String PercentageThroughPutUser2
        {
            get { return percentageThroughPutUser2; }
            set { percentageThroughPutUser2 = value; OnPropertyChanged("PercentageThroughPutUser2"); }
        }
        public int ThresholdValueSet
        {
            get { return diffThresholdValue; }
            set { diffThresholdValue = value;OnPropertyChanged("ThresholdValueSet");
                StaticVariables.DiffThreshold = diffThresholdValue;
                 }
        }
        public ObservableCollection<SpreadLogModel> ResultSetObsrvColl
        {
            get { return resultSetObsrvColl; }
            set { resultSetObsrvColl = value; OnPropertyChanged("ResultSetObsrvColl"); }
        }

        public AppCompInitializer AppInitializer
        {
            get { return appInitializer; }
        }

        public List<String> ListOfParticipants
        {
            get { return ListOfUsers; }
            set { ListOfUsers = value; OnPropertyChanged("ListOfUsers"); }
        }
        public DataComparer DataComparer
        {
            get { return dataComparer; }
        }

        public String SelectedItemUser1CmbBox
        {
            get { return selectedItemUser1CmbBox; }
            set { selectedItemUser1CmbBox = value; OnPropertyChanged("SelectedItemUser1CmbBox"); IsReadyToCompare = false;

                if(IsDataFromFileSelectedRadioBtnFlag)
                {
                  //  DataComparer.FileReaderObj.OpenFileDialog.ShowDialog();
             

                    if (DataComparer.FileReaderObj.OpenFileDialog.ShowDialog() == DialogResult.OK)

                    {
                        FileLoc1= DataComparer.FileReaderObj.OpenFileDialog.FileName;

                    }

                    if (SelectedItemUser1CmbBox != "" && SelectedItemUser1CmbBox != null && SelectedItemUser2CmbBox != "" && SelectedItemUser2CmbBox != null)
                    {

                        if (FileLoc1.Length > 0 && FileLoc2.Length > 0)
                        {
                            IsReadyToCompare = true;
                            IsCacheSet = true;
                        }


                    }
                }

            }
        }

        public int SelectedIndexUser1CmbBox
        {
            get { return selectedIndexUser1CmbBox; }
            set { selectedIndexUser1CmbBox = value; OnPropertyChanged("SelectedIndexUser1CmbBox"); }
        }
        public int SelectedIndexUser2CmbBox
        {
            get { return selectedIndexUser2CmbBox; }
            set { selectedIndexUser2CmbBox = value; OnPropertyChanged("SelectedIndexUser2CmbBox"); }
        }

        public String SelectedItemUser2CmbBox
        {
            get { return selectedItemUser2CmbBox; }
            set { selectedItemUser2CmbBox = value; OnPropertyChanged("SelectedItemUser2CmbBox"); IsReadyToCompare = false;


                if (IsDataFromFileSelectedRadioBtnFlag || IsDataFromDbAndFileRadioBtnFlag)
                {
                    // DataComparer.FileReaderObj.OpenFileDialog.ShowDialog();


                    if (DataComparer.FileReaderObj.OpenFileDialog.ShowDialog() == DialogResult.OK)

                    {

                        FileLoc2 = DataComparer.FileReaderObj.OpenFileDialog.FileName;

                    }

                    if (SelectedItemUser1CmbBox != "" && SelectedItemUser1CmbBox != null && SelectedItemUser2CmbBox != "" && SelectedItemUser2CmbBox != null)
                    {

                        if (FileLoc1.Length > 0 && FileLoc2.Length > 0)
                        {
                            IsReadyToCompare = true;
                            IsCacheSet = true;
                        }


                    }


                }
               
            }
        }


        public string FileLoc1
        {
            get { return fileLoc1; }
            set { fileLoc1 = value; OnPropertyChanged("FileLoc1"); }
        }

        public string FileLoc2
        {
            get { return fileLoc2; }
            set { fileLoc2 = value; OnPropertyChanged("FileLoc2"); }
        }

        #endregion

        #region #commands and Command Handlers
        public RelayCommand<object> ButtonClickCommand
        {
            get { return new RelayCommand<object>(ButtonClickCommandHandler); }

        }

        public async void ButtonClickCommandHandler(object param)
        {

            switch (param.ToString())
            {
                case "GetFile":

                    IsReadyToCompare = false;
                    if (SelectedItemUser1CmbBox != "" && SelectedItemUser1CmbBox != null && SelectedItemUser2CmbBox != "" && SelectedItemUser2CmbBox != null)
                    {
                        if (SelectedItemUser1CmbBox == SelectedItemUser2CmbBox)
                        {
                            System.Windows.MessageBox.Show("Selected participants are same.Please choose different users to compare");
                        }
                        else
                        {
                            //FileLoc1 = AppInitializer.GetFileLocation(SelectedItemUser1CmbBox);
                            //FileLoc2 = AppInitializer.GetFileLocation(SelectedItemUser2CmbBox);

                            FileLoc1 = AppInitializer.FileLocation1;
                            FileLoc2 = AppInitializer.FileLocation2;


                            if (FileLoc1.Length > 0 && FileLoc2.Length > 0)
                            {
                                IsReadyToCompare = true;
                                IsCacheSet = true;
                            }

                        }
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("Participants aren't properly loaded");
                    }


                    break;

                case "Compare":

                    DataComparer.FileReaderObj.User1DataObsrvColl.Clear();
                    DataComparer.FileReaderObj.User2DataObsrvColl.Clear();
                    ResultSetObsrvColl.Clear();
                    PercentageThroughPutUser1 = "";
                    PercentageThroughPutUser2 = "";
                    

                    if (SelectedItemUser1CmbBox != "" && SelectedItemUser1CmbBox != null && SelectedItemUser2CmbBox != "" && SelectedItemUser2CmbBox != null)
                    {

                        ProgressDialogController controller = await dialogCoordinator.ShowProgressAsync(this, "Please Wait...Comparing participants", "Comparing participants");

                        if (IsDataFromFileSelectedRadioBtnFlag)
                        {
                            #region #Loading data from Xml or txt file 

//                            ProgressDialogController controller = await dialogCoordinator.ShowProgressAsync(this, "Please Wait...Comparing participants", "Comparing participants");
                            controller.SetIndeterminate();




                            controller.SetMessage("loading data for both participants.");
                            await Task.Delay(1000);

                            bool loadingDataForUser1 = DataComparer.FileReaderObj.LoadDataForUser(1, FileLoc1, true);

                            #region #When 2nd participant is Console
                            if (SelectedItemUser2CmbBox != "" && SelectedItemUser2CmbBox == "Console")
                            {
                                loadDataForUser2 = DataComparer.FileReaderObj.LoadDataForUser(3, FileLoc2, true);
                                await Task.Delay(1000);
                                controller.SetMessage("data has been loaded for both participants.");
                                await Task.Delay(1000);
                                controller.SetMessage("displaying result...");
                                await Task.Delay(1000);

                                ResultSetObsrvColl = DataComparer.GetFinalResultSet(DataComparer.FileReaderObj.User1DataObsrvColl, DataComparer.FileReaderObj.ConsoleObsrvColl, false, true, "IsComparisonWithConsole");

                                int user2Count = 0;
                                int user1Count = DataComparer.GetPercentage(ResultSetObsrvColl, SelectedItemUser1CmbBox, out user2Count);
                                //PercentageThroughPutUser1 = SelectedItemUser1CmbBox + " % : " + DataComparer.GetPercentage(DataComparer.user1Counter, ResultSetObsrvColl.Count);
                                PercentageThroughPutUser1 = SelectedItemUser1CmbBox + " % : " + DataComparer.GetPercentage(user1Count, ResultSetObsrvColl.Count) + "    Total Matches : " + user1Count + " | " + ResultSetObsrvColl.Count;
                                PercentageThroughPutUser2 = SelectedItemUser2CmbBox + " % : " + DataComparer.GetPercentage(user2Count, ResultSetObsrvColl.Count) + "    Total Matches : " + user2Count + " | " + ResultSetObsrvColl.Count;

                                await controller.CloseAsync();
                            }
                            #endregion


                            #region #When 2nd participant is Quantum user
                            else if (SelectedItemUser2CmbBox != "" && SelectedItemUser2CmbBox != "Console" && SelectedItemUser2CmbBox != "Console_NSE")
                            {


                                loadDataForUser2 = DataComparer.FileReaderObj.LoadDataForUser(2, FileLoc2, true);
                                await Task.Delay(1000);
                                controller.SetMessage("data has been loaded for both participants.");
                                await Task.Delay(1000);
                                controller.SetMessage("displaying result...");
                                await Task.Delay(1000);
                                ResultSetObsrvColl = DataComparer.GetFinalResultSet(DataComparer.FileReaderObj.User1DataObsrvColl, DataComparer.FileReaderObj.User2DataObsrvColl, false, true, "IsComparisonWithQuantum");

                                int user2Count = 0;
                                int user1Count = DataComparer.GetPercentage(ResultSetObsrvColl, SelectedItemUser1CmbBox, out user2Count);
                                //PercentageThroughPutUser1 = SelectedItemUser1CmbBox + " % : " + DataComparer.GetPercentage(DataComparer.user1Counter, ResultSetObsrvColl.Count);
                                PercentageThroughPutUser1 = SelectedItemUser1CmbBox + " % : " + DataComparer.GetPercentage(user1Count, ResultSetObsrvColl.Count) + "    Total Matches : " + user1Count + " | " + ResultSetObsrvColl.Count;
                                PercentageThroughPutUser2 = SelectedItemUser2CmbBox + " % : " + DataComparer.GetPercentage(user2Count, ResultSetObsrvColl.Count) + "    Total Matches : " + user2Count + " | " + ResultSetObsrvColl.Count;
                                await controller.CloseAsync();
                            }
                            #endregion

                            else
                            {
                                System.Windows.MessageBox.Show("Error in selection made for the users OR there aren't any matches available in db for users " + SelectedItemUser1CmbBox + " | " + SelectedItemUser2CmbBox);
                                await controller.CloseAsync();
                            }
                            #endregion
                        }
                        else if(IsDataFromDbAndFileRadioBtnFlag)
                        {
                            #region #When data is from db and file

                            controller.SetIndeterminate();
                            controller.SetMessage("loading data for both participants.");
                            await Task.Delay(1000);
                            bool loadingDataForUser1 = DataComparer.FileReaderObj.LoadDataForUserFromDataBase(Convert.ToInt16(SelectedItemUser1CmbBox.Remove(0, 1)), SelectedDateFrom, SelectedDateTo, 1, IsLive);
                            if (SelectedItemUser2CmbBox != "" && SelectedItemUser2CmbBox == "Console_NSE")
                            {
                                loadDataForUser2 = DataComparer.FileReaderObj.LoadDataForUser(5, FileLoc2, true);
                                await Task.Delay(1000);
                                controller.SetMessage("data has been loaded for both participants.");
                                await Task.Delay(1000);
                                controller.SetMessage("displaying result...");
                                await Task.Delay(1000);

                                ResultSetObsrvColl = DataComparer.GetFinalResultSet(DataComparer.FileReaderObj.User1DataObsrvColl, DataComparer.FileReaderObj.ConsoleObsrvColl, false, true, "IsComparisonWithConsole_NSE");

                                int user2Count = 0;
                                int user1Count = DataComparer.GetPercentage(ResultSetObsrvColl, SelectedItemUser1CmbBox, out user2Count);
                                //PercentageThroughPutUser1 = SelectedItemUser1CmbBox + " % : " + DataComparer.GetPercentage(DataComparer.user1Counter, ResultSetObsrvColl.Count);
                                PercentageThroughPutUser1 = SelectedItemUser1CmbBox + " % : " + DataComparer.GetPercentage(user1Count, ResultSetObsrvColl.Count) + "    Total Matches : " + user1Count + " | " + ResultSetObsrvColl.Count;
                                PercentageThroughPutUser2 = SelectedItemUser2CmbBox + " % : " + DataComparer.GetPercentage(user2Count, ResultSetObsrvColl.Count) + "    Total Matches : " + user2Count + " | " + ResultSetObsrvColl.Count;

                                await controller.CloseAsync();
                            }

                     

                            #endregion
                        }
                        else if(IsDataFromDbSelectedRadioBtnFlag)
                        {
                            #region #Loading data from database

                           // ProgressDialogController controller = await dialogCoordinator.ShowProgressAsync(this, "Please Wait...Comparing participants", "Comparing participants");
                            controller.SetIndeterminate();




                            controller.SetMessage("loading data for both participants.");
                            await Task.Delay(1000);
                            //bool loadingDataForUser1 = DataComparer.FileReaderObj.LoadDataForUser(1, FileLoc1, true);
                            bool loadingDataForUser1 = DataComparer.FileReaderObj.LoadDataForUserFromDataBase(Convert.ToInt16(SelectedItemUser1CmbBox.Remove(0, 1)), SelectedDateFrom, SelectedDateTo, 1, IsLive);


                            if (SelectedItemUser2CmbBox != "" && SelectedItemUser2CmbBox != "Console" && loadingDataForUser1)
                            {
                                loadDataForUser2 = DataComparer.FileReaderObj.LoadDataForUserFromDataBase(Convert.ToInt16(SelectedItemUser2CmbBox.Remove(0, 1)), SelectedDateFrom, SelectedDateTo, 2, IsLive);
                                await Task.Delay(1000);
                                controller.SetMessage("data has been loaded for both participants.");
                                await Task.Delay(1000);
                                controller.SetMessage("displaying result...");
                                await Task.Delay(1000);
                                ResultSetObsrvColl = DataComparer.GetFinalResultSet(DataComparer.FileReaderObj.User1DataObsrvColl, DataComparer.FileReaderObj.User2DataObsrvColl, false, true, "IsComparisonWithQuantum");

                                int user2Count = 0;
                                int user1Count = DataComparer.GetPercentage(ResultSetObsrvColl, SelectedItemUser1CmbBox, out user2Count);
                                //PercentageThroughPutUser1 = SelectedItemUser1CmbBox + " % : " + DataComparer.GetPercentage(DataComparer.user1Counter, ResultSetObsrvColl.Count);
                                PercentageThroughPutUser1 = SelectedItemUser1CmbBox + " % : " + DataComparer.GetPercentage(user1Count, ResultSetObsrvColl.Count) + "    Total Matches : " + user1Count + " | " + ResultSetObsrvColl.Count;
                                PercentageThroughPutUser2 = SelectedItemUser2CmbBox + " % : " + DataComparer.GetPercentage(user2Count, ResultSetObsrvColl.Count) + "    Total Matches : " + user2Count + " | " + ResultSetObsrvColl.Count;
                                await controller.CloseAsync();
                            }
                            else
                            {
                                System.Windows.MessageBox.Show("Error in selection made for the users OR there aren't result available in db for user : " + SelectedItemUser1CmbBox + " | " + SelectedItemUser2CmbBox);
                                await controller.CloseAsync();
                            }
                            #endregion

                        }
                        else
                        {
                         
                            await controller.CloseAsync();
                            System.Windows.MessageBox.Show("Radiobutton is not selected");


                        }
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("Participants aren't properly loaded");
                    }

                    if (DataComparer.FileReaderObj.ListOfErrorneousConsoleRecord.Count>0)
                        {
                            foreach(var item in DataComparer.FileReaderObj.ListOfErrorneousConsoleRecord)
                            {
                            System.Windows.MessageBox.Show(item);
                            }
                        }
                      

                    break;

                case "Cache":

                    #region #Loading Cahce from database

                    DataComparer.FileReaderObj.User1DataObsrvColl.Clear();
                    DataComparer.FileReaderObj.User2DataObsrvColl.Clear();
                    ResultSetObsrvColl.Clear();
                    PercentageThroughPutUser1 = "";
                    PercentageThroughPutUser2 = "";
                   
                    ProgressDialogController controller_buildCache = await dialogCoordinator.ShowProgressAsync(this, "Please Wait...", "building cache");
                    controller_buildCache.SetIndeterminate();
                    controller_buildCache.SetMessage("Building cache from all the fetched records");
                    await Task.Delay(1000);
                    if (SelectedDateTo!=null && SelectedDateTo!="" && SelectedDateFrom!=null && SelectedDateFrom!="")
                    {

                        bool   loadCache = DataComparer.FileReaderObj.LoadDataForUserFromDataBase(3, SelectedDateFrom, SelectedDateTo, 3,IsLive);
                        if(loadCache)
                        {
                            controller_buildCache.SetMessage("Cahche download finished. Dowloaded records within the selected date range is : " + DataComparer.FileReaderObj.UserDataReaderDataBase.AlphaDataTable.Rows.Count);
                            await Task.Delay(2000);
                            controller_buildCache.SetMessage("System ready for comparison");
                            if(DataComparer.FileReaderObj.UserDataReaderDataBase.AlphaDataTable.Rows.Count>0)
                            {
                                IsCacheSet = true;
                            }
                            await Task.Delay(1000);
                            await controller_buildCache.CloseAsync();

                        }
                        else
                        {
                            controller_buildCache.SetMessage("Failed to load cache.");
                            await Task.Delay(1000);
                            await controller_buildCache.CloseAsync();
                          
                        }
                    }
                    else
                    {
                        controller_buildCache.SetMessage("Problem encountered with selected dates.Kindly check selected date ranges.");
                        await Task.Delay(1000);
                        await controller_buildCache.CloseAsync();
                    }

                    #endregion

                    break;

            }
        }

        #endregion

        #region #Constructors
        public CompareViewModel(IDialogCoordinator instance)
        {
            FileLoc1 = "";
            FileLoc2 = "";
            IsReadyToCompare = false;
            dialogCoordinator = instance;
            ResultSetObsrvColl = new ObservableCollection<SpreadLogModel>();
            appInitializer = new AppCompInitializer();
            dataComparer = new DataComparer();
            dataComparer.FileReaderObj.SetConfigReaderAndUserDataReaderFromDataBase(appInitializer.ConfigReader);
            ThresholdValueSet = StaticVariables.DiffThreshold;
            //IsDataFromDbSelectedRadioBtnFlag = true;
            //IsDataFromFileSelectedRadioBtnFlag = false;
           // SelectedDateFrom = DateTime.Now.ToString("dd/MM/yyyy");
          //  SelectedDateTo = DateTime.Now.ToString("dd/MM/yyyy") ;
            if (FillComboBox())
            {
                SelectedIndexUser1CmbBox = 0;
                SelectedIndexUser2CmbBox = 1;
            }
            //IsBuildCacheBtnEnabled = false;
            //if (IsDataFromFileSelectedRadioBtnFlag)
            //{
            //    IsCacheSet = true;
            //}
            //else
            //{
            //    IsCacheSet = false;
                
            //}
            IsHist = true;
        }

        #endregion

        #region #Helper Methods
        public bool FillComboBox()
        {
            try
            {
                ListOfParticipants = AppInitializer.ReturnListOfParticipants();
                return true;

            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
                return false;
            }
        }

        #endregion



    }
}
