using ECommerceMVC.Helper;
using ECommerceMVC.Models.Entities;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace ECommerceMVC.Helpers
{
    public static class VnPayHelper
    {
        public static string CreatePaymentUrl(VnPayConfig settings, HoaDon hoaDon, double amount, string clientIp)
        {
            var vnpParams = new Dictionary<string, string>
            {
                { "vnp_Version", "2.1.0" },
                { "vnp_Command", "pay" },
                { "vnp_TmnCode", settings.TmnCode },
                { "vnp_Amount", ((long)(amount * 100)).ToString() },
                { "vnp_CurrCode", "VND" },
                { "vnp_TxnRef", hoaDon.MaHd.ToString() },
                { "vnp_OrderInfo", $"Thanh toan don hang {hoaDon.MaHd}" },
                { "vnp_OrderType", "other" },
                { "vnp_Locale", "vn" },
                { "vnp_ReturnUrl", settings.ReturnUrl },
                { "vnp_IpAddr", clientIp },
                { "vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss") }
            };

            // Sắp xếp theo key
            var sorted = vnpParams
                .Where(k => !string.IsNullOrEmpty(k.Value))
                .OrderBy(k => k.Key)
                .ToList();

            // Tạo query string VÀ hash data (đều encode)
            var data = new StringBuilder();
            foreach (var kvp in sorted)
            {
                data.Append(WebUtility.UrlEncode(kvp.Key) + "=" + WebUtility.UrlEncode(kvp.Value) + "&");
            }

            var queryString = data.ToString();

            // Bỏ dấu & cuối cùng để tính hash
            var signData = queryString;
            if (signData.Length > 0)
            {
                signData = signData.Remove(signData.Length - 1, 1);
            }

            // Tính hash từ chuỗi ĐÃ ENCODE
            string vnp_SecureHash = HmacSHA512(settings.HashSecret, signData);

            // Tạo URL cuối cùng
            var finalUrl = $"{settings.Url}?{queryString}vnp_SecureHash={vnp_SecureHash}";

            // Debug
            Console.WriteLine("=== CREATE PAYMENT ===");
            Console.WriteLine("SignData: " + signData);
            Console.WriteLine("SecureHash: " + vnp_SecureHash);
            Console.WriteLine("Final URL: " + finalUrl);

            return finalUrl;
        }

        public static bool ValidateSignature(Dictionary<string, string> queryParams, string hashSecret)
        {
            Console.WriteLine("=== VALIDATE SIGNATURE ===");

            if (!queryParams.ContainsKey("vnp_SecureHash"))
            {
                Console.WriteLine("Không tìm thấy vnp_SecureHash");
                return false;
            }

            var vnp_SecureHash = queryParams["vnp_SecureHash"];
            Console.WriteLine("Received Hash: " + vnp_SecureHash);

            // Loại bỏ vnp_SecureHash và vnp_SecureHashType
            var sorted = queryParams
                .Where(k => k.Key.StartsWith("vnp_")
                         && k.Key != "vnp_SecureHash"
                         && k.Key != "vnp_SecureHashType")
                .OrderBy(k => k.Key)
                .ToList();

            Console.WriteLine($"Số params: {sorted.Count}");

            // Tạo chuỗi hash (ENCODE giống như khi tạo)
            var data = new StringBuilder();
            foreach (var kvp in sorted)
            {
                Console.WriteLine($"  {kvp.Key} = {kvp.Value}");
                data.Append(WebUtility.UrlEncode(kvp.Key) + "=" + WebUtility.UrlEncode(kvp.Value) + "&");
            }

            var signData = data.ToString();
            if (signData.Length > 0)
            {
                signData = signData.Remove(signData.Length - 1, 1);
            }

            Console.WriteLine("SignData: " + signData);
            Console.WriteLine("HashSecret: " + hashSecret);

            var checkHash = HmacSHA512(hashSecret, signData);

            Console.WriteLine("Computed Hash: " + checkHash);
            Console.WriteLine("Match: " + vnp_SecureHash.Equals(checkHash, StringComparison.OrdinalIgnoreCase));

            return vnp_SecureHash.Equals(checkHash, StringComparison.OrdinalIgnoreCase);
        }

        private static string HmacSHA512(string key, string data)
        {
            var hash = new StringBuilder();
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var dataBytes = Encoding.UTF8.GetBytes(data);

            using (var hmac = new HMACSHA512(keyBytes))
            {
                var hashValue = hmac.ComputeHash(dataBytes);
                foreach (var theByte in hashValue)
                {
                    hash.Append(theByte.ToString("x2"));
                }
            }

            return hash.ToString();
        }
    }
}