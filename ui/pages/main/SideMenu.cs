using System;
using System.Linq;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Markup.Declarative;
using Avalonia.Media;
using global::AvaloniaApplication1.global;

namespace AvaloniaApplication1.ui.pages.main;

public class SideMenu : ComponentBase
{
    private SideMenuVm _vm = new();

    protected override void OnInitialized()
    {
        base.OnInitialized();
        DataContext = _vm;
    }

    protected override object Build()
    {
        return new ScrollViewer()
            .Content(
                new StackPanel()
                    .Spacing(10)
                    .Orientation(Orientation.Vertical)
                    .Children(
                        Router.RouterConfig.Select(CreateGroupUi).ToArray()
                    )
            );
    }

    private Control CreateGroupUi(NavNode parentNode)
    {
        // 定义 Grid 引用，方便在事件里修改它
        Grid? headerGrid = null;

        // 图标路径
        var chevronPath = StreamGeometry.Parse("M7.41,8.58L12,13.17L16.59,8.58L18,10L12,16L6,10L7.41,8.58Z");
        return new StackPanel()
            .Children(
                new Grid()
                    .Ref(out headerGrid)!
                    .Cols("*, Auto")
                    .Background(Brushes.Transparent)
                    .OnPointerEntered((it) =>
                    {
                        if (headerGrid != null)
                            headerGrid.Background = Brushes.Cornsilk;
                        if (it.Source is InputElement el) el.Cursor = new Cursor(StandardCursorType.Hand);
                    })
                    .OnPointerExited(it =>
                    {
                        if (headerGrid != null)
                            headerGrid.Background = Brushes.Transparent;
                    })
                    .OnPointerPressed(e =>
                    {
                        // 逻辑判断：有子项才允许切换状态
                        if (parentNode.HasStretchIcon)
                            parentNode.StretchState = !parentNode.StretchState;
                        Console.WriteLine(
                            $"menu header  PointerPressed, name:{parentNode.Name}, " +
                            $"HasStretchIcon:{parentNode.HasStretchIcon}" +
                            $"StretchState: {parentNode.StretchState}");
                    })
                    .Children(
                        new Border()
                            .Col(0)
                            .Padding(horizontal: 10)
                            .Child(new TextBlock()
                                .Text(parentNode.Name)
                                .FontSize(16)
                                .FontWeight(FontWeight.Bold)
                                .Margin(5, 10, 0, 5)
                            ), // 上边距大一点，区分不同组
                        new Border()
                            .Col(1)
                            .Padding(horizontal: 10)
                            .Child(new PathIcon()
                                .Data(chevronPath)
                                .Width(16).Height(16)
                                .Foreground(Brushes.Gray)
                                .VerticalAlignment(VerticalAlignment.Center)
                                .IsVisible(parentNode.HasStretchIcon)
                                .RenderTransformOrigin(new RelativePoint(0.5, 0.5, RelativeUnit.Relative))
                                // 2. 绑定旋转：监听 StretchState 变化
                                .RenderTransform(
                                    new RotateTransform()
                                        {
                                            [!RotateTransform.AngleProperty] =
                                                new Binding(nameof(NavNode.StretchState))
                                                {
                                                    Source = parentNode,
                                                    Converter = new FuncValueConverter<bool, double>(expanded =>
                                                        expanded ? 180 : 0)
                                                }
                                        }
                                        // A. 定义动画规则：监听 Angle 属性的变化
                                        .Transitions(new Transitions
                                        {
                                            new DoubleTransition
                                            {
                                                Property = RotateTransform.AngleProperty, // 只要 Angle 变了，动画就开始
                                                Duration = TimeSpan.FromMilliseconds(200), // 耗时 0.2 秒
                                                Easing = new CubicEaseOut() // (可选) 缓动效果：一开始快，快结束时慢
                                            }
                                        })
                                ))
                    ),
                // B. 子节点渲染为：缩进的按钮列表
                new StackPanel()
                    .Margin(20, 0, 0, 0) // 🔥 关键：左侧缩进 20px，体现层级
                    .IsVisible(new Binding(nameof(NavNode.StretchState)) { Source = parentNode })
                    .Children(
                        parentNode.Children.Select(childNode =>
                            new Button()
                                .Content(childNode.Name)
                                .HorizontalAlignment(HorizontalAlignment.Stretch) // 让按钮横向填满
                                .HorizontalContentAlignment(HorizontalAlignment.Left) // 文字靠左
                                .Background(new Binding(nameof(SideMenuVm.SelectItemId))
                                    {
                                        Converter = new FuncValueConverter<string, IBrush>(selectId =>
                                            childNode.Id == selectId ? Brushes.DarkCyan : Brushes.Transparent),
                                    }
                                ) // 样式微调：透明背景
                                .OnClick(_ =>
                                {
                                    // 🔥 点击触发动作
                                    if (childNode.Action == null) return;
                                    _vm.SelectItemId = childNode.Id; // 被选中的,  背景色改变
                                    ZNavigator.Instance.SwitchTo(childNode.Action, childNode.Id);
                                })
                        ).ToArray<Control>()
                    )
            );
    }
}