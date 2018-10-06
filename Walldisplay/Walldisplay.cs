using System;
using System.Collections.Generic;
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
        private string outputPath = string.Empty;
        private string htmlTemplate = string.Empty;
        private HiPathProCenterLibrary.AggregateRealtimeEvent aggregateRTEv;
        private HiPathProCenterLibrary.UserRealtimeEvent userRTEv;
        private HiPathProCenterLibrary.QueueRealtimeEvent queueRTEv;
        private HiPathProCenterLibrary.GroupRealtimeEvent groupRTEv;
        private HiPathProCenterLibrary.Queues queuesCol;
        private HiPathProCenterLibrary.Users usersCol;
        private HiPathProCenterLibrary.Groups groupsCol;
        private HiPathProCenterLibrary.Aggregates aggregatesCol;
        private int WallDisplayID;
        private string queuesLine;
        private string usersLine;
        private string aggregatesLine;
        private string groupsLine;
        private KeysData Data;
        private readonly log4net.ILog wbLogger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public Walldisplay(int WalldisplayID)
        {
            if ((WalldisplayID > 0) && (WalldisplayID < 50)) { WallDisplayID = WalldisplayID; }

        }

        public void updateWallBoard()
        {

        }

        public void setOutputpath(string input)
        {
            outputPath = input;
        }

        public void setTemplate(string input)
        {
            htmlTemplate = input;

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
            string line = "";
            
            foreach (HiPathProCenterLibrary.AggregateRealtimeElement elAggrRT in aggregateRTEv)
            {
                wbLogger.Debug("wb nr :" + WallDisplayID + ". Aggr element Key: " + elAggrRT.AggregateKey + " abb rate: " + elAggrRT.Contacts);
                line += "Aggr" + elAggrRT.AggregateKey;
                line += ":" + elAggrRT.Contacts;
                line += ";" + (int)Math.Round(elAggrRT.ServiceLevel);
                line += ";" + elAggrRT.OldestContactWaitTime;
                line += ",";
            }

            if (line.Length > 0) { aggregatesLine = line.Substring(0, line.Length - 1); }
            else
            {
                aggregatesLine = line;
            }
        }

        public void setUserRTevent(HiPathProCenterLibrary.UserRealtimeEvent ev)
        {
            userRTEv = ev;
            string line = "";
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
                    line += "User" + elUserRT.UserKey;
                    line += ":" + elUserRT.PresenceState;
                    line += ";" + elUserRT.RoutingState;
                    line += ";" + elUserRT.TimeInPresenceState;
                    line += ";" + elUserRT.TimeInRoutingState;
                    ///line += ";" + elUserRT.HandlingStates
                    line += ",";
                    loggedOffList.Remove(elUserRT.UserKey);
                    
                }
            }

            foreach (int i in loggedOffList)
            {
                line += "User" + i;
                line += ":0;0;0;0,";
            }


            if (line.Length > 0) { usersLine = line.Substring(0, line.Length - 1); }
            else
            {
                usersLine = line;
            }
        }

        public void setQueueRTevent(HiPathProCenterLibrary.QueueRealtimeEvent ev)
        {
            queueRTEv = ev;
            string line = "";
            foreach (HiPathProCenterLibrary.QueueRealtimeElement elQueueRT in queueRTEv)
            {
                wbLogger.Debug("wb nr :" + WallDisplayID + ". Queue element key: " + elQueueRT.QueueKey + " Q Contacts: " + elQueueRT.Contacts);
                line += "Queue" + elQueueRT.QueueKey;
                line += ":" + elQueueRT.Contacts;
                line += ";" + (int)Math.Round(elQueueRT.ServiceLevel);
                line += ";" + elQueueRT.OldestContactWaitTime;
                line += ",";
            }

            if (line.Length > 0) { queuesLine = line.Substring(0, line.Length - 1); }
            else
            {
                queuesLine = line;
            }
        }

        public void setGroupRTevent(HiPathProCenterLibrary.GroupRealtimeEvent ev)
        {
            groupRTEv = ev;
            string line = "";
            foreach (HiPathProCenterLibrary.GroupRealtimeElement elGroupRT in groupRTEv)
            {
                wbLogger.Debug("wb nr :" + WallDisplayID + ". Group element key: " + elGroupRT.GroupKey + " Q Contacts: " + elGroupRT.LoggedOnUsers);
                line += "Group" + elGroupRT.GroupKey;
                line += ":" + elGroupRT.LoggedOnUsers;
                line += ";" + elGroupRT.BusyUsers;
                line += ";" + elGroupRT.CallsWaiting;
                line += ",";

            }
            if (line.Length > 0) { groupsLine = line.Substring(0, line.Length - 1); }
            else
            {
                groupsLine = line;
            }
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
