using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace BridgeInterface
{
	public class Implementor
	{
		private List<Type> _Interfaces = new List<Type>();
		private Type _Implementation;
		private string _Property;
		private string _ClassName;
		private string _SourceCode;
		private List<Type> _GenericTypes = new List<Type>();

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


		bool _BridgeConstructors = false;

		public bool BridgeConstructors
		{
			get { return _BridgeConstructors; }
			set { _BridgeConstructors = value; }
		}

		List<string> _Generics = new List<string>();

		protected List<string> Generics
		{
			get { return _Generics; }
		}

		public string Generate()
		{
			StringBuilder content = new StringBuilder();

			Type implementation = this.Implementation;
			if (implementation == null)
				throw new ArgumentNullException();

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

			string prop = this.Property;
			string accessor = null;
			if (prop == null)
				accessor = "base";
			else
				accessor = "this." + prop;

			// create class head...
			content.Append("class ").Append(this.GenerateTypeTemplate(this.ClassName, this.Generics.Count)).AppendLine().Append("\t").Append(": ");
			//if (prop == null)
			//{
			//    // derive from bridge type...
			//    content.Append(GenerateTypeTemplate(this.Implementation)).Append(",").AppendLine().Append("\t");
			//}
			// inherit interfaces...
			for (int i = 0; i < this.Interfaces.Count; i++)
			{
				this.AppendInterface(this.Interfaces[i], content);

				if (i < this.Interfaces.Count - 1)
					content.Append(",");
				if (this.Interfaces[i].GetGenericArguments().Length != _Generics.Count)
				{
					content.Append("//TODO: Check if generic types are passed properly.");
				}

				content.AppendLine();
				if (i < this.Interfaces.Count - 1)
					content.Append("\t");
			}

			content.Append("{").AppendLine().Append("\t");

			if (prop != null)
			{

				string impl = GenerateTypeTemplate(this.Implementation);
				// create property...
				content.Append("protected ").Append(impl).Append(" ").Append(this.Property).Append(" = new ").Append(impl).Append("();").AppendLine().Append("\t");

				content.AppendLine().AppendLine();
				List<Type> interfacesDone = new List<Type>();
				List<MethodBase> methodsDone = new List<MethodBase>();
				List<ConstructorInfo> constructorsDone = new List<ConstructorInfo>();
				List<PropertyInfo> propertiesDone = new List<PropertyInfo>();
				foreach (Type interf in this.Interfaces)
				{
					this.BridgeInterface(accessor, interf, content, interfacesDone, methodsDone, propertiesDone, constructorsDone);
				}
			}
			else
				content.Append("//ERROR: Property name was null!");

			// finish class
			content.AppendLine().Append("}").AppendLine();

			return content.ToString();
		}

		protected StringBuilder BridgeInterface(string accessor, Type itype, StringBuilder content, List<Type> interfacesDone, List<MethodBase> methodsDone, List<PropertyInfo> propertiesDone, List<ConstructorInfo> constructorsDone)
		{
			foreach (Type baseinterface in itype.GetInterfaces())
			{
				this.BridgeInterface(accessor, baseinterface, content, interfacesDone, methodsDone, propertiesDone, constructorsDone);
			}

			if (interfacesDone.Contains(itype))
				return content;

			content.Append("#region " + (itype.FullName ?? (itype.Namespace + "." + itype.Name) ?? itype.ToString())).AppendLine().AppendLine();

			Type[] generics = itype.GetGenericArguments();

			for (int i = 0; i < generics.Length; i++)
				if (!this.TypeReplace.ContainsKey(generics[i]))
					this.TypeReplace.Add(generics[i], this.Generics[i]);

			foreach (MethodInfo method in itype.GetMethods())
			{
				this.BridgeMethod(accessor, method, content, methodsDone);
			}

			if (this.BridgeConstructors)
			{
				foreach (ConstructorInfo method in itype.GetConstructors())
				{
					this.BridgeConstructor(accessor, method, content, constructorsDone);
				}
			}

			foreach (PropertyInfo property in itype.GetProperties())
			{
				this.BridgeProperty(accessor, property, content, propertiesDone);
			}

			content.Append("#endregion " + (itype.FullName ?? (itype.Namespace + "." + itype.Name) ?? itype.ToString())).AppendLine().AppendLine().AppendLine();

			interfacesDone.Add(itype);

			return content;
		}

		protected StringBuilder BridgeMethod(string accessor, MethodInfo method, StringBuilder content, List<MethodBase> methodsDone)
		{
			if (methodsDone.Contains(method) || method.Name.StartsWith("get_") || method.Name.StartsWith("set_"))
				return content;

			content.Append("\tpublic ");
			this.AppendTypeTemplate(method.ReturnType, content).Append(" ");
			content.Append(method.Name);

			this.AppendGenerics(method.GetGenericArguments().Length, content);

			content.Append("(");

			ParameterInfo[] parameters = method.GetParameters();
			for (int i = 0; i < parameters.Length; i++)
			{
				this.AppendParameter(parameters[i], content);
				if (i < parameters.Length - 1)
					content.Append(", ");
			}

			content.Append(")").AppendLine().Append("\t").Append("{").AppendLine().Append("\t").Append("\t");

			if (method.ReturnType != typeof(void))
				content.Append("return ");

			content.Append(accessor).Append(".").Append(method.Name).Append("(");
			for (int i = 0; i < parameters.Length; i++)
			{
				content.Append(parameters[i].Name);
				if (i < parameters.Length - 1)
					content.Append(", ");
			}
			content.Append(");").AppendLine().Append("\t").Append("}").AppendLine().AppendLine();
			methodsDone.Add(method);

			return content;
		}

		protected StringBuilder BridgeConstructor(string accessor, ConstructorInfo method, StringBuilder content, List<ConstructorInfo> constructorsDone)
		{
			if (constructorsDone.Contains(method))// || method.Name.StartsWith("get_") || method.Name.StartsWith("set_"))
				return content;


			// public 
			content.Append("\tpublic ");
			// public Name
			content.Append(this.ClassName);

			// public Name(
			content.Append("(");

			ParameterInfo[] parameters = method.GetParameters();
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
			this.AppendTypeTemplate(method.DeclaringType, content);

			// public Name(type param) { this.Name = new Target(
			content.Append("(");
			// public Name(type param) { this.Name = new Target(param
			for (int i = 0; i < parameters.Length; i++)
			{
				content.Append(parameters[i].Name);
				if (i < parameters.Length - 1)
					content.Append(", ");
			}
			// public Name(type param) { this.Name = new Target(param); }
			content.Append(");").AppendLine().Append("\t").Append("}").AppendLine().AppendLine();
			constructorsDone.Add(method);

			return content;
		}

		protected StringBuilder BridgeProperty(string accessor, PropertyInfo property, StringBuilder content, List<PropertyInfo> propertiesDone)
		{
			if (propertiesDone.Contains(property))
				return content;

			content.Append("\tpublic ");
			this.AppendTypeTemplate(property.PropertyType, content).Append(" ");

			ParameterInfo[] parameters = null;
			if (property.Name == "Item")
			{
				content.Append("this[");
				parameters = property.GetIndexParameters();

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
				content.Append(property.Name);
			}


			content.AppendLine().Append("\t").Append("{").AppendLine().Append("\t");
			foreach (MethodInfo access in property.GetAccessors())
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
				if (property.Name == "Item")
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
					content.Append(".").Append(property.Name);
				}

				post();

				content.Append("; }").AppendLine().Append("\t");
			}
			content.Append("}").AppendLine().AppendLine();

			propertiesDone.Add(property);

			return content;
		}

		protected void AppendParameter(ParameterInfo param, StringBuilder content)
		{
			if (param.ParameterType.IsArray)
				content.Append(this[param.ParameterType.GetElementType()]).Append("[]");
			else
				content.Append(this[param.ParameterType]);
			this.AppendGenerics(param.ParameterType.GetGenericArguments().Length, content);
			content.Append(" ").Append(param.Name);
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
				content.Append(name);

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
				return (this.TypeReplace.ContainsKey(type) ? this.TypeReplace[type] : (type.FullName ?? ((!type.IsGenericParameter && type.Namespace != null ? (type.Namespace + ".") : "") + type.Name))).Split('`')[0];
			}
		}
	}
}
