namespace QIA.Opc.Infrastructure.Application;
using global::Opc.Ua;
using Serilog;
using Serilog.Core;

public class LoggerManager
{
    public static Logger Logger { get; set; }

    private static readonly string LogLevel = "info";

    /// <summary>
    /// Mapping of the application logging levels to OPC stack logging levels.
    /// </summary>
#pragma warning disable CS0414 // The field 'LoggerManager.OpcTraceToLoggerVerbose' is assigned but its value is never used
    private static int OpcTraceToLoggerVerbose;
    private static int OpcTraceToLoggerDebug;
    private static int OpcTraceToLoggerInformation;
    private static int OpcTraceToLoggerWarning;
    private static int OpcTraceToLoggerError;
    private static int OpcTraceToLoggerFatal;
#pragma warning restore CS0414 // The field 'LoggerManager.OpcTraceToLoggerVerbose' is assigned but its value is never used
    /// <summary>
    /// Set the OPC stack log level.
    /// </summary>
    public static int OpcStackTraceMask { get; set; }
    private static readonly string LogFileName = $"{Utils.GetHostName()}-bpc.log";
    private static readonly string LogUrl = "http://localhost:5341";

    public static void InitLogging()
    {
        LoggerConfiguration loggerConfiguration = new();

        // set the log level
        switch (LogLevel)
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
            default:
                break;
        }

        // set logging sinks
        loggerConfiguration.WriteTo.Console();
        loggerConfiguration.WriteTo.Seq(LogUrl);
        if (!string.IsNullOrEmpty(LogFileName))
        {
            // configure rolling file sink
            //const int MAX_LOGFILE_SIZE = 1024 * 1024;
            //const int MAX_RETAINED_LOGFILES = 2;
            //loggerConfiguration.WriteTo.File(_logFileName, fileSizeLimitBytes: MAX_LOGFILE_SIZE, flushToDiskInterval: _logFileFlushTimeSpanSec, rollOnFileSizeLimit: true, retainedFileCountLimit: MAX_RETAINED_LOGFILES);
        }

        Logger = loggerConfiguration.CreateLogger();
        Logger.Information($"Current directory is: {Directory.GetCurrentDirectory()}");
        Logger.Information($"Log file is: {LogFileName}");
        Logger.Information($"Log level is: {LogLevel}");
        Logger.Information($"Log Seq is: {LogUrl}");
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
        var message = string.Format(e.Format, e.Arguments).Trim();
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
    }
}
