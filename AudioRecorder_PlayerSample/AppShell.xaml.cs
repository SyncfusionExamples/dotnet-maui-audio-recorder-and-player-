namespace AudioRecorder_PlayerSample;

public partial class AppShell : Shell
{
	public AppShell(MainPage page)
	{
		InitializeComponent();
		shellContent.Content = page;
	}
}
