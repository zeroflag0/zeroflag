using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Xna.Framework.Graphics;
using zeroflag.wpf;

namespace zeroflag.Wpf.Xna
{
	public abstract class RenderingControllerBase : DependencyObject
	{
		public Command<GraphicsDevice> DrawCommand
		{
			get
			{
				return new Command<GraphicsDevice>(this.DrawAction);
			}
		}

		protected abstract void DrawAction(GraphicsDevice device);
	}
}
