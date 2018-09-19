using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Walldisplay
{
    interface IUserStatisticData
    {
        string getName();
        string getState();
        DateTime getStateStart();
    }
    interface IGroupStatisticsData
    {
        string getGroup();
        IUserStatisticData[] getUsers();
    }
    interface IStatisticsData
    {
        IGroupStatisticsData getGroupStatistics(string group);
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

        public IStatisticsData data { get; set; }
    }
}
