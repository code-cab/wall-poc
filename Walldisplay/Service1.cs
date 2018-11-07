using IniParser;
using IniParser.Model;
using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Threading;
using log4net;
using System.Timers;
using HiPathProCenterLibrary;

namespace Walldisplay
{
    public partial class Service1 : ServiceBase
    {
        IDisposable webApp;

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            String url = args.FirstOrDefault(a => a.StartsWith("http"));
            if (url == null)
            {
                //url = "http://127.0.0.1:9000/";
                url = "http://172.16.0.41:9000/";
            }

            if (!Environment.UserInteractive) EventLog.WriteEntry("Walldisplay webinterface listening at: " + url, EventLogEntryType.Information);
            logger.Info("Listening at: " + url);
            webApp = WebApp.Start<WebStartup>(url);
            if (!Environment.UserInteractive) EventLog.WriteEntry("Walldisplay started", EventLogEntryType.Information);

            var parser = new FileIniDataParser();
            settingIniFile = parser.ReadFile("walldisplay.ini");

            //data.WaitingWarnLimit = Convert.ToInt32(ini["WallDisplay"]["WaitingCallsMax"]);
            //data.WaitingWarnLimit = Convert.ToInt32(settingIniFile["WallDisplay"]["WaitingCallsMax"]);
            // settingIniFile = new IniFile(@"d:\walldisplay\walldisplay.ini");
            checkLicense();
            //DataStore.instance.Data(get);
            if (validLicense)
            {
                SetupOsccConnection();
                timer1.Interval = 1000;
                timer1.Elapsed += new System.Timers.ElapsedEventHandler(Timer1_Elapsed);
                connectionChecktimer2.Interval = 5000;
                connectionChecktimer2.Elapsed += new System.Timers.ElapsedEventHandler(Timer2_Elapsed);
                connectionChecktimer2.Enabled = true;
            }
            else { logger.Info("Wallboard service not fully started due to invalid license"); }



        }

        protected override void OnStop()
        {
            webApp.Dispose();
            EventLog.WriteEntry("Walldisplay stopped", EventLogEntryType.Information);

            DisconnectOsccConnection();
            // TODO: Add code here to perform any tear-down necessary to stop your service.

        }

        public void ConsoleStart(string[] args)
        {
            OnStart(args);
        }

        public void ConsoleStop()
        {
            OnStop();
        }

        private static HiPathProCenterLibrary.HiPathProCenterManager objOSCC;
        private static HiPathProCenterLibrary.StatisticsManager objStatMan;
        private static HiPathProCenterLibrary.AdministrationManager objAdmin;
        private static HiPathProCenterLibrary.RoutingManager objRoutMan;
        private static HiPathProCenterLibrary.MediaManager objMediaMan;
        private static IniData settingIniFile;
        private static int numberOfWalldisplays = 0;
        private static int numberOfWalldisplayLayouts = 0;
        private static List<Walldisplay> wallDisplayList = new List<Walldisplay>();
        private static System.Timers.Timer timer1 = new System.Timers.Timer();
        private static System.Timers.Timer connectionChecktimer2 = new System.Timers.Timer();
        private static Boolean osccConnected = false;
        private static Boolean eventFiltersSet = false;
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static Boolean validLicense = true;

        [STAThread]

