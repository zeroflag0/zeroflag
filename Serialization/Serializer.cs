using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Serialization
{
	public abstract class Serializer
	{
		private string m_FileName;
		private Serializer m_Parent;

		/// <summary>
		/// This serializer's parent serializer.
		/// </summary>
		protected Serializer Parent
		{
			get { return m_Parent; }
			set { m_Parent = value; }
		}

		public string FileName
		{
			get { return m_FileName ?? this.Parent.FileName; }
			set { m_FileName = value; }
		}

		/// <summary>
		/// This serializer's root serializer.
		/// </summary>
		protected Serializer Root
		{
			get
			{
				return this.Parent != null ? this.Parent.Root : this;
			}
		}
	
		public Serializer()
		{
		}

		public Serializer(string fileName)
		{
			this.FileName = fileName;
		}

		public Serializer(Serializer parent)
		{
			this.Parent = parent;
		}

		public object Deserialize(Type type)
		{
			return Deserialize(new ObjectDescription(type));
		}

		public abstract object Deserialize(ObjectDescription type);

		public void Serialize(object value)
		{
			ObjectDescription desc = new ObjectDescription(value);
			//desc.Parse(value);
			this.Serialize(desc);
		}
		protected abstract void Serialize(ObjectDescription value);

		/// <summary>
		/// Creates a child-serializer.
		/// </summary>
		protected abstract Serializer CreateChild();



	}
}
