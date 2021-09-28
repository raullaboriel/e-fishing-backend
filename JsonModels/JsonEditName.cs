using System;
namespace efishingAPI.JsonModels
{
    public class JsonEditName
    {
        public string name { get; set; }
        public string lastname { get; set; }

        public bool IsValidName()
        {
            if(this.name.Trim().Length <=1 && this.lastname.Trim().Length <= 1)
            {
                return false;
            }

            return true;
        }
    }
}
