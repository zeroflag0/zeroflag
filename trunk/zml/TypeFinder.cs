using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Zml
{
	public class TypeFinder// : IDictionary<string, Type>, ICollection<Type>
	{
		#region Singleton
		static TypeFinder _Instance;

		public static TypeFinder Instance
		{
			get
			{
				if (TypeFinder._Instance == null)
				{
					lock (typeof(TypeFinder))
					{
						if (TypeFinder._Instance == null)
						{
							TypeFinder._Instance = new TypeFinder();
						}
					}
				}
				return TypeFinder._Instance;
			}
		}

		protected TypeFinder()
		{
			Scan();
		}
		#endregion Singleton

		#region Types
		public void Scan()
		{
			this.Add(typeof(List<>));
			this.ScanAssemblies();

			foreach (Type type in this.Types)
			{
				this.Add(type);
			}
		}

		protected virtual void Add(Type type)
		{
			this.TypeNames[type.FullName] = type;

			string key = type.Name;
			if (key == null)
				key = type.FullName.Substring(type.FullName.LastIndexOf('.') + 1);
			this.TypeNames[key] = type;

			try
			{
				int i = key.IndexOfAny(new char[] { '`', '~', '<', '>' });
				if (i > 0)
				{
					string temp = this.TryAdd(key.Substring(0, i), type);
					temp = temp.ToLower();
					this.TryAdd(temp, type);
				}
			}
			catch { }

			this.TryAdd(key.ToLower(), type);
		}

		string TryAdd(string key, Type type)
		{
			if (key.Length > 0 && !this.TypeNames.ContainsKey(key))
				this.TypeNames[key] = type;
			return key;
		}

		void ScanAssemblies()
		{
			if (available == null)
				available = new List<System.Reflection.Assembly>(AppDomain.CurrentDomain.GetAssemblies());
			if (availableNames == null)
			{
				availableNames = new Dictionary<string, System.Reflection.Assembly>();
				foreach (var ass in available)
				{
					availableNames.Add(ass.FullName, ass);
				}
			}

			List<System.Reflection.Assembly> order = new List<System.Reflection.Assembly>();

			this.Order(System.Reflection.Assembly.GetExecutingAssembly(), order);

			this.ScanAssemblies(order.ToArray());
		}

		void Order(System.Reflection.Assembly current, List<System.Reflection.Assembly> ordered)
		{
			var arr = available.ToArray();
			do
			{
				foreach (var assembly in arr)
					foreach (var name in assembly.GetReferencedAssemblies())
					{
						try
						{
							if (!availableNames.ContainsKey(name.FullName))
							{
								var ass = System.Reflection.Assembly.Load(name.FullName);
								available.Add(ass);
								availableNames.Add(ass.FullName, ass);
							}
							//var other = availableNames[name.FullName];
							//Console.WriteLine("`" + assembly.GetName().Name + "` depends on `" + other.GetName().Name + "`");
							//this.Order(other, ordered);
						}
						catch (KeyNotFoundException)
						{
							Console.WriteLine("reference " + name.FullName + " not found for " + assembly.FullName + "...");
						}
						catch (Exception exc)
						{
							Console.WriteLine(exc);
						}
					}
				foreach (var assembly in arr)
				{
					if (!ordered.Contains(assembly))
					{
						//Console.WriteLine("Inserted " + assembly.GetName().Name);
						ordered.Insert(0, assembly);
					}
				}
			}
			while ((arr == null || arr.Length != available.Count) && (arr = available.ToArray()) != null);
			return;
			if (ordered.Contains(current))
				return;

			// check if there are any assemblies depending on the current...
			foreach (var other in available.ToArray())
			{
				//foreach (var refe in other.GetReferencedAssemblies())
				//{
				//    if (refe.FullName == current.FullName)
				var temp = new List<System.Reflection.AssemblyName>(other.GetReferencedAssemblies());
				if (temp.Find(o => o.FullName == current.FullName) != null)
				{
					// if it depends, scan it...
					//Console.WriteLine("`" + other.GetName().Name + "` depends on `" + current.GetName().Name + "`");
					Console.WriteLine("`" + current.GetName().Name + "` is referenced by `" + other.GetName().Name + "`");
					Order(other, ordered);
				}
				//}
			}

			Console.WriteLine("Inserted " + current.GetName().Name);
			ordered.Add(current);

			foreach (var name in current.GetReferencedAssemblies())
			{
				try
				{
					if (!availableNames.ContainsKey(name.FullName))
					{
						var ass = System.Reflection.Assembly.Load(name.FullName);
						available.Add(ass);
						availableNames.Add(ass.FullName, ass);
					}
					var other = availableNames[name.FullName];
					Console.WriteLine("`" + current.GetName().Name + "` depends on `" + other.GetName().Name + "`");
					this.Order(other, ordered);
				}
				catch (KeyNotFoundException)
				{
					Console.WriteLine("reference " + name.FullName + " not found for " + current.FullName + "...");
				}
				catch (Exception exc)
				{
					Console.WriteLine(exc);
				}
			}

		}

		List<System.Reflection.Assembly> available = null;
		Dictionary<string, System.Reflection.Assembly> availableNames = null;
		void ScanAssemblies(params System.Reflection.Assembly[] assemblies)
		{
			if (available == null)
				available = new List<System.Reflection.Assembly>(AppDomain.CurrentDomain.GetAssemblies());
			if (availableNames == null)
			{
				availableNames = new Dictionary<string, System.Reflection.Assembly>();
				foreach (var ass in available)
				{
					availableNames.Add(ass.FullName, ass);
				}
			}

			lock (Assemblies)
			{
				// check if all assemblies are already parsed...
				foreach (System.Reflection.Assembly assembly in assemblies)// AppDomain.CurrentDomain.GetAssemblies())
				{
					if (!Assemblies.Contains(assembly))
					{
						Assemblies.Add(assembly);

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
							{
								Types.Add(type);
								if (type.FullName != null && !TypeNames.ContainsKey(type.FullName) && this.ValidateType(type))
									this.Add(type);
							}
						}

					}
				}
			}
		}

		protected virtual bool ValidateType(Type type)
		{
			return type.IsPublic && !type.IsAbstract && !type.IsInterface && type.GetConstructor(System.Type.EmptyTypes) != null;
		}

		List<Type> Types = new List<Type>();
		List<System.Reflection.Assembly> Assemblies = new List<System.Reflection.Assembly>();
		private System.Collections.Generic.Dictionary<string, Type> _TypeNames = new System.Collections.Generic.Dictionary<string, Type>();

		protected System.Collections.Generic.Dictionary<string, Type> TypeNames
		{
			get { return _TypeNames; }
		}
		#endregion Types

		public virtual Type this[string key]
		{
			get
			{
				return this.Find(key);
			}
			set
			{
				this.Add(value);
				this.TypeNames[key] = value;
			}
		}


		#region Finder

		protected delegate Type FindHandler(string key);
		protected delegate string SearchHandler(string key);

		protected Type Find(string key)
		{
			return this.TryGet(key)
				?? this.TryGet(key.ToLower())
				?? this.Search(key);
		}

		List<string> tempKeys;

		protected Type Search(string key)
		{
			List<Type> stack = this.SearchAll(key);
			if (stack != null && stack.Count > 0)
				return stack[0];
			else
				return null;
		}

		public List<Type> SearchAll(string key)
		{
			if (tempKeys == null || tempKeys.Count != this.TypeNames.Count)
				tempKeys = new List<string>(this.TypeNames.Keys);

			Type result = null;
			List<Type> results = new List<Type>();
			if (result != null && !results.Contains(result)) results.Insert(0, result);
			List<string> found = tempKeys.FindAll(k => k.Contains(key));
			found.AddRange(tempKeys.FindAll(k => k.ToLower().Contains(key.ToLower())));

			string temp = key;
			string target = null;
			string targetMatch = null;
			foreach (string f in found)
			{
				temp = f;
				if (temp.Contains("."))
					temp = temp.Substring(temp.LastIndexOf('.') + 1);
				int i = temp.IndexOfAny(new char[] { '`', '~', '<', '>' });
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
					if (target == null || Math.Abs(targetMatch.Length - key.Length) > Math.Abs(temp.Length - key.Length) || Math.Abs(target.Length) > Math.Abs(f.Length))
					{
						targetMatch = temp;
						target = f;
						result = this.TryGet(target);
						if (result != null && !results.Contains(result))
						{
							if (results.Count <= 0)
								results.Add(result);
							else if (results[0].FullName.Length < result.FullName.Length)
							{
								Console.WriteLine(result.FullName + " is longer than " + results[0].FullName);
								results.Insert(1, result);
							}
							else
							{
								results.Insert(0, result);
							}
						}
					}
			}

			result = this.TryGet(target);
			if (result != null && !results.Contains(result)) results.Insert(0, result);
			result = GetType(key);
			if (result != null && !results.Contains(result))
			{
				if (results[0].FullName.Length < target.Length && results.Count > 0)
					results.Insert(1, result);
				else
					results.Insert(0, result);
			}

			return results;

			//foreach (string f in found)
			//    if ((result = this.TryGet(f)) != null)
			//        return result;
			//found = tempKeys.FindAll(k => k.ToLower().StartsWith(key.ToLower()));
			//foreach (string f in found)
			//    if ((result = this.TryGet(f)) != null)
			//        return result;
			//found = tempKeys.FindAll(k => k.Contains(key));
			//foreach (string f in found)
			//    if ((result = this.TryGet(f)) != null)
			//        return result;
			//found = tempKeys.FindAll(k => k.ToLower().EndsWith(key.ToLower()));
			//foreach (string f in found)
			//    if ((result = this.TryGet(f)) != null)
			//        return result;


			//return this.TryGet(
			//    tempKeys.Find(k => k.StartsWith(key)) ??
			//    tempKeys.Find(k => k.ToLower().StartsWith(key.ToLower())) ??
			//    tempKeys.Find(k => k.EndsWith(key)) ??
			//    tempKeys.Find(k => k.ToLower().EndsWith(key.ToLower()))
			//    ) ?? this.GetType(key);
		}
		public Type GetType(string name)
		{
			Type type = null;
			if (TypeNames.ContainsKey(name))
				type = TypeNames[name];
			else
			{
				foreach (System.Reflection.Assembly ass in AppDomain.CurrentDomain.GetAssemblies())
				{
					if ((type = ass.GetType(name)) != null)
						break;
				}
			}

			return type;
		}
		protected Type TryGet(string key)
		{
			try
			{
				if (this.TypeNames.ContainsKey(key))
					return this.TypeNames[key];
			}
			catch
			{
			}
			return null;
		}


		#endregion Finder


	}
}
