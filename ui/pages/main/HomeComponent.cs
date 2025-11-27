using Avalonia.Controls;
using Avalonia.Markup.Declarative;
using Avalonia.Layout;

namespace AvaloniaApplication1.ui.pages.main;

public class HomeComponent(string? content = null) : ComponentBase
{
    private readonly string _content = content ?? "欢迎来到首页";

    protected override object Build() =>
        new StackPanel()
            .HorizontalAlignment(HorizontalAlignment.Center)
            .VerticalAlignment(VerticalAlignment.Center)
            .Children(
                new TextBlock().Text(_content).FontSize(30)
            );
}