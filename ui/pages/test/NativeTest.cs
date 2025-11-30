using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Markup.Declarative;

namespace AvaloniaApplication1.ui.pages.test;

public class NativeTest() : ComponentBase
{
    private TestVm _vm = new();

    protected override object Build() =>
        new StackPanel()
            .HorizontalAlignment(HorizontalAlignment.Center)
            .VerticalAlignment(VerticalAlignment.Center)
            .Children(
                new Button()
                    .Content("点击测试nativeMethod")
                    .OnClick((it) => { _vm.TestNativeMethod(); })
            );
}