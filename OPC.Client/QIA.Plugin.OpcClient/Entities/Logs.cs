using System;

namespace QIA.Plugin.OpcClient.Entities
{
    ///TODO: do we need db with logs if we have seq logger already
    public class Logs
    {
        public DateTime DateTime { get; set; }
        public string Message { get; set; }
        public ErrorCodes ErrorCode { get; set; }
        public LogCategory LogCategory { get; set; }
        public string ClassInstance { get; set; }
        public string ExceptionMessage { get; set; }
        public string Method { get; set; }
        public string CallStack { get; set; }
    }

    public enum ErrorCodes
    {
        C00000,
        C00001,
        C00002,
        C00003,
        C00004,
        C00005,
        C00006,
        C00007,
        C00008,
        C00009,
        C000010,
        C000011
    }

    public enum LogCategory
    {
        DEBUG,
        INFO,
        INIT,
        ESSENCE,
        INIT_ESSENCE,
        WARNING,
        ERROR,
        EXCEPTION,
        INIT_WARNING,
        INIT_ERROR,
        INIT_EXCEPTION,
        FAILURE
    }
}
