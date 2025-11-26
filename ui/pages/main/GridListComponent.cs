using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Markup.Declarative;
using Avalonia.Media;
using AvaloniaApplication1.viewmodel;

namespace AvaloniaApplication1.ui.pages.main;

public class GridListComponent : ComponentBase
{
    private MainViewModel _vm = new();

    private delegate TControl FuncTemplate1<TControl>(TControl control);

    protected override object Build()
    {
        return new ScrollViewer()
            .Content(
                new ItemsControl()
                    // 绑定数据源
                    .ItemsSource(new Binding(nameof(_vm.Items)) { Source = _vm })
                    // 🔥🔥🔥【核心核心核心】🔥🔥🔥
                    // 修改 ItemsPanel 为 UniformGrid，并强制设为 2 列
                    .ItemsPanel(new FuncTemplate<Panel>(() => new UniformGrid().Columns(2)))
                    .ItemTemplate<string, ItemsControl>(item =>
                        new Border()
                            .Margin(5) // 格子之间的间距
                            .Padding(20)
                            .CornerRadius(8)
                            .Background(Brushes.White)
                            .BoxShadow(new BoxShadows(new BoxShadow { Blur = 5, Color = Color.Parse("#33000000") }))
                            .Child(
                                new StackPanel()
                                    .Children(
                                        new TextBlock().Text("📦").FontSize(24)
                                            .HorizontalAlignment(HorizontalAlignment.Center),
                                        new TextBlock().Text(item).HorizontalAlignment(HorizontalAlignment.Center)
                                    )
                            )
                    )
            );
    }
}