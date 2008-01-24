using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Serialization.Descriptors
{
	public abstract class Descriptor
	{
		#region static Parse
		public static Descriptor DoParse(Type t)
		{
			Console.WriteLine("DoParse(" + t + ")");
			return GetDescriptor(t).Parse(t.Name.Replace("~", "_"), t, null);
		}

		public static Descriptor DoParse(Type t, Descriptor owner)
		{
			Console.WriteLine("DoParse(" + t + ", " + owner + ")");
			return GetDescriptor(t).Parse(t.Name.Replace("~", "_"), t, owner);
		}

		public static Descriptor DoParse(object value)
		{
			Console.WriteLine("DoParse(" + value + ")");
			return GetDescriptor(value.GetType()).Parse(value.GetType().Name.Replace("~", "_"), value.GetType(), value);
		}

		public static Descriptor DoParse(object value, Descriptor owner)
		{
			Console.WriteLine("DoParse(" + value + ", " + owner + ")");
			return GetDescriptor(value.GetType()).Parse(value.GetType().Name.Replace("~", "_"), value.GetType(), owner, value);
		}

		public static Descriptor DoParse(System.Reflection.PropertyInfo info)
		{
			Console.WriteLine("DoParse(" + info + ")");
			return GetDescriptor(info.PropertyType).Parse(info);
		}

		public static Descriptor DoParse(System.Reflection.PropertyInfo info, Descriptor owner)
		{
			Console.WriteLine("DoParse(" + info + ", " + owner + ")");
			return GetDescriptor(info.PropertyType).Parse(info, owner);
		}
		#endregion static Parse

		#region Descriptors
		static Dictionary<Type, Descriptor> _Descriptors;

		static Descriptor()
		{
			_Descriptors = new Dictionary<Type, Descriptor>();

			List<Type> types = TypeHelper.GetDerived(typeof(Descriptor<>));

			foreach (Type type in types)
			{
				if (type.IsAbstract || type.IsInterface)
					continue;
				Descriptor descriptor = (Descriptor)TypeHelper.CreateInstance(type);

				if (!_Descriptors.ContainsKey(descriptor.Type))
					_Descriptors.Add(descriptor.Type, descriptor);
			}
		}

		protected static Descriptor GetDescriptor(Type t)
		{
			if (!_Descriptors.ContainsKey(t))
			{
				Descriptor inner = null;

				if (t.IsGenericType)
				{
					Type generic = t.GetGenericTypeDefinition();
					if (generic != t)
					{
						inner = GetDescriptor(generic);
					}
				}

				if (inner == null || inner.GetType() == typeof(Default))
				{
					Type[] interfaces = t.GetInterfaces();
					foreach (Type inter in interfaces)
					{
						inner = GetDescriptor(inter);
						if (inner != null && inner.GetType() != typeof(Default))
							break;
					}

					if (inner == null || inner.GetType() == typeof(Default))
						inner = GetDescriptor(t.BaseType);
				}
				_Descriptors.Add(t, inner);
			}
			//Console.WriteLine("Selected " + _Descriptors[t] + " for " + t);
			return _Descriptors[t];
		}
		#endregion Descriptors


		public delegate T GetHandler<T>();
		public delegate void SetHandler<T>(T value);

		private Type _Type;

		public virtual Type Type
		{
			get { return _Type; }
			set { _Type = value; }
		}


		#region Owner

		private Descriptor _Owner = default(Descriptor);

		public Descriptor Owner
		{
			get { return _Owner; }
			set
			{
				if (_Owner != value)
				{
					if (_Owner != null)
						_Owner.Inner.Remove(this);
					_Owner = value;
					if (value != null)
						value.Inner.Add(this);
				}
			}
		}
		#endregion Owner

		List<Descriptor> _Inner = new List<Descriptor>();

		public List<Descriptor> Inner
		{
			get { return _Inner; }
		}

		public abstract object GetValue();
		public abstract void SetValue(object value);

		public Descriptor Parse(System.Reflection.PropertyInfo info, Descriptor owner)
		{
			this.Owner = owner;
			return this.Parse(info);
		}

		public virtual Descriptor Parse(System.Reflection.PropertyInfo info)
		{
			return this.Parse(info.Name, info.PropertyType);
		}

		public abstract Descriptor Parse(string name, Type type, Descriptor owner, object value);
		public abstract Descriptor Parse(string name, Type type, object value);

		public virtual Descriptor Parse(string name, Type type, Descriptor owner)
		{
			this.Owner = owner;
			return this.Parse(name, type);
		}
		public Descriptor Parse(string name, Type type)
		{
			if (type == null)
				type = this.Type;
			this.Type = type;

			if (name == null)
				name = type.Name;
			this.Name = name;

			return this.Parse();
		}

		public abstract Descriptor Parse();

		private string _Name;

		public string Name
		{
			get { return _Name; }
			set { _Name = value; }
		}

		public virtual int Id
		{
			get { return -1; }
		}

		public override string ToString()
		{
			//return this.GetType().Name + "[" + this.Name + ", " + this.Type + ", " + this.Id + "] -> " + this.Owner;
			return this.ToString(new StringBuilder()).ToString();
		}

		public StringBuilder ToString(StringBuilder builder)
		{
			builder.Append(this.GetType().Name).Append("[").Append(this.Name).Append(", ").Append(this.Type).Append(", ").Append(this.Id).Append("] -> { ");
			foreach (Descriptor inner in this.Inner)
				inner.ToString(builder);
			return builder.Append(" } ");
		}
	}
}
