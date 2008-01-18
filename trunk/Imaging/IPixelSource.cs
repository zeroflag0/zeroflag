using System;
namespace zeroflag.Imaging
{
	public interface IPixelSource
	{
		Color this[int x, int y] { get; set; }
		int Width { get; }
		int Height { get; }
	}
}
