using KASHOP.DAL.DTO.Request;
using KASHOP.DAL.DTO.Response;
using KASHOP.DAL.Models;
using KASHOP.DAL.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using NPOI.SS.Formula.Functions;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BL.Service
{
    public class CheckoutService : ICheckoutService
    {
        private readonly ICartRepository _cartRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOrderRepository _orderRepository;
        private readonly ICartService _cartService;
        private readonly IProductRepository _productRepository;
        private readonly IEmailSender _emailSender;
        public CheckoutService(ICartRepository cartRepository, UserManager<ApplicationUser> userManager,
            IHttpContextAccessor httpContextAccessor,
            IOrderRepository orderRepository,
            ICartService cartService, IProductRepository productRepository, IEmailSender emailSender = null)
        {
            _cartRepository = cartRepository;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _orderRepository = orderRepository;
            _cartService = cartService;
            _productRepository = productRepository;
            _emailSender = emailSender;
        }

        public ICartRepository CartRepository { get; }

        public async Task<CheckoutResponse> ProcessCheckout(string userId, CheckoutRequest request)
        {
            var cartItems = await _cartRepository.GetAllAsync(
               filter:  c => c.UserId == userId, 
               includes: new[] { nameof(Cart.Product),
               $"{nameof(Cart.Product)}.{nameof(Product.Translations)}"
               }
                );

            if (!cartItems.Any())
            {
                return new CheckoutResponse
                {
                    Error = "Cart is empty",
                    Success = false
                };
            }

            var user = await _userManager.FindByIdAsync(userId);

            var city = request.City ?? user.city;
            if (city is null)
            {
                return new CheckoutResponse
                {
                    Error = "city is required",
                    Success = false
                };
            }
            var street = request.Street ?? user.street;
            if (street is null)
            {
                return new CheckoutResponse
                {
                    Error = "street is required",
                    Success = false
                };
            }
            var phoneNumber = request.PhoneNumber ?? user.PhoneNumber;
            if (phoneNumber is null)
            {
                return new CheckoutResponse
                {
                    Error = "phone number is required",
                    Success = false
                };
            }

            foreach(var item in cartItems)
            {
                if(item.Count > item.Product.Quantity)
                {
                    return new CheckoutResponse
                    {
                        Error = "out of stock",
                        Success = false
                    };
                }
            }

            var order = new Order()
            {
                UserId = userId,
                City = city,
                Street = street,
                PhoneNumber = phoneNumber,
                PaymentMethod = request.PaymentMethod,
                AmountPaid = cartItems.Sum(c => c.Product.Price * c.Count),
                OrderItems = cartItems.Select(c => new OrderItem
                {
                    ProductId = c.ProductId,
                    Quantity = c.Count,
                    UnitPrice = c.Product.Price,
                    TotalPrice = c.Product.Price * c.Count,
                }).ToList()
            };

            if (request.PaymentMethod == PaymentMethodEnum.Visa)
            {
                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    Mode = "payment",
                    SuccessUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/api/checkouts/success?sessionId={{CHECKOUT_SESSION_ID}}",
                    CancelUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/api/checkouts/cancel",

                    LineItems = new List<SessionLineItemOptions>()
                };


                foreach(var item in cartItems)
                {
                    options.LineItems.Add(

                        new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                Currency = "USD",
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = item.Product.Translations.FirstOrDefault(t=>t.Language == "en").Name,
                                },
                                UnitAmount = (long) (item.Product.Price * 100),
                            },
                            Quantity = item.Count,
                        }
                        );
                }
                var service = new SessionService();
                var session = service.Create(options);
                order.StripeSessionId = session.Id;

                await _orderRepository.CreateAsync(order);

                return new CheckoutResponse
                {
                    Success = true,
                    StripeUrl = session.Url
                };

            }

            return new CheckoutResponse
            {
                Error = "invalid payment method",
                Success = false
            };
        }

        public async Task<CheckoutResponse> HandleSuccess(string sessionId)
        {
            var order = await _orderRepository.GetOne(o => o.StripeSessionId == sessionId,
                includes: new[]
                {
                    nameof(Order.OrderItems),
                    $"{nameof(Order.OrderItems)}.{nameof(OrderItem.Product)}.{nameof(Product.Translations)}"
                });

            if (order == null)
            {
                return new CheckoutResponse
                {
                    Success = false,
                    Error = "Order not found for this session"
                };
            }

            order.OrderStatus = OrderStatusEnum.Paid;
            await _orderRepository.UpdateAsync(order);

            await _cartService.ClearCart(order.UserId);

            var user = await _userManager.FindByIdAsync(order.UserId);
            await _emailSender.SendEmailAsync(user.Email, "order confirmed", "<h2> your order has been placed successfully </h2>");

            var lowStockProducts = await _productRepository.DecreaseQuantityAsync(order.OrderItems);
            foreach(var item in lowStockProducts)
            {
                if (lowStockProducts != null)
                {
                    await _emailSender.SendEmailAsync("hatembartra@gmail.com", "low stock", $"<h2> product {item.Translations.FirstOrDefault(t => t.Language == "en").Name} has low quantity {item.Quantity}  </h2>");

                }

            }
            return new CheckoutResponse()
            {
                Success = true,
                OrderId = order.Id
            };
        }
    }
}
