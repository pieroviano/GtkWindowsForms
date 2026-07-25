// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.VisualStyles;

/// <summary>
/// Identifies a control part or state drawn with visual styles, exposed as a hierarchy of well-known
/// elements (for example <c>VisualStyleElement.Button.PushButton.Normal</c>). On this GTK layer the
/// class/part/state triple is metadata only; the theme itself is drawn by GTK.
/// </summary>
public class VisualStyleElement
{
    private VisualStyleElement(string className, int part, int state)
    {
        ClassName = className;
        Part = part;
        State = state;
    }

    /// <summary>Gets the class name of the visual style element.</summary>
    public string ClassName { get; }

    /// <summary>Gets the part of the visual style element.</summary>
    public int Part { get; }

    /// <summary>Gets the state of the visual style element.</summary>
    public int State { get; }

    /// <summary>Creates a visual style element from the specified class name, part and state.</summary>
    /// <param name="className">The class name.</param>
    /// <param name="part">The part identifier.</param>
    /// <param name="state">The state identifier.</param>
    /// <returns>A new <see cref="T:System.Windows.Forms.VisualStyles.VisualStyleElement" />.</returns>
    public static VisualStyleElement CreateElement(string className, int part, int state)
        => new VisualStyleElement(className, part, state);

    /// <summary>Provides visual style elements for the Button control.</summary>
    public static class Button
    {
        /// <summary>Provides the Button.PushButton visual style elements.</summary>
        public static class PushButton
        {
            /// <summary>Gets the Button.PushButton.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("BUTTON", 1, 1);

            /// <summary>Gets the Button.PushButton.Hot visual style element.</summary>
            public static VisualStyleElement Hot => CreateElement("BUTTON", 1, 2);

            /// <summary>Gets the Button.PushButton.Pressed visual style element.</summary>
            public static VisualStyleElement Pressed => CreateElement("BUTTON", 1, 3);

            /// <summary>Gets the Button.PushButton.Disabled visual style element.</summary>
            public static VisualStyleElement Disabled => CreateElement("BUTTON", 1, 4);

            /// <summary>Gets the Button.PushButton.Default visual style element.</summary>
            public static VisualStyleElement Default => CreateElement("BUTTON", 1, 5);
        }

        /// <summary>Provides the Button.RadioButton visual style elements.</summary>
        public static class RadioButton
        {
            /// <summary>Gets the Button.RadioButton.UncheckedNormal visual style element.</summary>
            public static VisualStyleElement UncheckedNormal => CreateElement("BUTTON", 2, 1);

            /// <summary>Gets the Button.RadioButton.UncheckedHot visual style element.</summary>
            public static VisualStyleElement UncheckedHot => CreateElement("BUTTON", 2, 2);

            /// <summary>Gets the Button.RadioButton.UncheckedPressed visual style element.</summary>
            public static VisualStyleElement UncheckedPressed => CreateElement("BUTTON", 2, 3);

            /// <summary>Gets the Button.RadioButton.UncheckedDisabled visual style element.</summary>
            public static VisualStyleElement UncheckedDisabled => CreateElement("BUTTON", 2, 4);

            /// <summary>Gets the Button.RadioButton.CheckedNormal visual style element.</summary>
            public static VisualStyleElement CheckedNormal => CreateElement("BUTTON", 2, 5);

            /// <summary>Gets the Button.RadioButton.CheckedHot visual style element.</summary>
            public static VisualStyleElement CheckedHot => CreateElement("BUTTON", 2, 6);

            /// <summary>Gets the Button.RadioButton.CheckedPressed visual style element.</summary>
            public static VisualStyleElement CheckedPressed => CreateElement("BUTTON", 2, 7);

            /// <summary>Gets the Button.RadioButton.CheckedDisabled visual style element.</summary>
            public static VisualStyleElement CheckedDisabled => CreateElement("BUTTON", 2, 8);
        }

        /// <summary>Provides the Button.CheckBox visual style elements.</summary>
        public static class CheckBox
        {
            /// <summary>Gets the Button.CheckBox.UncheckedNormal visual style element.</summary>
            public static VisualStyleElement UncheckedNormal => CreateElement("BUTTON", 3, 1);

            /// <summary>Gets the Button.CheckBox.UncheckedHot visual style element.</summary>
            public static VisualStyleElement UncheckedHot => CreateElement("BUTTON", 3, 2);

            /// <summary>Gets the Button.CheckBox.UncheckedPressed visual style element.</summary>
            public static VisualStyleElement UncheckedPressed => CreateElement("BUTTON", 3, 3);

            /// <summary>Gets the Button.CheckBox.UncheckedDisabled visual style element.</summary>
            public static VisualStyleElement UncheckedDisabled => CreateElement("BUTTON", 3, 4);

            /// <summary>Gets the Button.CheckBox.CheckedNormal visual style element.</summary>
            public static VisualStyleElement CheckedNormal => CreateElement("BUTTON", 3, 5);

            /// <summary>Gets the Button.CheckBox.CheckedHot visual style element.</summary>
            public static VisualStyleElement CheckedHot => CreateElement("BUTTON", 3, 6);

            /// <summary>Gets the Button.CheckBox.CheckedPressed visual style element.</summary>
            public static VisualStyleElement CheckedPressed => CreateElement("BUTTON", 3, 7);

            /// <summary>Gets the Button.CheckBox.CheckedDisabled visual style element.</summary>
            public static VisualStyleElement CheckedDisabled => CreateElement("BUTTON", 3, 8);

            /// <summary>Gets the Button.CheckBox.MixedNormal visual style element.</summary>
            public static VisualStyleElement MixedNormal => CreateElement("BUTTON", 3, 9);

            /// <summary>Gets the Button.CheckBox.MixedHot visual style element.</summary>
            public static VisualStyleElement MixedHot => CreateElement("BUTTON", 3, 10);

            /// <summary>Gets the Button.CheckBox.MixedPressed visual style element.</summary>
            public static VisualStyleElement MixedPressed => CreateElement("BUTTON", 3, 11);

            /// <summary>Gets the Button.CheckBox.MixedDisabled visual style element.</summary>
            public static VisualStyleElement MixedDisabled => CreateElement("BUTTON", 3, 12);
        }

        /// <summary>Provides the Button.GroupBox visual style elements.</summary>
        public static class GroupBox
        {
            /// <summary>Gets the Button.GroupBox.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("BUTTON", 4, 1);

            /// <summary>Gets the Button.GroupBox.Disabled visual style element.</summary>
            public static VisualStyleElement Disabled => CreateElement("BUTTON", 4, 2);
        }

        /// <summary>Provides the Button.UserButton visual style elements.</summary>
        public static class UserButton
        {
            /// <summary>Gets the Button.UserButton.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("BUTTON", 5, 1);
        }
    }

    /// <summary>Provides visual style elements for the ComboBox control.</summary>
    public static class ComboBox
    {
        /// <summary>Provides the ComboBox.DropDownButton visual style elements.</summary>
        public static class DropDownButton
        {
            /// <summary>Gets the ComboBox.DropDownButton.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("COMBOBOX", 1, 1);

            /// <summary>Gets the ComboBox.DropDownButton.Hot visual style element.</summary>
            public static VisualStyleElement Hot => CreateElement("COMBOBOX", 1, 2);

            /// <summary>Gets the ComboBox.DropDownButton.Pressed visual style element.</summary>
            public static VisualStyleElement Pressed => CreateElement("COMBOBOX", 1, 3);

            /// <summary>Gets the ComboBox.DropDownButton.Disabled visual style element.</summary>
            public static VisualStyleElement Disabled => CreateElement("COMBOBOX", 1, 4);
        }
    }

    /// <summary>Provides visual style elements for the Page control.</summary>
    public static class Page
    {
        /// <summary>Provides the Page.Up visual style elements.</summary>
        public static class Up
        {
            /// <summary>Gets the Page.Up.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("PAGE", 1, 1);

            /// <summary>Gets the Page.Up.Hot visual style element.</summary>
            public static VisualStyleElement Hot => CreateElement("PAGE", 1, 2);

            /// <summary>Gets the Page.Up.Pressed visual style element.</summary>
            public static VisualStyleElement Pressed => CreateElement("PAGE", 1, 3);

            /// <summary>Gets the Page.Up.Disabled visual style element.</summary>
            public static VisualStyleElement Disabled => CreateElement("PAGE", 1, 4);
        }

        /// <summary>Provides the Page.Down visual style elements.</summary>
        public static class Down
        {
            /// <summary>Gets the Page.Down.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("PAGE", 2, 1);

            /// <summary>Gets the Page.Down.Hot visual style element.</summary>
            public static VisualStyleElement Hot => CreateElement("PAGE", 2, 2);

            /// <summary>Gets the Page.Down.Pressed visual style element.</summary>
            public static VisualStyleElement Pressed => CreateElement("PAGE", 2, 3);

            /// <summary>Gets the Page.Down.Disabled visual style element.</summary>
            public static VisualStyleElement Disabled => CreateElement("PAGE", 2, 4);
        }

        /// <summary>Provides the Page.UpHorizontal visual style elements.</summary>
        public static class UpHorizontal
        {
            /// <summary>Gets the Page.UpHorizontal.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("PAGE", 3, 1);

            /// <summary>Gets the Page.UpHorizontal.Hot visual style element.</summary>
            public static VisualStyleElement Hot => CreateElement("PAGE", 3, 2);

            /// <summary>Gets the Page.UpHorizontal.Pressed visual style element.</summary>
            public static VisualStyleElement Pressed => CreateElement("PAGE", 3, 3);

            /// <summary>Gets the Page.UpHorizontal.Disabled visual style element.</summary>
            public static VisualStyleElement Disabled => CreateElement("PAGE", 3, 4);
        }

        /// <summary>Provides the Page.DownHorizontal visual style elements.</summary>
        public static class DownHorizontal
        {
            /// <summary>Gets the Page.DownHorizontal.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("PAGE", 4, 1);

            /// <summary>Gets the Page.DownHorizontal.Hot visual style element.</summary>
            public static VisualStyleElement Hot => CreateElement("PAGE", 4, 2);

            /// <summary>Gets the Page.DownHorizontal.Pressed visual style element.</summary>
            public static VisualStyleElement Pressed => CreateElement("PAGE", 4, 3);

            /// <summary>Gets the Page.DownHorizontal.Disabled visual style element.</summary>
            public static VisualStyleElement Disabled => CreateElement("PAGE", 4, 4);
        }
    }

    /// <summary>Provides visual style elements for the Spin control.</summary>
    public static class Spin
    {
        /// <summary>Provides the Spin.Up visual style elements.</summary>
        public static class Up
        {
            /// <summary>Gets the Spin.Up.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("SPIN", 1, 1);

            /// <summary>Gets the Spin.Up.Hot visual style element.</summary>
            public static VisualStyleElement Hot => CreateElement("SPIN", 1, 2);

            /// <summary>Gets the Spin.Up.Pressed visual style element.</summary>
            public static VisualStyleElement Pressed => CreateElement("SPIN", 1, 3);

            /// <summary>Gets the Spin.Up.Disabled visual style element.</summary>
            public static VisualStyleElement Disabled => CreateElement("SPIN", 1, 4);
        }

        /// <summary>Provides the Spin.Down visual style elements.</summary>
        public static class Down
        {
            /// <summary>Gets the Spin.Down.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("SPIN", 2, 1);

