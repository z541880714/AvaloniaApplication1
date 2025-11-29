using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Markup.Declarative;
using Avalonia.Media;
using Avalonia.Data;

namespace AvaloniaApplication1.ui.pages.main;

public class GridListComponent : ComponentBase
{
    private GridListComponentVm _vm = new();

    protected override object Build()
    {
        return new ListBox()
            .Margin(10)
            .ItemsSource(new Binding(nameof(_vm.Items)) { Source = _vm })
            .ItemTemplate<TodoItem>(item => // item 依然是 TodoItem 对象引用
                new Border()
                    .Padding(15)
                    .Margin(0, 0, 0, 5)
                    .Background(Brushes.White)
                    .CornerRadius(5)
                    .BoxShadow(new BoxShadows(new BoxShadow { Blur = 3, Color = Color.Parse("#15000000") }))
                    .Child(
                        new Grid
                            {
                                ColumnSpacing = 10
                            }
                            .Cols("*, Auto, Auto")
                            .Children(
                                // 🔥 修正 1：改为 Data Binding
                                new TextBlock()
                                    .Col(0)
                                    .Text(new Binding(nameof(item.Content))) // 👈 绑定到当前 DataContext 的 Content 属性
                                    .VerticalAlignment(VerticalAlignment.Center),
                                new Button()
                                    .Col(1)
                                    .Content("更新")
                                    .HorizontalContentAlignment(HorizontalAlignment.Center)
                                    .Width(100)
                                    .Background(Brushes.DarkCyan)
                                    .Foreground(Brushes.White)
                                    .VerticalAlignment(VerticalAlignment.Center)
                                    .OnClick(_ => { _vm.UpdateItem(item); }),
                                // 删除按钮
                                new Button()
                                    .Col(2)
                                    .Content("🗑️ 删除")
                                    .HorizontalContentAlignment(HorizontalAlignment.Center)
                                    .Width(100)
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