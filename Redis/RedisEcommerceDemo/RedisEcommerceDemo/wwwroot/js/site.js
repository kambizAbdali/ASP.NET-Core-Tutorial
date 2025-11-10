// Real-time cache monitoring system
class CacheMonitor {
    constructor() {
        this.operations = [];
        this.isConnected = false;
        this.lastUpdate = null;
    }

    // Initialize cache monitoring
    init() {
        this.startConnectionMonitoring();
        this.startPerformanceMonitoring();
        this.bindCacheEvents();
    }

    // Monitor Redis connection status
    startConnectionMonitoring() {
        setInterval(async () => {
            try {
                const response = await fetch('/api/cart/health');
                const health = await response.json();
                this.updateConnectionStatus(health.IsRedisConnected);
            } catch (error) {
                this.updateConnectionStatus(false);
            }
        }, 5000);
    }

    // Monitor cache performance
    startPerformanceMonitoring() {
        setInterval(() => {
            this.updatePerformanceMetrics();
        }, 3000);
    }

    // Bind to cache-related events
    bindCacheEvents() {
        // Intercept form submissions
        document.addEventListener('submit', (e) => {
            if (e.target.method === 'post') {
                this.logOperation('WRITE', `Submitting data to cache: ${e.target.action}`);
            }
        });

        // Intercept AJAX requests
        const originalFetch = window.fetch;
        window.fetch = async (...args) => {
            const startTime = performance.now();
            const response = await originalFetch(...args);
            const endTime = performance.now();

            this.logOperation('READ', `API call: ${args[0]} (${Math.round(endTime - startTime)}ms)`);
            return response;
        };
    }

    // Update connection status
    updateConnectionStatus(connected) {
        this.isConnected = connected;
        const event = new CustomEvent('cacheStatusChange', {
            detail: { connected, timestamp: new Date() }
        });
        document.dispatchEvent(event);
    }

    // Update performance metrics
    updatePerformanceMetrics() {
        const event = new CustomEvent('cacheMetricsUpdate', {
            detail: {
                operations: this.operations.length,
                hitRate: this.calculateHitRate(),
                averageResponse: this.calculateAverageResponse()
            }
        });
        document.dispatchEvent(event);
    }

    // Log cache operation
    logOperation(type, message) {
        const operation = {
            type,
            message,
            timestamp: new Date(),
            id: Math.random().toString(36).substr(2, 9)
        };

        this.operations.unshift(operation);
        this.operations = this.operations.slice(0, 50); // Keep last 50

        const event = new CustomEvent('cacheOperation', { detail: operation });
        document.dispatchEvent(event);
    }

    // Calculate cache hit rate
    calculateHitRate() {
        const reads = this.operations.filter(op => op.type === 'READ').length;
        const writes = this.operations.filter(op => op.type === 'WRITE').length;
        return reads > 0 ? Math.round((reads / (reads + writes)) * 100) : 0;
    }

    // Calculate average response time
    calculateAverageResponse() {
        // This would be implemented with actual timing data
        return Math.random() * 100 + 50; // Simulated
    }
}

// Initialize cache monitor when page loads
document.addEventListener('DOMContentLoaded', function () {
    window.cacheMonitor = new CacheMonitor();
    window.cacheMonitor.init();
});