using AutoMapper;
using Inventory.Product.API.Entities;
using Shared.DTOs.Inventory;

namespace Inventory.Product.API;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<InventoryEntry, InventoryEntryDto>();
        CreateMap<PurchaseItemDto, InventoryEntry>();
    }
}