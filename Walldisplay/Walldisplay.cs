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
        private List<int> queuesList = new List<int>();
        private List<int> usersList = new List<int>();
        private List<int> aggregatesList = new List<int>();
        private List<int> groupsList = new List<int>();
        private string wbView = string.Empty;
        private HiPathProCenterLibrary.AggregateRealtimeEvent aggregateRTEv;
        private HiPathProCenterLibrary.UserRealtimeEvent userRTEv;
        private HiPathProCenterLibrary.QueueRealtimeEvent queueRTEv;
        private HiPathProCenterLibrary.GroupRealtimeEvent groupRTEv;
        private HiPathProCenterLibrary.Queues queuesCol;
        private HiPathProCenterLibrary.Users usersCol;
        private HiPathProCenterLibrary.Groups groupsCol;
        private HiPathProCenterLibrary.Aggregates aggregatesCol;
        private int WallDisplayID;
        private ConcurrentDictionary<int, QueueKey> queuesDict = new ConcurrentDictionary<int, QueueKey>();
        private ConcurrentDictionary<int, UserKey> usersDict = new ConcurrentDictionary<int, UserKey>();
        private ConcurrentDictionary<int, AggregateKey> aggregatesDict = new ConcurrentDictionary<int, AggregateKey>();
        private ConcurrentDictionary<int, GroupKey> groupsDict = new ConcurrentDictionary<int, GroupKey>();

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
            //initKeysData.View = wbView;
            DataStore.Instance.dataDictionary.AddOrUpdate("WB" + WallDisplayID, initKeysData, (k, v) => initKeysData);

        }

        public void initializeWallBoard()
        {
            KeysData initKeysData = new KeysData();

            Database osccDB = new Database();
            //wbLogger.Info("OSCC ODBC settings: " + Convert.ToString(settingIniFile.GetKey("DSN")));
            osccDB.setDsn("OSCC");
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
                usersDict[i].State = "ACTIVE";
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

        }


        public void setQueuesCol(HiPathProCenterLibrary.Queues col)
        {
            queuesCol = col;

            foreach (HiPathProCenterLibrary.Queue objQueue in queuesCol)
            {
            }


        }

        public void setUsersCol(HiPathProCenterLibrary.Users col)
        {
            usersCol = col;
            foreach (HiPathProCenterLibrary.User objUser in usersCol)
            {
            }

        }

        public void setGroupsCol(HiPathProCenterLibrary.Groups col)
        {
            groupsCol = col;
            foreach (HiPathProCenterLibrary.Group objGroup in groupsCol)
            {
            }

        }

        public void setAggregatesCol(HiPathProCenterLibrary.Aggregates col)
        {
            aggregatesCol = col;
            foreach (HiPathProCenterLibrary.Aggregate objAggregate in aggregatesCol)
            {
            }

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
                    usersDict[elUserRT.UserKey].State = elUserRT.RoutingState.ToString();
                    usersDict[elUserRT.UserKey].DurationSec = elUserRT.TimeInRoutingState;
                    loggedOffList.Remove(elUserRT.UserKey);
                }
            }

            foreach (int i in loggedOffList)
            {
                usersDict[i].State = "Logged off";
                usersDict[i].DurationSec = 0;
            }

            var retUserKeys = new UserKey[usersDict.Count];
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
            retQueueKeys = queuesDict.Values.ToArray();
            DataStore.Instance.dataDictionary["WB" + WallDisplayID].QueueKeys = retQueueKeys;

        }

        public void setGroupRTevent(HiPathProCenterLibrary.GroupRealtimeEvent ev)
        {
            groupRTEv = ev;
            foreach (HiPathProCenterLibrary.GroupRealtimeElement elGroupRT in groupRTEv)
            {
                wbLogger.Debug("wb nr :" + WallDisplayID + ". Group element key: " + elGroupRT.GroupKey + " Q Contacts: " + elGroupRT.LoggedOnUsers);
                groupsDict[elGroupRT.GroupKey].LoggedOn = elGroupRT.LoggedOnUsers;
                groupsDict[elGroupRT.GroupKey].Busy = elGroupRT.BusyUsers;
                groupsDict[elGroupRT.GroupKey].CallsWaiting = elGroupRT.CallsWaiting;
            }
            var retGroupKeys = new GroupKey[groupsDict.Count];
            retGroupKeys = groupsDict.Values.ToArray();
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