        private static void SetupOsccConnection()
        {
            logger.Info("Enter setupOsccConnection()");
            objOSCC = new HiPathProCenterManager();

            // check number of configured walldisplay to licensed amount of walldisplays            

            logger.Debug("number of Walldisplay configured in ini file " + Convert.ToInt32(settingIniFile["WallDisplay"]["numberOfWalldisplays"]));

            int numberWBfromIni = Convert.ToInt32(settingIniFile["WallDisplay"]["numberOfWalldisplays"]);
            logger.Debug("number of Walldisplay configured in ini file " + numberWBfromIni);
            if (numberWBfromIni > numberOfWalldisplays)
            {
                logger.Error("Number of configured walldiplays is greater then the number of licensed walldisplays.");
                logger.Error("Number of walldisplays set the licensed amount.");
                numberWBfromIni = numberOfWalldisplays;
            }
            // end check number of configured walldisplay to licensed amount of walldisplays

            /// create walldisplays
            for (int j = 1; j <= numberWBfromIni; j++)
            {
                wallDisplayList.Add(new Walldisplay(j));
                logger.Debug("Number of Walldisplays to create : " + numberWBfromIni + ". Just created wallboard nr: " + j );
            }
            /// end creating walldisplays


            /// initalize information on walldisplay
            for (int j = 1; j <= numberWBfromIni; j++)
            {
                wallDisplayList.ElementAt(j - 1).setUsersKeylist(settingIniFile["WB" + j]["UserKeys"]);
                wallDisplayList.ElementAt(j - 1).setGroupsKeylist(settingIniFile["WB" + j]["GroupKeys"]);
                wallDisplayList.ElementAt(j - 1).setAggregatesKeylist(settingIniFile["WB" + j]["AggregateKeys"]);
                wallDisplayList.ElementAt(j - 1).setQueuesKeylist(settingIniFile["WB" + j]["QueueKeys"]);
                wallDisplayList.ElementAt(j - 1).setView(settingIniFile["WB" + j]["View"]);
                wallDisplayList.ElementAt(j - 1).initializeWallBoard();
                logger.Debug("WB initialized with nr: " + j);
            }
            /// initalize information on walldisplay

            /// connect to oscc
            setOsccConnection();
            /// end connection to oscc
            /// 
            ///



            /// set listen filters and store query id
            if (osccConnected)
            {
                logger.Debug("Going to set Eventfiltes ");
                setEventFilters();
            }
            else { logger.Info("No event filters set because OSCC is not connected"); }
        ///objOSCC.ListenForEvents(enHiPathProCenterEventTypes.HiPathProCenterEventType_ServerStateChangedEvents);

        /// end set listen filters and store query id
        /// subscribe events
        /// end /// subscribe events
        ///logger.Debug("statistic state : " + objStatMan.State.ToString());



    }

        private static void setOsccConnection()
        {
            try
            {
                //objOSCC.Initialize("6000@192.168.30.58", enEventModes.EventMode_FireEvents);
                logger.Info("oscc server string = '" + settingIniFile["WallDisplay"]["osccServer"] + "'");
                objOSCC.Initialize(settingIniFile["WallDisplay"]["osccServer"], enEventModes.EventMode_FireEvents);
                osccConnected = true;                
                logger.Info("Info: osscConnected : " + osccConnected);
            }
            catch
            {
                logger.Info("Error openning connection to oscc server");
                osccConnected = false;
            }
            try
            {
                objOSCC.Logon(Convert.ToInt32(settingIniFile["WallDisplay"]["LogonUserKey"]), settingIniFile["WallDisplay"]["password"]);
                objStatMan = (HiPathProCenterLibrary.StatisticsManager)objOSCC.HireStatisticsManager(enEventModes.EventMode_FireEvents);
                objAdmin = (HiPathProCenterLibrary.AdministrationManager)objOSCC.HireAdministrationManager(enEventModes.EventMode_FireEvents);
                objRoutMan = (HiPathProCenterLibrary.RoutingManager)objOSCC.HireRoutingManager(enEventModes.EventMode_FireEvents);
                objMediaMan = (HiPathProCenterLibrary.MediaManager)objOSCC.HireMediaManager(enEventModes.EventMode_FireEvents);
                logger.Debug("Stat funct state Hist     : " + objStatMan.GetFunctionalityState(enStatisticsFunctionalities.StatisticsFunctionality_Historical));
                logger.Debug("Stat funct state Network  : " + objStatMan.GetFunctionalityState(enStatisticsFunctionalities.StatisticsFunctionality_Network));
                logger.Debug("Stat funct state realtime : " + objStatMan.GetFunctionalityState(enStatisticsFunctionalities.StatisticsFunctionality_RealTime));
                logger.Debug("Admin funct state Database    : " + objAdmin.GetFunctionalityState(enAdministrationFunctionalities.AdministrationFunctionality_AdministrationDatabase));
                logger.Debug("Admin funct state config sync : " + objAdmin.GetFunctionalityState(enAdministrationFunctionalities.AdministrationFunctionality_ConfigurationSynchronization));
                logger.Debug("Media funct state Voice    : " + objMediaMan.GetFunctionalityState(enMediaFunctionalities.MediaFunctionality_Voice));
                logger.Debug("Media funct state email    : " + objMediaMan.GetFunctionalityState(enMediaFunctionalities.MediaFunctionality_Email));
                logger.Debug("Media funct state callback : " + objMediaMan.GetFunctionalityState(enMediaFunctionalities.MediaFunctionality_Callback));
                logger.Debug("Media funct state webcol   : " + objMediaMan.GetFunctionalityState(enMediaFunctionalities.MediaFunctionality_WebCollaboration));
                logger.Debug("Routing funct state realtime   : " + objRoutMan.GetFunctionalityState(enRoutingFunctionalities.RoutingFunctionality_RealTime));
                logger.Debug("Routing funct state local rout : " + objRoutMan.GetFunctionalityState(enRoutingFunctionalities.RoutingFunctionality_LocalRouting));
                logger.Debug("Routing funct state netw rout  : " + objRoutMan.GetFunctionalityState(enRoutingFunctionalities.RoutingFunctionality_NetworkRouting));

                logger.Debug(" objstat  state : " + objStatMan.State.ToString());
                logger.Debug(" objmedia state : " + objMediaMan.State.ToString());
                logger.Debug(" objRout  state : " + objRoutMan.State.ToString());
                logger.Debug(" objAdmin state : " + objAdmin.State.ToString());

            }
            catch
            {
                logger.Error("Error to logon to oscc server");
            }


        }