            /// <summary>Gets the Spin.Down.Hot visual style element.</summary>
            public static VisualStyleElement Hot => CreateElement("SPIN", 2, 2);

            /// <summary>Gets the Spin.Down.Pressed visual style element.</summary>
            public static VisualStyleElement Pressed => CreateElement("SPIN", 2, 3);

            /// <summary>Gets the Spin.Down.Disabled visual style element.</summary>
            public static VisualStyleElement Disabled => CreateElement("SPIN", 2, 4);
        }

        /// <summary>Provides the Spin.UpHorizontal visual style elements.</summary>
        public static class UpHorizontal
        {
            /// <summary>Gets the Spin.UpHorizontal.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("SPIN", 3, 1);

            /// <summary>Gets the Spin.UpHorizontal.Hot visual style element.</summary>
            public static VisualStyleElement Hot => CreateElement("SPIN", 3, 2);

            /// <summary>Gets the Spin.UpHorizontal.Pressed visual style element.</summary>
            public static VisualStyleElement Pressed => CreateElement("SPIN", 3, 3);

            /// <summary>Gets the Spin.UpHorizontal.Disabled visual style element.</summary>
            public static VisualStyleElement Disabled => CreateElement("SPIN", 3, 4);
        }

        /// <summary>Provides the Spin.DownHorizontal visual style elements.</summary>
        public static class DownHorizontal
        {
            /// <summary>Gets the Spin.DownHorizontal.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("SPIN", 4, 1);

            /// <summary>Gets the Spin.DownHorizontal.Hot visual style element.</summary>
            public static VisualStyleElement Hot => CreateElement("SPIN", 4, 2);

            /// <summary>Gets the Spin.DownHorizontal.Pressed visual style element.</summary>
            public static VisualStyleElement Pressed => CreateElement("SPIN", 4, 3);

            /// <summary>Gets the Spin.DownHorizontal.Disabled visual style element.</summary>
            public static VisualStyleElement Disabled => CreateElement("SPIN", 4, 4);
        }
    }

    /// <summary>Provides visual style elements for the ScrollBar control.</summary>
    public static class ScrollBar
    {
        /// <summary>Provides the ScrollBar.ArrowButton visual style elements.</summary>
        public static class ArrowButton
        {
            /// <summary>Gets the ScrollBar.ArrowButton.UpNormal visual style element.</summary>
            public static VisualStyleElement UpNormal => CreateElement("SCROLLBAR", 1, 1);

            /// <summary>Gets the ScrollBar.ArrowButton.UpHot visual style element.</summary>
            public static VisualStyleElement UpHot => CreateElement("SCROLLBAR", 1, 2);

            /// <summary>Gets the ScrollBar.ArrowButton.UpPressed visual style element.</summary>
            public static VisualStyleElement UpPressed => CreateElement("SCROLLBAR", 1, 3);

            /// <summary>Gets the ScrollBar.ArrowButton.UpDisabled visual style element.</summary>
            public static VisualStyleElement UpDisabled => CreateElement("SCROLLBAR", 1, 4);

            /// <summary>Gets the ScrollBar.ArrowButton.DownNormal visual style element.</summary>
            public static VisualStyleElement DownNormal => CreateElement("SCROLLBAR", 1, 5);

            /// <summary>Gets the ScrollBar.ArrowButton.DownHot visual style element.</summary>
            public static VisualStyleElement DownHot => CreateElement("SCROLLBAR", 1, 6);

            /// <summary>Gets the ScrollBar.ArrowButton.DownPressed visual style element.</summary>
            public static VisualStyleElement DownPressed => CreateElement("SCROLLBAR", 1, 7);

            /// <summary>Gets the ScrollBar.ArrowButton.DownDisabled visual style element.</summary>
            public static VisualStyleElement DownDisabled => CreateElement("SCROLLBAR", 1, 8);

            /// <summary>Gets the ScrollBar.ArrowButton.LeftNormal visual style element.</summary>
            public static VisualStyleElement LeftNormal => CreateElement("SCROLLBAR", 1, 9);

            /// <summary>Gets the ScrollBar.ArrowButton.LeftHot visual style element.</summary>
            public static VisualStyleElement LeftHot => CreateElement("SCROLLBAR", 1, 10);

            /// <summary>Gets the ScrollBar.ArrowButton.LeftPressed visual style element.</summary>
            public static VisualStyleElement LeftPressed => CreateElement("SCROLLBAR", 1, 11);

            /// <summary>Gets the ScrollBar.ArrowButton.LeftDisabled visual style element.</summary>
            public static VisualStyleElement LeftDisabled => CreateElement("SCROLLBAR", 1, 12);

            /// <summary>Gets the ScrollBar.ArrowButton.RightNormal visual style element.</summary>
            public static VisualStyleElement RightNormal => CreateElement("SCROLLBAR", 1, 13);

            /// <summary>Gets the ScrollBar.ArrowButton.RightHot visual style element.</summary>
            public static VisualStyleElement RightHot => CreateElement("SCROLLBAR", 1, 14);

            /// <summary>Gets the ScrollBar.ArrowButton.RightPressed visual style element.</summary>
            public static VisualStyleElement RightPressed => CreateElement("SCROLLBAR", 1, 15);

            /// <summary>Gets the ScrollBar.ArrowButton.RightDisabled visual style element.</summary>
            public static VisualStyleElement RightDisabled => CreateElement("SCROLLBAR", 1, 16);
        }

        /// <summary>Provides the ScrollBar.ThumbButtonHorizontal visual style elements.</summary>
        public static class ThumbButtonHorizontal
        {
            /// <summary>Gets the ScrollBar.ThumbButtonHorizontal.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("SCROLLBAR", 2, 1);

            /// <summary>Gets the ScrollBar.ThumbButtonHorizontal.Hot visual style element.</summary>
            public static VisualStyleElement Hot => CreateElement("SCROLLBAR", 2, 2);

            /// <summary>Gets the ScrollBar.ThumbButtonHorizontal.Pressed visual style element.</summary>
            public static VisualStyleElement Pressed => CreateElement("SCROLLBAR", 2, 3);

            /// <summary>Gets the ScrollBar.ThumbButtonHorizontal.Disabled visual style element.</summary>
            public static VisualStyleElement Disabled => CreateElement("SCROLLBAR", 2, 4);
        }

        /// <summary>Provides the ScrollBar.ThumbButtonVertical visual style elements.</summary>
        public static class ThumbButtonVertical
        {
            /// <summary>Gets the ScrollBar.ThumbButtonVertical.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("SCROLLBAR", 3, 1);

            /// <summary>Gets the ScrollBar.ThumbButtonVertical.Hot visual style element.</summary>
            public static VisualStyleElement Hot => CreateElement("SCROLLBAR", 3, 2);

            /// <summary>Gets the ScrollBar.ThumbButtonVertical.Pressed visual style element.</summary>
            public static VisualStyleElement Pressed => CreateElement("SCROLLBAR", 3, 3);

            /// <summary>Gets the ScrollBar.ThumbButtonVertical.Disabled visual style element.</summary>
            public static VisualStyleElement Disabled => CreateElement("SCROLLBAR", 3, 4);
        }

        /// <summary>Provides the ScrollBar.RightTrackHorizontal visual style elements.</summary>
        public static class RightTrackHorizontal
        {
            /// <summary>Gets the ScrollBar.RightTrackHorizontal.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("SCROLLBAR", 4, 1);

            /// <summary>Gets the ScrollBar.RightTrackHorizontal.Hot visual style element.</summary>
            public static VisualStyleElement Hot => CreateElement("SCROLLBAR", 4, 2);

            /// <summary>Gets the ScrollBar.RightTrackHorizontal.Pressed visual style element.</summary>
            public static VisualStyleElement Pressed => CreateElement("SCROLLBAR", 4, 3);

            /// <summary>Gets the ScrollBar.RightTrackHorizontal.Disabled visual style element.</summary>
            public static VisualStyleElement Disabled => CreateElement("SCROLLBAR", 4, 4);
        }

        /// <summary>Provides the ScrollBar.LeftTrackHorizontal visual style elements.</summary>
        public static class LeftTrackHorizontal
        {
            /// <summary>Gets the ScrollBar.LeftTrackHorizontal.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("SCROLLBAR", 5, 1);

            /// <summary>Gets the ScrollBar.LeftTrackHorizontal.Hot visual style element.</summary>
            public static VisualStyleElement Hot => CreateElement("SCROLLBAR", 5, 2);

            /// <summary>Gets the ScrollBar.LeftTrackHorizontal.Pressed visual style element.</summary>
            public static VisualStyleElement Pressed => CreateElement("SCROLLBAR", 5, 3);

            /// <summary>Gets the ScrollBar.LeftTrackHorizontal.Disabled visual style element.</summary>
            public static VisualStyleElement Disabled => CreateElement("SCROLLBAR", 5, 4);
        }

        /// <summary>Provides the ScrollBar.LowerTrackVertical visual style elements.</summary>
        public static class LowerTrackVertical
        {
            /// <summary>Gets the ScrollBar.LowerTrackVertical.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("SCROLLBAR", 6, 1);

            /// <summary>Gets the ScrollBar.LowerTrackVertical.Hot visual style element.</summary>
            public static VisualStyleElement Hot => CreateElement("SCROLLBAR", 6, 2);

            /// <summary>Gets the ScrollBar.LowerTrackVertical.Pressed visual style element.</summary>
            public static VisualStyleElement Pressed => CreateElement("SCROLLBAR", 6, 3);

            /// <summary>Gets the ScrollBar.LowerTrackVertical.Disabled visual style element.</summary>
            public static VisualStyleElement Disabled => CreateElement("SCROLLBAR", 6, 4);
        }

        /// <summary>Provides the ScrollBar.UpperTrackVertical visual style elements.</summary>
        public static class UpperTrackVertical
        {
            /// <summary>Gets the ScrollBar.UpperTrackVertical.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("SCROLLBAR", 7, 1);

            /// <summary>Gets the ScrollBar.UpperTrackVertical.Hot visual style element.</summary>
            public static VisualStyleElement Hot => CreateElement("SCROLLBAR", 7, 2);

            /// <summary>Gets the ScrollBar.UpperTrackVertical.Pressed visual style element.</summary>
            public static VisualStyleElement Pressed => CreateElement("SCROLLBAR", 7, 3);

            /// <summary>Gets the ScrollBar.UpperTrackVertical.Disabled visual style element.</summary>
            public static VisualStyleElement Disabled => CreateElement("SCROLLBAR", 7, 4);
        }

        /// <summary>Provides the ScrollBar.GripperHorizontal visual style elements.</summary>
        public static class GripperHorizontal
        {
            /// <summary>Gets the ScrollBar.GripperHorizontal.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("SCROLLBAR", 8, 1);
        }

        /// <summary>Provides the ScrollBar.GripperVertical visual style elements.</summary>
        public static class GripperVertical
        {
            /// <summary>Gets the ScrollBar.GripperVertical.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("SCROLLBAR", 9, 1);
        }

        /// <summary>Provides the ScrollBar.SizeBox visual style elements.</summary>
        public static class SizeBox
        {
            /// <summary>Gets the ScrollBar.SizeBox.RightAlign visual style element.</summary>
            public static VisualStyleElement RightAlign => CreateElement("SCROLLBAR", 10, 1);

