using System;
namespace zeroflag.Serialization.Converters
{
	public interface IConverter
	{
		object __Get(object value);
		object __Set(object value);
		Type Type1 { get; }
		Type Type2 { get; }
	}
}
