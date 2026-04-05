using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using StackExchange.Redis;
using System.Net;
using System.Text.Json;

namespace lms.buildingblocks.middleware
{
    /// <summary>
    /// Redis-based distributed rate limiting middleware supporting multiple algorithms
    /// </summary>
    public class RedisRateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IDatabase _redisDb;
        private readonly RateLimitConfiguration _configuration;

        /// <summary>
        /// Configuration for rate limiting parameters
        /// </summary>
        public class RateLimitConfiguration
        {
            /// <summary>Maximum number of requests allowed</summary>
            public int MaxRequests { get; set; } = 100;

            /// <summary>Time window in seconds</summary>
            public int WindowSeconds { get; set; } = 60;

            /// <summary>Rate limiting algorithm to use</summary>
            public RateLimitAlgorithm Algorithm { get; set; } = RateLimitAlgorithm.FixedWindow;
        }

        /// <summary>
        /// Available rate limiting algorithms
        /// </summary>
        public enum RateLimitAlgorithm
        {
            FixedWindow,
            SlidingWindow,
            TokenBucket,
            LeakyBucket
        }

        /// <summary>
        /// Constructor for Redis-based rate limiting middleware
        /// </summary>
        /// <param name="next">Next middleware in the pipeline</param>
        /// <param name="redisConnection">Redis connection multiplexer</param>
        /// <param name="configuration">Rate limiting configuration</param>
        public RedisRateLimitingMiddleware(
            RequestDelegate next,
            IConnectionMultiplexer redisConnection,
            RateLimitConfiguration? configuration = null)
        {
            _next = next;
            _redisDb = redisConnection.GetDatabase();
            _configuration = configuration ?? new RateLimitConfiguration();
        }

        /// <summary>
        /// Middleware invocation method to apply rate limiting
        /// </summary>
        /// <param name="context">HTTP context</param>
        /// <returns>Task representing the asynchronous operation</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            string? clientKey = GetClientKey(context);

            if (string.IsNullOrEmpty(clientKey))
            {
                await _next(context);
                return;
            }

            bool isAllowed = await CheckRateLimit(clientKey);

