using HappyHeadlines.NewsletterService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HappyHeadlines.NewsletterService.Controllers
{
    [ApiController]
    [Route("api/test-email")]
    public class TestEmailController : ControllerBase
    {
        private readonly IEmailSender _email;
        public TestEmailController(IEmailSender email) => _email = email;

        [HttpPost]
        public async Task<IActionResult> Send([FromQuery] string to = "alice@example.com")
        {
            await _email.SendAsync(to, "Hello from NewsletterService", "<b>It works 🎉</b>");
            return Ok("sent");
        }
    }

}
