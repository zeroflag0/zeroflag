using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Serialization.Descriptors
{
	public class DateTimeDescriptor : Descriptor<System.DateTime>
	{
		public override object GenerateLink()
		{
			//return base.GenerateLink();
			if ( this.Inner.Count == 1 )
			{
				return System.DateTime.FromBinary( (long)this.Inner[ 0 ].GenerateLink() );
			}
			else
				return DateTime.Now;
		}

		public override void Parse()
		{
			this.Inner.Add( this.Context.Parse( "timestamp", typeof( long ), this.GetValue().ToBinary(), this.Value, null ) );
		}
	}
}
