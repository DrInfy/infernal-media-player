﻿#region Usings

using System;
using System.Windows;
using System.Windows.Controls;

#endregion

namespace ImpControls
{
    public class ToolTipEx : ToolTip
    {
        #region Static Fields and Constants

        public static readonly DependencyProperty IsReallyOpenProperty;

        #endregion

        #region Properties

        public bool IsReallyOpen
        {
            get
            {
                var b = (bool) GetValue(IsReallyOpenProperty);
                return b;
            }
            set { SetValue(IsReallyOpenProperty, value); }
        }

        #endregion

        static ToolTipEx()
        {
            IsReallyOpenProperty =
                DependencyProperty.Register(
                    "IsReallyOpen",
                    typeof (bool),
                    typeof (ToolTipEx),
                    new FrameworkPropertyMetadata(
                        defaultValue: false,
                        flags: FrameworkPropertyMetadataOptions.None,
                        propertyChangedCallback: StaticOnIsReallyOpenedChanged));
        }

        protected static void StaticOnIsReallyOpenedChanged(
            DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var self = (ToolTipEx) o;
            self.OnIsReallyOpenedChanged((bool) e.OldValue, (bool) e.NewValue);
        }

        protected void OnIsReallyOpenedChanged(bool oldValue, bool newValue)
        {
            IsOpen = newValue;
        }

        protected override void OnClosed(RoutedEventArgs e)
        {
            System.Diagnostics.Debug.Print(String.Format(
                "OnClosed: IsReallyOpen: {0}, StaysOpen: {1}", IsReallyOpen, StaysOpen));
            if (IsReallyOpen && StaysOpen)
            {
                e.Handled = true;
                // We cannot set this.IsOpen directly here.  Instead, send an event asynchronously.
                // DispatcherPriority.Send is the highest priority possible.
                //Dispatcher.CurrentDispatcher.BeginInvoke(
                //    (Action)(() => this.IsOpen = true),
                //    DispatcherPriority.Send);
            }
            else
            {
                base.OnClosed(e);
            }
        }
    }
}