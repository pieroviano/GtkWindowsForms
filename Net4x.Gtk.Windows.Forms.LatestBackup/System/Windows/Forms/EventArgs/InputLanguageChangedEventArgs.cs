// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="!:System.Windows.Forms.Form.InputLanguageChanged" /> event.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class InputLanguageChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.InputLanguageChangedEventArgs" /> class with the specified locale and character set.
        /// </summary>
        /// <param name="culture">The locale of the input language.
        /// </param>
        /// <param name="charSet">The character set associated with the new input language.
        /// </param>
        public InputLanguageChangedEventArgs(CultureInfo culture, byte charSet)
        {
            InputLanguage = InputLanguage.FromCulture(culture);
            Culture = culture;
            CharSet = charSet;
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.InputLanguageChangedEventArgs" /> class with the specified input language and character set.
        /// </summary>
        /// <param name="inputLanguage">The input language.
        /// </param>
        /// <param name="charSet">The character set associated with the new input language.
        /// </param>
        public InputLanguageChangedEventArgs(InputLanguage inputLanguage, byte charSet)
        {
            InputLanguage = inputLanguage ?? throw new ArgumentNullException(nameof(inputLanguage));
            Culture = inputLanguage.Culture;
            CharSet = charSet;
        }

        /// <summary>
        /// Gets the character set associated with the new input language.
        /// </summary>
        /// <returns>An 8-bit unsigned integer that corresponds to the character set, as shown in the following table.Character Set Value ANSI_CHARSET 0 DEFAULT_CHARSET 1 SYMBOL_CHARSET 2 MAC_CHARSET 77 SHIFTJI_CHARSET 128 HANGEUL_CHARSET 129 HANGUL_CHARSET 129 JOHAB_CHARSET 130 GB2312_CHARSET 134 CHINESEBIG5_CHARSET 136 GREEK_CHARSET 161 TURKISH_CHARSET 162 VIETNAMESE_CHARSET 163 HEBREW_CHARSET 177 ARABIC_CHARSET 178 BALTIC_CHARSET 186 RUSSIAN_CHARSET 204 THAI_CHARSET 222 EASTEUROPE_CHARSET 238 OEM_CHARSET 255
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public byte CharSet { get; }

        /// <summary>
        /// Gets the locale of the input language.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Globalization.CultureInfo" /> that specifies the locale of the input language.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public CultureInfo Culture { get; }

        /// <summary>
        /// Gets a value indicating the input language.
        /// </summary>
        /// <returns>An
        /// <see cref="System.Windows.Forms.InputLanguage" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public InputLanguage InputLanguage { get; }

    }
}
