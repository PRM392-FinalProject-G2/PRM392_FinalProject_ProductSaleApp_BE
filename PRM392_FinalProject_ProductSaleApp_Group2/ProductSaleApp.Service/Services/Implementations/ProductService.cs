using System;
using System.Linq;
using AutoMapper;
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

    public async Task<PagedResult<ProductBM>> GetPagedFilteredAsync(ProductBM filter, int pageNumber, int pageSize)
    {
        var repo = UnitOfWork.ProductRepository;
        var repositoryFilter = new Product
        {
            Productid = filter?.ProductId ?? 0,
            Productname = filter?.ProductName,
            Categoryid = filter?.CategoryId,
            Brandid = filter?.BrandId
        };

        var (entities, total) = await repo.GetPagedWithDetailsAsync(repositoryFilter, pageNumber, pageSize);
        var items = _mapper.Map<IReadOnlyList<ProductBM>>(entities);
        var totalPages = (int)Math.Ceiling((double)total / pageSize);
        return new PagedResult<ProductBM>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalItems = total,
            TotalPages = totalPages,
            Items = items
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