            /// <summary>Gets the ScrollBar.SizeBox.LeftAlign visual style element.</summary>
            public static VisualStyleElement LeftAlign => CreateElement("SCROLLBAR", 10, 2);
        }
    }

    /// <summary>Provides visual style elements for the Tab control.</summary>
    public static class Tab
    {
        /// <summary>Provides the Tab.TabItem visual style elements.</summary>
        public static class TabItem
        {
            /// <summary>Gets the Tab.TabItem.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("TAB", 1, 1);

            /// <summary>Gets the Tab.TabItem.Hot visual style element.</summary>
            public static VisualStyleElement Hot => CreateElement("TAB", 1, 2);

            /// <summary>Gets the Tab.TabItem.Pressed visual style element.</summary>
            public static VisualStyleElement Pressed => CreateElement("TAB", 1, 3);

            /// <summary>Gets the Tab.TabItem.Disabled visual style element.</summary>
            public static VisualStyleElement Disabled => CreateElement("TAB", 1, 4);
        }

        /// <summary>Provides the Tab.TabItemLeftEdge visual style elements.</summary>
        public static class TabItemLeftEdge
        {
            /// <summary>Gets the Tab.TabItemLeftEdge.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("TAB", 2, 1);

            /// <summary>Gets the Tab.TabItemLeftEdge.Hot visual style element.</summary>
            public static VisualStyleElement Hot => CreateElement("TAB", 2, 2);

            /// <summary>Gets the Tab.TabItemLeftEdge.Pressed visual style element.</summary>
            public static VisualStyleElement Pressed => CreateElement("TAB", 2, 3);

            /// <summary>Gets the Tab.TabItemLeftEdge.Disabled visual style element.</summary>
            public static VisualStyleElement Disabled => CreateElement("TAB", 2, 4);
        }

        /// <summary>Provides the Tab.TabItemRightEdge visual style elements.</summary>
        public static class TabItemRightEdge
        {
            /// <summary>Gets the Tab.TabItemRightEdge.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("TAB", 3, 1);

            /// <summary>Gets the Tab.TabItemRightEdge.Hot visual style element.</summary>
            public static VisualStyleElement Hot => CreateElement("TAB", 3, 2);

            /// <summary>Gets the Tab.TabItemRightEdge.Pressed visual style element.</summary>
            public static VisualStyleElement Pressed => CreateElement("TAB", 3, 3);

            /// <summary>Gets the Tab.TabItemRightEdge.Disabled visual style element.</summary>
            public static VisualStyleElement Disabled => CreateElement("TAB", 3, 4);
        }

        /// <summary>Provides the Tab.TabItemBothEdges visual style elements.</summary>
        public static class TabItemBothEdges
        {
            /// <summary>Gets the Tab.TabItemBothEdges.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("TAB", 4, 1);
        }

        /// <summary>Provides the Tab.TopTabItem visual style elements.</summary>
        public static class TopTabItem
        {
            /// <summary>Gets the Tab.TopTabItem.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("TAB", 5, 1);

            /// <summary>Gets the Tab.TopTabItem.Hot visual style element.</summary>
            public static VisualStyleElement Hot => CreateElement("TAB", 5, 2);

            /// <summary>Gets the Tab.TopTabItem.Pressed visual style element.</summary>
            public static VisualStyleElement Pressed => CreateElement("TAB", 5, 3);

            /// <summary>Gets the Tab.TopTabItem.Disabled visual style element.</summary>
            public static VisualStyleElement Disabled => CreateElement("TAB", 5, 4);
        }

        /// <summary>Provides the Tab.TopTabItemLeftEdge visual style elements.</summary>
        public static class TopTabItemLeftEdge
        {
            /// <summary>Gets the Tab.TopTabItemLeftEdge.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("TAB", 6, 1);

            /// <summary>Gets the Tab.TopTabItemLeftEdge.Hot visual style element.</summary>
            public static VisualStyleElement Hot => CreateElement("TAB", 6, 2);

            /// <summary>Gets the Tab.TopTabItemLeftEdge.Pressed visual style element.</summary>
            public static VisualStyleElement Pressed => CreateElement("TAB", 6, 3);

            /// <summary>Gets the Tab.TopTabItemLeftEdge.Disabled visual style element.</summary>
            public static VisualStyleElement Disabled => CreateElement("TAB", 6, 4);
        }

        /// <summary>Provides the Tab.TopTabItemRightEdge visual style elements.</summary>
        public static class TopTabItemRightEdge
        {
            /// <summary>Gets the Tab.TopTabItemRightEdge.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("TAB", 7, 1);

            /// <summary>Gets the Tab.TopTabItemRightEdge.Hot visual style element.</summary>
            public static VisualStyleElement Hot => CreateElement("TAB", 7, 2);

            /// <summary>Gets the Tab.TopTabItemRightEdge.Pressed visual style element.</summary>
            public static VisualStyleElement Pressed => CreateElement("TAB", 7, 3);

            /// <summary>Gets the Tab.TopTabItemRightEdge.Disabled visual style element.</summary>
            public static VisualStyleElement Disabled => CreateElement("TAB", 7, 4);
        }

        /// <summary>Provides the Tab.TopTabItemBothEdges visual style elements.</summary>
        public static class TopTabItemBothEdges
        {
            /// <summary>Gets the Tab.TopTabItemBothEdges.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("TAB", 8, 1);
        }

        /// <summary>Provides the Tab.Pane visual style elements.</summary>
        public static class Pane
        {
            /// <summary>Gets the Tab.Pane.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("TAB", 9, 1);
        }

        /// <summary>Provides the Tab.Body visual style elements.</summary>
        public static class Body
        {
            /// <summary>Gets the Tab.Body.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("TAB", 10, 1);
        }
    }

    /// <summary>Provides visual style elements for the ExplorerBar control.</summary>
    public static class ExplorerBar
    {
        /// <summary>Provides the ExplorerBar.HeaderBackground visual style elements.</summary>
        public static class HeaderBackground
        {
            /// <summary>Gets the ExplorerBar.HeaderBackground.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("EXPLORERBAR", 1, 1);
        }

        /// <summary>Provides the ExplorerBar.HeaderClose visual style elements.</summary>
        public static class HeaderClose
        {
            /// <summary>Gets the ExplorerBar.HeaderClose.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("EXPLORERBAR", 2, 1);

            /// <summary>Gets the ExplorerBar.HeaderClose.Hot visual style element.</summary>
            public static VisualStyleElement Hot => CreateElement("EXPLORERBAR", 2, 2);

            /// <summary>Gets the ExplorerBar.HeaderClose.Pressed visual style element.</summary>
            public static VisualStyleElement Pressed => CreateElement("EXPLORERBAR", 2, 3);
        }

        /// <summary>Provides the ExplorerBar.HeaderPin visual style elements.</summary>
        public static class HeaderPin
        {
            /// <summary>Gets the ExplorerBar.HeaderPin.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("EXPLORERBAR", 3, 1);

            /// <summary>Gets the ExplorerBar.HeaderPin.Hot visual style element.</summary>
            public static VisualStyleElement Hot => CreateElement("EXPLORERBAR", 3, 2);

            /// <summary>Gets the ExplorerBar.HeaderPin.Pressed visual style element.</summary>
            public static VisualStyleElement Pressed => CreateElement("EXPLORERBAR", 3, 3);

            /// <summary>Gets the ExplorerBar.HeaderPin.SelectedNormal visual style element.</summary>
            public static VisualStyleElement SelectedNormal => CreateElement("EXPLORERBAR", 3, 4);

            /// <summary>Gets the ExplorerBar.HeaderPin.SelectedHot visual style element.</summary>
            public static VisualStyleElement SelectedHot => CreateElement("EXPLORERBAR", 3, 5);

            /// <summary>Gets the ExplorerBar.HeaderPin.SelectedPressed visual style element.</summary>
            public static VisualStyleElement SelectedPressed => CreateElement("EXPLORERBAR", 3, 6);
        }

        /// <summary>Provides the ExplorerBar.IEBarMenu visual style elements.</summary>
        public static class IEBarMenu
        {
            /// <summary>Gets the ExplorerBar.IEBarMenu.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("EXPLORERBAR", 4, 1);

            /// <summary>Gets the ExplorerBar.IEBarMenu.Hot visual style element.</summary>
            public static VisualStyleElement Hot => CreateElement("EXPLORERBAR", 4, 2);

            /// <summary>Gets the ExplorerBar.IEBarMenu.Pressed visual style element.</summary>
            public static VisualStyleElement Pressed => CreateElement("EXPLORERBAR", 4, 3);
        }

        /// <summary>Provides the ExplorerBar.NormalGroupBackground visual style elements.</summary>
        public static class NormalGroupBackground
        {
            /// <summary>Gets the ExplorerBar.NormalGroupBackground.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("EXPLORERBAR", 5, 1);
        }

        /// <summary>Provides the ExplorerBar.NormalGroupCollapse visual style elements.</summary>
        public static class NormalGroupCollapse
        {
            /// <summary>Gets the ExplorerBar.NormalGroupCollapse.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("EXPLORERBAR", 6, 1);

            /// <summary>Gets the ExplorerBar.NormalGroupCollapse.Hot visual style element.</summary>
            public static VisualStyleElement Hot => CreateElement("EXPLORERBAR", 6, 2);

            /// <summary>Gets the ExplorerBar.NormalGroupCollapse.Pressed visual style element.</summary>
            public static VisualStyleElement Pressed => CreateElement("EXPLORERBAR", 6, 3);
        }

        /// <summary>Provides the ExplorerBar.NormalGroupExpand visual style elements.</summary>
        public static class NormalGroupExpand
        {
            /// <summary>Gets the ExplorerBar.NormalGroupExpand.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("EXPLORERBAR", 7, 1);

            /// <summary>Gets the ExplorerBar.NormalGroupExpand.Hot visual style element.</summary>
            public static VisualStyleElement Hot => CreateElement("EXPLORERBAR", 7, 2);

            /// <summary>Gets the ExplorerBar.NormalGroupExpand.Pressed visual style element.</summary>
            public static VisualStyleElement Pressed => CreateElement("EXPLORERBAR", 7, 3);
        }

        /// <summary>Provides the ExplorerBar.NormalGroupHead visual style elements.</summary>
        public static class NormalGroupHead
        {
            /// <summary>Gets the ExplorerBar.NormalGroupHead.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("EXPLORERBAR", 8, 1);
        }

        /// <summary>Provides the ExplorerBar.SpecialGroupBackground visual style elements.</summary>
        public static class SpecialGroupBackground
        {
            /// <summary>Gets the ExplorerBar.SpecialGroupBackground.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("EXPLORERBAR", 9, 1);
        }

        /// <summary>Provides the ExplorerBar.SpecialGroupCollapse visual style elements.</summary>
        public static class SpecialGroupCollapse
        {
            /// <summary>Gets the ExplorerBar.SpecialGroupCollapse.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("EXPLORERBAR", 10, 1);

            /// <summary>Gets the ExplorerBar.SpecialGroupCollapse.Hot visual style element.</summary>
            public static VisualStyleElement Hot => CreateElement("EXPLORERBAR", 10, 2);

