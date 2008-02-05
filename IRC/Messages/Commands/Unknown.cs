using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.IRC.Messages.Commands
{
	class Unknown : Command
	{
		string[] _Parameters;

		public string[] Parameters
		{
			get { return _Parameters; }
			set { _Parameters = value; }
		}

		string _Name;
		public override string Name
		{
			get
			{
				return _Name ?? base.Name;
			}
			set
			{
				_Name = value;
			}
		}

		public override void ParseAssign(string[] parameters)
		{
			this.Parameters = parameters;
		}

		protected override StringBuilder GenerateParams(StringBuilder builder)
		{
			foreach (string param in Parameters)
			{
				builder.Append(param).Append(" ");
			}
			return builder;
		}

		public override string ToString()
		{
			string value = this.GetType().Name + "[name='" + (this.Name ?? "<null>") + "', ";
			if (this.Parameters != null)
				for (int i = 0; i < this.Parameters.Length; i++)
				{
					value += i + "='" + this.Parameters[i] + "', ";
				}
			else
				value += "<null>";
			value = value.TrimEnd(',', ' ') + "]";

			return value;
		}
	}
}
