using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using ProductSaleApp.Repository.UnitOfWork;
using ProductSaleApp.Service.BusinessModel;
using ProductSaleApp.Service.Services.Interfaces;

namespace ProductSaleApp.Service.Services.Implementations;

public abstract class CrudService<TEntity, TBusinessModel> : ICrudService<TBusinessModel>
    where TEntity : class
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    protected readonly IUnitOfWork UnitOfWork;

    protected CrudService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        UnitOfWork = unitOfWork;
    }

    protected abstract ProductSaleApp.Repository.Repositories.Interfaces.IEntityRepository<TEntity> GetRepository();

    public async Task<TBusinessModel> GetByIdAsync(int id, bool includeDetails = true)
    {
        var repo = GetRepository();
        var entity = includeDetails
            ? await repo.GetByIdWithDetailsAsync(id)
            : await repo.GetByIdAsync(id);
        return _mapper.Map<TBusinessModel>(entity);
    }

    public async Task<PagedResult<TBusinessModel>> GetPagedAsync(int pageNumber, int pageSize)
    {
        var repo = GetRepository();
        var (entities, total) = await repo.GetPagedWithDetailsAsync(pageNumber, pageSize);
        var items = _mapper.Map<IReadOnlyList<TBusinessModel>>(entities);
        var totalPages = (int)Math.Ceiling((double)total / pageSize);
        return new PagedResult<TBusinessModel>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalItems = total,
            TotalPages = totalPages,
            Items = items
        };
    }

    public async Task<TBusinessModel> CreateAsync(TBusinessModel model)
    {
        var entity = _mapper.Map<TEntity>(model);
        await GetRepository().AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<TBusinessModel>(entity);
    }

    public virtual async Task<TBusinessModel> UpdateAsync(int id, TBusinessModel model)
    {
        var repo = GetRepository();
        // Use tracking = true for update operations
        var existing = await repo.GetByIdAsync(id, trackChanges: true);
        if (existing == null)
        {
            return default;
        }

        _mapper.Map(model, existing);
        repo.Update(existing);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<TBusinessModel>(existing);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var repo = GetRepository();
        // Use tracking = true for delete operations
        var existing = await repo.GetByIdAsync(id, trackChanges: true);
        if (existing == null)
        {
            return false;
        }
        repo.Delete(existing);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}


