using System.Collections.Generic;

namespace ProductSaleApp.API.Models.ResponseModel;

public class UserResponse
{
    public int UserId { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }
    public string Role { get; set; }
    public string Avatarurl { get; set; }

}


