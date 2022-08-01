using System.ComponentModel.DataAnnotations;
using System.Net;
using AutoMapper;
using Inventory.Product.API.Entities;
using Inventory.Product.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Shared.DTOs.Inventory;
using Shared.Enums.Inventory;

namespace Inventory.Product.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryMongoDbRepository _mongoDbRepository;
    private readonly IMapper _mapper;

    public InventoryController(IInventoryMongoDbRepository mongoDbRepository, IMapper mapper)
    {
        _mongoDbRepository = mongoDbRepository;
        _mapper = mapper;
    }
    
    [Route("items/{itemNo}", Name = "GetAllByItemNo")]
    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(IEnumerable<InventoryEntry>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<InventoryEntry>>> GetAllByItemNo([Required]string itemNo)
    {
        var entities = await _mongoDbRepository.GetAllByItemNoAsync(itemNo);
        var result = _mapper.Map<IEnumerable<InventoryEntryDto>>(entities);
        return Ok(result);
    }
    
    [Route("items/{itemNo}/paging", Name = "GetAllByItemNoPagingAsync")]
    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(IEnumerable<InventoryEntry>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<InventoryEntry>>> GetAllByItemNoPagingAsync([Required]string itemNo, [FromQuery] GetInventoryPagingQuery query)
    {
        var entities = await _mongoDbRepository
            .GetAllByItemNoPagingAsync(itemNo, query);
        var result = _mapper.Map<IEnumerable<InventoryEntryDto>>(entities);
        return Ok(result);
    }
    
    [Route("{id}", Name = "GetInventoryById")]
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<InventoryEntry>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<InventoryEntry>>> GetInventoryById([Required] string id)
    {
        var entity = await _mongoDbRepository.GetAllByIdAsync(id);
        var result = _mapper.Map<InventoryEntryDto>(entity);
        return Ok(result);
    }
    
    [HttpPost("purchase/{itemNo}", Name = "PurchaseOrder")]
    [ProducesResponseType(typeof(InventoryEntry), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<InventoryEntry>> PurchaseOrder([Required] string itemNo, [FromBody] PurchaseItemDto entryDto)
    {
        var itemToAdd = new InventoryEntry(ObjectId.GenerateNewId().ToString())
        {
            ItemNo = itemNo,
            Quantity = entryDto.Quantity,
            DocumentType = entryDto.DocumentType,
        };
        var entity = _mapper.Map<InventoryEntry>(itemToAdd);
        await _mongoDbRepository.CreateAsync(entity);
        var result = _mapper.Map<InventoryEntryDto>(entity);
        return Ok(result);
    }
    
    [Route("{id}", Name = "DeleteById")]
    [HttpDelete]
    [ProducesResponseType(typeof(IEnumerable<InventoryEntry>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> DeleteById([Required] string id)
    {
        await _mongoDbRepository.DeleteAsync(id);
        return NoContent();
    }
}