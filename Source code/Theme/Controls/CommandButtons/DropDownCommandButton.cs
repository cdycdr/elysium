﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

using Elysium.Controls.Automation;
using Elysium.Controls.Primitives;
using Elysium.Extensions;

using JetBrains.Annotations;

namespace Elysium.Controls
{
    [PublicAPI]
    [TemplatePart(Name = PopupName, Type = typeof(Popup))]
// ReSharper disable ClassWithVirtualMembersNeverInherited.Global
    public class DropDownCommandButton : CommandButtonBase
// ReSharper restore ClassWithVirtualMembersNeverInherited.Global
    {
        private const string PopupName = "PART_Popup";

        private Popup _popup;

        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static DropDownCommandButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DropDownCommandButton), new FrameworkPropertyMetadata(typeof(DropDownCommandButton)));
            EventManager.RegisterClassHandler(typeof(DropDownCommandButton), MenuItem.ClickEvent, new RoutedEventHandler(OnMenuItemClick), true);
        }

        [PublicAPI]
        public static readonly DependencyProperty SubmenuProperty =
            DependencyProperty.Register("Submenu", typeof(Submenu), typeof(DropDownCommandButton),
                                        new FrameworkPropertyMetadata(null, OnSubmenuChanged));

        [PublicAPI]
        [Bindable(true)]
        [Category("Content")]
        [Description("Popup menu that drop down.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Submenu Submenu
        {
            get { return (Submenu)GetValue(SubmenuProperty); }
            set { SetValue(SubmenuProperty, value); }
        }

        private static void OnSubmenuChanged([NotNull] DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ValidationHelper.NotNull(obj, () => obj);
            var instance = (DropDownCommandButton)obj;
            instance.OnSubmenuChanged((Submenu)e.OldValue, (Submenu)e.NewValue);
        }

        [PublicAPI]
        protected virtual void OnSubmenuChanged(Submenu oldSubmenu, Submenu newSubmenu)
        {
            if (_popup != null)
            {
                _popup.Child = newSubmenu;
            }
            HasSubmenu = newSubmenu != null;
        }

        private static readonly DependencyPropertyKey HasSubmenuPropertyKey =
            DependencyProperty.RegisterReadOnly("HasSubmenu", typeof(bool), typeof(DropDownCommandButton),
                                                new FrameworkPropertyMetadata(BooleanBoxingHelper.FalseBox, OnHasSubmenuChanged));

        [PublicAPI]
        public static readonly DependencyProperty HasSubmenuProperty = HasSubmenuPropertyKey.DependencyProperty;

        [PublicAPI]
        [Bindable(false)]
        [Browsable(false)]
        public bool HasSubmenu
        {
            get { return BooleanBoxingHelper.Unbox(GetValue(HasSubmenuProperty)); }
            private set { SetValue(HasSubmenuPropertyKey, BooleanBoxingHelper.Box(value)); }
        }

        private static void OnHasSubmenuChanged([NotNull] DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ValidationHelper.NotNull(obj, () => obj);
            var instance = (DropDownCommandButton)obj;
            instance.OnHasSubmenuChanged(BooleanBoxingHelper.Unbox(e.OldValue), BooleanBoxingHelper.Unbox(e.NewValue));
        }

        [PublicAPI]
        protected virtual void OnHasSubmenuChanged(bool oldHasSubmenu, bool newHasSubmenu)
        {
            var peer = UIElementAutomationPeer.FromElement(this) as DropDownCommandButtonAutomationPeer;
            if (peer != null)
            {
                peer.RaiseExpandCollapseStatePropertyChangedEvent(
                    oldHasSubmenu ? IsDropDownOpen ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed : ExpandCollapseState.LeafNode,
                    newHasSubmenu ? IsDropDownOpen ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed : ExpandCollapseState.LeafNode);
            }
        }

        [PublicAPI]
        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(DropDownCommandButton),
                                        new FrameworkPropertyMetadata(BooleanBoxingHelper.FalseBox, OnIsDropDownOpenChanged, CoerceIsDropDownOpen));

        [PublicAPI]
        [Bindable(true)]
        [Browsable(false)]
        [Category("Appearance")]
        [Description("Indicates whether drop down is open.")]
        public bool IsDropDownOpen
        {
            get { return BooleanBoxingHelper.Unbox(GetValue(IsDropDownOpenProperty)); }
            set { SetValue(IsDropDownOpenProperty, BooleanBoxingHelper.Box(value)); }
        }

        private static void OnIsDropDownOpenChanged([NotNull] DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ValidationHelper.NotNull(obj, () => obj);
            var instance = (DropDownCommandButton)obj;
            instance.OnIsDropDownOpenChanged(BooleanBoxingHelper.Unbox(e.OldValue), BooleanBoxingHelper.Unbox(e.NewValue));
        }

        private void OnIsDropDownOpenChanged(bool oldIsDropDownOpen, bool newIsDropDownOpen)
        {
            var peer = UIElementAutomationPeer.FromElement(this) as DropDownCommandButtonAutomationPeer;
            if (peer != null)
            {
                if (!HasSubmenu)
                {
                    peer.RaiseExpandCollapseStatePropertyChangedEvent(oldIsDropDownOpen ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed,
                                                                      newIsDropDownOpen ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed);
                }
            }

            switch (newIsDropDownOpen)
            {
                case true:
                    VisualStateManager.GoToState(this, "DropDown", true);
                    break;
                case false:
                    VisualStateManager.GoToState(this, "Normal", true);
                    break;
            }
        }

        private static object CoerceIsDropDownOpen([NotNull] DependencyObject obj, object baseValue)
        {
            ValidationHelper.NotNull(obj, () => obj);
            var instance = (DropDownCommandButton)obj;
            return instance.CoerceIsDropDownOpen(BooleanBoxingHelper.Unbox(baseValue));
        }

        private object CoerceIsDropDownOpen(bool baseValue)
        {
            return HasSubmenu && baseValue;
        }

        [PublicAPI]
        public event EventHandler DropDownOpened;

        [PublicAPI]
        protected virtual void OnDropDownOpened(EventArgs e)
        {
            if (DropDownOpened != null)
            {
                DropDownOpened(this, e);
            }
        }

        private void OnDropDownOpened(object sender, EventArgs e)
        {
            OnDropDownOpened(e);
        }

        [PublicAPI]
        public event EventHandler DropDownClosed;

        [PublicAPI]
        protected virtual void OnDropDownClosed(EventArgs e)
        {
            if (DropDownClosed != null)
            {
                DropDownClosed(this, e);
            }
        }

        private void OnDropDownClosed(object sender, EventArgs e)
        {
            OnDropDownClosed(e);
        }

        [PublicAPI]
        public static readonly DependencyProperty DropDownDirectionProperty =
            DependencyProperty.Register("DropDownDirection", typeof(DropDownDirection), typeof(DropDownCommandButton),
                                        new FrameworkPropertyMetadata(DropDownDirection.Down));

        [PublicAPI]
        [Category("Layout")]
        [Description("The direction of the drop down.")]
        public DropDownDirection DropDownDirection
        {
            get { return BoxingHelper<DropDownDirection>.Unbox(GetValue(DropDownDirectionProperty)); }
            set { SetValue(DropDownDirectionProperty, value); }
        }

        [PublicAPI]
        public static readonly DependencyProperty MaxDropDownHeightProperty =
            DependencyProperty.Register("MaxDropDownHeight", typeof(double), typeof(DropDownCommandButton), new FrameworkPropertyMetadata(300d));

        [PublicAPI]
        [Bindable(true)]
        [Category("Layout")]
        [Description("The maximum height constraint of the drop down.")]
        [TypeConverter(typeof(LengthConverter))]
        public double MaxDropDownHeight
        {
            get { return BoxingHelper<double>.Unbox(GetValue(MaxDropDownHeightProperty)); }
            set { SetValue(MaxDropDownHeightProperty, value); }
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new DropDownCommandButtonAutomationPeer(this);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (Template != null)
            {
                if (_popup != null)
                {
                    _popup.Child = null;
                    _popup.Closed -= OnDropDownClosed;
                    _popup.Opened -= OnDropDownOpened;
                    _popup.CustomPopupPlacementCallback = null;
                }
                // BUG in Code Contracts: FindName is pure method
                Contract.Assume(Template != null);
                _popup = Template.FindName(PopupName, this) as Popup;
                if (_popup == null)
                {
                    Trace.TraceWarning(PopupName + " not found.");
                }
                else
                {
                    _popup.CustomPopupPlacementCallback = PlacePopup;
                    _popup.Opened += OnDropDownOpened;
                    _popup.Closed += OnDropDownClosed;
                    if (Submenu != null)
                    {
                        _popup.Child = Submenu;
                    }
                }
            }
        }

        protected override void OnClick()
        {
            IsDropDownOpen = true;
            base.OnClick();
        }

        private static void OnMenuItemClick(object sender, RoutedEventArgs e)
        {
            var instance = sender as DropDownCommandButton;
            if (instance != null)
            {
                instance.IsDropDownOpen = false;
            }
        }

        private CustomPopupPlacement[] PlacePopup(Size popupsize, Size targetsize, Point offset)
        {
            var window = System.Windows.Window.GetWindow(this);
            if (window != null)
            {
                var transformToAncestor = TransformToAncestor(window);

                var x = targetsize.Width / 2 - popupsize.Width / 2 + offset.X;
                var y = DropDownDirection == DropDownDirection.Up ? -popupsize.Height + offset.Y : ActualHeight + popupsize.Height + offset.Y;
                var position = new Point(x, y);

                if (transformToAncestor != null)
                {
                    var relativePosition = transformToAncestor.Transform(position);
                    var selfPosition = TranslatePoint(new Point(0, 0), window);

                    if (relativePosition.X < 0)
                    {
                        relativePosition.X = selfPosition.X;
                    }
                    if (relativePosition.X + popupsize.Width > window.Width)
                    {
                        relativePosition.X = selfPosition.X + ActualWidth;
                    }
                    if (DropDownDirection == DropDownDirection.Up && relativePosition.Y < 0)
                    {
                        relativePosition.Y = selfPosition.Y + ActualHeight + popupsize.Height;
                    }
                    if (DropDownDirection == DropDownDirection.Down && relativePosition.Y + popupsize.Height > window.Height)
                    {
                        relativePosition.Y = -popupsize.Height;
                    }

                    var transformToDescendant = window.TransformToDescendant(this);
                    if (transformToDescendant != null)
                    {
                        position = transformToDescendant.Transform(relativePosition);
                    }
                }

                return new[] { new CustomPopupPlacement(position, PopupPrimaryAxis.None) };
            }
            return null;
        }
    }
} ;