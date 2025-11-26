using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Markup.Declarative;
using Avalonia.Media;
using Avalonia.Data;
using Avalonia.Input;
using AvaloniaApplication1.ui.pages.main;

namespace AvaloniaApplication1.ui;

public class MainWindow : Window
{
    public MainWindow()
    {
        Title = "Avalonia MVU";
        var primaryScreen = this.Screens.Primary;
        if (primaryScreen != null)
        {
            var workingArea = primaryScreen.WorkingArea;
            Width = workingArea.Width * 0.5;
            Height = workingArea.Height * 0.5;
            Position = new PixelPoint(
                workingArea.X + (workingArea.Width / 4),
                workingArea.Y + (workingArea.Height / 4)
            );
        }
        else
        {
            // 如果获取屏幕信息失败，设置默认回退值
            Width = 800;
            Height = 600;
        }

        // 使用 Grid 确保布局撑满
        Content = new MainShell();
    }
}