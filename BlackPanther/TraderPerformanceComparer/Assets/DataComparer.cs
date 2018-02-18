using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TraderPerformanceComparer.Assets;
using TraderPerformanceComparer.Commands;
using TraderPerformanceComparer.CustomControls;
using TraderPerformanceComparer.Model;
using TraderPerformanceComparer.ViewModel;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace TraderPerformanceComparer.Assets
{
 public  class DataComparer
    {

        #region #Private Field Variables
        private FileReader fileReader;
        private bool IsStrictMatchingFlag;
        private bool MatchWithEveryRec;
        private ObservableCollection<SpreadLogModel> finalResultCollAftrComparison;
        private bool foundMatchingElement;
        private string winneruserId;
        private SpreadLogModel leastValueObj;
        private SpreadLogModel greaterValueObj;

        private SpreadLogModel user1ValueObj;
        private SpreadLogModel user2ValueObj;
        private double user1PercentageValue;
        private double user2PercentageValue;

        private DbHandler dbHandler;

        #endregion

        #region #Properties
        public FileReader FileReaderObj
        {
            get { return fileReader; }
        }
        #endregion

        #region #commands and Command Handlers
        #endregion

        #region #Constructors
        public DataComparer()
        {
            fileReader = new FileReader();
            finalResultCollAftrComparison = new ObservableCollection<SpreadLogModel>();
            
        }
        #endregion

        #region #Helper Methods

        public ObservableCollection<SpreadLogModel> GetFinalResultSet(ObservableCollection<SpreadLogModel> user1Data, ObservableCollection<SpreadLogModel> user2Data, bool IsStrictChecking, bool MatchWithEveryRecordOfUser2,string comaprison)
        {
            finalResultCollAftrComparison.Clear();

       switch(comaprison)
            {
                case "IsComparisonWithConsole_NSE":

                    #region #When comparison is with Console_NSE 

                    //foreach(var item in user1Data)
                    //{
                    //    Console.WriteLine("Symbol " + item.StrategyTag + " | threshold : " + item.Threshold + " | orderPrices : " + item.OrderPrice + " | exchnageId : " + item.ExchangeOrderId + " | logtime : " + item.LogTime + " | DateTime : " + item.DateTime + " | " + item.LogTimeStr);
                    //}
                    //Console.WriteLine("Printing user 2 details ");

                    //foreach (var item in user2Data)
                    //{
                    //    Console.WriteLine("Symbol " + item.StrategyTag + " | threshold : " + item.Threshold + " | orderPrices : " + item.OrderPrice + " | exchnageId : " + item.ExchangeOrderId + " | logtime : " + item.LogTime + " | DateTime : " + item.DateTime + " | " + item.LogTimeStr);
                    //}

                    foreach (var item in user1Data)
                    {
                       

                        if (!IsStrictChecking)
                        {
                            //var coll = user2Data.Where(p => (p.LogTime == item.LogTime) && (p.StrategyTag == item.StrategyTag) && (p.Threshold == item.Threshold));
                            var coll = from p in user2Data where (p.LogTime == item.LogTime) && (p.StrategyTag == item.Symbol) && (Math.Abs(p.Threshold) == Math.Abs(item.Threshold)) select p;

                            if (coll.Any(p => (p.LogTime == item.LogTime) && (p.StrategyTag == item.Symbol) && (Math.Abs(p.Threshold) == Math.Abs(item.Threshold)) == true))
                            {

                                foreach (var value in coll)
                                {
                                    if ((item.CallPrice == value.CallPrice) || (item.PutPrice == value.PutPrice) || (item.FuturePrice == value.FuturePrice))
                                    {
                                        if (item.ExchangeOrderId > value.ExchangeOrderId)
                                        {
                                            leastValueObj = (SpreadLogModel)value;
                                            leastValueObj.UserCode = "Console_NSE";
                                            greaterValueObj = item;

                                        }
                                        else
                                        {

                                            leastValueObj = item;
                                            greaterValueObj = (SpreadLogModel)value;
                                            greaterValueObj.UserCode = "Console_NSE";

                                        }



                                    }

                                }


                                /***till here we have got the element with which item[user1] has to be compared****/
                                if (leastValueObj != null && greaterValueObj != null)
                                {
                                    if (leastValueObj.UserCode.Contains(item.UserCode))
                                    {
                                        user1ValueObj = (SpreadLogModel)leastValueObj;
                                        user2ValueObj = (SpreadLogModel)greaterValueObj;
                                    }
                                    else
                                    {
                                        user1ValueObj = (SpreadLogModel)greaterValueObj;
                                        user2ValueObj = (SpreadLogModel)leastValueObj;
                                    }




                                    WinnerHighlightStruct winnerStruct = new WinnerHighlightStruct();
                                    winnerStruct.participant1UserId = item.UserCode;
                                    winnerStruct.winneruserId = user1ValueObj.ExchangeOrderId < user2ValueObj.ExchangeOrderId ? user1ValueObj.UserCode : user2ValueObj.UserCode;

                                    SpreadLogModel element = new SpreadLogModel()
                                    {
                                        LogTimeStr = Convert.ToString(user1ValueObj.LogTime) + " || " + Convert.ToString(user2ValueObj.LogTime),
                                        ExchangeOrderIdStr = Convert.ToString(user1ValueObj.ExchangeOrderId) + " || " + Convert.ToString(user2ValueObj.ExchangeOrderId),
                                        OrderPrice = user1ValueObj.OrderPrice + " || " + user2ValueObj.OrderPrice,
                                        GreaterValue = user1ValueObj.ExchangeOrderId < user2ValueObj.ExchangeOrderId ? user1ValueObj.UserCode : user2ValueObj.UserCode,
                                        // DateTime = DateTimeHelper.getDateTimeStrFromNSEEpochSeconds(item1.LogTime, "HH:mm:ss") + "||" + DateTimeHelper.getDateTimeStrFromNSEEpochSeconds(leastValueItem.LogTime, "HH:mm:ss"),
                                        DateTime = DateTimeHelper.getDateTimeStrFromNSEEpochSeconds(item.LogTime, "MM/dd/yyyy HH:mm:ss"),
                                        StrategyTag = user1ValueObj.StrategyTag + " || " + user2ValueObj.StrategyTag,
                                        MktAnalaysisParams = user1ValueObj.Mkt + " || " + user2ValueObj.Mkt,
                                        Difference = Math.Abs(Convert.ToInt32(user1ValueObj.ExchangeOrderId - user2ValueObj.ExchangeOrderId)),
                                        WinnerHighLightVal = winnerStruct


                                    };




                                    if (Math.Abs(element.Difference) <= StaticVariables.DiffThreshold)
                                        finalResultCollAftrComparison.Add(element);
                                    leastValueObj = null;
                                    greaterValueObj = null;

                                }




                            }


                        }

                    }
                    #endregion


                    break;
                case "IsComparisonWithConsole":

                    #region #When comparison is with console
                    foreach (var item in user1Data)
                    {
                        double callPrice_Q = fileReader.SearchedObjectFromStringForConsole(item.OrderPrice, StaticVariables.RegexPattern_CALLQ);
                        double putPrice_Q = fileReader.SearchedObjectFromStringForConsole(item.OrderPrice, StaticVariables.RegexPattern_PUTQ);
                        double futPrice_Q = fileReader.SearchedObjectFromStringForConsole(item.OrderPrice, StaticVariables.RegexPattern_FUTQ);
                        string symbolAccToConsole = item.StrategyTag.Remove(0, 3);
                        symbolAccToConsole = symbolAccToConsole.Remove((symbolAccToConsole).IndexOf('_'));
                        item.StrategyTag = symbolAccToConsole;

                        if (!IsStrictChecking)
                        {

                            foreach (var value in user2Data)
                            {


                                if (value.LogTime == item.LogTime && value.StrategyTag == item.StrategyTag && (float)value.SpreadPrice == item.Threshold)
                                {

                                    if ((callPrice_Q == value.CallPrice) || (putPrice_Q == value.PutPrice) || (futPrice_Q == value.FuturePrice))
                                    {
                                        if (item.ExchangeOrderId > value.ExchangeOrderId)
                                        {
                                            leastValueObj = (SpreadLogModel)value;
                                            leastValueObj.UserCode = "Console";
                                            leastValueObj.OrderPrice = leastValueObj.FuturePriceStr + " | " + leastValueObj.CallPriceStr + " | " + leastValueObj.PutPriceStr;
                                            greaterValueObj = item;

                                        }
                                        else
                                        {

                                            leastValueObj = item;
                                            greaterValueObj = (SpreadLogModel)value;
                                            greaterValueObj.UserCode = "Console";
                                            greaterValueObj.OrderPrice = greaterValueObj.FuturePriceStr + " | " + greaterValueObj.CallPriceStr + " | " + greaterValueObj.PutPriceStr;

                                        }



                                        /***till here we have got the element with which item[user1] has to be compared****/

                                        if (leastValueObj.UserCode.Contains("T"))
                                        {
                                            user1ValueObj = (SpreadLogModel)leastValueObj;
                                            user2ValueObj = (SpreadLogModel)greaterValueObj;
                                        }
                                        else
                                        {
                                            user1ValueObj = (SpreadLogModel)greaterValueObj;
                                            user2ValueObj = (SpreadLogModel)leastValueObj;
                                        }


                                        WinnerHighlightStruct winnerStruct = new WinnerHighlightStruct();
                                        winnerStruct.participant1UserId = item.UserCode;
                                        winnerStruct.winneruserId = user1ValueObj.ExchangeOrderId < user2ValueObj.ExchangeOrderId ? user1ValueObj.UserCode : user2ValueObj.UserCode;

                                        SpreadLogModel element = new SpreadLogModel()
                                        {
                                            LogTimeStr = Convert.ToString(user1ValueObj.LogTime) + " || " + Convert.ToString(user2ValueObj.LogTime),
                                            ExchangeOrderIdStr = Convert.ToString(user1ValueObj.ExchangeOrderId) + " || " + Convert.ToString(user2ValueObj.ExchangeOrderId),
                                            OrderPrice = user1ValueObj.OrderPrice + " || " + user2ValueObj.OrderPrice,
                                            GreaterValue = user1ValueObj.ExchangeOrderId < user2ValueObj.ExchangeOrderId ? user1ValueObj.UserCode : user2ValueObj.UserCode,
                                            // DateTime = DateTimeHelper.getDateTimeStrFromNSEEpochSeconds(item1.LogTime, "HH:mm:ss") + "||" + DateTimeHelper.getDateTimeStrFromNSEEpochSeconds(leastValueItem.LogTime, "HH:mm:ss"),
                                            DateTime = DateTimeHelper.getDateTimeStrFromNSEEpochSeconds(item.LogTime, "HH:mm:ss"),
                                            StrategyTag = user1ValueObj.StrategyTag + " || " + user2ValueObj.StrategyTag,
                                            MktAnalaysisParams = (user1ValueObj.MktAnalaysisParams).Remove(0, user1ValueObj.MktAnalaysisParams.IndexOf(':') + 1),
                                            Difference = Math.Abs(Convert.ToInt32(user1ValueObj.ExchangeOrderId - user2ValueObj.ExchangeOrderId)),
                                            WinnerHighLightVal = winnerStruct


                                        };




                                        //SpreadLogModel element = new SpreadLogModel()
                                        //{
                                        //    LogTimeStr = Convert.ToString(leastValueObj.LogTime) + " || " + Convert.ToString(greaterValueObj.LogTime),
                                        //    ExchangeOrderIdStr = Convert.ToString(leastValueObj.ExchangeOrderId) + " || " + Convert.ToString(greaterValueObj.ExchangeOrderId),
                                        //    OrderPrice = leastValueObj.OrderPrice + " || " + greaterValueObj.OrderPrice,
                                        //    GreaterValue = greaterValueObj.UserCode,
                                        //    // DateTime = DateTimeHelper.getDateTimeStrFromNSEEpochSeconds(item1.LogTime, "HH:mm:ss") + "||" + DateTimeHelper.getDateTimeStrFromNSEEpochSeconds(leastValueItem.LogTime, "HH:mm:ss"),
                                        //    DateTime = DateTimeHelper.getDateTimeStrFromNSEEpochSeconds(item.LogTime, "HH:mm:ss"),
                                        //    StrategyTag = leastValueObj.StrategyTag + " || " + greaterValueObj.StrategyTag,
                                        //    MktAnalaysisParams = item.MktAnalaysisParams,
                                        //    Difference = Math.Abs(Convert.ToInt32(leastValueObj.ExchangeOrderId - greaterValueObj.ExchangeOrderId))


                                        //};



                                        if (Math.Abs(element.Difference) <= StaticVariables.DiffThreshold)
                                            finalResultCollAftrComparison.Add(element);


                                    }




                                }

                            }

                            #region #difficult to debug method
                            //var coll =  from p in user2Data where (p.LogTime == item.LogTime) && (p.StrategyTag == item.StrategyTag) && ((float)p.SpreadPrice == item.Threshold) select p;

                            //if (coll.Any(p => (p.LogTime == item.LogTime) && (p.StrategyTag == item.StrategyTag) && ((float)p.SpreadPrice == item.Threshold)) == true)
                            //{


                            //    foreach (var value in coll)
                            //    {
                            //        if ((callPrice_Q == value.CallPrice) || (putPrice_Q == value.PutPrice) || (futPrice_Q == value.FuturePrice))
                            //        {
                            //            if (item.ExchangeOrderId > value.ExchangeOrderId)
                            //            {
                            //                leastValueObj = (SpreadLogModel)value;
                            //                leastValueObj.UserCode = "Console";
                            //                leastValueObj.OrderPrice = leastValueObj.FuturePriceStr + " | " + leastValueObj.CallPriceStr + " | " + leastValueObj.PutPriceStr;
                            //                greaterValueObj = item;

                            //            }
                            //            else
                            //            {

                            //                leastValueObj = item;
                            //                greaterValueObj = (SpreadLogModel)value;
                            //                greaterValueObj.UserCode = "Console";
                            //                leastValueObj.OrderPrice = greaterValueObj.FuturePriceStr + " | " + greaterValueObj.CallPriceStr + " | " + greaterValueObj.PutPriceStr;

                            //            }

                            //        }

                            //    }


                            //    /***till here we have got the element with which item[user1] has to be compared****/

                            //    SpreadLogModel element = new SpreadLogModel()
                            //    {
                            //        LogTimeStr = Convert.ToString(leastValueObj.LogTime) + " || " + Convert.ToString(greaterValueObj.LogTime),
                            //        ExchangeOrderIdStr = Convert.ToString(leastValueObj.ExchangeOrderId) + " || " + Convert.ToString(greaterValueObj.ExchangeOrderId),
                            //        OrderPrice = leastValueObj.OrderPrice + " || " + greaterValueObj.OrderPrice,
                            //        GreaterValue = greaterValueObj.UserCode,
                            //        // DateTime = DateTimeHelper.getDateTimeStrFromNSEEpochSeconds(item1.LogTime, "HH:mm:ss") + "||" + DateTimeHelper.getDateTimeStrFromNSEEpochSeconds(leastValueItem.LogTime, "HH:mm:ss"),
                            //        DateTime = DateTimeHelper.getDateTimeStrFromNSEEpochSeconds(item.LogTime, "HH:mm:ss"),
                            //        StrategyTag = leastValueObj.StrategyTag + " || " + greaterValueObj.StrategyTag,
                            //        MktAnalaysisParams = item.MktAnalaysisParams,
                            //        Difference = Math.Abs(Convert.ToInt32(leastValueObj.ExchangeOrderId - greaterValueObj.ExchangeOrderId))


                            //    };



                            //    if (Math.Abs(element.Difference) <= StaticVariables.DiffThreshold)
                            //        finalResultCollAftrComparison.Add(element);


                            //}

                            #endregion

                            /**********Fill Percentage Value*********/

                        }

                    }
                    #endregion

                    break;
                case "IsComparisonWithQuantum":

                    #region #When comparison is with Quantum 
                    foreach (var item in user1Data)
                    {
                        // Console.WriteLine(item.StrategyTag);

                        if (!IsStrictChecking)
                        {
                            //var coll = user2Data.Where(p => (p.LogTime == item.LogTime) && (p.StrategyTag == item.StrategyTag) && (p.Threshold == item.Threshold));
                            var coll = from p in user2Data where (p.LogTime == item.LogTime) && (p.StrategyTag == item.StrategyTag) && (p.Threshold == item.Threshold) select p;

                            if (coll.Any(p => (p.LogTime == item.LogTime) && (p.StrategyTag == item.StrategyTag) && (p.Threshold == item.Threshold)) == true)
                            {

                                foreach (var value in coll)
                                {
                                    if ((item.CallPrice == value.CallPrice) || (item.PutPrice == value.PutPrice) || (item.FuturePrice == value.FuturePrice))
                                    {
                                        if (item.ExchangeOrderId > value.ExchangeOrderId)
                                        {
                                            leastValueObj = (SpreadLogModel)value;
                                            greaterValueObj = item;

                                        }
                                        else
                                        {

                                            leastValueObj = item;
                                            greaterValueObj = (SpreadLogModel)value;

                                        }

                                    }

                                }


                                /***till here we have got the element with which item[user1] has to be compared****/

                                if (leastValueObj.UserCode.Contains(item.UserCode))
                                {
                                    user1ValueObj = (SpreadLogModel)leastValueObj;
                                    user2ValueObj = (SpreadLogModel)greaterValueObj;
                                }
                                else
                                {
                                    user1ValueObj = (SpreadLogModel)greaterValueObj;
                                    user2ValueObj = (SpreadLogModel)leastValueObj;
                                }




                                WinnerHighlightStruct winnerStruct = new WinnerHighlightStruct();
                                winnerStruct.participant1UserId = item.UserCode;
                                winnerStruct.winneruserId = user1ValueObj.ExchangeOrderId < user2ValueObj.ExchangeOrderId ? user1ValueObj.UserCode : user2ValueObj.UserCode;

                                SpreadLogModel element = new SpreadLogModel()
                                {
                                    LogTimeStr = Convert.ToString(user1ValueObj.LogTime) + " || " + Convert.ToString(user2ValueObj.LogTime),
                                    ExchangeOrderIdStr = Convert.ToString(user1ValueObj.ExchangeOrderId) + " || " + Convert.ToString(user2ValueObj.ExchangeOrderId),
                                    OrderPrice = user1ValueObj.OrderPrice + " || " + user2ValueObj.OrderPrice,
                                    GreaterValue = user1ValueObj.ExchangeOrderId < user2ValueObj.ExchangeOrderId ? user1ValueObj.UserCode : user2ValueObj.UserCode,
                                    // DateTime = DateTimeHelper.getDateTimeStrFromNSEEpochSeconds(item1.LogTime, "HH:mm:ss") + "||" + DateTimeHelper.getDateTimeStrFromNSEEpochSeconds(leastValueItem.LogTime, "HH:mm:ss"),
                                    DateTime = DateTimeHelper.getDateTimeStrFromNSEEpochSeconds(item.LogTime, "MM/dd/yyyy HH:mm:ss"),
                                    StrategyTag = user1ValueObj.StrategyTag + " || " + user2ValueObj.StrategyTag,
                                    MktAnalaysisParams = user1ValueObj.Mkt + " || " + user2ValueObj.Mkt,
                                    Difference = Math.Abs(Convert.ToInt32(user1ValueObj.ExchangeOrderId - user2ValueObj.ExchangeOrderId)),
                                    WinnerHighLightVal = winnerStruct


                                };




                                if (Math.Abs(element.Difference) <= StaticVariables.DiffThreshold)
                                    finalResultCollAftrComparison.Add(element);

                            }


                        }

                    }
                    #endregion

                    break;
            }

         

            return finalResultCollAftrComparison;

        }


        public string GetPercentage(int usercount, int total)
        {


            return string.Format(" {0: 0.00 %}", (double)((double)(usercount) / (double)(total)));

        }

        int user1Count;

        public int GetPercentage(ObservableCollection<SpreadLogModel> finalColl, string user1, out int user2Count)
        {
            user1Count = 0;
            user2Count = 0;
            foreach (var item in finalColl)
            {
                if (item.GreaterValue == user1)
                {
                    user1Count++;
                }
                else
                {
                    user2Count++;
                }

            }


            return user1Count;


            // return string.Format(" {0: 0.00 %}", (double)((double)(usercount) / (double)(total)));

        }




        #endregion


    }
}
