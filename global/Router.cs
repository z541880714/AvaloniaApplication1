using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using AvaloniaApplication1.ui.components;
using AvaloniaApplication1.ui.pages.main;
using AvaloniaApplication1.ui.pages.test;

namespace AvaloniaApplication1.global;

public class Router
{
    public static List<NavNode> RouterConfig =
    [
        new("1-组件样例", children:
        [
            new("主页", action: () => new GridListComponent()),
            new("listBox", action: () => new ImagePageListBox()),
            new("listRepeater", action: () => new ImagePageRepeater()),
        ]),

        new("2-视觉渲染", children:
        [
            new("native_test", action: () => new NativeTest(), isMain: true),
            new("2-2", action: () => new HomeComponent("2-1")),
            new("2-3", action: () => new HomeComponent("2-2")),
            new("2-4", action: () => new HomeComponent("2-3")),
        ]),
    ];

    private static NavNode? FindMainNode(NavNode node)
    {
        Console.WriteLine($"name:{node.Name}, isMain:{node.IsMain}, action is null:{node.Action == null}");
        if (node is { IsMain: true, Action: not null }) return node;
        foreach (var child in node.Children)
        {
            var mainNode = FindMainNode(child);
            if (mainNode is { Action: not null })
            {
                return mainNode;
            }
        }

        return null;
    }

    public static string FindMainNodeId()
    {
        foreach (var parentNode in RouterConfig)
        {
            var mainNode = FindMainNode(parentNode);
            if (mainNode is { Action: not null })
            {
                parentNode.StretchState = true;
                return mainNode.Id;
            }
        }

        throw new Exception("must has a  main  node");
    }

    public static Control FindMainControl()
    {
        foreach (var parentNode in RouterConfig)
        {
            var mainNode = FindMainNode(parentNode);
            if (mainNode is { Action: not null })
            {
                parentNode.StretchState = true;
                return mainNode.Action();
            }
        }

        throw new Exception("must has a  main  node");
    }
}