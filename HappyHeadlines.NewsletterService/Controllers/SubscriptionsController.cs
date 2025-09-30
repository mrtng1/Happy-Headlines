using HappyHeadlines.NewsletterService.Data;
using HappyHeadlines.NewsletterService.DTOs;
using HappyHeadlines.NewsletterService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HappyHeadlines.NewsletterService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionsController : ControllerBase
    {
        private readonly AppDb _db;

        public SubscriptionsController(AppDb db) => _db = db;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSubscriptionRequest req)
        {
            var exists = await _db.Subscribers.AnyAsync(x => x.Email == req.Email);
            if (exists) return Conflict("Already subscribed");

            var sub = new Subscriber
            {
                Email = req.Email,
                WantsImmediate = req.WantsImmediate,
                WantsDaily = req.WantsDaily,
                Categories = req.Categories ?? Array.Empty<string>(),
                Continent = req.Continent ?? "Global",
                ConfirmationToken = Guid.NewGuid().ToString("N")
            };
            _db.Subscribers.Add(sub);
            await _db.SaveChangesAsync();

            // send confirm-mail her hvis du vil (udeladt for korthed)
            return CreatedAtAction(nameof(Get), new { email = sub.Email }, new { sub.Email, sub.Confirmed });
        }

        [HttpGet("{email}")]
        public async Task<IActionResult> Get(string email)
            => Ok(await _db.Subscribers.SingleOrDefaultAsync(x => x.Email == email));

        [HttpPost("confirm")]
        public async Task<IActionResult> Confirm([FromQuery] string token)
        {
            var sub = await _db.Subscribers.SingleOrDefaultAsync(x => x.ConfirmationToken == token);
            if (sub is null) return NotFound();
            sub.Confirmed = true; sub.ConfirmationToken = null;
            await _db.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{email}")]
        public async Task<IActionResult> Delete(string email)
        {
            var sub = await _db.Subscribers.SingleOrDefaultAsync(x => x.Email == email);
            if (sub is null) return NotFound();
            _db.Subscribers.Remove(sub);
            await _db.SaveChangesAsync();
            return Ok();
        }
    }
}
