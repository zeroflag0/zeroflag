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

namespace zeroflag.IRC.Messages
{
	public class Message : RegexBase
	{
		public Message()
		{
		}

		public Message(string msg)
			: base(msg)
		{
		}

		public Message(Peer peer, Commands.Command cmd)
		{
			this.Peer = peer;
			this.Command = cmd;
		}

		Peer _Peer = new Peer();

		public Peer Peer
		{
			get { return _Peer; }
			set { _Peer = value; }
		}

		Commands.Command _Command;

		public Commands.Command Command
		{
			get { return _Command; }
			set { _Command = value; }
		}

		protected override System.Text.RegularExpressions.Regex CreateRegex
		{
			get
			{
				return new System.Text.RegularExpressions.Regex(
					@"(\:(?<prefix>[^\s]+)\s)?(?<command>[^\s]+)(\s+(?<params>[^\r\n\s]+))*?(\s+\:(?<rest>[^\r\n]+))[\r\n]?",
					System.Text.RegularExpressions.RegexOptions.Compiled | System.Text.RegularExpressions.RegexOptions.Multiline);
			}
		}

		public override void ParseAssign(System.Text.RegularExpressions.Match match)
		{
			this.Peer = new Peer(match.Groups["prefix"].Value);
			string cmd = match.Groups["command"].Value;
			this.Command = Commands.Command.Get(cmd, this);
			this.Command.Name = cmd;

			List<string> ps = new List<string>();

			foreach (System.Text.RegularExpressions.Capture g in match.Groups["params"].Captures)
			{
				ps.Add(g.Value);
			}
			ps.Add(match.Groups["rest"].Value);

			this.Command.ParseAssign(ps.ToArray());
		}

		public override StringBuilder Generate(StringBuilder builder)
		{
			this.Peer.Generate(builder).Append(" ");
			this.Command.Generate(builder);
			return builder.Append("\r\n");
		}

		public override string ToString()
		{
			return this.GetType().Name + "[peer=" + this.Peer + ", cmd=" + this.Command + "']";
		}
	}
}
