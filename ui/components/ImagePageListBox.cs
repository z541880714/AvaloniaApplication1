using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Markup.Declarative;
using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace AvaloniaApplication1.ui.components;

public class ImagePageListBox : ComponentBase
{
    private ImagePageVm _vm = new();

    protected override object Build()
    {
        return new ListBox()
            .ItemsSource(new Binding(nameof(ImagePageVm.PathList)) { Source = _vm })
            .ItemTemplate(new FuncDataTemplate<ImageData>(
                (imageData, ns) => new AsyncImageView()
                {
                    // 必须用 Binding(".")，这样 DataContext 切换时，SourceUri 才会自动变
                    [!AsyncImageView.SourceUriProperty] = new Binding(nameof(ImageData.Path))
                }.Height(200),
                // 2. 🔥🔥🔥 关键：开启复用 🔥🔥🔥
                supportsRecycling: true
            ));
    }
}