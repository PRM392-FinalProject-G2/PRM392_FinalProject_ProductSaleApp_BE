using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using ProductSaleApp.Service.Helpers;
using ProductSaleApp.Service.Services.Interfaces;

namespace ProductSaleApp.Service.Services.Implementations;

public class VnPayService : IVnPayService
{
    private readonly VnPaySettings _settings;

    public VnPayService(IConfiguration configuration)
    {
        _settings = new VnPaySettings
        {
            TmnCode = configuration["VnPay:TmnCode"],
            HashSecret = configuration["VnPay:HashSecret"],
            BaseUrl = configuration["VnPay:BaseUrl"],
            ReturnUrl = configuration["VnPay:ReturnUrl"],
            FrontendReturnUrl = configuration["VnPay:FrontendReturnUrl"],
            ApiVersion = configuration["VnPay:ApiVersion"],
            Locale = configuration["VnPay:Locale"],
            CurrCode = configuration["VnPay:CurrCode"]
        };
    }

    public string CreatePaymentUrl(int paymentId, int orderId, decimal amount, string clientIp, string orderInfo = null)
    {
        var vnpParams = new SortedDictionary<string, string>
        {
            ["vnp_Version"] = _settings.ApiVersion,
            ["vnp_Command"] = "pay",
            ["vnp_TmnCode"] = _settings.TmnCode,
            ["vnp_Amount"] = ((long)(amount * 100)).ToString(CultureInfo.InvariantCulture),
            ["vnp_CurrCode"] = _settings.CurrCode,
            ["vnp_TxnRef"] = paymentId.ToString(),
            ["vnp_OrderInfo"] = string.IsNullOrWhiteSpace(orderInfo) ? $"Thanh toan don hang #{orderId}" : orderInfo,
            ["vnp_OrderType"] = "other",
            ["vnp_Locale"] = _settings.Locale,
            ["vnp_ReturnUrl"] = _settings.ReturnUrl,
            ["vnp_IpAddr"] = string.IsNullOrWhiteSpace(clientIp) ? "127.0.0.1" : clientIp,
            ["vnp_CreateDate"] = DateTime.UtcNow.ToString("yyyyMMddHHmmss")
        };

        var query = BuildQuery(vnpParams);
        var hashData = BuildHashData(vnpParams);
        var secureHash = HmacSha512(_settings.HashSecret, hashData);
        var url = $"{_settings.BaseUrl}?{query}&vnp_SecureHashType=HmacSHA512&vnp_SecureHash={secureHash}";
        return url;
    }

    public bool ValidateCallback(IQueryCollection query, out string txnStatus, out string txnRef, out decimal amount, out string message)
    {
        txnStatus = query["vnp_ResponseCode"].ToString();
        txnRef = query["vnp_TxnRef"].ToString();
        message = query["vnp_Message"].ToString();
        var amountStr = query["vnp_Amount"].ToString();
        amount = 0m;
        if (!string.IsNullOrEmpty(amountStr) && long.TryParse(amountStr, out var amountLong))
        {
            amount = amountLong / 100m;
        }

        var receivedHash = query["vnp_SecureHash"].ToString();
        var data = new SortedDictionary<string, string>();
        foreach (var kv in query)
        {
            var key = kv.Key;
            if (key.StartsWith("vnp_", StringComparison.Ordinal) && key != "vnp_SecureHash" && key != "vnp_SecureHashType")
            {
                data[key] = kv.Value.ToString();
            }
        }

        var hashData = BuildHashData(data);
        var computed = HmacSha512(_settings.HashSecret, hashData);
        return string.Equals(receivedHash, computed, StringComparison.OrdinalIgnoreCase);
    }

    private static string BuildQuery(SortedDictionary<string, string> dict)
    {
        return string.Join("&", dict.Select(kvp => $"{kvp.Key}={WebUtility.UrlEncode(kvp.Value)}"));
    }

    private static string BuildHashData(SortedDictionary<string, string> dict)
    {
        return string.Join("&", dict.Select(kvp => $"{kvp.Key}={WebUtility.UrlEncode(kvp.Value)}"));
    }

    private static string HmacSha512(string key, string data)
    {
        using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key));
        var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        var sb = new StringBuilder(hashBytes.Length * 2);
        foreach (var b in hashBytes)
        {
            sb.Append(b.ToString("x2"));
        }
        return sb.ToString();
    }
} 
