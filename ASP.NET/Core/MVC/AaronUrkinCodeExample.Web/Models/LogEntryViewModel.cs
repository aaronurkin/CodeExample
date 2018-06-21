using System;

namespace AaronUrkinCodeExample.Web.Models
{
    public class LogEntryViewModel
    {
        public int Id { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public string Logger { get; set; }

        public string Level { get; set; }

        public string Message { get; set; }

        public string StackTrace { get; set; }
    }
}
