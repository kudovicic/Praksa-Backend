using System;
using api_praksa.Controllers.Models;
using api_praksa.Controllers;
using api_praksa.Services;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using PraksaProjekt.Context;
using Dapper;
using System.Data;

namespace api_praksa.Services.PostService
{
    public class PostService : IPostService
    {

        private readonly DapperContext _context;
        public PostService(DapperContext context)
        {
            _context = context;
        }

        public async Task<Post> CreatePost(CreatePost post)
        {
            var query = "INSERT INTO [Post] (Title, Content, AuthorId) VALUES (@Title, @Content, @AuthorId)" +
                "SELECT CAST(SCOPE_IDENTITY() AS int)";

            var parameters = new DynamicParameters();
            parameters.Add("title", post.Title, DbType.String);
            parameters.Add("content", post.Content, DbType.String);
            parameters.Add("authorid", post.AuthorId, DbType.Int32);

            using (var connection = _context.CreateConnection())
            {
                var id = await connection.QuerySingleAsync<int>(query, parameters);

                var createdPost = new Post
                {
                    Id = id,
                    Title = post.Title,
                    Content = post.Content,
                    AuthorId = post.AuthorId

                };

                return createdPost;
            }

        }

        public async Task DeletePost(int id)
        {
            var query = "DELETE FROM Post WHERE Id = @Id";
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, new { id });
            }
        }

        public async Task<IEnumerable<Post>> GetPosts()

        {
            var query = "select * from dbo.[Post]";
            using (var connection = _context.CreateConnection())

            {
                var posts = await connection.QueryAsync<Post>(query);
                return (IEnumerable<Post>)posts.ToList();
            }

        }

        public async Task UpdatePost(int id, UpdatePost post)
        {
            var query = "UPDATE [Post] SET  Title=@Title, Content=@Content, AuthorId=@AuthorId WHERE Id=@Id ";

            var parameters = new DynamicParameters();
            parameters.Add("id", id, DbType.Int32);
            parameters.Add("title", post.Title, DbType.String);
            parameters.Add("content", post.Content, DbType.String);
            parameters.Add("authorid", post.AuthorId, DbType.Int32);

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        
        }

        public async Task<Post> GetPost(int id)
        {
            var query = "SELECT * FROM dbo.[Post] WHERE Id = @Id";

            using (var connection = _context.CreateConnection())
            {
                var post = await connection.QuerySingleOrDefaultAsync<Post>(query, new { id });

                return post;
            }

        }
    }


}

