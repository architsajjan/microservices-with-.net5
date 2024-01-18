using System;

public static class Globals
{
	public static Class1()
	{
        private static void LogDatabaseExecutionRuntime(string name, string additionalInfo = "")
        {
            stopwatch.Stop();
            _logger.LogDebug($"<<{name}>> ran for <<{stopwatch.ElapsedMilliseconds}>> ms at {DateTime.Now}.");
            if (!string.IsNullOrWhiteSpace(additionalInfo))
            {
                _logger.LogDebug($"Additional Info : {additionalInfo}");
            }
        }

        private static void InitializeStopWatch()
        {
            stopwatch = Stopwatch.StartNew();
        }

        private static dynamic CheckForNulls(dynamic value, string typeName) => value ?? throw new ArgumentException($"{nameof(value)} at {typeName} on {DateTime.Now}");
    }
}
