using IniParser;
using IniParser.Model;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HiPathProCenterLibrary;
using System.IO;
using log4net;


namespace Walldisplay
{
    public class Walldisplay
    {
        private int queuesRTQueryID;
        private int usersRTQueryID;
        private int aggregatesRTQueryID;
        private int groupsRTQueryID;
        private int queuesCumQueryID;
        private int usersCumQueryID;
        private int aggregatesCumQueryID;
        private int groupsCumQueryID;
        private List<int> queuesList = new List<int>();
        private List<int> usersList = new List<int>();
        private List<int> aggregatesList = new List<int>();
        private List<int> groupsList = new List<int>();
        private string wbView = string.Empty;
        private HiPathProCenterLibrary.AggregateRealtimeEvent aggregateRTEv;
        private HiPathProCenterLibrary.UserRealtimeEvent userRTEv;
        private HiPathProCenterLibrary.QueueRealtimeEvent queueRTEv;
        private HiPathProCenterLibrary.GroupRealtimeEvent groupRTEv;
        private HiPathProCenterLibrary.QueueCumulativeEvent queueCumEv;
        private HiPathProCenterLibrary.GroupCumulativeEvent groupCumEv;
        private HiPathProCenterLibrary.UserCumulativeEvent userCumEv;
        private HiPathProCenterLibrary.AggregateCumulativeEvent aggregateCumEv;
        private int WallDisplayID;
        private ConcurrentDictionary<int, QueueKey> queuesDict = new ConcurrentDictionary<int, QueueKey>();
        private ConcurrentDictionary<int, UserKey> usersDict = new ConcurrentDictionary<int, UserKey>();
        private ConcurrentDictionary<int, AggregateKey> aggregatesDict = new ConcurrentDictionary<int, AggregateKey>();
        private ConcurrentDictionary<int, GroupKey> groupsDict = new ConcurrentDictionary<int, GroupKey>();
        private Dictionary<string, string> agentStatusDict = new Dictionary<string, string>
                                       {
                                        { "RoutingState_Available", "IDLE" },
                                        { "RoutingState_Unknown", "IDLE" },
                                        { "RoutingState_Unavailable", "Away" },
                                        { "RoutingState_Work", "Active" },
                                        { "HandlingState_Ringing", "Ringing" },
                                        { "HandlingState_Holding", "Hold" },
                                        { "HandlingState_Talking", "Active" }
                                       };


        private readonly log4net.ILog wbLogger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public Walldisplay(int WalldisplayID)
        {
            if ((WalldisplayID > 0) && (WalldisplayID < 50)) { WallDisplayID = WalldisplayID; }

        }

        public void updateWallBoard()
        {
            KeysData initKeysData = new KeysData();

            initKeysData.UserKeys = usersDict.Values.ToArray();
            initKeysData.GroupKeys = groupsDict.Values.ToArray();
            initKeysData.AggregateKeys = aggregatesDict.Values.ToArray();
            initKeysData.QueueKeys = queuesDict.Values.ToArray();
            initKeysData.View = wbView;
            DataStore.Instance.dataDictionary.AddOrUpdate("WB" + WallDisplayID, initKeysData, (k, v) => initKeysData);

        }