        private static void setEventFilters()
        {
            try
            {

                foreach (Walldisplay wb in wallDisplayList)
                {
                    //wb.setAggregatesRTQueryID(objStatMan.ListenForEvents(enStatisticsEventTypes.StatisticsEventType_AggregateRealtimeEvent, wb.getAggregatesKeylist()));
                    //wb.setQueuesRTQueryID(objStatMan.ListenForEvents(enStatisticsEventTypes.StatisticsEventType_QueueRealtimeEvent, wb.getQueuesKeylist()));
                    wb.setUsersRTQueryID(objStatMan.ListenForEvents(enStatisticsEventTypes.StatisticsEventType_UserRealtimeEvent, wb.getUsersKeylist()));
                    //wb.setGroupsRTQueryID(objStatMan.ListenForEvents(enStatisticsEventTypes.StatisticsEventType_GroupRealtimeEvent, wb.getGroupsKeylist()));
                    // midnight of the running current
                    DateTime startTimeCumDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
                    //wb.setAggregatesCumQueryID(objStatMan.ListenForEvents(enStatisticsEventTypes.StatisticsEventType_AggregateCumulativeEvent, wb.getAggregatesKeylist(),startTimeCumDate));
                    //wb.setQueuesCumQueryID(objStatMan.ListenForEvents(enStatisticsEventTypes.StatisticsEventType_QueueCumulativeEvent, wb.getQueuesKeylist(),startTimeCumDate));
                    //wb.setGroupsCumQueryID(objStatMan.ListenForEvents(enStatisticsEventTypes.StatisticsEventType_GroupCumulativeEvent, wb.getGroupsKeylist()));
                    //wb.setUsersCumQueryID(objStatMan.ListenForEvents(enStatisticsEventTypes.StatisticsEventType_UserCumulativeEvent, wb.getUsersKeylist()));

                }
                eventFiltersSet = true;
                objOSCC.EventOccurred += ObjOSCC_EventOccurred;
                objStatMan.EventOccurred += ObjStatMan_EventOccurred;

            }
            catch
            {
                logger.Error("Fout by zetten van events ");
                eventFiltersSet = false;
                logger.Error("Stat funct state realtime : " + objStatMan.GetFunctionalityState(enStatisticsFunctionalities.StatisticsFunctionality_RealTime));
            }

        }

