using System;

namespace AaronUrkinCodeExample.BusinessLogicLayer.DataTransferObjects.Logger
{
    public class LogEntryDto
    {
        public int Id { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public string Logger { get; set; }

        public string Level { get; set; }

        public string Message { get; set; }

        public string StackTrace { get; set; }
    }
}
