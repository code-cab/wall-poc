using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Walldisplay
{
    public class UserKey
    {
        public string Name { get; set; }
        public string State { get; set; }
        public int DurationSec { get; set; }
    }
    
    public class GroupKey
    {
        public string GroupName { get; set; }
        public int LoggedOn { get; set; }
        public int OnRoutedCall { get; set; }
        public int OnDirectCall { get; set; }
        public int Idle { get; set; }
        public int Busy { get; set; }
        public int Away { get; set; }
        public int CallsWaiting { get; set; }
    }


    public class QueueKey
    {
        public string GroupName { get; set; }
        /// public string QueueName { get; set; }
        public int Received { get; set; }
        public int Abandoned { get; set; }
        public int Answered { get; set; }
        public long AvgWaitingTimeSec { get; set; }
        public long MaxWaitingTimeSec { get; set; }
        public int ServiceLevelPerc { get; set; }
    }

    public class AggregateKey : QueueKey
    {
        ///public string AggregateName { get; set; }
    }

    public class HourlyQueueKey : QueueKey
    {
        public int Hour { get; set; }
    }

    public class KeysData
    {
        public string View { get; set; }
        public UserKey[] UserKeys { get; set; }
        public GroupKey[] GroupKeys { get; set; }
        public QueueKey[] QueueKeys { get; set; }
        public AggregateKey[] AggregateKeys { get; set; }
        public HourlyQueueKey[][] HourlyQueueKeys { get; set; }
        public int WaitingWarnLimit { get; set; }
        public int AvgWaitingTimeLimit { get; set; }
        public int ServiceLevelWarnLimit { get; set; }

        public KeysData()
        {
            View = "initview";
            UserKeys = new UserKey[0];
            GroupKeys = new GroupKey[0];
            QueueKeys = new QueueKey[0];
            AggregateKeys = new AggregateKey[0];
            HourlyQueueKeys = new HourlyQueueKey[0][];

        }
    }



    sealed class DataStore
    {
        private static DataStore instance = null;
        private static readonly object padlock = new object();

        DataStore()
        {
            setDemoData();
        }



        public static DataStore Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new DataStore();
                    }
                    return instance;
                }
            }
        }

        /**
         * StatisticsData is thread safe as long as the array length is not being changed.
         * 
         * When the array length has to change, the whole StatisticsData instance object should be replaced
         * 
         **/ 
        public KeysData Data { get; set; }
        // Create a list of datasets, one dataset for each wallboard
        public ConcurrentDictionary<String, KeysData> dataDictionary = new ConcurrentDictionary<String, KeysData>();

        void setDemoData()
        {
            // Demo data           
            Data = new KeysData
            {
                UserKeys = new UserKey[]
                {
                    new UserKey
                    {
                        Name = "John",
                        State = "ACTIVE",
                        DurationSec = 42
                    },
                    new UserKey
                    {
                        Name = "Sue",
                        State = "IDLE",
                        DurationSec = 192
                    },
                    new UserKey
                    {
                        Name = "Paul",
                        State = "ACTIVE",
                        DurationSec = 8 * 60 + 1
                    },
                    new UserKey
                    {
                        Name = "Matt",
                        State = "AWAY",
                        DurationSec = 24 * 60 + 42
                    },
                    new UserKey
                    {
                        Name = "Anna",
                        State = "IDLE",
                        DurationSec = 5
                    },
                    new UserKey
                    {
                        Name = "Peter",
                        State = "ACTIVE",
                        DurationSec = 83
                    },
                    new UserKey
                    {
                        Name = "Steve",
                        State = "ACTIVE",
                        DurationSec = 5*60 + 56
                    },
                    new UserKey
                    {
                        Name = "Lee",
                        State = "IDLE",
                        DurationSec = 7
                    },
                    new UserKey
                    {
                        Name = "Ali",
                        State = "ACTIVE",
                        DurationSec = 103
                    },
                    new UserKey
                    {
                        Name = "Tyrone",
                        State = "ACTIVE",
                        DurationSec = 2*60+56
                    },
                },
                GroupKeys = new GroupKey[]
                {
                    new GroupKey
                    {
                        GroupName = "Group A",
                        LoggedOn = 20,
                        OnRoutedCall = 4,
                        OnDirectCall = 1,
                        Idle = 2,
                        Busy = 1,
                        Away = 2,
                        CallsWaiting = 3
                    },
                    new GroupKey
                    {
                        GroupName = "Group B",
                        LoggedOn = 2,
                        OnRoutedCall = 1,
                        OnDirectCall = 4,
                        Idle = 6,
                        Busy = 0,
                        Away = 0,
                        CallsWaiting = 5
                    },
                    new GroupKey
                    {
                        GroupName = "Group C",
                        LoggedOn = 20,
                        OnRoutedCall = 4,
                        OnDirectCall = 1,
                        Idle = 2,
                        Busy = 1,
                        Away = 2,
                        CallsWaiting = 4
                    },
                    new GroupKey
                    {
                        GroupName = "Group D",
                        LoggedOn = 20,
                        OnRoutedCall = 4,
                        OnDirectCall = 1,
                        Idle = 2,
                        Busy = 1,
                        Away = 2,
                        CallsWaiting = 6
                    },
                },
                QueueKeys = new QueueKey[]
                {
                    new QueueKey
                    {
                        GroupName = "Queue A - Helpdesk",
                        Received = 32,
                        Abandoned = 3,
                        Answered = 29,
                        AvgWaitingTimeSec = 26,
                        MaxWaitingTimeSec = 36,
                        ServiceLevelPerc = 89
                    },
                    new QueueKey
                    {
                        GroupName = "Queue B - Facilities",
                        Received = 32,
                        Abandoned = 3,
                        Answered = 29,
                        AvgWaitingTimeSec = 29,
                        MaxWaitingTimeSec = 36,
                        ServiceLevelPerc = 90
                    },
                    new QueueKey
                    {
                        GroupName = "Queue C",
                        Received = 32,
                        Abandoned = 3,
                        Answered = 29,
                        AvgWaitingTimeSec = 30,
                        MaxWaitingTimeSec = 36,
                        ServiceLevelPerc = 91
                    },
                    new QueueKey
                    {
                        GroupName = "Queue D",
                        Received = 32,
                        Abandoned = 3,
                        Answered = 29,
                        AvgWaitingTimeSec = 31,
                        MaxWaitingTimeSec = 36,
                        ServiceLevelPerc = 91
                    },

                },
                AggregateKeys = new AggregateKey[]
                {
                    new AggregateKey
                    {
                        GroupName = "XYZ",
                        Received = 66,
                        Abandoned = 8,
                        Answered = 58,
                        AvgWaitingTimeSec = 23,
                        MaxWaitingTimeSec = 5*60+36,
                        ServiceLevelPerc = 90
                    },

                },
                HourlyQueueKeys = new HourlyQueueKey[][]
                {
                    new int[][]{
                        new int[] { 9, 4, 4, 0 },
                        new int[] { 10, 6, 6, 0 },
                        new int[] { 11, 14, 12, 2 },
                        new int[] { 12, 16, 15, 3 },
                        new int[] { 13, 9, 8, 1 },
                        new int[] { 14, 6, 6, 0 },
                        new int[] { 15, 11, 11, 0 },
                        new int[] { 16, 9, 8, 1 },
                        new int[] { 17, 3, 3, 0 },
                    }.Select(d => new HourlyQueueKey
                        {
                            Hour = d[0],
                            Received = d[1],
                            Abandoned = d[2],
                            Answered = d[3]

                        }).ToArray()
                }
            };
            // demo data add two dataset (aka wallboard data ) to list
            //

        }

    }
}
