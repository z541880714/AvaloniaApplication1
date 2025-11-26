using Avalonia.Controls;
using Avalonia.Markup.Declarative;
using Avalonia.Layout;

namespace AvaloniaApplication1.ui.pages.main;

public class HomeComponent : ComponentBase
{
    protected override object Build() =>
        new StackPanel()
            .HorizontalAlignment(HorizontalAlignment.Center)
            .VerticalAlignment(VerticalAlignment.Center)
            .Children(
                new TextBlock().Text("欢迎来到首页").FontSize(30)
            );
}