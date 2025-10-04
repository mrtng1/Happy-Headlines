using HappyHeadlines.NewsletterService.Contracts;
using HappyHeadlines.NewsletterService.Data;
using HappyHeadlines.NewsletterService.Models;
using HappyHeadlines.NewsletterService.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace HappyHeadlines.NewsletterService.Consumers
{
    public class ArticlePublishedConsumer : IConsumer<ArticlePublished>
    {
        private readonly AppDb _db;
        private readonly IEmailSender _email;
        private readonly ITemplateRenderer _tpl;
        private readonly ILogger<ArticlePublishedConsumer> _log;

        public ArticlePublishedConsumer(AppDb db, IEmailSender email, ITemplateRenderer tpl, ILogger<ArticlePublishedConsumer> log)
            => (_db, _email, _tpl, _log) = (db, email, tpl, log);

        public async Task Consume(ConsumeContext<ArticlePublished> ctx)
        {
            var evt = ctx.Message;
            var subs = await _db.Subscribers
                .Where(s => s.Confirmed && s.WantsImmediate && (s.Continent == "Global" || s.Continent == evt.Continent))
                .ToListAsync(ctx.CancellationToken);

            foreach (var s in subs)
            {
                var exists = await _db.Deliveries.AnyAsync(d =>
                    d.Email == s.Email && d.ArticleId == evt.ArticleId && d.Type == "Immediate",
                    ctx.CancellationToken);
                if (exists) continue;

                var html = await _tpl.RenderAsync("Immediate.cshtml", evt);
                await _email.SendAsync(s.Email, $"Breaking: {evt.Title}", html, ctx.CancellationToken);

                _db.Deliveries.Add(new Delivery
                {
                    Email = s.Email,
                    ArticleId = evt.ArticleId,
                    Type = "Immediate",
                    SentAt = DateTimeOffset.UtcNow,
                    Status = "sent"
                });
                await _db.SaveChangesAsync(ctx.CancellationToken);
            }

            _log.LogInformation("Immediate newsletter sent for Article {ArticleId}", evt.ArticleId);
        }
    }
}
