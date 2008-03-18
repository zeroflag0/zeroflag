#region LGPL License
//********************************************************************
//	author:         Thomas "zeroflag" Kraemer
//	author email:   zeroflag@zeroflag.de
//	
//	Copyright (C) 2006-2008  Thomas "zeroflag" Kraemer
//	
//	license:	(LGPL)
//	
//		This library is free software; you can redistribute it and/or
//		modify it under the terms of the GNU Lesser General Public
//		License as published by the Free Software Foundation; either
//		version 2.1 of the License, or (at your option) any later version.
//
//		This library is distributed in the hope that it will be useful,
//		but WITHOUT ANY WARRANTY; without even the implied warranty of
//		MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//		Lesser General Public License for more details.
//
//		You should have received a copy of the GNU Lesser General Public
//		License along with this library; if not, write to the Free Software
//		Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
//
//		http://www.gnu.org/licenses/lgpl.html#TOC1
//
//*********************************************************************
#endregion LGPL License

using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Serialization.Descriptors
{
	/// <summary>
	/// Descriptor class used to describe an object or a type so it can be used for serialization.
	/// </summary>
	public abstract class Descriptor
	{
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
				CWL("GetDescriptorType(" + value + ", " + generics + ")");
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
							if (genericDescriptor.IsGenericType && !genericDescriptor.IsGenericTypeDefinition)
								genericDescriptor = genericDescriptor.GetGenericTypeDefinition();
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
						if (descriptor == null || descriptor == typeof(ObjectDescriptor))
							continue;
						else
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
			//CWL("Selected " + _Descriptors[t] + " for " + t);
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

		#region Value

		private object _Value = null;

		public object Value
		{
			get { return _Value; }
			set
			{
				if (_Value != value)
				{
					_Value = value;
					if (value != null)
						this.IsNull = false;
				}
			}
		}
		#endregion Value

		bool? _IsNull = true;

		public bool IsNull
		{
			get { return (bool)(_IsNull ?? (this.Value == null)); }
			set { _IsNull = value; }
		}

		private string _Name;

		public string Name
		{
			get { return _Name; }
			set { _Name = value; }
		}

		int? _Id = null;
		public virtual int? Id
		{
			get { return _Id; }
			set { _Id = value; }
		}

		bool _IsReferenced = false;

		public bool IsReferenced
		{
			get { return _IsReferenced; }
			set { _IsReferenced = value; }
		}

		//public abstract object GetValue();
		//public abstract void SetValue(object value);

		List<Descriptor> _Inner = new List<Descriptor>();

		public List<Descriptor> Inner
		{
			get { return _Inner; }
		}


		#region Parse
		#region static Parse
		static string GetCleanName(string value)
		{
			return value.Replace("~", "_").Replace("`", "_");
		}
		public static Descriptor DoParse(Type t)
		{
			t = GetUsableType(t);
			CWL("DoParse(" + t + ")");
			return GetDescriptor(t).Parse(GetCleanName(t.Name), t, null);
		}

		public static Descriptor DoParse(Type t, Descriptor owner)
		{
			t = GetUsableType(t);
			CWL("DoParse(" + t + ", " + owner + ")");
			return GetDescriptor(t).Parse(GetCleanName(t.Name), t, owner);
		}

		public static Descriptor DoParse(string name, object value, Descriptor owner)
		{
			CWL("DoParse(" + name + ", " + value + ", " + owner + ")");
			return GetDescriptor(GetUsableType(value.GetType())).Parse(name, GetUsableType(value.GetType()), value);
		}

		public static Descriptor DoParse(object value)
		{
			CWL("DoParse(" + value + ")");
			return GetDescriptor(GetUsableType(value.GetType())).Parse(GetCleanName(GetUsableType(value.GetType()).Name), GetUsableType(value.GetType()), value);
		}

		public static Descriptor DoParse(object value, Type type, Descriptor owner)
		{
			type = GetUsableType(type);
			CWL("DoParse(" + value + ", " + type + ", " + owner + ")");
			return GetDescriptor(type).Parse(GetCleanName(type.Name), type, owner, value);
		}


		public static Descriptor DoParse(object value, Descriptor owner)
		{
			if (value == null)
				return null;
			CWL("DoParse(" + value + ", " + owner + ")");
			return GetDescriptor(GetUsableType(value.GetType())).Parse(GetCleanName(GetUsableType(value.GetType()).Name), GetUsableType(value.GetType()), owner, value);
		}

		public static Descriptor DoParse(System.Reflection.PropertyInfo info)
		{
			CWL("DoParse(" + info + ")");
			return GetDescriptor(info.PropertyType).Parse(info);
		}

		public static Descriptor DoParse(System.Reflection.PropertyInfo info, Descriptor owner)
		{
			CWL("DoParse(" + info + ", " + owner + ")");
			return GetDescriptor(info.PropertyType).Parse(info, owner);
		}

		static Type GetUsableType(Type type)
		{
			if (!type.IsVisible && type.BaseType != null)
				return GetUsableType(type.BaseType);
			else
				return type;
		}
		#endregion static Parse

		Dictionary<object, Descriptor> _Parsed = null;

		protected Dictionary<object, Descriptor> Parsed
		{
			get { return this.Owner != null ? this.Owner.Parsed : _Parsed ?? (_Parsed = new Dictionary<object, Descriptor>()); }
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
		public virtual Descriptor Parse(string name, Type type, Descriptor owner)
		{
			this.Owner = owner;
			return this.Parse(name, type);
		}
		public Descriptor Parse(string name, Type type)
		{
			if (type == null)
				type = this.Type;
			this.Type = GetUsableType(type);

			if (name == null)
				name = type.Name;
			this.Name = name;

			return this.Parse();
		}

		protected Descriptor Parse()
		{
			if (this.Value != null)
			{
				if (this.Type.IsValueType || !this.Parsed.ContainsKey(this.Value))
				{
					if (this.Value != null && !this.Type.IsValueType)
					{
						this.Parsed.Add(this.Value, this);
					}
					this.DoParse();
				}
				else
				{
					Descriptor old = this.Parsed[this.Value];
					CWL("Skipping " + this + " for reference to " + old);
					//this.Name = old.Name;
					//this.Key = old.Key;
					this.Id = old.Id;
					old.IsReferenced = true;
					this.Inner.Clear();
					//return this.Parsed[this.Value];
				}
			}

			return this;
		}
		public abstract Descriptor Parse(string name, Type type, Descriptor owner, object value);
		public abstract Descriptor Parse(string name, Type type, object value);
		protected abstract void DoParse();
		#endregion Parse

		#region Generate
		Dictionary<int, Descriptor> _Generated = null;

		public Dictionary<int, Descriptor> Generated
		{
			get { return this.Owner != null ? this.Owner.Generated : _Generated ?? (_Generated = new Dictionary<int, Descriptor>()); }
		}

		bool _IsParsed = false;
		public void GenerateParse()
		{
			if (_IsParsed)
				return;
			_IsParsed = true;

			if (this.Id != null && this.Generated.ContainsKey(this.Id.Value))// && this != this.Generated[this.Id.Value])
			{
				if (this == this.Generated[this.Id.Value])
					CWL("Link self!");
				CWL("Link(name='" + this.Name + "', type='" + this.Type + "', isnull='" + this.IsNull + "', id='" + this.Id + "', value='" + this.Value + "', children='" + this.Inner.Count + "')");

				Descriptor other = this.Generated[this.Id.Value];
				CWL("  To(name='" + other.Name + "', type='" + other.Type + "', isnull='" + other.IsNull + "', id='" + other.Id + "', value='" + other.Value + "', children='" + other.Inner.Count + "')");
				this.Type = other.Type;
				this.Value = other.Value;
				this.IsNull = other.IsNull;
				this.Inner.AddRange(other.Inner);
				CWL("Result(name='" + this.Name + "', type='" + this.Type + "', isnull='" + this.IsNull + "', id='" + this.Id + "', value='" + this.Value + "', children='" + this.Inner.Count + "')");
			}
			else
			{
				if (this.Value == null && !this.IsNull)
				{
					this.DoCreateInstance();
				}
				if (this.Id != null)
				{
					if (!this.Generated.ContainsKey(this.Id.Value))
					{
						CWL("Reference(name='" + this.Name + "', type='" + this.Type + "', isnull='" + this.IsNull + "', id='" + this.Id + "', value='" + this.Value + "', children='" + this.Inner.Count + "')");
						this.Generated.Add(this.Id.Value, this);
					}
					else
						CWL("Reference already exists for " + this.Id);
				}
			}
			this.ApplyAttributes();
		}

		protected virtual void ApplyAttributes()
		{
			//TODO: come up with a concept for attributes... thinking > hacking.
		}

		bool _IsGenerated = false;
		public void GenerateCreate()
		{
			if (_IsGenerated)
				return;
			_IsGenerated = true;
			if (this.Value == null)
			{
				if (!this.IsNull)
					this.Value = this.DoCreateInstance();
				else
					this.Value = null;
			}
		}

		bool _IsLinked = false;
		public virtual object GenerateLink()
		{
			if (_IsLinked)
				return this.Value;
			_IsLinked = true;

			if (this.Value != null)
			{
				foreach (Descriptor sub in this.Inner)
				{
					System.Reflection.PropertyInfo prop = this.Type.GetProperty(sub.Name);
					if (prop != null)
					{
						//sub.SetValue(this.Value, prop);
						if (prop.CanWrite)
							prop.SetValue(this.Value, sub.GenerateLink(), new object[] { });
					}
				}
			}
			return this.Value;
		}

		public virtual object DoCreateInstance()
		{
			try
			{
				return this.Value = (this.IsNull ? null : (this.Value ?? (this.Value = (this.IsNull ? null : TypeHelper.CreateInstance(this.Type)))));
			}
			catch (Exception exc)
			{
				CWL(this + ".DoCreateInstance() failed:\n" + exc);
				return this.Value;
			}
		}
		#endregion Generate

		public List<System.Reflection.PropertyInfo> GetProperties(Type type)
		{
			List<System.Reflection.PropertyInfo> props = new List<System.Reflection.PropertyInfo>();
			do
			{
				props.AddRange(type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance));
			}
			while ((type = type.BaseType) != null);
			return props;
		}

		public override string ToString()
		{
			//return this.GetType().Name + "[" + this.Name + ", " + this.Type + ", " + this.Id + "] -> " + this.Owner;
			return this.ToString(new StringBuilder()).ToString();
		}

		public StringBuilder ToString(StringBuilder builder)
		{
			return builder.Append(this.GetType().Name).Append("[").Append(this.Name).Append(", ").Append(this.Type).Append(", ").Append(this.Id).Append("]");// -> { ");
		}

		[System.Diagnostics.Conditional("VERBOSE")]
		static internal void CWL(object value)
		{
			Console.WriteLine(value);
		}
	}
}
