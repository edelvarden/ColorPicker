using ColorPicker.Helpers;
using Microsoft.Xaml.Behaviors;
using System.Windows;

namespace ColorPicker.Behaviors
{
    public class CloseZoomWindowBehavior : Behavior<Window>
    {
        private ZoomWindowHelper _zoomWindowHelper;

        // Use constructor for initialization instead of attaching event handlers in OnAttached
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            // Attach event handlers here when the window is loaded
            AssociatedObject.PreviewKeyDown += AssociatedObject_PreviewKeyDown;
            AssociatedObject.MouseLeftButtonDown += AssociatedObject_MouseLeftButtonDown;

            // Use Dependency Injection to get the ZoomWindowHelper instance
            _zoomWindowHelper = Bootstrapper.Container.GetExportedValue<ZoomWindowHelper>();
        }

        private void AssociatedObject_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            CloseZoomWindowIfPossible();
        }

        private void AssociatedObject_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                CloseZoomWindowIfPossible();
            }
        }

        private void CloseZoomWindowIfPossible()
        {
            // Check if _zoomWindowHelper is not null before attempting to close the zoom window
            _zoomWindowHelper?.CloseZoomWindow();
        }
    }
}
