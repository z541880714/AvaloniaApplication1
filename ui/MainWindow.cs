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
using AvaloniaApplication1.viewmodel; // 👈 必须加这个引用，才能用 BindingMode

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

// 继承 ComponentBase，它实现了 INotifyPropertyChanged
public class DashboardComponent(Window window) : ComponentBase
{
    // 定义数据属性
    public string CurrentPage { get; set; } = "Home";
    public int ClickCount { get; set; } = 1;
    private TextBlock _textBlock = new();
    private string TextBlockStr => $"{CurrentPage}_count: {ClickCount}";
    private readonly MainViewModel _vm = new();

    protected override void OnInitialized()
    {
        base.OnInitialized();
        DataContext = _vm;
    }


    protected override object Build()
    {
        return new Grid()
            .Rows("Auto, Auto, *")
            .Children(
                new TextBlock()
                    .Row(0)
                    .Text(TextBlockStr)
                    .FontSize(30)
                    .Foreground(Brushes.Crimson)
                    .HorizontalAlignment(HorizontalAlignment.Center)
                    .Ref(out _textBlock),
                new StackPanel()
                    .Row(1)
                    .Orientation(Orientation.Horizontal)
                    .Spacing(15)
                    .HorizontalAlignment(HorizontalAlignment.Center)
                    .Children(
                        SidebarItem("首页 (Home)", () => SetPage("Home")),
                        SidebarItem("设置 (Settings)", () => SetPage("Settings")),
                        SidebarItem("vm", () =>
                            {
                                ClickCount++;
                                _vm.Message = $"vm_{ClickCount}";
                            })
                            .Content(new Binding(nameof(_vm.Message), BindingMode.TwoWay))
                    ),
                new GridListComponent()
                    .Row(2)
            );
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        Console.WriteLine("component launched .....");
        window.Activated += OnWindowActivated;
        window.Deactivated += OnWindowDeactivated;
        window.PointerEntered += OnWindowMouseEnter;
        window.PointerExited += OnWindowMouseLeave;
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        window.Activated -= OnWindowActivated;
        window.Deactivated -= OnWindowDeactivated;
        window.PointerEntered -= OnWindowMouseEnter;
        window.PointerExited -= OnWindowMouseLeave;
    }

    private void OnWindowActivated(object? sender, EventArgs e)
    {
        Console.WriteLine("component activated .....");
    }

    private void OnWindowDeactivated(object? sender, EventArgs e)
    {
        Console.WriteLine("component deactivated .....");
    }

    private void OnWindowMouseEnter(object? sender, PointerEventArgs e)
    {
        Console.WriteLine("component mouse entered .....");
    }

    private void OnWindowMouseLeave(object? sender, PointerEventArgs e)
    {
        Console.WriteLine("component mouse leave .....");
    }


    private void SetPage(string page)
    {
        CurrentPage = page;
        ClickCount++;
        _textBlock.Text = TextBlockStr;
    }

    private Button SidebarItem(string text, Action onClick)
    {
        return new Button()
            .Content(text)
            .Padding(10, 5)
            .OnClick(args => onClick());
    }
}