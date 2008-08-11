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

namespace zeroflag.Serialization.Descriptors
{
	/// <summary>
	/// Descriptor class used to describe an object or a type so it can be used for serialization.
	/// </summary>
	public abstract class Descriptor
	{
		public delegate T GetHandler<T>();
		//public delegate void SetHandler<T>(T value);

		private Type _Type;

		public virtual Type Type
		{
			get { return _Type; }
			set { _Type = value; }
		}

		#region OwnerInstance

		//private object _OwnerInstance = default(object);

		//public object OwnerInstance
		//{
		//    get { return _OwnerInstance; }
		//    set
		//    {
		//        if (_OwnerInstance != value)
		//        {
		//            _OwnerInstance = value;
		//        }
		//    }
		//}
		#endregion OwnerInstance

		#region Owner
#if OWNER
		private Descriptor _Owner;

		public Descriptor Owner
		{
			get
			{
				//if (!this.IsCircularReference(new List<Descriptor>()))
				return _Owner;
				//return null;
			}
			set
			{
				if (_Owner != value && value != this)// && !IsCircularReference(new List<Descriptor>() { value }))
				{
					if (_Owner != null)
						_Owner.Inner.Remove(this);
					_Owner = value;
					if (value != null)
						value.Inner.Add(this);
				}
			}
		}
#if ROOT
		public Descriptor Root
		{
			get { return this.GetRoot(new List<Descriptor>()); }
		}

		protected Descriptor GetRoot(List<Descriptor> trace)
		{
			if (this.Owner == null || trace.Contains(this))
			{
				return this;
			}
			trace.Add(this);
			return this.Owner.GetRoot(trace);
		}

		protected bool IsCircularReference(List<Descriptor> trace)
		{
			if (trace.Contains(this) || trace.Count > 100)
			{
				Console.WriteLine("CIRCULAR REFERENCE!");
				return true;
			}
			trace.Add(this);
			if (this._Owner == null)
				return false;
			return this._Owner.IsCircularReference(trace);
		}
#endif
#endif
		#endregion Owner

		#region Context

		Context _Context;

		public Context Context
		{
			get { return _Context; }
			set { _Context = value; }
		}

		#endregion


		#region Value

		private object _Value = null;

		public object Value
		{
			get { return _Value ?? (_Value = DefaultValue); }
			set
			{
				if (_Value != value)
				{
					_Value = value;
					if (value != null)
						this.IsNull = false;
				}
			}
		}

		protected abstract object DefaultValue { get; }
		#endregion Value

		public virtual bool NeedsWriteAccess
		{
			get { return true; }
		}

		bool _IsNull = true;

		public bool IsNull
		{
			get { return _IsNull; }
			set { _IsNull = value; }
		}

		private string _Name;

		public string Name
		{
			get { return _Name; }
			set { _Name = value; }
		}

		int? _Id = null;
		public virtual int? Id
		{
			get { return _Id; }
			set { _Id = value; }
		}

		bool _IsReferenced = false;

		public bool IsReferenced
		{
			get { return _IsReferenced; }
			set { _IsReferenced = value; }
		}

		//public abstract object GetValue();
		//public abstract void SetValue(object value);

		List<Descriptor> _Inner = new List<Descriptor>();

		public List<Descriptor> Inner
		{
			get { return _Inner; }
		}


		#region Parse

		//public Descriptor Parse(System.Reflection.PropertyInfo info, Descriptor owner)
		//{
		//    this.Owner = owner;
		//    return this.Parse(info);
		//}

		//public virtual Descriptor Parse(System.Reflection.PropertyInfo info)
		//{
		//    return this.Parse(info.Name, info.PropertyType);
		//}
		//public virtual Descriptor Parse(string name, Type type, Descriptor owner)
		//{
		//    this.Owner = owner;
		//    return this.Parse(name, type);
		//}
		//public Descriptor Parse(string name, Type type)
		//{
		//    if (type == null)
		//        type = this.Type;
		//    this.Type = this.Context.GetUsableType(type);

		//    //if (name == null)
		//    //    name = type.Name;
		//    this.Name = name ?? this.Name;

		//    this.Parse();
		//    return this;
		//}

		protected abstract void Parse();

