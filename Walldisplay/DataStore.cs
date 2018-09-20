using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Walldisplay
{
    public class UserRealtimeElement
    {
        public string Name { get; set; }
        public string State { get; set; }
        public DateTime StateStart { get; set; }

        public UserRealtimeElement()
        {
            StateStart = DateTime.Now;
        }
    }
    
    public class GroupRealtimeElement
    {
        public int LoggedOn { get; set; }
        public int OnRoutedCall { get; set; }
        public int OnDirectCall { get; set; }
        public int Idle { get; set; }
        public int Busy { get; set; }
        public int Away { get; set; }
        public int CallsWaiting { get; set; }
    }


    public class CummulativeElement
    {
        public int Received { get; set; }
        public int Abandoned { get; set; }
        public int Answered { get; set; }
        public long AvgWaitingTimeSec { get; set; }
        public long MaxWaitingTimeSec { get; set; }
        public int ServiceLevelPerc { get; set; }
    }
    public class GroupElements
    {
        public string Group { get; set; }
        public UserRealtimeElement[] UserRealtimeElement { get; set; }
        public GroupRealtimeElement GroupRealtimeElement { get; set; }
        public CummulativeElement AggregateCummulativeElement { get; set; }

        public GroupElements()
        {
            UserRealtimeElement = new UserRealtimeElement[0];
            GroupRealtimeElement = new GroupRealtimeElement();
            AggregateCummulativeElement = new CummulativeElement();
        }
    }

    public class CummelativeElement
    {
        public int Received { get; }
        public int Abandoned { get; }

    }

    public class StatisticsData
    {
        public GroupElements[] GroupData { get; set; }

        public CummulativeElement QueueCummulativeElement { get; set; }

        public CummulativeElement[] HourlyCummulativeElements { get; set; }
        public int EndHour { get; set; }

        public StatisticsData()
        {
            GroupData = new GroupElements[0];
            QueueCummulativeElement = new CummulativeElement();
            HourlyCummulativeElements = new CummulativeElement[0];
        }
    }


    sealed class DataStore
    {
        private static DataStore instance = null;
        private static readonly object padlock = new object();

        DataStore()
        {
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
        public StatisticsData Data { get; set; }
    }
}
