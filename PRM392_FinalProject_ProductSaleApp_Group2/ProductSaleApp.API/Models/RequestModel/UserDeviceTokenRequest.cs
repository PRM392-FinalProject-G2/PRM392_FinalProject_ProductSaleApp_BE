using System.ComponentModel.DataAnnotations;

namespace ProductSaleApp.API.Models.RequestModel;

public class UserDeviceTokenRequest
{
    [Required(ErrorMessage = "User ID is required")]
    public int UserId { get; set; }

    [Required(ErrorMessage = "FCM Token is required")]
    public string FcmToken { get; set; }
}


