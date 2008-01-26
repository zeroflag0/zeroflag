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
using zeroflag.Serialization;

namespace Test
{
	public class A
	{
		List<A> _Children = new List<A>();

		public List<A> Children
		{
			get { return _Children; }
			set { _Children = value; }
		}

		string _Name = null;

		public string Name
		{
			get { return _Name; }
			set { _Name = value; }
		}

		public A(string name)
		{
			this.Name = name;
		}

		public A(params A[] children)
		{
			this.Children.AddRange(children);
		}

		public override string ToString()
		{
			return this.Name ?? "<null>";
		}
	}

	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				Serializer seri = new XmlSerializer("test.xml");

				A a = new A(new A("foo"), new A("bar"), null);

				zeroflag.Serialization.Descriptors.Descriptor desc = zeroflag.Serialization.Descriptors.Descriptor.DoParse(a);

				Console.WriteLine(desc);
				//seri.Serialize(a);
				seri.Serialize(desc);
			}
			catch (Exception exc)
			{
				Console.WriteLine(exc);
			}
			finally { }
		}
	}
}
