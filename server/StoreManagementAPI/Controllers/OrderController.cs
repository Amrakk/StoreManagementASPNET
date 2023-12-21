using Microsoft.AspNetCore.Mvc;
using StoreManagementAPI.Models;
using StoreManagementAPI.Services;
using System.Net;

namespace StoreManagementAPI.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderController : Controller
    {
        private readonly OrderService _orderService;
        private readonly PaymentService _paymentService;
        public OrderController(OrderService orderService, PaymentService paymentService)
        {
            _orderService = orderService;
            _paymentService = paymentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders([FromQuery] string customerId)
        {
            List<Order> orders;

            if (!string.IsNullOrEmpty(customerId))
                orders = await _orderService.GetByCustomerId(customerId);
            else
                orders = await _orderService.GetAllOrders();

            if (orders == null || orders.Count == 0)
                return NotFound(new ApiResponse<Order>(StatusCodes.Status404NotFound, "Not Found", new List<Order>()));

            return Ok(new ApiResponse<Order>(StatusCodes.Status200OK, "Success", orders));
        }

        [HttpPost("create")]
        public IActionResult CreateOrder([FromBody] User user)
        {
            Order? createdOrder = _orderService.CreatePendingOrder(user);

            if (createdOrder == null)
            {
                return BadRequest(new
                {
                    code = HttpStatusCode.BadRequest,
                    message = "Failed to create order"
                });
            }

            return Ok(new
            {
                code = HttpStatusCode.Created,
                message = "Sucess",
                data = new List<Order> { createdOrder }
            });
        }

        [HttpGet("{oid}")]
        public IActionResult GetOrderByOID([FromRoute] string oid)
        {
            Order? foundOrder = _orderService.GetOrderByOID(oid);

            if (foundOrder == null)
            {
                return NotFound(new
                {
                    code = HttpStatusCode.NotFound,
                    message = "Not found"
                });
            }

            return Ok(new
            {
                code = HttpStatusCode.OK,
                message = "Found",
                data = new List<Order> { foundOrder }
            });
        }

        [HttpPost("{oid}")]
        public IActionResult UpdateOrder([FromBody] Order order)
        {
            if (order == null)
            {
                return BadRequest(new
                {
                    code = HttpStatusCode.BadRequest,
                    message = "Invalid parameter"
                });
            }

            Order? existingOrder = _orderService.GetOrderByOID(order.Oid);

            if (existingOrder == null)
            {
                return NotFound(new
                {
                    code = HttpStatusCode.NotFound,
                    message = "Not Found"
                });
            }

            existingOrder.OrderProducts = order.OrderProducts;
            existingOrder.Customer = order.Customer;
            existingOrder.TotalPrice = order.TotalPrice;

            bool isUpdated = _orderService.UpdateOrder(existingOrder);

            if (!isUpdated)
            {
                return BadRequest(new
                {
                    code = HttpStatusCode.BadRequest,
                    message = "Failed to update order"
                });
            }

            return Ok(new
            {
                code = HttpStatusCode.OK,
                message = "Success",
                data = new List<Order> { existingOrder }
            });
        }

        [HttpPost("{oid}/update-status")]
        public IActionResult UpdateOrderStatus([FromRoute] string oid, [FromQuery] Status status)
        {
            Order? updatedOrder = _orderService.UpdateOrderStatus(oid, status);

            if (updatedOrder != null)
            {
                return Ok(new
                {
                    code = HttpStatusCode.OK,
                    message = "Order status updated successfully",
                    data = new List<Order> { updatedOrder }
                });
            }
            else
            {
                return BadRequest(new
                {
                    code = HttpStatusCode.BadRequest,
                    message = "Failed to update order status"
                });
            }
        }

        [HttpGet("{oid}/cash/success")]
        public IActionResult HandlePaymentSuccess(string oid)
        {
            Order? existingOrder = _orderService.GetOrderByOID(oid);

            if (existingOrder == null || !existingOrder.OrderStatus.Equals(Status.PENDING))
            {
                return BadRequest(new
                {
                    code = HttpStatusCode.BadRequest,
                    message = "Invalid or non-pending order"
                });
            }

            _paymentService.CreatePayment(existingOrder, "cash");
            _orderService.UpdateOrderStatus(oid, Status.COMPLETED);

            return Ok(new
            {
                code = HttpStatusCode.OK,
                message = "Payment successfully"
            });
        }

        [HttpPost("{oid}/{payment}")]
        public IActionResult ProcessPayment([FromRoute] string oid, [FromRoute] string payment)
        {
            Order? existingOrder = _orderService.GetOrderByOID(oid);

            if (existingOrder == null)
            {
                return NotFound(new
                {
                    code = HttpStatusCode.NotFound,
                    message = "Not Found"
                });
            }

            if (!existingOrder.OrderStatus.Equals(Status.PENDING))
            {
                return BadRequest(new
                {
                    code = HttpStatusCode.BadRequest,
                    message = "Invalid data"
                });
            }

            bool isPaid = _paymentService.CreatePayment(existingOrder, payment);

            string url = $"/orders/{oid}/payment/{(isPaid ? "success" : "fail")}";

            return Ok(new
            {
                code = HttpStatusCode.OK,
                message = url
            });
        }
    }
}
