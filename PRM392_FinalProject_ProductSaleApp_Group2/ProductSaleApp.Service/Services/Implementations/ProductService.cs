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
    public ProductService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
    {
    }

    protected override ProductSaleApp.Repository.Repositories.Interfaces.IEntityRepository<Product> GetRepository() => UnitOfWork.ProductRepository;
}


