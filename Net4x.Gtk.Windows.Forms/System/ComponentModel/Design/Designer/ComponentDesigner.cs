// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Configuration;
using System.Windows.Forms.Design;

namespace System.ComponentModel.Design
{
    /// <summary>
    /// The default designer for all components.
    /// </summary>
    public partial class ComponentDesigner : ITreeDesigner, IDesignerFilter, IComponentInitializer
    {

        private protected const string SettingsKeyName = "SettingsKey";
        private IComponent? _component;

        private InheritanceAttribute? _inheritanceAttribute;

        private bool _settingsKeyExplicitlySet;

        private DesignerVerbCollection? _verbs;

        public ICollection Children => throw new NotImplementedException();

        public IComponent Component => throw new NotImplementedException();

        public IDesigner Parent => throw new NotImplementedException();

        public DesignerVerbCollection Verbs => throw new NotImplementedException();

        public void Dispose()
        {

        }

        public void DoDefaultAction()
        {

        }

        public void Initialize(IComponent component)
        {

        }

        public void InitializeExistingComponent(IDictionary defaultValues)
        {

        }

        public void InitializeNewComponent(IDictionary defaultValues)
        {

        }

        public void PostFilterAttributes(IDictionary attributes)
        {

        }

        public void PostFilterEvents(IDictionary events)
        {

        }

        public void PostFilterProperties(IDictionary properties)
        {

        }

        public void PreFilterAttributes(IDictionary attributes)
        {

        }

        public void PreFilterEvents(IDictionary events)
        {

        }

        public void PreFilterProperties(IDictionary properties)
        {

        }
    }
}
