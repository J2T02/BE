using SWP.Libaries;
using SWP.Models.Vnpay;

namespace SWP.Service.Vnpay
{
    public class VnPayService : IVnPayService
    {
        private readonly IConfiguration _configuration;

        public VnPayService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreatePaymentUrl(PaymentInformationModel model, HttpContext context)
        {
            var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_configuration["TimeZoneId"]);
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);

            var pay = new VnpayLibrary();
            var returnUrl = _configuration["Vnpay:ReturnUrl"];

            pay.AddRequestData("vnp_Version", _configuration["Vnpay:Version"]);
            pay.AddRequestData("vnp_Command", _configuration["Vnpay:Command"]);
            pay.AddRequestData("vnp_TmnCode", _configuration["Vnpay:TmnCode"]);

            // VNP yêu cầu amount x100, không có thập phân
            int amount = (int)(model.Amount * 100);
            pay.AddRequestData("vnp_Amount", amount.ToString());

            pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", _configuration["Vnpay:CurrCode"]);
            pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(context));
            pay.AddRequestData("vnp_Locale", _configuration["Vnpay:Locale"]);

            // ✅ Sử dụng BookingId làm TxnRef
            pay.AddRequestData("vnp_TxnRef", model.OrderId.ToString());

            // Mô tả đơn hàng
            var orderInfo = $"Thanh toan lich {model.OrderId}";
            pay.AddRequestData("vnp_OrderInfo", orderInfo);

            pay.AddRequestData("vnp_OrderType", model.OrderType ?? "other");
            pay.AddRequestData("vnp_ReturnUrl", returnUrl);

            return pay.CreateRequestUrl(
                _configuration["Vnpay:BaseUrl"].Trim(),
                _configuration["Vnpay:HashSecret"]
            );
        }

        public PaymentResponseModel PaymentExecute(IQueryCollection collections)
        {
            if (collections == null || !collections.Any())
            {
                return new PaymentResponseModel
                {
                    Success = false,
                    OrderDescription = "Dữ liệu truy vấn không hợp lệ."
                };
            }

            var vnPay = new VnpayLibrary();
            var response = vnPay.GetFullResponseData(collections, _configuration["Vnpay:HashSecret"]);

            if (!response.Success)
            {
                response.OrderDescription = "Xác thực chữ ký thất bại hoặc giao dịch không hợp lệ.";
            }

            return response;
        }
    }
}
