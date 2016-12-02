using Android.App;
using Android.Content.PM;
using Android.OS;
using Plugin.Permissions;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: UsesFeature("android.hardware.camera", Required = false)]
[assembly: UsesFeature("android.hardware.camera.autofocus", Required = false)]

namespace SaveTheBill.Droid
{
    [Activity(Label = "SaveTheBill", Icon = "@drawable/icon", Theme = "@android:style/Theme.Material.Light", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            Forms.Init(this, bundle);
            LoadApplication(new App());
        }
    }
}