using AudioRecorder_PlayerSample.Interface;
using Syncfusion.Maui.ListView.Hosting;

#if ANDROID || IOS
using AudioRecorder_PlayerSample.Platforms.Service;
#endif

namespace AudioRecorder_PlayerSample;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("AudioIconFonts.ttf", "AudioIconFonts");
            });

#if ANDROID || IOS
        builder.Services.AddTransient<IAudioPlayerService, AudioPlayerService>();
        builder.Services.AddTransient<IRecordAudioService, RecordAudioService>();
#endif

        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<AppShell>();
        builder.ConfigureSyncfusionListView();
        return builder.Build();
	}
}
