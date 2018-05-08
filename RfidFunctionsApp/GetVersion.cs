using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using WG3000_COMM.Core;

namespace RfidFunctionsApp
{
    public static class GetVersion
    {
        [FunctionName("GetVersion")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("Getting version...");

            // Get request body
            dynamic data = await req.Content.ReadAsAsync<object>();

            var controller = new wgMjController
            {
                ControllerSN = (int)data?.serialNumber,
                IP = data?.ip,
                PORT = (int)data?.port
            };

            bool loaded = await Task.Run(() =>
            {
                return controller.GetMjControllerRunInformationIP() > 0;
            });

            if (loaded)
            {
                return req.CreateResponse(HttpStatusCode.OK, controller.RunInfo.driverVersion);
            }
            else
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, "Unable to get remote information");
            }
        }
    }
}
