using System;
using System.IO;
using System.Text.Json;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Themes.Fluent;
using AvaloniaApplication1.ui;

namespace AvaloniaApplication1;

public class App : Application
{
    private readonly string _configPath = Path.Combine(AppContext.BaseDirectory, "window.layout.json");

    private class WindowLayout
    {
        public int X { get; set; }
        public int Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public WindowState State { get; set; }
    }

    private WindowLayout _savedLayout = new WindowLayout(); // 内存缓存

    public override void Initialize()
    {
        Styles.Add(new FluentTheme());
    }


    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindow = new MainWindow();
            desktop.MainWindow = mainWindow;
            LoadLayout(mainWindow);
            mainWindow.PositionChanged += (s, e) => UpdateLayoutConfig(mainWindow);
            mainWindow.Resized += (s, e) => UpdateLayoutConfig(mainWindow);
            mainWindow.PropertyChanged += (s, e) =>
            {
                if (e.Property.Name == nameof(Window.WindowState))
                    UpdateLayoutConfig(mainWindow);
            };
            mainWindow.Closing += (s, e) => SaveLayoutToDisk();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void UpdateLayoutConfig(Window window)
    {
        // 记录当前状态 (Maximized / Normal / Minimized)
        _savedLayout.State = window.WindowState;

        // 【关键】只有在 Normal 状态下，才更新位置和尺寸
        // 如果当前是最大化，不要更新！保留最后一次 Normal 的数据
        // 还要排除最小化的情况
        if (window.WindowState != WindowState.Normal) return;
        // 排除坐标为 0,0 且尺寸巨大的极端情况（防止 Snap 贴靠干扰，可选）
        // 但通常只要 State 是 Normal，我们就该信它
        if (!(window.Bounds.Width > 0) || !(window.Bounds.Height > 0)) return;

        _savedLayout.X = window.Position.X;
        _savedLayout.Y = window.Position.Y;
        _savedLayout.Width = window.Bounds.Width;
        _savedLayout.Height = window.Bounds.Height;
    }

    private void LoadLayout(MainWindow mainWindow)
    {
        if (!File.Exists(_configPath)) return;
        var json = File.ReadAllText(_configPath);
        var layout = JsonSerializer.Deserialize<WindowLayout>(json);
        if (layout == null) return;
        mainWindow.Width = layout.Width;
        mainWindow.Height = layout.Height;

        mainWindow.WindowStartupLocation = WindowStartupLocation.Manual;
        mainWindow.Position = new PixelPoint(layout.X, layout.Y);
        mainWindow.WindowState = layout.State;
    }

    private void SaveLayoutToDisk()
    {
        var json = JsonSerializer.Serialize(_savedLayout);
        File.WriteAllText(_configPath, json);
    }
}