# Run WinForms Apps on Linux and macOS Using GTK

## Introduction

What if you could run your existing Windows Forms applications on Linux and macOS **without rewriting your UI**?

Traditionally, Windows Forms applications are tightly coupled to Windows. Porting them to other platforms often means rebuilding the UI using a completely different framework. This is time-consuming, error-prone, and expensive.

**Gtk.Windows.Forms** offers a different approach: it allows developers to reuse their existing WinForms knowledge and codebase while targeting multiple operating systems.

---

## What is Gtk.Windows.Forms?

Gtk.Windows.Forms is a cross-platform framework that enables Windows Forms applications to run on:

- Windows
- Linux
- macOS

It works by implementing the Windows Forms API on top of GTK via GtkSharp, allowing developers to:

- Use Visual Studio and the WinForms designer
- Keep familiar APIs (controls, events, properties)
- Compile once and run across platforms

---

## Key Benefits

### 1. Reduced Development Time
Write your UI once and reuse it across platforms.

### 2. Lower Maintenance Costs
Fix bugs and ship updates in a single codebase.

### 3. Familiar Development Model
No need to learn a new UI framework—keep using WinForms patterns.

### 4. Cross-Platform Reach
Deploy your application to Windows, Linux, and macOS users.

---

## Quick Demo: Creating a Cross-Platform WinForms App

### 1. Install the .NET SDK

```bash
dotnet --version
```

Download if needed from:
https://dotnet.microsoft.com/en-us/download

---

### 2. Create a WinForms Project

```bash
dotnet new winforms -n MyWinFormsApp
cd MyWinFormsApp
```

---

### 3. Open in Visual Studio

```bash
devenv MyWinFormsApp.csproj
```

---

### 4. Add Gtk.Windows.Forms

Edit your `.csproj`:

```xml
<ItemGroup>
  <PackageReference Include="Gtk.Windows.Forms" Version="1.3.24.26085" />
</ItemGroup>
```

---

### 5. Make It Cross-Platform

Modify:

```xml
<TargetFramework>net10.0</TargetFramework>
```

Remove:

```xml
<UseWindowsForms>true</UseWindowsForms>
```

---

### 6. Restore Packages

```bash
dotnet restore
```

---

### 7. Run the Application

```bash
dotnet run
```

Your WinForms application can now run across platforms.

---

## How It Works (Simplified)

Gtk.Windows.Forms acts as a compatibility layer:

- WinForms controls are mapped to GTK widgets
- Events are translated from GTK signals to WinForms delegates
- Layout logic mimics WinForms behavior
- Rendering is adapted using GTK drawing APIs

Example:

- `RichTextBox` → backed by `Gtk.TextView`
- Layout containers → mapped to GTK containers

This allows existing applications to run with minimal changes.

---

## When Should You Use It?

Use Gtk.Windows.Forms if:

- You have an existing WinForms application to port
- You want cross-platform support without rewriting UI
- You prefer the WinForms development model

---

## Limitations

- Not all controls are fully implemented
- Some platform-specific behaviors may differ
- Performance depends on GTK integration

---

## Project Background

The project originated from EasyWebFactory and was later forked and extended.

Enhancements include:

- Translation from Chinese to English
- Multi-language support (EN, FR, DE, IT, ES, CN)
- Code refactoring and cleanup
- Introduction of unit testing for stability

---

## Conclusion

Gtk.Windows.Forms provides a practical solution for bringing WinForms applications to modern, cross-platform environments.

Instead of rewriting your UI, you can:

- Reuse your existing code
- Keep your development workflow
- Expand your application's reach

As the project evolves, it continues to improve in stability, coverage, and usability—making it a compelling option for developers looking to modernize legacy applications.

---

## Resources

- Project repository:
  https://bitbucket.org/piero-viano/Net4x.Gtk.Windows.Forms

- Tutorial repository:
  https://bitbucket.org/piero-viano/net4x.gtk.windows.forms.tutorial
