using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductSaleApp.Service.BusinessModel
{
    public class ProductFilterBM
    {
        public int? ProductId { get; set; }
        public string? Search { get; set; }

        public List<int>? CategoryIds { get; set; }
        public List<int>? BrandIds { get; set; }

        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        public double? AverageRating { get; set; }

        // SortBy có thể là "Price", "Popularity", "Category"
        public string? SortBy { get; set; }
    }

}