        private static void ObjStatMan_EventOccurred(StatisticsEvent StatisticsEvent)
        {
            logger.Debug("procenter statistic event occured with type: " + StatisticsEvent.ObjectType.ToString());
            switch (StatisticsEvent.ObjectType)
            {
                case enStatisticsEventObjectTypes.StatisticsEventObjectType_ManagerStateChanged:
                    HiPathProCenterLibrary.ManagerStateChangedEvent oManagerStateChangedEvent = (HiPathProCenterLibrary.ManagerStateChangedEvent)StatisticsEvent;
                    logger.Debug("procenter statistic event manager state " + oManagerStateChangedEvent.State);
                    ///oManagerStateChangedEvent.State;
                    break;
                case enStatisticsEventObjectTypes.StatisticsEventObjectType_AggregateRealtime:
                    HiPathProCenterLibrary.AggregateRealtimeEvent oAggregateRealtimeEvent = (HiPathProCenterLibrary.AggregateRealtimeEvent)StatisticsEvent;
                    logger.Debug("procenter statistic event RT Aggregate " + oAggregateRealtimeEvent.ToString());
                    foreach (Walldisplay wb in wallDisplayList)
                    {
                        logger.Debug("procenter statistic event RT Aggregate wb queryID " + wb.getAggregatesRTQueryID());
                        logger.Debug("procenter statistic event RT Aggregate event query ID " + oAggregateRealtimeEvent.QueryID);
                        if (oAggregateRealtimeEvent.QueryID == wb.getAggregatesRTQueryID())
                        {
                            logger.Debug("procenter statistic event RT Aggregate event set to WB with number: " + wb.getAggregatesRTQueryID());
                            wb.setAggregateRTevent(oAggregateRealtimeEvent);
                        }
                    }

                    break;
                case enStatisticsEventObjectTypes.StatisticsEventObjectType_UserRealtime:
                    HiPathProCenterLibrary.UserRealtimeEvent oUserRealtimeEvent = (HiPathProCenterLibrary.UserRealtimeEvent)StatisticsEvent;
                    logger.Debug("procenter statistic event user realtime :" + oUserRealtimeEvent.ToString());
                    logger.Debug("procenter statistic event user realtime :" + oUserRealtimeEvent.Count);
                    foreach (Walldisplay wb in wallDisplayList)
                    {
                        if (oUserRealtimeEvent.QueryID == wb.getUsersRTQueryID())
                        {
                            logger.Debug("procenter statistic event user realtime  :" + oUserRealtimeEvent.ToString());
                            wb.setUserRTevent(oUserRealtimeEvent);
                        }
                    }
                    break;
                case enStatisticsEventObjectTypes.StatisticsEventObjectType_QueueRealtime:
                    HiPathProCenterLibrary.QueueRealtimeEvent oQueueRealtimeEvent = (HiPathProCenterLibrary.QueueRealtimeEvent)StatisticsEvent;
                    logger.Debug("procenter stat event queue realtime :" + oQueueRealtimeEvent.QueryID);
                    foreach (Walldisplay wb in wallDisplayList)
                    {
                        if (oQueueRealtimeEvent.QueryID == wb.getQueuesRTQueryID())
                        {
                            wb.setQueueRTevent(oQueueRealtimeEvent);
                        }
                    }
                    break;
                case enStatisticsEventObjectTypes.StatisticsEventObjectType_GroupRealtime:
                    HiPathProCenterLibrary.GroupRealtimeEvent oGroupRealtimeEvent = (HiPathProCenterLibrary.GroupRealtimeEvent)StatisticsEvent;
                    logger.Debug("procenter stat event queue realtime :" + oGroupRealtimeEvent.QueryID);
                    foreach (Walldisplay wb in wallDisplayList)
                    {
                        if (oGroupRealtimeEvent.QueryID == wb.getGroupsRTQueryID())
                        {
                            wb.setGroupRTevent(oGroupRealtimeEvent);
                        }
                    }
                    break;
                case enStatisticsEventObjectTypes.StatisticsEventObjectType_QueueCumulative:
                    HiPathProCenterLibrary.QueueCumulativeEvent oQueuesCumEvent = (HiPathProCenterLibrary.QueueCumulativeEvent)StatisticsEvent;
                    logger.Info("procenter stat event queue Cumulative :" + oQueuesCumEvent.QueryID);
                    foreach (Walldisplay wb in wallDisplayList)
                    {
                        if (oQueuesCumEvent.QueryID == wb.getQueuesCumQueryID())
                        {
                            wb.setQueuesCumEvent(oQueuesCumEvent);
                        }
                    }
                    break;
                case enStatisticsEventObjectTypes.StatisticsEventObjectType_UserCumulative:
                    HiPathProCenterLibrary.UserCumulativeEvent oUsersCumEvent = (HiPathProCenterLibrary.UserCumulativeEvent)StatisticsEvent;
                    logger.Info("procenter stat event users Cumulative :" + oUsersCumEvent.QueryID);
                    foreach (Walldisplay wb in wallDisplayList)
                    {
                        if (oUsersCumEvent.QueryID == wb.getUsersCumQueryID())
                        {
                            //wb.setUsersCumEvent(oUsersCumEvent);
                        }
                    }
                    break;
                case enStatisticsEventObjectTypes.StatisticsEventObjectType_GroupCumulative:
                    HiPathProCenterLibrary.GroupCumulativeEvent oGroupsCumEvent = (HiPathProCenterLibrary.GroupCumulativeEvent)StatisticsEvent;
                    logger.Info("procenter stat event groups Cumulative :" + oGroupsCumEvent.QueryID);
                    foreach (Walldisplay wb in wallDisplayList)
                    {
                        if (oGroupsCumEvent.QueryID == wb.getGroupsCumQueryID())
                        {
                            wb.setGroupsCumEvent(oGroupsCumEvent);
                        }
                    }
                    break;
                case enStatisticsEventObjectTypes.StatisticsEventObjectType_AggregateCumulative:
                    HiPathProCenterLibrary.AggregateCumulativeEvent oAggregatesCumEvent = (HiPathProCenterLibrary.AggregateCumulativeEvent)StatisticsEvent;
                    logger.Info("procenter stat event groups Cumulative :" + oAggregatesCumEvent.QueryID);
                    foreach (Walldisplay wb in wallDisplayList)
                    {
                        if (oAggregatesCumEvent.QueryID == wb.getAggregatesCumQueryID())
                        {
                            wb.setAggregatesCumEvent(oAggregatesCumEvent);
                        }
                    }
                    break;
                default:
                    ///MsgBox "Unknown ObjectType";
                    logger.Error("stat event default unimplementated object type : " + StatisticsEvent.GetType());
                    break;
            }

            ///throw new NotImplementedException();
        }