		public virtual Descriptor Parse(string name, Type type, object value
			//, object ownerInstance
#if OWNER
			, Descriptor owner
#endif
)
		{
			this.Name = name;
			this.Type = type;
			this.Value = value;
#if OWNER
			this.Owner = owner;
#endif
			//this.OwnerInstance = ownerInstance;
			this.Parse();
			return this;
		}
		#endregion Parse

		#region Generate
		bool _IsParsed = false;
		public void GenerateParse()
		{
			if (_IsParsed)
				return;
			_IsParsed = true;

			if (this.Id != null && this.Context.CreatedInstances.ContainsKey(this.Id.Value))// && this != this.Generated[this.Id.Value])
			{
				if (this == this.Context.CreatedInstances[this.Id.Value])
					CWL("Link self!");
				CWL("Link(name='" + this.Name + "', type='" + this.Type + "', isnull='" + this.IsNull + "', id='" + this.Id + "', value='" + this.Value + "', children='" + this.Inner.Count + "')");

				Descriptor other = this.Context.CreatedInstances[this.Id.Value];
				CWL("  To(name='" + other.Name + "', type='" + other.Type + "', isnull='" + other.IsNull + "', id='" + other.Id + "', value='" + other.Value + "', children='" + other.Inner.Count + "')");
				this.Type = other.Type;
				this.Value = other.Value;
				this.IsNull = other.IsNull;
				this.Inner.AddRange(other.Inner);
				CWL("Result(name='" + this.Name + "', type='" + this.Type + "', isnull='" + this.IsNull + "', id='" + this.Id + "', value='" + this.Value + "', children='" + this.Inner.Count + "')");
			}
			else
			{
				if (this.Value == null && !this.IsNull)
				{
					this.DoCreateInstance();
				}
				if (this.Id != null)
				{
					if (!this.Context.CreatedInstances.ContainsKey(this.Id.Value))
					{
						CWL("Reference(name='" + this.Name + "', type='" + this.Type + "', isnull='" + this.IsNull + "', id='" + this.Id + "', value='" + this.Value + "', children='" + this.Inner.Count + "')");
						this.Context.CreatedInstances.Add(this.Id.Value, this);
					}
					else
						CWL("Reference already exists for " + this.Id);
				}
			}
			this.ApplyAttributes();
		}

		protected virtual void ApplyAttributes()
		{
			//TODO: come up with a concept for attributes... thinking > hacking.
		}

		bool _IsGenerated = false;
		public void GenerateCreate()
		{
			if (_IsGenerated)
				return;
			_IsGenerated = true;
			if (this.Value == null)
			{
				if (!this.IsNull)
					this.Value = this.DoCreateInstance();
				else
					this.Value = null;
			}
		}

		bool _IsLinked = false;
		public virtual object GenerateLink()
		{
			if (_IsLinked)
				return this.Value;
			_IsLinked = true;

			if (this.Value == null)
			{
				this.GenerateParse();
				this.GenerateCreate();
			}

			//if (this.Value != null)
			{
				foreach (Descriptor sub in this.Inner)
				{
					if (sub.Name == null || sub.IsNull)
					    continue;
					System.Reflection.PropertyInfo prop = null;
					try
					{
						prop = this.Type.GetProperty(sub.Name);
					}
					catch (System.Reflection.AmbiguousMatchException exc)
					{
						this.Context.Exceptions.Add(new ExceptionTrace(exc, this, this.Type, this.Value));
						// manual search...
						foreach (var pr in this.Type.GetProperties())
						{
							if (pr != null && pr.GetIndexParameters().Length == 0 && pr.Name == sub.Name && pr.PropertyType.IsAssignableFrom(sub.Type))
								prop = pr;
						}
					}
					finally
					{ }

					if (prop != null)
					{
						if (prop.CanWrite)
						{
							//sub.SetValue(this.Value, prop);
							object value = sub.GenerateLink();

							if (value != null && this.Value != null)
							{
								try
								{
									prop.SetValue( this.Value, value, new object[] { } );
								}
								catch ( Exception exc )
								{
									Console.WriteLine( exc );
								}
							}
						}
					}
				}
			}
			return this.Value;
		}

		public virtual object DoCreateInstance()
		{
			try
			{
				return this.Value = (this.IsNull ? null : (this.Value ?? (this.Value = (this.IsNull ? null : TypeHelper.CreateInstance(this.Type)))));
			}
			catch (Exception exc)
			{
				CWL(this + ".DoCreateInstance() failed:\n" + exc);
				return this.Value;
			}
		}
		#endregion Generate

