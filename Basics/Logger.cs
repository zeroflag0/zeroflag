using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

public class Logger : IDisposable
{
	public static StreamWriter Output { get; set; }
	static int _InstanceCounter;
	readonly int _InstanceId;
	public static Logger Instance { get; private set; }
	static TimeSpan TimeOffset;
	static Logger()
	{
		TimeOffset = DateTime.Now - DateTime.UtcNow;
		ConfigureConsole();
		Instance = new Logger();
		new System.Threading.Thread(new System.Threading.ThreadStart(Instance.ProcessLogging)) { Name = "logger" }.Start();
	}
#if WPF || WINDOWS || WINAPI
	[System.Runtime.InteropServices.DllImport("kernel32.dll")]
	static extern IntPtr GetConsoleWindow();
#endif

	private static bool? _IsConsoleAvailable;
	/// <summary>
	/// Problem: when running Base/Logger components without the standard windows-console attached, some parts might crash the Windows Console Host service.
	/// To avoid that it should be checked ahead of modifying the Console if it's attached.
	/// Most reliable: kernel32.dll GetConsoleWindow() -> only available when built with WPF defined.
	/// </summary>
	public static bool IsConsoleAvailable
	{
		get
		{
#if WPF
			var cw = GetConsoleWindow();
			if (cw == IntPtr.Zero)
				return false;
#endif
			if (!Environment.UserInteractive)
				return false;
			if (_IsConsoleAvailable == null)
			{
				_IsConsoleAvailable = true;
				try { int window_height = Console.CursorLeft; }
				catch { _IsConsoleAvailable = false; }
			}
			return _IsConsoleAvailable.Value;
		}
	}

	private static void ConfigureConsole()
	{
		if (!IsConsoleAvailable)
			return;
		try
		{
			Console.OpenStandardOutput();
			Console.OpenStandardError();
			Console.BufferHeight = 9999;
			Console.BufferWidth = 9999;
			Console.WindowHeight = 50;
			Console.WindowWidth = 160;
			Console.ForegroundColor = ConsoleColor.Cyan;
		}
		catch { }
	}

	public Logger()
	{
		_InstanceId = System.Threading.Interlocked.Increment(ref _InstanceCounter);
	}

	public override string ToString()
	{
		return base.ToString() + _InstanceId;
	}

	private string _LogFilePath;
	/// <summary>
	/// path to the log file
	/// </summary>
	public string LogFilePath
	{
		get { return _LogFilePath; }
		set
		{
			if (_LogFilePath != value)
			{
				if (Writer != null)
				{
					this.IgnoreError(Writer.Dispose);
					Writer = null;
				}
				if (Stream != null)
				{
					this.IgnoreError(Stream.Close);
					this.IgnoreError(Stream.Dispose);
					Stream = null;
				}
				_LogFilePath = value;
				if (value != null)
				{
					this.Stream = new FileStream(value, FileMode.Append, FileAccess.Write, FileShare.Read);
					this.Writer = new StreamWriter(this.Stream);
				}
			}
		}
	}

	FileStream Stream { get; set; }
	StreamWriter Writer { get; set; }

	public bool IsRunning = true;