            /// <summary>Gets the ExplorerBar.SpecialGroupCollapse.Pressed visual style element.</summary>
            public static VisualStyleElement Pressed => CreateElement("EXPLORERBAR", 10, 3);
        }

        /// <summary>Provides the ExplorerBar.SpecialGroupExpand visual style elements.</summary>
        public static class SpecialGroupExpand
        {
            /// <summary>Gets the ExplorerBar.SpecialGroupExpand.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("EXPLORERBAR", 11, 1);

            /// <summary>Gets the ExplorerBar.SpecialGroupExpand.Hot visual style element.</summary>
            public static VisualStyleElement Hot => CreateElement("EXPLORERBAR", 11, 2);

            /// <summary>Gets the ExplorerBar.SpecialGroupExpand.Pressed visual style element.</summary>
            public static VisualStyleElement Pressed => CreateElement("EXPLORERBAR", 11, 3);
        }

        /// <summary>Provides the ExplorerBar.SpecialGroupHead visual style elements.</summary>
        public static class SpecialGroupHead
        {
            /// <summary>Gets the ExplorerBar.SpecialGroupHead.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("EXPLORERBAR", 12, 1);
        }
    }

    /// <summary>Provides visual style elements for the Header control.</summary>
    public static class Header
    {
        /// <summary>Provides the Header.Item visual style elements.</summary>
        public static class Item
        {
            /// <summary>Gets the Header.Item.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("HEADER", 1, 1);

            /// <summary>Gets the Header.Item.Hot visual style element.</summary>
            public static VisualStyleElement Hot => CreateElement("HEADER", 1, 2);

            /// <summary>Gets the Header.Item.Pressed visual style element.</summary>
            public static VisualStyleElement Pressed => CreateElement("HEADER", 1, 3);
        }

        /// <summary>Provides the Header.ItemLeft visual style elements.</summary>
        public static class ItemLeft
        {
            /// <summary>Gets the Header.ItemLeft.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("HEADER", 2, 1);

            /// <summary>Gets the Header.ItemLeft.Hot visual style element.</summary>
            public static VisualStyleElement Hot => CreateElement("HEADER", 2, 2);

            /// <summary>Gets the Header.ItemLeft.Pressed visual style element.</summary>
            public static VisualStyleElement Pressed => CreateElement("HEADER", 2, 3);
        }

        /// <summary>Provides the Header.ItemRight visual style elements.</summary>
        public static class ItemRight
        {
            /// <summary>Gets the Header.ItemRight.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("HEADER", 3, 1);

            /// <summary>Gets the Header.ItemRight.Hot visual style element.</summary>
            public static VisualStyleElement Hot => CreateElement("HEADER", 3, 2);

            /// <summary>Gets the Header.ItemRight.Pressed visual style element.</summary>
            public static VisualStyleElement Pressed => CreateElement("HEADER", 3, 3);
        }

        /// <summary>Provides the Header.SortArrow visual style elements.</summary>
        public static class SortArrow
        {
            /// <summary>Gets the Header.SortArrow.SortedUp visual style element.</summary>
            public static VisualStyleElement SortedUp => CreateElement("HEADER", 4, 1);

            /// <summary>Gets the Header.SortArrow.SortedDown visual style element.</summary>
            public static VisualStyleElement SortedDown => CreateElement("HEADER", 4, 2);
        }
    }

    /// <summary>Provides visual style elements for the ListView control.</summary>
    public static class ListView
    {
        /// <summary>Provides the ListView.Item visual style elements.</summary>
        public static class Item
        {
            /// <summary>Gets the ListView.Item.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("LISTVIEW", 1, 1);

            /// <summary>Gets the ListView.Item.Hot visual style element.</summary>
            public static VisualStyleElement Hot => CreateElement("LISTVIEW", 1, 2);

            /// <summary>Gets the ListView.Item.Selected visual style element.</summary>
            public static VisualStyleElement Selected => CreateElement("LISTVIEW", 1, 3);

            /// <summary>Gets the ListView.Item.Disabled visual style element.</summary>
            public static VisualStyleElement Disabled => CreateElement("LISTVIEW", 1, 4);

            /// <summary>Gets the ListView.Item.SelectedNotFocus visual style element.</summary>
            public static VisualStyleElement SelectedNotFocus => CreateElement("LISTVIEW", 1, 5);
        }

        /// <summary>Provides the ListView.Group visual style elements.</summary>
        public static class Group
        {
            /// <summary>Gets the ListView.Group.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("LISTVIEW", 2, 1);
        }

        /// <summary>Provides the ListView.Detail visual style elements.</summary>
        public static class Detail
        {
            /// <summary>Gets the ListView.Detail.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("LISTVIEW", 3, 1);
        }

        /// <summary>Provides the ListView.SortedDetail visual style elements.</summary>
        public static class SortedDetail
        {
            /// <summary>Gets the ListView.SortedDetail.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("LISTVIEW", 4, 1);
        }

        /// <summary>Provides the ListView.EmptyText visual style elements.</summary>
        public static class EmptyText
        {
            /// <summary>Gets the ListView.EmptyText.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("LISTVIEW", 5, 1);
        }
    }

    /// <summary>Provides visual style elements for the MenuBand control.</summary>
    public static class MenuBand
    {
        /// <summary>Provides the MenuBand.NewApplicationButton visual style elements.</summary>
        public static class NewApplicationButton
        {
            /// <summary>Gets the MenuBand.NewApplicationButton.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("MENUBAND", 1, 1);

            /// <summary>Gets the MenuBand.NewApplicationButton.Hot visual style element.</summary>
            public static VisualStyleElement Hot => CreateElement("MENUBAND", 1, 2);

            /// <summary>Gets the MenuBand.NewApplicationButton.Pressed visual style element.</summary>
            public static VisualStyleElement Pressed => CreateElement("MENUBAND", 1, 3);

            /// <summary>Gets the MenuBand.NewApplicationButton.Disabled visual style element.</summary>
            public static VisualStyleElement Disabled => CreateElement("MENUBAND", 1, 4);

            /// <summary>Gets the MenuBand.NewApplicationButton.Checked visual style element.</summary>
            public static VisualStyleElement Checked => CreateElement("MENUBAND", 1, 5);

            /// <summary>Gets the MenuBand.NewApplicationButton.HotChecked visual style element.</summary>
            public static VisualStyleElement HotChecked => CreateElement("MENUBAND", 1, 6);
        }

        /// <summary>Provides the MenuBand.Separator visual style elements.</summary>
        public static class Separator
        {
            /// <summary>Gets the MenuBand.Separator.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("MENUBAND", 2, 1);
        }
    }

    /// <summary>Provides visual style elements for the Menu control.</summary>
    public static class Menu
    {
        /// <summary>Provides the Menu.Item visual style elements.</summary>
        public static class Item
        {
            /// <summary>Gets the Menu.Item.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("MENU", 1, 1);

            /// <summary>Gets the Menu.Item.Selected visual style element.</summary>
            public static VisualStyleElement Selected => CreateElement("MENU", 1, 2);

            /// <summary>Gets the Menu.Item.Demoted visual style element.</summary>
            public static VisualStyleElement Demoted => CreateElement("MENU", 1, 3);
        }

        /// <summary>Provides the Menu.DropDown visual style elements.</summary>
        public static class DropDown
        {
            /// <summary>Gets the Menu.DropDown.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("MENU", 2, 1);
        }

        /// <summary>Provides the Menu.BarItem visual style elements.</summary>
        public static class BarItem
        {
            /// <summary>Gets the Menu.BarItem.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("MENU", 3, 1);
        }

        /// <summary>Provides the Menu.BarDropDown visual style elements.</summary>
        public static class BarDropDown
        {
            /// <summary>Gets the Menu.BarDropDown.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("MENU", 4, 1);
        }

        /// <summary>Provides the Menu.Chevron visual style elements.</summary>
        public static class Chevron
        {
            /// <summary>Gets the Menu.Chevron.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("MENU", 5, 1);
        }

        /// <summary>Provides the Menu.Separator visual style elements.</summary>
        public static class Separator
        {
            /// <summary>Gets the Menu.Separator.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("MENU", 6, 1);
        }
    }

    /// <summary>Provides visual style elements for the ProgressBar control.</summary>
    public static class ProgressBar
    {
        /// <summary>Provides the ProgressBar.Bar visual style elements.</summary>
        public static class Bar
        {
            /// <summary>Gets the ProgressBar.Bar.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("PROGRESS", 1, 1);
        }

        /// <summary>Provides the ProgressBar.BarVertical visual style elements.</summary>
        public static class BarVertical
        {
            /// <summary>Gets the ProgressBar.BarVertical.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("PROGRESS", 2, 1);
        }

        /// <summary>Provides the ProgressBar.Chunk visual style elements.</summary>
        public static class Chunk
        {
            /// <summary>Gets the ProgressBar.Chunk.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("PROGRESS", 3, 1);
        }

        /// <summary>Provides the ProgressBar.ChunkVertical visual style elements.</summary>
        public static class ChunkVertical
        {
            /// <summary>Gets the ProgressBar.ChunkVertical.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("PROGRESS", 4, 1);
        }
    }

    /// <summary>Provides visual style elements for the Rebar control.</summary>
    public static class Rebar
    {
        /// <summary>Provides the Rebar.Gripper visual style elements.</summary>
        public static class Gripper
        {
            /// <summary>Gets the Rebar.Gripper.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("REBAR", 1, 1);
        }

        /// <summary>Provides the Rebar.GripperVertical visual style elements.</summary>
        public static class GripperVertical
        {
            /// <summary>Gets the Rebar.GripperVertical.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("REBAR", 2, 1);
        }

        /// <summary>Provides the Rebar.Band visual style elements.</summary>
        public static class Band
        {
            /// <summary>Gets the Rebar.Band.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("REBAR", 3, 1);
        }

        /// <summary>Provides the Rebar.Chevron visual style elements.</summary>
        public static class Chevron
        {
            /// <summary>Gets the Rebar.Chevron.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("REBAR", 4, 1);

            /// <summary>Gets the Rebar.Chevron.Hot visual style element.</summary>
            public static VisualStyleElement Hot => CreateElement("REBAR", 4, 2);

            /// <summary>Gets the Rebar.Chevron.Pressed visual style element.</summary>
            public static VisualStyleElement Pressed => CreateElement("REBAR", 4, 3);
        }

        /// <summary>Provides the Rebar.ChevronVertical visual style elements.</summary>
        public static class ChevronVertical
        {
            /// <summary>Gets the Rebar.ChevronVertical.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("REBAR", 5, 1);

            /// <summary>Gets the Rebar.ChevronVertical.Hot visual style element.</summary>
            public static VisualStyleElement Hot => CreateElement("REBAR", 5, 2);

            /// <summary>Gets the Rebar.ChevronVertical.Pressed visual style element.</summary>
            public static VisualStyleElement Pressed => CreateElement("REBAR", 5, 3);
        }
    }

    /// <summary>Provides visual style elements for the StartPanel control.</summary>
    public static class StartPanel
    {
        /// <summary>Provides the StartPanel.UserPane visual style elements.</summary>
        public static class UserPane
        {
            /// <summary>Gets the StartPanel.UserPane.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("STARTPANEL", 1, 1);
        }

