using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;

namespace Basics
{
	public class Progress : Base
	{
		public Progress()
		{
			this.Parts = new ObservableCollection<Progress>();
			this.timer = new System.Timers.Timer();
			timer.Elapsed += timer_Elapsed;
			timer.Interval = 200;
			timer.Enabled = false;
			timer.AutoReset = false;
		}

		public void Begin(string name, string status = null, long? maximum = null)
		{
			this.Name = name ?? this.Name;
			this.Status = status;
			if (maximum.HasValue)
				this.Maximum = maximum.Value;
			this.Current = 0;
		}

		public void Begin(long maximum, string fallbackName, string status = null)
		{
			if (this.Name == null)
				this.Name = fallbackName;
			this.Status = status;
			this.Maximum = maximum;
			this.Current = 0;
		}

		string OldStatus;
		public void Next(string status = null)
		{
			this.IsDone = false;
			OldStatus += TrimExcess(StatusInner + "... ");
			this.Status = status;
		}

		public void Increment()
		{
			Interlocked.Increment(ref _Current);
			this.OnChange();
		}

		public void Finish()
		{
			this.Next();
			this.IsWaitingInput = false;
			this.IsWaitingOutput = false;
			this.IsDone = true;
			this.Status = null;
			this.Current = this.Maximum;
		}
		public ObservableCollection<Progress> Parts { get; private set; }

		private long _Current;
		/// <summary>
		/// 
		/// </summary>
		public long Current
		{
			get { return _Current; }
			set
			{
				if (_Current != value)
				{
					_Current = value;
					this.OnChange();
				}
			}
		}

		private long _Maximum = long.MaxValue;
		/// <summary>
		/// 
		/// </summary>
		public long Maximum
		{
			get { return _Maximum; }
			set
			{
				if (_Maximum != value)
				{
					_Maximum = value;
					this.OnChange();
				}
			}
		}

		public double Percentage
		{
			get
			{
				return (double)this.Current / (double)this.Maximum;
			}
		}

		private bool _IsWaitingOutput;
		/// <summary>
		/// is waiting
		/// </summary>
		public bool IsWaitingOutput
		{
			get { return _IsWaitingOutput; }
			set
			{
				if (_IsWaitingOutput != value)
				{
					_IsWaitingOutput = value;
					this.OnPropertyChanged("IsWaitingOutput");
				}
			}
		}

		private bool _IsWaitingInput;
		/// <summary>
		/// is waiting
		/// </summary>
		public bool IsWaitingInput
		{
			get { return _IsWaitingInput; }
			set
			{
				if (_IsWaitingInput != value)
				{
					_IsWaitingInput = value;
					this.OnPropertyChanged("IsWaitingInput");
				}
			}
		}

		System.Timers.Timer timer;

		void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			this.OnChange();
		}

		public event Action<Progress> Changed;
		DateTime _LastChange = DateTime.MinValue;
		protected void OnChange()
		{
			DateTime now = DateTime.UtcNow;
			TimeSpan change = (now - _LastChange);
			timer.Start();
			if (change.TotalSeconds < 0.1)
				return;
			_LastChange = now;
			this.OnPropertyChanged("Current");
			this.OnPropertyChanged("Maximum");
			this.OnPropertyChanged("Percentage");
			this.OnPropertyChanged("Status");
			this.OnPropertyChanged("Name");
			this.OnPropertyChanged("StatusText");
			this.OnPropertyChanged("IsDone");
			if (Changed != null)
				Changed(this);
		}

		string StatusInner { get { return TrimExcess(this.Status + "... " + (_Status == null && !IsDone ? "" : (this.Maximum != long.MaxValue ? this.Current + "/" + this.Maximum + " " : ""))); } }

		private string TrimExcess(string v)
		{
			return v.Replace("......", "...").Replace("... ... ", "... ").Replace("... ...", "...");
		}

		public string StatusText { get { return this.Name + "... " + OldStatus + (!this.IsDone ? StatusInner + this.Percentage.ToString("0.0%") : " done."); } }
		string PartsProgress
		{
			get
			{
				StringBuilder sb = new StringBuilder();
				foreach (var part in this.Parts)
				{
					sb.Append(part.StatusText).Append("... ");
				}
				return sb.ToString();
			}
		}
		public override string ToString()
		{
			return StatusText;
		}

		private bool _IsDone;
		public bool IsDone
		{
			get { return _IsDone; }
			set
			{
				if (_IsDone != value)
				{
					_IsDone = value;
					this.OnChange();
				}
			}
		}


		private string _Status;
		/// <summary>
		/// status
		/// </summary>
		public string Status
		{
			get { return _Status ?? (this.Percentage * 1.0).ToString("0.0%"); }
			set
			{
				if (_Status != value)
				{
					_Status = value;
				}
			}
		}

		private string _Name;
		/// <summary>
		/// 
		/// </summary>
		public string Name
		{
			get { return _Name; }
			set
			{
				if (_Name != value)
				{
					_Name = value;
					this.OnChange();
				}
			}
		}

		public void Add(Progress part)
		{
			this.Invoke(() => this.Parts.Add(part));
		}

		public void WaitOutput()
		{
			this.WaitOutput(200);
		}
		public void WaitOutput(int duration)
		{
			this.IsWaitingOutput = true;
			Thread.Sleep(duration);
			this.IsWaitingOutput = false;
		}

		public void WaitInput()
		{
			this.WaitInput(200);
		}
		public void WaitInput(int duration)
		{
			this.IsWaitingInput = true;
			Thread.Sleep(duration);
			this.IsWaitingInput = false;
		}

		public void Reset(long max = long.MaxValue)
		{
			this.IsDone = false;
			this.OldStatus = null;
			this.IsWaitingOutput = this.IsWaitingInput = false;
			this.Begin(max, this.Name);
		}
	}
}
