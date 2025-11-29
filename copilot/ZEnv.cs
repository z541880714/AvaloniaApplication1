using System;
using System.IO;

namespace AvaloniaApplication1.copilot;

public class ZEnv
{
    public static string ExeDir = AppContext.BaseDirectory;
    public static DirectoryInfo ProjectRoot => Directory.GetParent(ExeDir)?.Parent?.Parent?.Parent!;

    public static string AssetsDir => Path.Combine(ExeDir, "Assets");
}