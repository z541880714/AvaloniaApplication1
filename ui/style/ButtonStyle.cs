using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Markup.Declarative;
using Avalonia.Media;

namespace AvaloniaApplication1.ui.style;

public static class ButtonStyle
{
    public static T ListItemButtonStyle<T>(this T button, IBrush? background = null) where T : Button
    {
        return button
            .Width(50)
            .Height(20)
            .Padding(10, 0) // 对应 Padding(horizontal: 10) -> Left/Right 10, Top/Bottom 0
            .Background(background ?? Brushes.CadetBlue)
            .HorizontalAlignment(HorizontalAlignment.Center)
            .HorizontalContentAlignment(HorizontalAlignment.Center);
    }
}