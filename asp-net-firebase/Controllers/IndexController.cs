using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace asp_net_firebase.Controllers
{
    [Route("api/[controller]")]
    public class IndexController : Controller
    {
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get(string title = "notification title", string body = "notification body", int badge = 1)
        {
            try
            {
                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "post";
                // ServerKey - Key from Firebase cloud messaging server  
                tRequest.Headers.Add(string.Format("Authorization: key={0}", "{ServerKey}"));
                // SenderId - From firebase project setting  
                tRequest.Headers.Add(string.Format("Sender: id={0}", "{SenderId}"));
                tRequest.ContentType = "application/json";
                // DeviceId - obtained when the device is registered
                var payload = new
                {
                    to = "{DeviceId}",
                    priority = "high",
                    content_available = true,
                    notification = new
                    {
                        body,
                        title,
                        badge
                    },
                };

                string postbody = JsonConvert.SerializeObject(payload).ToString();
                Byte[] byteArray = Encoding.UTF8.GetBytes(postbody);
                tRequest.ContentLength = byteArray.Length;
                using (Stream dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    using (WebResponse tResponse = tRequest.GetResponse())
                    {
                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {
                            if (dataStreamResponse != null) using (StreamReader tReader = new StreamReader(dataStreamResponse))
                                {
                                    String sResponseFromServer = tReader.ReadToEnd();
                                    return new string[] { sResponseFromServer };
                                }
                        }
                    }
                }

                return new string[] { "No response" };
            }
            catch (Exception exp)
            {
                return new string[] { exp.Message };
            }
        }
    }
}
