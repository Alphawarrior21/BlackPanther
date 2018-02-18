using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using TraderPerformanceComparer.ViewModel;
using TraderPerformanceComparer.Assets;

namespace TraderPerformanceComparer.Model
{
   public class SpreadLogModel :ViewModelBase
    {

        private string symbol;
        public string Symbol
        {
            get { return symbol; }
            set { symbol = value; OnPropertyChanged("Symbol"); }
        }

        private bool isThisRowMarkedAsFinal;
        public bool IsThisRowMarkedAsFinal
        {
            get { return isThisRowMarkedAsFinal; }
            set { isThisRowMarkedAsFinal = value;OnPropertyChanged("IsThisRowMarkedAsFinal"); }
        }

        private double strike;
        public double Strike
        {
            get { return strike; }
            set { strike = value;OnPropertyChanged("Strike"); }
        }

        private int strategyid;
        public int StrategyID
        {
            get { return strategyid; }
            set { strategyid = value;  OnPropertyChanged( "StrategyID"); }
        }

        private string strategytype;
        public string StrategyType
        {
            get { return strategytype; }
            set
            {
                strategytype = value;OnPropertyChanged( "StrategyType");
            }
        }

        private string strategytag;
        public string StrategyTag
        {
            get { return strategytag; }
            set { strategytag = value;OnPropertyChanged( "StrategyTag"); }
        }


        private int spreadqty;
        public int SpreadQty
        {
            get { return spreadqty; }
            set { spreadqty = value;OnPropertyChanged( "SpreadQty"); }
        }

        private double spreadprice;
        public double SpreadPrice
        {
            get { return spreadprice; }
            set { spreadprice = value;OnPropertyChanged( "SpreadPrice"); }
        }

        private string spreadPriceStr;
        public string SpreadPriceStr
        {
            get { return spreadPriceStr; }
            set { spreadPriceStr = value;OnPropertyChanged( "SpreadPriceStr"); }
        }

        private int logtime;
        public int LogTime
        {
            get { return logtime; }
            set { logtime = value;OnPropertyChanged( "LogTime"); }
        }

        private string side;
        public string Side
        {
            get { return side; }
            set { side = value;OnPropertyChanged( "Side"); }
        }

        private string datetime;
        public string DateTime
        {
            get { return datetime; }
            set { datetime = value;OnPropertyChanged( "DateTime"); }
        }

        private float threshold;
        public float Threshold
        {
            get { return threshold; }
            set { threshold = value;OnPropertyChanged( "Threshold"); }
        }

        private Int64 _exchangeOrderId;
        public Int64 ExchangeOrderId
        {
            get { return _exchangeOrderId; }
            set { _exchangeOrderId = value;OnPropertyChanged( "ExchangeOrderId"); }
        }



        private string _hybridID;
        public string HybridID
        {
            get { return _hybridID; }
            set { _hybridID = value;OnPropertyChanged( "HybridID"); }
        }

        private string _spreadState;

        public string SpreadState
        {
            get { return _spreadState; }
            set { _spreadState = value;OnPropertyChanged( "SpreadState"); }
        }

        private Int16 userID;
        public Int16 UserID
        {
            get { return userID; }
            set { userID = value;OnPropertyChanged( "UserID"); }
        }


        private string orderPrice;
        public string OrderPrice
        {
            get { return orderPrice; }
            set { orderPrice = value;OnPropertyChanged( "OrderPrice"); }
        }

        private string _mktAnalaysisParams;
        public string MktAnalaysisParams
        {
            get { return _mktAnalaysisParams; }
            set { _mktAnalaysisParams = value;OnPropertyChanged( "MktAnalaysisParams"); }
        }

        private char _mkt;
        public char Mkt
        {
            get { return _mkt; }
            set { _mkt = value; OnPropertyChanged("Mkt"); }
        }

        private string userCode;
        public string UserCode
        {
            get { return userCode; }
            set { userCode = value;OnPropertyChanged( "UserCode"); }
        }

        /********************************************CompareResultSet***************************************************/


        private long difference;
        public long Difference
        {
            get { return difference; }
            set { difference = value; OnPropertyChanged("Difference"); }
        }

        private string greaterValue;
        public string GreaterValue
        {
            get { return greaterValue; }
            set
            {
                greaterValue = value; OnPropertyChanged("GreaterValue");
            }
        }


        private string spreadqtyStr;
        public string SpreadQtyStr
        {
            get { return spreadqtyStr; }
            set { spreadqtyStr = value; OnPropertyChanged( "SpreadQty"); }
        }

        private string logtimeStr;
        public string LogTimeStr
        {
            get { return logtimeStr; }
            set { logtimeStr = value; OnPropertyChanged( "LogTimeStr"); }
        }


        private string thresholdStr;
        public string ThresholdStr
        {
            get { return thresholdStr; }
            set { thresholdStr = value; OnPropertyChanged( "ThresholdStr"); }
        }




        private string exchangeOrderIdStr;
        public string ExchangeOrderIdStr
        {
            get { return exchangeOrderIdStr; }
            set { exchangeOrderIdStr = value; OnPropertyChanged("ExchangeOrderIdStr"); }
        }

        private string userIDStr;
        public string UserIDStr
        {
            get { return userIDStr; }
            set { userIDStr = value; OnPropertyChanged( "UserIDStr"); }
        }


        private string orderPriceStr;
        public string OrderPriceStr
        {
            get { return orderPriceStr; }
            set { orderPriceStr = value; OnPropertyChanged( "OrderPriceStr"); }
        }

        /****************************************ConsoleSpecific Parameters**********************************************************************/

        private int id_console;
        public int ID_Console
        {
            get { return id_console; }
            set { id_console = value; OnPropertyChanged( "ID_Console"); }
        }
        private string optionType;
        public string OptionType
        {
            get { return optionType; }
            set { optionType = value; OnPropertyChanged("OptionType"); }
        }


        private double callPrice;
        public double CallPrice
        {
            get { return callPrice; }
            set { callPrice = value; OnPropertyChanged("CallPrice"); }
        }

        private string callPriceStr;
        public string CallPriceStr
        {
            get { return callPriceStr; }
            set { callPriceStr = value; OnPropertyChanged("CallPriceStr"); }
        }

        private double putPrice;
        public double PutPrice
        {
            get { return putPrice; }
            set { putPrice = value; OnPropertyChanged("PutPrice"); }
        }

        private string putPriceStr;
        public string PutPriceStr
        {
            get { return putPriceStr; }
            set { putPriceStr = value; OnPropertyChanged("PutPriceStr"); }
        }



        private double futurePrice;
        public double FuturePrice
        {
            get { return futurePrice; }
            set { futurePrice = value; OnPropertyChanged("FuturePrice"); }
        }

        private string futurePriceStr;
        public string FuturePriceStr
        {
            get { return futurePriceStr; }
            set { futurePriceStr = value; OnPropertyChanged("FuturePriceStr"); }
        }

        private WinnerHighlightStruct winnerHighLightVal;
        public WinnerHighlightStruct WinnerHighLightVal
        {
            get { return winnerHighLightVal; }
            set { winnerHighLightVal = value;OnPropertyChanged("WinnerHighLightVal"); }
        }




    }
}
