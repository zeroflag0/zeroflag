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

//#define RAW
using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.IRC.Messages
{
	public class Peer : zeroflag.IRC.Messages.RegexBase
	{
		private string _Nick;
		private string _User;
		private string _Host;

		public string Host
		{
			get { return _Host; }
			set { _Host = value != "" ? value : null; }
		}

		public string User
		{
			get { return _User; }
			set { _User = value != "" ? value : null; }
		}

		public string Nick
		{
			get { return _Nick; }
			set { _Nick = value != "" ? value : null; }
		}

		public enum PeerType
		{
			Unknown,
			Client,
			Server,
		}

		public PeerType Type
		{
			get
			{
				if (this.Host != null)
				{
					if (this.User != null || this.Nick != null)
						return PeerType.Client;
					else
						return PeerType.Server;
				}
				else
					return PeerType.Unknown;
			}
		}

		public Peer()
		{
		}

		public Peer(string value)
			: base(value)
		{
		}

		public override StringBuilder Generate(StringBuilder builder)
		{
			if (this.Nick != null && this.Nick != "")
				builder.Append(this.Nick).Append("!");
			if (this.User != null && this.User != "")
				builder.Append(this.User).Append("@");
			if (this.Host != null && this.Host != "")
				builder.Append(this.Host);
			return builder;
		}

#if RAW
		string _raw = "";
#endif
		protected override System.Text.RegularExpressions.Regex CreateRegex
		{
			get
			{
				return new System.Text.RegularExpressions.Regex(@"\:?((?<nick>[^\s]+)!)?((?<user>[^\s]+)@)?(?<host>[^\s]+)", System.Text.RegularExpressions.RegexOptions.Compiled);
			}
		}

		public override void ParseAssign(System.Text.RegularExpressions.Match match)
		{
			this.Nick = match.Groups["nick"].Value;
			this.User = match.Groups["user"].Value;
			this.Host = match.Groups["host"].Value;

#if RAW
			_raw = "";
			for (int i = 0; i < match.Captures.Count; i++)
			{
				_raw += "Captures[" + i + "] = '" + match.Captures[i].Value + "'\n";
			}
			for (int i = 0; i < match.Groups.Count; i++)
			{
				_raw += "Groups[" + i + "] = '" + match.Groups[i].Value + "'\n";
			}
#endif
		}

		public override string ToString()
		{
			return this.GetType().Name + "[nick='" + (this.Nick ?? "<null>") + "', user='" + (this.User ?? "<null>") + "', host='" + (this.Host ?? "<null>") + "', type='" + this.Type + "']"
#if RAW
				 + " = \n" + _raw
#endif
;
		}
	}
}
