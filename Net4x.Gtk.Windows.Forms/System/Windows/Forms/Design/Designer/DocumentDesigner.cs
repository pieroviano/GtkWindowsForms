// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing.Design;
using System.Reflection;

namespace System.Windows.Forms.Design;

/// <summary>
/// Provides a designer that extends the ScrollableControlDesigner and implements
/// IRootDesigner.
/// </summary>
[ToolboxItemFilter("System.Windows.Forms")]
public partial class DocumentDesigner : ScrollableControlDesigner, IRootDesigner, IToolboxUser, IOleDragClient
{
    internal static IDesignerSerializationManager? manager;

    private const string AxClipFormat = "CLSID";

    // ActiveX support
    //
    private static readonly Guid htmlDesignTime = new("73CEF3DD-AE85-11CF-A406-00AA00C00940");

    private readonly bool initializing;
    private readonly bool queriedTabOrder;
    private readonly MenuCommand? tabOrderCommand;
    private readonly bool trayAutoArrange;
    private readonly int trayHeight = 80;
    private readonly bool trayLargeIcon;
    private readonly bool trayLayoutSuspended;

    /// <summary>
    /// Gets the set of technologies that this designer can support for its display.
    /// </summary>
    /// <value>
    /// The supported technologies.
    /// </value>
    public ViewTechnology[] SupportedTechnologies
    {
        get
        {
            // WinForms designers typically support only Default view
            return new[] { ViewTechnology.Default };
        }
    }

    /// <summary>
    /// Gets a value indicating whether the specified tool is supported by the current designer.
    /// </summary>
    /// <param name="tool">
    /// The
    /// <see cref="T:System.Drawing.Design.ToolboxItem" /> to be tested for toolbox support.
    /// </param>
    /// <returns>
    /// true if the tool is supported by the toolbox and can be enabled; false if the document designer does not know how to use the tool.
    /// </returns>
    public bool GetToolSupported(ToolboxItem tool)
    {
        if (tool is null)
        {
            return false;
        }

        try
        {
            // Some ToolboxItem implementations expose a GetType(Type) method that returns the component type
            // (different from the parameterless Object.GetType()). Try to call it via reflection if present.
            Type? componentType = null;

            var getTypeMethod = tool.GetType().GetMethod("GetType", new[] { typeof(Type) });
            if (getTypeMethod != null)
            {
                var result = getTypeMethod.Invoke(tool, new object[] { typeof(Control) });
                componentType = result as Type;
            }

            // If that didn't work, try common properties that may hold the represented type.
            if (componentType == null)
            {
                var prop = tool.GetType().GetProperty("ToolType") ??
                           tool.GetType().GetProperty("WrappedType") ??
                           tool.GetType().GetProperty("Type");
                if (prop != null)
                {
                    componentType = prop.GetValue(tool) as Type;
                }
            }

            // Fallback to runtime type of the toolbox item (best-effort).
            if (componentType == null)
            {
                componentType = tool.GetType();
            }

            return componentType is not null && typeof(Control).IsAssignableFrom(componentType);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Gets a view object for the specified view technology.
    /// </summary>
    /// <param name="technology">
    /// A
    /// <see cref="T:System.ComponentModel.Design.ViewTechnology">
    /// </see> that indicates a particular view technology.
    /// </param>
    /// <returns>
    /// An object that represents the view for this designer.
    /// </returns>
    public object GetView(ViewTechnology technology)
    {
        if (technology != ViewTechnology.Default)
        {
            throw new ArgumentException($"Unsupported view technology: {technology}", nameof(technology));
        }

        if (Component is Control control)
        {
            return control;
        }

        throw new NotSupportedException("The root component is not a Control.");
    }

    /// <summary>
    /// Selects the specified tool.
    /// </summary>
    /// <param name="tool">
    /// The
    /// <see cref="T:System.Drawing.Design.ToolboxItem" /> to select.
    /// </param>
    public void ToolPicked(ToolboxItem tool)
    {
        if (tool is null)
        {
            return;
        }

        // Try to obtain a designer host from the root component's site as a fallback
        var host = (this.Component as IComponent)?.Site?.GetService(typeof(IDesignerHost)) as IDesignerHost;
        if (host is null)
        {
            return;
        }

        using DesignerTransaction transaction = host.CreateTransaction("ToolPicked");

        try
        {
            object[]? components = null;

            // ToolboxItem implementations differ across frameworks; attempt to invoke CreateComponents via reflection.
            var createMethod = tool.GetType().GetMethod("CreateComponents", new[] { typeof(IDesignerHost) })
                            ?? tool.GetType().GetMethod("CreateComponents", Type.EmptyTypes);

            if (createMethod != null)
            {
                try
                {
                    var parameters = createMethod.GetParameters();
                    object? invokeResult = createMethod.Invoke(tool, parameters.Length == 1 ? new object?[] { host } : null);
                    components = invokeResult as object[];
                }
                catch (TargetInvocationException)
                {
                    // Invocation failed. Fall through to fallback logic below.
                    components = null;
                }
            }

            // Fallback: if no CreateComponents method or invocation failed, try to create component using a ToolType/Type property.
            if ((components == null || components.Length == 0) && host != null)
            {
                Type? toolType = null;
                var prop = tool.GetType().GetProperty("ToolType") ??
                           tool.GetType().GetProperty("WrappedType") ??
                           tool.GetType().GetProperty("Type");
                if (prop != null)
                {
                    toolType = prop.GetValue(tool) as Type;
                }

                // If still not found, try a well-known field/backing store name.
                if (toolType == null)
                {
                    var field = tool.GetType().GetField("_toolType", BindingFlags.Instance | BindingFlags.NonPublic)
                                ?? tool.GetType().GetField("toolType", BindingFlags.Instance | BindingFlags.NonPublic);
                    if (field != null)
                    {
                        toolType = field.GetValue(tool) as Type;
                    }
                }

                if (toolType != null)
                {
                    var created = host.CreateComponent(toolType);
                    if (created != null)
                    {
                        components = new object[] { created };
                    }
                }
            }

            if (components is { Length: > 0 } &&
                components[0] is Control control &&
                host.RootComponent is Control parent)
            {
                if (parent is Form form)
                {
                    form.Controls.Add(control);
                }
                else
                {
                    parent.Controls?.Add(control);
                }

                // Basic placement logic (simple default)
                control.Location = new System.Drawing.Point(10, 10);
            }

            transaction.Commit();
        }
        catch
        {
            transaction.Cancel();
            throw;
        }
    }
}