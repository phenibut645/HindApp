using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HindApp.Models
{
    public class ProductPriceDisplayModel
    {
        public int Id { get; set; }
        public string StoreName { get; set; }
        public double Price { get; set; }
        public DateTime LastUpdated { get; set; }
        public bool IsFavorite { get; set; }
    }

}
