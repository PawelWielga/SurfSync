using System;
using System.IO;
using System.Text;
using System.Threading;

namespace SurfSync.Logging;

internal static class ErrorLogger
{
    private static readonly object SyncRoot = new();
    private static string _logDirectory;
    private static int _sequence;
    private static bool _initialized;

    public static void Initialize()
    {
        if (_initialized)
            return;

        lock (SyncRoot)
        {
            if (_initialized)
                return;

            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            _logDirectory = Path.Combine(baseDirectory, "Logs");
            Directory.CreateDirectory(_logDirectory);
            _initialized = true;
        }
    }

    public static void LogException(Exception exception, string context)
    {
        if (exception == null)
            return;

        try
        {
            EnsureInitialized();
            var filePath = GetLogFilePath();
            var content = BuildExceptionContent(exception, context);
            File.WriteAllText(filePath, content);
        }
        catch
        {
            // Ignore logging failures.
        }
    }

    public static void LogMessage(string context, string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return;

        try
        {
            EnsureInitialized();
            var filePath = GetLogFilePath();
            var content = BuildMessageContent(context, message);
            File.WriteAllText(filePath, content);
        }
        catch
        {
            // Ignore logging failures.
        }
    }

    private static void EnsureInitialized()
    {
        if (_initialized)
            return;

        Initialize();
    }

    private static string GetLogFilePath()
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss_fff");
        var sequence = Interlocked.Increment(ref _sequence);
        var fileName = $"error_{timestamp}_{Environment.ProcessId}_{sequence}.log";
        return Path.Combine(_logDirectory, fileName);
    }

    private static string BuildExceptionContent(Exception exception, string context)
    {
        var sb = new StringBuilder();
        sb.AppendLine("TimestampUtc: " + DateTime.UtcNow.ToString("O"));
        if (!string.IsNullOrWhiteSpace(context))
            sb.AppendLine("Context: " + context);
        sb.AppendLine("ProcessId: " + Environment.ProcessId);
        sb.AppendLine("ThreadId: " + Thread.CurrentThread.ManagedThreadId);
        sb.AppendLine();
        sb.AppendLine(exception.ToString());
        return sb.ToString();
    }

    private static string BuildMessageContent(string context, string message)
    {
        var sb = new StringBuilder();
        sb.AppendLine("TimestampUtc: " + DateTime.UtcNow.ToString("O"));
        if (!string.IsNullOrWhiteSpace(context))
            sb.AppendLine("Context: " + context);
        sb.AppendLine("ProcessId: " + Environment.ProcessId);
        sb.AppendLine("ThreadId: " + Thread.CurrentThread.ManagedThreadId);
        sb.AppendLine();
        sb.AppendLine(message);
        return sb.ToString();
    }
}
