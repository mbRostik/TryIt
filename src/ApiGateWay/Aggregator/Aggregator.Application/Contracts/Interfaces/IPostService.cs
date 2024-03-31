using Aggregator.Application.Contracts.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aggregator.Application.Contracts.Interfaces
{
    public interface IPostService
    {
        Task<List<GiveFollowedPostsDTO>> GetFollowedPosts(string userId, string accessToken);

    }
}
