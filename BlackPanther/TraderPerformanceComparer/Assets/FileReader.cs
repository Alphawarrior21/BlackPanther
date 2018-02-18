
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using TraderPerformanceComparer.Assets;
using TraderPerformanceComparer.Commands;
using TraderPerformanceComparer.CustomControls;
using TraderPerformanceComparer.Model;
using TraderPerformanceComparer.ViewModel;
using System.Windows.Forms;
using System.Globalization;

namespace TraderPerformanceComparer.Assets
{
    public class FileReader
    {
        #region #Private variables
        private List<string> listOfLines;
        private ObservableCollection<SpreadLogModel> user1DataObsrvColl;
        private ObservableCollection<SpreadLogModel> user2DataObsrvColl;
        private ObservableCollection<SpreadLogModel> consoleObsrvColl;
        private UserDataReaderFromDataBase userDataReaderFromDataBase;
        private List<String> listOfErrorneousConsoleRecord;
        private List<SpreadLogModel> listOfItemToRemove;
        private ConfigFileReader configReader;
        bool IsSchemaCreated;
        bool IsCombinedTableCreationComplete;
        string firstDate;
        private OpenFileDialog openFileDialog;
        double callPrc;
        double PutPrc;
        double FutPrc;
        double strike;
        Dictionary<Int64, SpreadLogModel> dictionaryOrderId_SpreadLogModel;
        Int64 leastOrderId;



        //openFileDialog.ShowDialog();
        //            openFileDialog.InitialDirectory = @"C:\";
        //            openFileDialog1.RestoreDirectory = true;


        #endregion

        #region #Properties

        public Dictionary<Int64, SpreadLogModel> DictionaryOrderId_SpreadLogModel
        {
            get { return dictionaryOrderId_SpreadLogModel; }
            set { dictionaryOrderId_SpreadLogModel = value; }
        }
        public OpenFileDialog OpenFileDialog
        {
            get { return openFileDialog; }
            set { openFileDialog = value; }
        }
        public UserDataReaderFromDataBase UserDataReaderDataBase
        {
            get { return userDataReaderFromDataBase; }
            set { userDataReaderFromDataBase = value; }
        }

        public List<string> ListOfLinesFromConsole
        {
            get { return listOfLines; }
        }
        public ObservableCollection<SpreadLogModel> User1DataObsrvColl
        {
            get { return user1DataObsrvColl; }
        }

        public ObservableCollection<SpreadLogModel> User2DataObsrvColl
        {
            get { return user2DataObsrvColl; }
        }

        public ObservableCollection<SpreadLogModel> ConsoleObsrvColl
        {
            get { return consoleObsrvColl; }
        }

        public List<String> ListOfErrorneousConsoleRecord
        {
            get { return listOfErrorneousConsoleRecord; }
        }

        #endregion

        #region #Constructor

        public FileReader()
        {
            listOfLines = new List<string>();
            user1DataObsrvColl = new ObservableCollection<SpreadLogModel>();
            user2DataObsrvColl = new ObservableCollection<SpreadLogModel>();
            consoleObsrvColl = new ObservableCollection<SpreadLogModel>();
            listOfErrorneousConsoleRecord = new List<string>();
            listOfItemToRemove = new List<SpreadLogModel>();
            openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = @"C:\";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.Filter = "txt files (*.txt)|*.txt|xml files (*.xml)|*.xml|csv files (*.csv)|*.csv";
            openFileDialog.ReadOnlyChecked = true;
            openFileDialog.ShowReadOnly = true;
            dictionaryOrderId_SpreadLogModel = new Dictionary<long, SpreadLogModel>();
        }

        #endregion

        #region #Helper Methods

