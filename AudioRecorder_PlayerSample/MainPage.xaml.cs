using AudioRecorder_PlayerSample.Interface;
using AudioRecorder_PlayerSample.ViewModels;
namespace AudioRecorder_PlayerSample;

public partial class MainPage : ContentPage
{
	public MainPage(IAudioPlayerService audioPlayerService, IRecordAudioService recordAudioService)
	{
		InitializeComponent();
        BindingContext = new MainPageViewModel(audioPlayerService, recordAudioService);
    }
}

