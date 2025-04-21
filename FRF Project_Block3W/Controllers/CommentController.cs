using Data.Models;
using FRF_Project_Block3W.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Core;

namespace FRF_Project_Block3W.Controllers
{
    [Route("api/comment")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpGet("replies/{parentCommentId}")]
        public async Task<IActionResult> GetReplies(Guid parentCommentId)
        {
            var result = await _commentService.GetRepliesByCommentIdAsync(parentCommentId);

            return Ok(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Member, Administrator")]
        public async Task<IActionResult> UpdateComment(Guid id, [FromBody]CommentUpdateModel model)
        {
            var userId = User.Claims.GetUserIdFromJwtToken();

            var result = await _commentService.UpdateCommentAsync(id, userId, model);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Member, Administrator")]
        public async Task<IActionResult> DeleteComment(Guid id)
        {
            var userId = User.Claims.GetUserIdFromJwtToken();

            var result = await _commentService.DeleteCommentAsync(id, userId);

            return Ok(result);
        }
    }
}
