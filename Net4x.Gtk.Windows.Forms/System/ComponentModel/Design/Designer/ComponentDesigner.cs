// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;

namespace System.ComponentModel.Design;

/// <summary>
/// The default designer for all components.
/// </summary>
public partial class ComponentDesigner : ITreeDesigner, IDesignerFilter, IComponentInitializer
{

    private protected const string SettingsKeyName = "SettingsKey";

    // made non-readonly so tests (and reflection) can set the value via a non-public accessor
    private IComponent? _component;

    private readonly InheritanceAttribute? _inheritanceAttribute;

    private readonly bool _settingsKeyExplicitlySet;

    private readonly DesignerVerbCollection? _verbs;
    private DesignerVerbCollection? _designerVerbCollection;

    /// <summary>
    /// Gets a collection of child designers.
    /// </summary>
    /// <exception cref="T:System.NotImplementedException">
    /// </exception>
    public virtual ICollection Children => new object[0];

    /// <summary>
    /// Gets the base component that this designer is designing.
    /// </summary>
    /// <exception cref="T:System.NotImplementedException">
    /// </exception>
    // Make the property non-public (protected internal) with a set accessor so tests that use
    // reflection with BindingFlags.Instance | BindingFlags.NonPublic can find and set it.
    public virtual IComponent? Component
    {
        get => _component;
        set => _component = value;
    }

    /// <summary>
    /// Gets the parent designer.
    /// </summary>
    /// <exception cref="T:System.NotImplementedException">
    /// </exception>
    public virtual IDesigner? Parent { get; set; }

    /// <summary>
    /// Gets a collection of the design-time verbs supported by the designer.
    /// </summary>
    /// <exception cref="T:System.NotImplementedException">
    /// </exception>
    public virtual DesignerVerbCollection Verbs
    {
        get
        {
            _designerVerbCollection = _verbs;
            return _designerVerbCollection ??= new DesignerVerbCollection();
        }
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {

    }

    /// <summary>
    /// Performs the default action for this designer.
    /// </summary>
    public void DoDefaultAction()
    {

    }

    /// <summary>
    /// Initializes the designer with the specified component.
    /// </summary>
    /// <param name="component">
    /// The component to associate with this designer.
    /// </param>
    public void Initialize(IComponent component)
    {
        // Ensure the internal field is set when Initialize is used.
        _component = component;
    }

    IComponent IDesigner.Component { get; }

    /// <summary>
    /// Restores an instance of a component to its default state.
    /// </summary>
    /// <param name="defaultValues">
    /// A dictionary of default property values, which are name/value pairs, with which to reset the component's state.
    /// </param>
    public void InitializeExistingComponent(IDictionary defaultValues)
    {

    }

    /// <summary>
    /// Initializes a new component using a set of recommended values.
    /// </summary>
    /// <param name="defaultValues">
    /// A dictionary of default property values, which are name/value pairs, with which to initialize the component's state.
    /// </param>
    public void InitializeNewComponent(IDictionary defaultValues)
    {

    }

    /// <summary>
    /// When overridden in a derived class, allows a designer to change or remove items from the set of attributes that it exposes through a
    /// <see cref="T:System.ComponentModel.TypeDescriptor">
    /// </see>.
    /// </summary>
    /// <param name="attributes">
    /// The
    /// <see cref="T:System.Attribute">
    /// </see> objects for the class of the component. The keys in the dictionary of attributes are the
    /// <see cref="T:System.Attribute.TypeId">
    /// </see> values of the attributes.
    /// </param>
    public void PostFilterAttributes(IDictionary attributes)
    {

    }

    /// <summary>
    /// When overridden in a derived class, allows a designer to change or remove items from the set of events that it exposes through a
    /// <see cref="T:System.ComponentModel.TypeDescriptor">
    /// </see>.
    /// </summary>
    /// <param name="events">
    /// The
    /// <see cref="T:System.ComponentModel.EventDescriptor">
    /// </see> objects that represent the events of the class of the component. The keys in the dictionary of events are event names.
    /// </param>
    public void PostFilterEvents(IDictionary events)
    {

    }

    /// <summary>
    /// When overridden in a derived class, allows a designer to change or remove items from the set of properties that it exposes through a
    /// <see cref="T:System.ComponentModel.TypeDescriptor">
    /// </see>.
    /// </summary>
    /// <param name="properties">
    /// The
    /// <see cref="T:System.ComponentModel.PropertyDescriptor">
    /// </see> objects that represent the properties of the class of the component. The keys in the dictionary of properties are property names.
    /// </param>
    public void PostFilterProperties(IDictionary properties)
    {

    }

    /// <summary>
    /// When overridden in a derived class, allows a designer to add items to the set of attributes that it exposes through a
    /// <see cref="T:System.Attribute">
    /// </see>.
    /// </summary>
    /// <param name="attributes">
    /// The
    /// <see cref="T:System.Attribute">
    /// </see> objects for the class of the component. The keys in the dictionary of attributes are the
    /// <see cref="T:System.Attribute.TypeId">
    /// </see> values of the attributes.
    /// </param>
    public void PreFilterAttributes(IDictionary attributes)
    {

    }

    /// <summary>
    /// When overridden in a derived class, allows a designer to add items to the set of events that it exposes through a
    /// <see cref="T:System.ComponentModel.TypeDescriptor">
    /// </see>.
    /// </summary>
    /// <param name="events">
    /// The
    /// <see cref="T:System.ComponentModel.EventDescriptor">
    /// </see> objects that represent the events of the class of the component. The keys in the dictionary of events are event names.
    /// </param>
    public void PreFilterEvents(IDictionary events)
    {

    }

    /// <summary>
    /// When overridden in a derived class, allows a designer to add items to the set of properties that it exposes through a
    /// <see cref="T:System.ComponentModel.TypeDescriptor">
    /// </see>.
    /// </summary>
    /// <param name="properties">
    /// The
    /// <see cref="T:System.ComponentModel.PropertyDescriptor">
    /// </see> objects that represent the properties of the class of the component. The keys in the dictionary of properties are property names.
    /// </param>
    public void PreFilterProperties(IDictionary properties)
    {

    }
}