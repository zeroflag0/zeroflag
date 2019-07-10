//#define THREADED2
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Basics;

namespace TraceMerge
{
	public class Trace : Base//StreamParser
	{
		static int _InstanceCounter = 0;
		int _InstanceId;
		public Trace()
		{
			_InstanceId = System.Threading.Interlocked.Increment(ref _InstanceCounter);
		}

		public override string ToString()
		{
			return "Trace" + _InstanceId;
		}

		protected void Log(StreamReader stream, string message, bool overrideOld = false)
		{
			this.Log("[" + stream.BaseStream.Position + "] " + message, overrideOld);
		}

		protected void Log(StreamWriter stream, string message, bool overrideOld = false)
		{
			this.Log("[" + stream.BaseStream.Position + "] " + message, overrideOld);
		}

		protected void Verbose(StreamReader stream, string message, bool overrideOld = false)
		{
			this.Verbose("[" + stream.BaseStream.Position + "] " + message, overrideOld);
		}

		private bool _IsCanceled;
		/// <summary>
		/// is canceled
		/// </summary>
		public bool IsCanceled
		{
			get { return _IsCanceled; }
			set
			{
				if (_IsCanceled != value)
				{
					_IsCanceled = value;
					this.OnPropertyChanged("IsCanceled");
				}
			}
		}


		private StreamReader _Reader;
		/// <summary>
		/// reader stream
		/// </summary>
		public StreamReader Reader
		{
			get { return _Reader; }
			set
			{
				if (_Reader != value)
				{
					_Reader = value;
					this.OnPropertyChanged("Reader");
				}
			}
		}

		private string _OutputName = "merge_%.asc";
		/// <summary>
		/// output name
		/// </summary>
		public string OutputName
		{
			get { return _OutputName; }
			set
			{
				if (_OutputName != value)
				{
					_OutputName = value;
					this.OnPropertyChanged("OutputName");
				}
			}
		}

		public int OutputCounter { get; set; }

		private StreamWriter _Writer;
		/// <summary>
		/// writer stream
		/// </summary>
		public StreamWriter Writer
		{
			get
			{
				if (_Writer == null)
					lock (this)
						if (_Writer == null)
						{
							int i = this.OutputCounter++;
							string name = this.OutputName;
							if (name.Contains("%"))
							{
								name = name.Replace("%", i.ToString());
							}
							else
							{
								//string part1 = Path.GetFileNameWithoutExtension(name);
								//string ext = Path.GetExtension(name);
								//string path = Path.GetDirectoryName(name);
								//name = part1 + i + ext;
								//name = Path.Combine(path, name);
							}
							this._Writer = new StreamWriter(name, false);
						}
				return _Writer;
			}
			set
			{
				if (_Writer != value)
				{
					//if (_Writer != null)
					//{
					//	this.Log(_Writer, "Old trace... writing footer...");
					//	this.WriteFooter(_Writer);
					//	_Writer.Flush();
					//	_Writer.Dispose();
					//}
					_Writer = value;
					//if (_Writer != null)
					//{
					//	this.Log(_Writer, "New trace... writing header...");
					//	this.WriteHeader(_Writer);
					//}

					this.OnPropertyChanged("Writer");
				}
			}
		}

		private Trace _SyncTrace;
		/// <summary>
		/// other trace to sync with
		/// </summary>
		public Trace SyncTrace
		{
			get { return _SyncTrace; }
			set
			{
				if (_SyncTrace != value)
				{
					_SyncTrace = value;
					this.OnPropertyChanged("SyncTrace");
				}
			}
		}

		public bool SyncMerge(Trace other = null, Progress progress = null)
		{
			if (this.Reader == null)
			{
				this.Log("Reader is null!");
				throw new NullReferenceException("Reader is null!");
			}
			if (this.Writer == null)
			{
				this.Log("Writer is null!");
				throw new NullReferenceException("Writer is null!");
			}
			other = other ?? this.SyncTrace;
			if (other == null)
			{
				this.Log("other trace is null!");
				throw new NullReferenceException("other trace is null!");
			}
			return this.SyncMerge(this.Reader, this.Writer, other, progress);
		}

		class Reference
		{
			ulong _Value;
			public ulong Value
			{
				get
				{
					lock (this)
						return _Value;
				}
				set
				{
					lock (this)
						_Value = value;
				}
			}
			public Message Message { get; set; }
			public static implicit operator ulong(Reference reference)
			{
				return reference.Value;
			}
			public override string ToString()
			{
				return "ref " + Value.ToString("X");
			}
		}

		class Reference<T>
		{
			public T Value { get; set; }

			public static implicit operator T(Reference<T> reference)
			{
				return reference.Value;
			}
			public override string ToString()
			{
				return "ref " + Value;
			}
		}

