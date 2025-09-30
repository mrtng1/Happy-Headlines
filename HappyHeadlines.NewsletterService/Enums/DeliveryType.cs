namespace HappyHeadlines.NewsletterService.Enums
{
    public enum DeliveryType
    {
        Immediate = 1, // Sendes straks ved ArticlePublished-event
        Digest = 2     // Sendes i dagligt nyhedsbrev
    }
}
