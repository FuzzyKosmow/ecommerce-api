using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        [DefaultValue(0)]
        public decimal ImportPrice { get; set; }
        public List<string> Colors { get; set; }
        public List<string> StorageOptions { get; set; }
        // Final price of product is calculated by Price * StoragePriceModifiers[StorageOptions.IndexOf(SelectedStorage)]
        // The modifier should be bigger than 1.0. And is default to 1.0
        public List<decimal> StorageModifiers { get; set; }
        public List<string> Images { get; set; }


        public string Description { get; set; }
        //Key - Value pair for product specifications
        public string SpecificationsJson { get; set; }
        // Marked by admin or system
        [DefaultValue(false)]
        public bool IsBestSeller { get; set; }
        // Marked by admin
        [DefaultValue(false)]
        public bool IsFeatured { get; set; }
        public DateTime ReleaseDate { get; set; }

        public DateTime CreatedAt { get; set; }
        // Marked by admin or system
        [DefaultValue(false)]
        public bool IsNewArrival { get; set; }

        //Ignore this property. Used for mapping

        public ICollection<Category> Categories { get; set; } = new List<Category>();

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