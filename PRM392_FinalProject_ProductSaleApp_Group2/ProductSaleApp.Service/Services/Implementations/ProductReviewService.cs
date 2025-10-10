using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using ProductSaleApp.Repository.Models;
using ProductSaleApp.Repository.UnitOfWork;
using ProductSaleApp.Service.BusinessModel;
using ProductSaleApp.Service.Services.Interfaces;

namespace ProductSaleApp.Service.Services.Implementations;

public class ProductReviewService : CrudService<Productreview, ProductReviewBM>, IProductReviewService
{
    private readonly IMapper _mapper;

    public ProductReviewService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
    {
        _mapper = mapper;
    }

    protected override ProductSaleApp.Repository.Repositories.Interfaces.IEntityRepository<Productreview> GetRepository() => UnitOfWork.ProductReviewRepository;

    public async Task<IReadOnlyList<ProductReviewBM>> GetByProductIdAsync(int productId)
    {
        var repo = UnitOfWork.ProductReviewRepository;
        var entities = await repo.GetByProductIdAsync(productId);
        return _mapper.Map<IReadOnlyList<ProductReviewBM>>(entities);
    }

    public async Task<IReadOnlyList<ProductReviewBM>> GetByUserIdAsync(int userId)
    {
        var repo = UnitOfWork.ProductReviewRepository;
        var entities = await repo.GetByUserIdAsync(userId);
        return _mapper.Map<IReadOnlyList<ProductReviewBM>>(entities);
    }

    public async Task<PagedResult<ProductReviewBM>> GetPagedByProductIdAsync(int productId, int pageNumber, int pageSize)
    {
        var repo = UnitOfWork.ProductReviewRepository;
        var (entities, total) = await repo.GetPagedByProductIdAsync(productId, pageNumber, pageSize);
        var items = _mapper.Map<IReadOnlyList<ProductReviewBM>>(entities);
        var totalPages = (int)Math.Ceiling((double)total / pageSize);
        return new PagedResult<ProductReviewBM>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalItems = total,
            TotalPages = totalPages,
            Items = items
        };
    }
}