        public void initializeWallBoard()
        {
            KeysData initKeysData = new KeysData();

            Database osccDB = new Database();
            var parser = new FileIniDataParser();
            IniData settingIniFile = parser.ReadFile("walldisplay.ini");

            wbLogger.Info("OSCC ODBC settings: " + Convert.ToString(settingIniFile.GetKey("DSN")));
            osccDB.setDsn(settingIniFile["ODBC"]["DSN"]);
            osccDB.MakeConnection();

            foreach (int i in groupsList)
            {
                groupsDict.AddOrUpdate(i, new GroupKey(), (v, k) => new GroupKey());
                // belowline should should be replaced by DB query
                groupsDict[i].GroupName = osccDB.getGroupName(Convert.ToString(i));
                groupsDict[i].LoggedOn = 1;
                groupsDict[i].OnRoutedCall = 1;
                groupsDict[i].OnDirectCall = 1;
                groupsDict[i].Idle = 2;
                groupsDict[i].Busy = 1;
                groupsDict[i].Away = 2;
                groupsDict[i].CallsWaiting = 0;
                wbLogger.Info("GroupKey name: " + groupsDict[i].GroupName + " of walldiplay nr: " + WallDisplayID);
            }

            foreach (int i in usersList)
            {

                usersDict.AddOrUpdate(i, new UserKey(), (v, k) => new UserKey());
                // belowline should should be replaced by DB query
                usersDict[i].Name = osccDB.getUserName(Convert.ToString(i));
                usersDict[i].State = "Away";
                usersDict[i].DurationSec = 0;
                wbLogger.Info("UserKey name: " + usersDict[i].Name + " of walldiplay nr: " + WallDisplayID);
            }



            foreach (int i in aggregatesList)
            {
                aggregatesDict.AddOrUpdate(i, new AggregateKey(), (v, k) => new AggregateKey());
                // belowline should should be replaced by DB query
                aggregatesDict[i].GroupName = osccDB.getAggregateName(Convert.ToString(i));
                aggregatesDict[i].Received = 0;
                aggregatesDict[i].Abandoned = 0;
                aggregatesDict[i].Answered = 0;
                aggregatesDict[i].AvgWaitingTimeSec = 0;
                aggregatesDict[i].MaxWaitingTimeSec = 0;
                aggregatesDict[i].ServiceLevelPerc = 0;
                wbLogger.Info("AggregateKey name: " + aggregatesDict[i].GroupName + " of walldiplay nr: " + WallDisplayID);
            }

            foreach (int i in queuesList)
            {
                queuesDict.AddOrUpdate(i, new QueueKey(), (v, k) => new QueueKey());
                // belowline should should be replaced by DB query
                queuesDict[i].GroupName = osccDB.getQueueName(Convert.ToString(i));
                queuesDict[i].Received = 0;
                queuesDict[i].Abandoned = 0;
                queuesDict[i].Answered = 0;
                queuesDict[i].AvgWaitingTimeSec = 0;
                queuesDict[i].MaxWaitingTimeSec = 0;
                queuesDict[i].ServiceLevelPerc = 0;
                wbLogger.Info("queueKey name: " + queuesDict[i].GroupName + " of walldiplay nr: " + WallDisplayID);
            }

            osccDB.CloseConnection();
            initKeysData.UserKeys = usersDict.Values.ToArray();
            initKeysData.GroupKeys = groupsDict.Values.ToArray();
            initKeysData.AggregateKeys = aggregatesDict.Values.ToArray();
            initKeysData.QueueKeys = queuesDict.Values.ToArray();
            initKeysData.View = wbView;
            DataStore.Instance.dataDictionary.AddOrUpdate("WB" + WallDisplayID, initKeysData, (k, v) => initKeysData);

        }

        public void setView(string input)
        {
            wbView = input;
            wbLogger.Debug("wb view :" + wbView + " voor walldiplay id: "+ WallDisplayID );
        }

        public void setAggregateRTevent(HiPathProCenterLibrary.AggregateRealtimeEvent ev)
        {
            aggregateRTEv = ev;

            foreach (HiPathProCenterLibrary.AggregateRealtimeElement elAggrRT in aggregateRTEv)
            {
                wbLogger.Debug("wb nr :" + WallDisplayID + ". Aggr element Key: " + elAggrRT.AggregateKey + " abb rate: " + elAggrRT.Contacts);
                aggregatesDict[elAggrRT.AggregateKey].ServiceLevelPerc = (int)Math.Round(elAggrRT.ServiceLevel);
                aggregatesDict[elAggrRT.AggregateKey].Received = (int)(elAggrRT.Contacts);
            }

            var retAggregateKeys = new AggregateKey[aggregatesDict.Count];
            wbLogger.Debug("Parsing RT aggregatesDictCount: " + aggregatesDict.Count);
            wbLogger.Debug("return values RT aggregatesDictValues: " + aggregatesDict.Values.ToString());
            retAggregateKeys = aggregatesDict.Values.ToArray();
            DataStore.Instance.dataDictionary["WB" + WallDisplayID].AggregateKeys = retAggregateKeys;

        }

