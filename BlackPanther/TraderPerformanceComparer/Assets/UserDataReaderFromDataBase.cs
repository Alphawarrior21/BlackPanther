using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using TraderPerformanceComparer.Model;

namespace TraderPerformanceComparer.Assets
{
    public class UserDataReaderFromDataBase
    {

        private DbHandler dbHandler;
        private List<Tuple<Int64, double, short, short, int, int, string>> orderPrices;
        string strategyType;
        private List<Tuple<Int64, short, SpreadLogModel>> orderPriceAgainstOrderId;
        private DataTable alphaDataTable;
        double callPrice;
        double putPrice;
        double futprice;
        float threshold;
        string orderPrice;
        double strike;
        bool TraversingNotReq;
        


        public DataTable AlphaDataTable
        {
            get { return alphaDataTable; }
            set { alphaDataTable = value; }
        }

      
        public DbHandler DBHandler
        {
            get { return dbHandler; }

        }

        public UserDataReaderFromDataBase(ConfigFileReader configReader)
        {
            dbHandler = new DbHandler(configReader.GetAmtHostName(), configReader.GetAmtPort(), configReader.GetAmtdbName(), configReader.GetAmtUserName(), configReader.GetAmtUserPassword());
            orderPrices = new List<Tuple<Int64, double, short, short, int, int, string>>();
            orderPriceAgainstOrderId = new List<Tuple<long, short, SpreadLogModel>>();
            

            alphaDataTable = new DataTable();
        

        }

       
        
        public bool CreateTableForCombineResult(string query)
        {
            try
            {
                DBHandler.connect();
            }
            catch (Exception ex)
            {
                dbHandler.disConnect();
                MessageBox.Show("Failed to connect to database.System exiting");
                System.Environment.Exit(1);

            }

            if (DBHandler.isConnected())
            {
                try
                {
                    DBHandler.runTrasectionalQuery(query);
                    dbHandler.disConnect();
                    return true;
                }
                catch(Exception ex)
                {
                    dbHandler.disConnect();
                    MessageBox.Show("Failed to run query : " + query);
                    return false;
                    
                }
             

            }
            else
            {
                return false;
            }

         }

        public DataTable GetDataTable(short userId,string dateFrom,string dateTo)
        {
            if(AlphaDataTable.Rows.Count>0)
            {
                try
                {
                    DataView dv = new DataView(AlphaDataTable);
                    DataTable dt = new DataTable();
                    //DateTime dateFromInDateFormat = Convert.ToDateTime(dateFrom);
                    //DateTime dateToInDateFormat = Convert.ToDateTime(dateTo);
                    //string filterCriteria = "(userid=" + userId + ") and (tsinsert>='" + dateFromInDateFormat.ToString("MM/dd/yyyy") + "') and (tsinsert<='" + dateToInDateFormat.ToString("MM/dd/yyyy") + "')";

                 
                    string filterCriteria = "(userid=" + userId + ") and (tsinsert>='" + dateFrom + "') and (tsinsert<='" + dateTo + "')";

                    dv.RowFilter = filterCriteria;
                    dt = dv.ToTable();
                    return dt;
                }
                catch(Exception ex)
                {
                    string filePath = @"C:\Error.txt";

                    using (StreamWriter writer = new StreamWriter(filePath, true))
                    {
                        writer.WriteLine("Message :" + ex.Message + "<br/>" + Environment.NewLine + "StackTrace :" + ex.StackTrace +
                           "" + Environment.NewLine + "Date :" + DateTime.Now.ToString() + "Date entered : dateFrom : " + dateFrom + "| dateto : "+ dateTo);
                        writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
                    }
                    return null;
                }
              
            }
            else
            {
                return null;
            }
        }

        public DataTable GetDataTable(string query)
        {
            try
            {
                DBHandler.connect();
            }
            catch (Exception ex)
            {
                dbHandler.disConnect();
                MessageBox.Show("Failed to connect to database.System exiting");
                System.Environment.Exit(1);

            }
            DataTable dt = new DataTable();
            DataView dv = new DataView();



            if (DBHandler.isConnected())
            {
                dv = DBHandler.runNonTrasectionalQuery(query);

                if (dv != null && dv.Count > 0)
                {
                    dt = dv.ToTable();
                    dbHandler.disConnect();
                    return dt;
                }
                else
                {
                    dbHandler.disConnect();
                    return null;
                }

            }
            else
            {
                dbHandler.disConnect();
                return null;
            }



        }



