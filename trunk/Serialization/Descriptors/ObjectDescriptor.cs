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
	public class ObjectDescriptor : Descriptor<object>
	{
		public override bool NeedsWriteAccess
		{
			get
			{
				return false;
			}
		}
		public override void Parse()
		{
			//if ( this.Value == null )
			//    return;

			Type type = this.Type;

			System.Reflection.PropertyInfo[] properties = this.GetProperties( type ).ToArray();

			foreach ( System.Reflection.PropertyInfo prop in properties )
			{
				System.Reflection.PropertyInfo property = prop;
				if ( this.HasFilters )
				{
					bool filtered = true;
					foreach ( var filter in this.Filters )
					{
						if ( ( filter( this.Value, ref property ) ) )
						{
							filtered = false;
							break;
						}
					}
					if ( filtered )
					{
						//CWL( "\t" + this + " filtered property " + property );
						continue;
					}
					else
					{
						//CWL( "\t" + this + " un-filtered property " + property );
					}
				}

				Descriptor desc = this.Context.Parse( property, this.Value );
				if ( desc != null && !this.Inner.Contains( desc ) )
					this.Inner.Add( desc );
			}

			//if ( type.IsValueType )
			//{
			//    System.Reflection.FieldInfo[] fields = type.GetFields();

			//    foreach ( System.Reflection.FieldInfo property in fields )
			//    {
			//        Descriptor desc = this.Context.Parse( property, this.Value );
			//        if ( desc != null && !this.Inner.Contains( desc ) )
			//            this.Inner.Add( desc );
			//    }

			//}
		}
	}
}
