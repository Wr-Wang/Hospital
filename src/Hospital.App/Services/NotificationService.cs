using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Hospital.App.Services;

/// <summary>全局通知服务：底部 Toast 消息（成功/错误/信息）</summary>
public interface INotificationService
{
    void Success(string message);
    void Error(string message);
    void Info(string message);
}

/// <summary>
/// 底部 Toast 实现（单例）：固定定位在窗口底部居中、滚动不消失，
/// 每次显示 3 秒后自动隐藏。
/// </summary>
public sealed partial class NotificationService : ObservableObject, INotificationService
{
    private static readonly Brush SuccessBackground = new SolidColorBrush(Color.FromRgb(0x05, 0x96, 0x67)); // Green600
    private static readonly Brush ErrorBackground = new SolidColorBrush(Color.FromRgb(0xB9, 0x1C, 0x1C));   // Red700
    private static readonly Brush InfoBackground = new SolidColorBrush(Color.FromRgb(0x0D, 0x94, 0x88));    // BluePrimary
    private static readonly Brush WhiteForeground = new SolidColorBrush(Colors.White);

    private DispatcherTimer? _hideTimer;

    [ObservableProperty]
    private bool isVisible;

    [ObservableProperty]
    private string message = string.Empty;

    [ObservableProperty]
    private string icon = "✓";

    [ObservableProperty]
    private Brush background = SuccessBackground;

    [ObservableProperty]
    private Brush foreground = WhiteForeground;

    public void Success(string message) => Show(message, SuccessBackground, "✓");

    public void Error(string message) => Show(message, ErrorBackground, "✕");

    public void Info(string message) => Show(message, InfoBackground, "ℹ");

    private void Show(string message, Brush background, string icon)
    {
        Message = message;
        Background = background;
        Icon = icon;
        IsVisible = true;

        // 每次显示重新计时，避免旧定时器误隐藏新 Toast
        _hideTimer?.Stop();
        _hideTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
        _hideTimer.Tick += (_, _) =>
        {
            _hideTimer?.Stop();
            IsVisible = false;
        };
        _hideTimer.Start();
    }
}
