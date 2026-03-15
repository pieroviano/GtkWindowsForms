// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.Globalization;

namespace System.Windows.Forms;
/// <summary>
/// Manages the collection of
/// <see cref="T:System.Windows.Forms.BindingManagerBase" />
/// objects for any object that inherits from the
/// <see cref="T:System.Windows.Forms.Control" /> class.
/// </summary>
/// <filterpriority>
/// 2
/// </filterpriority>
public class BindingContext : ContextBoundObject
{

    private readonly Hashtable listManagers = new();

    /// <summary>
    /// Determines whether this instance contains the object.
    /// </summary>
    /// <param name="dataSource">
    /// The data source.
    /// </param>
    /// <param name="dataMember">
    /// The data member.
    /// </param>
    /// <returns>
    /// <c>
    /// true
    /// </c> if contains the specified data source; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </returns>
    public bool Contains(object? dataSource, string? dataMember)
    {
        return listManagers.ContainsKey(GetKey(dataSource, dataMember));
    }

    /// <summary>
    /// Updates the binding.
    /// </summary>
    /// <param name="newBindingContext">
    /// The new binding context.
    /// </param>
    /// <param name="binding">
    /// The binding.
    /// </param>
    public static void UpdateBinding(BindingContext? newBindingContext, Binding binding)
    {
        var bindingManagerBase = binding.BindingManagerBase;
        bindingManagerBase?.Bindings.Remove(binding);
        if (newBindingContext != null)
        {
            if (binding.BindToObject?.BindingManagerBase is PropertyManager)
            {
                CheckPropertyBindingCycles(newBindingContext, binding);
            }

            var bindToObject = binding.BindToObject;
            if (bindToObject != null)
            {
                var bindingManagerBase1 = newBindingContext.EnsureListManager(bindToObject.DataSource,
                    bindToObject.BindingMemberInfo.BindingPath);
                bindingManagerBase1?.Bindings.Add(binding);
            }
        }
    }

    internal BindingManagerBase EnsureListManager(object? dataSource, string? dataMember)
    {
        BindingManagerBase? relatedCurrencyManager = null;
        if (dataMember == null)
        {
            dataMember = "";
        }

        if (dataSource is ICurrencyManagerProvider)
        {
            relatedCurrencyManager =
                (dataSource as ICurrencyManagerProvider)?.GetRelatedCurrencyManager(dataMember);
            if (relatedCurrencyManager != null)
            {
                return relatedCurrencyManager;
            }
        }

        var key = GetKey(dataSource, dataMember);
        var item = listManagers[key] as WeakReference;
        if (item != null)
        {
            relatedCurrencyManager = (BindingManagerBase)item.Target;
        }

        if (relatedCurrencyManager != null)
        {
            return relatedCurrencyManager;
        }

        if (dataMember.Length != 0)
        {
            var num = dataMember.LastIndexOf(".", StringComparison.Ordinal);
            var str = num == -1 ? "" : dataMember.Substring(0, num);
            var str1 = dataMember.Substring(num + 1);
            var bindingManagerBase = EnsureListManager(dataSource, str);
            var propertyDescriptor = (bindingManagerBase.GetItemProperties()?.Find(str1, true)) ?? throw new ArgumentException("RelatedListManagerChild");
            if (!typeof(IList).IsAssignableFrom(propertyDescriptor.PropertyType))
            {
                relatedCurrencyManager = new RelatedPropertyManager(bindingManagerBase, str1);
            }
            else
            {
                relatedCurrencyManager = new RelatedCurrencyManager(bindingManagerBase, str1);
            }
        }
        else if (dataSource is IList || dataSource is IListSource)
        {
            relatedCurrencyManager = new CurrencyManager(dataSource);
        }
        else
        {
            relatedCurrencyManager = new PropertyManager(dataSource);
        }

        if (item != null)
        {
            item.Target = relatedCurrencyManager;
        }
        else
        {
            listManagers.Add(key, new WeakReference(relatedCurrencyManager, false));
        }

        ScrubWeakRefs();

        return relatedCurrencyManager;
    }

    internal HashKey GetKey(object? dataSource, string? dataMember)
    {
        return new HashKey(dataSource, dataMember);
    }

    private static void CheckPropertyBindingCycles(BindingContext? newBindingContext, Binding propBinding)
    {
        if (newBindingContext == null || propBinding == null)
        {
            return;
        }

        if (newBindingContext.Contains(propBinding.BindableComponent, ""))
        {
            var bindingManagerBase = newBindingContext.EnsureListManager(propBinding.BindableComponent, "");
            for (var i = 0; i < bindingManagerBase.Bindings.Count; i++)
            {
                var item = bindingManagerBase.Bindings[i];
                if (item.DataSource == propBinding.BindableComponent)
                {
                    if (propBinding.BindToObject?.BindingMemberInfo.BindingMember.Equals(item.PropertyName) ??
                        false)
                    {
                        throw new ArgumentException(@"DataBindingCycle", "propBinding");
                    }
                }
                else if (propBinding.BindToObject?.BindingManagerBase is PropertyManager)
                {
                    CheckPropertyBindingCycles(newBindingContext, item);
                }
            }
        }
    }

    private void ScrubWeakRefs()
    {
        ArrayList? arrayLists = null;
        foreach (DictionaryEntry listManager in listManagers)
        {
            if (((WeakReference)listManager.Value).Target != null)
            {
                continue;
            }

            if (arrayLists == null)
            {
                arrayLists = new ArrayList();
            }

            arrayLists.Add(listManager.Key);
        }

        if (arrayLists != null)
        {
            foreach (var arrayList in arrayLists)
            {
                listManagers.Remove(arrayList);
            }
        }
    }

    internal class HashKey
    {

        private readonly string dataMember;

        private readonly int dataSourceHashCode;

        private readonly WeakReference wRef;

        internal HashKey(object? dataSource, string? dataMember)
        {
            if (dataSource == null)
            {
                throw new ArgumentNullException("dataSource");
            }

            if (dataMember == null)
            {
                dataMember = "";
            }

            wRef = new WeakReference(dataSource, false);
            dataSourceHashCode = dataSource.GetHashCode();
            this.dataMember = dataMember.ToLower(CultureInfo.InvariantCulture);
        }

        public override bool Equals(object? target)
        {
            if (!(target is HashKey))
            {
                return false;
            }

            var hashKey = (HashKey)target;
            if (wRef.Target != hashKey.wRef.Target)
            {
                return false;
            }

            return dataMember == hashKey.dataMember;
        }

        public override int GetHashCode()
        {
            return dataSourceHashCode * dataMember.GetHashCode();
        }

    }
}