        /// <summary>Provides the StartPanel.MorePrograms visual style elements.</summary>
        public static class MorePrograms
        {
            /// <summary>Gets the StartPanel.MorePrograms.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("STARTPANEL", 2, 1);
        }

        /// <summary>Provides the StartPanel.MoreProgramsArrow visual style elements.</summary>
        public static class MoreProgramsArrow
        {
            /// <summary>Gets the StartPanel.MoreProgramsArrow.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("STARTPANEL", 3, 1);

            /// <summary>Gets the StartPanel.MoreProgramsArrow.Hot visual style element.</summary>
            public static VisualStyleElement Hot => CreateElement("STARTPANEL", 3, 2);

            /// <summary>Gets the StartPanel.MoreProgramsArrow.Pressed visual style element.</summary>
            public static VisualStyleElement Pressed => CreateElement("STARTPANEL", 3, 3);
        }

        /// <summary>Provides the StartPanel.ProgList visual style elements.</summary>
        public static class ProgList
        {
            /// <summary>Gets the StartPanel.ProgList.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("STARTPANEL", 4, 1);
        }

        /// <summary>Provides the StartPanel.ProgListSeparator visual style elements.</summary>
        public static class ProgListSeparator
        {
            /// <summary>Gets the StartPanel.ProgListSeparator.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("STARTPANEL", 5, 1);
        }

        /// <summary>Provides the StartPanel.PlaceList visual style elements.</summary>
        public static class PlaceList
        {
            /// <summary>Gets the StartPanel.PlaceList.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("STARTPANEL", 6, 1);
        }

        /// <summary>Provides the StartPanel.PlaceListSeparator visual style elements.</summary>
        public static class PlaceListSeparator
        {
            /// <summary>Gets the StartPanel.PlaceListSeparator.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("STARTPANEL", 7, 1);
        }

        /// <summary>Provides the StartPanel.LogOff visual style elements.</summary>
        public static class LogOff
        {
            /// <summary>Gets the StartPanel.LogOff.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("STARTPANEL", 8, 1);
        }

        /// <summary>Provides the StartPanel.LogOffButtons visual style elements.</summary>
        public static class LogOffButtons
        {
            /// <summary>Gets the StartPanel.LogOffButtons.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("STARTPANEL", 9, 1);

            /// <summary>Gets the StartPanel.LogOffButtons.Hot visual style element.</summary>
            public static VisualStyleElement Hot => CreateElement("STARTPANEL", 9, 2);

            /// <summary>Gets the StartPanel.LogOffButtons.Pressed visual style element.</summary>
            public static VisualStyleElement Pressed => CreateElement("STARTPANEL", 9, 3);
        }

        /// <summary>Provides the StartPanel.UserPicture visual style elements.</summary>
        public static class UserPicture
        {
            /// <summary>Gets the StartPanel.UserPicture.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("STARTPANEL", 10, 1);
        }

        /// <summary>Provides the StartPanel.Preview visual style elements.</summary>
        public static class Preview
        {
            /// <summary>Gets the StartPanel.Preview.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("STARTPANEL", 11, 1);
        }
    }

    /// <summary>Provides visual style elements for the Status control.</summary>
    public static class Status
    {
        /// <summary>Provides the Status.Bar visual style elements.</summary>
        public static class Bar
        {
            /// <summary>Gets the Status.Bar.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("STATUS", 1, 1);
        }

        /// <summary>Provides the Status.Pane visual style elements.</summary>
        public static class Pane
        {
            /// <summary>Gets the Status.Pane.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("STATUS", 2, 1);
        }

        /// <summary>Provides the Status.GripperPane visual style elements.</summary>
        public static class GripperPane
        {
            /// <summary>Gets the Status.GripperPane.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("STATUS", 3, 1);
        }

        /// <summary>Provides the Status.Gripper visual style elements.</summary>
        public static class Gripper
        {
            /// <summary>Gets the Status.Gripper.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("STATUS", 4, 1);
        }
    }

    /// <summary>Provides visual style elements for the TaskBand control.</summary>
    public static class TaskBand
    {
        /// <summary>Provides the TaskBand.GroupCount visual style elements.</summary>
        public static class GroupCount
        {
            /// <summary>Gets the TaskBand.GroupCount.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("TASKBAND", 1, 1);
        }

        /// <summary>Provides the TaskBand.FlashButton visual style elements.</summary>
        public static class FlashButton
        {
            /// <summary>Gets the TaskBand.FlashButton.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("TASKBAND", 2, 1);
        }

        /// <summary>Provides the TaskBand.FlashButtonGroupMenu visual style elements.</summary>
        public static class FlashButtonGroupMenu
        {
            /// <summary>Gets the TaskBand.FlashButtonGroupMenu.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("TASKBAND", 3, 1);
        }
    }

    /// <summary>Provides visual style elements for the TaskbarClock control.</summary>
    public static class TaskbarClock
    {
        /// <summary>Provides the TaskbarClock.Time visual style elements.</summary>
        public static class Time
        {
            /// <summary>Gets the TaskbarClock.Time.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("CLOCK", 1, 1);
        }
    }

    /// <summary>Provides visual style elements for the Taskbar control.</summary>
    public static class Taskbar
    {
        /// <summary>Provides the Taskbar.BackgroundBottom visual style elements.</summary>
        public static class BackgroundBottom
        {
            /// <summary>Gets the Taskbar.BackgroundBottom.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("TASKBAR", 1, 1);
        }

        /// <summary>Provides the Taskbar.BackgroundRight visual style elements.</summary>
        public static class BackgroundRight
        {
            /// <summary>Gets the Taskbar.BackgroundRight.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("TASKBAR", 2, 1);
        }

        /// <summary>Provides the Taskbar.BackgroundTop visual style elements.</summary>
        public static class BackgroundTop
        {
            /// <summary>Gets the Taskbar.BackgroundTop.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("TASKBAR", 3, 1);
        }

        /// <summary>Provides the Taskbar.BackgroundLeft visual style elements.</summary>
        public static class BackgroundLeft
        {
            /// <summary>Gets the Taskbar.BackgroundLeft.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("TASKBAR", 4, 1);
        }

        /// <summary>Provides the Taskbar.SizingBarBottom visual style elements.</summary>
        public static class SizingBarBottom
        {
            /// <summary>Gets the Taskbar.SizingBarBottom.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("TASKBAR", 5, 1);
        }

        /// <summary>Provides the Taskbar.SizingBarRight visual style elements.</summary>
        public static class SizingBarRight
        {
            /// <summary>Gets the Taskbar.SizingBarRight.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("TASKBAR", 6, 1);
        }

        /// <summary>Provides the Taskbar.SizingBarTop visual style elements.</summary>
        public static class SizingBarTop
        {
            /// <summary>Gets the Taskbar.SizingBarTop.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("TASKBAR", 7, 1);
        }

        /// <summary>Provides the Taskbar.SizingBarLeft visual style elements.</summary>
        public static class SizingBarLeft
        {
            /// <summary>Gets the Taskbar.SizingBarLeft.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("TASKBAR", 8, 1);
        }
    }

    /// <summary>Provides visual style elements for the ToolBar control.</summary>
    public static class ToolBar
    {
        /// <summary>Provides the ToolBar.Button visual style elements.</summary>
        public static class Button
        {
            /// <summary>Gets the ToolBar.Button.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("TOOLBAR", 1, 1);

            /// <summary>Gets the ToolBar.Button.Hot visual style element.</summary>
            public static VisualStyleElement Hot => CreateElement("TOOLBAR", 1, 2);

            /// <summary>Gets the ToolBar.Button.Pressed visual style element.</summary>
            public static VisualStyleElement Pressed => CreateElement("TOOLBAR", 1, 3);

            /// <summary>Gets the ToolBar.Button.Disabled visual style element.</summary>
            public static VisualStyleElement Disabled => CreateElement("TOOLBAR", 1, 4);

            /// <summary>Gets the ToolBar.Button.Checked visual style element.</summary>
            public static VisualStyleElement Checked => CreateElement("TOOLBAR", 1, 5);

            /// <summary>Gets the ToolBar.Button.HotChecked visual style element.</summary>
            public static VisualStyleElement HotChecked => CreateElement("TOOLBAR", 1, 6);
        }

        /// <summary>Provides the ToolBar.DropDownButton visual style elements.</summary>
        public static class DropDownButton
        {
            /// <summary>Gets the ToolBar.DropDownButton.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("TOOLBAR", 2, 1);

            /// <summary>Gets the ToolBar.DropDownButton.Hot visual style element.</summary>
            public static VisualStyleElement Hot => CreateElement("TOOLBAR", 2, 2);

            /// <summary>Gets the ToolBar.DropDownButton.Pressed visual style element.</summary>
            public static VisualStyleElement Pressed => CreateElement("TOOLBAR", 2, 3);

            /// <summary>Gets the ToolBar.DropDownButton.Disabled visual style element.</summary>
            public static VisualStyleElement Disabled => CreateElement("TOOLBAR", 2, 4);

            /// <summary>Gets the ToolBar.DropDownButton.Checked visual style element.</summary>
            public static VisualStyleElement Checked => CreateElement("TOOLBAR", 2, 5);

            /// <summary>Gets the ToolBar.DropDownButton.HotChecked visual style element.</summary>
            public static VisualStyleElement HotChecked => CreateElement("TOOLBAR", 2, 6);
        }

        /// <summary>Provides the ToolBar.SplitButton visual style elements.</summary>
        public static class SplitButton
        {
            /// <summary>Gets the ToolBar.SplitButton.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("TOOLBAR", 3, 1);

            /// <summary>Gets the ToolBar.SplitButton.Hot visual style element.</summary>
            public static VisualStyleElement Hot => CreateElement("TOOLBAR", 3, 2);

            /// <summary>Gets the ToolBar.SplitButton.Pressed visual style element.</summary>
            public static VisualStyleElement Pressed => CreateElement("TOOLBAR", 3, 3);

            /// <summary>Gets the ToolBar.SplitButton.Disabled visual style element.</summary>
            public static VisualStyleElement Disabled => CreateElement("TOOLBAR", 3, 4);

            /// <summary>Gets the ToolBar.SplitButton.Checked visual style element.</summary>
            public static VisualStyleElement Checked => CreateElement("TOOLBAR", 3, 5);

            /// <summary>Gets the ToolBar.SplitButton.HotChecked visual style element.</summary>
            public static VisualStyleElement HotChecked => CreateElement("TOOLBAR", 3, 6);
        }

        /// <summary>Provides the ToolBar.SplitButtonDropDown visual style elements.</summary>
        public static class SplitButtonDropDown
        {
            /// <summary>Gets the ToolBar.SplitButtonDropDown.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("TOOLBAR", 4, 1);

            /// <summary>Gets the ToolBar.SplitButtonDropDown.Hot visual style element.</summary>
            public static VisualStyleElement Hot => CreateElement("TOOLBAR", 4, 2);

            /// <summary>Gets the ToolBar.SplitButtonDropDown.Pressed visual style element.</summary>
            public static VisualStyleElement Pressed => CreateElement("TOOLBAR", 4, 3);

            /// <summary>Gets the ToolBar.SplitButtonDropDown.Disabled visual style element.</summary>
            public static VisualStyleElement Disabled => CreateElement("TOOLBAR", 4, 4);

            /// <summary>Gets the ToolBar.SplitButtonDropDown.Checked visual style element.</summary>
            public static VisualStyleElement Checked => CreateElement("TOOLBAR", 4, 5);

