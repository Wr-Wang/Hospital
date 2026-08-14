using System;
using System.Globalization;
using System.Windows.Data;
using Hospital.Application.DTOs;

namespace Hospital.App.Converters;

/// <summary>药品下拉展示：取 DRUG 字典项 Name（「药品名 | 规格 | 剂型 | 频次 | ...」）前两段，显示为「药品名 规格」。</summary>
public sealed class DrugDisplayConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DictionaryItemDto { Name: not null } item && item.Name.Length > 0)
        {
            var parts = item.Name.Split('|');
            var name = parts[0].Trim();
            if (parts.Length > 1 && parts[1].Trim().Length > 0)
                return $"{name}  {parts[1].Trim()}";
            return name;
        }
        return string.Empty;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
