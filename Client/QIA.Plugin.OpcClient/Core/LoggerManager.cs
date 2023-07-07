using Opc.Ua;
using Serilog;
using System;

namespace QIA.Plugin.OpcClient.Core
{
    public class LoggerManager
    {
        public static Serilog.Core.Logger Logger { get; set; } = null;

        private static readonly string _logLevel = "info";
        private static readonly TimeSpan _logFileFlushTimeSpanSec = TimeSpan.FromSeconds(30);
        /// <summary>
        /// Mapping of the application logging levels to OPC stack logging levels.
        /// </summary>
        private static int OpcTraceToLoggerVerbose = 0;
        private static int OpcTraceToLoggerDebug = 0;
        private static int OpcTraceToLoggerInformation = 0;
        private static int OpcTraceToLoggerWarning = 0;
        private static int OpcTraceToLoggerError = 0;
        private static int OpcTraceToLoggerFatal = 0;
        /// <summary>
        /// Set the OPC stack log level.
        /// </summary>
        public static int OpcStackTraceMask { get; set; } = 0;
        private static readonly string _logFileName = $"{Utils.GetHostName()}-bpc.log";
        private static readonly string _logUrl = "http://localhost:5341";

        internal static void InitLogging()
        {
            LoggerConfiguration loggerConfiguration = new();

            // set the log level
            switch (_logLevel)
            {
                case "fatal":
                    loggerConfiguration.MinimumLevel.Fatal();
                    OpcTraceToLoggerFatal = 0;
                    break;
                case "error":
                    loggerConfiguration.MinimumLevel.Error();
                    OpcStackTraceMask = OpcTraceToLoggerError = Utils.TraceMasks.Error;
                    break;
                case "warn":
                    loggerConfiguration.MinimumLevel.Warning();
                    OpcTraceToLoggerWarning = 0;
                    break;
                case "info":
                    loggerConfiguration.MinimumLevel.Information();
                    OpcStackTraceMask = OpcTraceToLoggerInformation = 0;
                    break;
                case "debug":
                    loggerConfiguration.MinimumLevel.Debug();
                    OpcStackTraceMask = OpcTraceToLoggerDebug = Utils.TraceMasks.StackTrace | Utils.TraceMasks.Operation |
                        Utils.TraceMasks.StartStop | Utils.TraceMasks.ExternalSystem | Utils.TraceMasks.Security;
                    break;
                case "verbose":
                    loggerConfiguration.MinimumLevel.Verbose();
                    OpcStackTraceMask = OpcTraceToLoggerVerbose = Utils.TraceMasks.All;
                    break;
            }

            // set logging sinks
            loggerConfiguration.WriteTo.Console();
            loggerConfiguration.WriteTo.Seq(_logUrl);
            if (!string.IsNullOrEmpty(_logFileName))
            {
                // configure rolling file sink
                const int MAX_LOGFILE_SIZE = 1024 * 1024;
                const int MAX_RETAINED_LOGFILES = 2;
                loggerConfiguration.WriteTo.File(_logFileName, fileSizeLimitBytes: MAX_LOGFILE_SIZE, flushToDiskInterval: _logFileFlushTimeSpanSec, rollOnFileSizeLimit: true, retainedFileCountLimit: MAX_RETAINED_LOGFILES);
            }

            Logger = loggerConfiguration.CreateLogger();
            Logger.Information($"Current directory is: {System.IO.Directory.GetCurrentDirectory()}");
            Logger.Information($"Log file is: {_logFileName}");
            Logger.Information($"Log level is: {_logLevel}");
            Logger.Information($"Log Seq is: {_logUrl}");
            return;
        }

        public static void UpdateConsoleLine(int lineNumber, string newText)
        {
            // Store the current cursor position
            int cursorTop = Console.CursorTop;
            int cursorLeft = Console.CursorLeft;

            // Set the cursor position to the desired line
            Console.SetCursorPosition(0, lineNumber - 1);

            // Clear the line
            Console.Write(new string(' ', Console.WindowWidth));

            // Set the cursor position back to the desired line
            Console.SetCursorPosition(0, lineNumber - 1);

            // Write the updated content
            Logger.Information(newText);

            // Restore the cursor position
            Console.SetCursorPosition(cursorLeft, cursorTop);
        }
    }
}
