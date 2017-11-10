using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace UrbanSketchers.Support
{
    /// <summary>
    /// Activity indicator scope
    /// </summary>
    public class ActivityIndicatorScope : IDisposable
    {
        private readonly bool _showIndicator;
        private readonly ActivityIndicator _indicator;
        private readonly Task _indicatorDelay;

        /// <summary>
        /// Initializes a new instance of the ActivityIndicatorScope class.
        /// </summary>
        /// <param name="indicator">the indicator</param>
        /// <param name="showIndicator">true to show the indicator</param>
        public ActivityIndicatorScope(ActivityIndicator indicator, bool showIndicator)
        {
            _indicator = indicator;
            _showIndicator = showIndicator;

            if (showIndicator)
            {
                _indicatorDelay = Task.Delay(2000);
                SetIndicatorActivity(true);
            }
            else
            {
                _indicatorDelay = Task.FromResult(0);
            }
        }

        private void SetIndicatorActivity(bool isActive)
        {
            _indicator.IsVisible = isActive;
            _indicator.IsRunning = isActive;
        }

        /// <summary>
        /// Hide the activity indicator
        /// </summary>
        public void Dispose()
        {
            if (_showIndicator)
            {
                _indicatorDelay.ContinueWith(t => SetIndicatorActivity(false), TaskScheduler.FromCurrentSynchronizationContext());
            }
        }
    }
}
