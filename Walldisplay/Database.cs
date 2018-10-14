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
        private string sqlCmdQueueName = "select calltypename from calltypes where calltypekey = ";
        private string sqlCmdgroupName = "select virtualgroupname from virtualgroups where virtualgroupkey = ";
        private string sqlCmdaggregateName = "select aggregatename from aggregates where aggregatekey = ";
        private string sqlCmduserName = "select username from users where userkey = ";
        private string sqlCmduserLastName = "select userlastname from users where userkey = ";
        private string sqlCmduserFirstName = "select userfirstname from users where userkey = ";
        // unloading the keys
        private string sqlCmdQueueNameAll = "select calltypename,calltypekey from calltypes";
        private string sqlCmdgroupNameAll = "select virtualgroupname,virtualgroupkey from virtualgroups";
        private string sqlCmdaggregateNameAll = "select aggregatename,aggregatekey from aggregates";
        private string sqlCmduserNameAll = "select username,userkey from users";
        private string sqlCmduserLastNameAll = "select userlastname,userkey from users";
        private string sqlCmduserFirstNameAll = "select userfirstname,userkey from users";

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

        public string logAllKeys() {

            String retVal = "No DB Connection";
            try
            {
                if (conn.State == ConnectionState.Open)
                {
                    OdbcCommand myCommand = new OdbcCommand(sqlCmduserNameAll, conn);
                    OdbcDataReader myReader = myCommand.ExecuteReader();
                    while (myReader.Read())
                    {
                        if ( myReader.HasRows && !myReader.IsDBNull(1) && !myReader.IsDBNull(0) )
                        {
                            //string a = myReader.GetString(0);
                            //string b = Convert.ToString( myReader.GetInt32(1));
                            //dbLogger.Info("User key: " + b + " has username: " + a);
                            dbLogger.Info("User key: " + Convert.ToString(myReader.GetInt32(1)) + " has username: " + myReader.GetString(0) ) ;
                            retVal = myReader.GetString(0);
                        }
                        else
                        {
                            dbLogger.Error("error. user with key: " + Convert.ToString(myReader.GetInt32(1))  + " does not exist");
                            retVal = "UserDoesNotExist";
                        }
                    }
                    myCommand = new OdbcCommand(sqlCmdQueueNameAll, conn);
                    myReader = myCommand.ExecuteReader();
                    while (myReader.Read())
                    {
                        if (myReader.HasRows && !myReader.IsDBNull(1) && !myReader.IsDBNull(0) )
                        {   
                            dbLogger.Info("Queue key: " + Convert.ToString(myReader.GetInt32(1)) + " has Queue name: " + myReader.GetString(0));
                            retVal = myReader.GetString(0);
                        }
                        else
                        {
                            dbLogger.Error("error. queue with key: " + Convert.ToString(myReader.GetInt32(1)) + " does not exist");
                            retVal = "QueueDoesNotExist";
                        }
                    }
                    myCommand = new OdbcCommand(sqlCmdgroupNameAll, conn);
                    myReader = myCommand.ExecuteReader();
                    while (myReader.Read())
                    {
                        if (myReader.HasRows && !myReader.IsDBNull(1) && !myReader.IsDBNull(0))
                        {
                            dbLogger.Info("Group key: " + Convert.ToString(myReader.GetInt32(1)) + " has Group name: " + myReader.GetString(0));
                            retVal = myReader.GetString(0);
                        }
                        else
                        {
                            dbLogger.Error("error. group with key: " + Convert.ToString(myReader.GetInt32(1)) + " does not exist");
                            retVal = "GroupDoesNotExist";
                        }
                    }
                    myCommand = new OdbcCommand(sqlCmdaggregateNameAll, conn);
                    myReader = myCommand.ExecuteReader();
                    while (myReader.Read())
                    {
                        if (myReader.HasRows && !myReader.IsDBNull(1) && !myReader.IsDBNull(0))
                        {
                            dbLogger.Info("Aggregate key: " + Convert.ToString(myReader.GetInt32(1)) + " has Aggregate name: " + myReader.GetString(0));
                            retVal = myReader.GetString(0);
                        }
                        else
                        {
                            dbLogger.Error("error. Aggregate with key: " + Convert.ToString(myReader.GetInt32(1)) + " does not exist");
                            retVal = "AggregateDoesNotExist";
                        }
                    }


                }
                else { retVal = "NoDBConnection"; }
            }
            catch (OdbcException ex) { dbLogger.Error("error unloading userkeys : " + ex.ToString()); }
            return retVal;

        }

        public string getUserName(string Key)
        {
            String retVal = "Name";
            try {
                if (conn.State == ConnectionState.Open)
                { OdbcCommand myCommand = new OdbcCommand(sqlCmduserName + Key + ";", conn);
                  OdbcDataReader myReader = myCommand.ExecuteReader();
                  myReader.Read();
                    if (myReader.HasRows)
                    {
                        retVal = myReader.GetString(0);
                    }
                    else
                    {
                        dbLogger.Error("error. user with key: " + Key + " does not exist");
                        retVal = "UserDoesNotExist";
                    }
                }
                else { retVal = "NoDBConnection"; }
                }
            catch (OdbcException ex) { dbLogger.Error("error getting sitekey : " + ex.ToString()); }
            return retVal;

        }

        public string getQueueName(string Key)
        {
            String retVal = "Name";
            try
            { if (conn.State == ConnectionState.Open)
                { OdbcCommand myCommand = new OdbcCommand(sqlCmdQueueName + Key + ";", conn);
                  OdbcDataReader myReader = myCommand.ExecuteReader(); myReader.Read();
                    if (myReader.HasRows)
                    {
                        retVal = myReader.GetString(0);
                    }
                    else {
                        dbLogger.Error("error. queue with key: " + Key + " does not exist");
                        retVal = "QueueDoesNotExist";
                    }
                } else
                { retVal = "NoDBConnection"; }
            }
            catch (OdbcException ex) { dbLogger.Error("error getting sitekey : " + ex.ToString()); }
            return retVal;
        }

        public string getGroupName(string Key)
        {
            String retVal = "Name";
            try
            {
                if (conn.State == ConnectionState.Open)
                {
                    OdbcCommand myCommand = new OdbcCommand(sqlCmdgroupName + Key + ";", conn);
                    OdbcDataReader myReader = myCommand.ExecuteReader();
                    myReader.Read();
                    if (myReader.HasRows)
                    {
                        retVal = myReader.GetString(0);
                    }
                    else
                    {
                        dbLogger.Error("error. group with key: " + Key + " does not exist");
                        retVal = "GroupDoesNotExist";
                    }
                }
                else
                { retVal = "NoDBConnection"; }
            }
            catch (OdbcException ex) { dbLogger.Error("error getting sitekey : " + ex.ToString()); }
            return retVal;
        }


        public string getAggregateName(string Key)
        {
            String retVal = "Name";
            try
            {
                if (conn.State == ConnectionState.Open)
                {
                    OdbcCommand myCommand = new OdbcCommand(sqlCmdaggregateName + Key + ";", conn);
                    OdbcDataReader myReader = myCommand.ExecuteReader();
                    myReader.Read();
                    if (myReader.HasRows)
                    {
                        retVal = myReader.GetString(0);
                    }
                    else
                    {
                        dbLogger.Error("error. aggregate with key: " + Key + " does not exist");
                        retVal = "AggrDoesNotExist";
                    }
                    
                }
                else
                { retVal = "NoDBConnection"; }
            } catch (OdbcException ex) { dbLogger.Error("error getting sitekey : " + ex.ToString()); }
            return retVal;
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
