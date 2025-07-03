using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using SWP.Models.Vnpay;

namespace SWP.Libaries
{
    public class VnpayLibrary
    {
        private readonly SortedList<string, string> _requestData = new(new VnPayCompare());
        private readonly SortedList<string, string> _responseData = new(new VnPayCompare());

        public void AddRequestData(string key, string value)
        {
            if (!string.IsNullOrWhiteSpace(key) && !string.IsNullOrWhiteSpace(value))
                _requestData[key] = value;
        }

        public void AddResponseData(string key, string value)
        {
            if (!string.IsNullOrWhiteSpace(key) && !string.IsNullOrWhiteSpace(value))
                _responseData[key] = value;
        }

        public string GetResponseData(string key)
        {
            return _responseData.TryGetValue(key, out var value) ? value : string.Empty;
        }

        public string CreateRequestUrl(string baseUrl, string hashSecret)
        {
            var queryBuilder = new StringBuilder();

            foreach (var (key, value) in _requestData)
            {
                if (!string.IsNullOrWhiteSpace(value))
                    queryBuilder.Append($"{WebUtility.UrlEncode(key)}={WebUtility.UrlEncode(value)}&");
            }

            if (queryBuilder.Length > 0)
                queryBuilder.Length--; // Remove trailing '&'

            var query = queryBuilder.ToString();
            var secureHash = HmacSha512(hashSecret, query);

            return $"{baseUrl}?{query}&vnp_SecureHash={secureHash}";
        }

        public PaymentResponseModel GetFullResponseData(IQueryCollection query, string hashSecret)
        {
            foreach (var (key, value) in query)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                    AddResponseData(key, value);
            }

            var txnRef = GetResponseData("vnp_TxnRef");
            var transId = GetResponseData("vnp_TransactionNo");
            var responseCode = GetResponseData("vnp_ResponseCode");
            var transactionStatus = GetResponseData("vnp_TransactionStatus"); // 🔥 quan trọng!
            var orderInfo = GetResponseData("vnp_OrderInfo");
            var secureHash = query["vnp_SecureHash"];

            var amountRaw = GetResponseData("vnp_Amount");
            decimal.TryParse(amountRaw, out var rawAmount);
            var amount = rawAmount / 100;

            var isValidSignature = ValidateSignature(secureHash, hashSecret);
            if (!isValidSignature)
            {
                return new PaymentResponseModel
                {
                    Success = false
                };
            }

            // ✅ Chỉ khi cả hai đều là "00" mới là thanh toán thành công
            bool isSuccess = responseCode == "00" && transactionStatus == "00";

            return new PaymentResponseModel
            {
                Success = isSuccess,
                OrderDescription = orderInfo,
                OrderId = int.TryParse(txnRef, out var orderId) ? orderId : 0,
                PaymentId = transId,
                TransactionId = transId,
                Token = secureHash,
                VnPayResponseCode = responseCode,
                PaymentMethod = "VnPay",
                PaymentMethodId = 1,
                Amount = amount
            };
        }





        public bool ValidateSignature(string inputHash, string secretKey)
        {
            var data = GetRawResponseDataForHash();
            var computedHash = HmacSha512(secretKey, data);
            return string.Equals(computedHash, inputHash, StringComparison.InvariantCultureIgnoreCase);
        }

        private string GetRawResponseDataForHash()
        {
            var filteredData = _responseData
                .Where(kv => kv.Key != "vnp_SecureHash" && kv.Key != "vnp_SecureHashType" && !string.IsNullOrWhiteSpace(kv.Value));

            var builder = new StringBuilder();
            foreach (var (key, value) in filteredData)
                builder.Append($"{WebUtility.UrlEncode(key)}={WebUtility.UrlEncode(value)}&");

            if (builder.Length > 0)
                builder.Length--; // Remove trailing '&'

            return builder.ToString();
        }

        private static string HmacSha512(string key, string data)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var dataBytes = Encoding.UTF8.GetBytes(data);

            using var hmac = new HMACSHA512(keyBytes);
            var hashBytes = hmac.ComputeHash(dataBytes);

            return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        }

        public string GetIpAddress(HttpContext context)
        {
            try
            {
                var ip = context.Connection.RemoteIpAddress;

                if (ip?.AddressFamily == AddressFamily.InterNetworkV6)
                {
                    ip = Dns.GetHostEntry(ip).AddressList
                        .FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork);
                }

                return ip?.ToString() ?? "127.0.0.1";
            }
            catch
            {
                return "127.0.0.1";
            }
        }
    }

    public class VnPayCompare : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            if (x == y) return 0;
            if (x == null) return -1;
            if (y == null) return 1;

            return CompareInfo.GetCompareInfo("en-US")
                .Compare(x, y, CompareOptions.Ordinal);
        }
    }
}
