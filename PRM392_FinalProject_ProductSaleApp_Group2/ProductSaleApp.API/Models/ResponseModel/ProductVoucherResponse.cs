namespace ProductSaleApp.API.Models.ResponseModel;

public class ProductVoucherResponse
{
    public int ProductVoucherId { get; set; }
    public int VoucherId { get; set; }
    public int ProductId { get; set; }

    public ProductResponse Product { get; set; }
    public VoucherResponse Voucher { get; set; }
}



