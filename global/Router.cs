using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using AvaloniaApplication1.ui.components;
using AvaloniaApplication1.ui.pages.main;

namespace AvaloniaApplication1.global;

public class Router
{
    public static List<NavNode> RouterConfig =
    [
        new("1-组件样例", children:
        [
            new("主页", action: () => new GridListComponent(), isMain: true),
            new("listBox", action: () => new ImagePageListBox()),
            new("listRepeater", action: () => new ImagePageRepeater()),
        ]),

        new("2-视觉渲染", children:
        [
            new("2-1", action: () => new HomeComponent("2-1")),
            new("2-2", action: () => new HomeComponent("2-2")),
            new("2-3", action: () => new HomeComponent("2-3")),
        ]),
    ];

    public static Control FindMain()
    {
        List<NavNode> nodeList = new();
        foreach (var parentNode in RouterConfig)
        {
            if (parentNode.Children.Count == 0)
            {
                nodeList.Add(parentNode);
            }
            else
            {
                nodeList.AddRange(parentNode.Children);
            }
        }

        var mainNode = nodeList.FirstOrDefault(it => it?.IsMain ?? false, null);
        if (mainNode == null || mainNode.Action == null)
        {
            return new GridListComponent();
        }

        return mainNode.Action();
    }
}