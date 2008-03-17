using System;
namespace zeroflag.Zml
{
	public interface IFactory
	{
		object Create();
		Type ProductType { get; }
	}
}
