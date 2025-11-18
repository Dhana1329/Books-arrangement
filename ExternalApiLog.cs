using System;

namespace LibraryManagement.Models
{
    public class ExternalApiLog
    {
        public int Id { get; set; }
        public string Endpoint { get; set; } = null!;
        public string Request { get; set; } = null!;
        public string Response { get; set; } = null!;
        public DateTime CalledAt { get; set; }
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