		protected bool SyncMerge(StreamReader reader, StreamWriter writer, Trace panel, Progress progress = null)
		{
			this.IsCanceled = false;
			List<string> fabacklog = this.PrepareSync(progress);
			List<string> panelbacklog = panel.PrepareSync(null);

			this.Log(writer, "Writing trace... writing header...");
			this.WriteHeader(writer);
			this.Log(writer, "Writing trace... writing trigger block...");
			//this.WriteTriggerBlock(writer, progress);
			//Dictionary<ulong, Message> mastersyncset = new Dictionary<ulong, Message>();
			//Dictionary<ulong, Message> slavesyncset = new Dictionary<ulong, Message>();
			Reference fasyncid = new Reference { Value = 0 };
			Reference panelsyncid = new Reference { Value = 0 };
			LinkedList<Message> fabuffer = new LinkedList<Message>();
			LinkedList<Message> panelbuffer = new LinkedList<Message>();
			double offset = double.NaN;
			Reference<bool> fafinished = new Reference<bool> { Value = false };
			Reference<bool> panelfinished = new Reference<bool> { Value = false };
			uint lastmastersync = 0;
			uint lastslavesync = 0;

			Progress faprogress = new Progress();
			faprogress.Begin(reader.BaseStream.Length, "fa");
			if (progress != null)
				progress.Add(faprogress);

			Progress panelprogress = new Progress();
			panelprogress.Begin(panel.Reader.BaseStream.Length, "panel");
			if (progress != null)
				progress.Add(panelprogress);

			Progress writeprogress = new Progress();
			writeprogress.Name = "write";
			if (progress != null) progress.Add(writeprogress);

			bool foundmatch = false;

			Func<Trace, Trace, Message, bool> messagehandler = (master, trace, msg) =>
				{
					master = master ?? trace;// master will be null if self is master

					string desc;
					//Dictionary<ulong, Message> syncset;
					//Dictionary<ulong, Message> othersyncset;
					Reference othersyncid;
					Reference selfsyncid;
					LinkedList<Message> buffer;
					Reference<bool> otherfinished;
					Progress prog;

					if (master == trace)
					{
						desc = "fa can";
						//syncset = mastersyncset;
						//othersyncset = slavesyncset;
						selfsyncid = fasyncid;
						othersyncid = panelsyncid;
						buffer = fabuffer;
						otherfinished = panelfinished;
						prog = faprogress;
					}
					else
					{
						desc = "panel ";
						//syncset = slavesyncset;
						//othersyncset = mastersyncset;
						selfsyncid = panelsyncid;
						othersyncid = fasyncid;
						buffer = panelbuffer;
						otherfinished = fafinished;
						prog = panelprogress;
					}
					if (msg.Id == SyncId)
					{
						ulong syncid = trace.GetSyncPayload(msg);

						this.Verbose(desc + ": Found sync message: " + msg.Payload + " " + syncid.ToString("X").PadLeft(12, '0') + " ; other=" + othersyncid.Value.ToString("X").PadLeft(12, '0'));

						// remember that this syncid has been found...
						selfsyncid.Value = syncid;
						selfsyncid.Message = msg;
						if (!otherfinished.Value)
						{
							if (syncid >= othersyncid.Value)
							{
								//master.Log(desc + ": Waiting for other trace... " + othersyncid.Value.ToString("X"), true);
								if (trace == master)
								{
									while (!otherfinished.Value && selfsyncid.Value > othersyncid.Value)
									{
										//master.Log(desc + ": Waiting for other trace... " + selfsyncid.Value.ToString("X") + ">" + othersyncid.Value.ToString("X"), true);
										prog.WaitOutput(10);
									}
									if (!otherfinished.Value)
									{
										uint current = ((uint)((selfsyncid.Value << 16) >> 32));
										if (lastmastersync + 1 < current)
										{
											master.Log(desc + ": Missing sync messages " + lastmastersync.ToHex(" ", 4, false) + "->" + current.ToHex(" ", 4, false) + "...");
											master.Log();
										}
										lastmastersync = current;
									}
									var old = buffer.toarray();
									HashSet<Message> added = new HashSet<Message>();
									buffer.Clear();
									int synccounter = 0;
									for (int i = old.Length - 1; i >= 0; i--)
									{
										var item = old[i];
										buffer.AddFirst(item);
										added.Add(item);
										if (item.Id == SyncId)
										{
											synccounter++;
											if (synccounter > 3)
												break;
										}
									}
									foreach (var item in from o in old
														 where o.Channel != this.FilterChannel
														 && !added.Contains(o)
														 select o)
									{
										buffer.AddFirst(item);
									}
									if (buffer.Count != old.Length)
									{
										master.Log(desc + ": Skipped " + (old.Length - buffer.Count) + " messages before " + ((uint)((selfsyncid.Value << 16) >> 32)).ToHex(" ", 4, false) + "...");
										master.Log();
									}
								}
								else
								{
									//if (!otherfinished.Value && selfsyncid.Value >= othersyncid.Value)
									//	master.Log(desc + ": Waiting for other trace... " + selfsyncid.Value.ToString("X") + ">=" + othersyncid.Value.ToString("X"));
									while (!otherfinished.Value && selfsyncid.Value >= othersyncid.Value)
									{
										//master.Log(desc + ": Waiting for other trace... " + selfsyncid.Value.ToString("X") + ">=" + othersyncid.Value.ToString("X"), true);
										prog.WaitOutput(10);
									}
									if (!otherfinished.Value)
									{
										uint current = ((uint)((selfsyncid.Value << 16) >> 32));
										if (lastslavesync + 1 < current)
										{
											master.Log(desc + ": Skipped messages " + lastslavesync.ToHex(" ", 4, false) + "->" + current.ToHex(" ", 4, false) + "...");
											master.Log();
										}
										lastslavesync = current;
									}
									//master.Verbose(desc + ": Waiting for other trace... ready.");
								}

							}
						}

						//if (othersyncset.ContainsKey(syncid))
						//	lock (othersyncset)
						//		if (othersyncset.ContainsKey(syncid))
						if (trace == master && (syncid == othersyncid/* || slavefinished.Value == true*/))
						{
							//master.Log(desc + ": Writing... " + (fabuffer.Count + panelbuffer.Count) + " " + syncid.ToString("X") + " " + msg.Payload + " offset=" + offset, true);
							if (double.IsNaN(offset))
							{
								//ulong syncpayload = GetSyncPayload(msg);
								//Message othersync = slavebuffer.First(m => m.Id == SyncId && GetSyncPayload(m) == syncpayload);
								offset = GetOffset(msg, othersyncid.Message);
							}

							// if the master reached the same syncid as the slave...
							lock (fabuffer)
								lock (panelbuffer)
									// write all buffered messages with synchronized times...
									master.WriteSyncBuffer(fabuffer, panelbuffer, writeprogress, offset);
							foundmatch = true;
							selfsyncid.Value++;
						}
						if (panelfinished.Value == true)
						{
							master.Log(desc + ": Other trace has finished... " + othersyncid.Value.ToString("X"));
							foundmatch = true;
							//buffer.Clear();
							if (!double.IsNaN(offset))
								lock (fabuffer)
									lock (panelbuffer)
										// write all buffered messages with synchronized times...
										master.WriteSyncBuffer(fabuffer, panelbuffer, writeprogress, offset);
							else
							{
								master.Log(desc + ": Could not find sync point!");
							}
							return false;
						}
					}

					if (trace.IgnoreMessageIds.Contains(msg.Id))
					// if the message is ignored skip the message...
					{
						return true;
					}
					if (trace.FilterMessageIds.Count != 0 && trace.FilterChannel == msg.Channel && !trace.FilterMessageIds.Contains(msg.Id))
					// if filters are set and the message does not match the filter skip the message...
					{
						return true;
					}
					lock (buffer)
					{
						//trace1.Remove(msg => /*msg.Id == 0x328 || */msg.Id == 0x287);
						// add it to the buffer...
						buffer.AddLast(msg);
					}

					return true;
				};
			System.Threading.Thread slavethread = new System.Threading.Thread(new System.Threading.ThreadStart(() =>
				{
					panel.SyncMergeContent(progress, this, messagehandler, panelbacklog);
					panel.Log("Slave finished.");
					panelfinished.Value = true;
				})) { Name = "slave" };
			slavethread.Start();
			this.SyncMergeContent(progress, null, messagehandler, fabacklog);
			this.Log("Master finished.");
			fafinished.Value = true;

			try
			{
				slavethread.Join();
			}
			catch (Exception exc)
			{
				this.Log("Failed to join slave: " + Environment.NewLine + exc);
			}
			if (!this.IsCanceled && foundmatch)
				this.WriteSyncBuffer(fabuffer, panelbuffer, progress, offset);

			if (this.IsCanceled)
				this.Log("Writing trace... canceled!");
			else
			{
				this.Log(writer, "Writing trace... writing footer...");
				this.WriteFooter(writer);

				this.Log(writer, "Writing trace... done.");
			}
			return foundmatch;
		}
		HashSet<int> _IgnoreMessageIds = new HashSet<int>();
		public HashSet<int> IgnoreMessageIds { get { return _IgnoreMessageIds; } }
		HashSet<int> _FilterMessageIds = new HashSet<int>();
		public HashSet<int> FilterMessageIds { get { return _FilterMessageIds; } }
		private string _FilterChannel = "1";
		/// <summary>
		/// Channel the filter will be applied to.
		/// </summary>
		public string FilterChannel
		{
			get { return _FilterChannel; }
			set
			{
				if (_FilterChannel != value)
				{
					_FilterChannel = value;
					this.OnPropertyChanged("FilterChannel");
				}
			}
		}