        private static void ObjOSCC_EventOccurred(HiPathProCenterEvent HiPathProCenterEvent)
        {
            switch (HiPathProCenterEvent.ObjectType)
            {
                case enHiPathProCenterEventObjectTypes.HiPathProCenterEventObjectType_ServerStateChanged:
                    HiPathProCenterLibrary.HiPathProCenterEvent oServerStartChangedEvent = (HiPathProCenterLibrary.HiPathProCenterEvent)HiPathProCenterEvent;
                    ///throw new NotImplementedException();    
                    //Console.WriteLine("procenter event: " + oServerStartChangedEvent.ToString());
                    logger.Info("procenter event: " + oServerStartChangedEvent.ToString());
                    break;
                default:
                    ///throw new NotImplementedException();
                    logger.Debug("procenter event default. event not processed.");
                    break;
            }
        }

        public static void DisconnectOsccConnection()
        {
            logger.Debug("disconnect function");
            if (eventFiltersSet)
            {
                foreach (Walldisplay wb in wallDisplayList)
                {
                    objStatMan.StopListeningForEvents(wb.getAggregatesRTQueryID());
                    objStatMan.StopListeningForEvents(wb.getQueuesRTQueryID());
                    objStatMan.StopListeningForEvents(wb.getUsersRTQueryID());
                    objStatMan.StopListeningForEvents(wb.getGroupsRTQueryID());
                    objStatMan.StopListeningForEvents(wb.getAggregatesCumQueryID());
                    objStatMan.StopListeningForEvents(wb.getGroupsCumQueryID());
                    objStatMan.StopListeningForEvents(wb.getQueuesCumQueryID());
                    objStatMan.StopListeningForEvents(wb.getUsersCumQueryID());

                }

                objOSCC.EventOccurred -= ObjOSCC_EventOccurred;
                objStatMan.EventOccurred -= ObjStatMan_EventOccurred;
            }
            timer1.Elapsed -= Timer1_Elapsed;
            timer1.Enabled = false;
            connectionChecktimer2.Enabled = false;
            connectionChecktimer2.Elapsed -= Timer2_Elapsed;
            // objMediaMan = null; ???
            logger.Debug("stoped listening to events");
        }

