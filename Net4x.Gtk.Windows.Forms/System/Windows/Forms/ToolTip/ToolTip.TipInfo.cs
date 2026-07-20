// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms;
/// <summary>
/// Defines a set of standardized icons that can be associated with a ToolTip.
/// </summary>
/// <filterpriority>
/// 2
/// </filterpriority>
public partial class ToolTip
{
    private partial class TipInfo
    {
        private string? _caption;

        private readonly string? _designerText;

        public TipInfo(string? caption, Type type)
        {
            _caption = caption;
            TipType = type;
            if (type == Type.Auto)
            {
                _designerText = caption;
            }
        }

        public string? Caption
        {
            get => ((TipType & (Type.Absolute | Type.SemiAbsolute)) != 0) ? _caption : _designerText;
            set => _caption = value;
        }

        public Point Position
        {
            get;
            set;
        }

        public Type TipType
        {
            get;
            set;
        } = Type.Auto;

    }
}