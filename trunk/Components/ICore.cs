using System;
namespace zeroflag.Components
{
	public interface ICore
	{
		ComponentCollection<Module> Modules { get; }
		void Run();
		void Shutdown();
	}
}
