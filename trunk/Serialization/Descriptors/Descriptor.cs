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

		public static Descriptor DoParse(object value, Type type, Descriptor owner)
		{
			Console.WriteLine("DoParse(" + value + ", " + type + ", " + owner + ")");
			return GetDescriptor(type).Parse(type.Name.Replace("~", "_"), type, owner, value);
		}


		public static Descriptor DoParse(object value, Descriptor owner)
		{
			if (value == null)
				return null;
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
		static Dictionary<Type, Type> _DescriptorTypes = new Dictionary<Type, Type>();

		static Descriptor()
		{
			List<Type> types = TypeHelper.GetDerived(typeof(Descriptor<>));
			Type valueType, descriptor;
			foreach (Type descriptorType in types)
			{
				if (descriptorType.IsAbstract || descriptorType.IsInterface)
					continue;

				descriptor = descriptorType;

				Type genericDescriptor = descriptor;
				while (!genericDescriptor.IsGenericType || typeof(Descriptor<>) != genericDescriptor.GetGenericTypeDefinition())
					genericDescriptor = genericDescriptor.BaseType;

				valueType = genericDescriptor.GetGenericArguments()[0];
				if (valueType.IsGenericType)
					valueType = valueType.GetGenericTypeDefinition();

				_DescriptorTypes.Add(valueType, descriptor);
			}
		}

		public static Descriptor GetDescriptor(Type value, params Type[] generics)
		{
			return (Descriptor)TypeHelper.CreateInstance(GetDescriptorType(value, generics), generics);
		}
		public static Type GetDescriptorType(Type value, params Type[] generics)
		{
			if (!_DescriptorTypes.ContainsKey(value))
			{
				Console.WriteLine("GetDescriptorType(" + value + ", " + generics + ")");

				Type descriptor = null;
				if (value.IsGenericType)
				// value type is generic...
				{
					if (!value.IsGenericTypeDefinition)
					// if the value type is specialized (e.g. not <T> but <int> or <string>)...
					{
						if (generics.Length <= 0)
							// get the generic arguments...
							generics = value.GetGenericArguments();

						// get the fully generic type definition... (get <T> instead of <int>)
						Type genericValue = value.GetGenericTypeDefinition();

						// search for any descriptor that fits the generic definition...
						Type genericDescriptor = GetDescriptorType(genericValue, generics);

						if (genericDescriptor != null && genericValue != null)
						{
							// we got ourselves a base descriptor... now we need to specialize it for our generic type...
							descriptor = TypeHelper.SpecializeType(genericDescriptor, generics);
						}
					}
				}

				if (descriptor == null || descriptor == typeof(ObjectDescriptor))
				// no suitable descriptor yet...
				{
					// scan base types...
					if (value.BaseType != null)
						descriptor = GetDescriptorType(value.BaseType, generics);
				}

				if (descriptor == null || descriptor == typeof(ObjectDescriptor))
				// no suitable descriptor yet...
				{
					// scan interfaces...
					foreach (Type interf in value.GetInterfaces())
					{
						descriptor = GetDescriptorType(interf, generics);
						if (descriptor != null)
							break;
					}
				}

				if (descriptor != null)
					_DescriptorTypes.Add(value, descriptor);
				else
					return typeof(ObjectDescriptor);
			}
			return _DescriptorTypes[value];
		}
#if OBSOLETE
		//static Dictionary<Type, Descriptor> _Descriptors = new Dictionary<Type, Descriptor>();
		static Dictionary<Type, Type> _DescriptorTypes = new Dictionary<Type, Type>();
		static Descriptor()
		{
			List<Type> types = TypeHelper.GetDerived(typeof(Descriptor<>));

			Type valueType, descriptor;
			foreach (Type descriptorType in types)
			{
				if (descriptorType.IsAbstract || descriptorType.IsInterface)
					continue;

				descriptor = descriptorType;

				//if (!descriptor.IsGenericType)
				//{
				//    Descriptor instance = (Descriptor)TypeHelper.CreateInstance(descriptor);

				//    if (!_Descriptors.ContainsKey(instance.Type))
				//        _Descriptors.Add(instance.Type, instance);
				//}
				//else
				{
					Type genericDescriptor = descriptor;
					while (typeof(Descriptor<>) != genericDescriptor.GetGenericTypeDefinition())
						genericDescriptor = genericDescriptor.BaseType;
					//descriptor = descriptor.GetGenericTypeDefinition();
					valueType = genericDescriptor.GetGenericArguments()[0].GetGenericTypeDefinition();
					_DescriptorTypes.Add(valueType, descriptor);
				}
			}
		}

		protected static Descriptor GetDescriptor(Type type, params Type[] generics)
		{
			if (type == null)
				return null;
			if (!_DescriptorTypes.ContainsKey(type))
			{
				Descriptor inner = null;

				if (type.IsGenericType)
				{
					if (!type.IsGenericTypeDefinition)
					{
						Type generic = type.GetGenericTypeDefinition();
						if (generics.Length <= 0)
							generics = type.GetGenericArguments();
						else
						{
							List<Type> genericTypes = new List<Type>(generics);
							genericTypes.AddRange(type.GetGenericArguments());
							//generics = new Type[generics.Length + t.GetGenericArguments().Length];
						}
						if (_DescriptorTypes.ContainsKey(generic))
						{
							type = generic;
							generic = TypeHelper.SpecializeType(generic, generics);
							//inner = (Descriptor)TypeHelper.CreateInstance(_DescriptorTypes[type], generics);
							inner = GetDescriptor(_DescriptorTypes[type], generics);
						}

						if ((inner == null || inner.GetType() == typeof(Default)) && generic != type && generic != null)
						{
							inner = GetDescriptor(generic, generics);
						}
					}
				}

				if (inner == null || inner.GetType() == typeof(Default))
				{
					Type[] interfaces = type.GetInterfaces();
					foreach (Type inter in interfaces)
					{
						inner = GetDescriptor(inter, generics);
						if (inner != null && inner.GetType() != typeof(Default))
							break;
					}

					if ((inner == null || inner.GetType() == typeof(Default)) && type.BaseType != null)
						inner = GetDescriptor(type.BaseType, generics);
				}
				_DescriptorTypes.Add(type, inner.GetType());
			}
			//Console.WriteLine("Selected " + _Descriptors[t] + " for " + t);
			return (Descriptor)TypeHelper.CreateInstance(_DescriptorTypes[type], generics);
		}
#endif//OBSOLETE
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

		object _Value = null;
		public object Value
		{
			get
			{
				return _Value;
			}
			set
			{
				_Value = value;
			}
		}
		//public abstract object GetValue();
		//public abstract void SetValue(object value);

		List<Descriptor> _Inner = new List<Descriptor>();

		public List<Descriptor> Inner
		{
			get { return _Inner; }
		}


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
			//Console.WriteLine(this.GetType().Name + ".ToString(" + builder.ToString() + ")");
			builder.Append(this.GetType().Name).Append("[").Append(this.Name).Append(", ").Append(this.Type).Append(", ").Append(this.Id).Append("] -> { ");
			//builder.Append(this.GetType().Name).Append("[").Append(this.Id).Append("] -> { ");
			//foreach (Descriptor inner in this.Inner)
			//    inner.ToString(builder);
			return builder.Append(" } ");
		}
	}
}
