using Caching.Redis.OpenTracing.Abstractions;
using System;
using System.Diagnostics;

namespace Caching.Redis.OpenTracing.Diagnostic
{
    public class RedisDiagnosticObserver : IObserver<DiagnosticListener>
    {
        private readonly RedisDiagnosticListener _listener;

        public RedisDiagnosticObserver(RedisDiagnosticListener listener)
        {
            _listener = listener ??
                throw new ArgumentNullException(nameof(listener));
        }

        public void OnCompleted() { }

        public void OnError(Exception error) { }

        public void OnNext(DiagnosticListener value)
        {
            if (value.Name == "RedisClient")
            {
                value.SubscribeWithAdapter(_listener);
            }
        }
    }
}
