using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HappyHeadlines.CommentService.DTOs;
using HappyHeadlines.CommentService.Entities;
using HappyHeadlines.CommentService.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HappyHeadlines.CommentService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private const int PageSize = 30;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCommentRequest request)
        {
            CommentDto commentDto = await _commentService.CreateComment(request);
            
            return CreatedAtAction(nameof(GetCommentsByArticle), new { articleId = commentDto.ArticleId }, commentDto);
        }

        [HttpGet("article/{articleId:guid}")]
        public async Task<IActionResult> GetCommentsByArticle(Guid articleId, [FromQuery] int page = 1)
        {
            List<CommentDto> comments = await _commentService.GetCommentsByArticleId(articleId, page, PageSize);
            return Ok(comments);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            bool success = await _commentService.DeleteComment(id);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}