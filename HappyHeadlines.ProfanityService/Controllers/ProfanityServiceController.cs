using Microsoft.AspNetCore.Mvc;
using HappyHeadlines.ProfanityService.Services;
using HappyHeadlines.ProfanityService.Data;

namespace HappyHeadlines.ProfanityService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProfanityServiceController : ControllerBase
    {
        private readonly HappyHeadlines.ProfanityService.Services.ProfanityService _profanityService;

        public ProfanityServiceController(HappyHeadlines.ProfanityService.Services.ProfanityService profanityService)
        {
            _profanityService = profanityService;
        }

        [HttpPost("check")]
        public async Task<IActionResult> CheckProfanity([FromBody] string text)
        {
            bool hasProfanity = await _profanityService.ContainsProfanityAsync(text);
            return Ok(new { hasProfanity });
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddProfanityWord([FromBody] string word)
        {
            var entity = new ProfanityWord { Word = word };
            _profanityService.AddWord(entity); // You need to implement AddWord in your service
            return Ok();
        }
    }
}