        public void SetConfigReaderAndUserDataReaderFromDataBase(ConfigFileReader configReaderObj)
        {
            configReader = (ConfigFileReader)configReaderObj;
            UserDataReaderDataBase = new UserDataReaderFromDataBase(configReader);
        }
        public List<String> ReadFromTextFile(string filePath)
        {
            listOfLines.Clear();

            using (FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (BufferedStream bs = new BufferedStream(fs))
            using (StreamReader sr = new StreamReader(bs))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line != "")
                    {
                        listOfLines.Add(line);
                    }
                }
            }
            return listOfLines;
        }

        #region #Read data from csv file

        public SpreadLogModel ReturnSpreadLogModelObjPostReadingCommaSeparatedTXTFile(string csvLine,Int64 foId)
        {
            string[] values = csvLine.Split(',');
            Int64 Fo_Id = Convert.ToInt64(values[30]);
            string stateStr = (Convert.ToString(values[29])).TrimEnd();
         
       
            if (Fo_Id == foId && stateStr!="OCXL")
            {
                #region #Extracting values of the csv file line and returning the object 
                string dateTimeStr = (Convert.ToString(values[10]) + " " + Convert.ToString(values[28]));
                
                DateTime dt;
                if (DateTime.TryParse(dateTimeStr, out dt))
                {
                    // int result = DateTimeHelper.calculateSeconds(dt);
                }


                string Symbol_console = Convert.ToString(values[9]);

                string side = Convert.ToString(values[7]);
                string OptType = Convert.ToString(values[12]);
                //string priceIndicator = Convert.ToString(values[12]);
                
                if (OptType == "CE")
                {
                    callPrc = Convert.ToDouble(values[18]);
                     strike = Convert.ToDouble(values[11]);
                }
                else if (OptType == "PE")
                {
                    PutPrc = Convert.ToDouble(values[18]);
                    strike = Convert.ToDouble(values[11]);
                }
                else
                {
                    FutPrc = Convert.ToDouble(values[18]);
                    strike = 0;
                }

                Int64 ExcgId = Convert.ToInt64(values[6]);

                SpreadLogModel dictItem;
                if(DictionaryOrderId_SpreadLogModel.TryGetValue(ExcgId,out dictItem))
                {
                    if(dictItem.OptionType=="CE")
                    {

                    }
                    else if(dictItem.OptionType=="PE")
                    {

                    }
                    else
                    {

                    }

                    return DictionaryOrderId_SpreadLogModel[ExcgId];

                }
                else
                {
                    SpreadLogModel item = new SpreadLogModel()
                    {

                        LogTime = DateTimeHelper.calculateSeconds(dt),
                        DateTime = DateTimeHelper.getDateTimeStrFromNSEEpochSeconds(DateTimeHelper.calculateSeconds(dt), "HH:mm:ss"),
                        StrategyTag = Symbol_console,
                        Side = side == "B" ? "Buy" : "Sell",
                        OptionType = OptType,
                        CallPrice = callPrc,
                        PutPrice = PutPrc,
                        FuturePrice = FutPrc,
                        ExchangeOrderId = ExcgId,
                        SpreadState = stateStr == "OCXL" ? "CANCELLED" : "TRADED",
                        Strike = strike

                    };

                    DictionaryOrderId_SpreadLogModel.Add(ExcgId, item);
                    return DictionaryOrderId_SpreadLogModel[ExcgId];
                }
                

                #endregion

            }
            else
            {
                return null;
            }
           
        }




        #endregion


        public ObservableCollection<SpreadLogModel> TransferFromDictionaryToCollection(Dictionary<Int64,SpreadLogModel> dict)
        {
            ObservableCollection<SpreadLogModel> tempColl = new ObservableCollection<SpreadLogModel>();
            ObservableCollection<SpreadLogModel> finalColl = new ObservableCollection<SpreadLogModel>();
            listOfItemToRemove.Clear();
            leastOrderId = 0;
            foreach (var item in dict)
            {
                tempColl.Add(item.Value);
            }

            foreach(var item in tempColl.ToList())
            {
                if(item.ExchangeOrderId== 1700000001452627)
                {
                    Console.WriteLine("symbol : " + item.StrategyTag);
                }

                bool hasValue = finalColl.Any(m => m.LogTime == item.LogTime && m.StrategyTag == item.StrategyTag && m.Threshold == item.Threshold);
                if(hasValue)
                {
                    var coll = from p in finalColl where (p.LogTime == item.LogTime) && (p.StrategyTag == item.StrategyTag) && (p.Threshold == item.Threshold) select p;
                   
                   leastOrderId = coll.FirstOrDefault().ExchangeOrderId;
                    SpreadLogModel leastValue = coll.FirstOrDefault();
                    if(leastOrderId!=0)
                    {
                        foreach(var val in coll)
                        {
                            if(val.ExchangeOrderId<leastOrderId)
                            {
                                listOfItemToRemove.Add(leastValue);
                                leastOrderId = val.ExchangeOrderId;
                                leastValue = val;
                                
                            }
                        }
                        if(!(finalColl.Contains(leastValue)))
                        {
                            finalColl.Add(leastValue);
                            foreach (var delItem in listOfItemToRemove)
                            {
                                tempColl.Remove(delItem);
                            }
                        }
                       
                    }

                }
                else
                {
                    finalColl.Add(item);
                }

            }

            //foreach(var item in finalColl)
            //{
            //    Console.WriteLine("Symbol " + item.StrategyTag + " | threshold : " + item.Threshold + " | orderPrices : " + item.OrderPrice + " | exchnageId : " + item.ExchangeOrderId + " | logtime : " + item.LogTime + " | DateTime : " + item.DateTime + " | " + item.LogTimeStr);
            //}

            return finalColl;
        }

        public SpreadLogModel SetDictionaryForConsole_NSEComparison(string csvLine, Int64 foId)
        {
            string[] values = csvLine.Split(',');
            Int64 Fo_Id = Convert.ToInt64(values[30]);
            string stateStr = (Convert.ToString(values[29])).TrimEnd();


            if (Fo_Id == foId && stateStr != "OCXL")
            {
                
                string dateTimeStr = (Convert.ToString(values[10]) + " " + Convert.ToString(values[28]));

                DateTime dt;
                if (DateTime.TryParse(dateTimeStr, out dt))
                {
                    // int result = DateTimeHelper.calculateSeconds(dt);
                }


                string Symbol_console = Convert.ToString(values[9]).TrimStart();
                Symbol_console = Symbol_console.TrimEnd();

                string side = Convert.ToString(values[7]);
                string OptType = Convert.ToString(values[12]);
                //string priceIndicator = Convert.ToString(values[12]);

                if (OptType == "CE")
                {
                    callPrc = Convert.ToDouble(values[18]);
                    strike = Convert.ToDouble(values[11]);
                }
                else if (OptType == "PE")
                {
                    PutPrc = Convert.ToDouble(values[18]);
                    strike = Convert.ToDouble(values[11]);
                }
                else
                {
                    FutPrc = Convert.ToDouble(values[18]);
                    strike = 0;
                }

                Int64 ExcgId = Convert.ToInt64(values[6]);

                SpreadLogModel dictItem;
                if (DictionaryOrderId_SpreadLogModel.TryGetValue(ExcgId, out dictItem))
                {
                    if ( OptType=="CE")
                    {
                        dictItem.CallPrice = callPrc;
                        dictItem.Strike = strike;
                    }
                    else if (OptType=="PE")
                    {
                        dictItem.PutPrice = PutPrc;
                        dictItem.Strike = strike;
                    }
                    else
                    {
                        dictItem.FuturePrice = FutPrc;
                        
                    }

                    if(dictItem.FuturePrice!=0 && dictItem.CallPrice!=0 && dictItem.PutPrice!=0)
                    {
                        dictItem.OrderPrice = "CE : " + dictItem.CallPrice + " | PE : " + dictItem.PutPrice + " | FT : " + dictItem.FuturePrice;
                        //dictItem.Threshold = (float)(Convert.ToDouble(dictItem.FuturePrice - dictItem.Strike) - Convert.ToDouble(dictItem.PutPrice - dictItem.CallPrice));
                        dictItem.Threshold = (float)(Convert.ToDouble((dictItem.CallPrice - dictItem.PutPrice) - (dictItem.FuturePrice - dictItem.Strike)));

                    }
                }
                else
                {
                    SpreadLogModel item = new SpreadLogModel()
                    {

                        LogTime = DateTimeHelper.calculateSeconds(dt),
                        DateTime = DateTimeHelper.getDateTimeStrFromNSEEpochSeconds(DateTimeHelper.calculateSeconds(dt), "HH:mm:ss"),
                        StrategyTag = Symbol_console,
                        Side = side == "B" ? "Buy" : "Sell",
                        OptionType = OptType,
                        CallPrice = callPrc,
                        PutPrice = PutPrc,
                        FuturePrice = FutPrc,
                        ExchangeOrderId = ExcgId,
                        SpreadState = stateStr == "OCXL" ? "CANCELLED" : "TRADED",
                        Strike = strike

                    };

                    DictionaryOrderId_SpreadLogModel.Add(ExcgId, item);

                }
            }
            return null;
         
        }

        public bool LoadDataForUserFromDataBase(short user, string dateFrom, string dateTo, short sequence,bool IsLiveDb)
        {
           
            switch (sequence)
            {
                case 1:

                   // string query_user1 = "select DISTINCT L.exorderno,L.logtime,ob.secdesc,L.price,L.origth ,L.ticktype,L.orderstate,L.side,L.token,L.qty,L.strategytype,L.userid,ob.type from  order_combined_table L INNER JOIN order_combined_table R ON L.exorderno=R.exorderno and L.token=R.token and L.price=R.price ,orderbook_20171221 ob where  R.userid = " + user + "and  L.token=ob.token and L.orderstate=2 and  L.tsinsert>='" + dateFrom + "' and  L.tsinsert<='" + dateTo + "'order by L.exorderno";
                    if ((user1DataObsrvColl = UserDataReaderDataBase.GetDataFromDatabase(UserDataReaderDataBase.GetDataTable(user,dateFrom,dateTo))).Count > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }



                case 2:

                   // string query_user2 = "select DISTINCT L.exorderno,L.logtime,ob.secdesc,L.price,L.origth ,L.ticktype,L.orderstate,L.side,L.token,L.qty,L.strategytype,L.userid,ob.type from  order_combined_table L INNER JOIN order_combined_table R ON L.exorderno=R.exorderno and L.token=R.token and L.price=R.price ,orderbook_20171221 ob where  R.userid = " + user + "and  L.token=ob.token and L.orderstate=2 and  L.tsinsert>='" + dateFrom + "' and  L.tsinsert<='" + dateTo + "'order by L.exorderno";
                    if ((user2DataObsrvColl = UserDataReaderDataBase.GetDataFromDatabase(UserDataReaderDataBase.GetDataTable(user, dateFrom, dateTo))).Count > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case 3:
                    //UserDataReaderDataBase.AlphaDataTable.Clear();
                    if (!IsLiveDb)
                    {
                        #region #History Database

                        IsSchemaCreated = false;
                        IsCombinedTableCreationComplete = false;
                        List<string> tablesName = UserDataReaderDataBase.GetTableNames(UserDataReaderDataBase.GetDateArray(Convert.ToDateTime(dateFrom), Convert.ToDateTime(dateTo)));
                        List<String> ActualTableList = new List<string>();
                        firstDate = "";
                        foreach (var table in tablesName)
                        {
                            string query = "select count(*) as numberofrec from " + table;
                            DataTable dt = UserDataReaderDataBase.GetDataTable(query);
                            if (dt != null)
                            {
                                if (dt.Rows.Count > 0)
                                {
                                    DataRow row = dt.Rows[0];
                                    int count = Convert.ToInt32(row["numberofrec"]);
                                    if (count > 0)
                                    {
                                        if (!IsSchemaCreated)
                                        {
                                            firstDate = table;
                                            bool IsTableAlreadyExist = UserDataReaderDataBase.CreateTableForCombineResult("drop table order_combined_table;");
                                            IsSchemaCreated = UserDataReaderDataBase.CreateTableForCombineResult("select * into order_combined_table from " + firstDate);
                                        }

                                    }
                                }

                            }
                            else
                            {
                                ActualTableList.Add(table);
                            }

                        }

                        if (ActualTableList.Count > 0)
                        {
                            foreach (var item in ActualTableList)
                            {
                                tablesName.Remove(item);
                            }
                        }



                        if (tablesName.Count > 0)
                        {



                            if (IsSchemaCreated)
                            {

                                foreach (var table in tablesName)
                                {

                                    string query = "Insert into order_combined_table select * from " + table;
                                    IsCombinedTableCreationComplete = UserDataReaderDataBase.CreateTableForCombineResult(query);


                                }

                                string date = tablesName[0];
                                date = date.Remove(0, date.IndexOf('_') + 1);
                                string query_user3 = "select DISTINCT L.exorderno,L.logtime,ob.secdesc,L.price,L.origth ,L.ticktype,L.orderstate,L.side,L.token,L.qty,L.strategytype,L.userid,L.tsinsert,ob.type from  order_combined_table L INNER JOIN order_combined_table R ON L.exorderno=R.exorderno and L.token=R.token and L.price=R.price ,orderbook_" + date + " ob where  L.token=ob.token and L.orderstate=2 order by L.exorderno";
                                if ((UserDataReaderDataBase.AlphaDataTable = (DataTable)UserDataReaderDataBase.GetDataTable(query_user3)).Rows.Count > 0)
                                {
                                    return true;
                                }
                                else
                                {
                                    return false;
                                }

                            }
                            else
                            {
                                return false;
                            }

                        }
                        else
                        {
                            return false;
                        }

                        #endregion
                    }
                    else
                    {

                        #region #Live Database

                        try
                        {
                            string query_user3 = "select DISTINCT L.exorderno,L.logtime,ob.secdesc,ob.symbol,L.price,L.origth ,L.ticktype,L.orderstate,L.side,L.token,L.qty,L.strategytype,L.userid,L.tsinsert,ob.type from  orders L INNER JOIN orders R ON L.exorderno=R.exorderno and L.token=R.token and L.price=R.price ,orderbook ob where  L.token=ob.token and L.orderstate=2 order by L.exorderno";
                            if ((UserDataReaderDataBase.AlphaDataTable = (DataTable)UserDataReaderDataBase.GetDataTable(query_user3)).Rows.Count > 0)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }

                        }
                        catch (Exception ex)
                        {
                            System.Windows.MessageBox.Show(ex.ToString());
                            return false;
                        }
                               
                         
                        #endregion

                    }



                default: return false;
            }
        }



        public bool LoadDataForUser(short user, string fileLoc, bool matchwithevryRec)
        {


            switch (user)
            {
                case 1:

                    if ((user1DataObsrvColl = GetDataFromXml(fileLoc, user, matchwithevryRec)).Count > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }


                case 2:

                    if ((user2DataObsrvColl = GetDataFromXml(fileLoc, user, matchwithevryRec)).Count > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case 3:
                    if ((consoleObsrvColl = ProcessDataFromConsoleDataSource(ReadFromTextFile(fileLoc), matchwithevryRec)).Count > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case 4:


                    if ((user1DataObsrvColl = UserDataReaderDataBase.GetDataFromDatabase(UserDataReaderDataBase.GetDataTable(""))).Count > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case 5:
                    consoleObsrvColl.Clear();
                    Int64 num = 122001001424000;


                   
                    var tempColl = new ObservableCollection<SpreadLogModel>(File.ReadAllLines(fileLoc).Select(v => SetDictionaryForConsole_NSEComparison(v, num)).Where(x=>x!=null));


                    //  Console.WriteLine(DictionaryOrderId_SpreadLogModel.Count);

                    consoleObsrvColl = (ObservableCollection<SpreadLogModel>)TransferFromDictionaryToCollection(DictionaryOrderId_SpreadLogModel);
                    if (consoleObsrvColl.Count > 0)
                    {
                        
                        return true;
                    }
                    else
                    {
                        return false;
                    }


                default:
                    return false;


            }

        }


        public ObservableCollection<SpreadLogModel> GetDataFromXml(string filePath, short user, bool MatchWithEveryRec)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(filePath);
                XmlElement root = doc.DocumentElement;
                XmlNodeList nodes = root.SelectNodes("Table1");
                ObservableCollection<SpreadLogModel> UserResult = new ObservableCollection<SpreadLogModel>();
                foreach (XmlNode node in nodes)
                {
                    if (node.HasChildNodes)
                    {

                        string mktA = Convert.ToString((node["MktAnalaysisParams"].InnerText));
                        // Console.WriteLine(mktA.IndexOf("TickType : "));

                        mktA = mktA.Substring(mktA.IndexOf("TickType"), 11);

                        SpreadLogModel item = new SpreadLogModel()

                        {
                            UserID = Convert.ToInt16((node["UserID"].InnerText)),
                            UserCode = Convert.ToString("T" + (node["UserID"].InnerText)),
                            DateTime = Convert.ToString((node["DateTime"].InnerText)),
                            LogTime = Convert.ToInt32((node["LogTime"].InnerText)),
                            SpreadPrice = Convert.ToDouble((node["SpreadPrice"].InnerText)),
                            ExchangeOrderId = Convert.ToInt64((node["ExchangeOrderId"].InnerText)),
                            SpreadQty = Convert.ToInt32((node["SpreadQty"].InnerText)),
                            StrategyTag = (node["StrategyTag"].InnerText),
                            Threshold = float.Parse(node["Threshold"].InnerText),
                            StrategyType = Convert.ToString((node["StrategyType"].InnerText)),
                            Side = Convert.ToString((node["Side"].InnerText)),
                            StrategyID = Convert.ToInt32((node["StrategyID"].InnerText)),
                            //HybridID = Convert.ToString((node["HybridID"].InnerText)),
                            SpreadState = Convert.ToString((node["SpreadState"].InnerText)),
                            SpreadPriceStr = Convert.ToString((node["SpreadState"].InnerText)) == "CANCELLED" ? "" : Convert.ToString((node["SpreadPrice"].InnerText)),
                            OrderPrice = Convert.ToString((node["OrderPrice"].InnerText)) + " ]",
                            //MktAnalaysisParams=Convert.ToString((node["MktAnalaysisParams"].InnerText))
                            MktAnalaysisParams = mktA


                        };

                        if (MatchWithEveryRec)
                        {
                            if (UserResult.Any(p => (p.LogTime == item.LogTime) && (p.StrategyTag == item.StrategyTag) && (p.Threshold == item.Threshold)) == true)
                            {

                                var coll = from p in UserResult where (p.LogTime == item.LogTime) && (p.StrategyTag == item.StrategyTag) && (p.Threshold == item.Threshold) select p;
                                foreach (var value in coll)
                                {
                                    if (value.ExchangeOrderId > item.ExchangeOrderId)
                                    {
                                        UserResult.Add(item);

                                        UserResult.Remove(value);
                                        break;
                                    }
                                }


                            }
                            else
                            {
                                UserResult.Add(item);
                            }

                        }
                        else
                        {
                            UserResult.Add(item);
                        }


                    }

                }


                return UserResult;
            }



            catch (Exception ex)
            {
                return null;
            }


        }


        public ObservableCollection<SpreadLogModel> ProcessDataFromConsoleDataSource(List<string> rawData, bool MatchWithEveryRec)
        {

            ObservableCollection<SpreadLogModel> _consoleObsrvColl = new ObservableCollection<SpreadLogModel>();
            try
            {

                foreach (var item in rawData)
                {
                    try
                    {
                        SpreadLogModel obj = (SpreadLogModel)ReturnSpreadLogObjFromConsoleDate(item);
                        if (MatchWithEveryRec)
                        {
                            if (_consoleObsrvColl.Any(p => (p.LogTime == obj.LogTime) && (p.StrategyTag == obj.StrategyTag) && (p.SpreadPrice == obj.SpreadPrice)) == true)
                            {

                                var coll = from p in _consoleObsrvColl where (p.LogTime == obj.LogTime) && (p.StrategyTag == obj.StrategyTag) && (p.SpreadPrice == obj.SpreadPrice) select p;
                                foreach (var value in coll)
                                {
                                    if (value.ExchangeOrderId > obj.ExchangeOrderId)
                                    {
                                        _consoleObsrvColl.Add(obj);
                                        listOfItemToRemove.Add(value);
                                        _consoleObsrvColl.Remove(value);
                                        break;
                                    }
                                }


                            }
                            else
                            {
                                _consoleObsrvColl.Add(obj);
                            }

                        }
                        else
                        {
                            _consoleObsrvColl.Add(obj);
                        }

                    }
                    catch (Exception ex)
                    {
                        listOfErrorneousConsoleRecord.Add(item);
                    }


                }

            }
            catch (Exception ex)
            {
                return null;
            }


            return _consoleObsrvColl;
        }



        public SpreadLogModel ReturnSpreadLogObjFromConsoleDate(string line)
        {

            string logTimeStr = SearchedObjectFromString(line, StaticVariables.RegexPattern_Time);
            DateTime dt;
            if (DateTime.TryParse(DateTimeHelper.ReturnDate_from_DDD_MMM_YYYY(logTimeStr.Substring(logTimeStr.IndexOf(":") + 2, (logTimeStr.Length) - (logTimeStr.IndexOf(":") + 3))), out dt))
            {
                // int result = DateTimeHelper.calculateSeconds(dt);
            }

            string Symbol_console = SearchedObjectFromString(line, StaticVariables.RegexPattern_Symbol);
            string id_console = SearchedObjectFromString(line, StaticVariables.RegexPattern_ID);
            string Side = SearchedObjectFromString(line, StaticVariables.RegexPattern_Type);
            string OptType = SearchedObjectFromString(line, StaticVariables.RegexPattern_Option);
            string callPrc = SearchedObjectFromString(line, StaticVariables.RegexPattern_CallPrice);
            string PutPrc = SearchedObjectFromString(line, StaticVariables.RegexPattern_PutPrice);
            string FutPrc = SearchedObjectFromString(line, StaticVariables.RegexPattern_FuturePrice);
            string ExcgId = (SearchedObjectFromString(line, StaticVariables.RegexPattern_OrderNum)).Substring(0, (SearchedObjectFromString(line, StaticVariables.RegexPattern_OrderNum).Length) - 1);
            string ExecPrice = SearchedObjectFromString(line, StaticVariables.RegexPattern_Spread);

            SpreadLogModel item = new SpreadLogModel()
            {
                LogTime = DateTimeHelper.calculateSeconds(dt),
                DateTime = DateTimeHelper.getDateTimeStrFromNSEEpochSeconds(DateTimeHelper.calculateSeconds(dt), "HH:mm:ss"),
                StrategyTag = (Symbol_console.Substring(Symbol_console.IndexOf(":") + 2, ((Symbol_console.Length) - (Symbol_console.IndexOf(":") + 3)))).Trim(),
                ID_Console = Convert.ToInt32(id_console.Substring(id_console.IndexOf(":") + 2, ((id_console.Length) - (id_console.IndexOf(":") + 3)))),
                Side = Side.Substring(Side.IndexOf(":") + 2, ((Side.Length) - (Side.IndexOf(":") + 3))),
                OptionType = OptType.Substring(OptType.IndexOf(":") + 2, ((OptType.Length) - (OptType.IndexOf(":") + 3))),
                CallPrice = Convert.ToDouble(callPrc.Substring(callPrc.IndexOf(":") + 2, ((callPrc.Length) - (callPrc.IndexOf(":") + 3)))),
                CallPriceStr = callPrc,
                PutPrice = Convert.ToDouble(PutPrc.Substring(PutPrc.IndexOf(":") + 2, ((PutPrc.Length) - (PutPrc.IndexOf(":") + 3)))),
                PutPriceStr = PutPrc,
                FuturePrice = Convert.ToDouble(FutPrc.Substring(FutPrc.IndexOf(":") + 2, ((FutPrc.Length) - (FutPrc.IndexOf(":") + 3)))),
                FuturePriceStr = FutPrc,
                ExchangeOrderId = Convert.ToInt64(ExcgId.Substring(ExcgId.IndexOf(":") + 2, ((ExcgId.Length) - (ExcgId.IndexOf(":") + 3)))),
                SpreadPrice = Convert.ToDouble(ExecPrice.Substring(ExecPrice.IndexOf(":") + 2, ((ExecPrice.Length) - (ExecPrice.IndexOf(":") + 3))))

            };

            return item;

        }




        public string SearchedObjectFromString(string line, string pattern)
        {
            Regex regex = new Regex(pattern);
            MatchCollection matches = regex.Matches(line);
            //for (var i = 0; i < matches.Count; i++)
            //{

            //}

            return matches[0].ToString();
        }

        public double SearchedObjectFromStringForConsole(string line, string pattern)
        {
            Regex regex = new Regex(pattern);
            MatchCollection matches = regex.Matches(line);
            for (var i = 0; i < matches.Count; i++)
            {
                double priceVal;

                // (matches[i].ToString()).Remove(matches[i].Length - 1);
                //                Console.WriteLine(matches[i].ToString());

                if (matches[i].ToString() != "" && double.TryParse((matches[i].ToString()).Remove(0, (matches[i].ToString()).IndexOf(':') + 1), out priceVal))
                {
                    return priceVal;
                }

            }
            return 0;


        }

        #endregion



    }
}
