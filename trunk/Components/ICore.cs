using System;
namespace zeroflag.Components
{
	public interface ICore
	{
		ComponentCollection<Module> Modules { get; }
		bool Run();
		void Shutdown();
	}
}
