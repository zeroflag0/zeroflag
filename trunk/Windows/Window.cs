#region LGPL License
//********************************************************************
//	author:         Thomas "zeroflag" Kraemer
//	author email:   zeroflag@zeroflag.de
//	
//	Copyright (C) 2006-2008  Thomas "zeroflag" Kraemer
//	
//	license:	(LGPL)
//	
//		This library is free software; you can redistribute it and/or
//		modify it under the terms of the GNU Lesser General Public
//		License as published by the Free Software Foundation; either
//		version 2.1 of the License, or (at your option) any later version.
//
//		This library is distributed in the hope that it will be useful,
//		but WITHOUT ANY WARRANTY; without even the implied warranty of
//		MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//		Lesser General Public License for more details.
//
//		You should have received a copy of the GNU Lesser General Public
//		License along with this library; if not, write to the Free Software
//		Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
//
//		http://www.gnu.org/licenses/lgpl.html#TOC1
//
//*********************************************************************
#endregion LGPL License

namespace zeroflag.Windows
{
	using System;
	using System.Diagnostics;
	using System.Drawing;
	using System.Threading;
	using System.Drawing.Imaging;
#if WPF
	using button = System.Windows.Input.MouseButton;
	using key = System.Windows.Input.Key;
#else
	using button = System.Windows.Forms.MouseButtons;
	using key = System.Windows.Forms.Keys;
#endif
	public class Window
	{
		private DebugMode m_Debug;
		private string m_DebugLog;
		private IntPtr m_Handle;

		private button m_MouseButtons;
		//System.Diagnostics.Process m_Process;

		//public System.Diagnostics.Process Process
		//{
		//    get { return m_Process; }
		//    set
		//    {
		//        m_Process = value;
		//        if (value != null)
		//        {
		//            this.Handle = value.MainWindowHandle;
		//        }
		//    }
		//}

		public Window()
		{
		}

		//public Window(System.Diagnostics.Process process) : this(process.MainWindowHandle)
		//{
		//    this.Process = process;
		//}

		public Window(IntPtr handle)
		{
			this.m_DebugLog = "";
			this.Handle = handle;
		}

		IntPtr lastHandle;
		IntPtr hdcSrc;
		int width;
		int height;
		IntPtr hdcDest, hBitmap, hOld;
		/// <summary>
		/// Creates an Image object containing a screen shot of a specific window
		/// </summary>
		/// <param name="handle">The handle to the window. (In windows forms, this is obtained by the Handle property)</param>
		/// <returns></returns>
		public System.Drawing.Image Capture()
		{
			// get te hDC of the target window

			//((HdcSrc == null || lastHandle != this.Handle) ? HdcSrc = ScreenCapture.User32.GetWindowDC(this.Handle) : HdcSrc);
			if (lastHandle != this.Handle)
			{
				if (this.Handle == default(IntPtr)) return null;
				hdcSrc = ScreenCapture.User32.GetWindowDC(this.Handle);
				// get the size
				ScreenCapture.User32.RECT windowRect = new ScreenCapture.User32.RECT();
				ScreenCapture.User32.GetWindowRect(this.Handle, ref windowRect);
				width = windowRect.right - windowRect.left;
				height = windowRect.bottom - windowRect.top;
				// create a device context we can copy to
				hdcDest = ScreenCapture.GDI32.CreateCompatibleDC(hdcSrc);
				// create a bitmap we can copy it to,
				// using GetDeviceCaps to get the width/height
				hBitmap = ScreenCapture.GDI32.CreateCompatibleBitmap(hdcSrc, width, height);
				lastHandle = this.Handle;
			}
			// select the bitmap object
			hOld = ScreenCapture.GDI32.SelectObject(hdcDest, hBitmap);
			// bitblt over
			ScreenCapture.GDI32.BitBlt(hdcDest, 0, 0, width, height, hdcSrc, 0, 0, ScreenCapture.GDI32.SRCCOPY);
			// restore selection
			ScreenCapture.GDI32.SelectObject(hdcDest, hOld);
			// get a .NET image object for it
			System.Drawing.Image img = System.Drawing.Image.FromHbitmap(hBitmap);

			return img;
		}
		private void CaptureCleanup()
		{
			// free up the Bitmap object
			if (hBitmap != default(IntPtr)) ScreenCapture.GDI32.DeleteObject(hBitmap);
			// clean up
			if (hdcDest != default(IntPtr)) ScreenCapture.GDI32.DeleteDC(hdcDest);
			if (hdcSrc != default(IntPtr)) ScreenCapture.User32.ReleaseDC(lastHandle, hdcSrc);
		}

