using System;
namespace efishingAPI.JsonModels
{
    public class JsonUser
    {
        public int id { get; set; }
        public string name { get; set; }
        public string lastname { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public bool admin { get; set; }


        public bool ValidEmail ()
        {
            if (this.email.Contains("@") && this.email.Contains(".") && !this.email.Trim().Equals(""))
            {
                return true;
            }
            return false;
        }
    }
}
