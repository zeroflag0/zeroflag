using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag
{
#if TODO
	public class Bindable<T>
	{
		#region Value

		public Bindable(T value)
		{
			this.Value = value;
		}

		private T _Value;

		/// <summary>
		/// The bound value.
		/// </summary>
		public T Value
		{
			get { return _Value; }
			set
			{
				if (_Value != value)
				{
					this.OnValueChanged(_Value, _Value = value);
				}
			}
		}

		#region ValueChanged event
		public delegate void ValueChangedHandler(object sender, T oldvalue, T newvalue);

		private event ValueChangedHandler _ValueChanged;
		/// <summary>
		/// Occurs when Value changes.
		/// </summary>
		public event ValueChangedHandler ValueChanged
		{
			add { this._ValueChanged += value; }
			remove { this._ValueChanged -= value; }
		}

		/// <summary>
		/// Raises the ValueChanged event.
		/// </summary>
		protected virtual void OnValueChanged(T oldvalue, T newvalue)
		{
			// if there are event subscribers...
			if (this._ValueChanged != null)
			{
				// call them...
				this._ValueChanged(this, oldvalue, newvalue);
			}
		}
		#endregion ValueChanged event
		#endregion Value

		public static implicit operator T(Bindable<T> bind)
		{
			return bind != null ? bind.Value : default(T);
		}

		public static implicit operator Bindable<T>(T value)
		{
			return new Bindable<T>(value);
		}
	}
#endif//TODO
}
