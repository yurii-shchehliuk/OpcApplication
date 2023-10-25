using Opc.Ua;
using Serilog;

namespace Qia.Opc.Domain.Core
{
	public class LoggerManager
	{
		public static Serilog.Core.Logger Logger { get; set; } = null;

		private static readonly string _logLevel = "info";
		private static readonly TimeSpan _logFileFlushTimeSpanSec = TimeSpan.FromSeconds(30);
		/// <summary>
		/// Mapping of the application logging levels to OPC stack logging levels.
		/// </summary>
#pragma warning disable IDE0052 // Remove unread private members
#pragma warning disable CS0414 // The field 'LoggerManager.OpcTraceToLoggerVerbose' is assigned but its value is never used
		private static int OpcTraceToLoggerVerbose = 0;
		private static int OpcTraceToLoggerDebug = 0;
		private static int OpcTraceToLoggerInformation = 0;
		private static int OpcTraceToLoggerWarning = 0;
		private static int OpcTraceToLoggerError = 0;
		private static int OpcTraceToLoggerFatal = 0;
#pragma warning restore CS0414 // The field 'LoggerManager.OpcTraceToLoggerVerbose' is assigned but its value is never used
#pragma warning restore IDE0052 // Remove unread private members
		/// <summary>
		/// Set the OPC stack log level.
		/// </summary>
		public static int OpcStackTraceMask { get; set; } = 0;
		private static readonly string _logFileName = $"{Utils.GetHostName()}-bpc.log";
		private static readonly string _logUrl = "http://localhost:5341";

		public static void InitLogging()
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
				//loggerConfiguration.WriteTo.File(_logFileName, fileSizeLimitBytes: MAX_LOGFILE_SIZE, flushToDiskInterval: _logFileFlushTimeSpanSec, rollOnFileSizeLimit: true, retainedFileCountLimit: MAX_RETAINED_LOGFILES);
			}

			Logger = loggerConfiguration.CreateLogger();
			Logger.Information($"Current directory is: {Directory.GetCurrentDirectory()}");
			Logger.Information($"Log file is: {_logFileName}");
			Logger.Information($"Log level is: {_logLevel}");
			Logger.Information($"Log Seq is: {_logUrl}");
			return;
		}

		/// <summary>
		/// Event handler to log OPC UA stack trace messages into own logger.
		/// </summary>
		public static void LoggerOpcUaTraceHandler(object sender, TraceEventArgs e)
		{
			// return fast if no trace needed
			if ((e.TraceMask & OpcStackTraceMask) == 0)
			{
				return;
			}

			// e.Exception and e.Message are always null

			// format the trace message
			string message = string.Empty;
			message = string.Format(e.Format, e.Arguments).Trim();
			message = "OPC: " + message;

			// map logging level
			if ((e.TraceMask & OpcTraceToLoggerVerbose) != 0)
			{
				Logger.Verbose(message);
				return;
			}
			if ((e.TraceMask & OpcTraceToLoggerDebug) != 0)
			{
				Logger.Debug(message);
				return;
			}
			if ((e.TraceMask & OpcTraceToLoggerInformation) != 0)
			{
				Logger.Information(message);
				return;
			}
			if ((e.TraceMask & OpcTraceToLoggerWarning) != 0)
			{
				Logger.Warning(message);
				return;
			}
			if ((e.TraceMask & OpcTraceToLoggerError) != 0)
			{
				Logger.Error(message);
				return;
			}
			if ((e.TraceMask & OpcTraceToLoggerFatal) != 0)
			{
				Logger.Fatal(message);
				return;
			}
			return;
		}

		public static void UpdateConsoleLine(int lineNumber, string newText)
		{
			// Store the current cursor position
			if (lineNumber == 0)
			{
				Logger.Information(newText);
				return;
			}

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
