using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using System.Data.Odbc;
using System.Data;

namespace Walldisplay
{
    public class Database
    {
        private string HOST = "192.168.0.1";
        private string SERVICENUM = "turbo";
        private string SERVER = "ol_";
        private string DATABASE = "hppcdb";
        private string USER = "informix";
        private string PASSWORD = "password";
        private readonly log4net.ILog dbLogger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        OdbcConnection conn = new OdbcConnection();
        private string sqlCmdSitekey = "select * from systemparameters where parmname = 'LocalServerName';";
        private string sqlCmdCstaAdres = "select * from systemparameters where parmname = 'TeleSwitchInterfaceModuleHost';";

        private String Dsn;


        public Database()
        {

        }

        public void setDsn(string dsn)
        {
            Dsn = dsn;
        }

        public void MakeConnection()
        {
            dbLogger.Debug("MakeConnection before building connection string");
            conn.ConnectionString = "Dsn=" + Dsn;
            /// or build your ODBC connection from scratch
            /// +
            ///"Host=192.168.0.1;" +
            ///"Server=ol_;" +
            ///"Service=turbo;" +
            ///"Protocol=olsoctcp;" +
            ///"Database=hppcdb;" +
            ///"Uid=informix;" +
            ///"Pwd=password;";

            try
            {
                dbLogger.Debug("Makeconnection before open Database connection");

                conn.Open();
                dbLogger.Debug("Made Database connection!");


            }
            catch (OdbcException ex)
            {
                dbLogger.Error("error opening db connection : " + ex.ToString());
            }

        }

        public string getSitekey()
        {
            string retVal = "0";
            try
            {
                if (conn.State == ConnectionState.Open)
                {

                    OdbcCommand myCommand = new OdbcCommand(sqlCmdSitekey, conn);
                    OdbcDataReader myReader = myCommand.ExecuteReader();
                    myReader.Read();
                    retVal = myReader.GetString(3);
                }
                else { retVal = "-1"; }
            }
            catch (OdbcException ex) { dbLogger.Error("error getting sitekey : " + ex.ToString()); }
            return retVal;
        }

        public string getCstaAdres()
        {
            string retVal = "0";
            try
            {
                if (conn.State == ConnectionState.Open)
                {
                    OdbcCommand myCommand = new OdbcCommand(sqlCmdCstaAdres, conn);
                    OdbcDataReader myReader = myCommand.ExecuteReader();
                    myReader.Read();
                    retVal = myReader.GetString(2);
                }
                else { retVal = "-1"; }
            }
            catch (OdbcException ex) { dbLogger.Error("error getting sitekey : " + ex.ToString()); }

            return retVal;

        }

        public string getServerName()
        {
            string retVal = conn.DataSource;
            return retVal;
        }

        public void CloseConnection()
        {
            conn.Close();
            dbLogger.Debug("Closed Database connection!");
        }
    }

}