            /// <summary>Gets the ToolBar.SplitButtonDropDown.HotChecked visual style element.</summary>
            public static VisualStyleElement HotChecked => CreateElement("TOOLBAR", 4, 6);
        }

        /// <summary>Provides the ToolBar.SeparatorHorizontal visual style elements.</summary>
        public static class SeparatorHorizontal
        {
            /// <summary>Gets the ToolBar.SeparatorHorizontal.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("TOOLBAR", 5, 1);
        }

        /// <summary>Provides the ToolBar.SeparatorVertical visual style elements.</summary>
        public static class SeparatorVertical
        {
            /// <summary>Gets the ToolBar.SeparatorVertical.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("TOOLBAR", 6, 1);
        }
    }

    /// <summary>Provides visual style elements for the ToolTip control.</summary>
    public static class ToolTip
    {
        /// <summary>Provides the ToolTip.Standard visual style elements.</summary>
        public static class Standard
        {
            /// <summary>Gets the ToolTip.Standard.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("TOOLTIP", 1, 1);

            /// <summary>Gets the ToolTip.Standard.Link visual style element.</summary>
            public static VisualStyleElement Link => CreateElement("TOOLTIP", 1, 2);
        }

        /// <summary>Provides the ToolTip.StandardTitle visual style elements.</summary>
        public static class StandardTitle
        {
            /// <summary>Gets the ToolTip.StandardTitle.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("TOOLTIP", 2, 1);
        }

        /// <summary>Provides the ToolTip.Balloon visual style elements.</summary>
        public static class Balloon
        {
            /// <summary>Gets the ToolTip.Balloon.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("TOOLTIP", 3, 1);

            /// <summary>Gets the ToolTip.Balloon.Link visual style element.</summary>
            public static VisualStyleElement Link => CreateElement("TOOLTIP", 3, 2);
        }

        /// <summary>Provides the ToolTip.BalloonTitle visual style elements.</summary>
        public static class BalloonTitle
        {
            /// <summary>Gets the ToolTip.BalloonTitle.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("TOOLTIP", 4, 1);
        }

        /// <summary>Provides the ToolTip.Close visual style elements.</summary>
        public static class Close
        {
            /// <summary>Gets the ToolTip.Close.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("TOOLTIP", 5, 1);

            /// <summary>Gets the ToolTip.Close.Hot visual style element.</summary>
            public static VisualStyleElement Hot => CreateElement("TOOLTIP", 5, 2);

            /// <summary>Gets the ToolTip.Close.Pressed visual style element.</summary>
            public static VisualStyleElement Pressed => CreateElement("TOOLTIP", 5, 3);
        }
    }

    /// <summary>Provides visual style elements for the TrackBar control.</summary>
    public static class TrackBar
    {
        /// <summary>Provides the TrackBar.Track visual style elements.</summary>
        public static class Track
        {
            /// <summary>Gets the TrackBar.Track.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("TRACKBAR", 1, 1);
        }

        /// <summary>Provides the TrackBar.TrackVertical visual style elements.</summary>
        public static class TrackVertical
        {
            /// <summary>Gets the TrackBar.TrackVertical.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("TRACKBAR", 2, 1);
        }

        /// <summary>Provides the TrackBar.Thumb visual style elements.</summary>
        public static class Thumb
        {
            /// <summary>Gets the TrackBar.Thumb.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("TRACKBAR", 3, 1);

            /// <summary>Gets the TrackBar.Thumb.Hot visual style element.</summary>
            public static VisualStyleElement Hot => CreateElement("TRACKBAR", 3, 2);

            /// <summary>Gets the TrackBar.Thumb.Pressed visual style element.</summary>
            public static VisualStyleElement Pressed => CreateElement("TRACKBAR", 3, 3);

            /// <summary>Gets the TrackBar.Thumb.Focused visual style element.</summary>
            public static VisualStyleElement Focused => CreateElement("TRACKBAR", 3, 4);

            /// <summary>Gets the TrackBar.Thumb.Disabled visual style element.</summary>
            public static VisualStyleElement Disabled => CreateElement("TRACKBAR", 3, 5);
        }

        /// <summary>Provides the TrackBar.ThumbBottom visual style elements.</summary>
        public static class ThumbBottom
        {
            /// <summary>Gets the TrackBar.ThumbBottom.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("TRACKBAR", 4, 1);

            /// <summary>Gets the TrackBar.ThumbBottom.Hot visual style element.</summary>
            public static VisualStyleElement Hot => CreateElement("TRACKBAR", 4, 2);

            /// <summary>Gets the TrackBar.ThumbBottom.Pressed visual style element.</summary>
            public static VisualStyleElement Pressed => CreateElement("TRACKBAR", 4, 3);

            /// <summary>Gets the TrackBar.ThumbBottom.Focused visual style element.</summary>
            public static VisualStyleElement Focused => CreateElement("TRACKBAR", 4, 4);

            /// <summary>Gets the TrackBar.ThumbBottom.Disabled visual style element.</summary>
            public static VisualStyleElement Disabled => CreateElement("TRACKBAR", 4, 5);
        }

        /// <summary>Provides the TrackBar.ThumbTop visual style elements.</summary>
        public static class ThumbTop
        {
            /// <summary>Gets the TrackBar.ThumbTop.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("TRACKBAR", 5, 1);

            /// <summary>Gets the TrackBar.ThumbTop.Hot visual style element.</summary>
            public static VisualStyleElement Hot => CreateElement("TRACKBAR", 5, 2);

            /// <summary>Gets the TrackBar.ThumbTop.Pressed visual style element.</summary>
            public static VisualStyleElement Pressed => CreateElement("TRACKBAR", 5, 3);

            /// <summary>Gets the TrackBar.ThumbTop.Focused visual style element.</summary>
            public static VisualStyleElement Focused => CreateElement("TRACKBAR", 5, 4);

            /// <summary>Gets the TrackBar.ThumbTop.Disabled visual style element.</summary>
            public static VisualStyleElement Disabled => CreateElement("TRACKBAR", 5, 5);
        }

        /// <summary>Provides the TrackBar.ThumbVertical visual style elements.</summary>
        public static class ThumbVertical
        {
            /// <summary>Gets the TrackBar.ThumbVertical.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("TRACKBAR", 6, 1);

            /// <summary>Gets the TrackBar.ThumbVertical.Hot visual style element.</summary>
            public static VisualStyleElement Hot => CreateElement("TRACKBAR", 6, 2);

            /// <summary>Gets the TrackBar.ThumbVertical.Pressed visual style element.</summary>
            public static VisualStyleElement Pressed => CreateElement("TRACKBAR", 6, 3);

            /// <summary>Gets the TrackBar.ThumbVertical.Focused visual style element.</summary>
            public static VisualStyleElement Focused => CreateElement("TRACKBAR", 6, 4);

            /// <summary>Gets the TrackBar.ThumbVertical.Disabled visual style element.</summary>
            public static VisualStyleElement Disabled => CreateElement("TRACKBAR", 6, 5);
        }

        /// <summary>Provides the TrackBar.ThumbLeft visual style elements.</summary>
        public static class ThumbLeft
        {
            /// <summary>Gets the TrackBar.ThumbLeft.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("TRACKBAR", 7, 1);

            /// <summary>Gets the TrackBar.ThumbLeft.Hot visual style element.</summary>
            public static VisualStyleElement Hot => CreateElement("TRACKBAR", 7, 2);

            /// <summary>Gets the TrackBar.ThumbLeft.Pressed visual style element.</summary>
            public static VisualStyleElement Pressed => CreateElement("TRACKBAR", 7, 3);

            /// <summary>Gets the TrackBar.ThumbLeft.Focused visual style element.</summary>
            public static VisualStyleElement Focused => CreateElement("TRACKBAR", 7, 4);

            /// <summary>Gets the TrackBar.ThumbLeft.Disabled visual style element.</summary>
            public static VisualStyleElement Disabled => CreateElement("TRACKBAR", 7, 5);
        }

        /// <summary>Provides the TrackBar.ThumbRight visual style elements.</summary>
        public static class ThumbRight
        {
            /// <summary>Gets the TrackBar.ThumbRight.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("TRACKBAR", 8, 1);

            /// <summary>Gets the TrackBar.ThumbRight.Hot visual style element.</summary>
            public static VisualStyleElement Hot => CreateElement("TRACKBAR", 8, 2);

            /// <summary>Gets the TrackBar.ThumbRight.Pressed visual style element.</summary>
            public static VisualStyleElement Pressed => CreateElement("TRACKBAR", 8, 3);

            /// <summary>Gets the TrackBar.ThumbRight.Focused visual style element.</summary>
            public static VisualStyleElement Focused => CreateElement("TRACKBAR", 8, 4);

            /// <summary>Gets the TrackBar.ThumbRight.Disabled visual style element.</summary>
            public static VisualStyleElement Disabled => CreateElement("TRACKBAR", 8, 5);
        }

        /// <summary>Provides the TrackBar.Ticks visual style elements.</summary>
        public static class Ticks
        {
            /// <summary>Gets the TrackBar.Ticks.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("TRACKBAR", 9, 1);
        }

        /// <summary>Provides the TrackBar.TicksVertical visual style elements.</summary>
        public static class TicksVertical
        {
            /// <summary>Gets the TrackBar.TicksVertical.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("TRACKBAR", 10, 1);
        }
    }

    /// <summary>Provides visual style elements for the TreeView control.</summary>
    public static class TreeView
    {
        /// <summary>Provides the TreeView.Item visual style elements.</summary>
        public static class Item
        {
            /// <summary>Gets the TreeView.Item.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("TREEVIEW", 1, 1);

            /// <summary>Gets the TreeView.Item.Hot visual style element.</summary>
            public static VisualStyleElement Hot => CreateElement("TREEVIEW", 1, 2);

            /// <summary>Gets the TreeView.Item.Selected visual style element.</summary>
            public static VisualStyleElement Selected => CreateElement("TREEVIEW", 1, 3);

            /// <summary>Gets the TreeView.Item.Disabled visual style element.</summary>
            public static VisualStyleElement Disabled => CreateElement("TREEVIEW", 1, 4);

            /// <summary>Gets the TreeView.Item.SelectedNotFocus visual style element.</summary>
            public static VisualStyleElement SelectedNotFocus => CreateElement("TREEVIEW", 1, 5);
        }

        /// <summary>Provides the TreeView.Glyph visual style elements.</summary>
        public static class Glyph
        {
            /// <summary>Gets the TreeView.Glyph.Closed visual style element.</summary>
            public static VisualStyleElement Closed => CreateElement("TREEVIEW", 2, 1);

            /// <summary>Gets the TreeView.Glyph.Opened visual style element.</summary>
            public static VisualStyleElement Opened => CreateElement("TREEVIEW", 2, 2);
        }

        /// <summary>Provides the TreeView.Branch visual style elements.</summary>
        public static class Branch
        {
            /// <summary>Gets the TreeView.Branch.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("TREEVIEW", 3, 1);
        }
    }

    /// <summary>Provides visual style elements for the TextBox control.</summary>
    public static class TextBox
    {
        /// <summary>Provides the TextBox.TextEdit visual style elements.</summary>
        public static class TextEdit
        {
            /// <summary>Gets the TextBox.TextEdit.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("EDIT", 1, 1);

