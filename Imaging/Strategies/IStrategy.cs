using System;
namespace zeroflag.Imaging.Strategies
{
	public interface IStrategy
	{
		System.Collections.Generic.List<Strategy> Next { get; }
		void PostApply();
		void PreApply();
		Strategy Then(Strategy next);
		Color Apply(int x, int y);
		IPixelSource PixelSource { get; set; }
		void Prepare();
		System.Drawing.Rectangle MinimumPadding { get; }
	}
}
