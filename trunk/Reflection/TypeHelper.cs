#region LGPL License
//********************************************************************
//	author:         Thomas "zeroflag" Kraemer
//	author email:   zeroflag >>at<< zeroflag >>dot<< de
//	
//	Copyright (C) 2006-2009  Thomas "zeroflag" Kraemer
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

#region BSD License
/*
 * Copyright (c) 2006-2008, Thomas "zeroflag" Kraemer
 * All rights reserved.
 * Redistribution and use in source and binary forms, with or without modification, 
 * are permitted provided that the following conditions are met:
 * 
 * Redistributions of source code must retain the above copyright notice, this list 
 * of conditions and the following disclaimer. Redistributions in binary form must 
 * reproduce the above copyright notice, this list of conditions and the following 
 * disclaimer in the documentation and/or other materials provided with the distribution.
 * Neither the name of the SAGEngine nor the names of its contributors may be used 
 * to endorse or promote products derived from this software without specific prior 
 * written permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE 
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE 
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE 
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR 
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
 * GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
 * HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT 
 * LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT 
 * OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */
#endregion BSD License

using System;
using System.Text;
using System.Collections.Generic;
using Assembly = System.Reflection.Assembly;

public static class TypeExtensions
{
	public static object CreateInstance(this Type type)
	{
		return zeroflag.Reflection.TypeHelper.CreateInstance(type);
	}

	public static Type Specialize(this Type type, params System.Type[] generics)
	{
		return zeroflag.Reflection.TypeHelper.SpecializeType(type, generics);
	}

	public static List<Type> GetDerived(this Type type)
	{
		return zeroflag.Reflection.TypeHelper.GetDerived(type);
	}

	public static bool IsDerivedFrom(this Type type, Type other)
	{
		return zeroflag.Reflection.TypeHelper.IsDerived(type, other);
	}

	public static List<Type> GetBaseTypesAndInterfaces(this Type type)
	{
		return zeroflag.Reflection.TypeHelper.GetAllBaseTypesAndInterfaces(type);
	}

	//public static List<Type> GetBaseTypesAndInterfaces( this Type type, List<Type> results )
	//{
	//    return zeroflag.Reflection.TypeHelper.GetAllBaseTypesAndInterfaces( type, results );
	//}

	public static List<Type> GetBaseTypes(this Type type)
	{
		return zeroflag.Reflection.TypeHelper.GetBaseTypes(type);
	}

	public static bool IsGenericSimilar(this Type type, Type other)
	{
		return zeroflag.Reflection.TypeHelper.IsGenericSimilar(type, other);
	}
}

namespace zeroflag
{
	public class TypeHelper : zeroflag.Reflection.TypeHelper
	{
	}

	namespace Reflection
	{
		public class TypeHelper
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

#if !SILVERLIGHT
			public static List<Type> ScanAssemblies()
			{
				return ScanAssemblies((System.Reflection.Assembly[])new Assembly[] { Assembly.GetExecutingAssembly() });
			}
			public static List<Type> ScanAssemblies(params System.Reflection.Assembly[] assemblies)
			{
				try
				{
					lock (Assemblies)
					{
						lock (_Types)
						{
							//System.Reflection.Assembly[] available = AvailableAssemblies ?? (AvailableAssemblies = AppDomain.CurrentDomain.GetAssemblies());
							assemblies = assemblies ?? AppDomain.CurrentDomain.GetAssemblies();

							// check if all assemblies are already parsed...
							foreach (System.Reflection.Assembly assembly in assemblies)// AppDomain.CurrentDomain.GetAssemblies())
							{
								if (!Assemblies.Contains(assembly))
								{
									Assemblies.Add(assembly);

									foreach (var name in assembly.GetReferencedAssemblies())
										ScanAssemblies(AppDomain.CurrentDomain.Load(name));

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
										if (!_Types.Contains(type))
										{
											_Types.Add(type);
											if (type.FullName != null && !TypeNames.ContainsKey(type.FullName))
												TypeNames.Add(type.FullName, type);
										}
									}

								}
								// check if there are any assemblies depending on the current...
								//if (assemblies != available)
								//    foreach (var other in available)
								//    {
								//        foreach (var refe in other.GetReferencedAssemblies())
								//        {
								//            if (refe.FullName == assembly.GetName().FullName)
								//                // if it depends, scan it...
								//                ScanAssemblies(other);
								//        }
								//    }
							}
							return _Types;
						}
					}
				}
				catch (Exception exc)
				{
					System.Diagnostics.Debugger.Log(0, "zeroflag.Reflection.TypeHelper", exc.ToString());
					Console.WriteLine(exc);
					return _Types;
				}
			}
#else
			public static List<Type> ScanAssemblies()
			{
				try
				{
					lock ( Assemblies )
					{
						lock ( _Types )
						{
							ScanAssembly( Assembly.GetCallingAssembly() );
							ScanAssembly( Assembly.GetExecutingAssembly() );

							return _Types;
						}
					}
				}
				catch ( Exception exc )
				{
					System.Diagnostics.Debugger.Log( 0, "zeroflag.Reflection.TypeHelper", exc.ToString() );
					Console.WriteLine( exc );
					return _Types;
				}
			}


			static void ScanAssembly( Assembly assembly )
			{
				if ( !Assemblies.Contains( assembly ) )
				{
					Assemblies.Add( assembly );

					//foreach ( var name in assembly.GetReferencedAssemblies() )
					//    ScanAssemblies( AppDomain.CurrentDomain.Load( name ) );

					// assembly hasn't been parsed yet...
					Type[] types = null;
					try
					{
						types = assembly.GetExportedTypes();
					}
					catch ( System.Reflection.ReflectionTypeLoadException exc )
					{
						types = exc.Types;
					}
					// add all types...
					foreach ( System.Type type in types )
					{
						// avoid duplicates...
						if ( !_Types.Contains( type ) )
						{
							_Types.Add( type );
							if ( type.FullName != null && !TypeNames.ContainsKey( type.FullName ) )
								TypeNames.Add( type.FullName, type );
							if ( type.BaseType != null )
								ScanAssembly( type.BaseType.Assembly );
						}
					}

				}

			}
#endif

