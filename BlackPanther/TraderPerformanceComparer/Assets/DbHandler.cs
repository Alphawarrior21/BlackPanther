using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Npgsql;
using System.Data.Odbc;
using System.Data;

namespace TraderPerformanceComparer.Assets
{
    interface IDbHandler
    {
        bool connect();
        void disConnect();
        bool isConnected();
    }

    public class DbHandler : IDbHandler
    {

        NpgsqlConnection conn;
        string _host;
        Int32 _port;
        string _dbName;
        string _user;
        string _password;

        public DbHandler(string host, Int32 port, string dbName, string user, string password)
        {
            IPAddress tmpHost;
            if (!IPAddress.TryParse(host, out tmpHost))
                throw new Exception("Invalid Ip address");
            _host = host;
            _port = port;
            _dbName = dbName;
            _user = user;
            _password = password;
        }
        public bool connect()
        {
            conn = new NpgsqlConnection("Server=" + _host +
               ";Database=" + _dbName +
               ";User Id=" + _user +
               ";Password=" + _password + 
                ";Timeout=120" +
                ";COMMANDTIMEOUT=120");



            conn.Open();

            return conn.State == ConnectionState.Open;
        }
        public void disConnect()
        {
            if (conn != null)
            {
                conn.Close();
            }

        }
        public bool isConnected()
        {
            return conn.State == ConnectionState.Open;
        }
        public DataView runNonTrasectionalQuery(string query)
        {
            try
            {
                NpgsqlCommand comm = new NpgsqlCommand(query, conn);
                var res = comm.ExecuteReader();
                var dt = new DataTable();
                dt.Load(res);
                return new DataView(dt);

            }
            catch (Npgsql.NpgsqlException ex)
            {
                this.disConnect();
                return null;

            }


        }
        public int runTrasectionalQuery(string query)
        {
            NpgsqlCommand comm = new NpgsqlCommand(query, conn);
            return comm.ExecuteNonQuery();
        }
        ~DbHandler()
        {
            disConnect();
        }

   
    }
}
