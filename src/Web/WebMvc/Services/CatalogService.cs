using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ShoesOnContainers.Web.WebMvc.Infrastructure;
using ShoesOnContainers.Web.WebMvc.Models;

namespace ShoesOnContainers.Web.WebMvc.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly IHttpClient _apiClient;
        private readonly ILogger<CatalogService> _logger;

        private readonly string _remoteServiceBaseUrl;
        private readonly IOptionsSnapshot<AppSettings> _settings;

        public CatalogService(IOptionsSnapshot<AppSettings> settings, IHttpClient httpClient,
            ILogger<CatalogService> logger)
        {
            _settings = settings;
            _apiClient = httpClient;
            _logger = logger;

            _remoteServiceBaseUrl = $"{_settings.Value.CatalogUrl}/api/catalog/";
        }

        public async Task<Catalog> GetCatalogItems(int page, int take, int? brand, int? type)
        {
            var allcatalogItemsUri =
                ApiPaths.Catalog.GetAllCatalogItems(_remoteServiceBaseUrl, page, take, brand, type);

            var dataString = await _apiClient.GetStringAsync(allcatalogItemsUri);

            var response = JsonConvert.DeserializeObject<Catalog>(dataString);

            return response;
        }

        public async Task<IEnumerable<SelectListItem>> GetBrands()
        {
            var getBrandsUri = ApiPaths.Catalog.GetAllBrands(_remoteServiceBaseUrl);

            var dataString = await _apiClient.GetStringAsync(getBrandsUri);

            var items = new List<SelectListItem>
            {
                new SelectListItem {Value = null, Text = "All", Selected = true}
            };
            var brands = JArray.Parse(dataString);

            foreach (var brand in brands.Children<JObject>())
            {
                items.Add(new SelectListItem
                {
                    Value = brand.Value<string>("id"),
                    Text = brand.Value<string>("brand")
                });
            }

            return items;
        }

        public async Task<IEnumerable<SelectListItem>> GetTypes()
        {
            var getTypesUri = ApiPaths.Catalog.GetAllTypes(_remoteServiceBaseUrl);

            var dataString = await _apiClient.GetStringAsync(getTypesUri);

            var items = new List<SelectListItem>
            {
                new SelectListItem {Value = null, Text = "All", Selected = true}
            };
            var brands = JArray.Parse(dataString);
            foreach (var brand in brands.Children<JObject>())
            {
                items.Add(new SelectListItem
                {
                    Value = brand.Value<string>("id"),
                    Text = brand.Value<string>("type")
                });
            }

            return items;
        }
    }
}