using System;
namespace efishingAPI.JsonModels
{
    public class JsonProduct
    {
        public int id { get; set; }
        public string name { get; set; }
        public string brand { get; set; }
        public decimal price { get; set; }
        public string model { get; set; }
        public string description { get; set; }
        public string category { get; set; }
        public decimal size { get; set; }
        public decimal weight { get; set; }
        public int stock { get; set; }
    }
}
