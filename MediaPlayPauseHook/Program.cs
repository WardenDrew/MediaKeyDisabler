using System.Reflection;
using System.Text.Json;
using MediaPlayPauseHook;

Console.WriteLine(JsonSerializer.Serialize(Assembly.GetExecutingAssembly().GetManifestResourceNames()));

KeyHook.Hook(key => key switch
	{
		Keys.MediaPlayPause
			or Keys.MediaNextTrack
			or Keys.MediaPreviousTrack
			or Keys.MediaStop 
			=> 1,
		_ => 0,
	}
);

Application.Run(new Tray());
KeyHook.Unhook();