            if (isAllowed)
            {
                await _next(context);
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.Response.WriteAsJsonAsync(new
                {
                    message = "Rate limit exceeded. Please try again later.",
                    retryAfter = _configuration.WindowSeconds
                });
            }
        }

        /// <summary>
        /// Generates a unique key for rate limiting based on client details
        /// </summary>
        /// <param name="context">HTTP context</param>
        /// <returns>Unique client identifier</returns>
        private static string? GetClientKey(HttpContext context)
        {
            // Prioritize authentication if available
            string? userId = context.User.FindFirst(c => c.Type == "sub")?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                return $"ratelimit:user:{userId}";
            }

            // Fallback to IP address
            IPAddress? ipAddress = context.Connection.RemoteIpAddress;
            return ipAddress != null
                ? $"ratelimit:ip:{ipAddress}"
                : null;
        }

        /// <summary>
        /// Checks rate limit using the configured algorithm
        /// </summary>
        /// <param name="clientKey">Unique client identifier</param>
        /// <returns>Boolean indicating if request is allowed</returns>
        private async Task<bool> CheckRateLimit(string clientKey)
        {
            return _configuration.Algorithm switch
            {
                RateLimitAlgorithm.FixedWindow => await FixedWindowRateLimit(clientKey),
                RateLimitAlgorithm.SlidingWindow => await SlidingWindowRateLimit(clientKey),
                RateLimitAlgorithm.TokenBucket => await TokenBucketRateLimit(clientKey),
                RateLimitAlgorithm.LeakyBucket => await LeakyBucketRateLimit(clientKey),
                _ => true
            };
        }

        /// <summary>
        /// Implements Fixed Window rate limiting using Redis
        /// </summary>
        private async Task<bool> FixedWindowRateLimit(string clientKey)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            string windowKey = $"{clientKey}:fixed:{now.ToUnixTimeSeconds() / _configuration.WindowSeconds}";

            long requestCount = await _redisDb.StringIncrementAsync(windowKey, 1);
            await _redisDb.KeyExpireAsync(windowKey, TimeSpan.FromSeconds(_configuration.WindowSeconds));

            return requestCount <= _configuration.MaxRequests;
        }

        /// <summary>
        /// Implements Sliding Window rate limiting using Redis
        /// </summary>
        private async Task<bool> SlidingWindowRateLimit(string clientKey)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            string windowKey = $"{clientKey}:sliding";

            // Store timestamps as a sorted set
            await _redisDb.SortedSetAddAsync(windowKey, now.ToUnixTimeMilliseconds().ToString(), now.ToUnixTimeMilliseconds());

            // Remove timestamps outside the window
            long oldestAllowedTime = now.AddSeconds(-_configuration.WindowSeconds).ToUnixTimeMilliseconds();
            await _redisDb.SortedSetRemoveRangeByScoreAsync(windowKey, 0, oldestAllowedTime);

            long requestCount = await _redisDb.SortedSetLengthAsync(windowKey);
            await _redisDb.KeyExpireAsync(windowKey, TimeSpan.FromSeconds(_configuration.WindowSeconds));

            return requestCount <= _configuration.MaxRequests;
        }

        /// <summary>
        /// Implements Token Bucket rate limiting using Redis
        /// </summary>
        private async Task<bool> TokenBucketRateLimit(string clientKey)
        {
            string bucketKey = $"{clientKey}:tokenbucket";
            DateTimeOffset now = DateTimeOffset.UtcNow;

            // Retrieve or initialize bucket state
            BucketState bucketState = await GetBucketState(bucketKey);

            // Calculate tokens to add based on time elapsed
            int tokensToAdd = CalculateTokens(bucketState, now);

            // Update bucket state if tokens can be added
            if (tokensToAdd > 0)
            {
                bucketState.Tokens = Math.Min(
                    _configuration.MaxRequests,
                    bucketState.Tokens + tokensToAdd
                );
                bucketState.LastUpdateTime = now;
            }

            // Check if request can be processed
            if (bucketState.Tokens > 0)
            {
                bucketState.Tokens--;
                await SaveBucketState(bucketKey, bucketState);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Implements Leaky Bucket rate limiting using Redis
        /// </summary>
        private async Task<bool> LeakyBucketRateLimit(string clientKey)
        {
            string bucketKey = $"{clientKey}:leakybucket";
            DateTimeOffset now = DateTimeOffset.UtcNow;

            // Retrieve or initialize bucket state
            BucketState bucketState = await GetBucketState(bucketKey);

            // Calculate tokens to remove based on time elapsed
            int tokensToRemove = CalculateTokens(bucketState, now);

            // Update bucket state by removing tokens
            bucketState.Tokens = Math.Max(0, bucketState.Tokens - tokensToRemove);
            bucketState.LastUpdateTime = now;

            // Check if request can be processed
            if (bucketState.Tokens < _configuration.MaxRequests)
            {
                bucketState.Tokens++;
                await SaveBucketState(bucketKey, bucketState);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Calculates tokens to add or remove based on time elapsed
        /// </summary>
        private int CalculateTokens(BucketState state, DateTimeOffset now)
        {
            TimeSpan elapsedTime = now - state.LastUpdateTime;
            return (int)(elapsedTime.TotalSeconds * (_configuration.MaxRequests / (double)_configuration.WindowSeconds));
        }

        /// <summary>
        /// Retrieves bucket state from Redis
        /// </summary>
        private async Task<BucketState> GetBucketState(string bucketKey)
        {
            RedisValue serializedState = await _redisDb.StringGetAsync(bucketKey);

            if (serializedState.HasValue)
            {
                // explicit conversion to string to avoid overload ambiguity
                string json = serializedState.ToString();
                return JsonSerializer.Deserialize<BucketState>(json) ?? new BucketState
                {
                    Tokens = _configuration.MaxRequests,
                    LastUpdateTime = DateTimeOffset.UtcNow
                };
            }

            return new BucketState
            {
                Tokens = _configuration.MaxRequests,
                LastUpdateTime = DateTimeOffset.UtcNow
            };
        }

        /// <summary>
        /// Saves bucket state to Redis
        /// </summary>
        private async Task SaveBucketState(string bucketKey, BucketState state)
        {
            string serializedState = JsonSerializer.Serialize(state);
            await _redisDb.StringSetAsync(bucketKey, serializedState, TimeSpan.FromSeconds(_configuration.WindowSeconds));
        }

        /// <summary>
        /// Represents the state of a rate limiting bucket
        /// </summary>
        private sealed class BucketState
        {
            /// <summary>Current number of tokens</summary>
            public int Tokens { get; set; }

            /// <summary>Timestamp of last update</summary>
            public DateTimeOffset LastUpdateTime { get; set; }
        }
    }

    /// <summary>
    /// Extension method to easily add Redis rate limiting to the middleware pipeline
    /// </summary>
    public static class RateLimitingExtensions
    {
        /// <summary>
        /// Adds Redis-based rate limiting to the application
        /// </summary>
        /// <param name="app">Application builder</param>
        /// <param name="configuration">Optional rate limit configuration</param>
        /// <returns>Application builder</returns>
        public static IApplicationBuilder UseRedisRateLimiting(
            this IApplicationBuilder app,
            RedisRateLimitingMiddleware.RateLimitConfiguration? configuration = null)
        {
            return app.UseMiddleware<RedisRateLimitingMiddleware>(
                configuration ?? new RedisRateLimitingMiddleware.RateLimitConfiguration()
            );
        }
    }
}