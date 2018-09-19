using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Walldisplay
{
    interface IUserRealtimeElement
    {
        string Name();
        string State();
        DateTime StateStart();
    }
    
    interface IGroupRealtimeElement
    {
        int LoggedOn();
        int OnRoutedCall();
        int OnDirectCall();
        int Idle();
        int Busy();
        int Away();
        int CallsWaiting();

    }


    interface ICummulativeElement
    {
        int Received();
        int Abandoned();
        int Answered();
        long AvgWaitingTimeSec();
        long MaxWaitingTimeSec();
        int ServiceLevelPerc();

    }
    interface IGroupElements
    {
        string Group();
        IUserRealtimeElement[] UserRealtimeElement();
        IGroupRealtimeElement GroupRealtimeElement();
        ICummulativeElement AggregateCummulativeElement();

    }
    interface IStatisticsData
    {
        IGroupElements[] GroupData();

        ICummulativeElement QueueCummulativeElement();

        ICummulativeElement[] HourlyCummulativeElements();
        int EndHour();
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

        public IStatisticsData Data { get; set; }
    }
}
