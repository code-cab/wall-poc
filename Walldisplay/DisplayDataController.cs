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
using Walldisplay;

namespace Click2DialService
{
    public class DisplayDataController : ApiController
    {
        class Data {
            public String View;
            public UserKey[] UserKeys;
            public GroupKey[] GroupKeys;
            public QueueKey[] QueueKeys;
            public AggregateKey[] AggregateKeys;
            public HourlyQueueKey[][] HourlyQueueKeys;
            public int WaitingWarnLimit;
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

                Data data = new Data
                {
                    View = ini[displayId]["View"]
                };
                if (data.View == null) throw new ArgumentException("Invalid display id " + displayId);

                Func<string, int[]> getKeys = key => (ini[displayId][key] ?? "").Split(',').Where(s => s.Length > 0).Select(s => Convert.ToInt32(s) - 1).ToArray();
                
                data.UserKeys = keysData.UserKeys.Where((d, index) => getKeys("UserKeys").Contains(index)).ToArray();

                data.GroupKeys = keysData.GroupKeys.Where((d, index) => getKeys("GroupKeys").Contains(index)).ToArray();

                data.QueueKeys = keysData.QueueKeys.Where((d, index) => getKeys("QueueKeys").Contains(index)).ToArray();

                data.AggregateKeys = keysData.AggregateKeys.Where((d, index) => getKeys("AggregateKeys").Contains(index)).ToArray();

                data.HourlyQueueKeys = keysData.HourlyQueueKeys.Where((d, index) => getKeys("HourlyQueueKeys").Contains(index)).ToArray();

                // TODO: Get from INI/settings?
                data.WaitingWarnLimit = 5;
                data.AvgWaitingTimeLimit = 30;
                data.ServiceLevelWarnLimit = 90;

                HttpResponseMessage response;

                String json = JsonConvert.SerializeObject(data);
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
