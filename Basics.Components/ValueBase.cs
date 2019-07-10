using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Basics.Components
{

	public abstract class ValueBase<TSelf> : Content<TSelf>, IValue
		where TSelf : ValueBase<TSelf>, new()
	{
		public ValueBase()
		{
		}


		private ContentView _Owner;
		/// <summary>
		/// the content this parameter belongs to...
		/// </summary>
		[IgnoreErrors]
		public ContentView Owner
		{
			get { return _Owner; }
			set
			{
				if (_Owner != value)
				{
					_Owner = value;
					this.OnPropertyChanged("Owner");
				}
			}
		}


		private ContentView _Creator;
		/// <summary>
		/// the content that created this parameter...
		/// </summary>
		[IgnoreErrors]
		public ContentView Creator
		{
			get { return _Creator; }
			set
			{
				if (_Creator != value)
				{
					_Creator = value;
					this.OnPropertyChanged("Creator");
				}
			}
		}

		private decimal? _Value;
		/// <summary>
		/// the value
		/// </summary>
		public virtual decimal? Value
		{
			get { return _Value; }
			set
			{
				if (_Value != value)
				{
					this.OnValueChanged(_Value, _Value = value);
					this.OnPropertyChanged("Value");
				}
			}
		}


		private string _Format;

		/// <summary>
		/// format string
		/// </summary>
		public string Format
		{
			get { return _Format; }
			set
			{
				if (_Format != value)
				{
					_Format = value;
					this.OnPropertyChanged("Format");
				}
			}
		}

		protected override void SavableFields(SavableFields fields)
		{
			base.SavableFields(fields);
			fields.Add("Format", () => this.Format, v => this.Format = v);
		}


		private bool _NoEvents;
		/// <summary>
		/// if this instance should suppress all events
		/// </summary>
		public bool NoEvents
		{
			get { return _NoEvents; }
			set
			{
				if (_NoEvents != value)
				{
					_NoEvents = value;
					this.OnPropertyChanged("NoEvents");
				}
			}
		}

		public delegate void ValueChangedHandler(TSelf param, decimal? oldvalue, decimal? newvalue);
		public event ValueChangedHandler ValueChanged;
		protected virtual void OnValueChanged(decimal? oldvalue, decimal? newvalue)
		{
			if (NoEvents)
				return;

			if (ValueChanged != null)
				ValueChanged((TSelf)this, oldvalue, newvalue);

			this.Refresh();
		}

		public virtual void Refresh()
		{
		}

		public override void Dispose()
		{
			base.Dispose();

			this.ValueChanged = null;
		}
	}
}
