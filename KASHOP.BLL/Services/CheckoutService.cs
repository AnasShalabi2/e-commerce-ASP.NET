using KASHOP.BLL.Dto;
using KASHOP.DAL.Dto;
using KASHOP.DAL.Enums;
using KASHOP.DAL.Models;
using KASHOP.DAL.Repository;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Services
{
    public class CheckoutService : ICheckoutService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CheckoutService(IUnitOfWork unitOfWork,UserManager<ApplicationUser> userManager , IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Result<OrderResponse>> CheckOut(string userId, CheckoutRequest request)
        {
            var cartItems = await _unitOfWork.CartRepository.GetAllAsync(
                filter: c => c.UserId == userId,
                includes: new string[] { nameof(CartItem.Product), $"{nameof(CartItem.Product)}.{nameof(Product.Translations)}"}
                );
            if (!cartItems.Any())
                return Result<OrderResponse>.Fail("Cart Is Empty");
            foreach (var cartItem in cartItems)
            {
                if (cartItem.Count > cartItem.Product.Quantity)
                {
                    return Result<OrderResponse>.Fail($"Not Enough stock of Product{cartItem.ProductID}");
                }
            }
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
            {
                return Result<OrderResponse>.Fail("User Not Found");

            }
            var City = request.City ?? user.City;
            var Street = request.Street ?? user.Street;
            var PhoneNumber = request.PhoneNumber ?? user.PhoneNumber;
            if (string.IsNullOrEmpty(City) || string.IsNullOrEmpty(Street) || string.IsNullOrEmpty(PhoneNumber))
                return Result<OrderResponse>.Fail("Shipping info is Required");

            var order = new Order
            {
                UserId = userId,
                PaymentMethod = request.PaymentMethod,
                Status = OrderStatusEnum.Pending,
                PaymentStatus = PaymentStatusEnum.Pending,
                City = City,
                Street = Street,
                PhoneNumber = PhoneNumber,
                OrderItems = new List<OrderItem>()
            };
            decimal totalAmount = 0;
            foreach (var item in cartItems)
            {
                var orderItem = new OrderItem
                {
                    ProductId = item.ProductID,
                    Count = item.Count,
                    PriceAtPurchase = item.Product.Price

                };
                totalAmount += orderItem.PriceAtPurchase * orderItem.Count;
                order.OrderItems.Add(orderItem);
            }
            Console.WriteLine(totalAmount);
            order.TotalAmount = totalAmount;
            return request.PaymentMethod == PaymentMethodEnum.Cash ? await PlaceCashOrderAsync(order, cartItems) :
                await PlaceVisaOrderAsync(order, cartItems);
        }
       
        private async Task<Result<OrderResponse>> PlaceVisaOrderAsync(Order order, List<CartItem> cartItems)
        {
            var request = _httpContextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";
            var options = new SessionCreateOptions
            {
                Mode = "payment",
                LineItems = cartItems.Select(item => new SessionLineItemOptions
                {
                    Quantity = item.Count,
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Translations.FirstOrDefault(t => t.Language == "en")?.Name,
                        },
                        UnitAmount = (long)item.Product.Price * 100,
                    }
                }).ToList(),
                SuccessUrl = $"{baseUrl}/api/Orders/payment/success?sessionId={{CHECKOUT_SESSION_ID}}",
                ExpiresAt = DateTime.UtcNow.AddHours(1)

            };
            var service = new SessionService();
            var session = service.Create(options);
            order.StripeSessionId = session.Id;
            await _unitOfWork.OrderRepository.CreateAsync(order);
            await _unitOfWork.ComplateAsync();

            var response = order.Adapt<OrderResponse>();
            response.PaymentUrl = session.Url;

            return Result<OrderResponse>.Ok(response, "Redirect to Payment");
        }


        private async Task<Result<OrderResponse>> PlaceCashOrderAsync(Order order, List<CartItem> cartItems)
            {
            foreach (var item in cartItems) {
                item.Product.Quantity -= item.Count;
                _unitOfWork.CartRepository.Delete(item);
            }
            await _unitOfWork.OrderRepository.CreateAsync(order);
            await _unitOfWork.ComplateAsync();
            return Result<OrderResponse>.Ok(order.Adapt<OrderResponse>(), "Order Placed Succesfully");
            }

        public async Task<Result<OrderResponse>> HandleSuccess(string sessionId)
        {
            var order = await _unitOfWork.OrderRepository.GetOneAsync(
             filter: o => o.StripeSessionId == sessionId,
             includes: new string[] { $"{nameof(Order.OrderItems)}.{nameof(OrderItem.Product)}" }
        );
            if (order is null)
                return Result<OrderResponse>.Fail("Order Not Found");

            if (order.PaymentStatus == PaymentStatusEnum.Paid)
                return Result<OrderResponse>.Fail("Order Already Paid");

            var session = await new SessionService().GetAsync(sessionId);

            if (session.PaymentStatus != "paid")
                return Result<OrderResponse>.Fail("Payment not Completed");

            order.PaymentStatus = PaymentStatusEnum.Paid;
            order.Status = OrderStatusEnum.Confirmed;
            order.AmountPaid = session.AmountTotal;
            order.PaidAt = DateTime.UtcNow;
            order.PaymentIntentId = session.PaymentIntentId;

            foreach (var item in order.OrderItems)
                item.Product.Quantity -= item.Count;

            var cartItems = await _unitOfWork.CartRepository.GetAllAsync(c => c.UserId == order.UserId);
            foreach (var cartItem in cartItems)
                _unitOfWork.CartRepository.Delete(cartItem);

            await _unitOfWork.ComplateAsync();
            return Result<OrderResponse>.Ok(order.Adapt<OrderResponse>(), "Payment Completed");
        }

    }
    }