			static List<Type> _Types = new List<Type>();

			public static List<Type> Types
			{
				get
				{
#if !SILVERLIGHT
					if (Assemblies.Count != AppDomain.CurrentDomain.GetAssemblies().Length)
#else
#endif
						ScanAssemblies();
					return TypeHelper._Types;
				}
			}

			static TypeHelper()
			{
				ScanAssemblies();
			}

			static Dictionary<string, Type> TypeNames = new Dictionary<string, Type>();
			static Dictionary<Type, List<Type>> Derived = new Dictionary<Type, List<Type>>();
			static List<System.Reflection.Assembly> Assemblies = new List<System.Reflection.Assembly>();
			//static System.Reflection.Assembly[] AvailableAssemblies = null;
			public static List<Type> GetDerived(System.Type baseType)
			{
				//if (baseType.IsGenericType)
				//    baseType = baseType.GetGenericTypeDefinition();

				if (!Derived.ContainsKey(baseType) || Derived[baseType] == null)
					lock (Derived)
						// check if the type was already parsed...
						if (!Derived.ContainsKey(baseType) || Derived[baseType] == null)
						{
							Derived[baseType] = new List<Type>();

							//TODO: ScanAssemblies(baseType.Assembly);
							//ScanAssemblies(AppDomain.CurrentDomain.GetAssemblies());

							// find all types directly derived from the type...
							lock (_Types)
							{
								foreach (System.Type type in Types)
								{
									if (type == null)
										continue;
									//if (type.IsGenericType)
									//    type = type.GetGenericTypeDefinition();
									if (type != null && (baseType.IsAssignableFrom(type) || type.IsSubclassOf(baseType) || IsDerived(baseType, type.BaseType)))
									{
										Derived[baseType].Add(type);
									}
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

			public static bool IsGenericSimilar(Type type, Type other)
			{
				if (!type.IsGenericType || !other.IsGenericType)
					return false;
				if (type.GetGenericTypeDefinition().IsAssignableFrom(other.GetGenericTypeDefinition()) || other.GetGenericTypeDefinition().IsAssignableFrom(type.GetGenericTypeDefinition()))
					return true;
				//List<Type> baseA = type.GetBaseTypes();
				List<Type> baseB = other.GetBaseTypes();
				foreach (Type b in baseB)
				{
					if (b != other && IsGenericSimilar(type, b))
						return true;
				}
				//foreach ( Type a in baseA )
				//{
				//    if ( a.IsGenericType && ( a.GetGenericTypeDefinition().IsAssignableFrom( other.GetGenericTypeDefinition() ) || other.GetGenericTypeDefinition().IsAssignableFrom( a.GetGenericTypeDefinition() ) ) )
				//        return true;

				//    Type t = a;
				//    if ( t.IsGenericType && !t.IsGenericTypeDefinition )
				//    {
				//        t = t.GetGenericTypeDefinition();
				//        if ( t.IsGenericTypeDefinition )
				//            if ( t.IsGenericSimilar( other ) )
				//                return true;
				//    }
				//    if ( baseB.Find( b => b.IsGenericType && ( b.GetGenericTypeDefinition().IsAssignableFrom( t ) || t.IsAssignableFrom( b.GetGenericTypeDefinition() ) ) ) != null )
				//        return true;

				//}
				return false;
			}

			public static Type GetType(string name)
			{
				if (name == null)
					return null;
				Type type = null;
				if (TypeNames.ContainsKey(name))
					type = TypeNames[name];
				else
				{
					type = Type.GetType(name);
					if (type == null)
						foreach (System.Reflection.Assembly ass in Assemblies)
						{
							if ((type = ass.GetType(name)) != null)
								break;
						}
					if (type == null)
						foreach (Type t in Types)
						{
							if (t.Name != null)
							{
								TypeNames[t.Name] = t;
								//TypeNames.Add( t.Name.ToLower(), t );
								if (name.Contains(t.Name) || name.ToLower().Contains(t.Name.ToLower()))
								{
									type = t;
									break;
								}
							}
						}
				}

				return type;
			}

			public static Type GetType(string name, Type baseType)
			{
				if (name == null)
					return null;

				List<Type> types = GetDerived(baseType);
				types.Add(baseType);

				foreach (Type type in types)
				{
					if (type.Name == name)
						return type;
				}
				foreach (Type type in types)
				{
					if (type.Name.StartsWith(name) || type.Name.EndsWith(name))
						return type;
				}
				return null;
			}

			public static List<Type> GetAllBaseTypesAndInterfaces(Type type)
			{
				return GetAllBaseTypesAndInterfaces(type, new List<Type>(), true);
			}

			public static List<Type> GetAllBaseTypesAndInterfaces(Type type, List<Type> results, bool includeInterfaces)
			{
				if (type != null && type != typeof(object))
				{
					if (!results.Contains(type))
						results.Add(type);

					GetAllBaseTypesAndInterfaces(type.BaseType, results, includeInterfaces);

					if (includeInterfaces)
						foreach (Type interf in type.GetInterfaces())
							GetAllBaseTypesAndInterfaces(interf, results, includeInterfaces);
				}

				return results;
			}

			public static List<Type> GetBaseTypes(Type type)
			{
				return GetAllBaseTypesAndInterfaces(type, new List<Type>(), false);
			}
		}
	}
}