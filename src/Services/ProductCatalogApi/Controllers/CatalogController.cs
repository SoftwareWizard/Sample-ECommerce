using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ProductCatalogApi.Data;
using ProductCatalogApi.Domain;
using ProductCatalogApi.ViewModels;

namespace ProductCatalogApi.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class CatalogController : Controller
    {
        private const string REPLACE_URL = "http://externalcatalogbaseurltobereplaced";
        private readonly CatalogContext _catalogContext;
        private readonly string _baseUrl;

        public CatalogController(CatalogContext catalogContext, IOptionsSnapshot<CatalogSettings> settings)
        {
            _catalogContext = catalogContext;
            _baseUrl = settings.Value.ExternalCatalogBaseUrl;
            catalogContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> CatalogTypes()
        {
            var items = await _catalogContext.CatalogTypes.ToListAsync();
            return Ok(items);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> CatalogBrands()
        {
            var items = await _catalogContext.CatalogBrands.ToListAsync();
            return Ok(items);
        }

        [HttpGet]
        [Route("items/{id:int}")]
        public async Task<IActionResult> GetItemById(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var catalogItem = await _catalogContext.CatalogItems.SingleOrDefaultAsync(item => item.Id == id);

            if (catalogItem != null)
            {
                ChangeUrlPlaceholder(catalogItem);
                return Ok(catalogItem);
            }

            return NotFound();
        }

        // GET api/Catalog/items[?pageSize=4&pageIndex=3]
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Items([FromQuery] int pageSize = 6, [FromQuery] int pageIndex = 0)
        {
            var totalItems = await _catalogContext.CatalogItems.LongCountAsync();
            var itemsOnPage = await _catalogContext.CatalogItems
                .OrderBy(c => c.Name)
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync();

            itemsOnPage.ForEach(ChangeUrlPlaceholder);
            var model = new PaginatedItemsViewModel<CatalogItem>(pageSize, pageIndex, totalItems, itemsOnPage);
            return Ok(model);
        }

        // GET api/Catalog/items/withname/{name}[?pageSize=4&pageIndex=3]
        [HttpGet]
        [Route("[action]/withname/{name:minlength(1)}")]
        public async Task<IActionResult> Items(string name, [FromQuery] int pageSize = 6, [FromQuery] int pageIndex = 0)
        {
            var totalItems = await _catalogContext.CatalogItems
                .Where(item => item.Name.StartsWith(name))
                .LongCountAsync();

            var itemsOnPage = await _catalogContext.CatalogItems
                .Where(item => item.Name.StartsWith(name))
                .OrderBy(c => c.Name)
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync();

            itemsOnPage.ForEach(ChangeUrlPlaceholder);
            var model = new PaginatedItemsViewModel<CatalogItem>(pageSize, pageIndex, totalItems, itemsOnPage);
            return Ok(model);
        }

        // GET api/Catalog/items/type/{catalogTypeId}/brand/{catalogBrandId}[?pageSize=4&pageIndex=3]
        [HttpGet]
        [Route("[action]/type/{catalogTypeId}/brand/{catalogBrandId}")]
        public async Task<IActionResult> Items(int? catalogTypeId, int? catalogBrandId, [FromQuery] int pageSize = 6, [FromQuery] int pageIndex = 0)
        {
            var root = (IQueryable<CatalogItem>) _catalogContext.CatalogItems;

            root = catalogTypeId.HasValue 
                ? root.Where(c => c.CatalogTypeId == catalogTypeId) 
                : root;

           root = catalogBrandId.HasValue 
               ? root.Where(c => c.CatalogBrandId == catalogBrandId) 
               : root;

            var totalItems = await root
                .LongCountAsync();

            var itemsOnPage = await root
                .OrderBy(c => c.Name)
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync();

            itemsOnPage.ForEach(ChangeUrlPlaceholder);
            var model = new PaginatedItemsViewModel<CatalogItem>(pageSize, pageIndex, totalItems, itemsOnPage);
            return Ok(model);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> CreateProduct([FromBody] CatalogItem product)
        {
            var item = new CatalogItem
            {
                CatalogBrandId = product.CatalogBrandId,
                CatalogTypeId = product.CatalogTypeId,
                Description = product.Description,
                Name = product.Name,
                PictureFileName = product.PictureFileName,
                Price = product.Price
            };

            _catalogContext.CatalogItems.Add(item);
            await _catalogContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetItemById), new {id = item.Id}, item);
        }

        [HttpPut]
        [Route("items")]
        public async Task<IActionResult> UpdateProduct([FromBody] CatalogItem productToUpdate)
        {
            var catalogItem = await _catalogContext.CatalogItems
                .SingleOrDefaultAsync(item => item.Id == productToUpdate.Id);

            if (catalogItem == null)
            {
                return NotFound(new {Message = $"Item with id {productToUpdate.Id} not found"});
            }

            catalogItem = productToUpdate;
            _catalogContext.CatalogItems.Update(catalogItem);
            await _catalogContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetItemById), new { id = productToUpdate.Id }, productToUpdate);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _catalogContext.CatalogItems.SingleOrDefaultAsync(item => item.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            _catalogContext.CatalogItems.Remove(product);
            await _catalogContext.SaveChangesAsync();
            return NoContent();
        }

        private void ChangeUrlPlaceholder(CatalogItem item)
        {
            item.PictureUrl = item.PictureUrl?.Replace(REPLACE_URL, _baseUrl);
        }
    }
}