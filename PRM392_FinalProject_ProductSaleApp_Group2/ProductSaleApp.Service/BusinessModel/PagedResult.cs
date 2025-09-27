using System.Collections.Generic;

namespace ProductSaleApp.Service.BusinessModel;

public class PagedResult<T>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
    public IReadOnlyList<T> Items { get; set; }
}


