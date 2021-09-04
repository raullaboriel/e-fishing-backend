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
    public class UsersController : ControllerBase
    {
        private readonly eFishing Db;
        private readonly Jwt Jwt;
        public UsersController(eFishing dbContext, Jwt jwtService)
        {
            this.Db = dbContext;
            this.Jwt = jwtService;
        }

        [HttpPost("register")]
        public async Task<ActionResult> PostRegister(JsonUser o)
        {
            var query = from s in Db.Users
                        where s.email.Equals(o.email)
                        select new JsonUser { };


            try
            {
                List<JsonUser> u = await query.ToListAsync();

                if (u.Count > 0)
                {
                    return BadRequest("Email is not longer available.");
                }

                if (!o.ValidEmail())
                {
                    return BadRequest("Email its invalid");
                }


                User user = new User();
                user.name = o.name;
                user.lastname = o.lastname;
                user.email = o.email.ToLower();
                user.password = BCrypt.Net.BCrypt.HashPassword(o.password);
                user.admin = false;

                await Db.Users.AddAsync(user);
                if (await Db.SaveChangesAsync() > 0)
                {
                    user.password = "";
                    return Ok(user);
                }
                else
                {
                    return StatusCode(400, new { message = "Invalid data" });
                }
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult> PostLogin(JsonLogin o)
        {
            o.email = o.email.ToLower();
            var query = from e in Db.Users
                        where e.email.Equals(o.email)
                        select new JsonUser
                        {
                            id = e.id,
                            email = e.email,
                            name = e.name,
                            lastname = e.lastname,
                            password = e.password,
                            admin = e.admin
                        };

            try
            {
                JsonUser user = await query.SingleAsync();
                if (BCrypt.Net.BCrypt.Verify(o.password, user.password))
                {
                    var jwt = Jwt.generate(user.id);
                    HttpContext.Response.Cookies.Append("jwt", jwt, new CookieOptions {
                        HttpOnly = true,
                        SameSite = SameSiteMode.None,
                        Secure = true,
                        Expires = DateTime.Now.AddMinutes(30)
                    });
                    user.password = "";
                    var response = new {
                        name = user.name,
                        lastname = user.lastname,
                        email = user.email,
                        admin = user.admin
                    };
                    return Ok(response);
                }
                else
                {
                    return Unauthorized("Invalid credentials");
                }
            }
            catch
            {
                return StatusCode(500, "Error while trying to login");
            }
        }
    }
}