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
	public abstract class Command : Base
	{
		static Dictionary<string, Type> _CommandTypes = null;

		protected static Dictionary<string, Type> CommandTypes
		{
			get { return Command._CommandTypes ?? (Command._CommandTypes = CreateCommands()); }
		}

		private static Dictionary<string, Type> CreateCommands()
		{
			Dictionary<string, Type> commands = new Dictionary<string, Type>();

			List<Type> types = TypeHelper.GetDerived(typeof(Command));

			foreach (Type type in types)
			{
				try
				{
					if (type.IsAbstract)
						continue;

					Command cmd = TypeHelper.CreateInstance(type) as Command;
					string name = cmd.Name.ToLower();

					commands.Add(name, type);
				}
				catch
				{
				}
			}

			return commands;
		}

		public static Command Get(string cmd)
		{
			if (CommandTypes.ContainsKey(cmd.ToLower()))
				return (TypeHelper.CreateInstance(CommandTypes[cmd.ToLower()]) as Command)??new Unknown();
			return new Unknown();
		}

		public static Command Get(string cmd, Message owner)
		{
			Command value = Get(cmd);
			if (value != null)
				return value.SetMessage(owner);
			else
				return value;
		}

		Message _Message;

		public Message Message
		{
			get { return _Message; }
			set { _Message = value; }
		}

		public Command SetMessage(Message msg)
		{
			this.Message = msg;
			return this;
		}

		public virtual string Name
		{
			get { return this.GetType().Name; }
			set { }
		}

		//protected override System.Text.RegularExpressions.Regex CreateRegex
		//{
		//    get
		//    {
		//        return new System.Text.RegularExpressions.Regex(
		//            @"((?<params>[^\:\r\n\s]+)\s+)*?(\:(?<rest>[^\r\n]+))",
		//            System.Text.RegularExpressions.RegexOptions.Compiled
		//            );
		//    }
		//}

		//public override void ParseAssign(System.Text.RegularExpressions.Match match)
		//{
		//    Console.WriteLine("params: " + match.Groups["params"].Value);
		//    Console.WriteLine("rest:   " + match.Groups["rest"].Value);
		//    string[] parameters = new string[match.Captures.Count];
		//    for (int i = 0; i < match.Captures.Count; i++)
		//    {
		//        parameters[i] = match.Captures[i].Value;
		//    }

		//    this.ParseAssign(parameters);
		//}

		public override void Parse(string value)
		{
			this.ParseAssign(new string[] { value });
		}

		public abstract void ParseAssign(string[] parameters);

		public override StringBuilder Generate(StringBuilder builder)
		{
			return this.GenerateParams(builder.Append(this.Name.ToUpper()).Append(" :"));
		}

		protected abstract StringBuilder GenerateParams(StringBuilder builder);

	}
}
