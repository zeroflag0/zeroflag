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
#if !SILVERLIGHT
				return System.DateTime.FromBinary( (long)this.Inner[ 0 ].GenerateLink() );
#else
				long dateData = (long)this.Inner[0].GenerateLink();
				long utcOffsetFromUniversalTime;
				if ( ( dateData & -9223372036854775808L ) == 0L )
				{
					long num = dateData & 0x3fffffffffffffffL;
					if ( ( num < 0L ) || ( num > 0x2bca2875f4373fffL ) )
					{
						throw new ArgumentException( "Invalid binary data for DateTime.", "dateData" );
					}
					return new DateTime( dateData );
				}
				long ticks = dateData & 0x3fffffffffffffffL;
				if ( ticks > 0x3fffff36d5964000L )
				{
					ticks -= 0x4000000000000000L;
				}
				if ( ticks < 0L )
				{
					utcOffsetFromUniversalTime = TimeZoneInfo.Local.GetUtcOffset( DateTime.MinValue ).Ticks;
				}
				else if ( ticks > 0x2bca2875f4373fffL )
				{
					utcOffsetFromUniversalTime = TimeZoneInfo.Local.GetUtcOffset( DateTime.MaxValue ).Ticks;
				}
				else
				{
					utcOffsetFromUniversalTime = TimeZoneInfo.Local.GetUtcOffset( new DateTime( ticks ) ).Ticks;
				}
				ticks += utcOffsetFromUniversalTime;
				if ( ticks < 0L )
				{
					ticks += 0xc92a69c000L;
				}
				if ( ( ticks < 0L ) || ( ticks > 0x2bca2875f4373fffL ) )
				{
					throw new ArgumentException( "Invalid binary data for DateTime.", "dateData" );
				}
				return new DateTime( ticks, DateTimeKind.Local );

#endif
			}
			else
				return DateTime.Now;
		}

		public override void Parse()
		{
#if !SILVERLIGHT
			this.Inner.Add( this.Context.Parse( "timestamp", typeof( long ), this.GetValue().ToBinary(), this.Value, null ) );
#else

			this.Inner.Add( this.Context.Parse( "timestamp", typeof( long ), 0, this.Value, null ) );
#endif
		}
	}
}