        private static void checkLicense()
        {

        //    License wbLicense = new License();
        //    wbLicense.readLicense();
        //    logger.Info("Read license Base: " + wbLicense.getBaseLicense());
        //    logger.Info("Read number of WB: " + wbLicense.getNumberOfWalldisplays());
        //    logger.Info("Read number of layout: " + wbLicense.getNumberOflayouts());
        //    logger.Info("Read number of layout: " + wbLicense.getLicencedVersion());
        //    logger.Info("Read UID from license file: " + wbLicense.getUID());


            Database osccDB = new Database();

            logger.Info("OSCC ODBC settings: " + Convert.ToString(settingIniFile.GetKey("DSN")));

            osccDB.setDsn(settingIniFile["ODBC"]["DSN"]);
            osccDB.MakeConnection();
            logger.Debug("OSCC sitekey : " + osccDB.getSitekey());
            logger.Debug("OSCC cstaAdress: " + osccDB.getCstaAdres());
            logger.Debug("OSCC db servername: " + osccDB.getServerName());
            //logger.Info("logging all keys for convenience: " + osccDB.logAllKeys());

            string tempAppName = osccDB.getSitekey();
            tempAppName += osccDB.getServerName();
            tempAppName += osccDB.getCstaAdres();
           logger.Debug("contructed appname: " + tempAppName);
        //    wbLicense.setAppName(tempAppName);

            // check if data was retrieved from database
        //    if (osccDB.getSitekey() == "-1")
        //    {
        //        logger.Error("License UID could not be determined. Check ODBC connection.");
        //        validLicense = false;
        //    }

        //    else
        //    {
                // check if uuid is the same
        //        validLicense = wbLicense.doValidation();
        //        logger.Info("UID calculated from library: " + LicenseHandler.GenerateUID(tempAppName));
        //    }

        //    logger.Debug("Is license for this system : " + validLicense);

            // assign license amounts
            if (validLicense)
            {
                //  numberOfWalldisplays = wbLicense.getNumberOfWalldisplays();
                //  numberOfWalldisplayLayouts = wbLicense.getNumberOflayouts();
                numberOfWalldisplays = 10;
                numberOfWalldisplayLayouts = 1;

            }
            // assign license amounts

                osccDB.CloseConnection();

        }

        private static void Timer1_Elapsed(object sender, ElapsedEventArgs e)
        {
            logger.Debug("entering timer event");
            foreach (Walldisplay wb in wallDisplayList)
            {
                // Below line is obsolete, the frontend retrieves data from datastore on intervals
                //wb.updateWallBoard();
                logger.Debug("timer event method in for each loop");
            }
        }

        private static void Timer2_Elapsed(object sender, ElapsedEventArgs e)
        {
            logger.Debug("Entering timer2 event check connection");
            //Console.WriteLine("write value from ini file" + settingIniFile.Read("QueuesKeys", "WBX") );
            if (settingIniFile["WallDisplay"]["liveUpdate"] == "true")
            {
                settingIniFile["WallDisplay"]["liveUpdate"] = "done";              
                logger.Info("Live update executed");
            }

            if (osccConnected)
            {
                if (eventFiltersSet) { timer1.Enabled = true; }
                else
                { setEventFilters(); }
            }
            else
            {
                setOsccConnection();
            }


        }


    }
}
