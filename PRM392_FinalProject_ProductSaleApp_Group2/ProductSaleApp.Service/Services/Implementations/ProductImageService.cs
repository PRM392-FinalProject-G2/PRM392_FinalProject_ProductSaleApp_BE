using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using ProductSaleApp.Repository.Models;
using ProductSaleApp.Repository.UnitOfWork;
using ProductSaleApp.Service.BusinessModel;
using ProductSaleApp.Service.Services.Interfaces;

namespace ProductSaleApp.Service.Services.Implementations;

public class ProductImageService : CrudService<Productimage, ProductImageBM>, IProductImageService
{
    private readonly IMapper _mapper;

    public ProductImageService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
    {
        _mapper = mapper;
    }

    protected override ProductSaleApp.Repository.Repositories.Interfaces.IEntityRepository<Productimage> GetRepository() => UnitOfWork.ProductImageRepository;

    public async Task<IReadOnlyList<ProductImageBM>> GetByProductIdAsync(int productId)
    {
        var repo = UnitOfWork.ProductImageRepository;
        var entities = await repo.GetByProductIdAsync(productId);
        return _mapper.Map<IReadOnlyList<ProductImageBM>>(entities);
    }
}


