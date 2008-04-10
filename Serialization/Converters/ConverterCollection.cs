using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Serialization.Converters
{
	public class ConverterCollection : ICollection<Converter>, ICloneable
	{

		public ConverterCollection()
		{
			_Converters = new Dictionary<Type, Dictionary<Type, Converter>>();

			List<Type> types = TypeHelper.GetDerived(typeof(Converter<,>));

			foreach (Type type in types)
			{
				try
				{
					if (type.IsAbstract || type.IsInterface)
						continue;
					Converter converter = (Converter)TypeHelper.CreateInstance(type);
					this.Add(converter);
				}
				catch { }
			}
		}

		protected ConverterCollection(ConverterCollection from)
		{
			_Converters = new Dictionary<Type, Dictionary<Type, Converter>>();

			foreach (var type in from._Converters.Values)
			{
				foreach (var conv in type.Values)
					this.Add(conv);
			}
		}

		Dictionary<Type, Dictionary<Type, Converter>> _Converters;

		public Converter GetConverter<T>(Type t2)
		{
			return this.GetConverter(typeof(T), t2);
		}

		public Converter GetConverter(Type t1, Type t2)
		{
			//if (!Converters.ContainsKey(t1))
			//{
			//    return GetConverter(t1.BaseType, t2);
			//}
			//else 
			if (t2 == null)
				return null;
			if (!_Converters[t1].ContainsKey(t2))
			{
				Converter conv = GetConverter(t1, t2.BaseType);
				if (conv != null)
				{
					_Items = null;
					_Converters[t1].Add(t2, conv);
				}
				else
					return null;
			}
			return (Converter)_Converters[t1][t2];
		}

		public bool CanConvert<T>(object value)
		{
			return value != null && CanConvert<T>(value.GetType());
		}
		public bool CanConvert<T>(Type type)
		{
			return type != null && this.GetConverter(typeof(T), type) != null;
		}

		public T Generate<T>(object value)
		{
			return value == null ? default(T) : Generate<T>(value.GetType(), value);
		}
		public T Generate<T>(Type type, object value)
		{
			if (value == null)
				return default(T);
			Converter b = GetConverter(typeof(T), value.GetType());
			return b != null ? (T)b.__Generate(type, value) : default(T);
		}

		public object Parse<T>(Type type, T source)
		{
			try
			{
				Converter b = this.GetConverter<T>(type);
				if (b != null)
				{
					return b.__Parse(type, source);
				}
				else
				{
					try
					{
						System.Reflection.MethodInfo info = type.GetMethod("Parse");
						if (info == null)
							info = type.GetMethod("parse");
						object result = info.Invoke(null, new object[] { source });
						if (!type.IsAssignableFrom(result.GetType()))
							throw new InvalidCastException();
						return result;
					}
					catch (Exception exc)
					{
						Console.WriteLine(exc);
					}
					return null;
				}
			}
			catch (Exception exc)
			{
				throw new InvalidConversionException(typeof(string), type, source, exc);
			}
		}

		#region Collection
		public virtual ConverterCollection Add(Converter converter)
		{
			_Items = null;
			Type t1, t2;
			t1 = converter.Type1;
			t2 = converter.Type2;

			if (!_Converters.ContainsKey(t1))
				_Converters.Add(t1, new Dictionary<Type, Converter>());

			_Converters[t1][t2] = converter;

			return this;
		}

		public bool Contains(Converter converter)
		{
			Type t1, t2;
			t1 = converter.Type1;
			t2 = converter.Type2;

			return _Converters.ContainsKey(t1) && _Converters[t1].ContainsKey(t2) && _Converters[t1][t2] == converter;
		}

		public virtual ConverterCollection Remove<T>(Type t2)
		{
			return this.Remove(typeof(T), t2);
		}
		public virtual ConverterCollection Remove(Type t1, Type t2)
		{
			return this.Remove(this.GetConverter(t1, t2));
		}

		public virtual ConverterCollection Remove(Converter converter)
		{
			if (converter != null)
			{
				_Items = null;
				Type t1 = converter.Type1;
				List<Type> removes = new List<Type>(this._Converters[t1].Count);
				foreach (Type t2 in this._Converters[t1].Keys)
					if (this._Converters[t1][t2] == converter)
						removes.Add(t2);
				foreach (Type t2 in removes)
					this._Converters[t1].Remove(t2);
			}
			return this;
		}

		#endregion Collection

		#region ICloneable Members

		public ConverterCollection Clone()
		{
			return new ConverterCollection(this);
		}

		object ICloneable.Clone()
		{
			return this.Clone();
		}

		#endregion

		#region ICollection<Converter> Members

		void ICollection<Converter>.Add(Converter item)
		{
			this.Add(item);
		}

		public void Clear()
		{
			_Items = null;
			this._Converters.Clear();
		}


		public void CopyTo(Converter[] array, int arrayIndex)
		{

		}

		public int Count
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsReadOnly
		{
			get { throw new NotImplementedException(); }
		}

		bool ICollection<Converter>.Remove(Converter item)
		{
			if (this.Contains(item))
			{
				while (this.Contains(item))
					this.Remove(item);
				return true;
			}
			else
				return false;
		}

		#endregion

		#region IEnumerable<Converter> Members

		public IEnumerator<Converter> GetEnumerator()
		{
			foreach (var v1 in _Converters.Values)
			{
				foreach (var v2 in v1.Values)
				{
					yield return v2;
				}
			}
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		#endregion

		#region Items
		private List<Converter> _Items;

		/// <summary>
		/// All converters in this collection, as a list.
		/// </summary>
		public List<Converter> Items
		{
			get
			{
				return _Items ?? (_Items = this.ItemsCreate);
			}
		}

		/// <summary>
		/// Creates the default/initial value for Items.
		/// All converters in this collection, as a list.
		/// </summary>
		protected virtual List<Converter> ItemsCreate
		{
			get { return new List<Converter>(this); }
		}

		#endregion Items

	}
}