	void ProcessLogging()
	{
		for (int i = 0; i < 100;)
		{
			if (_LogMessages.Count > 0)
			{
				LogMessage msg = null;
				for (int skip = 0; skip < 1000000; skip++)
				{
					lock (_LogMessages)
					{
						msg = _LogMessages.Dequeue();
						if (msg.Override && _LogMessages.Count > 0)
						{
							LogMessage next = _LogMessages.Peek();
							if (msg.Sender == next.Sender)
								continue;
						}
						break;
					}
				}
				ProcessMessage(msg);
			}
			else
			{
				if (!IsRunning)
				{
					i++;
					ProcessMessage(new LogMessage("log", "Shutting down " + i + "/100, count=" + _LogMessages.Count, true) { Timestamp = DateTime.Now });
				}
				else
				{
					i = 0;
					System.Threading.Thread.Sleep(10);
				}
			}
		}
	}
	object _LogLastSender;
	void ProcessMessage(LogMessage msg)
	{
		string txt = msg.Message;

		if (msg.Message == null)
		{
			if (msg.Override)
			{
				_LogLastSender = msg.Sender;
			}
			else
				_LogLastSender = null;
			return;
		}

		StringBuilder b = new StringBuilder("");
		if (msg.Timestamp != null)
		{
			DateTime time = msg.Timestamp.Value;
			time += TimeOffset;
			b.Append(time.ToString("HH:mm:ss.fff")).Append(": ");
		}
		if (msg.ThreadId != null)
			b.Append("[").Append(msg.ThreadId.Value.ToString("00")).Append("]");
		b.Append(' ', msg.Depth);
		if (msg.Sender != null)
			b.Append("<").Append(msg.Sender).Append("> ");

		b.Append(txt);
		string output = b.ToString();
		// do file output...
		this.IgnoreError(() =>
			{
				var writer = this.Writer;
				if (writer != null)
				{
					writer.WriteLine(output);
				}
			});
		if (msg.Message != null)
		{
			StreamWriter writer = Output;
			if (writer != null)
			{
				string line = (b.ToString()).PadRight(0);
				if (msg.Override)
				{
					line = "\r" + line;
				}
				//else
				{
					line = line + "\n";
				}
				lock (writer)
				{
					writer.Write(line);
					writer.Flush();
				}
			}
		}
		if (msg.Override)
		{
			if (_LogLastSender == msg.Sender)
			{
				try
				{
					Console.SetCursorPosition(0, Console.CursorTop - 1);
					b.Insert(0, '\r');
					int append = Console.BufferWidth - 1 - b.Length;
					if (append > 0)
						b.Append(' ', append);
					//txt = ("\r" + txt).PadRight(Console.BufferWidth - 1);
				}
				catch (IOException exc)
				{
					Console.WriteLine("Forcing console open!" + Environment.NewLine + exc);
                    //this.IgnoreError(Console.OpenStandardOutput);
                    //this.IgnoreError(Console.OpenStandardError);
                    //this.IgnoreError(Console.OpenStandardInput);
				}
				catch (Exception exc)
				{
					Console.WriteLine(exc);
				}
			}

			_LogLastSender = msg.Sender;
		}
		else
			_LogLastSender = msg.Sender;

		if (msg.Message != null)
		{
			Console.WriteLine(b.ToString());
		}
	}

	//public static void WriteLog(object message, bool overrideOld = false)
	//{
	//	WriteLog(null, message == null ? null : ("" + message), overrideOld);
	//}

	public void WriteLog(string message = null, bool overrideOld = false)
	{
		WriteLog(null, 0, message, overrideOld);
	}

	public void WriteLog(object sender, int depth = 0, string message = null, bool overrideOld = false)
	{
		lock (_LogMessages)
			_LogMessages.Enqueue(new LogMessage(sender, message, overrideOld) { Depth = depth });
	}

	Queue<LogMessage> _LogMessages = new Queue<LogMessage>();
	class LogMessage
	{
		public LogMessage(object sender, string message, bool overrideOld = false)
			: this(message, overrideOld)
		{
			this.Sender = sender;
		}
		public LogMessage(string message, bool overrideOld = false)
			: this()
		{
			this.Message = message;
			this.Override = overrideOld;
		}
		public LogMessage()
		{
			this.Timestamp = DateTime.UtcNow;
			this.ThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
		}
		public DateTime? Timestamp { get; set; }
		public string Message { get; set; }
		public object Sender { get; set; }
		public int? ThreadId { get; set; }
		public bool Override { get; set; }
		public int Depth { get; set; }
	}

	public void Dispose()
	{
		this.LogFilePath = null;
	}
}