        public ObservableCollection<SpreadLogModel> GetDataFromDatabase(DataTable dt)
        {
            ObservableCollection<SpreadLogModel> UserResult = new ObservableCollection<SpreadLogModel>();
            orderPriceAgainstOrderId.Clear();



            if (dt != null && dt.Rows.Count > 0)
            {
                // FillOrderPriceTuple(dt);
                orderPriceAgainstOrderId = FillOrderPriceDictionary(dt, orderPriceAgainstOrderId);

                foreach (var item in orderPriceAgainstOrderId)
                {

                    if (item != null)
                    {
                        if (UserResult.Any(p => (p.LogTime == item.Item3.LogTime) && (p.StrategyTag == item.Item3.StrategyTag) && (p.Threshold == item.Item3.Threshold)) == true)
                        {

                            var coll = from p in UserResult where (p.LogTime == item.Item3.LogTime) && (p.StrategyTag == item.Item3.StrategyTag) && (p.Threshold == item.Item3.Threshold) select p;
                            foreach (var value in coll)
                            {
                                if (value.ExchangeOrderId > item.Item3.ExchangeOrderId)
                                {
                                    UserResult.Add(item.Item3);
                                    UserResult.Remove(value);
                                    break;
                                }
                            }


                        }
                        else
                        {
                            UserResult.Add(item.Item3);
                        }
                    }
                }

            }

            return UserResult;
        }



        #region Filling Order Price in the OrderIdOrderPrice Dictionary to be used by spread log


        public void FillOrderPriceTuple(DataTable dt)
        {

            foreach (DataRow node in dt.Rows)
            {
                try
                {
                    long exorderId = Convert.ToInt64(node["exorderno"]);
                    int logTime = Convert.ToInt32(node["logtime"]);
                    string DateTime = DateTimeHelper.getDateTimeStrFromNSEEpochSeconds(logTime, "HH:mm:ss");
                    string symbol = Convert.ToString(node["secdesc"]);
                    double orderPrice = (Convert.ToDouble(node["price"])) / 100;
                    short state = Convert.ToInt16(node["orderstate"]);
                    short side = Convert.ToInt16(node["side"]);
                    int token = Convert.ToInt32(node["token"]);
                    int qty = Convert.ToInt32(node["qty"]);
                    string tokenType = Convert.ToString(node["type"]);
                    FillOrderIdOrderPriceinDict(exorderId, orderPrice, state, side, qty, token, tokenType);

                    //SpreadPrice = ((double)e.SpreadItem.SpreadPrice) / 100,
                    //this.Threshold = ((float)ThresholdPS / 100);
                    //this.TickTypeInChar = Convert.ToChar(this.TickType);
                }
                catch (Exception ex)
                {

                }

            }

        }
        public void FillOrderIdOrderPriceinDict(Int64 OrderId, double Price, short State, short Side, int qty, int token, string tokenType)
        {
            try
            {


                // QuantumAppDevLogger.log(DevLoggerLevel.LOG_INFO, "Order details getting added to tuple for the use of spread log. Order Id : " + OrderId + " | price : " + Price + " | State : " + State + " | Side : " + Side + " | hybridId : " + hybridId + " | qty : " + qty);
                List<Tuple<Int64, double, short, short, int, int, string>> _ordersTuple = orderPrices;
                if (_ordersTuple.Count > 0)
                {

                    bool tupleHadProduct = _ordersTuple.Any(m => m.Item1 == OrderId && m.Item2 == Price && m.Item3 == State && m.Item4 == Side && m.Item5 == qty && m.Item6 == token && m.Item7 == tokenType);
                    if (!tupleHadProduct)
                    {
                        Tuple<Int64, double, short, short, int, int, string> _item = new Tuple<long, double, short, short, int, int, string>(OrderId, Price, State, Side, qty, token, tokenType);
                        orderPrices.Add(_item);
                    }
                }
                else
                {
                    Tuple<Int64, double, short, short, int, int, string> _item = new Tuple<long, double, short, short, int, int, string>(OrderId, Price, State, Side, qty, token, tokenType);
                    orderPrices.Add(_item);
                }


            }
            catch (Exception ex)
            {

            }
        }
        #endregion


