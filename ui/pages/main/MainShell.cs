using System;
using AvaloniaApplication1.vm;

namespace AvaloniaApplication1.ui.pages.main;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Markup.Declarative;
using Avalonia.Media;
using Avalonia.Data;
using Avalonia.Animation; // 用于动画

public class MainShell : ComponentBase
{
    public MainShell()
    {
        // 默认显示首页
        NavigationStore.Instance.SwitchTo(() => new HomeComponent());
    }

    protected override object Build()
    {
        return new Grid()
            .Cols("200, *") // 左侧菜单固定 200，右侧内容自适应
            .Children(
                // === 左侧侧边栏 (Sidebar) ===
                new Border()
                    .Col(0)
                    .Background(Brushes.DarkSlateGray)
                    .Padding(10)
                    .Child(
                        new StackPanel()
                            .Spacing(5)
                            .Children(
                                // 标题
                                new TextBlock()
                                    .Text("我的 App")
                                    .FontSize(24)
                                    .Foreground(Brushes.White)
                                    .HorizontalAlignment(HorizontalAlignment.Center)
                                    .Margin(0, 0, 0, 20),

                                // 菜单按钮 (使用 SwitchTo)
                                MenuButton("🏠 首页", () => NavigationStore.Instance.SwitchTo(() => new HomeComponent())),
                                MenuButton("📋 列表",
                                    () => NavigationStore.Instance.SwitchTo(() => new GridListComponent())),
                                MenuButton("⚙️ 设置",
                                    () => NavigationStore.Instance.SwitchTo(() => new SettingsComponent()))
                            )
                    ),

                // === 右侧内容区 (Content) ===
                new TransitioningContentControl()
                    .IsTransitionReversed(true)
                    .Col(1)
                    .Padding(20)
                    .Background(Brushes.White) // 内容区背景

                    // 🔥 绑定全局导航存储的 CurrentView
                    .Content(new Binding(nameof(NavigationStore.Instance.CurrentView))
                    {
                        Source = NavigationStore.Instance
                    })

                    // ✨ 加点动画：淡入淡出 (CrossFade)
                    .PageTransition(new CrossFade(TimeSpan.FromMilliseconds(300)))

                // 方案 B: 左右滑动 (现代，动感) - 强烈推荐试试这个！
                // 参数1: 动画时长
                // 参数2: 滑动方向 (Horizontal / Vertical)
                // .PageTransition(new PageSlide(TimeSpan.FromMilliseconds(400), PageSlide.SlideAxis.Horizontal))
            );
    }

    // 封装一个简单的菜单按钮样式
    private Control MenuButton(string text, Action onClick)
    {
        return new Button()
            .Content(text)
            .HorizontalAlignment(HorizontalAlignment.Stretch)
            .Background(Brushes.Transparent)
            .Foreground(Brushes.White)
            .FontSize(16)
            .HorizontalContentAlignment(HorizontalAlignment.Left) // 文字靠左
            .Padding(15, 10)
            .CornerRadius(5)
            // 鼠标悬停变色逻辑 (利用之前学的 Styles 也可以，这里为了演示简单直接写)
            .OnPointerEntered(e =>
            {
                if (e.Source is Button b) b.Background = Brushes.Teal;
            })
            .OnPointerExited(e =>
            {
                if (e.Source is Button b) b.Background = Brushes.Transparent;
            })
            .OnClick(_ => onClick());
    }
}