		public static int[] DefaultSliFilter
		{
			get
			{
				return new int[]
				{
					0x34A, // NAV_GPS1
					0x34C, // NAV_GPS2
					0x34E, // NAV_SYS_INF
					0x278, // NAV_GRAPH
					0x3CC, // NAV_GRAPH_V
					0x3F7, // NAV_GRAPH_LNE
					0x42C, // NAV_GRAPH_MAPDATA
					0x43D, // NAV_CUR_SEG
					0x43C, // NAVGRPH_2_RT_OFFS
					0x24F, // NAVGRPH_2_RT_DESCR
					0x348, // NAV_GRAPH_MATCH
					0x5DD, // NAV_GRAPH_RESYNC
					0x27A, // NAV_GRAPH_SYNC
					0x287, // RCOG_TRSG
					0x28F, // RCOG_PRES_TRSG
					0x2F7, // EINHEITEN
					0x2A6, // BEDIENUNG_WISCHER
					0x252, // STAT_WISCHER
					0x226, // WISCHERGESCHWINDIGKEIT
					0x2BB, // WEGSTRECKE
					0x19F, // VYAW_VEH
					0x1A1, // V_VEH
					0x12F, // KLEMMEN
					0x328, // RELATIVZEIT
					0x2F8, // UHRZEIT_DATUM
				};
			}
		}

		public void UseDefaultSliFilter()
		{
			foreach (int msgid in DefaultSliFilter)
			{
				this.FilterMessageIds.Add(msgid);
			}
		}

		private void WriteSyncBuffer(LinkedList<Message> masterbuffer, LinkedList<Message> slavebuffer, Progress progress, double offset)
		{
			IEnumerable<Message> buffer = Merge(masterbuffer, slavebuffer, progress, offset);

			this.WriteTriggerBlock(this.Writer, buffer, progress);

			masterbuffer.Clear();
			slavebuffer.Clear();
		}

		private IEnumerable<Message> Merge(LinkedList<Message> masterbuffer, LinkedList<Message> slavebuffer, Progress progress, double initialoffset)
		{
			Func<Message, double> offset = this.GetOffset(masterbuffer, initialoffset);

			//this.Log("Merging traces...");
			int i = 0;
			double offs = 0;
			if (progress != null) progress.Begin(slavebuffer.Count(), "merge");
			foreach (Message msg in slavebuffer)
			{
				offs = offset(msg) + 0.00000001;
				if (double.IsNaN(offs))
					offs = initialoffset;
				if (double.IsNaN(offs))
					offs = 0;

				++i;
				//if ((i % 1000) == 0)
				//	this.Log("Merging traces... " + (i - 1) + "/" + messages.Count() + " offset=" + offs, true);
				if (progress != null)
					progress.Current = i;

				masterbuffer.AddLast(msg);
				msg.Trace = this;
				double newoffset = msg.TimeOffset + offs;
				if (double.IsNaN(newoffset))
				{
					this.Log("Offset is NaN! " + msg);
				}
				msg.TimeOffset = newoffset;
				msg.Refresh();
			}
			if (progress != null && !this.IsCanceled) progress.Finish();

			//this.Log("Merging traces... " + i + "/" + messages.Count() + " offset=" + offs, true);
			//this.Log("Merging traces... done.");
			return masterbuffer;
		}

		private List<string> PrepareSync(Progress progress)
		{
			List<string> triggerlines = this.ParseToTriggerBlock(this.Reader, progress);

			triggerlines = this.ParseTriggerBlockHeader(triggerlines, this.Reader, progress);
			return triggerlines;
		}

		private void SyncMergeContent(Progress progress, Trace master, Func<Trace, Trace, Message, bool> messagehandler, List<string> backlog)
		{
			ParseTriggerBlockContent(this.Reader, progress, msg =>
				{
					return messagehandler(master, this, msg);
				}, backlog);
		}

		string _Header;

		Regex contentreg = new Regex(@"\s*\d+\.\d+\s+", RegexOptions.Compiled);
		Regex contenttimereg = new Regex(@"\s*(?<time>\d+\.\d+)\s+(?:(?!info)\w)", RegexOptions.Compiled);
		public List<string> ParseToTriggerBlock(StreamReader stream, Progress progress)
		{
			//long lastLine = 0;
			string dateline = null;
			int lineno = 0;
			_Header = "";
			do
			{
				lineno++;
				//lastLine = stream.BaseStream.Position;
				string line = stream.ReadLine();
				_Header += line + Environment.NewLine;
				// ignore empty lines...
				if (line.isempty() || line.Trim().isempty())
					continue;

				if (line.ToLower().StartsWith("date"))
				{
					dateline = line;
				}
				else if (line.ToLower().StartsWith("begin triggerblock"))
				{
					this.Log("Found trigger block at line " + lineno);
					//this.Log("Found trigger block at line " + lineno + " (position " + lastLine + ")");
					//stream.BaseStream.Position = lastLine;
					//stream.BaseStream.Seek(lastLine, SeekOrigin.Begin);
					return new List<string> { line };
				}
				else if (contentreg.Match(line).Success)
				{
					// content started without triggerblock...
					return new List<string> { dateline, line };
				}
			} while (!stream.EndOfStream);

			return null;
		}