        public void setUserRTevent(HiPathProCenterLibrary.UserRealtimeEvent ev)
        {
            userRTEv = ev;
            keyList loggedOffList = new keyList();

            foreach (int i in usersList)
            {
                loggedOffList.Add(i);
            }
            if (userRTEv.Count > 0)
            {
                foreach (HiPathProCenterLibrary.UserRealtimeElement elUserRT in userRTEv)

                {
                    wbLogger.Debug("wb nr :" + WallDisplayID + ". User element ext: " + elUserRT.Extension + " abb rate: " + elUserRT.PresenceState);
                    // we combine handling and routing state to 1 state on the walldisplay
                    // if there is a handling state, the ahndling state is shown
                    // if the handling state is none, the routing state is shown
                    // note that after a direct call the time of the routing state does not reset from the OSCC.
                    if (elUserRT.HandlingStates.Count > 0)
                    {
                        foreach (HiPathProCenterLibrary.UserRTHandlingState userHS in elUserRT.HandlingStates)
                        {
                            usersDict[elUserRT.UserKey].State = agentStatusDict[userHS.HandlingState.ToString()];
                            usersDict[elUserRT.UserKey].DurationSec = userHS.TimeInHandlingState;
                            wbLogger.Debug("Parsing RT User event of user key: " + elUserRT.UserKey + " with handlingstate : " + userHS.HandlingState.ToString());
                        }
                    }
                    else {
                        usersDict[elUserRT.UserKey].State = agentStatusDict[elUserRT.RoutingState.ToString()];
                        usersDict[elUserRT.UserKey].DurationSec = elUserRT.TimeInRoutingState;
                        wbLogger.Debug("Parsing RT User event of user key: " + elUserRT.UserKey + " with routingstate : " + elUserRT.RoutingState.ToString());
                    }

                    loggedOffList.Remove(elUserRT.UserKey);
                    wbLogger.Debug("Parsing RT User event of user key: " + elUserRT.UserKey+ " with userDict : "  + usersDict[elUserRT.UserKey].ToString() );
                }
            }

            foreach (int i in loggedOffList)
            {
                usersDict[i].State = "Away";
                usersDict[i].DurationSec = 0;
            }

            var retUserKeys = new UserKey[usersDict.Count];
            wbLogger.Debug("Parsing RT UserDictCount: " + usersDict.Count);
            wbLogger.Debug("return values RT UserDictValues: " + usersDict.Values.ToString());
            retUserKeys = usersDict.Values.ToArray();
            DataStore.Instance.dataDictionary["WB" + WallDisplayID].UserKeys = retUserKeys;
        }

        public void setQueueRTevent(HiPathProCenterLibrary.QueueRealtimeEvent ev)
        {
            queueRTEv = ev;
            foreach (HiPathProCenterLibrary.QueueRealtimeElement elQueueRT in queueRTEv)
            {
                wbLogger.Debug("wb nr :" + WallDisplayID + ". Queue element key: " + elQueueRT.QueueKey + " Q Contacts: " + elQueueRT.Contacts);
                ///queuesDict[elQueueRT.QueueKey].Abandoned = elQueueRT.AbandonedRate;
                queuesDict[elQueueRT.QueueKey].ServiceLevelPerc = (int)Math.Round(elQueueRT.ServiceLevel);
                queuesDict[elQueueRT.QueueKey].MaxWaitingTimeSec = elQueueRT.OldestContactWaitTime;
            }

            var retQueueKeys = new QueueKey[queuesDict.Count];
            wbLogger.Debug("Parsing RT QueuesDictCount: " + queuesDict.Count);
            wbLogger.Debug("return values RT QueuesDictValues: " + queuesDict.Values.ToString());

            retQueueKeys = queuesDict.Values.ToArray();
            DataStore.Instance.dataDictionary["WB" + WallDisplayID].QueueKeys = retQueueKeys;

        }

