﻿using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;

namespace AudioRecorder_PlayerSample;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    public static Context context;
    protected override void OnCreate(Bundle savedInstanceState)
    {
        context = this;
        base.OnCreate(savedInstanceState);
    }
}
