using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HappyHeadlines.CommentService.Entities;
using HappyHeadlines.CommentService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HappyHeadlines.CommentService.Infrastructure;

    public class CommentRepository : ICommentRepository
    {
        private readonly CommentDbContext _context;

        public CommentRepository(CommentDbContext context)
        {
            _context = context;
        }

        public async Task CreateComment(Comment comment)
        {
            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();
        }

        public async Task<Comment?> GetCommentById(Guid id)
        {
            return await _context.Comments.FindAsync(id);
        }

        public async Task<List<Comment>> GetCommentsByArticleId(Guid articleId, int page, int pageSize)
        {
            return await _context.Comments
                .Where(c => c.ArticleId == articleId)
                .OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        
        public async Task<bool> DeleteComment(Guid id)
        {
            var commentToDelete = await _context.Comments.FindAsync(id);
            if (commentToDelete == null)
            {
                return false;
            }

            _context.Comments.Remove(commentToDelete);
            await _context.SaveChangesAsync();
            return true;
        }
    }