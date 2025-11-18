using System;

namespace LibraryManagement.Models
{
    public class ExternalBookInfo
    {
        public int Id { get; set; }
        public string ISBN { get; set; } = null!;
        public string ResponseJson { get; set; } = null!;
        public DateTime FetchedAt { get; set; }
    }
}
