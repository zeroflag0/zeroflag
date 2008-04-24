using System;
using System.Collections.Generic;
using System.Text;
using dbg = System.Diagnostics.Debug;

namespace zeroflag.Serialization.Descriptors
{
	public class Context
	{
		#region Descriptors
		#region DescriptorTypes
		private Dictionary<Type, Type> _DescriptorTypes;

		/// <summary>
		/// The types of the available descriptors...
		/// </summary>
		public Dictionary<Type, Type> DescriptorTypes
		{
			get { return _DescriptorTypes ?? (_DescriptorTypes = this.DescriptorTypesCreate); }
		}

		/// <summary>
		/// Creates the default/initial value for DescriptorTypes.
		/// The types of the available descriptors...
		/// </summary>
		protected virtual Dictionary<Type, Type> DescriptorTypesCreate
		{
			get { return new Dictionary<Type, Type>(_StaticDescriptorTypes); }
		}

		#endregion DescriptorTypes

		static Dictionary<Type, Type> _StaticDescriptorTypes = new Dictionary<Type, Type>();

		static Context()
		{
			try
			{
				lock (typeof(Descriptor))
				{
					List<Type> types = TypeHelper.GetDerived(typeof(Descriptor<>));
					Type valueType, descriptor;
					foreach (Type descriptorType in types)
					{
						try
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
						catch (Exception exc)
						{
							Console.WriteLine(exc);
						}
					}
				}
			}
			catch (Exception exc)
			{
				Console.WriteLine(exc);
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

		#region Serializer

		private Serializer _Serializer = default(Serializer);

		public Serializer Serializer
		{
			get { return _Serializer; }
			set
			{
				if (_Serializer != value)
				{
					_Serializer = value;
				}
			}
		}
		#endregion Serializer

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
			if (owner != null && name != null)
				info = owner.GetType().GetProperty(name);

			if (info != null)
				return this.Parse(info, type, owner);
			else
				return this.Parse(name, type, null, outer.Value, null);
		}

		public Descriptor Parse(System.Reflection.PropertyInfo info, Descriptor outer)
		{
			return this.Parse(info, null, outer.Value);
		}
		public Descriptor Parse(System.Reflection.PropertyInfo info, Type type, Descriptor outer)
		{
			dbg.Assert(outer != null, "Outer is null! info='" + info + "'");

			return this.Parse(info, type, outer.Value);
		}

		public Descriptor Parse(System.Reflection.PropertyInfo info, object owner)
		{
			return this.Parse(info, null, owner);
		}
		public Descriptor Parse(System.Reflection.PropertyInfo info, Type type, object owner)
		{
			dbg.Assert(info != null, "Info is null!");

			string name = info.Name;
			type = type ?? info.PropertyType;
			object instance = null;
			if (owner != null && info.GetIndexParameters().Length == 0)
			{
				instance = info.GetValue(owner, null);
			}
			//dbg.Assert(info != null && owner != null && this.CanRead(info, owner), "Property read inaccessible. type='" + type + "' instance='" + instance + "' owner='" + owner + "' " + name);

			return this.Parse(name, type, instance, owner, info);
		}

		protected bool CanRead(System.Reflection.PropertyInfo info, object owner)
		{
			if (owner == null || info == null || !info.CanRead || info.GetIndexParameters().Length != 0)
				return false;

			var method = info.GetGetMethod();
			if (method == null || !method.IsPublic)
				return false;
			return true;
		}

		protected bool CanWrite(System.Reflection.PropertyInfo info, object owner)
		{
			if (owner == null || info == null || !info.CanWrite || info.GetIndexParameters().Length != 0)
				return false;

			var method = info.GetSetMethod();
			if (method == null || !method.IsPublic)
				return false;
			return true;
		}

		int _depth = 0;
		public virtual Descriptor Parse(string name, Type type, object instance, object owner, System.Reflection.PropertyInfo info)
		{
			if (_depth > 500)
			{
				this.Exceptions.Add(new ExceptionTrace(new StackOverflowException("Descriptor Context reached a depth of " + _depth + ". Terminating..."), null, null, type, instance));
				return null;
			}
			try
			{
				_depth++;
				return this._Parse(name, type, instance, owner, info);
			}
			finally
			{
				_depth--;
			}
		}
		Descriptor _Parse(string name, Type type, object instance, object owner, System.Reflection.PropertyInfo info)
		{
			if (type == null && instance != null)
				type = instance.GetType();

			//System.Diagnostics.Debug.Assert(type != null, "Type and instance were null! type='" + type + "' instance='" + instance + "' owner='" + owner + "' " + name);
			if (type == null)
				return null; // throw new NullReferenceException("Type and instance were null! type='" + type + "' instance='" + instance + "' owner='" + owner + "' " + name);

			//if (instance == null)
			//    System.Diagnostics.Debug.Assert(instance == null || type.IsAssignableFrom(instance.GetType()), "Type and instance do not match! type='" + type + "' instance='" + instance + "' instancetype='" + (instance != null?instance.GetType():null) + "' " + name);


			Descriptor desc = null;

			if (!type.IsValueType && type != typeof(string))
				if (instance != null)
				{
					// there's an instance to parse, so search for a value-descriptor...
					if (this.ParsedObjects.ContainsKey(instance))
					{
						this.ParsedObjects[instance].IsReferenced = true;
						return this.ParsedObjects[instance];
					}
				}
				else
				{
					// there's no instance, so search for a type-descriptor...
					if (owner == null)
					{
						while (this.ParsedTypes.ContainsKey(type))
						{
							desc = this.ParsedTypes[type];
							if (desc.Value != null)
							{
								while (this.ParsedTypes.Remove(type)) ;
								if (!this.ParsedObjects.ContainsKey(desc.Value))
									this.ParsedObjects.Add(desc.Value, desc);
								desc = null;
							}
							else
								return desc;
						}
					}
				}

			if (desc == null)
			// we don't have a descriptor yet => find one...
			{
				desc = GetDescriptor(type);

				System.Diagnostics.Debug.Assert(desc != null, "Cannot find descriptor for type='" + type + "' instance='" + instance + "' owner='" + owner + "' " + name);

				if (!type.IsValueType && type != typeof(string))
					if (instance != null)
					{
						// there's an instance to parse, so remember it as a value-descriptor...
						this.ParsedObjects.Add(instance, desc);
					}
					else if (owner == null)
					{
						// there's no instance, so remember it as a type-descriptor...
						this.ParsedTypes.Add(type, desc);
					}
			}

			if (info != null && owner != null && desc.NeedsWriteAccess && !type.IsValueType && !this.CanWrite(info, owner))
				return null;
			System.Diagnostics.Debug.Assert(owner == null || info == null || type.IsValueType || !desc.NeedsWriteAccess || this.CanWrite(info, owner),
				"Property write inaccessible. type='" + type + "' instance='" + instance + "' owner='" + owner + "' " + name);

			if (this.CanRead(info, owner) && (desc.NeedsWriteAccess && !this.CanWrite(info, owner)))
			{
				instance = info.GetValue(owner, null);
			}

			try
			{
				desc.Parse(name, type, instance);
			}
			catch (Exception exc)
			{
				this.Exceptions.Add(new ExceptionTrace(exc, desc, type, name));
			}

			if (!type.IsValueType && type != typeof(string) && desc.Value != null)
			{
				while (this.ParsedTypes.Remove(type)) ;
				instance = desc.Value;
				if (!this.ParsedObjects.ContainsKey(instance))
					this.ParsedObjects.Add(instance, desc);
			}

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
			protected set { _ParsedObjects = value; }
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
			protected set { _ParsedTypes = value; }
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
		Dictionary<int, Descriptor> _CreatedInstances;

		public Dictionary<int, Descriptor> CreatedInstances
		{
			get { return _CreatedInstances ?? (_CreatedInstances = new Dictionary<int, Descriptor>()); }
			protected set { _CreatedInstances = value; }
		}
		#endregion

		#region Exceptions
		private ExceptionCollection _Exceptions;

		/// <summary>
		/// Any exceptions that occured while parsing...
		/// </summary>
		public ExceptionCollection Exceptions
		{
			get { return _Exceptions ?? (_Exceptions = this.ExceptionsCreate); }
			set { _Exceptions = value; }
		}

		/// <summary>
		/// Creates the default/initial value for Exceptions.
		/// Any exceptions that occured while parsing...
		/// </summary>
		protected virtual ExceptionCollection ExceptionsCreate
		{
			get { return new ExceptionCollection(); }
		}

		#endregion Exceptions

		public Context CopyStatics(Context target)
		{
			//target.ParsedObjects = new Dictionary<object,Descriptor>(this.ParsedObjects);
			target.ParsedTypes = new Dictionary<Type, Descriptor>(this.ParsedTypes);
			//target.CreatedInstances = new Dictionary<int, Descriptor>(this.CreatedInstances);
			return target;
		}

		[System.Diagnostics.Conditional("VERBOSE")]
		static internal void CWL(object value)
		{
			Console.WriteLine(value);
		}
	}
}