        #region #Process dataTable row element and return the required spreadlogmodel object
        public SpreadLogModel ProcessDataTableRowElement(DataRow row)
        {
            try
            {

                int userId = Convert.ToInt32(row["userid"]);
                string userCode = "T" + userId;
                long exorderId = Convert.ToInt64(row["exorderno"]);
                int logTime = Convert.ToInt32(row["logtime"]);
                string DateTime = DateTimeHelper.getDateTimeStrFromNSEEpochSeconds(logTime, "HH:mm:ss");
                string symbol = Convert.ToString(row["secdesc"]);
                short state = Convert.ToInt16(row["orderstate"]);
                short side = Convert.ToInt16(row["side"]);
                short strategyTypeInShort = Convert.ToInt16(row["strategytype"]);
                int qty = Convert.ToInt32(row["qty"]);
                string tokenType = Convert.ToString(row["type"]);


                string stateStr = state == 9 ? "CANCELLED" : "TRADED";
                string spreadSide = side == 1 ? "BUY" : "SELL";


                if (strategyTypeInShort == 5)
                {
                    strategyType = "FF";
                }
                else if (strategyTypeInShort == 2)
                {
                    strategyType = "CR";
                }
                else if (strategyTypeInShort == 3)
                {
                    strategyType = "BF";
                }
                else if (strategyTypeInShort == 4)
                {
                    strategyType = "BOX";
                }
                else if (strategyTypeInShort == 6)
                {
                    strategyType = "CSR";
                }
                else if (strategyTypeInShort == 8)
                {
                    strategyType = "PULSE";
                }
                else
                {
                    strategyType = "";
                }

                string orderPricefinal = FillOrderPricesInSpread(exorderId, state);
                float threshold = ((float)(Convert.ToInt32(row["origth"])) / 100);
                char mktAnalysis = Convert.ToChar(row["ticktype"]);

                SpreadLogModel item = new SpreadLogModel()
                {
                    UserID = (short)userId,
                    UserCode = userCode,
                    DateTime = DateTime,
                    LogTime = logTime,

                    ExchangeOrderId = exorderId,
                    SpreadQty = qty,
                    StrategyTag = symbol,
                    Threshold = threshold,
                    StrategyType = strategyType,
                    Side = spreadSide,

                    //HybridID = Convert.ToString((node["HybridID"])),
                    SpreadState = stateStr,

                    OrderPrice = orderPricefinal + " ]",
                    //MktAnalaysisParams=Convert.ToString((node["MktAnalaysisParams"]))
                    Mkt = mktAnalysis


                };

                return item;


            }
            catch (Exception ex)
            {
                return null;
            }
        }





        #region Fill order prices in spread items 




        public string FillOrderPricesInSpread(Int64 exOrderId, short state)
        {
            try
            {


                var item = orderPriceAgainstOrderId.Find(s => (s.Item1 == exOrderId) && (s.Item2 == state));
                return item.Item3.OrderPrice;

            }
            catch (Exception ex)
            {
                return "";
            }
        }


        public List<String> GetTableNames(List<DateTime> dates)
        {
            List<string> tableNames = new List<string>();
            foreach (var date in dates)
            {
                tableNames.Add("order_" + date.ToString("yyyyMMdd"));
            }
            return tableNames;
        }


        public List<DateTime> GetDateArray(DateTime dtFrom, DateTime dtTo)
        {
            var dates = new List<DateTime>();


            for (var dt = dtFrom; dt <= dtTo; dt = dt.AddDays(1))
            {
                dates.Add(dt);
            }

            return (List<DateTime>)dates;
        }

