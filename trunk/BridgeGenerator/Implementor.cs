using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace zeroflag.BridgeGenerator
{
	public class Implementor
	{
		private List<Type> _Interfaces = new List<Type>();
		private Type _Implementation;
		private string _Property;
		private string _ClassName;
		private string _SourceCode;
		private List<Type> _GenericTypes = new List<Type>();
		private List<Type> _IgnoreInterfaces = new List<Type>();

		public List<Type> GenericTypes
		{
			get { return _GenericTypes; }
		}

		public string SourceCode
		{
			get { return _SourceCode; }
			set { _SourceCode = value; }
		}

		/// <summary>
		/// The name for the class generated.
		/// </summary>
		public string ClassName
		{
			get { return _ClassName; }
			set { _ClassName = value; }
		}

		/// <summary>
		/// The property that should hold the bridged target.
		/// </summary>
		public string Property
		{
			get { return _Property; }
			set { _Property = value; }
		}

		/// <summary>
		/// The type that should be bridged to.
		/// </summary>
		public Type Implementation
		{
			get { return _Implementation; }
			set { _Implementation = value; }
		}

		/// <summary>
		/// The interfaces which the bridge should implement.
		/// </summary>
		public List<Type> Interfaces
		{
			get { return _Interfaces; }
		}

		/// <summary>
		/// When implementing base-interfaces/classes, these will be ignored...
		/// </summary>
		public List<Type> IgnoreInterfaces
		{
			get { return _IgnoreInterfaces; }
			set { _IgnoreInterfaces = value; }
		}

		bool _IgnoreInterfaceMembers = false;
		/// <summary>
		/// Whether an ignored interfaces members should be ignored if implemented by an un-ignored interface.
		/// </summary>
		public bool IgnoreInterfaceMembers
		{
			get { return _IgnoreInterfaceMembers; }
			set { _IgnoreInterfaceMembers = value; }
		}

		bool _ImplementOverrides = true;
		/// <summary>
		/// Whether override stubs should be created for virtual members of the basetype...
		/// </summary>
		public bool ImplementOverrides
		{
			get { return _ImplementOverrides; }
			set { _ImplementOverrides = value; }
		}


		Type _BaseType = null;

		public Type BaseType
		{
			get { return _BaseType; }
			set { _BaseType = value; }
		}

		Dictionary<string, MemberInfo> _BaseTypeMembers = null;

		protected Dictionary<string, MemberInfo> BaseTypeMembers
		{
			get { return _BaseTypeMembers ?? (_BaseTypeMembers = this.CreateBaseTypeMembers()); }
		}

		private Dictionary<string, MemberInfo> CreateBaseTypeMembers()
		{
			if (this.BaseType == null)
				return null;
			Dictionary<string, MemberInfo> infos = new Dictionary<string, MemberInfo>();
			foreach (MemberInfo info in this.BaseType.GetMembers())
			{
				infos.Add(info.ToString(), info);
			}
			return infos;
		}

		bool _BridgeConstructors = false;
		/// <summary>
		/// Whether constructors (of bridged types) should be bridged as well.
		/// </summary>
		public bool BridgeConstructors
		{
			get { return _BridgeConstructors; }
			set { _BridgeConstructors = value; }
		}

		bool _AllVirtual = false;

		public bool AllVirtual
		{
			get { return _AllVirtual; }
			set { _AllVirtual = value; }
		}


		List<string> _Generics = new List<string>();

		protected List<string> Generics
		{
			get { return _Generics; }
		}

		Dictionary<string, List<Member>> _Members = new Dictionary<string, List<Member>>();

		public Dictionary<string, List<Member>> Members
		{
			get { return _Members; }
		}

		StringBuilder _Content;

		public StringBuilder Content
		{
			get { return _Content; }
			set { _Content = value; }
		}

		public string Generate()
		{
			this.Content = new StringBuilder();
			this.Members.Clear();

			Type implementation = this.Implementation;
			if (implementation == null)
				throw new ArgumentNullException();

			// preprocess ignored interfaces and add all basetypes and interfaces...
			this.IgnoreInterfaces = this.PreprocessIgnoreInterfaces(this.IgnoreInterfaces);

			if (implementation.GetGenericArguments().Length > 1)
			{
				for (int i = 0; i < implementation.GetGenericArguments().Length; i++)
				{
					if (i < this.GenericTypes.Count)
						this.Generics.Add(this.GenericTypes[i].FullName);
					else
						this.Generics.Add("T" + i);
				}
			}
			else if (implementation.GetGenericArguments().Length == 1)
				this.Generics.Add("T");

			string accessor = this.Property;
			if (accessor == null)
				accessor = "base";
			else
				accessor = "this." + accessor;

			// create class head...
			Content.Append("class ").Append(this.GenerateTypeTemplate(this.ClassName, this.Generics.Count));
			if (this.Property == null && this.BaseType == null)
			{
				if (this.Implementation != null)
					this.BaseType = this.Implementation;
				else
				{
					foreach (Type type in this.Interfaces)
					{
						if (type.IsClass)
						{
							this.BaseType = type;
							break;
						}
					}
				}
				if (this.BaseType == null && this.Property == null)
				{
					this.Property = "Value";
					accessor = "this.Value";
				}
			}
			if (this.BaseType != null)
			{
				if (!this.Interfaces.Contains(this.BaseType))
					this.Interfaces.Insert(0, this.BaseType);
				if (this.Implementation == this.BaseType && !this.IgnoreInterfaces.Contains(this.BaseType))
					this.IgnoreInterfaces.Add(this.BaseType);
			}
			//else if (this.Property == null && !this.Interfaces.Contains(this.Implementation))
			//{
			//    // derive from bridge type...
			//    if (this.Implementation != null && !this.Implementation.IsInterface && !this.Interfaces.Contains(this.Implementation))
			//        this.Interfaces.Add(this.Implementation);
			//    else
			//    {
			//        this.Property = "Value";
			//        accessor = "this.Value";
			//    }
			//    //content.Append(GenerateTypeTemplate(this.Implementation)).Append(",").AppendLine().Append("\t");
			//}

			// inherit interfaces...
			for (int i = 0, c = 0; i < this.Interfaces.Count; i++)
			{
				if (this.IgnoreInterfaces.Contains(this.Interfaces[i]))
					continue;

				if (c++ > 0)
					Content.AppendLine().Append("\t").Append(",");
				else
					this.Content.AppendLine().Append("\t").Append(": ");

				this.AppendInterface(this.Interfaces[i], Content);

				if (this.Interfaces[i].GetGenericArguments().Length != _Generics.Count)
				{
					Content.Append("//TODO: Check if generic types are passed properly.");
				}
			}

			Content.AppendLine().Append("{").AppendLine();

			if (accessor != null)
			{
				string impl = GenerateTypeTemplate(this.Implementation);
				if (this.Property != null)
				{
					// create property...
					Content.Append("\tprotected ").Append(impl).Append(" ").Append(this.Property).Append(" = new ").Append(impl).Append("();").AppendLine();
					Content.AppendLine().AppendLine();
				}

				List<Type> interfacesDone = new List<Type>();
				List<Member> done = new List<Member>();
				interfacesDone.AddRange(this.IgnoreInterfaces);
				if (this.IgnoreInterfaceMembers)
					foreach (Type ignore in this.IgnoreInterfaces)
						foreach (MemberInfo ignoreinfo in ignore.GetMembers())
							done.Add(ignoreinfo);


				if (this.BaseType != null && !interfacesDone.Contains(this.BaseType))
					interfacesDone.Add(this.BaseType);

				foreach (Type interf in this.Interfaces)
				{
					this.BridgeInterface(accessor, interf, Content, interfacesDone, done);
				}
			}
			else
				Content.Append("//ERROR: Property name was null!");


			foreach (string itype in this.Members.Keys)
			{
				this.Content.Append("\t#region ").Append(itype).AppendLine().AppendLine();
				foreach (Member member in this.Members[itype])
				{
					this.Content.Append(member.Content);
				}
				this.Content.Append("\t#endregion ").Append(itype).AppendLine().AppendLine().AppendLine();
			}
			//foreach (Member member in this.Members)
			//{
			//this.Content.Append(member.Content);
			//}


			// finish class
			Content.AppendLine().Append("}").AppendLine();

			return Content.ToString();
		}

		protected List<Type> PreprocessIgnoreInterfaces(List<Type> old)
		{
			List<Type> result = new List<Type>();

			foreach (Type type in old)
			{
				result.AddRange(zeroflag.TypeHelper.GetAllBaseTypesAndInterfaces(type));
			}

			result.AddRange(old);

			return result;
		}

		protected StringBuilder BridgeInterface(string accessor, Type itype, StringBuilder content, List<Type> interfacesDone, List<Member> done)
		{
			foreach (Type baseinterface in itype.GetInterfaces())
			{
				this.BridgeInterface(accessor, baseinterface, content, interfacesDone, done);
			}

			if (interfacesDone.Contains(itype))
				return content;

			//content.Append("\t#region " + (itype.FullName ?? (itype.Namespace + "." + itype.Name) ?? itype.ToString())).AppendLine().AppendLine();

			Type[] generics = itype.GetGenericArguments();

			for (int i = 0; i < generics.Length; i++)
				if (!this.TypeReplace.ContainsKey(generics[i]))
					this.TypeReplace.Add(generics[i], this.Generics[i]);

			List<MemberInfo> members = new List<MemberInfo>();
			if (this.BridgeConstructors)
				members.AddRange(itype.GetConstructors());
			members.AddRange(itype.GetProperties());
			members.AddRange(itype.GetEvents());
			members.AddRange(itype.GetMethods());

			//foreach (Member info in done)
			//{
			//    members.Remove(info);
			//}

			foreach (MemberInfo info in members)
			{
				if (this.IgnoreInterfaceMembers && this.IgnoreInterfaces.Contains(itype))
				{
					done.Add(info);
				}
				else if (this.IsImplemented(info) && (!this.ImplementOverrides || !this.IsOverrideable(info)))
				{
					done.Add(info);
				}
				else if (info is PropertyInfo)
				{
					this.BridgeProperty(accessor, itype, info as PropertyInfo, done);
				}
				else if (info is EventInfo)
				{
					this.BridgeEvent(accessor, itype, info as EventInfo, done);
				}
				else if (info is ConstructorInfo)
				{
					if (this.BridgeConstructors)
					{
						this.BridgeConstructor(accessor, itype, info as ConstructorInfo, done);
					}
					else
						done.Add(info);
				}
				else if (info is MethodInfo)
				{
					this.BridgeMethod(accessor, itype, info as MethodInfo, done);
				}

			}

			//content.Append("\t#endregion " + (itype.FullName ?? (itype.Namespace + "." + itype.Name) ?? itype.ToString())).AppendLine().AppendLine().AppendLine();

			interfacesDone.Add(itype);

			return content;
		}

		protected void BridgeMethod(string accessor, Type itype, MethodInfo info, List<Member> done)
		{

			string type = (itype.FullName ?? (itype.Namespace + "." + itype.Name) ?? itype.ToString());
			Member member = new Member();
			member.SetFrom(info);
			member.OwnerType = itype;
			StringBuilder content = member.Content;
			if (done.Contains(member))
				return;
			if (!this.Members.ContainsKey(type))
				this.Members.Add(type, new List<Member>());
			this.Members[type].Add(member);

			if (info.Name.StartsWith("get_") || info.Name.StartsWith("set_")
				|| info.Name.StartsWith("add_") || info.Name.StartsWith("remove_")
				|| info.IsStatic)
				return;

			this.AppendVisibility(info, content.Append("\t"));

			this.AppendTypeTemplate(info.ReturnType, content).Append(" ");
			content.Append(info.Name);

			this.AppendGenerics(info.GetGenericArguments().Length, content);

			content.Append("(");

			ParameterInfo[] parameters = info.GetParameters();
			for (int i = 0; i < parameters.Length; i++)
			{
				this.AppendParameter(parameters[i], content);
				if (i < parameters.Length - 1)
					content.Append(", ");
			}

			content.Append(")").AppendLine().Append("\t").Append("{").AppendLine().Append("\t").Append("\t");

			if (info.ReturnType != typeof(void))
				content.Append("return ");

			content.Append(accessor).Append(".").Append(info.Name).Append("(");
			for (int i = 0; i < parameters.Length; i++)
			{
				member.Parameters.Add(parameters[i].ParameterType);
				if (parameters[i].ParameterType.IsByRef)
					content.Append("ref ");
				content.Append(parameters[i].Name);
				if (i < parameters.Length - 1)
					content.Append(", ");
			}
			content.Append(");").AppendLine().Append("\t").Append("}").AppendLine().AppendLine();
			done.Add(info);

			return;
		}

		string[] DebugMemberKeys
		{
			get
			{
				string[] types = new string[this.Members.Count];
				this.Members.Keys.CopyTo(types, 0);
				return types;
			}
		}

		//bool MyEquals(MethodInfo method, object other)
		//{
		//    if (!method.IsGenericMethod)
		//    {
		//        return (other == this);
		//    } 

		//    MethodInfo info = other as RuntimeMethodInfo;
		//    RuntimeMethodHandle handle = method.GetMethodHandle().StripMethodInstantiation();
		//    RuntimeMethodHandle handle2 = info.GetMethodHandle().StripMethodInstantiation();
		//    if (handle != handle2)
		//    {
		//        return false;
		//    }
		//    if ((info == null) || !info.IsGenericMethod)
		//    {
		//        return false;
		//    }
		//    Type[] genericArguments = method.GetGenericArguments();
		//    Type[] typeArray2 = info.GetGenericArguments();
		//    if (genericArguments.Length != typeArray2.Length)
		//    {
		//        return false;
		//    }
		//    for (int i = 0; i < genericArguments.Length; i++)
		//    {
		//        if (genericArguments[i] != typeArray2[i])
		//        {
		//            return false;
		//        }
		//    }
		//    if (info.IsGenericMethod)
		//    {
		//        if (method.DeclaringType != itype)
		//        {
		//            return false;
		//        }
		//        if (method.ReflectedType != info.ReflectedType)
		//        {
		//            return false;
		//        }
		//    }

		//}

		protected void BridgeConstructor(string accessor, Type itype, ConstructorInfo info, List<Member> done)
		{
			try
			{
				//if (this.Contains(done, info))// || method.Name.StartsWith("get_") || method.Name.StartsWith("set_"))
				//    return;
			}
			catch (Exception exc)
			{
				#region BUGTRACKING
#if BUGTRACKING
				bool rethrow = false;
				try
				{
					Console.WriteLine(exc);
					new zeroflag.Serialization.XmlSerializer("failed_Info.xml").Serialize(info);
					new zeroflag.Serialization.XmlSerializer("failed_Done.xml").Serialize(done);
				}
				catch (Exception exc2)
				{
					Console.WriteLine(exc2);
				}
				foreach (MemberInfo other in done)
				{
					try
					{
						if (info.Equals(other))
							return;
					}
					catch
					{
						Console.WriteLine(info.GetType() + "[method=" + info.Name + ", type=" + info.DeclaringType + "].Equals(" + other.GetType() + "[method=" + other.Name + ", type=" + other.DeclaringType + "]) failed.");

						//MyEquals(other as MethodInfo, info);

						rethrow = true;
						try
						{
							if (other.Equals(info))
								return;
							Console.WriteLine("but " + other.GetType() + "[method=" + other.Name + ", type=" + other.DeclaringType + "].Equals(" + info.GetType() + "[method=" + info.Name + ", type=" + info.DeclaringType + "]) succeeded.");
						}
						catch
						{
							Console.WriteLine("and " + other.GetType() + "[method=" + other.Name + ", type=" + other.DeclaringType + "].Equals(" + info.GetType() + "[method=" + info.Name + ", type=" + info.DeclaringType + "]) failed as well.");
						}
						break;
					}
					try
					{
						if (other.Equals(info))
							return;
					}
					catch
					{
						Console.WriteLine(other.GetType() + "[method=" + other.Name + ", type=" + other.DeclaringType + "].Equals(" + info.GetType() + "[method=" + info.Name + ", type=" + info.DeclaringType + "]) failed.");
						try
						{
							rethrow = true;
							if (info.Equals(other))
								return;
							Console.WriteLine("but " + info.GetType() + "[method=" + info.Name + ", type=" + info.DeclaringType + "].Equals(" + other.GetType() + "[method=" + other.Name + ", type=" + other.DeclaringType + "]) succeeded.");
						}
						catch
						{
							Console.WriteLine("and " + info.GetType() + "[method=" + info.Name + ", type=" + info.DeclaringType + "].Equals(" + other.GetType() + "[method=" + other.Name + ", type=" + other.DeclaringType + "]) failed as well.");
						}
						break;
					}
				}
				if (rethrow)
					throw;
#endif
				#endregion BUGTRACKING

				//if (this.Contains(done, info))
				//    return;

			}

			string type = (itype.FullName ?? (itype.Namespace + "." + itype.Name) ?? itype.ToString());
			Member member = new Member();
			member.SetFrom(info);
			member.OwnerType = itype;
			StringBuilder content = member.Content;
			if (done.Contains(member))
				return;
			if (!this.Members.ContainsKey(type))
				this.Members.Add(type, new List<Member>());
			this.Members[type].Add(member);


			// public 
			this.AppendVisibility(info, content.Append("\t"));
			// public Name
			content.Append(this.ClassName);

			// public Name(
			content.Append("(");

			ParameterInfo[] parameters = info.GetParameters();
			for (int i = 0; i < parameters.Length; i++)
			// public Name(type param,
			{
				this.AppendParameter(parameters[i], content);
				if (i < parameters.Length - 1)
					content.Append(", ");
			}

			// public Name(type param) {
			content.Append(")").AppendLine().Append("\t").Append("{").AppendLine().Append("\t").Append("\t");

			//if (method.ReturnType != typeof(void))
			//    content.Append("return ");

			// public Name(type param) { this.Name
			content.Append(accessor);
			// public Name(type param) { this.Name = new
			content.Append(" = new ");

			// public Name(type param) { this.Name = new Target
			this.AppendTypeTemplate(itype, content);

			// public Name(type param) { this.Name = new Target(
			content.Append("(");
			// public Name(type param) { this.Name = new Target(param
			for (int i = 0; i < parameters.Length; i++)
			{
				member.Parameters.Add(parameters[i].ParameterType);
				if (parameters[i].ParameterType.IsByRef)
					content.Append("ref ");
				content.Append(parameters[i].Name);
				if (i < parameters.Length - 1)
					content.Append(", ");
			}
			// public Name(type param) { this.Name = new Target(param); }
			content.Append(");").AppendLine().Append("\t").Append("}").AppendLine().AppendLine();
			done.Add(info);
		}

		//private bool Contains(List<Member> done, MemberInfo info)
		//{
		//    MemberInfo y = info;
		//    return done.Find(delegate(MemberInfo x)
		//    {
		//        try
		//        {
		//            if (object.ReferenceEquals(x, y))
		//                return true;
		//            if (object.ReferenceEquals(x, null) || object.ReferenceEquals(null, y))
		//                return false;
		//            return x.Equals(y);
		//        }
		//        catch
		//        {
		//            return y.Equals(x);
		//        }
		//    }) != null;
		//}


		protected void BridgeEvent(string accessor, Type itype, EventInfo info, List<Member> done)
		{
			//if (done.Contains(info))
			//    return;

			string type = (itype.FullName ?? (itype.Namespace + "." + itype.Name) ?? itype.ToString());
			Member member = new Member();
			member.SetFrom(info);
			member.OwnerType = itype;
			StringBuilder content = member.Content;
			if (done.Contains(member))
				return;
			if (!this.Members.ContainsKey(type))
				this.Members.Add(type, new List<Member>());
			this.Members[type].Add(member);

			content.Append("\tpublic event ");
			this.AppendTypeTemplate(info.EventHandlerType, content).Append(" ");

			content.Append(info.Name);

			content.AppendLine().Append("\t").Append("{").AppendLine().Append("\t");
			foreach (MethodInfo access in new MethodInfo[] { info.GetAddMethod(false), info.GetRemoveMethod(false) })
			{
				if (access == null)
					continue;

				Append pre, post;

				content.Append("\t");

				if (access.Name.StartsWith("add_"))
				{
					content.Append("add");
					pre = delegate { };
					post = delegate { content.Append(" += value"); };
				}
				else if (access.Name.StartsWith("remove_"))
				{
					content.Append("remove");
					pre = delegate { };
					post = delegate { content.Append(" -= value"); };
				}
				else
				{
					content.Append("//ERROR: accessor ").Append(access.Name).Append(" not supported! ");
					pre = delegate { };
					post = delegate { };
				}

				content.Append(" { ");

				pre();


				content.Append(accessor);

				content.Append(".").Append(info.Name);

				post();

				content.Append("; }").AppendLine().Append("\t");
			}
			content.Append("}").AppendLine().AppendLine();

			done.Add(info);
		}

		protected void BridgeProperty(string accessor, Type itype, PropertyInfo info, List<Member> done)
		{
			try
			{
				//if (done.Contains(info))
				//    return;
			}
			catch
			{
				//if (this.Contains(done, info))
				//    return;
			}


			string type = (itype.FullName ?? (itype.Namespace + "." + itype.Name) ?? itype.ToString());
			Member member = new Member();
			member.SetFrom(info);
			member.OwnerType = itype;
			StringBuilder content = member.Content;
			if (done.Contains(member))
				return;
			if (!this.Members.ContainsKey(type))
				this.Members.Add(type, new List<Member>());
			this.Members[type].Add(member);

			this.AppendVisibility(info, content.Append("\t"));
			this.AppendTypeTemplate(info.PropertyType, content).Append(" ");

			ParameterInfo[] parameters = null;
			if (info.Name == "Item")
			{
				content.Append("this[");
				parameters = info.GetIndexParameters();

				for (int i = 0; i < parameters.Length; i++)
				{
					this.AppendParameter(parameters[i], content);
					if (i < parameters.Length - 1)
						content.Append(", ");
				}
				content.Append("]");
			}
			else
			{
				content.Append(info.Name);
			}


			content.AppendLine().Append("\t").Append("{").AppendLine().Append("\t");
			foreach (MethodInfo access in info.GetAccessors())
			{
				Append pre, post;

				content.Append("\t");

				if (access.Name.StartsWith("get_"))
				{
					content.Append("get");
					pre = delegate
					{
						content.Append("return ");
					};
					post = delegate { };
				}
				else if (access.Name.StartsWith("set_"))
				{
					content.Append("set");
					pre = delegate { };
					post = delegate
					{
						content.Append(" = value");
					};
				}
				else
				{
					content.Append("//ERROR: accessor ").Append(access.Name).Append(" not supported! ");
					pre = delegate { };
					post = delegate { };
				}

				content.Append(" { ");

				pre();


				content.Append(accessor);
				if (info.Name == "Item")
				{
					content.Append("[");
					for (int i = 0; i < parameters.Length; i++)
					{
						content.Append(parameters[i].Name);
						if (i < parameters.Length - 1)
							content.Append(", ");
					}
					content.Append("]");
				}
				else
				{
					content.Append(".").Append(info.Name);
				}

				post();

				content.Append("; }").AppendLine().Append("\t");
			}
			content.Append("}").AppendLine().AppendLine();

			done.Add(info);
		}

		protected StringBuilder AppendVisibility(MethodBase info, StringBuilder content)
		{
			if (info.IsPublic)
				content.Append("public ");
			else if (info.IsPrivate)
				content.Append("private ");
			else
				content.Append("protected ");

			if (info.IsStatic)
				content.Append("static ");

			if (info.IsAbstract && this.BaseType != null)
				//content.Append("abstract ");
				content.Append("override ");
			else if (this.AllVirtual)
				content.Append("virtual ");
			else if (info.IsVirtual)
			{
				if (this.IsOverrideable(info))
					content.Append("override ");
				else
					content.Append("virtual ");
			}

			return content;
		}

		protected StringBuilder AppendVisibility(PropertyInfo info, StringBuilder content)
		{
			if (info.CanRead && info.GetGetMethod(true).IsPublic || info.CanWrite && info.GetSetMethod(true).IsPublic)
				content.Append("public ");
			else if (info.CanRead && info.GetGetMethod(true).IsPrivate && (!info.CanWrite || info.GetSetMethod(true).IsPrivate))
				content.Append("private ");
			else
				content.Append("protected ");

			if (info.CanRead && info.GetGetMethod(true).IsStatic || info.CanWrite && info.GetSetMethod(true).IsStatic)
				content.Append("static ");

			if (this.BaseType != null && (info.CanRead && info.GetGetMethod(true).IsAbstract || info.CanWrite && info.GetSetMethod(true).IsAbstract))
				content.Append("override ");
			else if (this.AllVirtual)
				content.Append("virtual ");
			else if ((info.CanRead && info.GetGetMethod(true).IsVirtual || info.CanWrite && info.GetSetMethod(true).IsVirtual))
			{
				if (this.IsOverrideable(info))
					content.Append("override ");
				else
					content.Append("virtual ");
			}

			return content;
		}

		protected bool IsOverrideable(MemberInfo info)
		{
			return this.IsImplemented(info)
				&&
				(// virtual?
				info is MethodInfo && ((MethodInfo)info).IsVirtual ||
				info is PropertyInfo &&
				(
					((PropertyInfo)info).CanRead && ((PropertyInfo)info).GetGetMethod().IsVirtual ||
					((PropertyInfo)info).CanWrite && ((PropertyInfo)info).GetSetMethod().IsVirtual
				) ||
				info is EventInfo &&
				(
					((EventInfo)info).GetAddMethod() != null && ((EventInfo)info).GetAddMethod().IsVirtual ||
					((EventInfo)info).GetRemoveMethod() != null && ((EventInfo)info).GetRemoveMethod().IsVirtual
				)
				)
				;
		}

		protected bool IsImplemented(MemberInfo info)
		{
			return this.BaseType != null && this.BaseTypeMembers.ContainsKey(info.ToString());
		}

		protected StringBuilder AppendParameter(ParameterInfo param, StringBuilder content)
		{
			content.Append(this[param.ParameterType]);
			this.AppendGenerics(param.ParameterType.GetGenericArguments().Length, content);
			content.Append(" ").Append(param.Name);
			return content;
		}

		delegate void Append();


		protected StringBuilder AppendInterface(Type itype, StringBuilder content)
		{
			content.Append(itype.FullName.Split('`')[0]);

			this.AppendGenerics(itype.GetGenericArguments().Length, content);

			return content;
		}

		protected StringBuilder AppendGenerics(int generics, StringBuilder content)
		{
			if (generics > 0)
			{
				content.Append("<");
				int last = Math.Max(generics, this.Generics.Count);
				for (int i = 0; i < last; i++)
				{
					if (i < this.Generics.Count)
						content.Append(this.Generics[i]);
					else
						content.Append("T").Append(i + 1);
					if (i < last - 1)
						content.Append(", ");
				}
				content.Append(">");
			}
			return content;
		}


		protected string GenerateTypeTemplate(Type type)
		{
			return this.GenerateTypeTemplate(this[type], type.GetGenericArguments().Length);
		}

		protected string GenerateTypeTemplate(string name, int generics)
		{
			return this.AppendTypeTemplate(name, generics, new StringBuilder()).ToString();
		}
		protected StringBuilder AppendTypeTemplate(Type type, StringBuilder content)
		{
			return this.AppendTypeTemplate(this[type], type.GetGenericArguments().Length, content);
		}

		protected StringBuilder AppendTypeTemplate(string name, int generics, StringBuilder content)
		{
			if (this.Generics.Count > 0)
			{
				content.Append(name.Replace('+', '.'));

				this.AppendGenerics(generics, content);
			}
			else
				content.Append(name);

			return content;
		}

		Dictionary<Type, string> _TypeReplace = null;

		public Dictionary<Type, string> TypeReplace
		{
			get
			{
				if (_TypeReplace == null)
				{
					_TypeReplace = new Dictionary<Type, string>();
					_TypeReplace.Add(typeof(void), "void");
					_TypeReplace.Add(typeof(bool), "bool");
					_TypeReplace.Add(typeof(int), "int");
					_TypeReplace.Add(typeof(uint), "uint");
					_TypeReplace.Add(typeof(short), "short");
					_TypeReplace.Add(typeof(ushort), "ushort");
					_TypeReplace.Add(typeof(long), "long");
					_TypeReplace.Add(typeof(ulong), "ulong");
					_TypeReplace.Add(typeof(byte), "byte");
					_TypeReplace.Add(typeof(sbyte), "sbyte");
					_TypeReplace.Add(typeof(float), "float");
					_TypeReplace.Add(typeof(double), "double");
					_TypeReplace.Add(typeof(decimal), "decimal");
				}
				return _TypeReplace;
			}
			set { _TypeReplace = value; }
		}

		protected string this[Type type]
		{
			get
			{
				//return (this.TypeReplace.ContainsKey(type) ? this.TypeReplace[type] : (type.FullName ?? ((!type.IsGenericParameter && type.Namespace != null ? (type.Namespace + ".") : "") + type.Name))).Split('`')[0];
				if (this.TypeReplace.ContainsKey(type))
				{
					return this.TypeReplace[type];
				}
				else
				{
					string name = type.FullName;
					if (name == null)
					{
						name = ((!type.IsGenericParameter && type.Namespace != null ? (type.Namespace + ".") : "") + type.Name);
					}
					if (type.IsByRef)
					{
						name = "ref " + name.TrimEnd('&');
					}
					if (type.IsArray)
					{
						name = this[type.GetElementType()] + "[]";
					}
					return name.Split('`')[0].Replace('+', '.');
				}
			}
		}

	}
}
