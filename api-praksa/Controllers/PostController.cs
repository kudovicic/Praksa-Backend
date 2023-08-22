using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using api_praksa.Controllers.Models;
using api_praksa.Services;
using api_praksa.Services.PostService;
using Microsoft.Extensions.Hosting;

namespace api_praksa.Controllers
{
    [ApiController]
    [Route("api/[controller]")]


    public class PostController : ControllerBase
    {

        private readonly IPostService _postService;

        //konstruktor
        public PostController(IPostService postService)
        {
            _postService = postService;
        }


        [HttpGet("Get")]
        [SessionToken]
        public async Task<IActionResult> GetPosts()
        {
            try
            {
                var post = await _postService.GetPosts();
                return Ok(post);
            }
            catch (Exception ex)
            {
                //log error
                return BadRequest("Error in SQL");
            }
        }

        [HttpGet("Get/{id}", Name = "PostById")]
        [SessionToken]

        public async Task<IActionResult> GetPost(int id)
        {
            try
            {
                var post = await _postService.GetPost(id);
                if (post == null)
                    return NotFound();

                return Ok(post);
            }
            catch (Exception ex)
            {
                //log error
                return BadRequest("Error in SQL");
            }
        }


        [HttpPost("Create")]
        [AdminOnly]
        
        public async Task<IActionResult> CreatePost([FromBody] CreatePost post)
        {
            try
            {
                var formValidator = new FormValidation();
                if (!formValidator.ValidatePostTitle(post.Title))
                    return BadRequest("Invalid title length.");
                if (!formValidator.ValidateAuthorId(post.AuthorId))
                    return BadRequest("Invalid AuthorId.");
                var createdPost = await _postService.CreatePost(post);
                return CreatedAtRoute("UserById", new { id = createdPost.Id }, createdPost);
            }
            catch (Exception ex)
            {
                //log error
                return BadRequest("Error in SQL");
            }
        }

        
        [HttpPut("Update/{id}")]
        [AdminOnly]
        
        public async Task<IActionResult> UpdatePost(int id, UpdatePost post)
        {
         
            try
            {
                var formValidator = new FormValidation();
                if (!formValidator.ValidatePostTitle(post.Title))
                    return BadRequest("Invalid title.");
                if (!formValidator.ValidateAuthorId(post.AuthorId))
                    return BadRequest("Invalid AuthorId.");
                await _postService.UpdatePost(id, post);
                return Ok("The post has been successfully updated.");
                
            }
            catch (Exception ex)
            {

                return BadRequest("Error in SQL");
            }
        }


        [HttpDelete("Delete/{id}")]
        [AdminOnly]
        
        public async Task<IActionResult> DeletePost(int id)
        {

            try
            {
                await _postService.DeletePost(id);
                return Ok("The post has been successfully deleted.");
            }
            catch (Exception ex)
            {

                return BadRequest("Error in SQL");
            }
        }

    }
}

