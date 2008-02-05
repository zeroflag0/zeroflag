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
