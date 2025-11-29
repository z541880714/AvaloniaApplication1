using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Markup.Declarative;
using Avalonia.Media;
using Avalonia.Media.Imaging;

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
                    Layout = new UniformGridLayout
                    {
                        MinItemWidth = 200,
                        MinItemHeight = 200,
                        ItemsStretch = UniformGridLayoutItemsStretch.Fill,
                        MaximumRowsOrColumns = 3,
                        MinColumnSpacing = 10,
                    },
                    // 🔥 必须写在大括号里面！
                    // 语法：[!类名.静态属性名] = Binding
                    [!ItemsRepeater.ItemsSourceProperty] = new Binding(nameof(ImagePageVm.PathList))
                        { Source = _vm },

                    ItemTemplate = new FuncDataTemplate<ImageData>((data, ns) =>
                        new AsyncImageView()
                            .BindSource(data)
                    )
                }
            );
    }
}