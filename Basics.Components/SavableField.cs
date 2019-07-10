using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Basics.Components
{
	public abstract class SavableField : Base
	{
		private string _Name;
		/// <summary>
		/// the field's name
		/// </summary>
		public string Name
		{
			get { return _Name; }
			set
			{
				if (_Name != value)
				{
					_Name = value;
					this.OnPropertyChanged("Name");
				}
			}
		}

		public abstract object Save(DataSet data);
		public abstract void Load(DataSet data, object value);
		public abstract Type Type { get; }
	}

	public class NullableBox<T>
		where T : struct
	{
		public NullableBox(T? value)
		{
			if (value == null)
				this.HasValue = false;
			else
				this.Value = value.Value;
		}

		public NullableBox(T value)
		{
			this.Value = value;
		}

		T _Value;
		public T Value
		{
			get
			{
				if (!HasValue)
					throw new InvalidOperationException("Nullable has no value!");
				return _Value;
			}
			set
			{
				_Value = value;
				this.HasValue = true;
			}
		}

		public bool HasValue { get; set; }

		public const string NullString = "null";

		public static NullableBox<T> Parse(string text, System.Globalization.CultureInfo culture = null)
		{
			culture = culture ?? System.Globalization.CultureInfo.InvariantCulture;

			if (text == null || text.Length == 0 || text.ToLower() == NullString)
				return new NullableBox<T>(null);

			MethodInfo method = typeof(T).GetMethod("Parse", new System.Type[] { typeof(string), typeof(System.Globalization.CultureInfo) });
			if (method != null)
			{
				T value = (T)method.Invoke(null, new object[] { text, culture });
				return new NullableBox<T>(value);
			}
			else
			{
				method = typeof(T).GetMethod("Parse", new System.Type[] { typeof(string) });
				T value = (T)method.Invoke(null, new object[] { text });
				return new NullableBox<T>(value);
			}
		}

		public override string ToString()
		{
			if (!this.HasValue)
				return NullString;
			MethodInfo method = typeof(T).GetMethod("ToString", new System.Type[] { typeof(System.Globalization.CultureInfo) });
			if (method != null)
			{
				string value = (string)method.Invoke(this.Value, new object[] { System.Globalization.CultureInfo.InvariantCulture });
				return value;
			}
			else
			{
				return this.Value.ToString();
			}
		}

		public static explicit operator T?(NullableBox<T> value)
		{
			return value.HasValue ? (T?)value.Value : (T?)null;
		}
		public static explicit operator T(NullableBox<T> value)
		{
			return value.Value;
		}
		public static implicit operator NullableBox<T>(T? value)
		{
			return new NullableBox<T>(value);
		}
		public static implicit operator NullableBox<T>(T value)
		{
			return new NullableBox<T>(value);
		}
	}

	public class SavableField<T>
		: SavableField
	{
		public SavableField(string name, Func<DataSet, T> save, Action<DataSet, T> load, Func<string, T> parse = null)
		{
			if (name == null) throw new ArgumentNullException("name", "name cannot be null!");
			if (save == null) throw new ArgumentNullException("save", "save cannot be null!");
			if (load == null) throw new ArgumentNullException("load", "load cannot be null!");

			this.Name = name;
			this.SaveAction = save;
			this.LoadAction = load;
			this.ParseFunction = parse;
		}

		private SavableField()
		{
		}

		public Func<DataSet, T> SaveAction;
		public Action<DataSet, T> LoadAction;

		public override object Save(DataSet data)
		{
			if (IsNullable)
			{
				object value = SaveAction(data);
				string result = null;

				if (value == null)
					result = NullableBox<int>.NullString;
				else if (typeof(T) == typeof(decimal?))
					result = new NullableBox<decimal>((decimal?)value).ToString();
				else if (typeof(T) == typeof(int?))
					result = new NullableBox<int>((int?)value).ToString();
				else if (typeof(T) == typeof(long?))
					result = new NullableBox<long>((long?)value).ToString();
				else if (typeof(T) == typeof(float?))
					result = new NullableBox<float>((float?)value).ToString();
				else if (typeof(T) == typeof(double?))
					result = new NullableBox<double>((double?)value).ToString();
				else if (typeof(T) == typeof(byte?))
					result = new NullableBox<byte>((byte?)value).ToString();
				else if (typeof(T) == typeof(uint?))
					result = new NullableBox<uint>((uint?)value).ToString();
				else if (typeof(T) == typeof(ulong?))
					result = new NullableBox<ulong>((ulong?)value).ToString();
				else
					throw new ArgumentException("Unknown nullable type!");

				return result;
			}
			return SaveAction(data);
		}

		public override void Load(DataSet data, object raw)
		{
			string text = raw == DBNull.Value ? (string)null : ("" + raw);
			T value;
			Func<string, T> parse = this.ParseFunction	// try to use the local parse function...
				?? (ParseFunctions.ContainsKey(typeof(T)) ? this.ParseFunction = ParseFunctions[typeof(T)] : null) // ...if none is set... try to find the parse function in the cached parse functions...
				?? (this.ParseFunction = FindParseFunction()); // .. if none is found... try to find a "Parse()" function on the type... if none is found, fail.

			value = parse(text);
			LoadAction(data, (T)value);
		}

		Func<string, T> FindParseFunction()
		{
			Type type = typeof(T);
			Func<string, T> parse;
			if (IsNullable)
			{
				if (typeof(T) == typeof(decimal?))
					parse = text => (T)(object)(decimal?)NullableBox<decimal>.Parse(text);
				else if (typeof(T) == typeof(int?))
					parse = text => (T)(object)(int?)NullableBox<int>.Parse(text);
				else if (typeof(T) == typeof(long?))
					parse = text => (T)(object)(long?)NullableBox<long>.Parse(text);
				else if (typeof(T) == typeof(float?))
					parse = text => (T)(object)(float?)NullableBox<float>.Parse(text);
				else if (typeof(T) == typeof(double?))
					parse = text => (T)(object)(double?)NullableBox<double>.Parse(text);
				else if (typeof(T) == typeof(byte?))
					parse = text => (T)(object)(byte?)NullableBox<byte>.Parse(text);
				else if (typeof(T) == typeof(uint?))
					parse = text => (T)(object)(uint?)NullableBox<uint>.Parse(text);
				else if (typeof(T) == typeof(ulong?))
					parse = text => (T)(object)(ulong?)NullableBox<ulong>.Parse(text);
				else if (typeof(T) == typeof(bool?))
					parse = text => (T)(object)(bool?)NullableBox<bool>.Parse(text);
				else
					throw new ArgumentException("Unknown nullable type!");
			}
			else if (type == typeof(string))
			{
				parse = text =>
				{
					return (T)(object)text;
				};

			}
			else if (type.IsEnum)
			{
				parse = text =>
				{
					return (T)Enum.Parse(type, text);
				};

			}
			else if (type == typeof(Type))
			{
				parse = name => (T)(object)Type.GetType(name, true, false);
			}
			else
			{
				MethodInfo method = type.GetMethod("Parse", new System.Type[] { typeof(string) });
				parse = text =>
				{
					if (text == null)
						return default(T);
					try
					{
						return (T)method.Invoke(null, new object[] { text });
					}
					catch (Exception exc)
					{
						this.Log("Failed to parse parameter '" + Name + "': '" + text + "' to " + typeof(T) + "" + Environment.NewLine + exc);
						return default(T);
					}
				};
			}
			ParseFunctions[type] = parse;
			return parse;
		}

		Func<string, T> ParseFunction { get; set; }

		public override Type Type
		{
			get
			{
				if (IsNullable)
					return typeof(string);
				return typeof(T);
			}
		}

		static bool IsNullable
		{
			get
			{
				Type type = typeof(T);
				Type[] generics = type.GetGenericArguments();
				if (!type.IsClass && generics != null && generics.Length == 1 && type.Name.StartsWith("Nullable`1"))
					return true;
				return false;
			}
		}

		public static T Parse(object raw)
		{
			if (raw is T)
				return (T)raw;
			Func<string, T> parse =
				(ParseFunctions.ContainsKey(typeof(T)) ? ParseFunctions[typeof(T)] : null) // ...if none is set... try to find the parse function in the cached parse functions...
				?? (new SavableField<T>().FindParseFunction()); // .. if none is found... try to find a "Parse()" function on the type... if none is found, fail.
			return parse((string)raw);
		}

		static Dictionary<Type, Func<string, T>> _ParseFunctions = new Dictionary<Type, Func<string, T>>();
		static Dictionary<Type, Func<string, T>> ParseFunctions { get { return _ParseFunctions; } }
	}
}