        public ObservableCollection<SpreadLogModel> FillThandSingleOrderRow(ObservableCollection<SpreadLogModel> obj)
        {

            ObservableCollection<SpreadLogModel> tempColl = (ObservableCollection<SpreadLogModel>)obj;
            ObservableCollection<SpreadLogModel> finalColl = new ObservableCollection<SpreadLogModel>();

         

            //foreach(var item in tempColl)
            //{
            //    int count = 0;

            //    foreach (var compVal in tempColl)
            //    {
            //        if (compVal.StrategyTag == item.StrategyTag && compVal.ExchangeOrderId == item.ExchangeOrderId && compVal.LogTime == item.LogTime && !compVal.IsThisRowMarkedAsFinal)
            //        {
            //            if (compVal.OptionType == "CE")
            //            {
            //                callPrice = compVal.CallPrice;
            //                putPrice = item.PutPrice;
            //                futprice = item.FuturePrice;
            //                strike = compVal.Strike;
            //                compVal.IsThisRowMarkedAsFinal = true;
            //                count++;
            //            }
            //            else if (compVal.OptionType == "PE")
            //            {
            //                putPrice = compVal.PutPrice;
            //                callPrice = item.CallPrice;
            //                futprice = item.FuturePrice;
            //                strike = compVal.Strike;
            //                compVal.IsThisRowMarkedAsFinal = true;
            //                count++;
            //            }
            //            else
            //            {
            //                futprice = compVal.FuturePrice;
            //                putPrice = item.PutPrice;
            //                callPrice = item.CallPrice;
            //                compVal.IsThisRowMarkedAsFinal = true;
            //                count++;
            //            }

            //            if (count == 2)
            //            {
            //                orderPrice = "CE : " + item.CallPrice + " | PE : " + putPrice + " | FT : " + futprice;
            //                threshold = (float)(Convert.ToDouble(futprice - strike) - Convert.ToDouble(putPrice - item.CallPrice));
            //                item.Threshold = threshold;
            //                item.OrderPrice = orderPrice;
                            

            //                break;

            //            }


            //        }

            //    }

            //}

            foreach (var item in tempColl.ToList())
            {
                bool collHasProduct = tempColl.Any(m => m.ExchangeOrderId == item.ExchangeOrderId && m.LogTime == item.LogTime && m.StrategyTag==item.StrategyTag);
                if (collHasProduct)
                {
                    //var element  = obj.Find(s => (s.Item1 == exorderId) && (s.Item2 == state));
                    var element = from p in obj where (p.ExchangeOrderId == item.ExchangeOrderId) && (p.StrategyTag == item.StrategyTag) && (p.LogTime == item.LogTime) select p;

                    //double callPrice;
                    //double putPrice;
                    //double futprice;
                    //float threshold;
                    //string orderPrice;
                    TraversingNotReq = false;
                    foreach (var col in element.ToList())
                    {
                        if(!col.IsThisRowMarkedAsFinal)
                        {
                            if (col.OptionType == "CE")
                            {
                                callPrice = col.CallPrice;
                                item.CallPrice = callPrice;
                                putPrice = item.PutPrice;
                                futprice = item.FuturePrice;
                                strike = col.Strike;
                                col.IsThisRowMarkedAsFinal = true;
                             
                               
                            }
                            else if (col.OptionType == "PE")
                            {
                                putPrice = col.PutPrice;
                                item.PutPrice = putPrice;
                                callPrice = item.CallPrice;
                                futprice = item.FuturePrice;
                                strike = col.Strike;
                                col.IsThisRowMarkedAsFinal = true;
                                tempColl.Remove(col);

                               
                            }
                            else
                            {
                                futprice = col.FuturePrice;
                                item.FuturePrice = futprice;
                                putPrice = item.PutPrice;
                                callPrice = item.CallPrice;
                                col.IsThisRowMarkedAsFinal = true;
                                tempColl.Remove(col);
                              
                            }
                        }
                        else
                        {
                            TraversingNotReq = true;
                            break;
                           
                        }
                       
                       

                    }
                    if(!TraversingNotReq)
                    {
                        orderPrice = "CE : " + item.CallPrice + " | PE : " + putPrice + " | FT : " + futprice;
                        threshold = (float)(Convert.ToDouble(futprice - strike) - Convert.ToDouble(putPrice - item.CallPrice));
                        item.Threshold = threshold;
                        item.OrderPrice = orderPrice;
                        

                    }
                   
                }

            }

            foreach(var item in tempColl)
            {
                if( item.OrderPrice!=null)
                {
                    finalColl.Add(item);
                }
            }
            return finalColl;


            
        }

