using AutoMapper;
using ProductSaleApp.Repository.Models;
using ProductSaleApp.Repository.UnitOfWork;
using ProductSaleApp.Service.BusinessModel;
using ProductSaleApp.Service.Services.Interfaces;

namespace ProductSaleApp.Service.Services.Implementations;

public class BrandService : CrudService<Brand, BrandBM>, IBrandService
{
    private readonly IMapper _mapper;

    public BrandService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
    {
        _mapper = mapper;
    }

    protected override ProductSaleApp.Repository.Repositories.Interfaces.IEntityRepository<Brand> GetRepository() => UnitOfWork.BrandRepository;

    public async Task<PagedResult<BrandBM>> GetPagedFilteredAsync(BrandBM filter, int pageNumber, int pageSize)
    {
        var repo = UnitOfWork.BrandRepository;
        var repositoryFilter = new Brand
        {
            Brandid = filter?.BrandId ?? 0,
            Brandname = filter?.BrandName,
            Description = filter?.Description
        };

        var (entities, total) = await repo.GetPagedWithDetailsAsync(repositoryFilter, pageNumber, pageSize);
        var items = _mapper.Map<IReadOnlyList<BrandBM>>(entities);
        var totalPages = (int)System.Math.Ceiling((double)total / pageSize);
        return new PagedResult<BrandBM>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalItems = total,
            TotalPages = totalPages,
            Items = items
        };
    }
}



