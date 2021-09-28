using System;
namespace efishingAPI.JsonModels
{
    public class JsonEditEmail
    {
        public string email { get; set; }
        public string password { get; set; }

        public bool IsValidEmail()
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(this.email);
                return addr.Address == this.email;
            }
            catch
            {
                return false;
            }
        }
    }
}
