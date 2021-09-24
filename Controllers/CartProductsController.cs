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

        [HttpPost]
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
    }
}