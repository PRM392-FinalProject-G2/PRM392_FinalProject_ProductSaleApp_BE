using System;

namespace ProductSaleApp.Service.BusinessModel;

public class UserDeviceTokenBM
{
    public int TokenId { get; set; }
    public int UserId { get; set; }
    public string FcmToken { get; set; }
    public bool IsActive { get; set; }
    public DateTime LastUpdatedDate { get; set; }
    
    // Navigation
    public UserBM User { get; set; }
}