		public System.Reflection.PropertyInfo Property(Type type)
		{
			var props = this.GetProperties(this.Type);
			foreach (var prop in props)
			{
				if (prop != null && (prop.PropertyType == type || prop.PropertyType.IsAssignableFrom(type)))
					return prop;
			}
			return null;
		}

		public System.Reflection.PropertyInfo Property(string property)
		{
			return this.Property(property, false);
		}

		public System.Reflection.PropertyInfo Property(string property, bool byType)
		{
			var props = this.GetPropertyNames(this.Type);
			// search by name, case sensitive (fast)...
			if (props.ContainsKey(property))
				return props[property];
			var names = new List<string>(props.Keys);
			// search by name, case insensitive...
			string key = names.Find(n => n.ToLower() == property.ToLower());
			if (key != null)
				return props[key];

			if (!byType)
				return null;

			// get all property types...
			Dictionary<Type, string> types = new Dictionary<Type, string>();
			foreach (string name in props.Keys)
			{
				if (!types.ContainsKey(props[name].PropertyType))
					types.Add(props[name].PropertyType, name);
			}

			// search types, case insensitive...
			List<Type> typeSearch = new List<Type>(types.Keys);
			Type type = typeSearch.Find(t => t.Name != null && t.Name.ToLower() == property.ToLower());
			if (type != null && types.ContainsKey(type) && props.ContainsKey(types[type]))
				return props[types[type]];

			type = typeSearch.Find(t => t.Name != null && (t.Name.ToLower().Contains(property.ToLower()) || property.ToLower().Contains(t.Name.ToLower())));
			if (type != null && types.ContainsKey(type) && props.ContainsKey(types[type]))
				return props[types[type]];

			return null;
		}

		public Dictionary<string, System.Reflection.PropertyInfo> GetPropertyNames(Type type)
		{
			//List<System.Reflection.PropertyInfo> props = new List<System.Reflection.PropertyInfo>();
			Dictionary<string, System.Reflection.PropertyInfo> props = new Dictionary<string, System.Reflection.PropertyInfo>();
			do
			{
				//break;
				//props.AddRange(type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.));
				foreach (var prop in type.GetProperties())
					if (!props.ContainsKey(prop.Name))
						props.Add(prop.Name, prop);
			}
			while ((type = type.BaseType) != null);
			return props;
		}
		public List<System.Reflection.PropertyInfo> GetProperties(Type type)
		{
			//List<System.Reflection.PropertyInfo> props = new List<System.Reflection.PropertyInfo>();
			Dictionary<string, System.Reflection.PropertyInfo> props = new Dictionary<string, System.Reflection.PropertyInfo>();
			do
			{
				//break;
				//props.AddRange(type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.));
				foreach (var prop in type.GetProperties())
					if (!props.ContainsKey(prop.Name))
						props.Add(prop.Name, prop);
			}
			while ((type = type.BaseType) != null);
			return new List<System.Reflection.PropertyInfo>(props.Values);
		}

		public override string ToString()
		{
			//return this.GetType().Name + "[" + this.Name + ", " + this.Type + ", " + this.Id + "] -> " + this.Owner;
			return this.ToString(new StringBuilder()).ToString();
		}

		public StringBuilder ToString(StringBuilder builder)
		{
			return builder.Append(this.GetType().Name).Append("[").Append(this.Name).Append(", ").Append(this.Type).Append(", ").Append(this.Id).Append("]");// -> { ");
		}

		public StringBuilder ToStringTree()
		{
			return this.ToStringTree(new StringBuilder("\n"), 0).AppendLine();
		}
		public StringBuilder ToStringTree(StringBuilder builder, int depth)
		{
			if (depth > 20)
				return builder;
			builder.Append(' ', depth * 2).Append(this.Name ?? "<noname>").Append(" (").Append(this.Type).Append(",").Append(this.Id).Append(") := ").Append(this.Value ?? (object)"<null>").AppendLine();
			depth++;
			foreach (Descriptor child in this.Inner)
				child.ToStringTree(builder, depth);
			return builder;
		}

		[System.Diagnostics.Conditional("VERBOSE")]
		static internal void CWL(object value)
		{
			Console.WriteLine(value);
		}

		public Type DescritorType
		{
			get { return this.GetType(); }
		}
	}
}
