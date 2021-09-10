using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure;
using efishingAPI.helpers;
using efishingAPI.Context;
using efishingAPI.Models;
using System.IO;

namespace efishingAPI.Controllers
{
    [Route("[controller]/{id}")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private Jwt Jwt;
        private readonly BlobServiceClient Client;
        private eFishing DbContext;

        public ImagesController(BlobServiceClient c, Jwt JwtService, eFishing Db)
        {
            Client = c;
            Jwt = JwtService;
            DbContext = Db;
        }

        [HttpGet]
        public async Task<ActionResult> GetProductUris(string id)
        {
            BlobContainerClient containerClient = Client.GetBlobContainerClient("images");
            string container_url = containerClient.Uri.AbsoluteUri;
            var response = containerClient.GetBlobsAsync(prefix: id+"/");
            //var response = containerClient.GetBlobsAsync();

            List<string> res = new List<string>();
            await foreach (var item in response)
            {
                //here you can concatenate the container url with blob name
                string blob_url = container_url + "/" + item.Name;
                res.Add(blob_url);
            }
            return Ok(new { uris = res });
        }

        
        [HttpPost]
        public async Task<ActionResult> Upload(IFormFile file, string id)
        {
            var jwt = HttpContext.Request.Cookies["jwt"];
            var token = Jwt.verify(jwt);
            int userId = int.Parse(token.Issuer);

            User finded = await DbContext.Users.FindAsync(userId);

            if (!finded.admin)
            {
                return Unauthorized("You don't have permissions");
            }

            try
            {
                FileInfo fi = new FileInfo(file.FileName);
                string fileName =
                    id + new Random().Next(1, 999999).ToString() + (char)new Random().Next(34, 124) + fi.Extension;

                var blobContainer = Client.GetBlobContainerClient("images");
                var blobClient = blobContainer.GetBlobClient(id + "/" + fileName);
                await blobClient.UploadAsync(file.OpenReadStream());
                return Ok("Image uploaded successfully");
            }
            catch
            {
                return StatusCode(500, "Error while trying to upload");
            }
        }
        


        /*
        [HttpPost]
        public async Task<ActionResult> UploadImages(IFormFileCollection files, string id)
        {
            var jwt = HttpContext.Request.Cookies["jwt"];
            var token = Jwt.verify(jwt);
            int userId = int.Parse(token.Issuer);

            User finded = await DbContext.Users.FindAsync(userId);

            if (!finded.admin)
            {
                return Unauthorized("You don't have permissions");
            }

            try
            {
                var blobContainer = Client.GetBlobContainerClient("images");
                int cont = 1;
                foreach (IFormFile file in files)
                {
                    FileInfo fi = new FileInfo(file.FileName);
                    string fileName = id+cont.ToString()+fi.Extension;
                    var blobClient = blobContainer.GetBlobClient(id + "/" + fileName);
                    await blobClient.UploadAsync(file.OpenReadStream());
                    cont++;
                }
                return Ok("Images uploaded successfully");
            }
            catch
            {
                return StatusCode(500, "Error while trying to upload");
            }
        }
        */
        
    }
}