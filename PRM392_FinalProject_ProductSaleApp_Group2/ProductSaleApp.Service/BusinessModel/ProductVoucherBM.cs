namespace ProductSaleApp.Service.BusinessModel;

public class ProductVoucherBM
{
    public int ProductVoucherId { get; set; }
    public int VoucherId { get; set; }
    public int ProductId { get; set; }

    public ProductBM Product { get; set; }
    public VoucherBM Voucher { get; set; }
}



