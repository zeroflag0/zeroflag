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
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Runtime.CompilerServices;
	using System.Windows.Forms;

	public class HotkeyForm : Form
	{
		private IContainer components;
		private Dictionary<Keys, Hotkey> _Hotkeys = new Dictionary<Keys, Hotkey>();
		//private zeroflag.Windows.Window _Window;

		public HotkeyForm()
		{
			this.InitializeComponent();
		}

		protected override void CreateHandle()
		{
			//base.CreateHandle();
			//this._Window = new zeroflag.Windows.Window(base.Handle);
			//this.RegisterHotkeys();
		}

		protected override void Dispose( bool disposing )
		{
			this.UnregisterHotkeys();
			if ( disposing && ( this.components != null ) )
			{
				this.components.Dispose();
			}
			base.Dispose( disposing );
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

		protected override void OnClosed( EventArgs e )
		{
			this.UnregisterHotkeys();
			base.OnClosed( e );
		}

		public void RegisterHotkeys()
		{
			foreach ( Hotkey key in this.Hotkeys.Values )
			{
				this.RegisterHotkey( key );
			}
		}

		public Hotkey RegisterHotkey( Keys key )
		{
			return this.RegisterHotkey( new Hotkey( key ) );
		}
		public Hotkey RegisterHotkey( Keys key, KeyEventHandler callback )
		{
			return this.RegisterHotkey( new Hotkey( key, callback ) );
		}

		public Hotkey RegisterHotkey( Hotkey key )
		{
			if ( key == null )
				return key;
			if ( !this.Hotkeys.ContainsValue( key ) )
			{
				if ( this.Hotkeys.ContainsKey( key.Key ) )
				{
					this.UnregisterHotkey( key.Key );
					this.Hotkeys.Remove( key.Key );
				}
				this.Hotkeys[ key.Key ] = key;
			}
			bool failSilently = false;
			if ( key.Registered )
			{
				failSilently = true;
			}
			Keys code = key.Key;
			ModKeys mod = ModKeys.None;
			if ( ( code & Keys.Control ) != 0 )
			{
				code ^= Keys.Control;
				mod |= ModKeys.MOD_CONTROL;
			}
			if ( ( code & Keys.Alt ) != 0 )
			{
				code ^= Keys.Alt;
				mod |= ModKeys.MOD_ALT;
			}
			if ( ( code & Keys.Shift ) != 0 )
			{
				code ^= Keys.Shift;
				mod |= ModKeys.MOD_SHIFT;
			}

			if ( !WinAPI.RegisterHotKey( base.Handle, key.ID, mod, code ) )
			{
				if ( !failSilently )
					MessageBox.Show( "Failed to register hotkey." );
			}
			key.Registered = true;
			return key;
		}

		public void UnregisterHotkeys()
		{
			foreach ( Hotkey key in this.Hotkeys.Values )
			{
				this.UnregisterHotkey( key );
			}
		}

		public void UnregisterHotkey( Hotkey key )
		{
			if ( key.Registered )
			{
				WinAPI.UnregisterHotKey( base.Handle, key.ID );
				key.Registered = false;
			}
		}

		public void UnregisterHotkey( Keys key )
		{
			this.UnregisterHotkey( this.Hotkeys[ key ] );
		}

		protected override void WndProc( ref Message m )
		{
			if ( m.Msg == (int)WindowsMessages.WM_HOTKEY )
			{
				uint param = (uint)( (int)m.LParam );
				uint mod = param;
				mod = (ushort)mod;
				param = param >> 0x10;
				Keys key = (Keys)param;
				ModKeys mkey = (ModKeys)mod;

				if ( ( mkey & ModKeys.MOD_CONTROL ) != 0 )
				{
					key |= Keys.Control;
				}
				if ( ( mkey & ModKeys.MOD_ALT ) != 0 )
				{
					key |= Keys.Alt;
				}
				if ( ( mkey & ModKeys.MOD_SHIFT ) != 0 )
				{
					key |= Keys.Shift;
				}

				if ( this.Hotkeys.ContainsKey( key ) )
				{
					this.Hotkeys[ key ].OnHotkeyPressed( new KeyEventArgs( key ) );
				}
			}
			base.WndProc( ref m );
		}

		public Dictionary<Keys, Hotkey> Hotkeys
		{
			get
			{
				return this._Hotkeys;
			}
		}

		//public zeroflag.Windows.Window Window
		//{
		//    get
		//    {
		//        return this._Window;
		//    }
		//}

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

			public Hotkey( Keys key )
				: this()
			{
				this.Key = key;
			}

			public Hotkey( Keys key, KeyEventHandler callback )
				: this( key )
			{
				this.HotkeyPressed += callback;
			}

			public void OnHotkeyPressed( KeyEventArgs e )
			{
				if ( this.m_HotkeyPressed != null )
				{
					this.m_HotkeyPressed( this, e );
				}
			}
		}

	}
}
