using System.ComponentModel;

namespace System.Windows.Forms;

internal class BindToObject
{

    private BindingManagerBase? _bindingManager;

    private readonly BindingMemberInfo _dataMember;

    private readonly object _dataSource;

    private bool _dataSourceInitialized;

    private string _errorText = string.Empty;

    private PropertyDescriptor? _fieldInfo;

    private readonly Binding? _owner;

    private bool _waitingOnDataSource;

    internal BindToObject(Binding owner, object dataSource, string? dataMember)
    {
        _owner = owner;
        _dataSource = dataSource;
        _dataMember = new BindingMemberInfo(dataMember);
        CheckBinding();
    }

    internal BindingManagerBase? BindingManagerBase => _bindingManager;

    internal BindingMemberInfo BindingMemberInfo => _dataMember;

    internal Type? BindToType
    {
        get
        {
            // If a specific field is bound, prefer the resolved descriptor's type.
            if (_dataMember.BindingField.Length != 0)
            {
                if (_fieldInfo != null)
                    return _fieldInfo.PropertyType;

                // Fallback: try to resolve the property descriptor from the active manager (explicit or owner's).
                var managerForType = _bindingManager ?? _owner?.BindingManagerBase;
                var pd = managerForType?.GetItemProperties()?.Find(_dataMember.BindingField, true);
                return pd?.PropertyType;
            }

            // Otherwise return the managed list/item type.
            var manager = _bindingManager ?? _owner?.BindingManagerBase;
            var bindType = manager?.BindType;
            if (bindType != null && typeof(Array).IsAssignableFrom(bindType))
            {
                bindType = bindType.GetElementType();
            }

            return bindType;
        }
    }

    internal string DataErrorText => _errorText;

    internal object DataSource => _dataSource;

    internal PropertyDescriptor? FieldInfo => _fieldInfo;

    private bool IsDataSourceInitialized
    {
        get
        {
            if (_dataSourceInitialized)
            {
                return true;
            }

            if (_dataSource is not ISupportInitializeNotification supportInitializeNotification || supportInitializeNotification.IsInitialized)
            {
                _dataSourceInitialized = true;
                return true;
            }
            if (_waitingOnDataSource)
            {
                return false;
            }
            supportInitializeNotification.Initialized += DataSource_Initialized;
            _waitingOnDataSource = true;

            return false;
        }
    }

    internal void CheckBinding()
    {
        // Preserve legacy design-time early exit
        if (_owner is { BindableComponent: not null } && _owner.ControlAtDesignTime())
        {
            return;
        }

        // Prefer explicit manager on this BindToObject; otherwise use owner's manager.
        var manager = _bindingManager ?? _owner?.BindingManagerBase;
        var component = manager?.Current;

        // If no manager or data source not initialized yet, clear resolved info and return.
        if (manager == null || !IsDataSourceInitialized)
        {
            _fieldInfo = null;
            return;
        }

        // Resolve field/property descriptor using the manager (works even when no control is attached).
        var bindingField = _dataMember.BindingField;
        _fieldInfo = manager.GetItemProperties()?.Find(bindingField, true);

        // Do not throw in headless scenarios where a control isn't attached yet.
        // Only validate/throw once a bindable component exists and is created (runtime behavior).
        if (manager.DataSource != null && _fieldInfo == null && bindingField.Length > 0)
        {
            if ((_owner?.BindableComponent != null) && _owner.ComponentCreated)
            {
                throw new ArgumentException(@"ListBindingBindField", nameof(_dataMember));
            }

            // otherwise keep _fieldInfo null and defer throwing until a control is attached
        }

        // Wire change events only when we have a concrete current component instance and manager indicates binding.
        if (_fieldInfo != null && manager.IsBinding && !(manager is CurrencyManager))
        {
            if (component != null)
            {
                _fieldInfo.AddValueChanged(component, PropValueChanged);
            }
        }
    }

    internal object? GetValue()
    {
        var manager = _bindingManager ?? _owner?.BindingManagerBase;
        var current = manager?.Current;
        _errorText = GetErrorText(current);
        if (_fieldInfo != null)
        {
            if (current != null)
            {
                current = _fieldInfo.GetValue(current);
            }
        }

        return current;
    }

    internal void SetBindingManagerBase(BindingManagerBase? lManager)
    {
        if (_bindingManager == lManager)
        {
            return;
        }
        if (_bindingManager != null && _fieldInfo != null && _bindingManager.IsBinding && !(_bindingManager is CurrencyManager))
        {
            if (_bindingManager.Current != null)
            {
                _fieldInfo.RemoveValueChanged(_bindingManager.Current, PropValueChanged);
            }

            _fieldInfo = null;
        }
        _bindingManager = lManager;
        CheckBinding();
    }

    internal void SetValue(object? value)
    {
        object? current = null;
        if (_fieldInfo == null)
        {
            if (_bindingManager is CurrencyManager currencyManager)
            {
                if (value != null)
                {
                    currencyManager[currencyManager.Position] = value;
                    current = value;
                }
            }
        }
        else
        {
            var manager = _bindingManager ?? _owner?.BindingManagerBase;
            current = manager?.Current;
            (current as IEditableObject)?.BeginEdit();
            if (!_fieldInfo.IsReadOnly)
            {
                _fieldInfo.SetValue(current, value);
            }
        }
        _errorText = GetErrorText(current);
    }

    private void DataSource_Initialized(object? sender, EventArgs e)
    {
        if (_dataSource is ISupportInitializeNotification supportInitializeNotification)
        {
            supportInitializeNotification.Initialized -= DataSource_Initialized;
        }
        _waitingOnDataSource = false;
        _dataSourceInitialized = true;
        CheckBinding();
    }

    private string GetErrorText(object? value)
    {
        var dataErrorInfo = value as IDataErrorInfo;
        var empty = string.Empty;
        if (dataErrorInfo != null)
        {
            empty = _fieldInfo != null ? dataErrorInfo[_fieldInfo.Name] : dataErrorInfo.Error;
        }

        return empty ?? string.Empty;
    }

    private void PropValueChanged(object? sender, EventArgs e)
    {
        var manager = _bindingManager ?? _owner?.BindingManagerBase;
        manager?.OnCurrentChanged(EventArgs.Empty);
    }
}