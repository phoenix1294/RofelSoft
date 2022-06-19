using System.Collections.Generic;

namespace RofelSoft.Models
{
    class Product
    {
        public double BeginPrice { get; set; }
        public double ActualPrice { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public int Id { get; set; }
        public bool StaticPrice { get; set; }
        public bool FirstSold { get; set; }
        public string ItemGroup { get; set; }
        public List<double> PriceHistory { get; set; }
    }
}
