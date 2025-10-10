using System;

namespace ProductSaleApp.API.Models.ResponseModel;

public class UserDeviceTokenResponse
{
    public int TokenId { get; set; }
    public int UserId { get; set; }
    public string FcmToken { get; set; }
    public bool IsActive { get; set; }
    public DateTime LastUpdatedDate { get; set; }
}


