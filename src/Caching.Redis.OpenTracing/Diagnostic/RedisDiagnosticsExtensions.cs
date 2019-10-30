using Caching.Redis.OpenTracing.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;

namespace Caching.Redis.OpenTracing.Diagnostic
{
    public static class RedisDiagnosticsExtensions
    {
        public static RedisDiagnosticListener listener;
        public static RedisDiagnosticObserver observer;

        /// <summary>
        /// Register an RedisDiagnosticObserver instance of RedisDiagnosticListener to listen for Redis Metrics with prometheus
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IServiceCollection AddRedisDiagnostics(this IServiceCollection services, RedisDiagnosticListener redisListener = null)
        {
            listener = redisListener ?? new RedisDiagnosticListener();

            services.AddSingleton(listener);

            if (listener == default)
                throw new NullReferenceException("listener cannot be null, use services.AddRedisListener() in ConfigureServices method.");

            observer = new RedisDiagnosticObserver(listener);

            DiagnosticListener.AllListeners.Subscribe(observer);

            return services;
        }
    }
}
