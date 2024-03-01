using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Logic.PayPal
{
    public class PayPalAPI
    {
        public IConfiguration configuration { get; }

        public PayPalAPI(IConfiguration _configuration) 
        {
           configuration = _configuration;
        }
        public async Task<string> getRedirectURLToPayPal(double total, string currency) 
        {
            try
            {
                return Task.Run(async () =>
                {
                    HttpClient http = GetPaypalHttpClient();

                    // get an access token
                    PayPalAccessToken accessToken = await GetPayPalAccessTokenAsync(http);

                    //create payment
                    PayPalCreatedResponse createdPayment = await CreatePaypalPaymentAsync(http, accessToken, total, currency);
                    var approval_url = createdPayment.links.First(x => x.rel == "approved_url").href;
                    return approval_url;
                }).Result;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private HttpClient GetPaypalHttpClient() 
        {
            const string sandbox = "https://api.sandbox.paypal.com";

            var http = new HttpClient 
            {
                BaseAddress = new Uri(sandbox),
                Timeout = TimeSpan.FromSeconds(30)
            };
            return http;
        }

        private async Task<PayPalAccessToken> GetPayPalAccessTokenAsync(HttpClient http) 
        {
            var clientId = configuration["Paypal:clientId"];
            var secret = configuration["Paypal:secret"];

            byte[] bytes = Encoding.GetEncoding("iso-8859-1").GetBytes($"{clientId}:{secret}");

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "/v1/oauth2/token");
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(bytes));

            var form = new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials"
            };

            request.Content = new FormUrlEncodedContent(form);

            HttpResponseMessage response = await http.SendAsync(request);

            string content = await response.Content.ReadAsStringAsync();
            PayPalAccessToken accessToken = JsonConvert.DeserializeObject<PayPalAccessToken>(content);
            return accessToken;
        }

        private static async Task<PayPalCreatedResponse> CreatePaypalPaymentAsync(HttpClient http, PayPalAccessToken accessToken, double total, string currency) 
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "v1/payments/payment");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.access_token);

            var payment = JObject.FromObject(new 
            {
                intent = "sale",
                redirect_urls = new
                {
                    return_url = "",
                    cancel_url = ""
                },
                payer = new { payment_method = "paypal" },
                transactions = JArray.FromObject(new[]
                {
                    new
                    {
                        amount = new
                        {
                            total = total,
                            currency = currency
                        }
                    }
                })
            });

            request.Content = new StringContent(JsonConvert.SerializeObject(payment), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await http.SendAsync(request);

            string content = await response.Content.ReadAsStringAsync();
            PayPalCreatedResponse paypalPaymentCreated = JsonConvert.DeserializeObject<PayPalCreatedResponse>(content);
            return paypalPaymentCreated;
        } 

        private static async Task<PayPalExecutedResponse> ExecutePaypalPaymentAsync(HttpClient http, PayPalAccessToken accessToken, string paymentId, string payerId) 
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"v1/payments/payment/{paymentId}/execute");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.access_token);

            var payment = JObject.FromObject(new
            {
                payer_id = payerId
            });

            request.Content = new StringContent(JsonConvert.SerializeObject(payment), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await http.SendAsync(request);
            string content = await response.Content.ReadAsStringAsync();
            PayPalExecutedResponse executedPayment = JsonConvert.DeserializeObject<PayPalExecutedResponse>(content);
            return executedPayment;
        }
    }
}
