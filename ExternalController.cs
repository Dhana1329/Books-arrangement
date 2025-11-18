using LibraryManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExternalController : ControllerBase
    {
        private readonly IExternalApiService _svc;
        public ExternalController(IExternalApiService svc) { _svc = svc; }

        [HttpGet("bookinfo/{isbn}")]
        public async Task<IActionResult> GetBookInfo(string isbn)
        {
            var res = await _svc.GetBookInfoByIsbnAsync(isbn);
            if (res == null) return NotFound();
            return Content(res, "application/json");
        }

        [HttpGet("logs")]
        [Authorize]
        public async Task<IActionResult> GetLogs()
        {
            var logs = await _svc.GetLogsAsync();
            return Ok(logs);
        }
    }
}
