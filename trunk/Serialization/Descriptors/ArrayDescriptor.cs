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
	public class ArrayDescriptor : Descriptor<Array>, IListDescriptor
	{
		//public override Type Type
		//{
		//    get
		//    {
		//        return this.Value != null ? this.Value.GetType() : (base.Type == typeof(System.Collections.IEnumerable) ? typeof(System.Collections.Generic.IList<>) : base.Type);
		//    }
		//    set
		//    {
		//        base.Type = value;
		//    }
		//}

		//public override Descriptor Parse(System.Reflection.PropertyInfo info)
		//{
		//    //this.Get = delegate() { return (System.Collections.Generic.IList<T>)info.GetGetMethod().Invoke(this.Owner.GetValue(), null); };
		//    //this.Set = delegate(T value) { info.GetSetMethod().Invoke(this.Owner.GetValue(), new object[] { value }); };

		//    return this.Parse();
		//}
		public const string NameItem = null;

		public override void Parse()
		{
			//if (this.Value != null)
			{
				CWL( this + " parsing " + ( this.Value ?? this.Type ?? (object)"<null>" ) );
				Array arr = this.GetValue();
				if ( arr != null )
				{
					foreach ( object value in arr )
					{
						Descriptor item = this.Context.Parse( null, this.ItemType, value, this.Value, null );
						//item.Name = NameItem;
						if ( !this.Inner.Contains( item ) )
							this.Inner.Add( item );
					}
				}
			}
		}

		//protected override object DoGenerate()
		//{
		//    if (this.Value == null)
		//        this.Value = this.DoCreateInstance();
		//    IList<T> value = this.GetValue();

		//    if (value != null && this.Inner.Count > 0)
		//    {
		//        value.Clear();
		//        foreach (Descriptor sub in this.Inner)
		//        {
		//            if (sub.Name == NameItem)
		//            {
		//                value.Add((T)sub.Generate());
		//            }
		//        }
		//    }
		//    return this.Value;
		//}

		public override object GenerateLink()
		{
			//return base.GenerateLink();
			Array arr = this.GetValue();

			if ( arr == null || arr.Length != this.Inner.Count )
			{
				Console.WriteLine( "Creating array..." );
				try
				{

					var m = typeof( ArrayCreator<> ).Specialize( this.ItemType ).GetMethod( "CreateArray", System.Reflection.BindingFlags.Static );
					Console.WriteLine( "m=" + m );
					m.Invoke( null, new object[] { this.Inner.Count } );
				}
				catch ( Exception exc )
				{
					Console.WriteLine( exc );
				}
			}
			if ( arr != null && this.Inner.Count > 0 )
			{
				System.Reflection.PropertyInfo indexer = null;
				indexer = arr.GetType().GetProperty( "Item", this.ItemType, new Type[] { typeof( int ) } );
				if ( indexer == null )
					foreach ( var p in arr.GetType().GetProperties() )
					{
						if ( p != null && p.PropertyType == this.ItemType && p.GetIndexParameters().Length == 1 && p.GetIndexParameters()[0].ParameterType == typeof( int ) )
						{
							indexer = p;
							Console.WriteLine( "Manually found indexer: " + indexer );
							break;
						}
					}
					Console.WriteLine( "Found indexer: " + indexer );
				int i = 0;
				foreach ( Descriptor sub in this.Inner )
				{
					//if (sub.Name == NameItem)
					//{
					object item;
					//try
					{
						item = sub.GenerateLink();
					}
					//catch ( InvalidCastException )
					//{
					//    sub.Value = null;
					//    sub.GenerateParse();
					//    sub.GenerateCreate();
					//    item = (T)sub.GenerateLink();
					//}
					if ( item != null )
					{
						indexer.SetValue( arr, item, new object[] { i } );
						//value.Add( item );
					}
					//}
					i++;
				}
			}
			return this.Value;
		}
		class ArrayCreator<T>
		{
			public static T[] CreateArray( int i )
			{
				return new T[i];
			}
		}

		//protected override void SetValue(object on, System.Reflection.PropertyInfo prop)
		//{
		//    this.Value = this.GenerateLink();
		//    IList<T> value = this.GetValue();

		//    if (value != null)
		//    {
		//        if (prop != null)
		//        {
		//            IList<T> onValue = null;
		//            if ((onValue = prop.GetValue(on, new object[0]) as IList<T>) == null)
		//                prop.SetValue(on, this.GenerateLink(), new object[] { });
		//            else
		//            {
		//                foreach (T item in value)
		//                {
		//                    onValue.Add(item);
		//                }
		//            }
		//        }
		//    }
		//}

		#region IListDescriptor Members

		public Type ItemType
		{
			get
			{
				if ( this.Value != null )
					return this.Value.GetType().GetElementType();
				if ( this.Type != null )
					return this.Type.GetElementType();
				return null;
			}
		}

		#endregion
	}
}
