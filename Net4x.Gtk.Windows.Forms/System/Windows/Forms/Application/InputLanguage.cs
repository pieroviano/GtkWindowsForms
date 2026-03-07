// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using Gdk;
using Microsoft.Win32;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides methods and fields to manage the input language. This class cannot be inherited.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public sealed class InputLanguage
    {
        /// <summary>
        /// The HKL handle.
        /// </summary>
        private readonly IntPtr handle;

        internal InputLanguage(IntPtr handle)
        {
            this.handle = handle;
        }

        /// <summary>
        /// Gets the culture of the current input language.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Globalization.CultureInfo" /> that represents the culture of the current input language.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public CultureInfo Culture => new CultureInfo((int)handle & 0xFFFF);

        /// <summary>
        /// Gets or sets the input language for the current thread.
        /// </summary>
        /// <returns>An
        /// <see cref="System.Windows.Forms.InputLanguage" /> that represents the input language for the current thread.
        /// </returns>
        /// <exception cref="System.ArgumentException">The input language is not recognized by the system.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public static InputLanguage CurrentInputLanguage
        {
            get
            {
                return new InputLanguage(PangoHelper.ContextGet().Language.Handle);
            }
            set
            {
                if (value == null)
                {
                    value = InputLanguage.DefaultInputLanguage;
                }

            }
        }

        /// <summary>
        /// Gets the default input language for the system.
        /// </summary>
        /// <returns>An
        /// <see cref="System.Windows.Forms.InputLanguage" /> representing the default input language for the system.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public static InputLanguage DefaultInputLanguage
        {
            get
            {
                return new InputLanguage(PangoHelper.ContextGet().Language.Handle);
            }
        }

        /// <summary>
        /// Gets the handle for the input language.
        /// </summary>
        /// <returns>An
        /// <see cref="System.IntPtr" /> that represents the handle of this input language.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public IntPtr Handle => handle;

        /// <summary>
        /// Gets a list of all installed input languages.
        /// </summary>
        /// <returns>An array of
        /// <see cref="System.Windows.Forms.InputLanguage" /> objects that represent the input languages installed on the computer.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public static InputLanguageCollection InstalledInputLanguages
        {
            get
            {
                int size = 1;
                InputLanguage[] ils = new InputLanguage[size];

                for (int i = 0; i < size; i++)
                {
                    ils[i] = new InputLanguage(PangoHelper.ContextGet().Language.Handle);
                }

                return new InputLanguageCollection(ils);
            }
        }

        /// <summary>
        /// Gets the name of the current keyboard layout as it appears in the regional settings of the operating system on the computer.
        /// </summary>
        /// <returns>The name of the layout.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode" />
        /// </PermissionSet>
        public string LayoutName
        {
            get
            {

                return PangoHelper.ContextGet().Language.ToString();
            }
        }

        /// <summary>
        /// Specifies whether two input languages are equal.
        /// </summary>
        /// <returns>true if the two languages are equal; otherwise, false.
        /// </returns>
        /// <param name="value">The language to test for equality.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public override bool Equals(object value)
        {
            return (value is InputLanguage other) && (handle == other.handle);
        }

        /// <summary>
        /// Returns the input language associated with the specified culture.
        /// </summary>
        /// <returns>An
        /// <see cref="System.Windows.Forms.InputLanguage" /> that represents the previously selected input language.
        /// </returns>
        /// <param name="culture">The
        /// <see cref="System.Globalization.CultureInfo" /> that specifies the culture to convert from.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public static InputLanguage FromCulture(CultureInfo culture)
        {
            if (culture == null)
            {
                throw new ArgumentNullException(nameof(culture));
            }
            // KeyboardLayoutId is the LCID for built-in cultures, but it
            // is the CU-preferred keyboard language for custom cultures.
            int lcid = culture.KeyboardLayoutId;

            foreach (InputLanguage lang in InstalledInputLanguages)
            {
                if ((unchecked((int)(long)lang.handle) & 0xFFFF) == lcid)
                {
                    return lang;
                }
            }

            return DefaultInputLanguage;
        }

        /// <summary>
        /// Returns the hash code for this input language.
        /// </summary>
        /// <returns>The hash code for this input language.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public override int GetHashCode() => unchecked((int)(long)handle);

        private static string PadWithZeroes(string input, int length)
        {
            return "0000000000000000".Substring(0, length - input.Length) + input;
        }
    }
}
