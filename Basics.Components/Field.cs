using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Basics.Components;

namespace Basics.Components
{
	/// <summary>
	/// type to store input and output values of 
	/// Fields must be declared class-global and persist for the entire livespan of the calculation context.
	/// </summary>
	[Serializable]
	public class Field : ValueBase<Field>, IValue
	{
		public Field(string name)
		{
			this.Name = name;
		}

		public Field()
		{
		}

		public override void Dispose()
		{
			base.Dispose();
			this.History.Clear();
		}

		decimal? _InitialValue = null;
		private void InitializeValue()
		{
			if (_InitialValue != null)
			{
				this.Value = _InitialValue.Value;
				_InitialValue = null;
			}
		}

		/// <summary>
		/// Link the variable to the context.
		/// </summary>
		public void Link()
		{
			this.InitializeValue();
		}

		//private decimal? _Value;
		///// <summary>
		///// the value.
		///// </summary>
		//public decimal? Value
		//{
		//	get { return _Value; }
		//	set
		//	{
		//		if (_Value != value)
		//		{
		//			_Value = value;
		//			this.SyncValue();
		//			this.OnPropertyChanged("Value");
		//		}
		//	}
		//}

		List<decimal?> _History = new List<decimal?>();
		[IgnoreErrors]
		public List<decimal?> History
		{
			get { return _History; }
		}
		/// <summary>
		/// retrieves the value from history
		/// </summary>
		/// <param name="t">how many iterations to go back</param>
		/// <returns></returns>
		public decimal? Hist(int t)
		{
			t = Math.Abs(t);
			int count = this.History.Count;
			while (t > count)
				t--;
			if (count == 0)
				return this.Value;
			return this.History[count - t];
		}

		/// <summary>
		/// retrieves the value from history
		/// </summary>
		/// <param name="t">how many iterations to go back</param>
		/// <returns></returns>
		public decimal? hist(int t)
		{
			return Hist(t);
		}

		public virtual void Step()
		{
			this.History.Add(this.Value);
		}

		public static implicit operator decimal?(Field value)
		{
			return value.Value;
		}

		public static implicit operator Field(decimal? value)
		{
			return new Field(null) { Value = value };
		}

		public static implicit operator decimal(Field value)
		{
			if (value.Value == null)
				throw new InvalidCastException("Field '" + value + "' has no value!");
			return value.Value.Value;
		}

		public static implicit operator Field(Basics.Components.Parameter value)
		{
			return new Field(null) { Value = value.Value };
		}

		public static implicit operator Field(decimal value)
		{
			return new Field(null) { Value = value };
		}

		public static implicit operator Field(double? value)
		{
			return new Field(null) { Value = (decimal)value };
		}

		public static implicit operator Field(double value)
		{
			return new Field(null) { Value = (decimal)value };
		}

		public static implicit operator Field(long? value)
		{
			return new Field(null) { Value = (decimal)value };
		}

		public static implicit operator Field(long value)
		{
			return new Field(null) { Value = (decimal)value };
		}

		public static implicit operator Field(string description)
		{
			return new Field(null) { Description = description };
		}


		//private string _Name;
		///// <summary>
		///// the name used to identify and store the value.
		///// </summary>
		//public string Name
		//{
		//	get { return _Name; }
		//	set
		//	{
		//		if (_Name != value)
		//		{
		//			_Name = value;
		//			this.OnPropertyChanged("Name");
		//			this.Register();
		//		}
		//	}
		//}

		//private string _Description;
		///// <summary>
		///// description for this field
		///// </summary>
		//public string Description
		//{
		//	get { return _Description; }
		//	set
		//	{
		//		if (_Description != value)
		//		{
		//			_Description = value;
		//			this.OnPropertyChanged("Description");
		//		}
		//	}
		//}

		private JobContext _Context;

		/// <summary>
		/// the calculation context this field belongs to
		/// </summary>
		[IgnoreErrors]
		public JobContext Context
		{
			get { return _Context; }
			set
			{
				if (_Context != value)
				{
					_Context = value;
					this.OnPropertyChanged("Context");
				}
			}
		}


		public override string ToString()
		{
			return "['" + this.Name + "']=" + this.Value;
		}

	}
}
