using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Caching.Redis.OpenTracing.Diagnostic
{
    internal static class DiagnosticEvents
    {
        public const string DiagnosticListenerName = "RedisClient";

        private const string _diagnosticSourceName = "RedisClient";
        private const string _delete = _diagnosticSourceName + ".Delete";
        private const string _get = _diagnosticSourceName + ".Get";
        private const string _set = _diagnosticSourceName + ".Set";
        private const string _hits = _diagnosticSourceName + ".Hit";
        private const string _misses = _diagnosticSourceName + ".Miss";
        private const string _errorEventName = _diagnosticSourceName + ".Error";

        private static readonly DiagnosticSource _source = new DiagnosticListener(DiagnosticListenerName);

        public static void ReceivedError<TKey>
            (
            IReadOnlyList<TKey> keys,
            Exception exception
            )
        {
            var payload = new
            {
                exception,
                keys
            };

            if (_source.IsEnabled(_errorEventName, payload))
            {

                _source.Write(_errorEventName, payload);
            }
        }

        public static void ReceivedError<TKey>
           (
           TKey keys,
           Exception exception
           )
        {
            var payload = new
            {
                exception,
                keys = new[] { keys }
            };

            if (_source.IsEnabled(_errorEventName, payload))
            {
                _source.Write(_errorEventName, payload);
            }
        }

        private static string GetActivity(this RedisDiagnosticActivityType type)
        {
            switch (type)
            {
                case RedisDiagnosticActivityType.Get:
                    return _get;
                case RedisDiagnosticActivityType.Set:
                    return _set;
                case RedisDiagnosticActivityType.Delete:
                    return _delete;
                case RedisDiagnosticActivityType.Hits:
                    return _hits;
                case RedisDiagnosticActivityType.Misses:
                    return _misses;
                default:
                    return _get;
            }
        }

        public static void ReportSingle<TKey>(TKey count, RedisDiagnosticActivityType activityType)
        {
            var payload = new
            {
                count
            };

            if (_source.IsEnabled(activityType.GetActivity(), payload))
            {
                _source.Write(activityType.GetActivity(), payload);
            }

        }

        public static Activity StartSingle(int key, RedisDiagnosticActivityType activityType)
        {
            var payload = new
            {
                count = key
            };

            if (_source.IsEnabled(activityType.GetActivity(), payload))
            {
                var activity = new Activity(activityType.GetActivity());


                _source.StartActivity(activity, payload);

                return activity;
            }

            return null;
        }

        public static Activity StartSingle(string key, RedisDiagnosticActivityType activityType)
        {
            var payload = new
            {
                key,
                count = 1                
            };

            if (_source.IsEnabled(activityType.GetActivity(), payload))
            {
                var activity = new Activity(activityType.GetActivity());

                activity.AddTag("redisKey", key);

                _source.StartActivity(activity, payload);

                return activity;
            }

            return null;
        }

        public static void StopSingle(
            Activity activity,
            RedisDiagnosticActivityType activityType)
        {
            if (activity != null)
            {
                var payload = new
                {
                    activity
                };

                if (_source.IsEnabled(activityType.GetActivity(), payload))
                {
                    _source.StopActivity(activity, payload);
                }
            }
        }
    }
}
