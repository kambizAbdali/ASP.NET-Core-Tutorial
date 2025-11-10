using System.Text.Json.Serialization;

namespace RedisEcommerceDemo.Models
{
    /// <summary>
    /// Health status information for shopping cart system
    /// Used by GetCartHealthAsync method to provide comprehensive health monitoring
    /// </summary>
    public class CartHealthStatus
    {
        // Basic Health Indicators
        public bool IsRedisConnected { get; set; }
        public bool CartExists { get; set; }
        public bool IsHealthy { get; set; }

        // Cart Content Information
        public int ItemsCount { get; set; }
        public decimal TotalValue { get; set; }
        public DateTime LastUpdated { get; set; }

        // Error Information
        public string? ErrorMessage { get; set; }
        public DateTime CheckTimestamp { get; set; } = DateTime.UtcNow;

        // Performance Metrics (can be extended)
        public TimeSpan ResponseTime { get; set; }

        // Calculated Properties for UI Display
        [JsonIgnore]
        public string HealthStatus => IsHealthy ? "Healthy" : "Unhealthy";

        [JsonIgnore]
        public string RedisStatus => IsRedisConnected ? "Connected" : "Disconnected";

        [JsonIgnore]
        public string CartStatus => CartExists ? "Exists" : "Not Found";

        [JsonIgnore]
        public string StatusColor
        {
            get
            {
                if (!IsRedisConnected) return "danger";
                if (!CartExists) return "warning";
                if (!IsHealthy) return "warning";
                return "success";
            }
        }

        [JsonIgnore]
        public string StatusIcon
        {
            get
            {
                if (!IsRedisConnected) return "❌";
                if (!CartExists) return "⚠️";
                if (!IsHealthy) return "⚠️";
                return "✅";
            }
        }

        [JsonIgnore]
        public string Summary
        {
            get
            {
                if (!IsRedisConnected) return "Redis connection failed";
                if (!CartExists) return "Cart does not exist";
                if (!IsHealthy) return "Cart contains invalid items";
                return "All systems operational";
            }
        }
    }
}