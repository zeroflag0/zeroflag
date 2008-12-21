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

		public static zeroflag.Windows.MouseButtons ConvertMouseButtons( button value )
		{
			zeroflag.Windows.MouseButtons converted = zeroflag.Windows.MouseButtons.None;
			if ( ( value & button.Left ) != 0 )
			{
				converted |= zeroflag.Windows.MouseButtons.MK_LBUTTON;
			}
			if ( ( value & button.Right ) != 0 )
			{
				converted |= zeroflag.Windows.MouseButtons.MK_RBUTTON;
			}
			if ( ( value & button.Middle ) != 0 )
			{
				converted |= zeroflag.Windows.MouseButtons.MK_MBUTTON;
			}
			if ( ( value & button.XButton1 ) != 0 )
			{
				converted |= zeroflag.Windows.MouseButtons.MK_XBUTTON1;
			}
			if ( ( value & button.XButton2 ) != 0 )
			{
				converted |= zeroflag.Windows.MouseButtons.MK_XBUTTON2;
			}
			return converted;
		}

		public static IntPtr ConvertMousePosition( Point location )
		{
			return new IntPtr( location.X | location.Y << 0x10 );
		}

		#region VK
		public enum VK : ushort
		{
			NONE = 0x00,
			/*
			 * Virtual Keys, Standard Set
			 */
			LBUTTON = 0x01,
			RBUTTON = 0x02,
			CANCEL = 0x03,
			MBUTTON = 0x04,    /* NOT contiguous with L & RBUTTON */

			XBUTTON1 = 0x05,    /* NOT contiguous with L & RBUTTON */
			XBUTTON2 = 0x06,    /* NOT contiguous with L & RBUTTON */

			/*
			 * 0x07 : unassigned
			 */

			BACK = 0x08,
			TAB = 0x09,

			/*
			 * 0x0A - 0x0B : reserved
			 */

			CLEAR = 0x0C,
			RETURN = 0x0D,

			SHIFT = 0x10,
			CONTROL = 0x11,
			MENU = 0x12,
			PAUSE = 0x13,
			CAPITAL = 0x14,

			KANA = 0x15,
			HANGEUL = 0x15,  /* old name - should be here for compatibility */
			HANGUL = 0x15,
			JUNJA = 0x17,
			FINAL = 0x18,
			HANJA = 0x19,
			KANJI = 0x19,

			ESCAPE = 0x1B,

			CONVERT = 0x1C,
			NONCONVERT = 0x1D,
			ACCEPT = 0x1E,
			MODECHANGE = 0x1F,

			SPACE = 0x20,
			PRIOR = 0x21,
			NEXT = 0x22,
			END = 0x23,
			HOME = 0x24,
			LEFT = 0x25,
			UP = 0x26,
			RIGHT = 0x27,
			DOWN = 0x28,
			SELECT = 0x29,
			PRINT = 0x2A,
			EXECUTE = 0x2B,
			SNAPSHOT = 0x2C,
			INSERT = 0x2D,
			DELETE = 0x2E,
			HELP = 0x2F,

			/*
			 * VK_0 - VK_9 are the same as ASCII '0' - '9' (0x30 - 0x39)
			 * 0x40 : unassigned
			 * VK_A - VK_Z are the same as ASCII 'A' - 'Z' (0x41 - 0x5A)
			 */
			D0 = 0x30,
			D1 = 0x31,
			D2 = 0x32,
			D3 = 0x33,
			D4 = 0x34,
			D5 = 0x35,
			D6 = 0x36,
			D7 = 0x37,
			D8 = 0x38,
			D9 = 0x39,

			A = 0x41,
			B = 0x42,
			C = 0x43,
			D = 0x44,
			E = 0x45,
			F = 0x46,
			G = 0x47,
			H = 0x48,
			I = 0x49,
			J = 0x4A,
			K = 0x4B,
			L = 0x4C,
			M = 0x4D,
			N = 0x4E,
			O = 0x4F,
			P = 0x50,
			Q = 0x51,
			R = 0x52,
			S = 0x53,
			T = 0x54,
			U = 0x55,
			V = 0x56,
			W = 0x57,
			X = 0x58,
			Y = 0x59,
			Z = 0x5A,

			LWIN = 0x5B,
			RWIN = 0x5C,
			APPS = 0x5D,

			/*
			 * 0x5E : reserved
			 */

			SLEEP = 0x5F,

			NUMPAD0 = 0x60,
			NUMPAD1 = 0x61,
			NUMPAD2 = 0x62,
			NUMPAD3 = 0x63,
			NUMPAD4 = 0x64,
			NUMPAD5 = 0x65,
			NUMPAD6 = 0x66,
			NUMPAD7 = 0x67,
			NUMPAD8 = 0x68,
			NUMPAD9 = 0x69,
			MULTIPLY = 0x6A,
			ADD = 0x6B,
			SEPARATOR = 0x6C,
			SUBTRACT = 0x6D,
			DECIMAL = 0x6E,
			DIVIDE = 0x6F,
			F1 = 0x70,
			F2 = 0x71,
			F3 = 0x72,
			F4 = 0x73,
			F5 = 0x74,
			F6 = 0x75,
			F7 = 0x76,
			F8 = 0x77,
			F9 = 0x78,
			F10 = 0x79,
			F11 = 0x7A,
			F12 = 0x7B,
			F13 = 0x7C,
			F14 = 0x7D,
			F15 = 0x7E,
			F16 = 0x7F,
			F17 = 0x80,
			F18 = 0x81,
			F19 = 0x82,
			F20 = 0x83,
			F21 = 0x84,
			F22 = 0x85,
			F23 = 0x86,
			F24 = 0x87,

			/*
			 * 0x88 - 0x8F : unassigned
			 */

			NUMLOCK = 0x90,
			SCROLL = 0x91,

			/*
			 * NEC PC-9800 kbd definitions
			 */
			OEM_NEC_EQUAL = 0x92,   // '=' key on numpad

			/*
			 * Fujitsu/OASYS kbd definitions
			 */
			OEM_FJ_JISHO = 0x92,   // 'Dictionary' key
			OEM_FJ_MASSHOU = 0x93,   // 'Unregister word' key
			OEM_FJ_TOUROKU = 0x94,   // 'Register word' key
			OEM_FJ_LOYA = 0x95,   // 'Left OYAYUBI' key
			OEM_FJ_ROYA = 0x96,   // 'Right OYAYUBI' key

			/*
			 * 0x97 - 0x9F : unassigned
			 */

			/*
			 * VK_L* & VK_R* - left and right Alt, Ctrl and Shift virtual keys.
			 * Used only as parameters to GetAsyncKeyState() and GetKeyState().
			 * No other API or message will distinguish left and right keys in this way.
			 */
			LSHIFT = 0xA0,
			RSHIFT = 0xA1,
			LCONTROL = 0xA2,
			RCONTROL = 0xA3,
			LMENU = 0xA4,
			RMENU = 0xA5,

			BROWSER_BACK = 0xA6,
			BROWSER_FORWARD = 0xA7,
			BROWSER_REFRESH = 0xA8,
			BROWSER_STOP = 0xA9,
			BROWSER_SEARCH = 0xAA,
			BROWSER_FAVORITES = 0xAB,
			BROWSER_HOME = 0xAC,

			VOLUME_MUTE = 0xAD,
			VOLUME_DOWN = 0xAE,
			VOLUME_UP = 0xAF,
			MEDIA_NEXT_TRACK = 0xB0,
			MEDIA_PREV_TRACK = 0xB1,
			MEDIA_STOP = 0xB2,
			MEDIA_PLAY_PAUSE = 0xB3,
			LAUNCH_MAIL = 0xB4,
			LAUNCH_MEDIA_SELECT = 0xB5,
			LAUNCH_APP1 = 0xB6,
			LAUNCH_APP2 = 0xB7,


			/*
			 * 0xB8 - 0xB9 : reserved
			 */

			OEM_1 = 0xBA,   // ';:' for US
			OEM_PLUS = 0xBB,   // '+' any country
			OEM_COMMA = 0xBC,   // ',' any country
			OEM_MINUS = 0xBD,   // '-' any country
			OEM_PERIOD = 0xBE,   // '.' any country
			OEM_2 = 0xBF,   // '/?' for US
			OEM_3 = 0xC0,   // '`~' for US

			/*
			 * 0xC1 - 0xD7 : reserved
			 */

			/*
			 * 0xD8 - 0xDA : unassigned
			 */

			OEM_4 = 0xDB,  //  '[{' for US
			OEM_5 = 0xDC,  //  '\|' for US
			OEM_6 = 0xDD,  //  ']}' for US
			OEM_7 = 0xDE,  //  ''"' for US
			OEM_8 = 0xDF,

			/*
			 * 0xE0 : reserved
			 */

			/*
			 * Various extended or enhanced keyboards
			 */
			OEM_AX = 0xE1,  //  'AX' key on Japanese AX kbd
			OEM_102 = 0xE2,  //  "<>" or "\|" on RT 102-key kbd.
			ICO_HELP = 0xE3,  //  Help key on ICO
			ICO_00 = 0xE4,  //  00 key on ICO

			PROCESSKEY = 0xE5,

			ICO_CLEAR = 0xE6,


			PACKET = 0xE7,

			/*
			 * 0xE8 : unassigned
			 */

			/*
			 * Nokia/Ericsson definitions
			 */
			OEM_RESET = 0xE9,
			OEM_JUMP = 0xEA,
			OEM_PA1 = 0xEB,
			OEM_PA2 = 0xEC,
			OEM_PA3 = 0xED,
			OEM_WSCTRL = 0xEE,
			OEM_CUSEL = 0xEF,
			OEM_ATTN = 0xF0,
			OEM_FINISH = 0xF1,
			OEM_COPY = 0xF2,
			OEM_AUTO = 0xF3,
			OEM_ENLW = 0xF4,
			OEM_BACKTAB = 0xF5,

			ATTN = 0xF6,
			CRSEL = 0xF7,
			EXSEL = 0xF8,
			EREOF = 0xF9,
			PLAY = 0xFA,
			ZOOM = 0xFB,
			NONAME = 0xFC,
			PA1 = 0xFD,
			OEM_CLEAR = 0xFE
		}
		#endregion VK

		#region ScanCode
		public enum ScanCode : ushort
		{
			NONE = 0x00,
			ESCAPE = 0x01,
			D1 = 0x02,
			D2 = 0x03,
			D3 = 0x04,
			D4 = 0x05,
			D5 = 0x06,
			D6 = 0x07,
			D7 = 0x08,
			D8 = 0x09,
			D9 = 0x0A,
			D0 = 0x0B,
			MINUS = 0x0C,    /* - on main keyboard */
			EQUALS = 0x0D,
			BACK = 0x0E,    /* backspace */
			TAB = 0x0F,
			Q = 0x10,
			W = 0x11,
			E = 0x12,
			R = 0x13,
			T = 0x14,
			Y = 0x15,
			U = 0x16,
			I = 0x17,
			O = 0x18,
			P = 0x19,
			LBRACKET = 0x1A,
			RBRACKET = 0x1B,
			RETURN = 0x1C,    /* Enter on main keyboard */
			LCONTROL = 0x1D,
			A = 0x1E,
			S = 0x1F,
			D = 0x20,
			F = 0x21,
			G = 0x22,
			H = 0x23,
			J = 0x24,
			K = 0x25,
			L = 0x26,
			SEMICOLON = 0x27,
			APOSTROPHE = 0x28,
			GRAVE = 0x29,    /* accent grave */
			LSHIFT = 0x2A,
			BACKSLASH = 0x2B,
			Z = 0x2C,
			X = 0x2D,
			C = 0x2E,
			V = 0x2F,
			B = 0x30,
			N = 0x31,
			M = 0x32,
			COMMA = 0x33,
			PERIOD = 0x34,    /* . on main keyboard */
			SLASH = 0x35,    /* / on main keyboard */
			RSHIFT = 0x36,
			MULTIPLY = 0x37,    /* * on numeric keypad */
			LMENU = 0x38,    /* left Alt */
			SPACE = 0x39,
			CAPITAL = 0x3A,
			F1 = 0x3B,
			F2 = 0x3C,
			F3 = 0x3D,
			F4 = 0x3E,
			F5 = 0x3F,
			F6 = 0x40,
			F7 = 0x41,
			F8 = 0x42,
			F9 = 0x43,
			F10 = 0x44,
			NUMLOCK = 0x45,
			SCROLL = 0x46,    /* Scroll Lock */
			NUMPAD7 = 0x47,
			NUMPAD8 = 0x48,
			NUMPAD9 = 0x49,
			SUBTRACT = 0x4A,    /* - on numeric keypad */
			NUMPAD4 = 0x4B,
			NUMPAD5 = 0x4C,
			NUMPAD6 = 0x4D,
			ADD = 0x4E,    /* + on numeric keypad */
			NUMPAD1 = 0x4F,
			NUMPAD2 = 0x50,
			NUMPAD3 = 0x51,
			NUMPAD0 = 0x52,
			DECIMAL = 0x53,    /* . on numeric keypad */
			OEM_102 = 0x56,    /* <> or \| on RT 102-key keyboard (Non-U.S.) */
			F11 = 0x57,
			F12 = 0x58,
			F13 = 0x64,    /*                     (NEC PC98) */
			F14 = 0x65,    /*                     (NEC PC98) */
			F15 = 0x66,    /*                     (NEC PC98) */
			KANA = 0x70,    /* (Japanese keyboard)            */
			ABNT_C1 = 0x73,    /* /? on Brazilian keyboard */
			CONVERT = 0x79,    /* (Japanese keyboard)            */
			NOCONVERT = 0x7B,    /* (Japanese keyboard)            */
			YEN = 0x7D,    /* (Japanese keyboard)            */
			ABNT_C2 = 0x7E,    /* Numpad . on Brazilian keyboard */
			NUMPADEQUALS = 0x8D,    /* = on numeric keypad (NEC PC98) */
			PREVTRACK = 0x90,    /* Previous Track (DIK_CIRCUMFLEX on Japanese keyboard) */
			AT = 0x91,    /*                     (NEC PC98) */
			COLON = 0x92,    /*                     (NEC PC98) */
			UNDERLINE = 0x93,    /*                     (NEC PC98) */
			KANJI = 0x94,    /* (Japanese keyboard)            */
			STOP = 0x95,    /*                     (NEC PC98) */
			AX = 0x96,    /*                     (Japan AX) */
			UNLABELED = 0x97,    /*                        (J3100) */
			NEXTTRACK = 0x99,    /* Next Track */
			NUMPADENTER = 0x9C,    /* Enter on numeric keypad */
			RCONTROL = 0x9D,
			MUTE = 0xA0,    /* Mute */
			CALCULATOR = 0xA1,    /* Calculator */
			PLAYPAUSE = 0xA2,    /* Play / Pause */
			MEDIASTOP = 0xA4,    /* Media Stop */
			VOLUMEDOWN = 0xAE,    /* Volume - */
			VOLUMEUP = 0xB0,    /* Volume + */
			WEBHOME = 0xB2,    /* Web home */
			NUMPADCOMMA = 0xB3,    /* , on numeric keypad (NEC PC98) */
			DIVIDE = 0xB5,    /* / on numeric keypad */
			SYSRQ = 0xB7,
			RMENU = 0xB8,    /* right Alt */
			PAUSE = 0xC5,    /* Pause */
			HOME = 0xC7,    /* Home on arrow keypad */
			UP = 0xC8,    /* UpArrow on arrow keypad */
			PRIOR = 0xC9,    /* PgUp on arrow keypad */
			LEFT = 0xCB,    /* LeftArrow on arrow keypad */
			RIGHT = 0xCD,    /* RightArrow on arrow keypad */
			END = 0xCF,    /* End on arrow keypad */
			DOWN = 0xD0,    /* DownArrow on arrow keypad */
			NEXT = 0xD1,    /* PgDn on arrow keypad */
			INSERT = 0xD2,    /* Insert on arrow keypad */
			DELETE = 0xD3,    /* Delete on arrow keypad */
			LWIN = 0xDB,    /* Left Windows key */
			RWIN = 0xDC,    /* Right Windows key */
			APPS = 0xDD,    /* AppMenu key */
			POWER = 0xDE,    /* System Power */
			SLEEP = 0xDF,    /* System Sleep */
			WAKE = 0xE3,    /* System Wake */
			WEBSEARCH = 0xE5,    /* Web Search */
			WEBFAVORITES = 0xE6,    /* Web Favorites */
			WEBREFRESH = 0xE7,    /* Web Refresh */
			WEBSTOP = 0xE8,    /* Web Stop */
			WEBFORWARD = 0xE9,    /* Web Forward */
			WEBBACK = 0xEA,    /* Web Back */
			MYCOMPUTER = 0xEB,    /* My Computer */
			MAIL = 0xEC,    /* Mail */
			MEDIASELECT = 0xED,    /* Media Select */

		}
		#endregion ScanCode

		#region KeyScanCodes

		#region ScanCode -> Key
