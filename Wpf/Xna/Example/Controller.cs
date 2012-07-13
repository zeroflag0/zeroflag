using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace zeroflag.Wpf.Xna.Example
{
	class Controller : RenderingControllerBase
	{
		protected override void DrawAction(GraphicsDevice device)
		{
			device.Clear(new Microsoft.Xna.Framework.Color(1f, 0f, 1f, 1f));
		}
	}
}
