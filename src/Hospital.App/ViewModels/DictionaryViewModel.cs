using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hospital.Application.DTOs;
using Hospital.Application.Services;

namespace Hospital.App.ViewModels;

/// <summary>字典管理 ViewModel</summary>
public sealed partial class DictionaryViewModel : ObservableObject
{
    private readonly IDictionaryApplicationService _dictService;

    public DictionaryViewModel(IDictionaryApplicationService dictService)
    {
        _dictService = dictService;
    }

    [ObservableProperty]
    private string? errorMessage;

    [ObservableProperty]
    private bool isBusy;

    // ===== 字典类型 =====
    [ObservableProperty]
    private List<DictionaryTypeDto> types = new();

    [ObservableProperty]
    private DictionaryTypeDto? selectedType;

    [ObservableProperty]
    private bool showTypeForm;

    [ObservableProperty]
    private string typeFormCode = string.Empty;

    [ObservableProperty]
    private string typeFormName = string.Empty;

    [ObservableProperty]
    private string? typeFormDesc;

    [ObservableProperty]
    private bool isEditingType;

    [ObservableProperty]
    private long editingTypeId;

    // ===== 字典项 =====
    [ObservableProperty]
    private List<DictionaryItemDto> items = new();

    [ObservableProperty]
    private bool showItemForm;

    [ObservableProperty]
    private string itemFormCode = string.Empty;

    [ObservableProperty]
    private string itemFormName = string.Empty;

    [ObservableProperty]
    private int itemFormSortOrder;

    [ObservableProperty]
    private bool isEditingItem;

    [ObservableProperty]
    private long editingItemId;

    public async Task InitializeAsync()
    {
        await LoadTypesAsync();
    }

    private async Task LoadTypesAsync()
    {
        IsBusy = true;
        ErrorMessage = null;

        try
        {
            Types = await _dictService.GetAllTypesAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"加载字典类型失败: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    partial void OnSelectedTypeChanged(DictionaryTypeDto? value)
    {
        if (value is not null)
            _ = LoadItemsAsync();
    }

    private async Task LoadItemsAsync()
    {
        if (SelectedType is null) return;

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            Items = await _dictService.GetItemsByTypeIdAsync(SelectedType.Id);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"加载字典项失败: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    // ===== 类型操作 =====

    [RelayCommand]
    private void ShowCreateTypeForm()
    {
        IsEditingType = false;
        EditingTypeId = 0;
        TypeFormCode = string.Empty;
        TypeFormName = string.Empty;
        TypeFormDesc = null;
        ShowTypeForm = true;
    }

    [RelayCommand]
    private void ShowEditTypeForm(DictionaryTypeDto? type)
    {
        if (type is null) return;
        IsEditingType = true;
        EditingTypeId = type.Id;
        TypeFormCode = type.Code;
        TypeFormName = type.Name;
        TypeFormDesc = type.Description;
        ShowTypeForm = true;
    }

    [RelayCommand]
    private void CancelTypeForm() => ShowTypeForm = false;

    [RelayCommand]
    private async Task SaveType()
    {
        if (string.IsNullOrWhiteSpace(TypeFormName))
        {
            ErrorMessage = "类型名称不能为空";
            return;
        }

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            if (IsEditingType)
            {
                var dto = new UpdateDictionaryTypeDto(TypeFormName, TypeFormDesc);
                await _dictService.UpdateTypeAsync(EditingTypeId, dto);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(TypeFormCode))
                {
                    ErrorMessage = "类型编码不能为空";
                    return;
                }

                var dto = new CreateDictionaryTypeDto(TypeFormCode, TypeFormName, TypeFormDesc);
                await _dictService.CreateTypeAsync(dto);
            }

            ShowTypeForm = false;
            await LoadTypesAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"保存类型失败: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task ToggleTypeActive(DictionaryTypeDto? type)
    {
        if (type is null) return;

        try
        {
            if (type.IsActive)
                await _dictService.DeactivateTypeAsync(type.Id);
            else
                await _dictService.ActivateTypeAsync(type.Id);

            await LoadTypesAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"操作失败: {ex.Message}";
        }
    }

    // ===== 字典项操作 =====

    [RelayCommand]
    private void ShowCreateItemForm()
    {
        IsEditingItem = false;
        EditingItemId = 0;
        ItemFormCode = string.Empty;
        ItemFormName = string.Empty;
        ItemFormSortOrder = 0;
        ShowItemForm = true;
    }

    [RelayCommand]
    private void ShowEditItemForm(DictionaryItemDto? item)
    {
        if (item is null) return;
        IsEditingItem = true;
        EditingItemId = item.Id;
        ItemFormCode = item.Code;
        ItemFormName = item.Name;
        ItemFormSortOrder = item.SortOrder;
        ShowItemForm = true;
    }

    [RelayCommand]
    private void CancelItemForm() => ShowItemForm = false;

    [RelayCommand]
    private async Task SaveItem()
    {
        if (SelectedType is null)
        {
            ErrorMessage = "请先选择一个字典类型";
            return;
        }

        if (string.IsNullOrWhiteSpace(ItemFormName))
        {
            ErrorMessage = "字典项名称不能为空";
            return;
        }

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            if (IsEditingItem)
            {
                var dto = new UpdateDictionaryItemDto(ItemFormName, null, ItemFormSortOrder);
                await _dictService.UpdateItemAsync(EditingItemId, dto);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(ItemFormCode))
                {
                    ErrorMessage = "字典项编码不能为空";
                    return;
                }

                var dto = new CreateDictionaryItemDto(SelectedType.Id, ItemFormCode, ItemFormName, null, ItemFormSortOrder);
                await _dictService.CreateItemAsync(dto);
            }

            ShowItemForm = false;
            await LoadItemsAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"保存字典项失败: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task ToggleItemActive(DictionaryItemDto? item)
    {
        if (item is null) return;

        try
        {
            if (item.IsActive)
                await _dictService.DeactivateItemAsync(item.Id);
            else
                await _dictService.ActivateItemAsync(item.Id);

            await LoadItemsAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"操作失败: {ex.Message}";
        }
    }
}
