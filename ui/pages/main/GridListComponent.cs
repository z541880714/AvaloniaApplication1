using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Markup.Declarative;
using Avalonia.Media;
using Avalonia.Data;
using AvaloniaApplication1.vm; // 假设 TodoItem 在这里

namespace AvaloniaApplication1.ui.pages.main;

// 假设组件已绑定到 _vm (MainViewModel)
public class GridListComponent : ComponentBase
{
    private MainViewModel _vm = new();

    protected override object Build()
    {
        return new ListBox()
            .ItemsSource(new Binding(nameof(_vm.Items)) { Source = _vm })
            .Margin(10)
            .Padding(0)
            .ItemTemplate<TodoItem>(item => // item 依然是 TodoItem 对象引用
                new Border()
                    .Padding(15)
                    .Margin(0, 0, 0, 5)
                    .Background(Brushes.White)
                    .CornerRadius(5)
                    .BoxShadow(new BoxShadows(new BoxShadow { Blur = 3, Color = Color.Parse("#15000000") }))
                    .Child(
                        new Grid()
                            .Cols("*, Auto")
                            .Children(
                                // 🔥 修正 1：改为 Data Binding
                                new TextBlock()
                                    .Col(0)
                                    .Text(new Binding(nameof(item.Content))) // 👈 绑定到当前 DataContext 的 Content 属性
                                    .VerticalAlignment(VerticalAlignment.Center),

                                // 删除按钮
                                new Button()
                                    .Col(1)
                                    .Content("🗑️ 删除")
                                    .Background(Brushes.IndianRed)
                                    .Foreground(Brushes.White)
                                    .VerticalAlignment(VerticalAlignment.Center)
                                    // ✅ 修正 2：OnClick 内部依然安全，因为它只是捕获了 item 对象，没有读取 item 的属性
                                    .OnClick(_ => { _vm.RemoveItem(item); })
                            )
                    )
            );
    }
}