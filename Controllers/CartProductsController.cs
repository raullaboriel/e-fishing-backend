using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using efishingAPI.Context;
using efishingAPI.helpers;
using efishingAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace efishingAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CartProductsController : ControllerBase
    {
        private eFishing DbContext;
        private Jwt Jwt;
        public CartProductsController(eFishing Db, Jwt _jwt)
        {
            DbContext = Db;
            Jwt = _jwt;
        }

        [HttpPost("Cart")]
        public async Task<ActionResult> GetCartProducts()
        {
            try
            {
                var jwt = HttpContext.Request.Cookies["jwt"];
                var token = Jwt.verify(jwt);
                int userId = int.Parse(token.Issuer);

                var results = from CartProduct in DbContext.CartProducts
                              join Product in DbContext.Products
                              on CartProduct.id_product equals Product.id into joined
                              from j in joined.DefaultIfEmpty()
                              where CartProduct.id_user == userId
                              select new
                              {
                                  id = j.id,
                                  name = j.name,
                                  brnd = j.brand,
                                  price = j.price,
                                  model = j.model,
                                  description = j.description,
                                  category = j.category,
                                  weight = j.weight,
                                  stock = j.stock,
                                  size = j.size,
                                  amount = CartProduct.amount
                              };
                var Cart = await results.ToListAsync();
                return Ok(Cart);
            }
            catch
            {
                return BadRequest("Invalid token");
            }
        }

        [HttpPut]
        public async Task<ActionResult> PutCartProduct(CartProduct product)
        {
            try
            {
                var jwt = HttpContext.Request.Cookies["jwt"];
                var token = Jwt.verify(jwt);
                int userId = int.Parse(token.Issuer);

                var query = from cartProduct in DbContext.CartProducts
                            where cartProduct.id_user == userId && cartProduct.id_product == product.id_product
                            select new
                            {
                                id = cartProduct.id
                            };
                var result = await query.SingleAsync();
                CartProduct finded = await DbContext.CartProducts.FindAsync(result.id);
                finded.amount = product.amount;

                await DbContext.SaveChangesAsync();
                return Ok();
            }
            catch
            {
                return BadRequest("Error while trying to modify cart");
            }
        }

        [HttpPost]
        public async Task<ActionResult> PostCartProduct(CartProduct cartProduct)
        {
            try
            {
                var jwt = HttpContext.Request.Cookies["jwt"];
                var token = Jwt.verify(jwt);
                int userId = int.Parse(token.Issuer);

                cartProduct.id_user = userId;

                await DbContext.CartProducts.AddAsync(cartProduct);
                await DbContext.SaveChangesAsync();
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpDelete("{idProduct}")]
        public async Task<ActionResult> DeleteCartProduct(int idProduct)
        {
            try
            {
                var jwt = HttpContext.Request.Cookies["jwt"];
                var token = Jwt.verify(jwt);
                int userId = int.Parse(token.Issuer);

                var query = from cp in DbContext.CartProducts
                            where cp.id_user == userId && cp.id_product == idProduct
                            select new
                            {
                                id = cp.id
                            };
                var result = await query.SingleAsync();
                CartProduct finded = await DbContext.CartProducts.FindAsync(result.id);
                DbContext.CartProducts.Remove(finded);
                await DbContext.SaveChangesAsync();
                return Ok(finded);
            }
            catch
            {
                return BadRequest();
            }

        }
    }
}