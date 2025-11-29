using Avalonia;
using Avalonia.Controls;
using AvaloniaApplication1.ui.pages.main;

namespace AvaloniaApplication1.ui;

public class MainWindow : Window
{
    public MainWindow()
    {
        Title = "Avalonia MVU";
        Width = 800;
        Height = 600;
        var primaryScreen = Screens.Primary;
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

        // 使用 Grid 确保布局撑满
        Content = new MainShell();
    }
}