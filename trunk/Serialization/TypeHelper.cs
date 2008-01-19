using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag
{
	public static class TypeHelper
	{
		public static object CreateInstance(System.Type type)
		{
			return System.Activator.CreateInstance(type);
		}

		static List<Type> Types = new List<Type>();
		static Dictionary<Type, List<Type>> Derived = new Dictionary<Type, List<Type>>();
		static List<System.Reflection.Assembly> Assemblies = new List<System.Reflection.Assembly>();
		public static List<Type> GetDerived(System.Type baseType)
		{
			//if (baseType.IsGenericType)
			//    baseType = baseType.GetGenericTypeDefinition();

			// check if the type was already parsed...
			if (!Derived.ContainsKey(baseType) || Derived[baseType] == null)
			{
				Derived[baseType] = new List<Type>();

				// check if all assemblies are already parsed...
				foreach (System.Reflection.Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
				{
					if (!Assemblies.Contains(assembly))
					{
						// assembly hasn't been parsed yet...
						Type[] types = null;
						try
						{
							types = assembly.GetTypes();
						}
						catch (System.Reflection.ReflectionTypeLoadException exc)
						{
							types = exc.Types;
						}
						// add all types...
						foreach (System.Type type in types)
						{
							// avoid duplicates...
							if (!Types.Contains(type))
								Types.Add(type);
						}
					}
				}
				// find all types directly derived from the type...
				foreach (System.Type type in Types)
				{
					//if (type.IsGenericType)
					//    type = type.GetGenericTypeDefinition();
					if (type != null && (type.IsSubclassOf(baseType) || IsDerived(baseType, type.BaseType)))
					{
						Derived[baseType].Add(type);
					}
				}

			}
			return Derived[baseType];
		}

		public static bool IsDerived(System.Type baseType, System.Type type)
		{
			if (object.ReferenceEquals(baseType, null) || baseType.Equals(typeof(object)) || object.ReferenceEquals(type, null) || type.Equals(typeof(object)))
				return false;
			if (type.IsGenericType)
				type = type.GetGenericTypeDefinition();
			if (baseType.IsInterface)
			{
				//TODO: implement interface checking...
				//return new List<Type>(type.GetInterfaces()).Contains(baseType);
			}
			if (baseType.Equals(type))
				return true;
			else
				return IsDerived(baseType, type.BaseType);
		}
	}
}
