﻿using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ShoesOnContainers.Web.WebMvc.Infrastructure
{
    public class CustomHttpClient: IHttpClient
    {
        private HttpClient _client;
        private ILogger<CustomHttpClient> _logger;

        public CustomHttpClient(ILogger<CustomHttpClient> logger)
        {
            _client = new HttpClient();
            _logger = logger;
        }

        public string Token { get; set; }

        public async Task<string> GetStringAsync(string uri)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, uri);
            requestMessage.Headers.SetToken(Token);
            var response = await _client.SendAsync(requestMessage);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }

            throw new InvalidOperationException();
        }

        private async Task<HttpResponseMessage> DoPostPutAsync<T>(HttpMethod method, string uri, T item)
        {
            if (method != HttpMethod.Post && method != HttpMethod.Put)
            {
                throw new ArgumentException("Value must be either post or put.", nameof(method));
            }

            // a new StringContent must be created for each retry 
            // as it is disposed after each call

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = new StringContent(JsonConvert.SerializeObject(item), System.Text.Encoding.UTF8, "application/json")
            };
            //if (requestId != null)
            //{
            //    requestMessage.Headers.Add("x-requestid", requestId);
            //}

            requestMessage.Headers.SetToken(Token);
            var response = await _client.SendAsync(requestMessage);

            // raise exception if HttpResponseCode 500 
            // needed for circuit breaker to track fails

            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new HttpRequestException();
            }

            return response;
        }

        public async Task<HttpResponseMessage> PostAsync<T>(string uri, T item)
        {
            return await DoPostPutAsync(HttpMethod.Post, uri, item);
        }

        public async Task<HttpResponseMessage> PutAsync<T>(string uri, T item)
        {
            return await DoPostPutAsync(HttpMethod.Put, uri, item);
        }

        public async Task<HttpResponseMessage> DeleteAsync(string uri)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);
            requestMessage.Headers.SetToken(Token);
            return await _client.SendAsync(requestMessage);
        }
    }

    public static class AuthenticationHeaderValueExtension
    {
        public static void SetToken(this HttpRequestHeaders headers, string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }
    }
}
 
