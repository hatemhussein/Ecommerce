using KASHOP.DAL.DTO.Request;
using KASHOP.DAL.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BL.Service
{
    public interface ICheckoutService
    {
        Task<CheckoutResponse> ProcessCheckout(string userId, CheckoutRequest request);
        Task<CheckoutResponse> HandleSuccess(string sessionId);
    }
}
