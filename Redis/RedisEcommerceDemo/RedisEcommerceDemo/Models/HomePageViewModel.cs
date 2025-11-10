namespace RedisEcommerceDemo.Models
{
    public class HomePageViewModel
    {
        public List<Product> FeaturedProducts { get; set; } = new List<Product>();
        public List<Product> BestSellingProducts { get; set; } = new List<Product>();
        public List<Product> NewArrivals { get; set; } = new List<Product>();
        public Dictionary<string, int> CategoryStats { get; set; } = new Dictionary<string, int>();
        public DateTime CacheTimestamp { get; set; }
        public string DataSource { get; set; } = "database";
    }
}
