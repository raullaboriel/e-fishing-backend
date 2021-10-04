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

                if(o.password.Length < 8)
                {
                    return BadRequest("Invalid password");
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
        public async Task<ActionResult> PostLogin(JsonLogin data)
        {
            try
            {
                data.email = data.email.ToLower();
                var query = from e in Db.Users
                            where e.email.Equals(data.email)
                            select new JsonUser
                            {
                                id = e.id,
                                email = e.email,
                                name = e.name,
                                lastname = e.lastname,
                                password = e.password,
                                admin = e.admin
                            };
                JsonUser user = await query.SingleAsync();

                if (BCrypt.Net.BCrypt.Verify(data.password, user.password))
                {
                    var jwt = Jwt.generate(user.id);

                    if (data.remenberMe)
                    {
                        HttpContext.Response.Cookies.Append("jwt", jwt, new CookieOptions
                        {
                            HttpOnly = true,
                            SameSite = SameSiteMode.None,
                            Secure = true,
                            Expires = DateTime.Now.AddYears(15)
                        });
                    }
                    else
                    {
                        HttpContext.Response.Cookies.Append("jwt", jwt, new CookieOptions
                        {
                            HttpOnly = true,
                            SameSite = SameSiteMode.None,
                            Secure = true,
                        });
                    }

                    user.password = "";
                    return Ok(user);
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

        [HttpPost]
        public async Task<ActionResult> GetUser ()
        {
            try
            {
                var jwt = HttpContext.Request.Cookies["jwt"];
                var token = Jwt.verify(jwt);
                int userId = int.Parse(token.Issuer);

                User finded = await Db.Users.FindAsync(userId);

                var response = new
                {
                    name = finded.name,
                    lastname = finded.lastname,
                    email = finded.email,
                    admin = finded.admin
                };

                return Ok(response);
            }
            catch
            {
                return StatusCode(204, "No user logged");
            }
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            HttpContext.Response.Cookies.Delete("jwt", new CookieOptions()
            {
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                Secure = true
            });
            return Ok();
        }

        /*Account modifies methods*/
        [HttpPut("name")]
        public async Task<ActionResult> EditName(JsonEditName data)
        {
            try
            {
                var jwt = HttpContext.Request.Cookies["jwt"];
                var token = Jwt.verify(jwt);
                int userId = int.Parse(token.Issuer);

                User user = Db.Users.Find(userId);

                //Check if passed names are valid
                if (!data.IsValidName())
                {
                    return BadRequest();
                }

                user.name = data.name;
                user.lastname = data.lastname;

                await Db.SaveChangesAsync();
                return Ok(data);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPut("email")]
        public async Task<ActionResult> EditEmail(JsonEditEmail data)
        {
            try
            {
                var jwt = HttpContext.Request.Cookies["jwt"];
                var token = Jwt.verify(jwt);
                int userId = int.Parse(token.Issuer);

                User user = Db.Users.Find(userId);

                if(!BCrypt.Net.BCrypt.Verify(data.password, user.password))
                {
                    return Unauthorized("Invalid credentials");
                }

                if (!data.IsValidEmail())
                {
                    return BadRequest("Invalid email");
                }

                var query = from s in Db.Users
                            where s.email.Equals(data.email)
                            select new JsonUser
                            {
                                email = s.email
                            };

                List<JsonUser> list = await query.ToListAsync();

                if (list.Count() >= 1)
                {
                    return BadRequest("The e-mail is alredy taken");
                }

                //At this point everything shoud be Ok, so we make the changes
                user.email = data.email;
                await Db.SaveChangesAsync();
                return Ok(data.email);
            }
            catch
            {
                return BadRequest("No user logged");
            }
        }

        [HttpPut("password")]
        public async Task<ActionResult> EditPassword(JsonEditPassword data)
        {
            try
            {
                if (!data.IsValidPassowrd())
                {
                    return BadRequest("New password is not valid");
                }

                var jwt = HttpContext.Request.Cookies["jwt"];
                var token = Jwt.verify(jwt);
                int userId = int.Parse(token.Issuer);

                User user = Db.Users.Find(userId);

                if (!BCrypt.Net.BCrypt.Verify(data.currentPassword, user.password))
                {
                    return Unauthorized("Invalid credentials");
                }

                user.password = BCrypt.Net.BCrypt.HashPassword(data.newPassword);
                await Db.SaveChangesAsync();
                return Ok("Password successfully changed");
            }
            catch
            {
                return BadRequest("No user logged");
            }
        }

        [HttpPost("validate")]
        public async Task<ActionResult> Validate(JsonValidate data)
        {
            var query = from user in Db.Users
                        where user.email.Equals(data.email)
                        select new JsonValidate
                        {
                            email = user.email
                        };

            var results = await query.ToListAsync();

            if(results.Count() != 0)
            {
                return Ok("email_taken");
            }

            return Ok();
        }
    }
}