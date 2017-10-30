using UrbanSketchers.Controls;
using Xamarin.Forms;

namespace UrbanSketchers
{
    /// <summary>
    ///     UWP Connected animations
    /// </summary>
    public class ConnectedAnimations
    {
        /// <summary>
        ///     the pepare connected animation attached property
        /// </summary>
        public static readonly BindableProperty PrepareConnectedAnimationProperty =
            BindableProperty.CreateAttached(
                "PrepareConnectedAnimation",
                typeof(PrepareConnectedAnimationData), typeof(ConnectedAnimations), null);

        /// <summary>
        ///     Gets the prepare connected animation
        /// </summary>
        /// <param name="target">the bindable object target</param>
        /// <returns>the prepare connected animation</returns>
        public static PrepareConnectedAnimationData GetPrepareConnectedAnimation(BindableObject target)
        {
            return target.GetValue(PrepareConnectedAnimationProperty) as PrepareConnectedAnimationData;
        }

        /// <summary>
        ///     Sets the prepare connected animation
        /// </summary>
        /// <param name="target"></param>
        /// <param name="data"></param>
        public static void SetPrepareConnectedAnimation(BindableObject target, PrepareConnectedAnimationData data)
        {
            target.SetValue(PrepareConnectedAnimationProperty, data);
        }
    }
}