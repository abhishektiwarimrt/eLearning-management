using Microsoft.AspNetCore.Http;
using System.Collections.Concurrent;

namespace lms.buildingblocks.middleware
{
    /// <summary>
    /// A middleware for implementing rate limiting on HTTP requests using various algorithms.
    /// Supports Fixed Window, Sliding Window, Token Bucket, and Leaky Bucket rate limiting strategies.
    /// </summary>
    public class RateLimitingMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;
        private static readonly ConcurrentDictionary<string, RateLimitInfo> _clients = new ConcurrentDictionary<string, RateLimitInfo>();

        /// <summary>
        /// Defines the available rate limiting algorithms.
        /// </summary>
        public enum RateLimitAlgorithm
        {
            /// <summary>Fixed time window rate limiting algorithm</summary>
            FixedWindow,
            /// <summary>Sliding time window rate limiting algorithm</summary>
            SlidingWindow,
            /// <summary>Token bucket rate limiting algorithm</summary>
            TokenBucket,
            /// <summary>Leaky bucket rate limiting algorithm</summary>
            LeakyBucket
        }

        /// <summary>
        /// Middleware invocation method that applies rate limiting based on the selected algorithm.
        /// </summary>
        /// <param name="context">The current HTTP context</param>
        /// <returns>A task representing the asynchronous operation</returns>
        public async Task Invoke(HttpContext context)
        {
            var clientIp = context.Connection.RemoteIpAddress?.ToString();

            if (clientIp == null)
            {
                await _next(context);
                return;
            }

            // Choose the algorithm (for example purposes, let's use FixedWindow)
            var selectedAlgorithm = RateLimitAlgorithm.FixedWindow;

            // Based on selected algorithm, apply the corresponding rate limiting
            switch (selectedAlgorithm)
            {
                case RateLimitAlgorithm.FixedWindow:
                    await FixedWindowRateLimit(context, clientIp);
                    break;
                case RateLimitAlgorithm.SlidingWindow:
                    await SlidingWindowRateLimit(context, clientIp);
                    break;
                case RateLimitAlgorithm.TokenBucket:
                    await TokenBucketRateLimit(context, clientIp);
                    break;
                case RateLimitAlgorithm.LeakyBucket:
                    await LeakyBucketRateLimit(context, clientIp);
                    break;
                default:
                    await _next(context);
                    break;
            }
        }

        /// <summary>
        /// Implements Fixed Window rate limiting algorithm.
        /// Allows a fixed number of requests within a specific time window.
        /// </summary>
        /// <param name="context">The current HTTP context</param>
        /// <param name="clientIp">The IP address of the client</param>
        /// <returns>A task representing the asynchronous operation</returns>
        private async Task FixedWindowRateLimit(HttpContext context, string clientIp)
        {
            var rateLimitInfo = _clients.GetOrAdd(clientIp, new RateLimitInfo());

            if (DateTime.UtcNow < rateLimitInfo.WindowEnd)
            {
                if (rateLimitInfo.RequestCount < 10)
                {
                    rateLimitInfo.RequestCount++;
                    await _next(context);
                }
                else
                {
                    context.Response.StatusCode = 429; // Too Many Requests
                    await context.Response.WriteAsync("Rate limit exceeded. Try again later.");
                }
            }
            else
            {
                rateLimitInfo.RequestCount = 1;
                rateLimitInfo.WindowEnd = DateTime.UtcNow.AddMinutes(1);
                await _next(context);
            }
        }

        /// <summary>
        /// Implements Sliding Window rate limiting algorithm.
        /// Tracks and limits requests over a rolling time window.
        /// </summary>
        /// <param name="context">The current HTTP context</param>
        /// <param name="clientIp">The IP address of the client</param>
        /// <returns>A task representing the asynchronous operation</returns>
        private async Task SlidingWindowRateLimit(HttpContext context, string clientIp)
        {
            var slidingWindowRateLimitInfo = _clients.GetOrAdd(clientIp, new SlidingWindowRateLimitInfo()) as SlidingWindowRateLimitInfo;
            if (slidingWindowRateLimitInfo != null)
            {
                var requestTimes = slidingWindowRateLimitInfo.RequestTimes;
                var currentTime = DateTime.UtcNow;

                // Remove expired requests
                while (requestTimes.TryPeek(out var timestamp) && (currentTime - timestamp).TotalMinutes > 1)
                {
                    requestTimes.TryDequeue(out _);
                }

                if (requestTimes.Count < 10)
                {
                    requestTimes.Enqueue(currentTime);
                    await _next(context);
                }
                else
                {
                    context.Response.StatusCode = 429; // Too Many Requests
                    await context.Response.WriteAsync("Rate limit exceeded. Try again later.");
                }
            }
            else
            {
                context.Response.StatusCode = 429; // Too Many Requests
                await context.Response.WriteAsync("Rate limit exceeded. Try again later.");
            }
        }

        /// <summary>
        /// Implements Token Bucket rate limiting algorithm.
        /// Allows requests based on available tokens that are periodically refilled.
        /// </summary>
        /// <param name="context">The current HTTP context</param>
        /// <param name="clientIp">The IP address of the client</param>
        /// <returns>A task representing the asynchronous operation</returns>
        private async Task TokenBucketRateLimit(HttpContext context, string clientIp)
        {
            var bucket = (_clients.GetOrAdd(clientIp, new TokenBucketRateLimitInfo(10, 1)) as TokenBucketRateLimitInfo)?.Bucket;

            if (bucket != null && bucket.AllowRequest())
            {
                await _next(context);
            }
            else
            {
                context.Response.StatusCode = 429; // Too Many Requests
                await context.Response.WriteAsync("Rate limit exceeded. Try again later.");
            }
        }