#if NOT_WORKING
		static System.Collections.Generic.Dictionary<ScanCode, key> CreateScanCodeKeys()
		{
			System.Collections.Generic.Dictionary<ScanCode, key> codes = new System.Collections.Generic.Dictionary<ScanCode, key>();
			codes.Add(ScanCode.NONE, key.NoName);
			codes.Add(ScanCode.ESCAPE, key.Escape);
			codes.Add(ScanCode.D1, key.D1);
			codes.Add(ScanCode.D2, key.D2);
			codes.Add(ScanCode.D3, key.D3);
			codes.Add(ScanCode.D4, key.D4);
			codes.Add(ScanCode.D5, key.D5);
			codes.Add(ScanCode.D6, key.D6);
			codes.Add(ScanCode.D7, key.D7);
			codes.Add(ScanCode.D8, key.D8);
			codes.Add(ScanCode.D9, key.D9);
			codes.Add(ScanCode.D0, key.D0);
			codes.Add(ScanCode.MINUS, key.OemMinus);    /* - on main keyboard */
			codes.Add(ScanCode.EQUALS, key.Oemplus);
			codes.Add(ScanCode.BACK, key.Back);    /* backspace */
			codes.Add(ScanCode.TAB, key.Tab);
			codes.Add(ScanCode.Q, key.Q);
			codes.Add(ScanCode.W, key.W);
			codes.Add(ScanCode.E, key.E);
			codes.Add(ScanCode.R, key.R);
			codes.Add(ScanCode.T, key.T);
			codes.Add(ScanCode.Y, key.Y);
			codes.Add(ScanCode.U, key.U);
			codes.Add(ScanCode.I, key.I);
			codes.Add(ScanCode.O, key.O);
			codes.Add(ScanCode.P, key.P);
			codes.Add(ScanCode.LBRACKET, key.OemOpenBrackets);
			codes.Add(ScanCode.RBRACKET, key.OemCloseBrackets);
			codes.Add(ScanCode.RETURN, key.Enter);    /* Enter on main keyboard */
			codes.Add(ScanCode.LCONTROL, key.LControlKey);
			codes.Add(ScanCode.A, key.A);
			codes.Add(ScanCode.S, key.S);
			codes.Add(ScanCode.D, key.D);
			codes.Add(ScanCode.F, key.F);
			codes.Add(ScanCode.G, key.G);
			codes.Add(ScanCode.H, key.H);
			codes.Add(ScanCode.J, key.J);
			codes.Add(ScanCode.K, key.K);
			codes.Add(ScanCode.L, key.L);
			codes.Add(ScanCode.SEMICOLON, key.OemSemicolon);
			codes.Add(ScanCode.APOSTROPHE, key.OemQuotes);
			codes.Add(ScanCode.GRAVE, key.Oemtilde);    /* accent grave */
			codes.Add(ScanCode.LSHIFT, key.LShiftKey);
			codes.Add(ScanCode.BACKSLASH, key.OemBackslash);
			codes.Add(ScanCode.Z, key.Z);
			codes.Add(ScanCode.X, key.X);
			codes.Add(ScanCode.C, key.C);
			codes.Add(ScanCode.V, key.V);
			codes.Add(ScanCode.B, key.B);
			codes.Add(ScanCode.N, key.N);
			codes.Add(ScanCode.M, key.M);
			codes.Add(ScanCode.COMMA, key.Oemcomma);
			codes.Add(ScanCode.PERIOD, key.OemPeriod);    /* . on main keyboard */
			codes.Add(ScanCode.SLASH, key.OemQuestion);    /* / on main keyboard */
			codes.Add(ScanCode.RSHIFT, key.RShiftKey);
			codes.Add(ScanCode.MULTIPLY, key.Multiply);    /* * on numeric keypad */
			codes.Add(ScanCode.LMENU, key.Alt);    /* left Alt */
			codes.Add(ScanCode.SPACE, key.Space);
			codes.Add(ScanCode.CAPITAL, key.Capital);
			codes.Add(ScanCode.F1, key.F1);
			codes.Add(ScanCode.F2, key.F2);
			codes.Add(ScanCode.F3, key.F3);
			codes.Add(ScanCode.F4, key.F4);
			codes.Add(ScanCode.F5, key.F5);
			codes.Add(ScanCode.F6, key.F6);
			codes.Add(ScanCode.F7, key.F7);
			codes.Add(ScanCode.F8, key.F8);
			codes.Add(ScanCode.F9, key.F9);
			codes.Add(ScanCode.F10, key.F10);
			codes.Add(ScanCode.NUMLOCK, key.NumLock);
			codes.Add(ScanCode.SCROLL, key.Scroll);    /* Scroll Lock */
			codes.Add(ScanCode.NUMPAD7, key.NumPad7);
			codes.Add(ScanCode.NUMPAD8, key.NumPad8);
			codes.Add(ScanCode.NUMPAD9, key.NumPad9);
			codes.Add(ScanCode.SUBTRACT, key.Subtract);    /* - on numeric keypad */
			codes.Add(ScanCode.NUMPAD4, key.NumPad4);
			codes.Add(ScanCode.NUMPAD5, key.NumPad5);
			codes.Add(ScanCode.NUMPAD6, key.NumPad6);
			codes.Add(ScanCode.ADD, key.Add);    /* + on numeric keypad */
			codes.Add(ScanCode.NUMPAD1, key.NumPad1);
			codes.Add(ScanCode.NUMPAD2, key.NumPad2);
			codes.Add(ScanCode.NUMPAD3, key.NumPad3);
			codes.Add(ScanCode.NUMPAD0, key.NumPad0);
			codes.Add(ScanCode.DECIMAL, key.Decimal);    /* . on numeric keypad */
			codes.Add(ScanCode.OEM_102, key.Oem102);    /* <> or \| on RT 102-key keyboard (Non-U.S.) */
			codes.Add(ScanCode.F11, key.F11);
			codes.Add(ScanCode.F12, key.F12);
			codes.Add(ScanCode.F13, key.F13);    /*                     (NEC PC98) */
			codes.Add(ScanCode.F14, key.F14);    /*                     (NEC PC98) */
			codes.Add(ScanCode.F15, key.F15);    /*                     (NEC PC98) */
			codes.Add(ScanCode.KANA, key.KanaMode);    /* (Japanese keyboard)            */
			codes.Add(ScanCode.ABNT_C1, key.FinalMode);    /* /? on Brazilian keyboard */
			codes.Add(ScanCode.CONVERT, key.FinalMode);    /* (Japanese keyboard)            */
			codes.Add(ScanCode.NOCONVERT, key.FinalMode);    /* (Japanese keyboard)            */
			//YEN = 0x7D,    /* (Japanese keyboard)            */
			//ABNT_C2 = 0x7E,    /* Numpad . on Brazilian keyboard */
			//NUMPADEQUALS = 0x8D,    /* = on numeric keypad (NEC PC98) */
			//PREVTRACK = 0x90,    /* Previous Track (DIK_CIRCUMFLEX on Japanese keyboard) */
			//AT = 0x91,    /*                     (NEC PC98) */
			//COLON = 0x92,    /*                     (NEC PC98) */
			//UNDERLINE = 0x93,    /*                     (NEC PC98) */
			//KANJI = 0x94,    /* (Japanese keyboard)            */
			//STOP = 0x95,    /*                     (NEC PC98) */
			//AX = 0x96,    /*                     (Japan AX) */
			//UNLABELED = 0x97,    /*                        (J3100) */
			//NEXTTRACK = 0x99,    /* Next Track */
			codes.Add(ScanCode.NUMPADENTER, key.Enter);    /* Enter on numeric keypad */
			codes.Add(ScanCode.RCONTROL, key.RControlKey);
			//MUTE = 0xA0,    /* Mute */
			//CALCULATOR = 0xA1,    /* Calculator */
			//PLAYPAUSE = 0xA2,    /* Play / Pause */
			//MEDIASTOP = 0xA4,    /* Media Stop */
			//VOLUMEDOWN = 0xAE,    /* Volume - */
			//VOLUMEUP = 0xB0,    /* Volume + */
			//WEBHOME = 0xB2,    /* Web home */
			codes.Add(ScanCode.NUMPADCOMMA, key.Decimal);    /* , on numeric keypad (NEC PC98) */
			codes.Add(ScanCode.DIVIDE, key.Divide);    /* / on numeric keypad */
			//SYSRQ = 0xB7,
			codes.Add(ScanCode.RMENU, key.Alt);    /* right Alt */
			codes.Add(ScanCode.PAUSE, key.Pause);    /* Pause */
			codes.Add(ScanCode.HOME, key.Home);    /* Home on arrow keypad */
			codes.Add(ScanCode.UP, key.Up);    /* UpArrow on arrow keypad */
			codes.Add(ScanCode.PRIOR, key.PageUp);    /* PgUp on arrow keypad */
			codes.Add(ScanCode.LEFT, key.Left);    /* LeftArrow on arrow keypad */
			codes.Add(ScanCode.RIGHT, key.Right);    /* RightArrow on arrow keypad */
			codes.Add(ScanCode.END, key.End);    /* End on arrow keypad */
			codes.Add(ScanCode.DOWN, key.Down);    /* DownArrow on arrow keypad */
			codes.Add(ScanCode.NEXT, key.PageDown);    /* PgDn on arrow keypad */
			codes.Add(ScanCode.INSERT, key.Insert);    /* Insert on arrow keypad */
			codes.Add(ScanCode.DELETE, key.Delete);    /* Delete on arrow keypad */
			codes.Add(ScanCode.LWIN, key.LWin);    /* Left Windows key */
			codes.Add(ScanCode.RWIN, key.RWin);    /* Right Windows key */
			//APPS = 0xDD,    /* AppMenu key */
			//POWER = 0xDE,    /* System Power */
			//SLEEP = 0xDF,    /* System Sleep */
			//WAKE = 0xE3,    /* System Wake */
			//WEBSEARCH = 0xE5,    /* Web Search */
			//WEBFAVORITES = 0xE6,    /* Web Favorites */
			//WEBREFRESH = 0xE7,    /* Web Refresh */
			//WEBSTOP = 0xE8,    /* Web Stop */
			//WEBFORWARD = 0xE9,    /* Web Forward */
			//WEBBACK = 0xEA,    /* Web Back */
			//MYCOMPUTER = 0xEB,    /* My Computer */
			//MAIL = 0xEC,    /* Mail */
			//MEDIASELECT = 0xED,    /* Media Select */

			return codes;
		}

		static System.Collections.Generic.Dictionary<ScanCode, key> _ScanCodeKeys;

		public static System.Collections.Generic.Dictionary<ScanCode, key> ScanCodeKeys
		{
			get { return WinAPI._ScanCodeKeys ?? (WinAPI._ScanCodeKeys = WinAPI.CreateScanCodeKeys()); }
		}
