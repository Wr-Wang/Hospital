using System;
using System.Globalization;
using System.Windows.Data;

namespace Hospital.App.Converters;

/// <summary>
/// 下拉框「闭合态」展示转换：
/// ComboBox 自定义模板中，ContentSite 需同时支持 ItemTemplate 与 DisplayMemberPath。
/// 取 (SelectedItem, DisplayMemberPath)：
///   - 未选中 → null（显示占位水印）
///   - DisplayMemberPath 为空 → 原样返回项，交由 ItemTemplate 渲染
///   - DisplayMemberPath 非空 → 反射取该属性值作为展示文本
/// </summary>
public sealed class SelectionDisplayConverter : IMultiValueConverter
{
    public object? Convert(object[] values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Length < 2 || values[0] is null)
            return null;

        var item = values[0];
        var path = values[1] as string;

        if (string.IsNullOrWhiteSpace(path))
            return item; // 交给 ItemTemplate 渲染

        var prop = item.GetType().GetProperty(path);
        return prop?.GetValue(item) ?? item.ToString();
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
