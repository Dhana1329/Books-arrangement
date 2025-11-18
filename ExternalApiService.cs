using LibraryManagement.Data;
using LibraryManagement.Models;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Services
{
    public class ExternalApiService : IExternalApiService
    {
        private readonly IHttpClientFactory _httpFactory;
        private readonly IMemoryCache _cache;
        private readonly AppDbContext _db;
        private readonly TimeSpan _ttl = TimeSpan.FromMinutes(30);

        public ExternalApiService(IHttpClientFactory httpFactory, IMemoryCache cache, AppDbContext db)
        {
            _httpFactory = httpFactory;
            _cache = cache;
            _db = db;
        }

        public async Task<string?> GetBookInfoByIsbnAsync(string isbn)
        {
            if (string.IsNullOrWhiteSpace(isbn)) return null;
            var cacheKey = $"bookinfo:{isbn}";
            if (_cache.TryGetValue(cacheKey, out string cached)) return cached;

            var client = _httpFactory.CreateClient();
            var endpoint = $"https://openlibrary.org/api/books?bibkeys=ISBN:{isbn}&format=json&jscmd=data";
            var log = new ExternalApiLog { Endpoint = endpoint, Request = $"GET {endpoint}", CalledAt = DateTime.UtcNow };

            try
            {
                var resp = await client.GetAsync(endpoint);
                var text = await resp.Content.ReadAsStringAsync();

                log.Response = text;
                log.Success = resp.IsSuccessStatusCode;
                _db.ExternalApiLogs.Add(log);

                if (resp.IsSuccessStatusCode)
                {
                    _db.ExternalBookInfos.Add(new ExternalBookInfo { ISBN = isbn, ResponseJson = text, FetchedAt = DateTime.UtcNow });
                    _cache.Set(cacheKey, text, _ttl);
                }

                await _db.SaveChangesAsync();
                return text;
            }
            catch (Exception ex)
            {
                log.Success = false;
                log.ErrorMessage = ex.Message;
                log.Response = string.Empty;
                _db.ExternalApiLogs.Add(log);
                await _db.SaveChangesAsync();
                return null;
            }
        }

        public async Task<IEnumerable<dynamic>> GetLogsAsync()
        {
            return await _db.ExternalApiLogs
                .OrderByDescending(l => l.CalledAt)
                .Select(l => new { l.Id, l.Endpoint, l.CalledAt, l.Success, l.ErrorMessage })
                .ToListAsync<dynamic>();
        }
    }
}
