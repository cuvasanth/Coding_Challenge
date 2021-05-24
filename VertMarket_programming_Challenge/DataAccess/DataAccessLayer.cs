using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using VertMarket_programming_Challenge.Interfaces;

namespace VertMarket_programming_Challenge.DataAccess
{
   public class DataAccessLayer : IMagazineStoreSa
    {
        private readonly Model.ConfigurationModel
            _configuration;
       
        private readonly HttpClient _client;
       
        private const string ContentTypeJson = "application/json";
       
        private string _token;

        public DataAccessLayer(IOptions<Model.ConfigurationModel> options) :
            this(options, new HttpClient())
        { }
        public DataAccessLayer(IOptions<Model.ConfigurationModel> options, HttpClient httpClient)
        {
            _configuration = options.Value;
            _client = httpClient;
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ContentTypeJson));
            _client.Timeout = TimeSpan.FromSeconds(_configuration.TimeOutInSeconds);
            if (string.IsNullOrEmpty(_configuration.BaseUrl))
            {
                throw new ArgumentException(nameof(_configuration.BaseUrl));
            }
        }

        public async Task<List<string>> GetCategories()
        {
            var url = $"{_configuration.BaseUrl}/api/categories/{await GetToken()}";
            var response = await _client.SendAsync(new HttpRequestMessage(HttpMethod.Get, url));
            var responseContent = await response.Content.ReadAsStringAsync();
            if (response?.StatusCode is HttpStatusCode.OK)
                return JsonConvert.DeserializeObject<Model.CategoriesResponse>(responseContent)?.Data ??
                       throw new Exception("Failed to get categories");

            throw new Exception("Failed to get categories");
        }

        public async Task<List<Model.Subscriber>> GetSubscribers()
        {
            var url = $"{_configuration.BaseUrl}/api/subscribers/{await GetToken()}";
            var response = await _client.SendAsync(new HttpRequestMessage(HttpMethod.Get, url));
            var responseContent = await response.Content.ReadAsStringAsync();
            if (response?.StatusCode is HttpStatusCode.OK)
                return JsonConvert.DeserializeObject<Model.SubscriberResponse>(responseContent)?.Data ??
                       throw new Exception("Failed to get subscribers");

            throw new Exception("Failed to get subscribers");
        }

        /// <summary>
        /// Gets the magazines.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <returns></returns>
        public async Task<List<Model.Magazine>> GetMagazines(string category)
        {
            var url = $"{_configuration.BaseUrl}/api/magazines/{await GetToken()}/{category}";
            var response = await _client.SendAsync(new HttpRequestMessage(HttpMethod.Get, url));
            var responseContent = await response.Content.ReadAsStringAsync();
            if (response?.StatusCode is HttpStatusCode.OK)
                return JsonConvert.DeserializeObject<Model.MagazineResponse>(responseContent)?.Data ??
                       throw new Exception("Failed to get magazines");

            throw new Exception("Failed to get magazines");
        }

        /// <summary>
        /// Submits the answer.
        /// </summary>
        /// <param name="subcribers">The subcribers.</param>
        /// <returns></returns>
        public async Task<Model.SubmissionResponse> SubmitAnswer(IEnumerable<string> subcribers)
        {
            var url = $"{_configuration.BaseUrl}/api/answer/{await GetToken()}";
            var data = JsonConvert.SerializeObject(new Model.ApiAnswer { Subscribers = subcribers });
            var request = new HttpRequestMessage(HttpMethod.Post, url) { Content = new StringContent(data, Encoding.UTF8, ContentTypeJson) }; ;
            var response = await _client.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();
            if (response?.StatusCode is HttpStatusCode.OK)
                return JsonConvert.DeserializeObject<Model.SubmissionResponse>(responseContent) ??
                       throw new Exception("Failed to sumbit answer");

            throw new Exception("Failed to sumbit answer");

        }



        private async Task<string> GetToken()
        {
            return _token ?? (_token = await GetTokenFromService());
            async Task<String> GetTokenFromService()
            {
                var url = $"{_configuration.BaseUrl}/api/token";
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                var response = await _client.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();
                if (response?.StatusCode is HttpStatusCode.OK)
                    return JsonConvert.DeserializeObject<Model.ApiResponse>(responseContent)?.token ??
                           throw new Exception("Failed to get token");

                throw new Exception("Failed to get token");
            }
        }


        public async Task<Model.SubmissionResponse> IdentifySubcribersToAllCategories()
        {
            var categories = await GetCategories();
            var subscribers = await GetSubscribers();
            var magazineTasks = new List<Task<List<Model.Magazine>>>();
            foreach (var category in categories)
            {
                magazineTasks.Add(GetMagazines(category));
            }

            Task.WaitAll(magazineTasks.ToArray());
            var subcribersToAllCategories = from subscriber in subscribers.SelectMany(s => s.MagazineIds,
                    (subscriber, magazineId) => new { SubscriberId = subscriber.Id, MagazineId = magazineId })
                                            join magazine in magazineTasks.SelectMany(x => x.Result)
                                                on subscriber.MagazineId equals magazine.Id
                                            select new
                                            {
                                                subscriber.SubscriberId,
                                                magazine.Category
                                            }
                into subscriberCategory
                                            group subscriberCategory by subscriberCategory.SubscriberId
                into subscriberGrouping
                                            select new
                                            {
                                                SubcriberId = subscriberGrouping.Key,
                                                CategoryCount = subscriberGrouping.Distinct().Count()
                                            }
                into subscriberCategoryCount
                                            where subscriberCategoryCount.CategoryCount == categories.Count
                                            select subscriberCategoryCount.SubcriberId;

            return await SubmitAnswer(subcribersToAllCategories);
        }


    }
}
