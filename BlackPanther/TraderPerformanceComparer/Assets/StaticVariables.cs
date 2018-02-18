using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace TraderPerformanceComparer.Assets
{
    public static class StaticVariables
    {
        public static string userName_console = "Console";
        public static string RegexPattern_Time = @"(Time : .*?\,)";
        public static string RegexPattern_Symbol = @"(Symbol : .*?\,)";
        public static string RegexPattern_ID = @"(ID : .*?\,)";
        public static string RegexPattern_Type = @"(Type : .*?\,)";
        public static string RegexPattern_Option = @"(Option : .*?\,)";
        public static string RegexPattern_CallPrice = @"(Call Price : .*?\,)";
        public static string RegexPattern_PutPrice = @"(Put Price : .*?\,)";
        public static string RegexPattern_FuturePrice = @"(Future Price : .*?\,)";
        public static string RegexPattern_Spread = @"(Spread : .*?\,)";
        public static string RegexPattern_OrderNum = @"(Order No. : .*?\.0)";
        public static string RegexPattern_MktAnalysis = @"(TickType : .*?\ |)";
        public static string RegexPattern_FUTQ = @"(FUT : .*?\ |)";
        public static string RegexPattern_CALLQ = @"(CALL : .*?\ |)";
        public static string RegexPattern_PUTQ = @"(PUT : .*?\ )";
        public static int DiffThreshold = 200;
        public static string fileNtFndMsg= "File not found";
        //public static string RegexPattern_Time = @"(Time\s*:.*?\,)(\w*)";


    }
}
