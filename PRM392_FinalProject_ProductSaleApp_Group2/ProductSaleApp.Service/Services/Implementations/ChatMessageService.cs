using System;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using ProductSaleApp.Repository.Models;
using ProductSaleApp.Repository.UnitOfWork;
using ProductSaleApp.Service.BusinessModel;
using ProductSaleApp.Service.Services.Interfaces;

namespace ProductSaleApp.Service.Services.Implementations;

public class ChatMessageService : CrudService<Chatmessage, ChatMessageBM>, IChatMessageService
{
    private readonly IMapper _mapper;

    public ChatMessageService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
    {
        _mapper = mapper;
    }

    protected override ProductSaleApp.Repository.Repositories.Interfaces.IEntityRepository<Chatmessage> GetRepository() => UnitOfWork.ChatMessageRepository;

    public async Task<PagedResult<ChatMessageBM>> GetPagedFilteredAsync(ChatMessageBM filter, int pageNumber, int pageSize)
    {
        var repo = UnitOfWork.ChatMessageRepository;
        var repositoryFilter = new Chatmessage
        {
            Chatmessageid = filter?.ChatMessageId ?? 0,
            Senderid = filter?.SenderId ?? 0,
            Receiverid = filter?.ReceiverId ?? 0
        };

        var (entities, total) = await repo.GetPagedWithDetailsAsync(repositoryFilter, pageNumber, pageSize);
        var items = _mapper.Map<IReadOnlyList<ChatMessageBM>>(entities);
        var totalPages = (int)System.Math.Ceiling((double)total / pageSize);
        return new PagedResult<ChatMessageBM>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalItems = total,
            TotalPages = totalPages,
            Items = items
        };
    }
}