		protected List<string> ParseTriggerBlockHeader(List<string> triggerlines, StreamReader stream, Progress progress)
		{
			this.Log(stream, "Parsing triggerblock...");
			string datestring;
			DateTime date;
			int current = -1;
			foreach (string triggerline in triggerlines)
			{
				current++;

				if (triggerline.ToLower().StartsWith("begin triggerblock"))
					datestring = triggerline.Remove(0, "begin triggerblock ".Length);
				else if (triggerline.ToLower().StartsWith("date"))
					datestring = triggerline.Remove(0, "date ".Length);
				else
					continue;
				date = this.ParseDate(datestring);
				if (date == DateTime.MinValue)
					date = this.ParseDate(this.FileName);
				this.TraceDate = date;
				triggerlines.RemoveAt(current);
				this.Log(stream, "Parsing triggerblock... date=" + date);
				return triggerlines;
			}
			return triggerlines;
		}
		protected void ParseTriggerBlockContent(StreamReader stream, Progress progress, Func<Message, bool> callback = null, List<string> backlog = null)
		{
			int i = 0;
			long length = stream.BaseStream.Length;
			bool cancel = false;
			bool running = true;
#if THREADED2
			Queue<KeyValuePair<int, string>> lines = new Queue<KeyValuePair<int, string>>();
			Queue<Message> messages = new Queue<Message>();

			if (backlog != null)
				foreach (var line in backlog)
				{
					lines.Enqueue(new KeyValuePair<int, string>(i++, line));
				}
#endif
			Progress parseprogress = new Progress();
			parseprogress.Begin(length, "parsing");
			if (progress != null)
				progress.Add(parseprogress);

			Progress readprogress = new Progress();
			readprogress.Begin(length, "reading");
			if (progress != null)
				progress.Add(readprogress);


			Func<Message, bool> callbackaction = msg =>
			{
				if (msg == null)
					return false;
				if (callback != null)
				{
					if (callback(msg))
						cancel = false;
					else
					{
						cancel = true;
						return false;
					}
				}
				return true;
			};
#if THREADED2
			Func<bool> callbackloop = () =>
				{
					if (lines.Count == 0 && !running)
						return false;
					if (this.IsCanceled)
						return false;
					if (!CheckBufferInput(messages, null))
						return true;

					Message msg;
					lock (messages)
					{
						msg = messages.Dequeue();
					}

					return callbackaction(msg);
				};
			System.Threading.Thread callbackthread = new System.Threading.Thread(new System.Threading.ThreadStart(() =>
			{
				while (callbackloop()) { }
				return;
			})) { Name = "parse_callback" };
			callbackthread.Start();
#endif


			Func<string, int, bool> parseaction = (line, lineno) =>
				{
					parseprogress.Current += line.Length;

					Message msg = this.ParseMessage(line, lineno, parseprogress.Current);
					if (msg == null)
						return false;
#if THREADED2
					lock (messages)
						messages.Enqueue(msg);

					CheckBufferOutput(messages, parseprogress);
#else
					callbackaction(msg);
#endif
					return true;
				};
#if THREADED2
			Func<bool> parseloop = () =>
			{
				if (lines.Count == 0 && !running)
					return false;
				if (this.IsCanceled)
					return false;

				if (!CheckBufferInput(lines, parseprogress))
					return true;

				string line;
				int lineno;
				lock (lines)
				{
					var pair = lines.Dequeue();
					line = pair.Value;
					lineno = pair.Key;
				}
				return parseaction(line, lineno);
			};
			System.Threading.Thread parsethread = new System.Threading.Thread(new System.Threading.ThreadStart(() =>
			{
				while (parseloop()) { }
				return;
			})) { Name = "parse" };
			parsethread.Start();
#endif
			while (!stream.EndOfStream && !cancel)
			{
				if (this.IsCanceled)
					break;

				i++;
				string line = stream.ReadLine();

				//if ((i % (length / 10000)) == 0)
				readprogress.Current += line.Length;

				// ignore empty lines...
				if (line.isempty() || line.Trim().isempty())
					continue;

				Match match = __RegexLine.Match(line);
				if (match.Success)
				{
					while (!stream.EndOfStream && !cancel)
					{
						readprogress.Current += line.Length;
						if (this.IsCanceled)
							break;

						if (line.isempty() || line.Trim().isempty())
						{
							i++;
							line = stream.ReadLine();
							continue;
						}
						//if (i % 10000 == 0)
						//{
						//	this.Log(stream, "Parsing message... " + this.Messages.Count, true);
						//	if (progress != null/* && (stream.BaseStream.Position % (end / 10000)) == 0*/)
						//		progress.Current = stream.BaseStream.Position;
						//}
						//this.Log(stream, "Parsing message... done.", true);
#if THREADED2
						lock (lines)
							lines.Enqueue(new KeyValuePair<int, string>(i, line));

						CheckBufferOutput(lines, readprogress);
#else
						parseaction(line, i);
#endif
						i++;
						line = stream.ReadLine();
					}
				}
			}
			if (!this.IsCanceled) readprogress.Finish();
			running = false;

#if THREADED2
			parsethread.Join();
			callbackthread.Join();
#endif
			if (!this.IsCanceled) parseprogress.Finish();

			this.Log(stream, "Parsing triggerblock... " + i + " messages done.");
		}

		void CheckBufferOutput<T>(Queue<T> queue, Progress progress)
		{
			if (queue.Count > 20000)
			{
				while (queue.Count > 10000 && !this.IsCanceled)
					progress.WaitOutput();
			}
		}

		bool CheckBufferInput<T>(Queue<T> queue, Progress progress)
		{
			if (queue.Count == 0)
			{
				if (progress != null) progress.WaitInput();
				return false;
			}
			return true;
		}

		static Regex __RegexLine = new Regex(@"^\s*\d+\.\d+\s", RegexOptions.Compiled);
		struct DateParsingInstructions
		{
			public string Regex;
			public string Replace;
			public string Dateformat;
		}
		static DateParsingInstructions[] __RegexDateTime = new DateParsingInstructions[]
		{
			new DateParsingInstructions() { Regex = @"(?<weekday>\w{3})\s(?<month>\w{3})\s(?<day>\d{2})\s(?<time>\d+:\d{2}:\d{2})\s(?<year>\d{4})", Replace = "${month} ${day} ${year} ${time}", Dateformat = "MMM dd yyyy HH:mm:ss"},
			new DateParsingInstructions() { Regex = @"(?<weekday>\w+),\s(?<month>\w+)\s(?<day>\d+),\s(?<year>\d{4})\s(?<time>\d+:\d{2}:\d{2}\s\w+)", Replace = "${month} ${day} ${year} ${time}", Dateformat = "MMM dd yyyy h:mm:ss tt"},
			new DateParsingInstructions() { Regex = @"(?<weekday>\w+)\s+(?<month>\w+)\s+(?<day>\d)\s(?<time>\d+:\d{2}:\d{2}\s\w+)\s(?<year>\d{4})", Replace = "${month} 0${day} ${year} ${time}", Dateformat = "MMM dd yyyy h:mm:ss tt"},
			new DateParsingInstructions() { Regex = @"(?<weekday>\w+)\s+(?<month>\w+)\s+(?<day>\d{2})\s(?<time>\d+:\d{2}:\d{2}\s\w+)\s(?<year>\d{4})", Replace = "${month} ${day} ${year} ${time}", Dateformat = "MMM dd yyyy h:mm:ss tt"},
			new DateParsingInstructions() { Regex = @"(\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2})", Replace = "${1}", Dateformat = "yyyy-MM-dd HH:mm:ss"},
			new DateParsingInstructions() { Regex = @"panel_(\d{4})(\d{2})(\d{2})(\d{2})(\d{2})(\d{2})\.asc", Replace = "${1}-${2}-${3} ${4}:${5}:${6}", Dateformat = "yyyy-MM-dd HH:mm:ss"},
		};

