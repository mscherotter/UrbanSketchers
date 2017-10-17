using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrbanSketchers.Controls;
using Xamarin.Forms;

namespace UrbanSketchers
{
    public class ConnectedAnimations
    {
        public static readonly BindableProperty PrepareConnectedAnimationProperty =
            BindableProperty.CreateAttached(
                "PrepareConnectedAnimation",
                typeof(PrepareConnectedAnimationData), typeof(ConnectedAnimations), null);

        public static PrepareConnectedAnimationData GetPrepareConnectedAnimation(BindableObject target)
        {
            return target.GetValue(PrepareConnectedAnimationProperty) as PrepareConnectedAnimationData;
        }

        public static void SetPrepareConnectedAnimation(BindableObject target, PrepareConnectedAnimationData data)
        {
            target.SetValue(PrepareConnectedAnimationProperty, data);
        }
    }
}
