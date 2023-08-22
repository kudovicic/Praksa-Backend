using System;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace api_praksa.Services.PostService
{
	public interface IPostService
	{
        Task <IEnumerable<Post>> GetPosts();
        public Task<Post> CreatePost(CreatePost post);
        public Task UpdatePost(int id, UpdatePost post);
        public Task DeletePost(int id);
        Task<Post> GetPost(int id);
    }
}

