using ColorPicker.Settings;
using Microsoft.Xaml.Behaviors;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ColorPicker.Behaviors
{
    public class SetShortcutBehavior : Behavior<UserControl>
    {
        private HashSet<Key> monitoredKeys = new HashSet<Key>();

        public bool MonitorKeys
        {
            get { return (bool)GetValue(MonitorKeysProperty); }
            set { SetValue(MonitorKeysProperty, value); }
        }

        public static DependencyProperty MonitorKeysProperty = DependencyProperty.Register(
            "MonitorKeys", typeof(bool), typeof(SetShortcutBehavior), new PropertyMetadata(false, OnMonitorKeysChanged));

        public string ShortCutPreview
        {
            get { return (string)GetValue(ShortCutPreviewProperty); }
            set { SetValue(ShortCutPreviewProperty, value); }
        }

        public static DependencyProperty ShortCutPreviewProperty = DependencyProperty.Register(
            "ShortCutPreview", typeof(string), typeof(SetShortcutBehavior));

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Loaded -= AssociatedObject_Loaded;
            AssociatedObject.PreviewKeyDown -= AssociatedObject_PreviewKeyDown;
        }

        private static void OnMonitorKeysChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = d as SetShortcutBehavior;
            if (behavior != null)
            {
                if ((bool)e.NewValue)
                {
                    behavior.AssociatedObject.PreviewKeyDown += behavior.AssociatedObject_PreviewKeyDown;
                }
                else
                {
                    behavior.AssociatedObject.PreviewKeyDown -= behavior.AssociatedObject_PreviewKeyDown;
                    behavior.ShortCutPreview = string.Empty; // Clear preview when monitoring stops
                }
            }
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            if (MonitorKeys)
            {
                AssociatedObject.PreviewKeyDown += AssociatedObject_PreviewKeyDown;
            }
        }

        private void AssociatedObject_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            UpdateMonitoredKeys();

            var pressedKeys = new List<string>();
            foreach (var key in monitoredKeys)
            {
                if (System.Windows.Input.Keyboard.IsKeyDown(key))
                {
                    pressedKeys.Add(key.ToString());
                }
            }

            // ignore modifiers, we captured them above already
            if (!IsModifierKey(e.Key))
            {
                pressedKeys.Add(e.Key.ToString());
            }

            var allKeys = string.Join(" + ", pressedKeys);
            ShortCutPreview = allKeys;
        }

        private void UpdateMonitoredKeys()
        {
            monitoredKeys.Clear();
            if (System.Windows.Input.Keyboard.IsKeyDown(Key.LeftShift) || System.Windows.Input.Keyboard.IsKeyDown(Key.RightShift))
            {
                monitoredKeys.Add(Key.LeftShift);
                monitoredKeys.Add(Key.RightShift);
            }
            if (System.Windows.Input.Keyboard.IsKeyDown(Key.LeftCtrl) || System.Windows.Input.Keyboard.IsKeyDown(Key.RightCtrl))
            {
                monitoredKeys.Add(Key.LeftCtrl);
                monitoredKeys.Add(Key.RightCtrl);
            }
            if (System.Windows.Input.Keyboard.IsKeyDown(Key.LeftAlt) || System.Windows.Input.Keyboard.IsKeyDown(Key.RightAlt))
            {
                monitoredKeys.Add(Key.LeftAlt);
                monitoredKeys.Add(Key.RightAlt);
            }
        }

        private bool IsModifierKey(Key key)
        {
            return key == Key.LeftShift || key == Key.RightShift ||
                   key == Key.LeftCtrl || key == Key.RightCtrl ||
                   key == Key.LeftAlt || key == Key.RightAlt ||
                   key == Key.System;
        }
    }
}