            /// <summary>Gets the TextBox.TextEdit.Hot visual style element.</summary>
            public static VisualStyleElement Hot => CreateElement("EDIT", 1, 2);

            /// <summary>Gets the TextBox.TextEdit.Selected visual style element.</summary>
            public static VisualStyleElement Selected => CreateElement("EDIT", 1, 3);

            /// <summary>Gets the TextBox.TextEdit.Disabled visual style element.</summary>
            public static VisualStyleElement Disabled => CreateElement("EDIT", 1, 4);

            /// <summary>Gets the TextBox.TextEdit.Focused visual style element.</summary>
            public static VisualStyleElement Focused => CreateElement("EDIT", 1, 5);

            /// <summary>Gets the TextBox.TextEdit.ReadOnly visual style element.</summary>
            public static VisualStyleElement ReadOnly => CreateElement("EDIT", 1, 6);

            /// <summary>Gets the TextBox.TextEdit.Assist visual style element.</summary>
            public static VisualStyleElement Assist => CreateElement("EDIT", 1, 7);
        }

        /// <summary>Provides the TextBox.Caret visual style elements.</summary>
        public static class Caret
        {
            /// <summary>Gets the TextBox.Caret.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("EDIT", 2, 1);
        }
    }

    /// <summary>Provides visual style elements for the TrayNotify control.</summary>
    public static class TrayNotify
    {
        /// <summary>Provides the TrayNotify.Background visual style elements.</summary>
        public static class Background
        {
            /// <summary>Gets the TrayNotify.Background.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("TRAYNOTIFY", 1, 1);
        }

        /// <summary>Provides the TrayNotify.AnimateBackground visual style elements.</summary>
        public static class AnimateBackground
        {
            /// <summary>Gets the TrayNotify.AnimateBackground.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("TRAYNOTIFY", 2, 1);
        }
    }

    /// <summary>Provides visual style elements for the Window control.</summary>
    public static class Window
    {
        /// <summary>Provides the Window.Caption visual style elements.</summary>
        public static class Caption
        {
            /// <summary>Gets the Window.Caption.Active visual style element.</summary>
            public static VisualStyleElement Active => CreateElement("WINDOW", 1, 1);

            /// <summary>Gets the Window.Caption.Inactive visual style element.</summary>
            public static VisualStyleElement Inactive => CreateElement("WINDOW", 1, 2);

            /// <summary>Gets the Window.Caption.Disabled visual style element.</summary>
            public static VisualStyleElement Disabled => CreateElement("WINDOW", 1, 3);
        }

        /// <summary>Provides the Window.SmallCaption visual style elements.</summary>
        public static class SmallCaption
        {
            /// <summary>Gets the Window.SmallCaption.Active visual style element.</summary>
            public static VisualStyleElement Active => CreateElement("WINDOW", 2, 1);

            /// <summary>Gets the Window.SmallCaption.Inactive visual style element.</summary>
            public static VisualStyleElement Inactive => CreateElement("WINDOW", 2, 2);

            /// <summary>Gets the Window.SmallCaption.Disabled visual style element.</summary>
            public static VisualStyleElement Disabled => CreateElement("WINDOW", 2, 3);
        }

        /// <summary>Provides the Window.MinCaption visual style elements.</summary>
        public static class MinCaption
        {
            /// <summary>Gets the Window.MinCaption.Active visual style element.</summary>
            public static VisualStyleElement Active => CreateElement("WINDOW", 3, 1);

            /// <summary>Gets the Window.MinCaption.Inactive visual style element.</summary>
            public static VisualStyleElement Inactive => CreateElement("WINDOW", 3, 2);

            /// <summary>Gets the Window.MinCaption.Disabled visual style element.</summary>
            public static VisualStyleElement Disabled => CreateElement("WINDOW", 3, 3);
        }

        /// <summary>Provides the Window.SmallMinCaption visual style elements.</summary>
        public static class SmallMinCaption
        {
            /// <summary>Gets the Window.SmallMinCaption.Active visual style element.</summary>
            public static VisualStyleElement Active => CreateElement("WINDOW", 4, 1);

            /// <summary>Gets the Window.SmallMinCaption.Inactive visual style element.</summary>
            public static VisualStyleElement Inactive => CreateElement("WINDOW", 4, 2);

            /// <summary>Gets the Window.SmallMinCaption.Disabled visual style element.</summary>
            public static VisualStyleElement Disabled => CreateElement("WINDOW", 4, 3);
        }

        /// <summary>Provides the Window.MaxCaption visual style elements.</summary>
        public static class MaxCaption
        {
            /// <summary>Gets the Window.MaxCaption.Active visual style element.</summary>
            public static VisualStyleElement Active => CreateElement("WINDOW", 5, 1);

            /// <summary>Gets the Window.MaxCaption.Inactive visual style element.</summary>
            public static VisualStyleElement Inactive => CreateElement("WINDOW", 5, 2);

            /// <summary>Gets the Window.MaxCaption.Disabled visual style element.</summary>
            public static VisualStyleElement Disabled => CreateElement("WINDOW", 5, 3);
        }

        /// <summary>Provides the Window.SmallMaxCaption visual style elements.</summary>
        public static class SmallMaxCaption
        {
            /// <summary>Gets the Window.SmallMaxCaption.Active visual style element.</summary>
            public static VisualStyleElement Active => CreateElement("WINDOW", 6, 1);

            /// <summary>Gets the Window.SmallMaxCaption.Inactive visual style element.</summary>
            public static VisualStyleElement Inactive => CreateElement("WINDOW", 6, 2);

            /// <summary>Gets the Window.SmallMaxCaption.Disabled visual style element.</summary>
            public static VisualStyleElement Disabled => CreateElement("WINDOW", 6, 3);
        }

        /// <summary>Provides the Window.FrameLeft visual style elements.</summary>
        public static class FrameLeft
        {
            /// <summary>Gets the Window.FrameLeft.Active visual style element.</summary>
            public static VisualStyleElement Active => CreateElement("WINDOW", 7, 1);

            /// <summary>Gets the Window.FrameLeft.Inactive visual style element.</summary>
            public static VisualStyleElement Inactive => CreateElement("WINDOW", 7, 2);
        }

        /// <summary>Provides the Window.FrameRight visual style elements.</summary>
        public static class FrameRight
        {
            /// <summary>Gets the Window.FrameRight.Active visual style element.</summary>
            public static VisualStyleElement Active => CreateElement("WINDOW", 8, 1);

            /// <summary>Gets the Window.FrameRight.Inactive visual style element.</summary>
            public static VisualStyleElement Inactive => CreateElement("WINDOW", 8, 2);
        }

        /// <summary>Provides the Window.FrameBottom visual style elements.</summary>
        public static class FrameBottom
        {
            /// <summary>Gets the Window.FrameBottom.Active visual style element.</summary>
            public static VisualStyleElement Active => CreateElement("WINDOW", 9, 1);

            /// <summary>Gets the Window.FrameBottom.Inactive visual style element.</summary>
            public static VisualStyleElement Inactive => CreateElement("WINDOW", 9, 2);
        }

        /// <summary>Provides the Window.SmallFrameLeft visual style elements.</summary>
        public static class SmallFrameLeft
        {
            /// <summary>Gets the Window.SmallFrameLeft.Active visual style element.</summary>
            public static VisualStyleElement Active => CreateElement("WINDOW", 10, 1);

            /// <summary>Gets the Window.SmallFrameLeft.Inactive visual style element.</summary>
            public static VisualStyleElement Inactive => CreateElement("WINDOW", 10, 2);
        }

        /// <summary>Provides the Window.SmallFrameRight visual style elements.</summary>
        public static class SmallFrameRight
        {
            /// <summary>Gets the Window.SmallFrameRight.Active visual style element.</summary>
            public static VisualStyleElement Active => CreateElement("WINDOW", 11, 1);

            /// <summary>Gets the Window.SmallFrameRight.Inactive visual style element.</summary>
            public static VisualStyleElement Inactive => CreateElement("WINDOW", 11, 2);
        }

        /// <summary>Provides the Window.SmallFrameBottom visual style elements.</summary>
        public static class SmallFrameBottom
        {
            /// <summary>Gets the Window.SmallFrameBottom.Active visual style element.</summary>
            public static VisualStyleElement Active => CreateElement("WINDOW", 12, 1);

            /// <summary>Gets the Window.SmallFrameBottom.Inactive visual style element.</summary>
            public static VisualStyleElement Inactive => CreateElement("WINDOW", 12, 2);
        }

        /// <summary>Provides the Window.SysButton visual style elements.</summary>
        public static class SysButton
        {
            /// <summary>Gets the Window.SysButton.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("WINDOW", 13, 1);

            /// <summary>Gets the Window.SysButton.Hot visual style element.</summary>
            public static VisualStyleElement Hot => CreateElement("WINDOW", 13, 2);

            /// <summary>Gets the Window.SysButton.Pressed visual style element.</summary>
            public static VisualStyleElement Pressed => CreateElement("WINDOW", 13, 3);

            /// <summary>Gets the Window.SysButton.Disabled visual style element.</summary>
            public static VisualStyleElement Disabled => CreateElement("WINDOW", 13, 4);
        }

        /// <summary>Provides the Window.MdiSysButton visual style elements.</summary>
        public static class MdiSysButton
        {
            /// <summary>Gets the Window.MdiSysButton.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("WINDOW", 14, 1);

            /// <summary>Gets the Window.MdiSysButton.Hot visual style element.</summary>
            public static VisualStyleElement Hot => CreateElement("WINDOW", 14, 2);

            /// <summary>Gets the Window.MdiSysButton.Pressed visual style element.</summary>
            public static VisualStyleElement Pressed => CreateElement("WINDOW", 14, 3);

            /// <summary>Gets the Window.MdiSysButton.Disabled visual style element.</summary>
            public static VisualStyleElement Disabled => CreateElement("WINDOW", 14, 4);
        }

        /// <summary>Provides the Window.MinButton visual style elements.</summary>
        public static class MinButton
        {
            /// <summary>Gets the Window.MinButton.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("WINDOW", 15, 1);

            /// <summary>Gets the Window.MinButton.Hot visual style element.</summary>
            public static VisualStyleElement Hot => CreateElement("WINDOW", 15, 2);

            /// <summary>Gets the Window.MinButton.Pressed visual style element.</summary>
            public static VisualStyleElement Pressed => CreateElement("WINDOW", 15, 3);

            /// <summary>Gets the Window.MinButton.Disabled visual style element.</summary>
            public static VisualStyleElement Disabled => CreateElement("WINDOW", 15, 4);
        }

        /// <summary>Provides the Window.MdiMinButton visual style elements.</summary>
        public static class MdiMinButton
        {
            /// <summary>Gets the Window.MdiMinButton.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("WINDOW", 16, 1);

            /// <summary>Gets the Window.MdiMinButton.Hot visual style element.</summary>
            public static VisualStyleElement Hot => CreateElement("WINDOW", 16, 2);

            /// <summary>Gets the Window.MdiMinButton.Pressed visual style element.</summary>
            public static VisualStyleElement Pressed => CreateElement("WINDOW", 16, 3);

            /// <summary>Gets the Window.MdiMinButton.Disabled visual style element.</summary>
            public static VisualStyleElement Disabled => CreateElement("WINDOW", 16, 4);
        }

