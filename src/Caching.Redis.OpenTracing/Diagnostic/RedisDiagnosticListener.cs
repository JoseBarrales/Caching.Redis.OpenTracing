using Caching.Redis.OpenTracing.Abstractions;
using Microsoft.Extensions.DiagnosticAdapter;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Caching.Redis.OpenTracing.Diagnostic
{
    /// <summary>
    /// Uses interlock to count metrics for later export with prometheus, further actions must be placed in the implemented class
    /// </summary>
    public class RedisDiagnosticListener 
    {
        public static long ExceptionCount;
        public static long GetCount;
        public static long SetCount;
        public static long DeleteCount;
        public static long Hits;
        public static long Misses;


        [DiagnosticName("RedisClient.Error")]
        public virtual void OnError(IReadOnlyList<string> keys, Exception exception)
        {
            Interlocked.Add(ref ExceptionCount, 1);
        }

        [DiagnosticName("RedisClient.Hit")]
        public virtual void OnCacheHit(int count)
        {
            Interlocked.Add(ref Hits, count);
        }

        [DiagnosticName("RedisClient.Miss")]
        public virtual void OnCacheMiss(int count)
        {
            Interlocked.Add(ref Misses, count);
        }


        [DiagnosticName("RedisClient.Set")]
        public virtual void OnSet() { }

        [DiagnosticName("RedisClient.Set.Start")]
        public virtual void OnSetStart(int count)
        {
            Interlocked.Add(ref SetCount, count);
        }

        [DiagnosticName("RedisClient.Set.Stop")]
        public virtual void OnSetStop(Activity activity)
        { }

        [DiagnosticName("RedisClient.Get")]
        public virtual void OnGet() { }

        [DiagnosticName("RedisClient.Get.Start")]
        public virtual void OnGetStart(int count)
        {
            Interlocked.Add(ref GetCount, count);
        }

        [DiagnosticName("RedisClient.Get.Stop")]
        public virtual void OnGetStop(Activity activity)
        { }

        [DiagnosticName("RedisClient.Delete")]
        public virtual void OnDelete() { }

        [DiagnosticName("RedisClient.Delete.Start")]
        public virtual void OnDeleteStart(int count)
        {
            Interlocked.Add(ref DeleteCount, count);
        }

        [DiagnosticName("RedisClient.Delete.Stop")]
        public virtual void OnDeleteStop(Activity activity)
        { }

    }
}
