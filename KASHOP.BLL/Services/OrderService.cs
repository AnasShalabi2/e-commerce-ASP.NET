using KASHOP.BLL.Dto;
using KASHOP.DAL.Dto;
using KASHOP.DAL.Enums;
using KASHOP.DAL.Models;
using KASHOP.DAL.Repository;
using Mapster;
using Stripe.Climate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
        }
        public async  Task<Result<OrderDetailsResponse>> GetUserOrderDetailsAsync(int orderId, string userId)
        {
              var order = await _unitOfWork.OrderRepository.GetOneAsync(
                filter: o => o.UserId == userId && o.Id == orderId
                );
            var response = order.Adapt<OrderDetailsResponse>();

            return Result<OrderDetailsResponse>.Ok(response);
        }

        public async Task<Result<List<OrderSummaryResponse>>> GetUserOrdersAsync(string userId)
        {
            var orders = await _unitOfWork.OrderRepository.GetAllAsync(
              filter: o => o.UserId == userId,
              includes: new[] { nameof(DAL.Models.Order.OrderItems) }
              );
            var response = orders.OrderByDescending(o => o.OrderDate).Adapt<List<OrderSummaryResponse>>();
            return Result<List<OrderSummaryResponse>>.Ok(response);
        }
        public async Task<Result<List<OrderSummaryResponse>>> GetAllOrdersAsync(OrderStatusEnum? status)
        {
            var orders = await _unitOfWork.OrderRepository.GetAllAsync(
                filter : o => status == null || o.Status == status,
                includes: new[] { nameof(DAL.Models.Order.OrderItems) }
                );
            var response = orders.OrderByDescending(o => o.OrderDate).Adapt<List<OrderSummaryResponse>>();
            return Result<List<OrderSummaryResponse>>.Ok(response);

        }
        public async Task<Result<OrderDetailsResponse>> ChangeStatusAsync(int orderId , ChangeOrderStatusRequest request)
        {
            var order = await _unitOfWork.OrderRepository.GetOneAsync(o=>o.Id == orderId);
            if(order is null)
            {
                return Result<OrderDetailsResponse>.Fail("Order Not Found");
            }
            if (order.Status == request.Status)
            {
                return Result<OrderDetailsResponse>.Fail($"Order Is Already {request.Status}");
            }
            if(order.PaymentMethod == PaymentMethodEnum.Visa && order.PaymentStatus != PaymentStatusEnum.Paid)
            {
                return Result<OrderDetailsResponse>.Fail("Visa Order Not Paid Yet");
            }

            bool isAllowed = false;
            if(order.Status == OrderStatusEnum.Pending && request.Status == OrderStatusEnum.Confirmed)
                isAllowed = true;

            if (order.Status == OrderStatusEnum.Confirmed && request.Status == OrderStatusEnum.Preparing)
                isAllowed = true;

            if (order.Status == OrderStatusEnum.Preparing && request.Status == OrderStatusEnum.Deliverd)
                isAllowed = true;

            if (!isAllowed)
                return Result<OrderDetailsResponse>.Fail($"Cannot Change Status From {order.Status} To {request.Status}");

            order.Status = request.Status;

            if (request.Status == OrderStatusEnum.Deliverd)
            {
                order.ShippedDate = DateTime.Now;
                if (order.PaymentMethod == PaymentMethodEnum.Cash)
                {
                    order.PaymentStatus = PaymentStatusEnum.Paid;
                    order.AmountPaid = order.TotalAmount;
                    order.PaidAt = DateTime.Now;
                }
            }
                await _unitOfWork.ComplateAsync();
            return Result<OrderDetailsResponse>.Ok(order.Adapt<OrderDetailsResponse>());
        }


    }
}
