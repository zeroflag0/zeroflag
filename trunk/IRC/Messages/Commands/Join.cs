using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.IRC.Messages.Commands
{
	public class Join : Command
	{
		public Join()
		{
		}

		public Join(string channel)
		{
		}

		string _Channel;

		public string Channel
		{
			get { return _Channel; }
			set { _Channel = value; }
		}

		public override void ParseAssign(string[] parameters)
		{
			this.Channel = parameters[0];
		}

		protected override StringBuilder GenerateParams(StringBuilder builder)
		{
			return builder.Append(this.Channel);
		}
	}
}