		private DateTime ParseDate(string datestring)
		{
			var culture = System.Globalization.CultureInfo.InvariantCulture;//.GetCultureInfo("en-US");
			System.Threading.Thread.CurrentThread.CurrentCulture = culture;

			foreach (DateParsingInstructions instructions in __RegexDateTime)
			{
				Regex regex = new Regex(instructions.Regex, RegexOptions.Compiled);
				Match match = regex.Match(datestring);
				string reformat = null;
				if (match.Success)
				{
					reformat = match.Result(instructions.Replace);
					DateTime date = DateTime.ParseExact(reformat, instructions.Dateformat, culture);
					return date;
				}
			}

			return DateTime.MinValue;
		}

		private Message ParseMessage(string line, int linenumber, long position)
		{

			Message msg = new Message()
				{
					Trace = this,
					//IsVerbose = this.IsVerbose,
					Position = position,
				};
			if (!msg.ParseMessage(line, linenumber))
				return null;
			if (msg.IsParsed)
			{
				if (msg.Id == SyncId)
					this.Verbose("Found sync message: ID=" + msg.Id.ToString("X") + " -> " + msg, true);
				//this.Messages.Add(msg);
			}
			return msg;
		}

		private HashSet<string> _ChannelFilter = new HashSet<string>();
		/// <summary>
		/// filter channels
		/// </summary>
		public HashSet<string> ChannelFilter
		{
			get { return _ChannelFilter; }
		}
		//HashSet<Message> _Messages = new HashSet<Message>();
		//public HashSet<Message> Messages
		//{
		//	get { return _Messages; }
		//}

		private DateTime _TraceDate;
		/// <summary>
		/// 
		/// </summary>
		public DateTime TraceDate
		{
			get { return _TraceDate; }
			set
			{
				if (_TraceDate != value)
				{
					_TraceDate = value;
					this.OnPropertyChanged("TraceDate");
				}
			}
		}

		private bool _ChangeTimestampEndian;
		/// <summary>
		/// ChangeTimestampEndian
		/// </summary>
		public bool ChangeTimestampEndian
		{
			get { return _ChangeTimestampEndian; }
			set
			{
				if (_ChangeTimestampEndian != value)
				{
					_ChangeTimestampEndian = value;
					this.OnPropertyChanged("ChangeTimestampEndian");
				}
			}
		}


		int removecounter;
		int removedcounter;
		//public void Remove(Predicate<Message> match, Progress progress = null)
		//{
		//	this.Log("Removing messages...");
		//	Message[] messages = this.Messages.toarray();
		//	int length = messages.Length;
		//	int sectionlength = length / 4;
		//	int sectioncount = 4;
		//	List<System.Threading.Thread> threads = new List<System.Threading.Thread>();
		//	if (progress != null) progress.Begin(sectioncount, "remove", null);

		//	removecounter = 0;
		//	removedcounter = 0;
		//	int start = 0;
		//	int end = sectionlength;
		//	for (int i = 0; i < sectioncount; i++)
		//	{
		//		System.Threading.Thread thread = this.Remove(messages, start, end, match, length);
		//		threads.Add(thread);

		//		// not first...
		//		start += sectionlength;

		//		if (i == sectioncount - 2)
		//		{
		//			// last...
		//			end = length;
		//		}
		//		else
		//		{
		//			// not first, not last...
		//			end += sectionlength;
		//		}
		//	}

		//	int finished = 0;
		//	while (finished < sectioncount)
		//		foreach (var thread in threads)
		//		{
		//			if (thread.Join(100))
		//			{
		//				finished++;
		//				if (progress != null) progress.Increment();
		//			}
		//		}
		//	if (progress != null && !this.IsCanceled) progress.Finish();

		//	this.Log("Removing messages... done.");
		//}

		//System.Threading.Thread Remove(Message[] messages, int sectionstart, int sectionend, Predicate<Message> match, int length)
		//{
		//	System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(() =>
		//		{
		//			this.RemoveAsync(messages, sectionstart, sectionend, match, length);
		//		}));
		//	thread.Start();
		//	return thread;
		//}

		//void RemoveAsync(Message[] messages, int sectionstart, int sectionend, Predicate<Message> match, int length)
		//{

		//	for (int i = sectionstart; i < sectionend; i++)
		//	{
		//		Message msg = messages[i];
		//		if ((System.Threading.Interlocked.Increment(ref removecounter) % 1000) == 0)
		//			this.Log("Removing messages... " + (removedcounter + 1) + "/" + (removecounter - 1) + "/" + length, true);

		//		if (match(msg))
		//		{
		//			System.Threading.Interlocked.Increment(ref removedcounter);
		//			lock (this.Messages)
		//				this.Messages.Remove(msg);
		//		}
		//	}
		//}


		public static int SyncId = 0x328;
		public static ulong SyncDataMask = 0xffffffff0000;
		ulong GetSyncPayload(Message msg, bool? reverse = null)
		{
			if (msg.IsReversed)
				return msg.PayloadBinary;
			ulong payload = GetSyncPayload(msg.PayloadBinary, reverse);
			msg.IsReversed = true;
			msg.PayloadBinary = payload;
			return payload;
		}
		ulong GetSyncPayload(ulong payload, bool? reverse = null)
		{
			if (reverse == null) reverse = this.ChangeTimestampEndian;

			payload = payload & SyncDataMask;
			if (reverse == false)
			{
				ulong decoded = 0;
				byte[] raw = BitConverter.GetBytes(payload).Reverse().toarray();
				decoded = BitConverter.ToUInt64(raw, 0);
				//for (int i = 0; i < raw.Length; i++)
				//{
				//	decoded <<= 8;
				//	ulong temp = (ulong)raw[i];
				//	decoded += temp;
				//}
				return decoded;
			}
			else
				return payload;

		}
		Dictionary<ulong, Message> FindSyncIds(IEnumerable<Message> messages)
		{
			Dictionary<ulong, Message> syncs = new Dictionary<ulong, Message>();

			foreach (Message msg in messages)
			{
				if (msg.Id != SyncId)
					continue;

				ulong payload = msg.Trace.GetSyncPayload(msg);

				if (syncs.ContainsKey(payload))
				{
					//trace.Log("Payload was parsed twice: " + msg);
					continue;
				}
				syncs.Add(payload, msg);
			}
			return syncs;
		}


		//public void Merge(Trace other, Func<Trace, Trace, double> offsetFinder, Progress progress = null)
		//{
		//	double offset = offsetFinder(this, other);
		//	this.Merge(other, offset, progress);
		//}
		//public void Merge(Trace other, double offset, Progress progress = null)
		//{
		//	this.Merge(other, msg => offset, progress);
		//}

