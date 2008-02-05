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
