using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MediaPlayPauseHook;

public static class KeyHook
{
	private const int WH_KEYBOARD_LL = 13;
	private const int WM_KEYDOWN = 0x0100;
	private static readonly LowLevelKeyboardProc _proc = HookCallback;
	private static IntPtr _hookID = IntPtr.Zero;

	private static Func<Keys, int>? _hookAction;

	public static void Hook(Func<Keys, int> hookAction)
	{
		_hookAction = hookAction;
		_hookID = SetHook(_proc);
	}
	
	public static void Unhook()
	{
		UnhookWindowsHookEx(_hookID);
	}

	private static IntPtr SetHook(LowLevelKeyboardProc proc)
	{
		using Process curProcess = Process.GetCurrentProcess();

		if (curProcess.MainModule is null) throw new InvalidOperationException();
		
		using ProcessModule curModule = curProcess.MainModule;
		
		return SetWindowsHookEx(
			WH_KEYBOARD_LL, 
			proc,
			GetModuleHandle(curModule.ModuleName), 
			0);
	}

	private delegate IntPtr LowLevelKeyboardProc(
		int nCode, IntPtr wParam, IntPtr lParam);

	private static IntPtr HookCallback(
		int nCode, IntPtr wParam, IntPtr lParam)
	{
		if (_hookAction is not null &&
			nCode >= 0 && 
			wParam == (IntPtr)WM_KEYDOWN)
		{
			int vkCode = Marshal.ReadInt32(lParam);
			int result = _hookAction.Invoke((Keys)vkCode);
			if (result != 0) return result;
		}
		return CallNextHookEx(_hookID, nCode, wParam, lParam);
	}

	[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	private static extern IntPtr SetWindowsHookEx(int idHook,
		LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

	[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool UnhookWindowsHookEx(IntPtr hhk);

	[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
		IntPtr wParam, IntPtr lParam);

	[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	private static extern IntPtr GetModuleHandle(string lpModuleName);
}