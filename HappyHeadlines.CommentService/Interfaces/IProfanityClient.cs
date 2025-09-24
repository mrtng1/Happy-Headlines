namespace HappyHeadlines.CommentService.Interfaces;

public interface IProfanityClient
{
    Task<bool> ContainsProfanityAsync(string text);
}