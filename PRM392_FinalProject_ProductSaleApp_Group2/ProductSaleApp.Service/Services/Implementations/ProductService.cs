using System;
using System.Linq;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProductSaleApp.Repository.Models;
using ProductSaleApp.Repository.UnitOfWork;
using ProductSaleApp.Service.BusinessModel;
using ProductSaleApp.Service.Services.Interfaces;

namespace ProductSaleApp.Service.Services.Implementations;

public class ProductService : CrudService<Product, ProductBM>, IProductService
{
    private readonly IMapper _mapper;

    public ProductService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
    {
        _mapper = mapper;
    }

    protected override ProductSaleApp.Repository.Repositories.Interfaces.IEntityRepository<Product> GetRepository() => UnitOfWork.ProductRepository;

    public async Task<PagedResult<ProductBM>> GetPagedFilteredAsync(ProductFilterBM filter, int pageNumber, int pageSize)
    {
        var repo = UnitOfWork.ProductRepository;

        var (entities, _) = await repo.GetPagedWithDetailsAsync(
            new Product { Productid = filter.ProductId ?? 0, Productname = null},
            1, int.MaxValue // lấy tất cả, lọc tiếp ở service
        );

        // list/price/rating trên service
        var query = entities.AsQueryable();

        if (!string.IsNullOrEmpty(filter.Search))
        {
            var keyword = filter.Search.ToLower();

            query = query.Where(p =>
                (p.Productname != null && p.Productname.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0) ||
                (p.Fulldescription != null && p.Fulldescription.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0) ||
                (p.Technicalspecifications != null && p.Technicalspecifications.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0) ||
                (p.Brand.Brandname != null && p.Brand.Brandname.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0) ||
                (p.Category.Categoryname != null && p.Category.Categoryname.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
            );
        }




        if (filter.CategoryIds != null && filter.CategoryIds.Any())
            query = query.Where(p => filter.CategoryIds.Contains(p.Categoryid ?? 0));

        if (filter.BrandIds != null && filter.BrandIds.Any())
            query = query.Where(p => filter.BrandIds.Contains(p.Brandid ?? 0));

        if (filter.MinPrice.HasValue)
            query = query.Where(p => p.Price >= filter.MinPrice.Value);

        if (filter.MaxPrice.HasValue)
            query = query.Where(p => p.Price <= filter.MaxPrice.Value);

        if (filter.AverageRating.HasValue)
            query = query.Where(p => p.Averagerating >= (decimal)filter.AverageRating.Value);

        // Sort
        query = filter.SortBy?.ToLower() switch
        {
            "price" => query.OrderBy(p => p.Price),
            "popularity" => query.OrderByDescending(p => p.Popularity), 
            "category" => query.OrderBy(p => p.Category.Categoryname),
            _ => query.OrderByDescending(p => p.Popularity).ThenByDescending(p => p.Averagerating)
        };

        // Pagination
        var total = query.Count();
        var items = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

        // Map sang BM
        var mappedItems = _mapper.Map<IReadOnlyList<ProductBM>>(items);

        return new PagedResult<ProductBM>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalItems = total,
            TotalPages = (int)Math.Ceiling((double)total / pageSize),
            Items = mappedItems
        };
    }


    public async Task<bool> IncrementPopularityAsync(IEnumerable<int> productIds, int delta = 1)
    {
        var repo = UnitOfWork.ProductRepository;
        var updated = false;
        foreach (var id in productIds.Distinct())
        {
            var entity = await repo.GetByIdAsync(id, trackChanges: true);
            if (entity == null) continue;
            entity.Popularity += delta;
            repo.Update(entity);
            updated = true;
        }
        if (updated)
        {
            await UnitOfWork.SaveChangesAsync();
        }
        return updated;
    }
}


