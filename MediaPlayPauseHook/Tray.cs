using System.Reflection;

namespace MediaPlayPauseHook;

public class Tray : ApplicationContext
{
	public Tray()
	{
		NotifyIcon notifyIcon = new();
		notifyIcon.Text = "Media Key Disabler";
		notifyIcon.Icon
			= new System.Drawing.Icon(
				Assembly.GetExecutingAssembly().GetManifestResourceStream("MediaPlayPauseHook.trayicon.ico")
					?? throw new InvalidOperationException("Missing trayicon.ico resource!"));
		
		ContextMenuStrip menu = new();
		notifyIcon.ContextMenuStrip = menu;

		ToolStripMenuItem menuName = new();
		menuName.Text = "Media Key Disabler";
		menuName.Enabled = false;
		menu.Items.Add(menuName);

		ToolStripMenuItem menuExit = new();
		menuExit.Text = "Exit";
		menuExit.Click += (_, _) =>
		{
			Application.Exit();
		};
		menu.Items.Add(menuExit);
		
		notifyIcon.Visible = true;
	}
	
}