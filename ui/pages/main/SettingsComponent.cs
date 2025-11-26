using Avalonia.Controls;
using Avalonia.Markup.Declarative;

namespace AvaloniaApplication1.ui.pages.main;

public class SettingsComponent : ComponentBase
{
    protected override object Build() =>
        new StackPanel()
            .Spacing(10)
            .Children(
                new TextBlock().Text("设置").FontSize(24),
                new CheckBox().Content("开启暗黑模式"),
                new CheckBox().Content("自动更新")
            );
}