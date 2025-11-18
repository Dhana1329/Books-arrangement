namespace LibraryManagement.Services
{
    public interface IExternalApiService
    {
        Task<string?> GetBookInfoByIsbnAsync(string isbn);
        Task<IEnumerable<dynamic>> GetLogsAsync();
    }
}
