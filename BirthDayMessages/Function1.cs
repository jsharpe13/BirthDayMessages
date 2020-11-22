using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace BirthDayMessages
{
    public static class Function1
    {
        [FunctionName("Function1")]

        //public static void Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log, ExecutionContext context)
        public static void Run([TimerTrigger("0 0 0 1 * *")] TimerInfo myTimer, ILogger log, ExecutionContext context)

        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            // var am = new AddressMessages(Environment.GetEnvironmentVariable("FILE"), log);
            string inputFile = Path.Combine(context.FunctionAppDirectory) + "\\" + Environment.GetEnvironmentVariable("FILE");
            var am = new AddressMessages(inputFile, log);
            am.sendBirthDayMessages();
        }

    }
}
