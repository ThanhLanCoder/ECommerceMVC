using System.Security.Cryptography;
using System.Text;
using ECommerceMVC.Models.Entities;

public static class OrderQrHelper
{
    public static string GenerateKey(HoaDon order, string secret)
    {
        string raw = $"{order.MaHd}|{order.NgayDat}|{order.HoTen}|{order.DienThoai}|{secret}";
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(raw));
        return BitConverter.ToString(bytes).Replace("-", "").ToLower();
    }

    public static string BuildTrackingUrl(HttpRequest req, int orderId, string key)
    {
        return $"{req.Scheme}://{req.Host}/OrderTracking/ViewOrder/{orderId}?key={key}";
    }

}
