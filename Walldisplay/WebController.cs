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
    public class UserStatisticsController : ApiController
    {
        public class UserStatistic
        {
            public string name { get; set; }
            public string state { get; set; }
            public long duration { get; set; }
        }

        public class UserStatistics
        {
            public string group { get; set; }
            public UserStatistic[] users { get; set; }
        }

        [HttpGet]
        public HttpResponseMessage Get(string group)
        {

            try
            {
                HttpResponseMessage response;
                StatisticsData data = DataStore.Instance.Data;

                GroupElements groupData = data.GroupData.First(d => d.Group.Equals(group));

                long now = DateTime.Now.Ticks;

                UserStatistics userStatistics = new UserStatistics
                {
                    group = group,
                    users = groupData.UserRealtimeElement.Select(tu => new UserStatistic()
                    {
                        name = tu.Name,
                        state = tu.State,
                        duration = Convert.ToInt64((now - tu.StateStart.Ticks) / TimeSpan.TicksPerSecond)
                    }).ToArray()
                };

                String json = JsonConvert.SerializeObject(userStatistics);
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
