namespace zeroflag.Windows
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Runtime.CompilerServices;
	using System.Windows.Forms;

	public class HotkeyForm : Form
	{
		private IContainer components;
		private Dictionary<Keys, Hotkey> m_Hotkeys = new Dictionary<Keys, Hotkey>();
		private zeroflag.Windows.Window m_Window;

		public HotkeyForm()
		{
			this.InitializeComponent();
		}

		protected override void CreateHandle()
		{
			base.CreateHandle();
			this.m_Window = new zeroflag.Windows.Window(base.Handle);
			this.RegisterHotkeys();
		}

		protected override void Dispose(bool disposing)
		{
			this.UnregisterHotkeys();
			if (disposing && (this.components != null))
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		~HotkeyForm()
		{
			this.UnregisterHotkeys();
		}

		private void InitializeComponent()
		{
			this.components = new Container();
			base.AutoScaleMode = AutoScaleMode.Font;
			this.Text = "HotkeyForm";
		}

		protected override void OnClosed(EventArgs e)
		{
			this.UnregisterHotkeys();
			base.OnClosed(e);
		}

		protected void RegisterHotkeys()
		{
			foreach (Hotkey key in this.Hotkeys.Values)
			{
				if (key.Registered)
				{
					continue;
				}
				Keys code = key.Key;
				ModKeys mod = ModKeys.None;
				if ((code & Keys.Control) != 0)
				{
					code ^= Keys.Control;
					mod |= ModKeys.MOD_CONTROL;
				}
				if ((code & Keys.Alt) != 0)
				{
					code ^= Keys.Alt;
					mod |= ModKeys.MOD_ALT;
				}
				if ((code & Keys.Shift) != 0)
				{
					code ^= Keys.Shift;
					mod |= ModKeys.MOD_SHIFT;
				}

				if (!WinAPI.RegisterHotKey(base.Handle, key.ID, mod, code))
				{
					MessageBox.Show("Failed to register hotkey.");
				}
				key.Registered = true;
			}
		}

		protected void UnregisterHotkeys()
		{
			foreach (Hotkey key in this.Hotkeys.Values)
			{
				if (key.Registered)
				{
					WinAPI.UnregisterHotKey(base.Handle, key.ID);
					key.Registered = false;
				}
			}
		}

		protected override void WndProc(ref Message m)
		{
			if (m.Msg == (int)WindowsMessages.WM_HOTKEY)
			{
				uint param = (uint)((int)m.LParam);
				uint mod = param;
				mod = (ushort)mod;
				param = param >> 0x10;
				Keys key = (Keys)param;
				ModKeys mkey = (ModKeys)mod;

				if ((mkey & ModKeys.MOD_CONTROL) != 0)
				{
					key |= Keys.Control;
				}
				if ((mkey & ModKeys.MOD_ALT) != 0)
				{
					key |= Keys.Alt;
				}
				if ((mkey & ModKeys.MOD_SHIFT) != 0)
				{
					key |= Keys.Shift;
				}

				if (this.Hotkeys.ContainsKey(key))
				{
					this.Hotkeys[key].OnHotkeyPressed(new KeyEventArgs(key));
				}
			}
			base.WndProc(ref m);
		}

		protected Dictionary<Keys, Hotkey> Hotkeys
		{
			get
			{
				return this.m_Hotkeys;
			}
		}

		public zeroflag.Windows.Window Window
		{
			get
			{
				return this.m_Window;
			}
		}

		public class Hotkey
		{
			private static int Counter;
			public int ID;
			private Keys key;

			public Keys Key
			{
				get { return key; }
				set { key = value; }
			}
			bool registered = false;
			[System.Xml.Serialization.XmlIgnore]
			public bool Registered
			{
				get { return registered; }
				set { registered = value; }
			}

			public event KeyEventHandler HotkeyPressed
			{
				add { m_HotkeyPressed += value; }
				remove { m_HotkeyPressed -= value; }
			}

			private event KeyEventHandler m_HotkeyPressed;

			public Hotkey()
			{
				this.m_HotkeyPressed = null;
				this.Registered = false;
				this.ID = Counter++;
			}

			public Hotkey(Keys key)
				: this()
			{
				this.Key = key;
			}

			public Hotkey(Keys key, KeyEventHandler callback)
				: this(key)
			{
				this.HotkeyPressed += callback;
			}

			public void OnHotkeyPressed(KeyEventArgs e)
			{
				if (this.m_HotkeyPressed != null)
				{
					this.m_HotkeyPressed(this, e);
				}
			}
		}
	}
}
