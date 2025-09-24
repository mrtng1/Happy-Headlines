using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HappyHeadlines.CommentService.Entities;

namespace HappyHeadlines.CommentService.Interfaces;


public interface ICommentRepository
{
    Task CreateComment(Comment comment);
    Task<Comment?> GetCommentById(Guid id);
    Task<List<Comment>> GetCommentsByArticleId(Guid articleId, int page, int pageSize);
    Task<bool> DeleteComment(Guid id);
}