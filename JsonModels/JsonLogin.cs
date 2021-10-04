using System;
namespace efishingAPI.JsonModels
{
    public class JsonLogin
    {
        public string email { get; set; }
        public string password { get; set; }
        public bool remenberMe { get; set; }
    }
}
