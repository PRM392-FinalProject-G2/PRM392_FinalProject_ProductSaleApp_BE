using AutoMapper;
using ProductSaleApp.Repository.Models;
using ProductSaleApp.Repository.UnitOfWork;
using ProductSaleApp.Service.BusinessModel;
using ProductSaleApp.Service.Services.Interfaces;

namespace ProductSaleApp.Service.Services.Implementations;

public class VoucherService : CrudService<Voucher, VoucherBM>, IVoucherService
{
    public VoucherService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
    {
    }

    protected override ProductSaleApp.Repository.Repositories.Interfaces.IEntityRepository<Voucher> GetRepository() => UnitOfWork.VoucherRepository;
}



