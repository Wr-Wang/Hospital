namespace Hospital.Application.DTOs;

// ===== 字典类型（DictionaryType） =====

public sealed record DictionaryTypeDto(
    long Id,
    string Code,
    string Name,
    string? Description,
    bool IsActive);

public sealed record CreateDictionaryTypeDto(
    string Code,
    string Name,
    string? Description);

public sealed record UpdateDictionaryTypeDto(
    string Name,
    string? Description);

// ===== 字典项（DictionaryItem） =====

public sealed record DictionaryItemDto(
    long Id,
    long TypeId,
    string Code,
    string Name,
    long? ParentId,
    int SortOrder,
    bool IsActive);

public sealed record CreateDictionaryItemDto(
    long TypeId,
    string Code,
    string Name,
    long? ParentId,
    int SortOrder);

public sealed record UpdateDictionaryItemDto(
    string Name,
    long? ParentId,
    int SortOrder);
