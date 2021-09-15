using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using efishingAPI.Context;
using efishingAPI.helpers;
using efishingAPI.JsonModels;
using efishingAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace efishingAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]

    public class ProductsController : ControllerBase
    {
        private eFishing DbContext;
        private Jwt Jwt;
        public ProductsController(eFishing Db, Jwt JwtService)
        {
            DbContext = Db;
            Jwt = JwtService;
        }

        [HttpGet]
        public async Task<ActionResult<List<JsonProduct>>> GetProducts()
        {
            try
            {
                var query = from e in DbContext.Products
                            select new JsonProduct
                            {
                                id = e.id,
                                name = e.name,
                                brand = e.brand,
                                price = e.price,
                                model = e.model,
                                description = e.description,
                                category = e.category,
                                size = e.size,
                                weight = e.weight,
                                stock = e.stock
                            };
                List<JsonProduct> ProductList = await query.ToListAsync();
                return Ok(ProductList);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<JsonProduct>> GetProductByID(int id)
        {
            var query = from e in DbContext.Products
                        where e.id == id
                        select new JsonProduct
                        {
                            id = e.id,
                            name = e.name,
                            brand = e.brand,
                            price = e.price,
                            model = e.model,
                            description = e.description,
                            category = e.category,
                            size = e.size,
                            weight = e.weight,
                            stock = e.stock
                        };
            try
            {
                JsonProduct product = await query.SingleAsync();
                return Ok(product);
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<ActionResult<JsonProduct>> PostProduct(JsonProduct e)
        {
            var jwt = HttpContext.Request.Cookies["jwt"];
            var token = Jwt.verify(jwt);
            int userId = int.Parse(token.Issuer);

            User finded = await DbContext.Users.FindAsync(userId);

            if (!finded.admin)
            {
                return Unauthorized("You don't have permissions");
            }

            Product product = new Product();
            product.name = e.name;
            product.brand = e.brand;
            product.price = e.price;
            product.category = e.category;
            product.model = e.model;
            product.description = e.description;
            product.size = e.size;
            product.weight = e.weight;
            product.stock = e.stock;

            try
            {
                await DbContext.Products.AddAsync(product);
                await DbContext.SaveChangesAsync();
                return Ok(product.id);
            }
            catch
            {
                return StatusCode(401, new { message = "Error while trying to add product"});
            }
        }

        [HttpGet("categories")]
        public async Task<ActionResult> GetCategories()
        {
            try
            {
                var query = (from e in DbContext.Products
                             select e.category).Distinct();
                List<string> categories = await query.ToListAsync();
                return Ok(new { categories = categories });
            }
            catch
            {
                return StatusCode(500, "Error while trying to get categories");
            }

        }

        [HttpGet("by")]
        public async Task<ActionResult> GetProductsBy(string category)
        {
            try
            {
                var query = from e in DbContext.Products
                            where e.category.Equals(category)
                            select new JsonProduct
                            {
                                id = e.id,
                                name = e.name,
                                brand = e.brand,
                                price = e.price,
                                model = e.model,
                                description = e.description,
                                category = e.category,
                                size = e.size,
                                weight = e.weight,
                                stock = e.stock
                            };
                List<JsonProduct> ProductList = await query.ToListAsync();
                return Ok(ProductList);
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}