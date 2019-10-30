namespace Caching.Redis.OpenTracing.Diagnostic
{
    public enum RedisDiagnosticActivityType
    {
        Get,
        Set,
        Delete,
        Hits,
        Misses
    }
}
