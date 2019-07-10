using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TraceMerge
{
	public class Message : Base
	{
		public Trace Trace { get; set; }

		public bool IsParsed { get; private set; }
		public bool IsCan { get; set; }

		//private DateTime? _Time;
		///// <summary>
		///// time
		///// </summary>
		//public DateTime Time
		//{
		//	get
		//	{
		//		if (_Time == null)
		//		{
		//			DateTime time = this.Trace.TraceDate;

		//			TimeSpan delta = TimeSpan.FromSeconds(this.TimeOffset);
		//			time = time + delta;

		//			this._Time = time;
		//		}
		//		return _Time.Value;
		//	}
		//	set
		//	{
		//		if (_Time != value)
		//		{
		//			_Time = value;
		//		}
		//	}
		//}

		public double TimeOffset { get; set; }
		public string Channel { get; set; }
		public int Id { get; set; }
		public int Length { get; set; }
		public string Payload { get; set; }
		public long Position { get; set; }

		ulong? _PayloadBinary;
		public ulong PayloadBinary
		{
			get
			{
				if (_PayloadBinary != null)
					return _PayloadBinary.Value;
				if (this.Payload.isempty())
					return 0;
				string payloadstring = this.Payload;
				if (payloadstring.Length > this.Length * 3)
				{
					payloadstring = payloadstring.Substring(0, this.Length * 3 - 1);
				}
				payloadstring = payloadstring.Replace(" ", "");
				ulong payload = ulong.Parse(payloadstring, System.Globalization.NumberStyles.HexNumber);
				_PayloadBinary = payload;
				return _PayloadBinary.Value;
			}
			set
			{
				if (_PayloadBinary != value)
				{
					_PayloadBinary = value;
					this.Payload = value.ToHex(" ", this.Length);
				}
			}
		}
		public bool IsReversed { get; set; }

		public int LineNumber { get; set; }

		//static Regex __LineMatch = new Regex(@"(?<time>\d+\.\d+)\s+(?<channel>\d+)\s+(?<id>[a-fA-F0-9]+)\s+Rx\s+d\s+(?<length>\d+)\s+(?<data>([a-fA-F0-9]{2}[^\n]?)+)", RegexOptions.Compiled);
		//static Regex __LineMatchAlt = new Regex(@"(?<time>\d+\.\d+)\s+(?<channel>\w+)\s+(?<content>[^\n]+)", RegexOptions.Compiled);

		//static Regex __LineMatch = new Regex(@"(?<id>[a-fA-F0-9]+)\s+Rx\s+d\s+(?<length>\d+)\s+(?<data>([a-fA-F0-9]{2}[^\n]?)+)", RegexOptions.Compiled);
		//static Regex __LineMatchAlt = new Regex(@"(?<content>[^\n]+)", RegexOptions.Compiled);

		public bool ParseMessage(string line, int linenumber)
		{
			string fullline = line;
			try
			{
				this.IsParsed = false;
				this.LineNumber = linenumber;
				if (line.isempty() || line.Trim().isempty())
					return true;

				double? timeoffset;
				string channel;

				//line = line.TrimStart();

				timeoffset = this.ParseTime(ref line);
				if (timeoffset == null)
					return false;
				this.TimeOffset = timeoffset.Value;

				//line = line.TrimStart();

				channel = this.ParseWord(ref line);
				//line = line.TrimStart();
				this.Channel = channel;
				if (this.Trace.ChannelFilter.Count > 0 && !this.Trace.ChannelFilter.Contains(channel) && !this.Trace.ChannelFilter.Contains("*"))
					return true;
				if (channel.isempty() || channel.Trim().isempty())
					return false;

				int ichannel;

				if (int.TryParse(channel, out ichannel))
				{
					this.IsCan = true;

					string id = this.ParseWord(ref line);//match.Result("${id}");
					int iid;
					if (int.TryParse(id, System.Globalization.NumberStyles.HexNumber, System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat, out iid))
					{
						this.Id = iid;
					}
					else
					{
						// error frame?
						this.Payload = line;
						return true;
					}

					// Rx   d 
					ParseWord(ref line);
					ParseWord(ref line);
					//line = line.Remove(0, 2).TrimStart().Remove(0, 1).TrimStart();

					string length = this.ParseWord(ref line);//match.Result("${length}");
					this.Length = int.Parse(length, System.Globalization.NumberStyles.HexNumber);

					this.Payload = line;//match.Result("${data}");
					//Logger.Instance.WriteLog("Parsing message... time=" + timeoffset + ", channel=" + channel + ", id=" + id + ", length=" + length + ", data=" + this.Payload, true);

					if (this.Length <= 0)
					{
						string[] segments = this.Payload.Split(new char[]{' '}, StringSplitOptions.RemoveEmptyEntries);
						this.Length = segments.Length;
					}
				}
				else
				{
					// no can message...
					this.IsCan = false;

					this.Payload = line;
					//Logger.Instance.WriteLog("Parsing message... time=" + timeoffset + ", channel=" + channel + ", data=" + this.Payload, true);
				}

				this.IsParsed = true;
				this.Refresh();
				return true;
			}
			catch (Exception exc)
			{
				Logger.Instance.WriteLog("Failed to parse line[" + linenumber + "]: " + fullline + Environment.NewLine + "@->" + line + Environment.NewLine + exc);
				return false;
			}
		}

		private string ParseWord(ref string line)
		{
			int start = FindStart(line);
			for (int i = start; i < line.Length; i++)
				switch (line[i])
				{
				case ' ':
				case '\t':
					if (i == 0)
						return null;

					string result = line.Substring(start, i - start);
					int end = FindStart(line, i);
					line = line.Remove(0, end).TrimStart();
					return result;
				}
			return line;
		}

		int FindStart(string line, int start = 0)
		{
			int i = start;
			for (; i < line.Length; i++)
				switch (line[i])
				{
				case ' ':
				case '\t':
					break;
				default:
					return i;
				}
			return 0;
		}

		double? ParseTime(ref string line)
		{
			int start = FindStart(line);

			for (int i = start; i < line.Length; i++)
			{
				switch (line[i])
				{
				case '0':
				case '1':
				case '2':
				case '3':
				case '4':
				case '5':
				case '6':
				case '7':
				case '8':
				case '9':
				case '.':
					continue;
				default:
					break;
				}

				if (i == 0)
					return null;

				string timestring = line.Substring(start, i - start);
				line = line.Remove(0, i).TrimStart();
				return double.Parse(timestring, System.Globalization.NumberStyles.AllowDecimalPoint | System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.GetCultureInfo("en-US").NumberFormat);
			}

			return null;
		}

		public void Refresh()
		{
			//_Time = null;
			this.Output = null;
		}


		public string Write()
		{
			if (this.Output != null)
				return this.Output;
			System.Threading.Thread.CurrentThread.CurrentCulture = System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo("en-US");

			string line = this.TimeOffset.ToString("0.0000");
			line += " " + this.Channel + "  ";

			if (this.IsCan)
			{
				line += this.Id.ToString("X").PadLeft(3);
				line += " ".PadLeft(5) + "Rx   d " + this.Length.ToString() + " " + this.Payload;
			}
			else
			{
				line += this.Payload;
			}

			return this.Output = line;
		}

		private string _Output;
		/// <summary>
		/// the output from the message
		/// </summary>
		public string Output
		{
			get { return _Output; }
			private set
			{
				if (_Output != value)
				{
					_Output = value;
					//this.OnPropertyChanged("Output");
				}
			}
		}

		public override string ToString()
		{
			return this.Write();
		}

		//public override void Dispose()
		//{
		//	base.Dispose();
		//	this.Trace = null;
		//}
		public override void Dispose()
		{
			this.Trace = null;
			base.Dispose();
		}
	}
}
