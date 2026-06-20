using KASHOP.DAL.DTO.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BL.Service
{
    public interface IReviewService
    {
        Task<bool> AddReview(string userId, AddReviewRequest request);
    }
}
