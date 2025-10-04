using HappyHeadlines.NewsletterService.Data;
using HappyHeadlines.NewsletterService.Models;
using HappyHeadlines.NewsletterService.Services;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace HappyHeadlines.NewsletterService.Jobs
{
    public class DailyDigestJob : IJob
    {
        private readonly AppDb _db;
        private readonly ArticleClient _client;
        private readonly ITemplateRenderer _tpl;
        private readonly IEmailSender _email;
        private readonly ILogger<DailyDigestJob> _log;

        public DailyDigestJob(AppDb db, ArticleClient client, ITemplateRenderer tpl, IEmailSender email, ILogger<DailyDigestJob> log)
            => (_db, _client, _tpl, _email, _log) = (db, client, tpl, email, log);

        public async Task Execute(IJobExecutionContext context)
        {
            var to = DateTime.UtcNow.Date;
            var from = to.AddDays(-1);
            var all = await _client.GetByRangeAsync(from, to, "Global"); // kan kaldes pr. kontinent hvis ønsket

            var subs = await _db.Subscribers.Where(s => s.Confirmed && s.WantsDaily).ToListAsync();
            foreach (var s in subs)
            {
                var date = DateOnly.FromDateTime(from);
                var sent = await _db.Deliveries.AnyAsync(d => d.Email == s.Email && d.DigestDate == date && d.Type == "Digest");
                if (sent) continue;

                var perUser = s.Continent == "Global" ? all : all.Where(a => a.Continent == s.Continent).ToList();
                var html = await _tpl.RenderAsync("Digest.cshtml", new { Date = date, Articles = perUser });

                await _email.SendAsync(s.Email, $"Dagens nyheder – {date}", html);
                _db.Deliveries.Add(new Delivery { Email = s.Email, DigestDate = date, Type = "Digest", SentAt = DateTimeOffset.UtcNow, Status = "sent" });
                await _db.SaveChangesAsync();
            }

            _log.LogInformation("Daily digest job completed for {Date}", from);
        }
    }
}
