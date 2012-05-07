using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace zeroflag.Serialization
{
	public class PropertyFinder // : IDictionary<string, PropertyInfo>, ICollection<PropertyInfo>
	{
		#region Singleton

		private static PropertyFinder _Instance;

		public static PropertyFinder Instance
		{
			get
			{
				if (_Instance == null)
				{
					lock (typeof (PropertyFinder))
					{
						if (_Instance == null)
						{
							_Instance = new PropertyFinder();
						}
					}
				}
				return _Instance;
			}
		}

		protected PropertyFinder()
		{
			this.Scan();
		}

		#endregion Singleton

		#region Properties

		private readonly List<Assembly> Assemblies = new List<Assembly>();
		private readonly Dictionary<Type, Dictionary<string, PropertyInfo>> _PropertyNames = new Dictionary<Type, Dictionary<string, PropertyInfo>>();

		public void Scan()
		{
			this.ScanTypes();
		}

		protected virtual void Add(Type type, PropertyInfo prop)
		{
			if (!this.PropertyNames.ContainsKey(type))
			{
				this.ScanTypes(type);
			}

			string key = prop.Name;
			this.PropertyNames[type][key] = prop;

			this.TryAdd(type, key.ToLower(), prop);
		}

		private string TryAdd(Type type, string key, PropertyInfo prop)
		{
			if (key.Length > 0 && !this.PropertyNames[type].ContainsKey(key))
			{
				this.PropertyNames[type][key] = prop;
			}
			return key;
		}

		//void ScanTypes()
		//{
		//    this.ScanTypes(order.ToArray());
		//}

		private void ScanTypes(params Type[] types)
		{
			lock (this.Assemblies)
			{
				// check if all assemblies are already parsed...
				foreach (Type type in types) // AppDomain.CurrentDomain.GetAssemblies())
				{
					if (!this.PropertyNames.ContainsKey(type))
					{
						this.PropertyNames[type] = new Dictionary<string, PropertyInfo>();
					}


					// assembly hasn't been parsed yet...
					var props = new List<PropertyInfo>();

					Type current = type;
					do
					{
						props.AddRange(current.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty | BindingFlags.GetProperty));
					} while ((current = current.BaseType) != null);

					// add all properties...
					foreach (PropertyInfo prop in props)
					{
						//if (prop.Name != null && this.ValidateProperty(prop))
						this.Add(type, prop);
					}
				}
			}
		}

		protected virtual bool ValidateProperty(PropertyInfo prop)
		{
			return this.IsValidAccessor(prop.GetGetMethod()) && this.IsValidAccessor(prop.GetSetMethod());
		}

		private bool IsValidAccessor(MethodInfo acc)
		{
			return acc != null && acc.IsPublic && !acc.IsAbstract;
		}

		protected Dictionary<Type, Dictionary<string, PropertyInfo>> PropertyNames
		{
			get { return this._PropertyNames ?? this._PropertyNames; }
		}

		#endregion Properties

		public virtual PropertyInfo this[Type type, string key]
		{
			get { return this.Find(type, key); }
			set
			{
				this.Add(type, value);
				this.PropertyNames[type][key] = value;
			}
		}

		#region Finder

		protected delegate PropertyInfo FindHandler(string key);

		protected delegate string SearchHandler(string key);

		private List<string> tempKeys;

		protected PropertyInfo Find(Type type, string key)
		{
			return this.TryGet(type, key)
				?? this.TryGet(type, key.ToLower())
					?? this.Search(type, key);
		}

		protected PropertyInfo Search(Type type, string key)
		{
			List<PropertyInfo> stack = this.SearchAll(type, key);
			if (stack != null && stack.Count > 0 && stack[0] != null)
			{
				return stack[0];
			}
			else
			{
				return null;
			}
		}

		public PropertyInfo SearchType(Type type, string key)
		{
			Collections.List<Type> possibleMatches = TypeFinder.Instance.SearchAll(key);
			PropertyInfo[] props = type.GetProperties();
			foreach (PropertyInfo info in props)
			{
				if (possibleMatches.Contains(info.PropertyType))
				{
					return info;
				}
			}
			var collections = new List<Type>();
			foreach (Type match in possibleMatches)
			{
				collections.Add(Reflection.TypeHelper.SpecializeType(typeof (ICollection<>), match));
			}
			foreach (PropertyInfo info in props)
			{
				foreach (Type coll in collections)
				{
					if (coll.IsAssignableFrom(info.PropertyType))
					{
						return info;
					}
				}
			}

			return null;
		}

		public List<PropertyInfo> SearchAll(Type type, string key)
		{
			if (!this.PropertyNames.ContainsKey(type))
			{
				this.ScanTypes(type);
			}
			if (this.tempKeys == null || this.tempKeys.Count != this.PropertyNames[type].Count)
			{
				this.tempKeys = new List<string>(this.PropertyNames[type].Keys);
			}

			PropertyInfo result = null;
			var results = new List<PropertyInfo>();

			var found = new List<string>(this.tempKeys.Where(k => k.Contains(key)));
			found.AddRange(this.tempKeys.Where(k => k.ToLower().Contains(key.ToLower())));

			string temp = key;
			string target = null;
			string targetMatch = null;
			foreach (string f in found)
			{
				temp = f;
				if (temp.Contains("."))
				{
					temp = temp.Substring(temp.LastIndexOf('.') + 1);
				}
				int i = temp.IndexOfAny(new[] { '`', '~', '<', '>' });
				if (i > 0)
				{
					temp = temp.Substring(0, i);
				}
				if (temp == key || temp.ToLower() == key)
				{
					targetMatch = temp;
					target = f;
					break;
				}
				else if (temp.Contains(key) || key.Contains(temp))
				{
					if (target == null || Math.Abs(targetMatch.Length - key.Length) > Math.Abs(temp.Length - key.Length) || Math.Abs(target.Length) > Math.Abs(f.Length))
					{
						targetMatch = temp;
						target = f;
						result = this.TryGet(type, target);
						if (result != null && !results.Contains(result))
						{
							if (results.Count <= 0)
							{
								results.Add(result);
							}
							else if (results[0].Name.Length < result.Name.Length)
							{
								Console.WriteLine(result.Name + " is longer than " + results[0].Name);
								results.Insert(1, result);
							}
							else
							{
								results.Insert(0, result);
							}
						}
					}
				}
			}

			result = this.TryGet(type, target);
			if (results.Count <= 0)
			{
				results.Add(result);
			}
			else if (results[0].Name.Length < result.Name.Length)
			{
				Console.WriteLine(result.Name + " is longer than " + results[0].Name);
				results.Insert(1, result);
			}
			else
			{
				results.Insert(0, result);
			}

			return results;
		}

		protected PropertyInfo TryGet(Type type, string key)
		{
			try
			{
				if (this.PropertyNames[type].ContainsKey(key))
				{
					return this.PropertyNames[type][key];
				}
			}
			catch
			{
			}
			return null;
		}

		#endregion Finder
	}
}
