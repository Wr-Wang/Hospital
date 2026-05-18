using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hospital.Application.DTOs;
using Hospital.Application.Services;

namespace Hospital.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DictionaryController : ControllerBase
{
    private readonly IDictionaryApplicationService _dictionaryService;

    public DictionaryController(IDictionaryApplicationService dictionaryService)
    {
        _dictionaryService = dictionaryService;
    }

    // ===== 字典类型 =====

    [HttpGet("types")]
    public async Task<IActionResult> GetAllTypes()
    {
        var types = await _dictionaryService.GetAllTypesAsync();
        return Ok(types);
    }

    [HttpGet("types/{id:long}")]
    public async Task<IActionResult> GetTypeById(long id)
    {
        var type = await _dictionaryService.GetTypeByIdAsync(id);
        if (type is null)
            return NotFound();
        return Ok(type);
    }

    [HttpPost("types")]
    public async Task<IActionResult> CreateType([FromBody] CreateDictionaryTypeRequest request)
    {
        var dto = new CreateDictionaryTypeDto(request.Code, request.Name, request.Description);
        var id = await _dictionaryService.CreateTypeAsync(dto);
        return CreatedAtAction(nameof(GetTypeById), new { id }, new { id });
    }

    [HttpPut("types/{id:long}")]
    public async Task<IActionResult> UpdateType(long id, [FromBody] UpdateDictionaryTypeRequest request)
    {
        var dto = new UpdateDictionaryTypeDto(request.Name, request.Description);
        await _dictionaryService.UpdateTypeAsync(id, dto);
        return NoContent();
    }

    [HttpPatch("types/{id:long}/activate")]
    public async Task<IActionResult> ActivateType(long id)
    {
        await _dictionaryService.ActivateTypeAsync(id);
        return NoContent();
    }

    [HttpPatch("types/{id:long}/deactivate")]
    public async Task<IActionResult> DeactivateType(long id)
    {
        await _dictionaryService.DeactivateTypeAsync(id);
        return NoContent();
    }

    // ===== 字典项 =====

    [HttpGet("types/{typeId:long}/items")]
    public async Task<IActionResult> GetItemsByType(long typeId)
    {
        var items = await _dictionaryService.GetItemsByTypeIdAsync(typeId);
        return Ok(items);
    }

    [HttpGet("types/by-code/{typeCode}/items")]
    public async Task<IActionResult> GetItemsByTypeCode(string typeCode)
    {
        var items = await _dictionaryService.GetItemsByTypeCodeAsync(typeCode);
        return Ok(items);
    }

    [HttpPost("items")]
    public async Task<IActionResult> CreateItem([FromBody] CreateDictionaryItemRequest request)
    {
        var dto = new CreateDictionaryItemDto(request.TypeId, request.Code, request.Name, request.ParentId, request.SortOrder);
        var id = await _dictionaryService.CreateItemAsync(dto);
        return CreatedAtAction(nameof(GetItemsByType), new { typeId = request.TypeId }, new { id });
    }

    [HttpPut("items/{id:long}")]
    public async Task<IActionResult> UpdateItem(long id, [FromBody] UpdateDictionaryItemRequest request)
    {
        var dto = new UpdateDictionaryItemDto(request.Name, request.ParentId, request.SortOrder);
        await _dictionaryService.UpdateItemAsync(id, dto);
        return NoContent();
    }

    [HttpPatch("items/{id:long}/activate")]
    public async Task<IActionResult> ActivateItem(long id)
    {
        await _dictionaryService.ActivateItemAsync(id);
        return NoContent();
    }

    [HttpPatch("items/{id:long}/deactivate")]
    public async Task<IActionResult> DeactivateItem(long id)
    {
        await _dictionaryService.DeactivateItemAsync(id);
        return NoContent();
    }
}

// Request DTOs
public class CreateDictionaryTypeRequest
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class UpdateDictionaryTypeRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class CreateDictionaryItemRequest
{
    public long TypeId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public long? ParentId { get; set; }
    public int SortOrder { get; set; }
}

public class UpdateDictionaryItemRequest
{
    public string Name { get; set; } = string.Empty;
    public long? ParentId { get; set; }
    public int SortOrder { get; set; }
}
