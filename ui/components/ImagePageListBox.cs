using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Layout;
using Avalonia.Markup.Declarative;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using AvaloniaApplication1.copilot;

namespace AvaloniaApplication1.ui.components;

public class ImagePageListBox : ComponentBase
{
    private readonly ImagePageVm _vm = new();

    private readonly ZLruCache<string, Bitmap> _lruCache = new(200, it => it.Dispose());
    // private readonly ZLruCache<string, Bitmap> _lruCache = new(500);

    protected override object Build()
    {
        return new ListBox()
            .ItemsSource(new Binding(nameof(ImagePageVm.PathList)) { Source = _vm })
            .ItemTemplate(new FuncDataTemplate<ImageData>((imageData, ns) =>
                new Grid()
                    {
                        ColumnSpacing = 20,
                    }
                    .Cols("100, *")
                    .Height(100)
                    .VerticalAlignment(VerticalAlignment.Center)
                    .Children(
                        new Image()
                        {
                            // 必须用 Binding(".")，这样 DataContext 切换时，SourceUri 才会自动变
                            [!Image.SourceProperty] = new Binding(nameof(ImageData.Path))
                            {
                                Source = imageData,
                                Converter = new FuncValueConverter<string, Bitmap>(it =>
                                {
                                    return _lruCache.GetOrAdd(it, ImageHelper.BitmapFactory);
                                })
                            },
                        }.Col(0),
                        new TextBlock()
                            .Col(1)
                            .Text(new Binding(nameof(ImageData.Path)))
                            .VerticalAlignment(VerticalAlignment.Center)
                    )
            ));
    }
}