using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class TypeHelper
{
	public static int GetSize(this Type type)
	{
		return System.Runtime.InteropServices.Marshal.SizeOf(type);
	}

	public static TInstance CreateInstance<TInstance>()
	{
		return (TInstance)typeof(TInstance).CreateInstance();
	}

	public static TInstance CreateInstance<TInstance>(this Type type)
		 where TInstance : class
	{
		if (type == null)
			return (TInstance)null;
		return (TInstance)type.CreateInstance();
	}

	public static object CreateInstance(this Type type)
	{
		return Activator.CreateInstance(type);
	}

	public static List<TItem> CreateAll<TItem>()
	{
		return typeof(TItem).CreateDerived<TItem>();
	}

	public static List<TItem> CreateDerived<TItem>(this Type basetype)
	{
		List<TItem> results = new List<TItem>();

		IEnumerable<Type> types = basetype.GetDerived(true);

		// scan all loaded assemblies...
		foreach (var type in types)
		{
			//if (type == null || type.GetGenericArguments().Length > 0 || type.IsAbstract || type.GetConstructor(new Type[0]) == null)
			//	continue;

			// create the item...
			TItem inst = (TItem)Activator.CreateInstance(type);

			// add it...
			results.Add(inst);
		}
		return results;
	}

	public static IEnumerable<Type> FindAll<TItem>()
	{
		return typeof(TItem).GetDerived();
	}

	public static IEnumerable<Type> GetDerived(this Type basetype, bool onlyCreateable = false)
	{
		List<Type> derived = basetype.GetDerivedInternal();
		IEnumerable<Type> filtered =
			derived.Where(type =>
				!(type == null
				|| type.GetGenericArguments().Length > 0
				|| type.IsAbstract
				|| type.GetConstructor(new Type[0]) == null));
		return filtered;
	}

	private static List<Type> GetDerivedInternal(this Type type)
	{
		List<Type> results = new List<Type>();

		HashSet<System.Reflection.AssemblyName> loadedNames = new HashSet<System.Reflection.AssemblyName>();
		HashSet<System.Reflection.Assembly> loaded = new HashSet<System.Reflection.Assembly>();
		HashSet<string> loadedPaths = new HashSet<string>();
		List<System.Reflection.Assembly> assemblies = new List<System.Reflection.Assembly>(AppDomain.CurrentDomain.GetAssemblies());
		foreach (var assembly in assemblies)
		{
			loaded.Add(assembly);
			loadedNames.Add(assembly.GetName());
			try
			{
				loadedPaths.Add(assembly.Location);
			}
			catch (NotSupportedException)
			{
			}
		}

		// scan for local assemblies that were not loaded...
		string[] files = System.IO.Directory.GetFiles(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "*.dll", System.IO.SearchOption.TopDirectoryOnly);
		foreach (string file in files)
		{
			if (loadedPaths.Contains(file) || IgnoreLocalAssemblies.Any(s => file.Contains(s)))
				continue;

			Logger.Instance.WriteLog("TypeHelper", 0, "Loading assembly '" + file + "'...");
			try
			{
				loadedPaths.Add(file);
				var assembly = System.Reflection.Assembly.LoadFile(file);
				AppDomain.CurrentDomain.Load(assembly.GetName());

				loaded.Add(assembly);
				loadedNames.Add(assembly.GetName());
				assemblies.Add(assembly);
			}
			catch (Exception exc)
			{
				Logger.Instance.WriteLog("TypeHelper", 0, "Failed to load assembly '" + file + "':" + Environment.NewLine + exc);
			}
		}

		// scan all loaded assemblies...
		foreach (var assembly in assemblies)
		{
			try
			{
				LoadTypes(type, assembly, loaded, loadedNames, results);
			}
			catch (Exception e)
			{
				Logger.Instance.WriteLog("Failed to load assembly: " + assembly.FullName);
			}
		}
		return results;
	}

	public static List<string> IgnoreAssemblies = new List<string>
		{
				"mscorlib",
				"Microsoft.",
				"System.",
				"System,",
				"vshost",
				"Presentation",
				"Accessibility,",
				"ReachFramework,",
				"UIAutomation",
				"WindowsBase"
		};

	public static List<string> IgnoreLocalAssemblies = new List<string>();

	private static void LoadTypes(this Type basetype, System.Reflection.Assembly assembly, HashSet<System.Reflection.Assembly> loaded, HashSet<System.Reflection.AssemblyName> loadedNames, List<Type> results)
	{
		// ignore standard libraries...
		if (IgnoreAssemblies.Any(s => assembly.FullName.StartsWith(s)))
			return;
		//if (assembly.FullName.StartsWith("mscorlib")
		//	|| assembly.FullName.StartsWith("Microsoft.")
		//	|| assembly.FullName.StartsWith("System.")
		//	|| assembly.FullName.StartsWith("System,")
		//	|| assembly.FullName.StartsWith("vshost")
		//	|| assembly.FullName.StartsWith("Presentation")
		//	|| assembly.FullName.StartsWith("Accessibility,")
		//	|| assembly.FullName.StartsWith("ReachFramework,")
		//	|| assembly.FullName.StartsWith("UIAutomation")
		//	|| assembly.FullName.StartsWith("WindowsBase"))
		//	return;

		// parse all referenced assemblies...
		System.Reflection.AssemblyName[] referenced = assembly.GetReferencedAssemblies();
		foreach (var reference in referenced)
		{
			// skip if already parsed...
			if (loadedNames.Contains(reference))
				continue;
			// skip if explicitly ignored...
			if (IgnoreAssemblies.Any(s => reference.FullName.StartsWith(s)))
				continue;

			// load the assembly from the reference's name...
			System.Reflection.Assembly extass = System.Reflection.Assembly.Load(reference);
			// try to find the matching assembly in the appdomain (e.g. the runtime has already loaded the assembly so we don't want to use our extra assembly)...
			System.Reflection.Assembly ass = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName == extass.FullName) ?? extass;

			// remember the reference...
			loadedNames.Add(reference);

			if (loaded.Contains(ass))
				continue;
			// remember the actual assembly...
			loaded.Add(ass);

			// recursively continue loading new assemblies...
			LoadTypes(basetype, ass, loaded, loadedNames, results);
			//LoadTypes<TItem>(ass, loaded, loadedNames, results);
		}

		//Type basetype = typeof(TItem);

		Type[] types;
		try
		{
			types = assembly.GetTypes();
		}
		catch (System.Reflection.ReflectionTypeLoadException exc)
		{
			types = exc.Types;
		}

		// scan all available types...
		foreach (Type type in types)
		{
			if (type == null)
				continue; // should not happen but there is a bug in older .NET versions...

			// see if it is the right type...
			if (!basetype.IsAssignableFrom(type))
				continue;

			// see if it can create an instance...
			if (type.IsAbstract)
				continue;
			if (type.GetConstructor(new Type[0]) == null)
				continue;

			// add it...
			results.Add(type);
		}
	}
}