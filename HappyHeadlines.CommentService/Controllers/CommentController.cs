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

        [HttpPost("CreateComment")]
        public async Task<IActionResult> Create(CreateCommentRequest request)
        {
            CommentDto commentDto;
            try
            {
                commentDto = await _commentService.CreateComment(request);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            
            return CreatedAtAction(nameof(GetCommentsByArticle), new { articleId = commentDto.ArticleId }, commentDto);
        }

        [HttpGet("article/{articleId:guid}")]
        public async Task<IActionResult> GetCommentsByArticle(Guid articleId, [FromQuery] int page = 1)
        {
            List<CommentDto> comments = new List<CommentDto>();
            try
            {
                comments = await _commentService.GetCommentsByArticleId(articleId, page, PageSize);
            }
            catch(Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            
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