using System;
using System.ComponentModel.Composition;
using System.Windows.Threading;

namespace ColorPicker.Helpers
{
    [Export(typeof(IThrottledActionInvoker))]
    public sealed class ThrottledActionInvoker : IThrottledActionInvoker
    {
        private Action _actionToRun;
        private DispatcherTimer _timer;

        public ThrottledActionInvoker()
        {
            _timer = new DispatcherTimer();
            _timer.Tick += Timer_Tick;
        }

        public void ScheduleAction(Action action, int milliseconds)
        {
            _actionToRun = action;
            _timer.Interval = TimeSpan.FromMilliseconds(milliseconds);

            if (_timer.IsEnabled)
            {
                _timer.Stop();
            }

            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            _timer.Stop(); // Stop the timer before invoking the action to ensure it doesn't overlap.
            _actionToRun.Invoke();
        }
    }
}