        public void setQueuesCumEvent(HiPathProCenterLibrary.QueueCumulativeEvent ev)
        {
            wbLogger.Info("wb nr :" + WallDisplayID + " entering SetQueuesCumEvent");
            wbLogger.Info("wb nr :" + WallDisplayID + " queue cumulative ev count: " + ev.Count);
            wbLogger.Info("wb nr :" + WallDisplayID + " queue cumulative ev get Type: " + ev.GetType());
            foreach (HiPathProCenterLibrary.TimeRange timerangeCUM in ev)
            {                
                wbLogger.Info("wb nr :" + WallDisplayID + ". queue timerange cumulative event count : " + timerangeCUM.Count);
                wbLogger.Info("wb nr :" + WallDisplayID + ". queue timerangeDateString : " + timerangeCUM.TimeRange.ToLongDateString() );
                wbLogger.Info("wb nr :" + WallDisplayID + ". queue timerangeTimeString : " + timerangeCUM.TimeRange.ToLongTimeString());
                foreach (HiPathProCenterLibrary.QueueCumulativeElement elQueueCum in timerangeCUM)
                {   
                    wbLogger.Info("wb nr :" + WallDisplayID + ". Queue cumulative element key: " + elQueueCum.QueueKey + " Q Reveived: " + elQueueCum.Received );
                    ///queuesDict[elQueueRT.QueueKey].Abandoned = elQueueRT.AbandonedRate;
                    queuesDict[elQueueCum.QueueKey].Received = elQueueCum.Received;
                    queuesDict[elQueueCum.QueueKey].Abandoned = elQueueCum.Abandoned;
                    queuesDict[elQueueCum.QueueKey].Answered = elQueueCum.Answered;
                    queuesDict[elQueueCum.QueueKey].AvgWaitingTimeSec = (int)Math.Round(elQueueCum.AverageWaitTime);
                    queuesDict[elQueueCum.QueueKey].MaxWaitingTimeSec = elQueueCum.MaximumWaitTime;
                }
            }
            var retQueueKeys = new QueueKey[queuesDict.Count];
            wbLogger.Debug("Parsing Cumulative QueuesDictCount: " + queuesDict.Count);
            wbLogger.Debug("return values Cumulative QueuesDictValues: " + queuesDict.Values.ToString());

            retQueueKeys = queuesDict.Values.ToArray();
            DataStore.Instance.dataDictionary["WB" + WallDisplayID].QueueKeys = retQueueKeys;

        }

        public void setAggregatesCumEvent(HiPathProCenterLibrary.AggregateCumulativeEvent ev)
        {
            wbLogger.Info("wb nr :" + WallDisplayID + " entering SetAggregatesCumEvent");
            wbLogger.Info("wb nr :" + WallDisplayID + " Aggregate Cumulative event count: " + ev.Count);
            foreach (HiPathProCenterLibrary.TimeRange timerangeCUM in ev)
            {
                wbLogger.Info("wb nr :" + WallDisplayID + ". aggr timerangeDateString : " + timerangeCUM.TimeRange.ToLongDateString());
                wbLogger.Info("wb nr :" + WallDisplayID + ". aggr timerangeTimeString : " + timerangeCUM.TimeRange.ToLongTimeString());
                foreach (HiPathProCenterLibrary.AggregateCumulativeElement elAggregateCum in timerangeCUM)
                {
                    wbLogger.Info("wb nr :" + WallDisplayID + ". aggr element key: " + elAggregateCum.AggregateKey + " aggr Reveived: " + elAggregateCum.Received);
                    aggregatesDict[elAggregateCum.AggregateKey].Received = elAggregateCum.Received;
                    aggregatesDict[elAggregateCum.AggregateKey].Abandoned = elAggregateCum.Abandoned;
                    aggregatesDict[elAggregateCum.AggregateKey].Answered = elAggregateCum.Answered;
                    aggregatesDict[elAggregateCum.AggregateKey].AvgWaitingTimeSec = (int)Math.Round(elAggregateCum.AverageWaitTime);
                    aggregatesDict[elAggregateCum.AggregateKey].MaxWaitingTimeSec = elAggregateCum.MaximumWaitTime;
                }
            }
            var retAggregateKeys = new AggregateKey[aggregatesDict.Count];
            wbLogger.Debug("Parsing Cumulative aggregatesDictCount: " + aggregatesDict.Count);
            wbLogger.Debug("return values Cumulative aggregatesDictValues: " + aggregatesDict.Values.ToString());

            retAggregateKeys = aggregatesDict.Values.ToArray();
            DataStore.Instance.dataDictionary["WB" + WallDisplayID].AggregateKeys = retAggregateKeys;
        }

