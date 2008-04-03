using System;
using System.Collections.Generic;
using System.Text;
using dbg = System.Diagnostics.Debug;

namespace zeroflag.Serialization.Descriptors
{
	public class Context
	{
		#region Descriptors
		Dictionary<Type, Type> _DescriptorTypes;

		public Dictionary<Type, Type> DescriptorTypes
		{
			get { return _DescriptorTypes ?? (_DescriptorTypes = new Dictionary<Type, Type>(_StaticDescriptorTypes)); }
		}
		static Dictionary<Type, Type> _StaticDescriptorTypes = new Dictionary<Type, Type>();

		static Context()
		{
			lock (typeof(Descriptor))
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

					_StaticDescriptorTypes.Add(valueType, descriptor);
				}
			}
		}

		public Descriptor GetDescriptor(Type value, params Type[] generics)
		{
			Descriptor desc = (Descriptor)TypeHelper.CreateInstance(GetDescriptorType(value, generics), generics);
			desc.Context = this;
			return desc;
		}
		public Type GetDescriptorType(Type value, params Type[] generics)
		{
			if (!DescriptorTypes.ContainsKey(value))
				lock (DescriptorTypes)
				{
					if (!DescriptorTypes.ContainsKey(value))
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
							{
								descriptor = GetDescriptorType(value.BaseType, generics);
							}
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
							DescriptorTypes.Add(value, descriptor);
						else
							return typeof(ObjectDescriptor);
					}
				}
			return DescriptorTypes[value];
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


		#region  Parse

		public Descriptor Parse(object instance)
		{
			if (instance == null)
				return null;
			Type type = instance.GetType();
			return this.Parse(null, type, instance, null, null);
		}

		public Descriptor Parse(Type type)
		{
			return this.Parse(null, type, null, null, null);
		}


		public Descriptor Parse(string name, Type type, Descriptor outer)
		{
			if (outer == null)
				return this.Parse(name, type, null, null, null);

			object owner = outer.Value;

			System.Reflection.PropertyInfo info = null;
			if (owner != null)
				info = owner.GetType().GetProperty(name);

			return this.Parse(info, owner);
		}

		public Descriptor Parse(System.Reflection.PropertyInfo info, object owner)
		{
			dbg.Assert(info != null, "Info is null!");

			string name = info.Name;
			Type type = info.PropertyType;
			object instance = null;
			if (owner != null && info.GetIndexParameters().Length == 0)
			{
				instance = info.GetValue(owner, null);
			}
			dbg.Assert(info.CanRead, "Property read inaccessible. type='" + type + "' instance='" + instance + "' instancetype='" + (instance ?? System.DBNull.Value).GetType() + "' " + name);

			return this.Parse(name, type, instance, owner, info);
		}

		public virtual Descriptor Parse(string name, Type type, object instance, object owner, System.Reflection.PropertyInfo info)
		{
			if (type == null && instance != null)
				type = instance.GetType();

			System.Diagnostics.Debug.Assert(type != null, "Type and instance were null! " + name);

			//if (instance == null)
			//    System.Diagnostics.Debug.Assert(instance == null || type.IsAssignableFrom(instance.GetType()), "Type and instance do not match! type='" + type + "' instance='" + instance + "' instancetype='" + (instance != null?instance.GetType():null) + "' " + name);


			Descriptor desc = null;

			if (instance != null)
			{
				// there's an instance to parse, so search for a value-descriptor...
				if (this.ParsedObjects.ContainsKey(instance))
					return this.ParsedObjects[instance];
			}
			else
			{
				// there's no instance, so search for a type-descriptor...
				if (this.ParsedTypes.ContainsKey(type))
					return this.ParsedTypes[type];
			}

			if (desc == null)
			// we don't have a descriptor yet => find one...
			{
				desc = GetDescriptor(type);

				System.Diagnostics.Debug.Assert(desc != null, "Cannot find descriptor for type='" + type + "' instance='" + instance + "' " + name);

				if (instance != null)
				{
					// there's an instance to parse, so remember it as a value-descriptor...
					this.ParsedObjects.Add(instance, desc);
				}
				else
				{
					// there's no instance, so remember it as a type-descriptor...
					this.ParsedTypes.Add(type, desc);
				}
			}

			System.Diagnostics.Debug.Assert(desc.NeedsWriteAccess || owner == null || info == null || info.CanWrite,
				"Property write inaccessible. type='" + type + "' instance='" + instance + "' " + name);

			if (owner != null && info != null && info.CanRead && (info.CanWrite || !desc.NeedsWriteAccess) && info.GetIndexParameters().Length == 0)
			{
				instance = info.GetValue(owner, null);
			}

			desc.Parse(name, type, instance);

			return desc;
		}


		string GetCleanName(string value)
		{
			return value.Replace("~", "_").Replace("`", "_");
		}

		public Type GetUsableType(Type type)
		{
			if (type != null && !type.IsVisible && type.BaseType != null)
				return GetUsableType(type.BaseType);
			else
				return type;
		}
		#endregion  Parse

		#region ParsedObjects
		private Dictionary<object, Descriptor> _ParsedObjects;

		/// <summary>
		/// All parsed objects and their descriptors...
		/// </summary>
		public Dictionary<object, Descriptor> ParsedObjects
		{
			get { return _ParsedObjects ?? (_ParsedObjects = this.ParsedObjectsCreate); }
		}

		/// <summary>
		/// Creates the default/initial value for ParsedObjects.
		/// All parsed objects and their descriptors...
		/// </summary>
		protected virtual Dictionary<object, Descriptor> ParsedObjectsCreate
		{
			get { return new Dictionary<object, Descriptor>(); }
		}

		#endregion ParsedObjects

		#region ParsedTypes
		private Dictionary<Type, Descriptor> _ParsedTypes;

		/// <summary>
		/// All parsed types(where no value was given) and their descriptors...
		/// </summary>
		public Dictionary<Type, Descriptor> ParsedTypes
		{
			get { return _ParsedTypes ?? (_ParsedTypes = this.ParsedTypesCreate); }
		}

		/// <summary>
		/// Creates the default/initial value for ParsedTypes.
		/// All parsed types(where no value was given) and their descriptors...
		/// </summary>
		protected virtual Dictionary<Type, Descriptor> ParsedTypesCreate
		{
			get { return new Dictionary<Type, Descriptor>(); }
		}

		#endregion ParsedTypes

		#region Instances
		Dictionary<int, Descriptor> _Instances;

		public Dictionary<int, Descriptor> Instances
		{
			get { return _Instances ?? (_Instances = new Dictionary<int, Descriptor>()); }
		}
		#endregion

		[System.Diagnostics.Conditional("VERBOSE")]
		static internal void CWL(object value)
		{
			Console.WriteLine(value);
		}
	}
}
