using IniParser;
using IniParser.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using log4net;


namespace Walldisplay
{
    public class DisplayDataController : ApiController
    {
        private readonly log4net.ILog wbLogger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        class Data {
            public String View;
            public UserKey[] UserKeys;
            public GroupKey[] GroupKeys;
            public QueueKey[] QueueKeys;
            public AggregateKey[] AggregateKeys;
            public HourlyQueueKey[][] HourlyQueueKeys;
            public int WaitingWarnLimit;
            public int AvgWaitingTimeLimit;
            public int ServiceLevelWarnLimit;
        }
        [HttpGet]
        public HttpResponseMessage Get(string displayId)
        {
            try
            {
                var parser = new FileIniDataParser();
                IniData ini = parser.ReadFile("walldisplay.ini");

                KeysData keysData = DataStore.Instance.Data;


                KeysData data = new KeysData();

                data.View = ini[displayId]["View"];
                

                if (data.View == null) throw new ArgumentException("Invalid display id " + displayId);

                wbLogger.Debug("displayDataController fetched walldisplay View :" + data.View);

                Func<string, int[]> getKeys = key => (ini[displayId][key] ?? "").Split(',').Where(s => s.Length > 0).Select(s => Convert.ToInt32(s) - 1).ToArray();
                
                data.UserKeys = keysData.UserKeys.Where((d, index) => getKeys("UserKeys").Contains(index)).ToArray();

                data.GroupKeys = keysData.GroupKeys.Where((d, index) => getKeys("GroupKeys").Contains(index)).ToArray();

                data.QueueKeys = keysData.QueueKeys.Where((d, index) => getKeys("QueueKeys").Contains(index)).ToArray();

                data.AggregateKeys = keysData.AggregateKeys.Where((d, index) => getKeys("AggregateKeys").Contains(index)).ToArray();

                data.HourlyQueueKeys = keysData.HourlyQueueKeys.Where((d, index) => getKeys("HourlyQueueKeys").Contains(index)).ToArray();

                // TODO: Get from INI/settings?
                data.WaitingWarnLimit = Convert.ToInt32(ini["WallDisplay"]["WaitingCallsMax"]);
                data.AvgWaitingTimeLimit = Convert.ToInt32(ini["WallDisplay"]["AvgWaitingTimeMax"]);
                data.ServiceLevelWarnLimit = Convert.ToInt32(ini["WallDisplay"]["ServiceLevelMin"]);

                HttpResponseMessage response;

                //String json = JsonConvert.SerializeObject(data);
                String json = JsonConvert.SerializeObject(DataStore.Instance.dataDictionary[displayId]);
                response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(json, Encoding.UTF8, "application/json");

                return response;

            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
    }
}
