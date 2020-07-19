using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;

namespace slackChannel
{
    public static class slackChannel
    {
        //Url + segredo do KeyVault
        private static string hookSlack = System.Environment.GetEnvironmentVariable("hookSlack");
        [FunctionName("slackChannel")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            //Registra o log de informações
            log.LogInformation("C# HTTP trigger function processed a request.");
            //Pega o valor query string da url na requisição HTTP
            string message = req.Query["message"];
            //Verifica se mensagem foi preenchida a mensagem foi preenchida
            string responseMessage = string.IsNullOrEmpty(message)
                ? "This HTTP triggered function executed successfully. " +
                "Pass a name in the query string or in the request body for a personalized response."
                : $"{message}";

            //Aqui eu intâncio o restclient com a url + token do slack armazenados no keyvault
            var client = new RestClient(hookSlack);
            client.Timeout = -1;
            //Crio uma requisição Post dos dados
            var request = new RestRequest(Method.POST);
            //Insiro o cabeçalho
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            //Monto payload
            var messageObject = new { channel = "#project-nodejs",
                username = "webhookbot", text = responseMessage,
                icon_emoji = ":ghost:" };
            //Adiciono como parametro convertendo o payload
            request.AddParameter("payload", JsonConvert.SerializeObject(messageObject));
            //Executo uma requisição com o endereço
            IRestResponse response = client.Execute(request);
            //Retorno no objeto
            return new OkObjectResult(response.Content);
        }
    }
}
