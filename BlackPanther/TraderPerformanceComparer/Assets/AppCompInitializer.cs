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
using System.Configuration;

namespace TraderPerformanceComparer.Assets
{
  public  class AppCompInitializer
    {
        #region #Private Field Variables
      //  private FileSearcher fileSearcher;
        private List<String> listOfParticipants;
        private ConfigFileReader configReader;
        private static string path = System.IO.Path.GetDirectoryName(
      System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
        private string pattern;
        private string fileLocation1;
        private string fileLocation2;
        #endregion

        #region #Properties

        public string FileLocation1
        {
            get { return fileLocation1; }
            set { fileLocation1 = value;}
        }

        public string FileLocation2
        {
            get { return fileLocation2; }
            set { fileLocation2 = value; }
        }
        public ConfigFileReader ConfigReader
        {
            get { return configReader; }
           
        }
       
        #endregion

        #region #commands and Command Handlers
        #endregion

        #region #Constructors
        public AppCompInitializer()
        {
            
            listOfParticipants = new List<string>();
            configReader = new ConfigFileReader(ConfigurationManager.AppSettings["startupConfig"]);
        }

        #endregion

        #region #Helper Methods
        public List<String> ReturnListOfParticipants()
        {
            if(configReader.GetTraderList.Count > 0)
            {
                return configReader.GetTraderList;
            }
            throw new ArgumentException("config Reader have empty Participant list");
        }
        public String GetFileLocation(string user)
        {
            if(user==StaticVariables.userName_console)
            {
               pattern= "*_" + user + ".txt";

            }
            if(user!= StaticVariables.userName_console)
            {
              pattern = "*_" + user + ".xml";
               
            }
            if(pattern!="")
            {
                path = new Uri(path).LocalPath;
                string[] files = System.IO.Directory.GetFiles(path, pattern, System.IO.SearchOption.TopDirectoryOnly);
                if (files.Length > 0)
                {
                    foreach (var item in files)
                    {
                        if (item.Contains(user))
                        {
                            return item;

                        }
                        else
                        {
                            return "File cannot be found for user";
                        }

                    }

                }
                else
                {
                    return StaticVariables.fileNtFndMsg;
                }
            }
            else
            {
                return "The type of file specified couldn't be found";
            }

           return StaticVariables.fileNtFndMsg;
            //throw new ArgumentException("File is not available for user " + user);
        }


        #endregion


    }
}