        public void setUsersCumEvent(HiPathProCenterLibrary.UserCumulativeEvent ev)
        {
            wbLogger.Info("wb nr :" + WallDisplayID + " entering SetUsersCumEvent");
            wbLogger.Info("wb nr :" + WallDisplayID + " User cumulative event count: " + ev.Count);
            foreach (HiPathProCenterLibrary.TimeRange timerangeCUM in ev)
            {
                wbLogger.Info("wb nr :" + WallDisplayID + ". user timerangeDateString : " + timerangeCUM.TimeRange.ToLongDateString());
                wbLogger.Info("wb nr :" + WallDisplayID + ". user timerangeTimeString : " + timerangeCUM.TimeRange.ToLongTimeString());
                foreach (HiPathProCenterLibrary.UserCumulativeElement elUserCum in timerangeCUM)
                {
                    wbLogger.Info("wb nr :" + WallDisplayID + ". user element key: " + elUserCum.UserKey + " U handeled: " + elUserCum.Handled);
                    ///queuesDict[elQueueRT.QueueKey].Abandoned = elQueueRT.AbandonedRate;
                    // usersDict[elUserCum.UserKey].TotalTalkTime = elUserCum.TotalTalkTime;
                }
            }
            var retUserKeys = new UserKey[usersDict.Count];
            wbLogger.Debug("Parsing Cumulative usersDictCount: " + usersDict.Count);
            wbLogger.Debug("return values Cumulative usersDictValues: " + usersDict.Values.ToString());

            retUserKeys = usersDict.Values.ToArray();
            DataStore.Instance.dataDictionary["WB" + WallDisplayID].UserKeys = retUserKeys;
        }

        public void setGroupsCumEvent(HiPathProCenterLibrary.GroupCumulativeEvent ev)
        {
            wbLogger.Info("wb nr :" + WallDisplayID + " entering SetGroupsCumEvent");
            wbLogger.Info("wb nr :" + WallDisplayID + " Group cumulative event count: " + ev.Count);
            foreach (HiPathProCenterLibrary.TimeRange timerangeCUM in ev)
            {
                wbLogger.Info("wb nr :" + WallDisplayID + ". group timerangeDateString : " + timerangeCUM.TimeRange.ToLongDateString());
                wbLogger.Info("wb nr :" + WallDisplayID + ". group timerangeTimeString : " + timerangeCUM.TimeRange.ToLongTimeString());
                foreach (HiPathProCenterLibrary.GroupCumulativeElement elGroupCum in timerangeCUM)
                {
                    wbLogger.Debug("wb nr :" + WallDisplayID + ". Group Cumulative element key: " + elGroupCum.GroupKey + " G Received: " + elGroupCum.Received);
                    ///queuesDict[elQueueRT.QueueKey].Abandoned = elQueueRT.AbandonedRate;
                    ///groupsDict[elGroupCum.GroupKey]. = elGroupCum.Received
                }
            }
            var retGroupKeys = new GroupKey[groupsDict.Count];
            wbLogger.Debug("Parsing Cumulative groupsDictCount: " + groupsDict.Count);
            wbLogger.Debug("return values Cumulative groupsDictValues: " + groupsDict.Values.ToString());

            retGroupKeys = groupsDict.Values.ToArray();
            DataStore.Instance.dataDictionary["WB" + WallDisplayID].GroupKeys = retGroupKeys;
        }

        public void setGroupRTevent(HiPathProCenterLibrary.GroupRealtimeEvent ev)
        {
            groupRTEv = ev;
            foreach (HiPathProCenterLibrary.GroupRealtimeElement elGroupRT in groupRTEv)
            {
                wbLogger.Debug("wb nr :" + WallDisplayID + ". Group RT element key: " + elGroupRT.GroupKey + " Q LoggonOnUsers: " + elGroupRT.LoggedOnUsers);
                groupsDict[elGroupRT.GroupKey].Away = elGroupRT.AwayUsers;
                groupsDict[elGroupRT.GroupKey].Busy = elGroupRT.BusyUsers;
                groupsDict[elGroupRT.GroupKey].CallsWaiting = elGroupRT.CallsWaiting;
                groupsDict[elGroupRT.GroupKey].Idle = elGroupRT.IdleUsers;
                groupsDict[elGroupRT.GroupKey].LoggedOn = elGroupRT.LoggedOnUsers;
                groupsDict[elGroupRT.GroupKey].OnDirectCall = elGroupRT.HandlingDirectUsers;
                groupsDict[elGroupRT.GroupKey].OnRoutedCall = elGroupRT.HandlingRoutedUsers;

            }
            var retGroupKeys = new GroupKey[groupsDict.Count];
            wbLogger.Debug("Parsing RT groupsDictCount: " + groupsDict.Count);            
            retGroupKeys = groupsDict.Values.ToArray();
            wbLogger.Debug("return values RT groupsDictValues: " + groupsDict.Values.ToString());
            DataStore.Instance.dataDictionary["WB" + WallDisplayID].GroupKeys = retGroupKeys;
        }


