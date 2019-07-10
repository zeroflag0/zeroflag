using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

public class Base : INotifyPropertyChanged, IDisposable
{
	static Logger Logger { get; set; }
	static Base()
	{
		Logger = Logger.Instance;
	}

	protected void Log(int depth, string message = null, bool overrideOld = false, bool flush = false)
	{
		if (flush) Logger.WriteLog(this, 0, null, false);

		Logger.WriteLog(this, depth, message, overrideOld);
	}

	protected void Log(object obj, bool overrideOld = false)
	{
		this.Log(0, obj == null ? null : obj.ToString(), overrideOld);
	}

	protected void Log(Exception exc)
	{
		this.Log(exc, false);
	}
	protected void Log(Exception exc, bool overrideOld)
	{
		this.Log(0, exc == null ? null : exc.ToString(), overrideOld, true);
	}

	protected void Log(string message = null, bool overrideOld = false, bool flush = false)
	{
		this.Log(0, message, overrideOld, flush);
	}

	protected void Verbose(string message = null, bool overrideOld = false)
	{
		if (this.IsVerbose)
			Logger.WriteLog(this, 0, message, overrideOld);
	}

	protected void TimeLog(Action action, string message = null, int minimumTime = 0)
	{
		this.TimeLog(Log, action, message, minimumTime);
	}

	protected void TimeVerbose(Action action, string message = null, int minimumTime = 0)
	{
		this.TimeLog(Verbose, action, message, minimumTime);
	}

	protected void TimeLog(Action<string, bool> log, Action action, string message = null, int minimumTime = 0)
	{
		if (minimumTime == 0)
		{
			log(message + "...", false);
			action();
			log(message + "... done.", false);
		}
		else
		{
			DateTime start = DateTime.Now;
			try
			{
				action();
			}
			finally
			{
				var time = DateTime.Now - start;
				if (time.TotalMilliseconds > minimumTime)
					log(message + "... took " + ((int)time.TotalMilliseconds) + "ms.", false);
			}
		}
	}

	protected T TimeVerbose<T>(Func<T> action, string message = null, int minimumTime = 0)
	{
		return this.TimeLog(Verbose, action, message, minimumTime);
	}

	protected T TimeLog<T>(Func<T> action, string message = null, int minimumTime = 0)
	{
		return this.TimeLog(Log, action, message, minimumTime);
	}

	private T TimeLog<T>(Action<string, bool> log, Func<T> action, string message = null, int minimumTime = 0)
	{
		if (minimumTime == 0)
		{
			try
			{
				return action();
			}
			finally
			{
				log(message + "... done.", false);
			}
		}
		else
		{
			DateTime start = DateTime.Now;
			try
			{
				return action();
			}
			finally
			{
				var time = DateTime.Now - start;
				if (time.TotalMilliseconds > minimumTime)
					log(message + "... took " + ((int)time.TotalMilliseconds) + "ms.", false);
			}
		}
	}

	bool _IsVerbose;

	public bool IsVerbose { get { return _IsVerbose; } set { _IsVerbose = value; } }

	/// <summary>
	/// Initializes the logging service. only use this on the root-object of your application! this destroys all Logging functionality!
	/// Alternative for Init()-extension as C# seems to confuse Base with Application for the extension-call.
	/// </summary>
	public void InitLogging()
	{
		this.Init();
	}
	public virtual void Dispose()
	{
		this.PropertyChanged = null;
		if (Disposing != null)
			Disposing();
	}

	public event Action Disposing;

	~Base()
	{
#if DEBUG
		//Console.WriteLine(this + ".~Desturctor()");
#endif
	}

	#region INotifyPropertyChanged Members

	/// <summary>
	/// Occurs when a property value changes.
	/// </summary>
	public event PropertyChangedEventHandler PropertyChanged;

	/// <summary>
	/// Call to raise the PropertyChanged event:
	/// Occurs when a property value changes.
	/// </summary>
	protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
	{
		//this.TimeLog(() =>
		//{
		// if there are event subscribers...
		if (this.PropertyChanged != null)
			// call them...
			this.PropertyChanged(this, e);//}, "PropertyChanged(" + e.PropertyName + ")", 2);
	}

	DateTime _LastPropertyChanged = DateTime.MinValue;
#if !NETFX_CORE
	System.Timers.Timer _PropertyChangedTimer;// = new System.Threading.Timer(ProcessPropertyChanged);
#endif
	Dictionary<string, bool> _ChangedProperties = new Dictionary<string, bool>();

	/// <summary>
	/// Call to raise the PropertyChanged event:
	/// Occurs when a property value changes.
	/// </summary>
	protected virtual void OnPropertyChanged(string property)
	{
		//if (_PropertyChangedTimer == null)
		//{
		//    lock (this)
		//        if (_PropertyChangedTimer == null)
		//        {
		//            _PropertyChangedTimer = new System.Timers.Timer { AutoReset = false, Interval = 100, Enabled = false };
		//            _PropertyChangedTimer.Elapsed += _PropertyChangedTimer_Elapsed;
		//        }
		//}

		//lock (this._ChangedProperties)
		//    this._ChangedProperties[property] = true;
		//lock (_PropertyChangedTimer)
		//    if (!_PropertyChangedTimer.Enabled)
		//        _PropertyChangedTimer.Start();
		if (this.PropertyChanged != null)
			this.Invoke(() => this.OnPropertyChanged(new PropertyChangedEventArgs(property)));
	}

#if !NETFX_CORE
	void _PropertyChangedTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
	{
		string[] propnames;
		lock (_ChangedProperties)
			propnames = this._ChangedProperties.Keys.toarray();
		PropertyChangedEventArgs[] properties = new PropertyChangedEventArgs[propnames.Length];
		for (int i = 0; i < properties.Length; i++)
			properties[i] = new PropertyChangedEventArgs(propnames[i]);
		this.Invoke(() =>
		{
			foreach (PropertyChangedEventArgs prop in properties)
			{
				this.OnPropertyChanged(prop);
			}
		});
	}
	void ProcessPropertyChanged(object state)
	{
	}
#endif
	#endregion INotifyPropertyChanged Members
}