        /// <summary>Provides the Window.MaxButton visual style elements.</summary>
        public static class MaxButton
        {
            /// <summary>Gets the Window.MaxButton.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("WINDOW", 17, 1);

            /// <summary>Gets the Window.MaxButton.Hot visual style element.</summary>
            public static VisualStyleElement Hot => CreateElement("WINDOW", 17, 2);

            /// <summary>Gets the Window.MaxButton.Pressed visual style element.</summary>
            public static VisualStyleElement Pressed => CreateElement("WINDOW", 17, 3);

            /// <summary>Gets the Window.MaxButton.Disabled visual style element.</summary>
            public static VisualStyleElement Disabled => CreateElement("WINDOW", 17, 4);
        }

        /// <summary>Provides the Window.CloseButton visual style elements.</summary>
        public static class CloseButton
        {
            /// <summary>Gets the Window.CloseButton.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("WINDOW", 18, 1);

            /// <summary>Gets the Window.CloseButton.Hot visual style element.</summary>
            public static VisualStyleElement Hot => CreateElement("WINDOW", 18, 2);

            /// <summary>Gets the Window.CloseButton.Pressed visual style element.</summary>
            public static VisualStyleElement Pressed => CreateElement("WINDOW", 18, 3);

            /// <summary>Gets the Window.CloseButton.Disabled visual style element.</summary>
            public static VisualStyleElement Disabled => CreateElement("WINDOW", 18, 4);
        }

        /// <summary>Provides the Window.SmallCloseButton visual style elements.</summary>
        public static class SmallCloseButton
        {
            /// <summary>Gets the Window.SmallCloseButton.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("WINDOW", 19, 1);

            /// <summary>Gets the Window.SmallCloseButton.Hot visual style element.</summary>
            public static VisualStyleElement Hot => CreateElement("WINDOW", 19, 2);

            /// <summary>Gets the Window.SmallCloseButton.Pressed visual style element.</summary>
            public static VisualStyleElement Pressed => CreateElement("WINDOW", 19, 3);

            /// <summary>Gets the Window.SmallCloseButton.Disabled visual style element.</summary>
            public static VisualStyleElement Disabled => CreateElement("WINDOW", 19, 4);
        }

        /// <summary>Provides the Window.MdiCloseButton visual style elements.</summary>
        public static class MdiCloseButton
        {
            /// <summary>Gets the Window.MdiCloseButton.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("WINDOW", 20, 1);

            /// <summary>Gets the Window.MdiCloseButton.Hot visual style element.</summary>
            public static VisualStyleElement Hot => CreateElement("WINDOW", 20, 2);

            /// <summary>Gets the Window.MdiCloseButton.Pressed visual style element.</summary>
            public static VisualStyleElement Pressed => CreateElement("WINDOW", 20, 3);

            /// <summary>Gets the Window.MdiCloseButton.Disabled visual style element.</summary>
            public static VisualStyleElement Disabled => CreateElement("WINDOW", 20, 4);
        }

        /// <summary>Provides the Window.RestoreButton visual style elements.</summary>
        public static class RestoreButton
        {
            /// <summary>Gets the Window.RestoreButton.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("WINDOW", 21, 1);

            /// <summary>Gets the Window.RestoreButton.Hot visual style element.</summary>
            public static VisualStyleElement Hot => CreateElement("WINDOW", 21, 2);

            /// <summary>Gets the Window.RestoreButton.Pressed visual style element.</summary>
            public static VisualStyleElement Pressed => CreateElement("WINDOW", 21, 3);

            /// <summary>Gets the Window.RestoreButton.Disabled visual style element.</summary>
            public static VisualStyleElement Disabled => CreateElement("WINDOW", 21, 4);
        }

        /// <summary>Provides the Window.MdiRestoreButton visual style elements.</summary>
        public static class MdiRestoreButton
        {
            /// <summary>Gets the Window.MdiRestoreButton.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("WINDOW", 22, 1);

            /// <summary>Gets the Window.MdiRestoreButton.Hot visual style element.</summary>
            public static VisualStyleElement Hot => CreateElement("WINDOW", 22, 2);

            /// <summary>Gets the Window.MdiRestoreButton.Pressed visual style element.</summary>
            public static VisualStyleElement Pressed => CreateElement("WINDOW", 22, 3);

            /// <summary>Gets the Window.MdiRestoreButton.Disabled visual style element.</summary>
            public static VisualStyleElement Disabled => CreateElement("WINDOW", 22, 4);
        }

        /// <summary>Provides the Window.HelpButton visual style elements.</summary>
        public static class HelpButton
        {
            /// <summary>Gets the Window.HelpButton.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("WINDOW", 23, 1);

            /// <summary>Gets the Window.HelpButton.Hot visual style element.</summary>
            public static VisualStyleElement Hot => CreateElement("WINDOW", 23, 2);

            /// <summary>Gets the Window.HelpButton.Pressed visual style element.</summary>
            public static VisualStyleElement Pressed => CreateElement("WINDOW", 23, 3);

            /// <summary>Gets the Window.HelpButton.Disabled visual style element.</summary>
            public static VisualStyleElement Disabled => CreateElement("WINDOW", 23, 4);
        }

        /// <summary>Provides the Window.MdiHelpButton visual style elements.</summary>
        public static class MdiHelpButton
        {
            /// <summary>Gets the Window.MdiHelpButton.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("WINDOW", 24, 1);

            /// <summary>Gets the Window.MdiHelpButton.Hot visual style element.</summary>
            public static VisualStyleElement Hot => CreateElement("WINDOW", 24, 2);

            /// <summary>Gets the Window.MdiHelpButton.Pressed visual style element.</summary>
            public static VisualStyleElement Pressed => CreateElement("WINDOW", 24, 3);

            /// <summary>Gets the Window.MdiHelpButton.Disabled visual style element.</summary>
            public static VisualStyleElement Disabled => CreateElement("WINDOW", 24, 4);
        }

        /// <summary>Provides the Window.HorizontalScroll visual style elements.</summary>
        public static class HorizontalScroll
        {
            /// <summary>Gets the Window.HorizontalScroll.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("WINDOW", 25, 1);

            /// <summary>Gets the Window.HorizontalScroll.Hot visual style element.</summary>
            public static VisualStyleElement Hot => CreateElement("WINDOW", 25, 2);

            /// <summary>Gets the Window.HorizontalScroll.Pressed visual style element.</summary>
            public static VisualStyleElement Pressed => CreateElement("WINDOW", 25, 3);

            /// <summary>Gets the Window.HorizontalScroll.Disabled visual style element.</summary>
            public static VisualStyleElement Disabled => CreateElement("WINDOW", 25, 4);
        }

        /// <summary>Provides the Window.HorizontalThumb visual style elements.</summary>
        public static class HorizontalThumb
        {
            /// <summary>Gets the Window.HorizontalThumb.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("WINDOW", 26, 1);

            /// <summary>Gets the Window.HorizontalThumb.Hot visual style element.</summary>
            public static VisualStyleElement Hot => CreateElement("WINDOW", 26, 2);

            /// <summary>Gets the Window.HorizontalThumb.Pressed visual style element.</summary>
            public static VisualStyleElement Pressed => CreateElement("WINDOW", 26, 3);

            /// <summary>Gets the Window.HorizontalThumb.Disabled visual style element.</summary>
            public static VisualStyleElement Disabled => CreateElement("WINDOW", 26, 4);
        }

        /// <summary>Provides the Window.VerticalScroll visual style elements.</summary>
        public static class VerticalScroll
        {
            /// <summary>Gets the Window.VerticalScroll.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("WINDOW", 27, 1);

            /// <summary>Gets the Window.VerticalScroll.Hot visual style element.</summary>
            public static VisualStyleElement Hot => CreateElement("WINDOW", 27, 2);

            /// <summary>Gets the Window.VerticalScroll.Pressed visual style element.</summary>
            public static VisualStyleElement Pressed => CreateElement("WINDOW", 27, 3);

            /// <summary>Gets the Window.VerticalScroll.Disabled visual style element.</summary>
            public static VisualStyleElement Disabled => CreateElement("WINDOW", 27, 4);
        }

        /// <summary>Provides the Window.VerticalThumb visual style elements.</summary>
        public static class VerticalThumb
        {
            /// <summary>Gets the Window.VerticalThumb.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("WINDOW", 28, 1);

            /// <summary>Gets the Window.VerticalThumb.Hot visual style element.</summary>
            public static VisualStyleElement Hot => CreateElement("WINDOW", 28, 2);

            /// <summary>Gets the Window.VerticalThumb.Pressed visual style element.</summary>
            public static VisualStyleElement Pressed => CreateElement("WINDOW", 28, 3);

            /// <summary>Gets the Window.VerticalThumb.Disabled visual style element.</summary>
            public static VisualStyleElement Disabled => CreateElement("WINDOW", 28, 4);
        }

        /// <summary>Provides the Window.Dialog visual style elements.</summary>
        public static class Dialog
        {
            /// <summary>Gets the Window.Dialog.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("WINDOW", 29, 1);
        }

        /// <summary>Provides the Window.CaptionSizingTemplate visual style elements.</summary>
        public static class CaptionSizingTemplate
        {
            /// <summary>Gets the Window.CaptionSizingTemplate.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("WINDOW", 30, 1);
        }

        /// <summary>Provides the Window.SmallCaptionSizingTemplate visual style elements.</summary>
        public static class SmallCaptionSizingTemplate
        {
            /// <summary>Gets the Window.SmallCaptionSizingTemplate.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("WINDOW", 31, 1);
        }

        /// <summary>Provides the Window.FrameLeftSizingTemplate visual style elements.</summary>
        public static class FrameLeftSizingTemplate
        {
            /// <summary>Gets the Window.FrameLeftSizingTemplate.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("WINDOW", 32, 1);
        }

        /// <summary>Provides the Window.SmallFrameLeftSizingTemplate visual style elements.</summary>
        public static class SmallFrameLeftSizingTemplate
        {
            /// <summary>Gets the Window.SmallFrameLeftSizingTemplate.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("WINDOW", 33, 1);
        }

        /// <summary>Provides the Window.FrameRightSizingTemplate visual style elements.</summary>
        public static class FrameRightSizingTemplate
        {
            /// <summary>Gets the Window.FrameRightSizingTemplate.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("WINDOW", 34, 1);
        }

        /// <summary>Provides the Window.SmallFrameRightSizingTemplate visual style elements.</summary>
        public static class SmallFrameRightSizingTemplate
        {
            /// <summary>Gets the Window.SmallFrameRightSizingTemplate.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("WINDOW", 35, 1);
        }

        /// <summary>Provides the Window.FrameBottomSizingTemplate visual style elements.</summary>
        public static class FrameBottomSizingTemplate
        {
            /// <summary>Gets the Window.FrameBottomSizingTemplate.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("WINDOW", 36, 1);
        }

        /// <summary>Provides the Window.SmallFrameBottomSizingTemplate visual style elements.</summary>
        public static class SmallFrameBottomSizingTemplate
        {
            /// <summary>Gets the Window.SmallFrameBottomSizingTemplate.Normal visual style element.</summary>
            public static VisualStyleElement Normal => CreateElement("WINDOW", 37, 1);
        }
    }
}
