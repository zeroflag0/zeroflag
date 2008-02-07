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

using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.IRC.Messages.Commands
{
	public class Nick : Command
	{
		public Nick()
		{
		}

		public Nick(string newNick)
		{
			this.NewNick = newNick;
		}

		string _OldNick;

		public string OldNick
		{
			get { return _OldNick; }
			set { _OldNick = value; }
		}

		string _NewNick;

		public string NewNick
		{
			get { return _NewNick; }
			set { _NewNick = value; }
		}

		protected override StringBuilder GenerateParams(StringBuilder builder)
		{
			return builder.Append(this.NewNick);
		}

		public override void ParseAssign(string[] parameters)
		{
			this.NewNick = parameters[0];
			this.OldNick = this.Message.Peer.Nick;
		}

		public override string ToString()
		{
			return this.GetType().Name + "[old='" + (this.OldNick ?? "<null>") + "', new='" + (this.NewNick ?? "<null>") + "']";
		}
	}
}
