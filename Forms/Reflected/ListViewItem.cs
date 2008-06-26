using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace zeroflag.Forms.Reflected
{
	public partial class ListViewItem<List, T> : System.Windows.Forms.ListViewItem
		where List : ListView<T>
	{
		#region Owner

		private List _Owner = default( List );

		public List Owner
		{
			get { return _Owner; }
			set
			{
				if ( _Owner != value )
				{
					_Owner = value;
					if ( _Owner != null )
						_Owner.Control.Items.Add( this );
				}
			}
		}
		#endregion Owner

		#region Value

		private T _Value = default( T );

		public T Value
		{
			get { return _Value; }
			set
			{
				if ( object.ReferenceEquals( _Value, null ) ||
					!object.ReferenceEquals( null, value ) && !value.Equals( _Value ) )
				{
					_Value = value;
				}
			}
		}
		#endregion Value

		public ListViewItem( List list, T value )
		{
			this.Owner = list;
			this.Value = value;
		}

		public void Synchronize()
		{
			TypeDescription desc = this.Owner.TypeDescription;

			var subItems = this.SubItems;
			//if ( subItems.Count > 0 &&
			//    subItems[ 0 ] != null &&
			//    ( subItems[ 0 ].Name == null || subItems[ 0 ].Name == "" ) )
			//    subItems.RemoveAt( 0 );


			foreach ( var prop in desc.Properties )
			{
				if ( !this.Owner.Control.Columns.ContainsKey( prop.Name ) )
					continue;
				ListViewSubItem cell;
				int index = this.Owner.Control.Columns.IndexOfKey( prop.Name );

				while ( subItems.Count <= index )
					subItems.Add( new ListViewSubItem( this, "" ) );
				cell = subItems[ index ];

				cell.Name = prop.Name;

				try
				{
					object value = prop.PropertyInfo.GetValue( this.Value, null );

					cell.Text = "" + value;

				}
				catch ( Exception exc )
				{
					cell.Text = exc.ToString();
				}
			}

		}
	}
}