#endif//NOT_WORKING
		#endregion

		static System.Collections.Generic.Dictionary<key, ScanCode> CreateKeyScanCodes()
		{
			System.Collections.Generic.Dictionary<key, ScanCode> codes = new System.Collections.Generic.Dictionary<key, ScanCode>();
			codes.Add( key.NoName, ScanCode.NONE );
			codes.Add( key.Escape, ScanCode.ESCAPE );
			codes.Add( key.D1, ScanCode.D1 );
			codes.Add( key.D2, ScanCode.D2 );
			codes.Add( key.D3, ScanCode.D3 );
			codes.Add( key.D4, ScanCode.D4 );
			codes.Add( key.D5, ScanCode.D5 );
			codes.Add( key.D6, ScanCode.D6 );
			codes.Add( key.D7, ScanCode.D7 );
			codes.Add( key.D8, ScanCode.D8 );
			codes.Add( key.D9, ScanCode.D9 );
			codes.Add( key.D0, ScanCode.D0 );
			codes.Add( key.OemMinus, ScanCode.MINUS );    /* - on main keyboard */
			codes.Add( key.Oemplus, ScanCode.EQUALS );
			codes.Add( key.Back, ScanCode.BACK );    /* backspace */
			codes.Add( key.Tab, ScanCode.TAB );
			codes.Add( key.Q, ScanCode.Q );
			codes.Add( key.W, ScanCode.W );
			codes.Add( key.E, ScanCode.E );
			codes.Add( key.R, ScanCode.R );
			codes.Add( key.T, ScanCode.T );
			codes.Add( key.Y, ScanCode.Y );
			codes.Add( key.U, ScanCode.U );
			codes.Add( key.I, ScanCode.I );
			codes.Add( key.O, ScanCode.O );
			codes.Add( key.P, ScanCode.P );
			codes.Add( key.OemOpenBrackets, ScanCode.LBRACKET );
			codes.Add( key.OemCloseBrackets, ScanCode.RBRACKET );
			codes.Add( key.Enter, ScanCode.RETURN );    /* Enter on main keyboard */
			codes.Add( key.LControlKey, ScanCode.LCONTROL );
			codes.Add( key.A, ScanCode.A );
			codes.Add( key.S, ScanCode.S );
			codes.Add( key.D, ScanCode.D );
			codes.Add( key.F, ScanCode.F );
			codes.Add( key.G, ScanCode.G );
			codes.Add( key.H, ScanCode.H );
			codes.Add( key.J, ScanCode.J );
			codes.Add( key.K, ScanCode.K );
			codes.Add( key.L, ScanCode.L );
			codes.Add( key.OemSemicolon, ScanCode.SEMICOLON );
			codes.Add( key.OemQuotes, ScanCode.APOSTROPHE );
			codes.Add( key.Oemtilde, ScanCode.GRAVE );    /* accent grave */
			codes.Add( key.LShiftKey, ScanCode.LSHIFT );
			codes.Add( key.OemBackslash, ScanCode.BACKSLASH );
			codes.Add( key.Z, ScanCode.Z );
			codes.Add( key.X, ScanCode.X );
			codes.Add( key.C, ScanCode.C );
			codes.Add( key.V, ScanCode.V );
			codes.Add( key.B, ScanCode.B );
			codes.Add( key.N, ScanCode.N );
			codes.Add( key.M, ScanCode.M );
			codes.Add( key.Oemcomma, ScanCode.COMMA );
			codes.Add( key.OemPeriod, ScanCode.PERIOD );    /* . on main keyboard */
			codes.Add( key.OemQuestion, ScanCode.SLASH );    /* / on main keyboard */
			codes.Add( key.RShiftKey, ScanCode.RSHIFT );
			codes.Add( key.Multiply, ScanCode.MULTIPLY );    /* * on numeric keypad */
			codes.Add( key.Alt, ScanCode.LMENU );    /* left Alt */
			codes.Add( key.Space, ScanCode.SPACE );
			codes.Add( key.Capital, ScanCode.CAPITAL );
			codes.Add( key.F1, ScanCode.F1 );
			codes.Add( key.F2, ScanCode.F2 );
			codes.Add( key.F3, ScanCode.F3 );
			codes.Add( key.F4, ScanCode.F4 );
			codes.Add( key.F5, ScanCode.F5 );
			codes.Add( key.F6, ScanCode.F6 );
			codes.Add( key.F7, ScanCode.F7 );
			codes.Add( key.F8, ScanCode.F8 );
			codes.Add( key.F9, ScanCode.F9 );
			codes.Add( key.F10, ScanCode.F10 );
			codes.Add( key.NumLock, ScanCode.NUMLOCK );
			codes.Add( key.Scroll, ScanCode.SCROLL );    /* Scroll Lock */
			codes.Add( key.NumPad7, ScanCode.NUMPAD7 );
			codes.Add( key.NumPad8, ScanCode.NUMPAD8 );
			codes.Add( key.NumPad9, ScanCode.NUMPAD9 );
			codes.Add( key.Subtract, ScanCode.SUBTRACT );    /* - on numeric keypad */
			codes.Add( key.NumPad4, ScanCode.NUMPAD4 );
			codes.Add( key.NumPad5, ScanCode.NUMPAD5 );
			codes.Add( key.NumPad6, ScanCode.NUMPAD6 );
			codes.Add( key.Add, ScanCode.ADD );    /* + on numeric keypad */
			codes.Add( key.NumPad1, ScanCode.NUMPAD1 );
			codes.Add( key.NumPad2, ScanCode.NUMPAD2 );
			codes.Add( key.NumPad3, ScanCode.NUMPAD3 );
			codes.Add( key.NumPad0, ScanCode.NUMPAD0 );
			codes.Add( key.Decimal, ScanCode.DECIMAL );    /* . on numeric keypad */
			//same as backslash: codes.Add(key.Oem102, ScanCode.OEM_102);    /* <> or \| on RT 102-key keyboard (Non-U.S.) */
			codes.Add( key.F11, ScanCode.F11 );
			codes.Add( key.F12, ScanCode.F12 );
			codes.Add( key.F13, ScanCode.F13 );    /*                     (NEC PC98) */
			codes.Add( key.F14, ScanCode.F14 );    /*                     (NEC PC98) */
			codes.Add( key.F15, ScanCode.F15 );    /*                     (NEC PC98) */
			//codes.Add(key.KanaMode, ScanCode.KANA);    /* (Japanese keyboard)            */
			//codes.Add(key.FinalMode, ScanCode.ABNT_C1);    /* /? on Brazilian keyboard */
			//codes.Add(key.FinalMode, ScanCode.CONVERT);    /* (Japanese keyboard)            */
			//codes.Add(key.FinalMode, ScanCode.NOCONVERT);    /* (Japanese keyboard)            */
			//same as Return: codes.Add(key.Enter, ScanCode.NUMPADENTER);    /* Enter on numeric keypad */
			codes.Add( key.RControlKey, ScanCode.RCONTROL );
			//codes.Add(key.Decimal, ScanCode.NUMPADCOMMA);    /* , on numeric keypad (NEC PC98) */
			codes.Add( key.Divide, ScanCode.DIVIDE );    /* / on numeric keypad */
			codes.Add( key.RMenu, ScanCode.RMENU );    /* right Alt */
			codes.Add( key.Pause, ScanCode.PAUSE );    /* Pause */
			codes.Add( key.Home, ScanCode.HOME );    /* Home on arrow keypad */
			codes.Add( key.Up, ScanCode.UP );    /* UpArrow on arrow keypad */
			codes.Add( key.PageUp, ScanCode.PRIOR );    /* PgUp on arrow keypad */
			codes.Add( key.Left, ScanCode.LEFT );    /* LeftArrow on arrow keypad */
			codes.Add( key.Right, ScanCode.RIGHT );    /* RightArrow on arrow keypad */
			codes.Add( key.End, ScanCode.END );    /* End on arrow keypad */
			codes.Add( key.Down, ScanCode.DOWN );    /* DownArrow on arrow keypad */
			codes.Add( key.PageDown, ScanCode.NEXT );    /* PgDn on arrow keypad */
			codes.Add( key.Insert, ScanCode.INSERT );    /* Insert on arrow keypad */
			codes.Add( key.Delete, ScanCode.DELETE );    /* Delete on arrow keypad */
			codes.Add( key.LWin, ScanCode.LWIN );    /* Left Windows key */
			codes.Add( key.RWin, ScanCode.RWIN );    /* Right Windows key */

			return codes;
		}

		static System.Collections.Generic.Dictionary<key, ScanCode> _KeyScanCodes;

		public static System.Collections.Generic.Dictionary<key, ScanCode> KeyScanCodes
		{
			get { return WinAPI._KeyScanCodes ?? ( WinAPI._KeyScanCodes = WinAPI.CreateKeyScanCodes() ); }
		}

		#endregion KeyScanCodes

		[StructLayout( LayoutKind.Sequential )]
		struct MOUSEINPUT
		{
			public int dx;
			public int dy;
			public uint mouseData;
			public InputMouseFlags Flags;
			public uint time;
			public IntPtr dwExtraInfo;
		}

		[StructLayout( LayoutKind.Sequential )]
		struct KEYBDINPUT
		{
			public VK Vk;
			public ScanCode ScanCode;
			public InputKeyFlags Flags;
			public uint time;
			public IntPtr dwExtraInfo;
		}

		[StructLayout( LayoutKind.Sequential )]
		struct HARDWAREINPUT
		{
			public uint uMsg;
			public ushort wParamL;
			public ushort wParamH;
		}

		[StructLayout( LayoutKind.Explicit, Size = 28 )]
		struct INPUT
		{
			[FieldOffset( 0 )]
			public InputSource type;
			[FieldOffset( 4 )]
			public MOUSEINPUT mi;
			[FieldOffset( 4 )]
			public KEYBDINPUT ki;
			[FieldOffset( 4 )]
			public HARDWAREINPUT hi;
		}

		enum InputSource : int
		{
			MOUSE = 0,
			KEYBOARD = 1,
			HARDWARE = 2,
		}
		enum InputKeyFlags : uint
		{
			KEYDOWN = 0x0000,
			EXTENDEDKEY = 0x0001,
			KEYUP = 0x0002,
			UNICODE = 0x0004,
			SCANCODE = 0x0008,
		}
		const uint XBUTTON1 = 0x0001;
		const uint XBUTTON2 = 0x0002;
		enum InputMouseFlags : uint
		{
			MOVE = 0x0001,
			LEFTDOWN = 0x0002,
			LEFTUP = 0x0004,
			RIGHTDOWN = 0x0008,
			RIGHTUP = 0x0010,
			MIDDLEDOWN = 0x0020,
			MIDDLEUP = 0x0040,
			XDOWN = 0x0080,
			XUP = 0x0100,
			WHEEL = 0x0800,
			VIRTUALDESK = 0x4000,
			ABSOLUTE = 0x8000,
		}
		//[System.Runtime.InteropServices.StructLayout(LayoutKind.Sequential)]
		//private struct KEYBOARD_INPUT
		//{
		//    //public uint type;
		//    public InputSource type;
		//    public ushort vk;
		//    public ushort scanCode;
		//    public uint flags;
		//    public uint time;
		//    public uint extrainfo;
		//    public uint padding1;
		//    public uint padding2;
		//}

		//[System.Runtime.InteropServices.DllImport("User32.dll")]
		//private static extern uint SendInput(uint numberOfInputs, [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPArray, SizeConst = 1)] KEYBOARD_INPUT[] input, int structSize);

		[System.Runtime.InteropServices.DllImport( "User32.dll" )]
		private static extern uint SendInput( uint numberOfInputs, [System.Runtime.InteropServices.MarshalAs( System.Runtime.InteropServices.UnmanagedType.LPArray, SizeConst = 1 )] INPUT[] input, int structSize );

		public static void KeyDown( VK vk )
		{
			SendKey( vk, true );
		}

		public static void KeyUp( VK vk )
		{
			SendKey( vk, false );
		}

		public static void SendKey( VK vk, bool press )
		{
			SendKey( vk, ScanCode.NONE, press );
		}

		public static void KeyDown( ScanCode scanCode )
		{
			SendKey( scanCode, true );
		}

		public static void KeyUp( ScanCode scanCode )
		{
			SendKey( scanCode, false );
		}

		public static void SendKey( ScanCode scanCode, bool press )
		{
			SendKey( VK.NONE, scanCode, press );
		}

		public static void KeyDown( key key )
		{
			SendKey( key, true );
		}

		public static void KeyUp( key key )
		{
			SendKey( key, false );
		}

		public static void SendKey( key key, bool press )
		{
			ScanCode code;
			if ( KeyScanCodes.ContainsKey( key ) )
				code = KeyScanCodes[key];
			else
				code = (ScanCode)(int)key;
			SendKey( VK.NONE, code, press );
		}

		public static void SendKey( VK vk, ScanCode scanCode, bool press )
		{
			INPUT[] input = new INPUT[1];
			input[0].ki = new KEYBDINPUT();
			input[0].type = InputSource.KEYBOARD;
			if ( scanCode > 0 )
				input[0].ki.Flags = InputKeyFlags.SCANCODE;

			if ( ( (int)scanCode & 0xFF00 ) == 0xE000 )
			{ // extended key?
				input[0].ki.Flags |= InputKeyFlags.EXTENDEDKEY;
			}

			input[0].ki.Vk = vk;

			if ( press )
			{ // press?
				input[0].ki.ScanCode = scanCode;
				Console.Write( "[" + scanCode + "]v " );
			}
			else
			{ // release?
				input[0].ki.ScanCode = scanCode;
				input[0].ki.Flags |= InputKeyFlags.KEYUP;
				Console.Write( "[" + scanCode + "]^ " );
			}

			uint result = SendInput( 1, input, Marshal.SizeOf( input[0] ) );

			if ( result != 1 )
			{
				throw new Exception( "Could not send key: " + scanCode );
			}
		}

		//[DllImport("user32.dll")]
		//public static extern IntPtr GetActiveWindow();
		[DllImport( "user32.dll" )]
		public static extern IntPtr GetForegroundWindow();
		[DllImport( "user32.dll" )]
		public static extern bool SetForegroundWindow( IntPtr handle );

		[DllImport( "user32.dll" )]
		public static extern IntPtr FindWindow( string lpClassName, string lpWindowName );

		[DllImport( "user32.dll" )]
		internal unsafe static extern int GetWindowText( IntPtr hWnd, byte* lpString, int nMaxCount );


		[DllImport( "user32.dll" )]
		public static extern bool GetClientRect( IntPtr hWnd, ref LPRECT lpRect );
		[DllImport( "user32.dll" )]
		public static extern ulong GetLastError();
		[DllImport( "user32.dll" )]
		public static extern bool RegisterHotKey( IntPtr hWnd, int id, uint fsModifiers, uint vk );
		[DllImport( "user32.dll" )]
		public static extern bool RegisterHotKey( IntPtr hWnd, int id, ModKeys fsModifiers, key vk );
		[DllImport( "user32.dll" )]
		public static extern uint SendMessage( IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam );
		[DllImport( "user32.dll" )]
		public static extern uint SendMessage( IntPtr hWnd, WindowsMessages msg, IntPtr wParam, IntPtr lParam );
		[DllImport( "user32.dll" )]
		public static extern bool UnregisterHotKey( IntPtr hWnd, int id );

		[DllImport( "user32.dll" )]
		public static extern IntPtr SetFocus( IntPtr hWnd );

		public static IntPtr NULL
		{
			get
			{
				return IntPtr.Zero;
			}
		}

		[StructLayout( LayoutKind.Sequential )]
		public struct LPRECT
		{
			public long left;
			public long top;
			public long right;
			public long bottom;
		}

		[DllImport( "kernel32.dll" )]
		public static extern void GetSystemInfo( [MarshalAs( UnmanagedType.Struct )] ref SystemInfo lpSystemInfo );

		[StructLayout( LayoutKind.Sequential )]
		public struct SystemInfo
		{
			public ProcessorInfo ProcessorInfo;
			public uint PageSize;
			public UInt32 MinimumApplicationAddress;
			public UInt32 MaximumApplicationAddress;
			public UInt32 ActiveProcessorMask;
			public uint NumberOfProcessors;
			public uint ProcessorType;
			public uint AllocationGranularity;
			public ushort ProcessorLevel;
			public ushort ProcessorRevision;
		}

		[StructLayout( LayoutKind.Explicit )]
		public struct ProcessorInfo
		{
			[FieldOffset( 0 )]
			public uint OemId;
			[FieldOffset( 0 )]
			public ProcessorArchitecture ProcessorArchitecture;
			[FieldOffset( 2 )]
			public ushort Reserved;
		}

		public enum ProcessorArchitecture : ushort
		{
			/// <summary>
			/// x64 (AMD or Intel)
			/// </summary>
			x64 = 9,


			/// <summary>
			/// Intel Itanium Processor Family (IPF)
			/// </summary>
			IA64 = 6,

			/// <summary>
			/// x86
			/// </summary>
			x86 = 0,

			/// <summary>
			/// Unknown architecture.
			/// </summary>
			Unknown = 0xffff,
		}
	}
}