        /// <summary>
        /// Implements Leaky Bucket rate limiting algorithm.
        /// Controls the rate of requests by maintaining a fixed capacity and leak rate.
        /// </summary>
        /// <param name="context">The current HTTP context</param>
        /// <param name="clientIp">The IP address of the client</param>
        /// <returns>A task representing the asynchronous operation</returns>
        private async Task LeakyBucketRateLimit(HttpContext context, string clientIp)
        {
            var bucket = (_clients.GetOrAdd(clientIp, new LeakyBucketRateLimitInfo(10, 1)) as LeakyBucketRateLimitInfo)?.Bucket;

            if (bucket != null && bucket.AllowRequest())
            {
                await _next(context);
            }
            else
            {
                context.Response.StatusCode = 429; // Too Many Requests
                await context.Response.WriteAsync("Rate limit exceeded. Try again later.");
            }
        }

        /// <summary>
        /// Basic rate limit information class to track request counts and window end time.
        /// </summary>
        private class RateLimitInfo
        {
            /// <summary>Number of requests made in the current window</summary>
            public int RequestCount { get; set; }

            /// <summary>The end time of the current rate limiting window</summary>
            public DateTime WindowEnd { get; set; } = DateTime.UtcNow.AddMinutes(1);
        }

        /// <summary>
        /// Extends RateLimitInfo to support Sliding Window algorithm by tracking request times.
        /// </summary>
        private sealed class SlidingWindowRateLimitInfo : RateLimitInfo
        {
            /// <summary>Queue to store timestamps of requests</summary>
            public ConcurrentQueue<DateTime> RequestTimes { get; } = new ConcurrentQueue<DateTime>();
        }

        /// <summary>
        /// Extends RateLimitInfo to support Token Bucket algorithm with a token bucket implementation.
        /// </summary>
        private sealed class TokenBucketRateLimitInfo(int capacity, int refillRate) : RateLimitInfo
        {
            /// <summary>Token bucket for managing request rate</summary>
            public TokenBucket Bucket { get; } = new TokenBucket(capacity, refillRate);
        }

        /// <summary>
        /// Extends RateLimitInfo to support Leaky Bucket algorithm with a leaky bucket implementation.
        /// </summary>
        /// <remarks>
        /// Initializes a new instance of the LeakyBucketRateLimitInfo class.
        /// </remarks>
        /// <param name="capacity">Maximum number of requests that can be held</param>
        /// <param name="leakRate">Rate at which requests are processed</param>
        private sealed class LeakyBucketRateLimitInfo(int capacity, int leakRate) : RateLimitInfo
        {
            /// <summary>Leaky bucket for controlling request rate</summary>
            public LeakyBucket Bucket { get; } = new LeakyBucket(capacity, leakRate);
        }

        /// <summary>
        /// Implements the Token Bucket algorithm for rate limiting.
        /// Manages a bucket of tokens that are periodically refilled and consumed by requests.
        /// </summary>
        private sealed class TokenBucket(int capacity, int refillRate)
        {
            /// <summary>Tracks the last time tokens were refilled</summary>
            private DateTime _lastRefillTime = DateTime.UtcNow;

            /// <summary>
            /// Determines if a request can be allowed based on available tokens.
            /// </summary>
            /// <returns>True if a request is allowed, false otherwise</returns>
            public bool AllowRequest()
            {
                RefillTokens();
                if (capacity > 0)
                {
                    capacity--;
                    return true;
                }
                return false;
            }

            /// <summary>
            /// Refills tokens based on the elapsed time since the last refill.
            /// </summary>
            private void RefillTokens()
            {
                var now = DateTime.UtcNow;
                var tokensToAdd = (int)((now - _lastRefillTime).TotalMinutes * refillRate);
                capacity = Math.Min(capacity, capacity + tokensToAdd);
                _lastRefillTime = now;
            }
        }

        /// <summary>
        /// Implements the Leaky Bucket algorithm for rate limiting.
        /// Controls the rate of requests by maintaining a fixed capacity and leak rate.
        /// </summary>
        private sealed class LeakyBucket(int capacity, int leakRate)
        {
            /// <summary>Current number of tokens (requests) in the bucket</summary>
            private int _tokens = 0;

            /// <summary>Tracks the last time tokens were leaked</summary>
            private DateTime _lastLeakTime = DateTime.UtcNow;

            /// <summary>
            /// Determines if a request can be allowed based on bucket capacity.
            /// </summary>
            /// <returns>True if a request is allowed, false otherwise</returns>
            public bool AllowRequest()
            {
                LeakTokens();
                if (_tokens < capacity)
                {
                    _tokens++;
                    return true;
                }
                return false;
            }

            /// <summary>
            /// Removes tokens from the bucket based on the elapsed time since the last leak.
            /// </summary>
            private void LeakTokens()
            {
                var now = DateTime.UtcNow;
                var tokensToRemove = (int)((now - _lastLeakTime).TotalMinutes * leakRate);
                _tokens = Math.Max(0, _tokens - tokensToRemove);
                _lastLeakTime = now;
            }
        }
    }
}