		public Func<Message, double> GetOffset(IEnumerable<Message> messages, double initial = 0)
		{
			//Dictionary<ulong, Message> msa = FindSyncIds(ta);
			Dictionary<ulong, Message> syncMessages = FindSyncIds(messages);

			double current = initial;

			//Dictionary<ulong, double> offsets = new Dictionary<ulong, double>();
			//foreach (var pair in (
			//	  from ap in msa
			//	  where msb.ContainsKey(ap.Key)
			//	  select new KeyValuePair<ulong, double>(ap.Key, ap.Value.TimeOffset - msb[ap.Key].TimeOffset)))
			//{
			//	if (double.IsNaN(current))
			//		current = pair.Value;
			//	offsets.Add(pair.Key, pair.Value);
			//}

			Func<Message, double> finder =
				msg =>
				{
					if (msg.Id == SyncId)
					{
						ulong payload = msg.Trace.GetSyncPayload(msg);
						if (syncMessages.ContainsKey(payload))
							//current = syncMessages[payload].TimeOffset - msg.TimeOffset;
							current = GetOffset(syncMessages[payload], msg);
					}
					return current;
				};

			return finder;
		}
		double GetOffset(Message reference, Message msg)
		{
			return reference.TimeOffset - msg.TimeOffset;
		}

		//public Trace Merge(Trace other, Progress progress)
		//{
		//	return this.Merge(other, (Func<Message, double>)null, progress);
		//}

		//public Trace Merge(Trace other, Func<Message, double> offset = null, Progress progress = null)
		//{
		//	if (this.TraceDate < other.TraceDate)
		//	{
		//		return this.Merge(this, other, offset, progress);
		//	}
		//	else
		//	{
		//		return this.Merge(other, this, offset, progress);
		//	}
		//}
		//public Trace Merge(Trace a, Trace b, Func<Message, double> offset = null, Progress progress = null)
		//{
		//	if (offset == null)
		//	{
		//		offset = a.GetOffset(a.Messages);
		//	}
		//	this.Log("Merging traces...");
		//	int i = 0;
		//	double offs = 0;
		//	IEnumerable<Message> messages = b.Messages;
		//	if (progress != null) progress.Begin(messages.Count(), "merge");
		//	foreach (Message msg in messages)
		//	{
		//		offs = offset(msg) + 0.00000001;
		//		if ((i++ % 1000) == 0)
		//			this.Log("Merging traces... " + (i - 1) + "/" + messages.Count() + " offset=" + offs, true);
		//		if (progress != null && (i % Math.Max(10, (messages.Count() / 10000))) == 0)
		//			progress.Current = i;

		//		a.Messages.Add(msg);
		//		msg.Trace = a;
		//		msg.TimeOffset += offs;
		//		msg.Refresh();
		//	}
		//	if (progress != null && !this.IsCanceled) progress.Finish();

		//	this.Log("Merging traces... " + i + "/" + messages.Count() + " offset=" + offs, true);
		//	this.Log("Merging traces... done.");
		//	return a;
		//}
		public void AppendInPlace(Trace other, Progress progress)
		{
			this.Log("Appending trace...");
			var stream = this.Writer;
			List<string> otherbacklog = other.PrepareSync(null);
			this.PrepareSync(null);

			TimeSpan difference = (other.TraceDate - this.TraceDate);
			double offset = difference.TotalSeconds;
			long i = 0;
			long length = this.Reader.BaseStream.Length + other.Reader.BaseStream.Length;

			this.Writer.BaseStream.Seek(0, SeekOrigin.End);

			Progress writeprogress = new Progress();
			writeprogress.Begin(length, "writing");
			if (progress != null)
				progress.Add(writeprogress);

			Progress processprogress = new Progress();
			processprogress.Begin(length, "processing");
			if (progress != null)
				progress.Add(processprogress);

			Action<string, Message> writeprepare = (line, msg) =>
			{
				writeprogress.Current = msg.Position;
				stream.WriteLine(msg.Write());
			};

			// append trace b with offset...
			this.AppendContent(other, progress, processprogress, stream, new Reference<int>(), otherbacklog, writeprepare, length, offset);

			stream.Flush();

			if (progress != null && !this.IsCanceled) progress.Finish();
			this.Log("Appending trace... " + i + "/" + length + " offset=" + offset, false);
			stream.Flush();
			if (!this.IsCanceled)
			{
				this.Log(stream, "Writing trace... writing footer...");
				this.WriteFooter(stream);
			}

			if (this.IsCanceled)
				this.Log("Appending trace... canceled!");
			else
				this.Log("Appending trace... done.");
		}


		private void AppendRaw(Trace a, Trace b, Progress progress, List<string> abacklog, List<string> bbacklog, bool infile)
		{
			this.Log("Appending trace...");
			var stream = this.Writer;
			if (stream != null)
				stream.Dispose();
		}


		public void AppendRaw(Trace other, Progress progress)
		{
			List<string> selfbacklog = this.PrepareSync(progress);
			List<string> otherbacklog = other.PrepareSync(null);

			this.AppendRaw(other, progress, selfbacklog, otherbacklog);
		}


		public void AppendRaw(Trace other, Progress progress, List<string> selfbacklog, List<string> otherbacklog)
		{
			if (this.TraceDate < other.TraceDate)
			{
				this.AppendRaw(this, other, progress, selfbacklog, otherbacklog);
			}
			else
			{
				other.Writer = this.Writer;
				this.AppendRaw(other, this, progress, otherbacklog, selfbacklog);
			}
		}

		private void AppendRaw(Trace a, Trace b, Progress progress, List<string> abacklog, List<string> bbacklog)
		{
			this.Log("Appending trace...");
			var stream = this.Writer;
			this.Log(stream, "Writing trace... writing header...");
			this.WriteHeader(stream);
			this.Log(stream, "Writing trace... writing trigger block...");

			TimeSpan difference = (b.TraceDate - a.TraceDate);
			double offset = difference.TotalSeconds;
			long i = 0;
			long length = a.Reader.BaseStream.Length + b.Reader.BaseStream.Length;

			Progress writeprogress1 = new Progress();
			writeprogress1.Begin(a.Reader.BaseStream.Length, "file1 raw");
			if (progress != null)
				progress.Add(writeprogress1);

			//Progress writeprogress2 = new Progress();
			//writeprogress2.Begin(b.Reader.BaseStream.Length, "file2");
			//if (progress != null)
			//	progress.Add(writeprogress2);

			AppendRaw(a, progress, writeprogress1, abacklog);
			//AppendRaw(b, writeprogress2, bbacklog);
			// append trace b with offset...
			this.AppendContent(a, b, progress, abacklog, bbacklog, false);

			stream.Flush();

			if (progress != null && !this.IsCanceled) progress.Finish();
			this.Log("Appending trace... " + i + "/" + length + " offset=" + offset, false);
			if (!this.IsCanceled)
			{
				this.Log(stream, "Writing trace... writing footer...");
				this.WriteFooter(stream);
			}
			stream.Flush();

			if (this.IsCanceled)
				this.Log("Appending trace... canceled!");
			else
				this.Log("Appending trace... done.");
		}

