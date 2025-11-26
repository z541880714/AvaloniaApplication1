# 构建 Avalonia C#  声明式 UI 样板

> **"Code is UI, Logic is Native."**
>
> 一个基于 **.NET 8** + **Avalonia UI** 的高性能桌面应用模板。
> 摒弃繁琐的 XAML，采用类似 Jetpack Compose / Flutter 的 **纯 C\# 声明式 (Declarative)** 写法。
> 专为追求极致性能、低内存占用及跨平台（Windows / Linux / Raspberry Pi）场景设计。

-----

## 🏗 技术栈 (Tech Stack)

* **Runtime:** [.NET 8 SDK](https://dotnet.microsoft.com/download) (LTS)
* **UI Framework:** [Avalonia UI](https://avaloniaui.net/) (v11+)
* **DSL:** [Avalonia.Markup.Declarative](https://github.com/AvaloniaUI/Avalonia.Markup.Declarative) (C\# Markup)
* **IDE (Recommended):** JetBrains Rider

## ✨ 核心特性 (Features)

* **No XAML:** 彻底移除 `.axaml` 文件，使用纯 C\# 构建 UI 树，类型安全，重构方便。
* **High Performance:** 相比 Electron，拥有原生渲染性能和多线程共享内存能力。
* **Declarative Syntax:** 现代化的流式 API 写法，类似 `new StackPanel().Children(...)`。
* **Cross-Platform:** 一套代码，原生编译至 Windows (x64), Linux (x64/arm64), macOS。
* **Native AOT Ready:** 支持提前编译为无依赖的单文件，启动速度极快。

-----

## 🚀 快速开始 (Quick Start)

### 1\. 环境准备

确保已安装 **.NET 8 SDK**。

```bash
dotnet --version
# 应输出 8.0.xxx
```

### 2\. 安装此模板

在当前项目根目录下（即包含 `.template.config` 的目录），运行：

```bash
dotnet new install .
```

*(如果后续更新了代码，加上 `--force` 参数覆盖安装)*

### 3\. 创建新项目

在任意空目录下，使用以下命令生成新工程：

```bash
# -n 指定项目名称 (会自动替换源码中的 namespace)
dotnet new avac -n MyCoolApp
```

-----

## 💻 开发指南 (Development)

### 1\. 声明式 UI 写法

所有 UI 逻辑位于 `MainWindow.cs`。不再需要 `InitializeComponent()`。

**示例：**

```csharp
Content = new StackPanel()
    .VerticalAlignment(VerticalAlignment.Center)
    .Spacing(20)
    .Children(
        new TextBlock()
            .Text("Hello World")
            .HorizontalAlignment(HorizontalAlignment.Center),
            
        new Button()
            .Content("Click Me")
            .OnClick(OnBtnClick)
    );
```

### 2\. 状态与引用

使用 `.Ref()` 获取控件引用，类似 React/Vue 的 ref。

```csharp
private TextBlock _statusLabel;

// UI 构建时
new TextBlock().Ref(out _statusLabel).Text("Init")

// 逻辑中修改
_statusLabel.Text = "Updated!";
```

### 3\. 多线程与异步 (The Native Way)

**告别 Electron 的 IPC 通信。** 直接使用 `Task.Run` 在后台线程计算，通过 `Dispatcher` 更新 UI。

```csharp
await Task.Run(() => {
    // 1. 后台线程：执行重型计算 (不卡 UI)
    var result = HeavyCalculation();
    
    // 2. UI 线程：更新界面
    Dispatcher.UIThread.Post(() => {
        _statusLabel.Text = result;
    });
});
```

-----

## 📦 部署与发布 (Deployment)

### 发布为 Windows 单文件 (EXE)

```bash
dotnet publish -c Release -r win-x64 --self-contained -p:PublishSingleFile=true
```

### 发布为 Linux / 树莓派 (ARM64)

这是部署到树莓派 5 的标准命令。生成的文件无依赖，直接运行。

```bash
dotnet publish -c Release -r linux-arm64 --self-contained -p:PublishSingleFile=true
```

*生成的二进制文件位于 `bin/Release/net8.0/linux-arm64/publish/`*

### 开启 Native AOT (极致极简)

在 `.csproj` 中添加 `<PublishAot>true</PublishAot>`，然后运行上述发布命令。

* **注意：** AOT 模式下不支持部分动态反射功能，需测试兼容性。

-----

## 📂 项目结构 (Structure)

```text
MyProject/
├── .template.config/    # 模板配置文件 (用于 dotnet new)
├── App.cs               # 程序入口与生命周期 (加载 FluentTheme)
├── MainWindow.cs        # 主窗口 UI 与 业务逻辑
├── Program.cs           # Main 函数 (标准启动引导)
├── app.manifest         # Windows 高 DPI 适配文件
└── MyProject.csproj     # 项目配置与依赖包管理
```

-----

## ❓ 常见问题 (FAQ)

**Q: 为什么找不到 `.Center()` 方法？**
A: 新版库移除了部分非标准简写。请使用原生属性映射：

* `.Center()` -\> `.VerticalAlignment(VerticalAlignment.Center).HorizontalAlignment(HorizontalAlignment.Center)`
* `.HCenter()` -\> `.HorizontalAlignment(HorizontalAlignment.Center)`

**Q: 如何添加新的依赖包？**
A: 直接在 Rider 中 `Manage NuGet Packages` 或使用 `dotnet add package <Name>`。模板生成的项目会自动继承这些依赖。

-----


## 🛠️ CLI 常用命令 (Tooling Reference)

所有命令均在项目根目录下执行（即 `.csproj` 文件所在目录）。

| 命令 | 别名 | 作用 (Function) | 备注 |
| :--- | :--- | :--- | :--- |
| `dotnet run` | | 启动当前项目 | Rider 的运行按钮执行的就是这个命令。 |
| `dotnet build` | | 编译项目代码 | **检查语法**，生成 `.dll` 和 `.exe`。 |
| `dotnet clean` | | 清理项目 | 删除 `bin/` 和 `obj/` 文件夹，用于解决编译缓存问题。 |
| `dotnet restore` | | 恢复依赖包 | 从 NuGet 缓存中拉取所有依赖，对应 `requirements.txt` / `package.json` 的安装过程。 |
| `dotnet test` | | 运行单元测试 | 如果有测试项目，用于执行所有测试。 |
| **`dotnet add package`** | | **安装第三方库** | **相当于 `npm install` 或 `pip install`。** |
| `dotnet new` | | 创建项目/模板 | 用于生成新文件或安装模板。 |

---

### 📦 依赖管理 (Package Management)

| 场景 | 命令示例 | 效果 |
| :--- | :--- | :--- |
| **安装新库** | `dotnet add package SkiaSharp` | 添加 SkiaSharp 库到当前 `.csproj`。 |
| **安装特定版本** | `dotnet add package SkiaSharp -v 2.88.9` | 添加指定版本。 |
| **移除库** | `dotnet remove package SkiaSharp` | 从 `.csproj` 中删除依赖。 |

---

### 🚀 部署命令 (Deployment Examples)

这些是您用于 **Raspberry Pi** 部署的关键命令：

| 目标 | 命令 (Command) | 产物 |
| :--- | :--- | :--- |
| **Linux (Pi)** | `dotnet publish -c Release -r linux-arm64 --self-contained` | 无依赖的 Linux ARM64 可执行文件。 |
| **Linux (AOT)** | `dotnet publish -c Release -r linux-arm64 -p:PublishAot=true` | **极小体积**的原生二进制文件。 |
| **Windows (Single File)** | `dotnet publish -c Release -r win-x64 -p:PublishSingleFile=true` | 单一 Windows EXE 文件（便于分发）。 |

---

这个清单应该能够涵盖您 99% 的开发和部署需求了！