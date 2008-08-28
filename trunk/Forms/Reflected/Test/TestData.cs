using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Test
{
	[Serializable]
	public class TestData
	{
		#region Name

		private string _Name = default( string );

		public string Name
		{
			get { return _Name; }
			set
			{
				if ( _Name != value )
				{
					_Name = value;
				}
			}
		}
		#endregion Name

		#region Int

		private int _Int = default( int );

		public int Int
		{
			get { return _Int; }
			set
			{
				if ( _Int != value )
				{
					_Int = value;
				}
			}
		}
		#endregion Int

		#region Hidden
		private bool _Hidden = default( bool );

		[System.ComponentModel.Browsable( false )]
		public bool Hidden
		{
			get { return _Hidden; }
			set
			{
				if ( _Hidden != value )
				{
					_Hidden = value;
				}
			}
		}
		#endregion Hidden

		double _Real;

		public double Real
		{
			get { return _Real; }
			set { _Real = value; }
		}

		zeroflag.Collections.Collection<TestData> _Inner = new zeroflag.Collections.Collection<TestData>();

		//[System.ComponentModel.Browsable(false)]
		public zeroflag.Collections.Collection<TestData> Inner
		{
			get { return _Inner; }
			//set { _Inner = value; }
		}

		public override string ToString()
		{
			return new StringBuilder().Append( this.GetType().Name ).Append( "[" ).Append( this.Name ).Append( "," ).Append( this.Int ).Append( "," ).Append( this.Real ).Append( "," ).Append( this.GetHashCode() ).Append( "]" ).ToString();
			//return this.ToString( new StringBuilder(), new List<TestData>(), 0 ).ToString();
		}

		public StringBuilder ToString( StringBuilder b, List<TestData> done, int depth )
		{
			b.AppendLine().Append( ' ', depth ).Append( this.GetType().Name ).Append( "[" ).Append( this.Name ).Append( "," ).Append( this.Int ).Append( "," ).Append( this.Real ).Append( "," ).Append( this.GetHashCode() );
			if ( false && this.Inner != null )
			{
				if ( depth > 10 )
				{
					b.Append( ",<...>" );
				}
				else if ( done.Contains( this ) )
				{
					b.Append( ",<link>" );
				}
				else
				{
					done.Add( this );
					depth++;
					b.AppendLine().Append( ' ', depth ).Append( "{" );
					foreach ( TestData inner in this.Inner )
					{
						inner.ToString( b, done, depth );
					}
					depth--;
					b.AppendLine().Append( ' ', depth ).Append( "}" );
				}
			}
			b.Append( "]" );
			return b;
		}

		public TestData()
		{
		}

		public TestData( string name, int integer, float real )
		{
			this.Name = name;
			this.Int = integer;
			this.Real = real;
		}

		public TestData Add( TestData inner )
		{
			this.Inner.Add( inner );
			return this;
		}
	}

	public class TestData2 : TestData
	{
		public TestData2()
		{
		}

		public TestData2( string name, int integer, float real )
			: base( name, integer, real )
		{ }
	}
}