		void AppendRaw(Trace trace, Progress masterprogress, Progress progress, List<string> backlog)
		{
			StreamWriter writer = this.Writer;

			foreach (string line in backlog)
			{
				writer.WriteLine(line);
			}

			StreamReader reader = trace.Reader;

			while (!reader.EndOfStream)
			{
				char[] buffer = new char[1048576];
				int length = reader.ReadBlock(buffer, 0, buffer.Length);

				progress.Current += length;
				if (masterprogress != null) masterprogress.Current += length;

				if (length < buffer.Length)
				{
					string text = new string(buffer, 0, length);

					int end = text.ToLower().IndexOf("end triggerblock");
					if (end >= 0)
					{
						text = text.Substring(0, end);
						length = end;
					}
				}

				writer.Write(buffer, 0, length);

				if (!this.IsCanceled) return;
			}

			if (!this.IsCanceled) progress.Finish();
		}

		public void Trim(Progress progress = null)
		{
			var stream = this.Writer;
			this.IsCanceled = false;

			this.Log("Trimming trace...");
			List<string> selfbacklog = this.PrepareSync(progress);

			this.Log(stream, "Writing trace... writing header...");
			this.WriteHeader(stream);

			this.AppendContent(this, null, progress, selfbacklog, new List<string>(), true);

			if (!this.IsCanceled)
			{
				this.Log(stream, "Writing trace... writing footer...");
				this.WriteFooter(stream);
			}
			stream.Flush();

			if (this.IsCanceled)
				this.Log("Trimming trace... canceled!");
			else
				this.Log("Trimming trace... done.");
		}

		public void SyncAppend(Trace other, Progress progress = null)
		{
			List<string> selfbacklog = this.PrepareSync(progress);
			List<string> otherbacklog = other.PrepareSync(null);

			this.Append(other, progress, selfbacklog, otherbacklog);
		}

		public void Append(Trace other, Progress progress = null, List<string> selfbacklog = null, List<string> otherbacklog = null)
		{
			if (this.TraceDate == other.TraceDate)
			{
				if (selfbacklog == null)
					selfbacklog = new List<string>();
				if (otherbacklog == null)
					otherbacklog = new List<string>();
				while (!selfbacklog.Any(s => contenttimereg.Match(s).Success))
				// if none of the lines in the backlog are content lines, add a content line...
				{
					selfbacklog.Add(this.Reader.ReadLine());
				}
				while (!otherbacklog.Any(s => contenttimereg.Match(s).Success))
				// if none of the lines in the backlog are content lines, add a content line...
				{
					otherbacklog.Add(other.Reader.ReadLine());
				}
				string contentself = (from m in
										  (from s in selfbacklog
										   select contenttimereg.Match(s))
									  where m.Success
									  select m.Result("${time}")).First();
				string contentother = (from m in
										   (from s in otherbacklog
											select contenttimereg.Match(s))
									   where m.Success
									   select m.Result("${time}")).First();
				double timeself = double.Parse(contentself);
				double timeother = double.Parse(contentother);
				if (timeself > timeother)
					this.TraceDate = this.TraceDate.AddMilliseconds(100);
				else
					other.TraceDate = other.TraceDate.AddMilliseconds(100);
			}
			if (this.TraceDate < other.TraceDate)
			{
				this.Append(this, other, progress, selfbacklog, otherbacklog);
			}
			else
			{
				other.Writer = this.Writer;
				this.Append(other, this, progress, otherbacklog, selfbacklog);
			}
		}

		private void Append(Trace a, Trace b, Progress progress, List<string> abacklog, List<string> bbacklog)
		{
			var stream = this.Writer;
			this.IsCanceled = false;

			this.Log("Appending trace...");
			this.Log(stream, "Writing trace... writing header...");
			this.WriteHeader(stream);
			this.Log(stream, "Writing trace... writing trigger block...");

			this.AppendContent(a, b, progress, abacklog, bbacklog, true);

			stream.Flush();
			if (!this.IsCanceled)
			{
				this.Log(stream, "Writing trace... writing footer...");
				this.WriteFooter(stream);
			}

			if (this.IsCanceled)
				this.Log("Appending trace... canceled!");
			else
				this.Log("Appending trace... done.");
		}

		private void AppendContent(Trace a, Trace b, Progress progress, List<string> abacklog, List<string> bbacklog, bool parseA = true)
		{
			var stream = this.Writer;

			TimeSpan difference = TimeSpan.FromSeconds(0.0);
			if (a != null && b != null)
				difference = (b.TraceDate - a.TraceDate);

			double offset = difference.TotalSeconds;
			long i = 0;
			long length;
			if (b != null)
				length = a.Reader.BaseStream.Length + b.Reader.BaseStream.Length;
			else
				length = a.Reader.BaseStream.Length;

			long step = Math.Max(10, (length / 1000000));

			Reference<int> linesa = new Reference<int>() { Value = 0 };
			Reference<int> linesb = new Reference<int>() { Value = 0 };

#if THREADED2
			bool running = true;
			Queue<Message> lines = new Queue<Message>();
#endif

			Progress writeprogress = new Progress();
			writeprogress.Begin(length, "writing");
			if (progress != null)
				progress.Add(writeprogress);

			Progress processprogress = new Progress();
			processprogress.Begin(length, "processing");
			if (progress != null)
				progress.Add(processprogress);
#if THREADED2

			Action<string, Message> writeprepare = (line, msg) =>
				{
					lock (lines)
					{
						lines.Enqueue(msg);
					}
					CheckBufferOutput(lines, processprogress);
				};

			Func<Message, bool> writeaction = msg =>
				{
					writeprogress.Current = msg.Position;
					stream.WriteLine(msg.Write());
					return true;
				};
			System.Threading.Thread writerthread = new System.Threading.Thread(new System.Threading.ThreadStart(() =>
			{
				while (lines.Count > 0 || running)
				{
					if (this.IsCanceled)
						break;

					if (!CheckBufferInput(lines, writeprogress))
						continue;

					Message msg;
					lock (lines)
					{
						msg = lines.Dequeue();
					}

					if (!writeaction(msg))
						break;
				}
				return;
			})) { Name = "write" };
			writerthread.Start();
#else
			Func<Message, bool> writeaction = msg =>
			{
				writeprogress.Current = msg.Position;
				stream.WriteLine(msg.Write());
				return true;
			};

			Action<string, Message> writeprepare = (line, msg) =>
			{
				writeaction(msg);
			};

#endif

			this.Log("Appending traces... 0/" + length + " offset=" + offset, true);
			if (progress != null) progress.Begin(length, "append");
			if (parseA)
				AppendContent(a, progress, processprogress, stream, linesa, abacklog, writeprepare, length, 0.0);

			if (!this.IsCanceled)
				if (b != null)
					AppendContent(b, progress, processprogress, stream, linesa, bbacklog, writeprepare, length, offset);

			if (!this.IsCanceled) processprogress.Finish();

#if THREADED2
			running = false;
			writerthread.Join();
#endif
			stream.Flush();
			if (!this.IsCanceled) writeprogress.Finish();

			if (progress != null && !this.IsCanceled) progress.Finish();
			this.Log("Appending trace... " + i + "/" + length + " offset=" + offset, false);

			this.Log("Appending trace... trace a=" + linesa + " lines, b=" + linesb + " lines, total=" + (linesa + linesb) + " lines");
		}

