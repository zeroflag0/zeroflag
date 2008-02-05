using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.IRC.Messages.Commands
{
	public class Mode : Command
	{
		string _Target;

		public string Target
		{
			get { return _Target; }
			set { _Target = value; }
		}

		string _Modification;

		public string Modification
		{
			get { return _Modification; }
			set { _Modification = value; }
		}

		public override void ParseAssign(string[] parameters)
		{
			this.Target = parameters[0];
			this.Modification = parameters[1];
		}

		protected override StringBuilder GenerateParams(StringBuilder builder)
		{
			return builder.Append(this.Target).Append(" ").Append(this.Modification);
		}
	}
}
