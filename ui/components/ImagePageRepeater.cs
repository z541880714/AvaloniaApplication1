using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Markup.Declarative;
using Avalonia.Media;

namespace AvaloniaApplication1.ui.components;

public class ImagePageRepeater : ComponentBase
{
    private ImagePageVm _vm = new();

    protected override object Build()
    {
        // 使用 ScrollViewer 包裹，因为 ItemsRepeater 自己不带滚动条
        return new ScrollViewer()
            .Content(
                new ItemsRepeater
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Layout = new UniformGridLayout
                    {
                        MaximumRowsOrColumns = 3,

                        // 3. 坚决使用 None，保证每个格子宽度雷打不动
                        ItemsStretch = UniformGridLayoutItemsStretch.Fill,

                        MinColumnSpacing = 20,
                        MinRowSpacing = 20,
                    },
                    // 🔥 必须写在大括号里面！
                    // 语法：[!类名.静态属性名] = Binding
                    [!ItemsRepeater.ItemsSourceProperty] = new Binding(nameof(ImagePageVm.PathList))
                        { Source = _vm },

                    ItemTemplate = new FuncDataTemplate<ImageData>((data, ns) =>
                        new Grid()
                            .Rows("200, 60")
                            .Children(
                                new AsyncImageView()
                                    .Row(0)
                                    .BindSource(data),
                                new TextBlock()
                                    .Row(1)
                                    .Text(new Binding(nameof(ImageData.Name)))
                                    .HorizontalAlignment(HorizontalAlignment.Center)
                            )
                    )
                }
            );
    }
}