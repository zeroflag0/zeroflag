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

namespace zeroflag
{
	public static class TypeHelper
	{
		public static object CreateInstance(System.Type type)
		{
			return System.Activator.CreateInstance(type);
		}

		public static object CreateInstance(System.Type type, params System.Type[] generics)
		{
			if (type == null)
				return null;
			//System.Activator.CreateInstance(type, null, 
			return CreateInstance(SpecializeType(type, generics));
		}

		public static Type SpecializeType(System.Type type, params System.Type[] generics)
		{
			if (type == null)
				return type;
			if (type.IsGenericTypeDefinition)
			{
				if (!type.IsGenericTypeDefinition)
					type = type.GetGenericTypeDefinition();
				type = type.MakeGenericType(generics);
			}
			return type;
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