		~Window()
		{
			this.CaptureCleanup();
		}
		/// <summary>
		/// Captures a screen shot of a specific window, and saves it to a file
		/// </summary>
		/// <param name="handle"></param>
		/// <param name="filename"></param>
		/// <param name="format"></param>
		public Image CaptureToFile(string filename, ImageFormat format)
		{
			System.Drawing.Image img = Capture();
			img.Save(filename, format);
			return img;
		}

		protected void _MouseDown(Point location, button button)
		{
			IntPtr position = WinAPI.ConvertMousePosition(location);
			this.MouseButtons |= button;
			zeroflag.Windows.MouseButtons winbutton = WinAPI.ConvertMouseButtons(this.MouseButtons);
			if ((button & button.Left) != 0)
			{
				WinAPI.SendMessage(this.Handle, WindowsMessages.WM_LBUTTONDOWN, (IntPtr)((long)winbutton), position);
			}
			else if ((button & button.Right) != 0)
			{
				WinAPI.SendMessage(this.Handle, WindowsMessages.WM_RBUTTONDOWN, (IntPtr)((long)winbutton), position);
			}
			else if ((button & button.Middle) != 0)
			{
				WinAPI.SendMessage(this.Handle, WindowsMessages.WM_MBUTTONDOWN, (IntPtr)((long)winbutton), position);
			}
			WinAPI.SendMessage(this.Handle, WindowsMessages.WM_NCHITTEST, WinAPI.NULL, position);
		}

		protected void _MouseUp(Point location, button button)
		{
			IntPtr position = WinAPI.ConvertMousePosition(location);
			this.MouseButtons &= ~button;
			zeroflag.Windows.MouseButtons winbutton = WinAPI.ConvertMouseButtons(this.MouseButtons);
			if ((button & button.Left) != 0)
			{
				WinAPI.SendMessage(this.Handle, WindowsMessages.WM_LBUTTONUP, (IntPtr)((long)winbutton), position);
			}
			else if ((button & button.Right) != 0)
			{
				WinAPI.SendMessage(this.Handle, WindowsMessages.WM_RBUTTONUP, (IntPtr)((long)winbutton), position);
			}
			else if ((button & button.Middle) != 0)
			{
				WinAPI.SendMessage(this.Handle, WindowsMessages.WM_MBUTTONUP, (IntPtr)((long)winbutton), position);
			}
			WinAPI.SendMessage(this.Handle, WindowsMessages.WM_NCHITTEST, WinAPI.NULL, position);
		}

		public void Activate(bool actiavte)
		{
			IntPtr wParam = actiavte ? ((IntPtr)1) : IntPtr.Zero;
			WinAPI.SendMessage(this.Handle, WindowsMessages.WM_ACTIVATE, wParam, WinAPI.NULL);
		}

		public bool Active
		{
			get { return WinAPI.GetForegroundWindow() == this.Handle; }
			set { if (this.Active != value) this.Activate(value); }
		}

		public void Foreground()
		{
			WinAPI.SetForegroundWindow(this.Handle);
		}

		public void KeyPress(System.Windows.Forms.Keys key)
		{
			this.KeyPress(key, 100);
		}
		public void KeyPress(System.Windows.Forms.Keys key, int duration)
		{
			this.KeyDown(key);
			System.Threading.Thread.Sleep(duration);
			this.KeyUp(key);
		}

		public void KeyDown(key key)
		{
			bool active = this.Active;
			this.Active = true;
			WinAPI.KeyDown(key);
			//WinAPI.SendMessage(this.Handle, WindowsMessages.WM_KEYDOWN, (IntPtr)((long)key), new IntPtr());
			this.Active = active;
		}

		public void MsgKeyDown(key key)
		{
			WinAPI.SendMessage(this.Handle, WindowsMessages.WM_KEYDOWN, (IntPtr)((long)key), new IntPtr());
		}