		void AppendContent(Trace tracea, Progress masterprogress, Progress progress, StreamWriter stream, Reference<int> linecounter, List<string> backlog, Action<string, Message> write, long length, double offset)
		{
			DateTime lastchange = DateTime.UtcNow;
			double last = 0;
			long step = 10000;
			long lastcounter = 0;
			Func<Trace, Trace, Message, bool> messagehandler = (master, trace, msg) =>
			{
				if (this.IsCanceled)
					return false;

				if (trace.FilterMessageIds.Count != 0 && trace.FilterChannel == msg.Channel && !trace.FilterMessageIds.Contains(msg.Id))
				// if filters are set and the message does not match the filter skip the message...
				{
					return true;
				}

				DateTime now = DateTime.UtcNow;
				TimeSpan change = (now - lastchange);
				//if ((progress.Current % step) == 0)
				if (change.TotalSeconds > 0.5)
				{
					lastchange = now;

					double current = (double)(linecounter.Value - lastcounter) / change.TotalSeconds;
					current = (current + last) / 2.0;
					last = current;
					current /= 1000.0;

					lastcounter = linecounter.Value;

					this.Log("Appending traces... " + progress.Current + "/" + length + " offset=" + 0 + ", troughput=" + current.ToString("0.00") + " k msgs/sec", true);
					//stream.Flush();
				}

				msg.TimeOffset += offset;
				msg.Refresh();
				string line = msg.Write();

				//linecounter.Value++;
				linecounter.Value = msg.LineNumber;

				progress.Current = msg.Position;
				if (masterprogress != null) masterprogress.Current += line.Length;
				write(line, msg);

				msg.Dispose();
				return true;
			};
			messagehandler(tracea, this, new Message() { Payload = this.FileName, Channel = "info" });
			if (!this.IsCanceled)
			{
				tracea.SyncMergeContent(masterprogress, tracea, messagehandler, backlog);
				stream.Flush();
			}
		}

		private string _FileName;
		/// <summary>
		/// file name
		/// </summary>
		public string FileName
		{
			get { return _FileName; }
			set
			{
				if (_FileName != value)
				{
					_FileName = value;
					this.OnPropertyChanged("FileName");
				}
			}
		}

		//public void Write(string filename, Progress progress = null)
		//{
		//	this.FileName = filename;
		//	using (StreamWriter stream = new StreamWriter(filename))
		//	{
		//		this.Log(stream, "Writing trace '" + filename + "'...");
		//		this.Write(stream, progress);
		//		this.Log(stream, "Writing trace '" + filename + "'... done.");
		//	}
		//}

		//public void Write(StreamWriter stream, Progress progress = null)
		//{
		//	this.Log(stream, "Writing trace... writing header...");
		//	this.WriteHeader(stream);
		//	this.Log(stream, "Writing trace... writing trigger block...");
		//	this.WriteTriggerBlock(stream, progress);
		//	this.Log(stream, "Writing trace... writing footer...");
		//	this.WriteFooter(stream);
		//}
		public void WriteHeader(StreamWriter stream = null)
		{
			stream = stream ?? this.Writer;
			stream.WriteLine("date " + this.WriteDate(this.TraceDate));
			stream.WriteLine("base hex  timestamps absolute");
			stream.WriteLine("internal events logged");
			string header = "Begin Triggerblock " + this.WriteDate(this.TraceDate);
			stream.WriteLine(header);
			//stream.WriteLine(Path.GetFullPath(this.FileName));
			stream.Flush();
		}
		public void WriteFooter(StreamWriter stream = null)
		{
			stream = stream ?? this.Writer;
			//stream.WriteLine("End Triggerblock");
			stream.Flush();
		}
		//protected void WriteTriggerBlock(StreamWriter stream, Progress progress)
		//{
		//	this.Log(stream, "Writing trace... sorting messages...");
		//	IEnumerable<Message> msgs = this.Messages;
		//	this.WriteTriggerBlock(stream, msgs, progress);
		//	this.Log(stream, "Writing trace... writing trigger block... done.");
		//}

		public string Write(Message msg)
		{
			StreamWriter stream = this.Writer;
			string line = msg.Write();
			//this.Log("Write: " + line);
			stream.WriteLine(line);
			stream.Flush();
			return line;
		}

		protected void WriteTriggerBlock(StreamWriter stream, IEnumerable<Message> msgs, Progress progress)
		{
			int i = 0;

			var messages = new List<Message>(msgs);
			if (progress != null) progress.Begin(messages.Count, "write", "sorting");

			messages.Sort(new Comparison<Message>((a, b) =>
			{
				return a.TimeOffset.CompareTo(b.TimeOffset);
			}));

			//foreach (Message msg in this.Messages)
			//{
			//	if (msg.Channel.isempty())
			//	{
			//		this.Log("Writing trace... writing trigger block... " + (i - 1) + "/" + this.Messages.Count + " message is null");
			//		continue;
			//	}
			//	messages.Add(msg.TimeOffset, msg);
			//}
			i = 0;
			if (progress != null) progress.Begin(messages.Count, "write", "writing");
			//this.Log(stream, "Writing trace... writing trigger block...");
			foreach (Message msg in messages)
			{
				++i;

				//if ((i % 1000) == 0)
				//	this.Log(stream, "Writing trace... writing trigger block... " + (i - 1) + "/" + messages.Count, true);
				if (progress != null) progress.Current = i;

				this.Write(msg);
				msg.Dispose();
			}
			if (progress != null) progress.Finish();
			//this.Log(stream, "Writing trace... writing trigger block... " + i + "/" + messages.Count, true);
		}

		private string WriteDate(DateTime date)
		{
			var culture = System.Globalization.CultureInfo.InvariantCulture;//.GetCultureInfo("en-US");
			System.Threading.Thread.CurrentThread.CurrentCulture = culture;
			//Match match = Regex.Match(datestring, @"(?<weekday>\w{3})\s(?<month>\w{3})\s(?<day>\d{2})\s(?<time>\d{2}:\d{2}:\d{2})\s(?<year>\d{4})");

			//DateTime date = DateTime.WriteExact(match.Result("${month} ${day} ${year} ${time}"), "MMM dd yyyy HH:mm:ss", culture);
			//return date;
			//string datestring = date.ToString("ddd") + " " + date.ToString("MMM dd HH:mm:ss yyyy");
			string datestring = date.ToString("yyyy-MM-dd HH:mm:ss");
			return datestring;
		}

		public override void Dispose()
		{
			if (this.Reader != null)
				this.Reader.Dispose();
			if (this._Writer != null)
				this._Writer.Dispose();
			base.Dispose();
		}
	}
}
