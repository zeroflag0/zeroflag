namespace zeroflag.Windows
{
	using System;
	using System.Drawing;
	using System.Runtime.InteropServices;
#if WPF
	using button = System.Windows.Input.MouseButton;
	using key = System.Windows.Input.Key;
#else
	using button = System.Windows.Forms.MouseButtons;
	using key = System.Windows.Forms.Keys;
#endif
	public static partial class WinAPI
	{
		//public static IntPtr Build(int low, int high)
		//{
		//    return (IntPtr)(((short)low) | (((ushort)high) << 0x10));
		//}

		public static zeroflag.Windows.MouseButtons ConvertMouseButtons(button value)
		{
			zeroflag.Windows.MouseButtons converted = zeroflag.Windows.MouseButtons.None;
			if ((value & button.Left) != 0)
			{
				converted |= zeroflag.Windows.MouseButtons.MK_LBUTTON;
			}
			if ((value & button.Right) != 0)
			{
				converted |= zeroflag.Windows.MouseButtons.MK_RBUTTON;
			}
			if ((value & button.Middle) != 0)
			{
				converted |= zeroflag.Windows.MouseButtons.MK_MBUTTON;
			}
			if ((value & button.XButton1) != 0)
			{
				converted |= zeroflag.Windows.MouseButtons.MK_XBUTTON1;
			}
			if ((value & button.XButton2) != 0)
			{
				converted |= zeroflag.Windows.MouseButtons.MK_XBUTTON2;
			}
			return converted;
		}

		public static IntPtr ConvertMousePosition(Point location)
		{
			return new IntPtr(location.X | location.Y << 0x10);
		}

		//[DllImport("user32.dll")]
		//public static extern IntPtr GetActiveWindow();
		[DllImport("user32.dll")]
		public static extern IntPtr GetForegroundWindow();
		[DllImport("user32.dll")]
		public static extern bool SetForegroundWindow(IntPtr handle);

		[DllImport("user32.dll")]
		public static extern bool GetClientRect(IntPtr hWnd, ref LPRECT lpRect);
		[DllImport("user32.dll")]
		public static extern ulong GetLastError();
		[DllImport("user32.dll")]
		public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
		[DllImport("user32.dll")]
		public static extern bool RegisterHotKey(IntPtr hWnd, int id, ModKeys fsModifiers, key vk);
		[DllImport("user32.dll")]
		public static extern uint SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
		[DllImport("user32.dll")]
		public static extern uint SendMessage(IntPtr hWnd, WindowsMessages msg, IntPtr wParam, IntPtr lParam);
		[DllImport("user32.dll")]
		public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

		public static IntPtr NULL
		{
			get
			{
				return IntPtr.Zero;
			}
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct LPRECT
		{
			public long left;
			public long top;
			public long right;
			public long bottom;
		}
	}
}
