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

namespace zeroflag.Serialization
{
	public abstract class Serializer
	{
		private string _FileName;
		private Serializer _Parent;

		/// <summary>
		/// This serializer's parent serializer.
		/// </summary>
		protected Serializer Parent
		{
			get { return _Parent; }
			set { _Parent = value; }
		}

		public string FileName
		{
			get { return _FileName ?? this.Parent.FileName; }
			set { _FileName = value; }
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

		Descriptors.Descriptor _Descriptor;

		public Descriptors.Descriptor Descriptor
		{
			get { return _Descriptor; }
			set { _Descriptor = value; }
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

		public void Serialize(object value)
		{
			this.Descriptor = Descriptors.Descriptor.DoParse(value);
			//desc.Parse(value);
			this.Serialize(this.Descriptor);
		}
		public abstract void Serialize(Descriptors.Descriptor value);

		//public object Deserialize(Type type)
		//{
		//    return Deserialize(new ObjectDescription(type));
		//}

		//public abstract object Deserialize(ObjectDescription type);


		///// <summary>
		///// Creates a child-serializer.
		///// </summary>
		//protected abstract Serializer CreateChild();

		public T Deserialize<T>()
		{
			return (T)this.Deserialize(typeof(T));
		}

		public object Deserialize(Type type)
		{
			Descriptors.Descriptor desc = Descriptors.Descriptor.DoParse(type);

			return this.Deserialize(desc);
		}

		public abstract object Deserialize(Descriptors.Descriptor desc);
	}
}