        public List<Tuple<Int64, short, SpreadLogModel>> FillOrderPriceDictionary(DataTable dt, List<Tuple<Int64, short, SpreadLogModel>> dict)
        {

            List<Tuple<Int64, short, SpreadLogModel>> tempDict = (List<Tuple<Int64, short, SpreadLogModel>>)dict;



            foreach (DataRow node in dt.Rows)
            {

                try
                {

                    long exorderId = Convert.ToInt64(node["exorderno"]);
                    int logTime = Convert.ToInt32(node["logtime"]);
                    string DateTime = DateTimeHelper.getDateTimeStrFromNSEEpochSeconds(logTime, "HH:mm:ss");
                    string symbol = Convert.ToString(node["secdesc"]);
                    string symbolShort = Convert.ToString(node["symbol"]);
                    double orderPrice = (Convert.ToDouble(node["price"])) / 100;
                    short state = 2;
                    short side = Convert.ToInt16(node["side"]);
                    int token = Convert.ToInt32(node["token"]);
                    int qty = Convert.ToInt32(node["qty"]);
                    string tokenType = Convert.ToString(node["type"]);
                    float threshold = ((float)(Convert.ToInt32(node["origth"])) / 100);
                    char mktAnalysis = Convert.ToChar(node["ticktype"]);
                   // char mktAnalysis = (char)(node["ticktype"]);
                    //char mktAnalysis = 'Z';
                    int userId = Convert.ToInt32(node["userid"]);
                    //int userId = Convert.ToInt32("AcceptRejectRule/");
                    string userCode = "T" + userId;

                    //if(tempDict.Count>0)
                    //{
                    if (state != 9)
                    {
                        bool tupleHasProduct = tempDict.Any(m => m.Item1 == exorderId && m.Item2 == state);
                        if (!tupleHasProduct)
                        {
                            SpreadLogModel element = new SpreadLogModel();
                            element.ExchangeOrderId = exorderId;
                            element.DateTime = DateTime;
                            element.LogTime = logTime;
                            element.SpreadQty = qty;
                            element.UserID = (short)userId;
                            element.UserCode = userCode;



                            element.Threshold = threshold;
                            element.StrategyType = strategyType;
                            element.Side = side == 1 ? "BUY" : "SELL";
                            element.SpreadState = state == 9 ? "CANCELLED" : "TRADED";
                            element.Mkt = mktAnalysis;

                            if (tokenType == "PE")
                            {
                                element.OrderPrice = tokenType + " | " + Convert.ToString(orderPrice);
                                element.PutPrice = orderPrice;
                                element.StrategyTag = symbol.Remove(symbol.Length - 2, 2);
                                element.Symbol = symbolShort;
                            }
                            else if (tokenType == "CE")
                            {
                                element.OrderPrice = tokenType + " | " + Convert.ToString(orderPrice);
                                element.CallPrice = orderPrice;
                                element.StrategyTag = symbol.Remove(symbol.Length - 2, 2);
                                element.Symbol = symbolShort;
                            }
                            else
                            {
                                element.OrderPrice = tokenType + " | " + Convert.ToString(orderPrice);
                                 element.FuturePrice = orderPrice;
                                // element.StrategyTag = symbol;
                            }


                            Tuple<Int64, short, SpreadLogModel> _item = new Tuple<Int64, short, SpreadLogModel>(exorderId, state, element);
                            tempDict.Add(_item);
                        }
                        else
                        {
                            var item = tempDict.Find(s => (s.Item1 == exorderId) && (s.Item2 == state));

                            SpreadLogModel element = new SpreadLogModel();
                            element.ExchangeOrderId = item.Item1;
                            element.DateTime = item.Item3.DateTime;
                            element.LogTime = item.Item3.LogTime;
                            element.SpreadQty = item.Item3.SpreadQty;
                            element.StrategyTag = item.Item3.StrategyTag;
                            element.Symbol = symbolShort;
                            element.Threshold = item.Item3.Threshold;
                            element.StrategyType = item.Item3.StrategyType;
                            element.Side = item.Item3.Side;
                            element.SpreadState = item.Item3.SpreadState;
                            element.Mkt = item.Item3.Mkt;
                            element.UserCode = item.Item3.UserCode;
                            element.UserID = item.Item3.UserID;



                            if (tokenType == "PE")
                            {
                                element.OrderPrice = item.Item3.OrderPrice + " | " + tokenType + " | " + Convert.ToString(orderPrice);
                                element.PutPrice = orderPrice;
                                // element.FuturePrice = item.Item3.FuturePrice;
                                element.CallPrice = item.Item3.CallPrice;


                            }
                            else if (tokenType == "CE")
                            {
                                element.OrderPrice = item.Item3.OrderPrice + " | " + tokenType + " | " + Convert.ToString(orderPrice);
                                element.PutPrice = item.Item3.PutPrice;
                                //  element.FuturePrice = item.Item3.FuturePrice;
                                element.CallPrice = orderPrice;
                            }
                            else
                            {
                                element.OrderPrice = item.Item3.OrderPrice + " | " + tokenType + " | " + Convert.ToString(orderPrice);
                                // element.StrategyTag = item.Item3.StrategyTag + "|" + symbol;
                                element.PutPrice = item.Item3.PutPrice;
                                element.FuturePrice = orderPrice;
                                element.CallPrice = item.Item3.CallPrice;
                            }

                            tempDict.Remove(item);
                            Tuple<long, short, SpreadLogModel> item_New = new Tuple<long, short, SpreadLogModel>(item.Item1, item.Item2, element);
                            tempDict.Add(item_New);



                        }
                    }
                    else
                    {
                        continue;
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    return null;
                }

            }





            return tempDict;
        }

        #endregion
        #endregion

    }

    public class OrderPriceParams
    {
        private Int64 orderId;
        public Int64 OrderId
        {
            get { return orderId; }
            set { orderId = value; }
        }

        private string orderPrice;
        public string OrderPrice
        {
            get { return orderPrice; }
            set { orderPrice = value; }
        }

        private short state;
        public short State
        {
            get { return state; }
            set { state = value; }
        }

        private short side;
        public short Side
        {
            get { return side; }
            set { side = value; }
        }


    }

}
