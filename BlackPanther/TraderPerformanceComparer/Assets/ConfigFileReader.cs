using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TraderPerformanceComparer.Assets
{
    public class ConfigFileReader
    {
        private amtDataBaseConfig _amtDataBaseConfiguration;
        private List<String> traderList;

        public List<String> GetTraderList
        {
            get { return traderList; }
        }

        #region amtDataBase value returns


        public string GetAmtdbName()
        {
            return _amtDataBaseConfiguration.DBName;
        }

        public string GetAmtHostName()
        {
            return _amtDataBaseConfiguration.Host;
        }
        public Int32 GetAmtPort()
        {
            return _amtDataBaseConfiguration.Port;
        }

        public string GetAmtUserName()
        {
            return _amtDataBaseConfiguration.User;
        }

        public string GetAmtUserPassword()
        {
            return _amtDataBaseConfiguration.Password;
        }
        #endregion

        public ConfigFileReader(string configFileNotation)
        {
            traderList = new List<string>();
            XmlDocument xdoc = new XmlDocument();
            _amtDataBaseConfiguration = new amtDataBaseConfig();

            try
            {
                xdoc.Load(configFileNotation);
                XmlNodeList xmlnode;
                xmlnode = xdoc.GetElementsByTagName("root");
                if (xmlnode.Count != 1)
                {
                    throw new Exception("Wrong config file: " + configFileNotation + " , One root element is required.");
                }
                XmlNode rootNode = xmlnode[0];
                int i = 0;

                for (i = 0; i < rootNode.ChildNodes.Count; ++i)
                {
                    XmlNode childNode = rootNode.ChildNodes[i];
                    /*Reading Config setting of TraderList*/
                    if (childNode.Name == "UserIds")
                    {
                        int j = 0;
                        for (j = 0; j < childNode.ChildNodes.Count; ++j)
                        {
                            if (childNode.ChildNodes[j].Name == "TraderId")
                            {

                                traderList.Add(childNode.ChildNodes[j].InnerText);


                            }
                        }
                    }
                    else if (childNode.Name == "amtDataBase")
                    {
                        int j = 0;
                        for (j = 0; j < childNode.ChildNodes.Count; ++j)
                        {
                            if (childNode.ChildNodes[j].Name == "dbName")
                            {
                                _amtDataBaseConfiguration.DBName = childNode.ChildNodes[j].InnerText;
                            }
                            else if (childNode.ChildNodes[j].Name == "host")
                            {
                                _amtDataBaseConfiguration.Host = childNode.ChildNodes[j].InnerText;
                            }
                            else if (childNode.ChildNodes[j].Name == "basicPort")
                            {
                                _amtDataBaseConfiguration.Port = Int32.Parse(childNode.ChildNodes[j].InnerText);
                            }
                            else if (childNode.ChildNodes[j].Name == "userName")
                            {
                                _amtDataBaseConfiguration.User = childNode.ChildNodes[j].InnerText;
                            }
                            else if (childNode.ChildNodes[j].Name == "passWord")
                            {
                                _amtDataBaseConfiguration.Password = childNode.ChildNodes[j].InnerText;
                            }

                        }
                    }


                }

            }
            catch (XmlException errMsg)
            {


            }
            catch (Exception ex)
            {

            }
        }
    }
    class amtDataBaseConfig
    {

        string _host;
        Int32 _port;
        string _dbName;
        string _user;
        string _password;


        public string Host
        {
            get { return _host; }
            set { _host = value; }
        }

        public Int32 Port
        {
            get { return _port; }
            set { _port = value; }
        }

        public string DBName
        {
            get { return _dbName; }
            set { _dbName = value; }
        }

        public string User
        {
            get { return _user; }
            set { _user = value; }
        }

        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

    }
}