		public void KeyUp(key key)
		{
			bool active = this.Active;
			this.Active = true;
			WinAPI.KeyUp(key);
			//WinAPI.SendMessage(this.Handle, WindowsMessages.WM_KEYUP, (IntPtr)((long)key), new IntPtr());
			this.Active = active;
		}

		public void MsgKeyUp(key key)
		{
			WinAPI.SendMessage(this.Handle, WindowsMessages.WM_KEYUP, (IntPtr)((long)key), new IntPtr());
		}

		public void MouseClick(Point location, button button)
		{
			MouseClick(location, button, 10);
		}
		public void MouseClick(Point location, button button, int duration)
		{
			this.MouseDown(location, button);
			System.Threading.Thread.Sleep(duration);
			this.MouseUp(location, button);
		}

		public void MouseDown(Point location, button button)
		{
			this.MouseMove(location);
			this._MouseDown(location, button);
		}

		public void MouseMove(Point location)
		{
			this.MouseMove(location, this.MouseButtons);
		}

		protected void MouseMove(Point location, button button)
		{
			IntPtr position = WinAPI.ConvertMousePosition(location);
			zeroflag.Windows.MouseButtons winbutton = WinAPI.ConvertMouseButtons(button);
			this.SendDebug(WindowsMessages.WM_MOUSEFIRST, (IntPtr)((long)winbutton), position);
			WinAPI.SendMessage(this.Handle, WindowsMessages.WM_MOUSEFIRST, (IntPtr)((long)winbutton), position);
			WinAPI.SendMessage(this.Handle, WindowsMessages.WM_NCHITTEST, WinAPI.NULL, position);
			if (button == 0)
			{
				WinAPI.SendMessage(this.Handle, WindowsMessages.WM_SETCURSOR, this.Handle, position);
			}
		}

		public void MouseUp(Point location, button button)
		{
			this._MouseUp(location, button);
			this.MouseMove(location);
		}

		protected void SendDebug(object value)
		{
			string text = "<null>";
			if (value != null)
			{
				text = value.ToString();
			}
			switch (this.Debug)
			{
				case DebugMode.Console:
					Console.WriteLine(text);
					return;

				case DebugMode.String:
					this.DebugLog = this.DebugLog + text;
					return;
			}
		}

		protected void SendDebug(WindowsMessages msg, int wParam, int lParam)
		{
			this.SendDebug(msg, (IntPtr)wParam, (IntPtr)lParam);
		}

		protected void SendDebug(WindowsMessages msg, IntPtr wParam, IntPtr lParam)
		{
			this.SendDebug(string.Concat(new object[] { msg, " wParam=0x", wParam.ToString("X").PadLeft(8, '0'), ", lParam=0x", lParam.ToString("X").PadLeft(8, '0') }));
		}

		public DebugMode Debug
		{
			get
			{
				return this.m_Debug;
			}
			set
			{
				this.m_Debug = value;
			}
		}

		public string DebugLog
		{
			get
			{
				return this.m_DebugLog;
			}
			protected set
			{
				this.m_DebugLog = value;
			}
		}

		public IntPtr Handle
		{
			get
			{
				return this.m_Handle;
			}
			set
			{
				this.m_Handle = value;
			}
		}

		public button MouseButtons
		{
			get
			{
				return this.m_MouseButtons;
			}
			protected set
			{
				this.m_MouseButtons = value;
			}
		}

		public Rectangle ScreenRegion
		{
			get
			{
				WinAPI.LPRECT rect = new WinAPI.LPRECT();
				WinAPI.GetClientRect(this.Handle, ref rect);
				return new System.Drawing.Rectangle((int)(rect.left), (int)(rect.top), (int)(rect.right - rect.left), (int)(rect.bottom - rect.top));

				//return Screen.FromHandle(this.Handle).WorkingArea;
			}
		}

		public System.Drawing.Size Size
		{
			get
			{
				WinAPI.LPRECT rect = new WinAPI.LPRECT();
				WinAPI.GetClientRect(this.Handle, ref rect);
				return new System.Drawing.Size((int)(rect.right - rect.left), (int)(rect.bottom - rect.top));
			}
		}

		public enum DebugMode
		{
			None,
			Console,
			String
		}
	}
}
