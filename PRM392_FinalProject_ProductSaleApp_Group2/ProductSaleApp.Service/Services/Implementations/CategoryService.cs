using System;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using ProductSaleApp.Repository.Models;
using ProductSaleApp.Repository.UnitOfWork;
using ProductSaleApp.Service.BusinessModel;
using ProductSaleApp.Service.Services.Interfaces;

namespace ProductSaleApp.Service.Services.Implementations;

public class CategoryService : CrudService<Category, CategoryBM>, ICategoryService
{
    private readonly IMapper _mapper;

    public CategoryService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
    {
        _mapper = mapper;
    }

    protected override ProductSaleApp.Repository.Repositories.Interfaces.IEntityRepository<Category> GetRepository() => UnitOfWork.CategoryRepository;

    public async Task<PagedResult<CategoryBM>> GetPagedFilteredAsync(CategoryBM filter, int pageNumber, int pageSize)
    {
        var repo = UnitOfWork.CategoryRepository;
        var repositoryFilter = new Category
        {
            Categoryid = filter?.CategoryId ?? 0,
            Categoryname = filter?.CategoryName
        };

        var (entities, total) = await repo.GetPagedWithDetailsAsync(repositoryFilter, pageNumber, pageSize);
        var items = _mapper.Map<IReadOnlyList<CategoryBM>>(entities);
        var totalPages = (int)Math.Ceiling((double)total / pageSize);
        return new PagedResult<CategoryBM>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalItems = total,
            TotalPages = totalPages,
            Items = items
        };
    }
}