        public void setQueuesKeylist(string keylistString)
        {
            string[] keysArrays;
            keysArrays = keylistString.Split(',');
            foreach (String keyString in keysArrays)
            {
                try
                {
                    queuesList.Add(Convert.ToInt32(keyString));
                }
                catch (FormatException e)
                {
                    wbLogger.Error("wb nr :" + WallDisplayID + ". Error parsing queueKey string to int " + e.Message);
                }

            }
        }

        public keyList getQueuesKeylist()
        {
            keyList retKeyList = new keyList();
            foreach (int i in queuesList)
            { retKeyList.Add(i); }
            return retKeyList;
        }

        public void setUsersKeylist(string keylistString)
        {
            string[] keysArrays;
            keysArrays = keylistString.Split(',');
            foreach (String keyString in keysArrays)
            {
                try
                {
                    usersList.Add(int.Parse(keyString));
                }
                catch (FormatException e)
                {
                    wbLogger.Error("wb nr :" + WallDisplayID + ". Error parsing userKey string to int " + e.Message);
                }

            }
        }

        public keyList getUsersKeylist()
        {
            keyList retKeyList = new keyList();
            foreach (int i in usersList)
            { retKeyList.Add(i); }
            return retKeyList;
        }


        public void setGroupsKeylist(string keylistString)
        {
            string[] keysArrays;
            keysArrays = keylistString.Split(',');
            foreach (String keyString in keysArrays)
            {
                try
                {
                    groupsList.Add(int.Parse(keyString));
                }
                catch (FormatException e)
                {
                    wbLogger.Error("wb nr :" + WallDisplayID + ". Error parsing groupKey string to int " + e.Message);
                }

            }
        }

        public keyList getGroupsKeylist()
        {
            keyList retKeyList = new keyList();
            foreach (int i in groupsList)
            { retKeyList.Add(i); }
            return retKeyList;

        }


        public void setAggregatesKeylist(string keylistString)
        {
            string[] keysArrays;
            keysArrays = keylistString.Split(',');
            foreach (String keyString in keysArrays)
            {
                try
                {
                    aggregatesList.Add(int.Parse(keyString));
                }
                catch (FormatException e)
                {
                    wbLogger.Error("wb nr :" + WallDisplayID + ". Error parsing aggregatesKey string to int " + e.Message);
                }

            }
        }

        public keyList getAggregatesKeylist()
        {
            keyList retKeyList = new keyList();
            foreach (int i in aggregatesList)
            { retKeyList.Add(i); }
            return retKeyList;
        }

        public void setAggregatesCumQueryID(int queryID)
        {
            aggregatesCumQueryID = queryID;
        }

        public int getAggregatesCumQueryID()
        {
            return aggregatesCumQueryID;
        }

        public void setQueuesCumQueryID(int queryID)
        {
            queuesCumQueryID = queryID;
        }

        public int getQueuesCumQueryID()
        {
            return queuesCumQueryID;
        }

        public void setUsersCumQueryID(int queryID)
        {
            usersCumQueryID = queryID;
        }

        public int getUsersCumQueryID()
        {
            return usersCumQueryID;
        }

        public void setGroupsCumQueryID(int queryID)
        {
            groupsCumQueryID = queryID;
        }

        public int getGroupsCumQueryID()
        {
            return groupsCumQueryID;
        }

        public void setQueuesRTQueryID(int queryID)
        {
            queuesRTQueryID = queryID;
        }
        public int getQueuesRTQueryID()
        {
            return queuesRTQueryID;
        }
        public void setUsersRTQueryID(int queryID)
        {
            usersRTQueryID = queryID;
        }
        public int getUsersRTQueryID()
        {
            return usersRTQueryID;
        }
        public void setAggregatesRTQueryID(int queryID)
        {
            aggregatesRTQueryID = queryID;
        }
        public int getAggregatesRTQueryID()
        {
            return aggregatesRTQueryID;
        }
        public void setGroupsRTQueryID(int queryID)
        {
            groupsRTQueryID = queryID;
        }
        public int getGroupsRTQueryID()
        {
            return groupsRTQueryID;
        }


    }

}
