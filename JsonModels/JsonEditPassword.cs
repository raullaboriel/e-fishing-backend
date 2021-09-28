using System;
namespace efishingAPI.JsonModels
{
    public class JsonEditPassword
    {
        public string currentPassword { get; set; }
        public string newPassword { get; set; }
        public string confirmPassword { get; set; }

        public bool IsValidPassowrd()
        {
            if(this.newPassword != confirmPassword)
            {
                return false;
            }else if (this.newPassword.Length < 8)
            {
                return false;
            }

            return true;
        }
    }
}
