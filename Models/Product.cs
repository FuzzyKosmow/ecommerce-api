using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ecommerce_api.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public float Rating { get; set; }
        public bool Availability { get; set; }
        public List<string> Colors { get; set; }
        public List<string> StorageOptions { get; set; }
        public List<string> Images { get; set; }
        public string Description { get; set; }
        //Key - Value pair for product specifications
        public string SpecificationsJson { get; set; }
        //Not mapped 
        //Returned value as a map
        public Dictionary<string, string> Specifications
        {
            get
            {
                return string.IsNullOrEmpty(SpecificationsJson)
                    ? new Dictionary<string, string>()
                    : JsonSerializer.Deserialize<Dictionary<string, string>>(SpecificationsJson);
            }
            set
            {
                SpecificationsJson = JsonSerializer.Serialize(value);
            }
        }
    }



}