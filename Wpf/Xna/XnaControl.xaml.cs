﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Xna.Framework.Graphics;
using zeroflag.wpf;

namespace zeroflag.Wpf.Xna
{
	/// <summary>
	/// Interaction logic for XnaControl.xaml
	/// </summary>
	public partial class XnaControl : UserControl
	{
		private GraphicsDeviceService graphicsService;
		private XnaImageSource imageSource;

		/// <summary>
		/// Gets the GraphicsDevice behind the control.
		/// </summary>
		public GraphicsDevice GraphicsDevice
		{
			get
			{
				return graphicsService.GraphicsDevice;
			}
		}

		/// <summary>
		/// Invoked when the XnaControl needs to be redrawn.
		/// </summary>
		public Action<GraphicsDevice> DrawFunction;

		public static readonly DependencyProperty DrawCommandProperty =
		DependencyProperty.Register("DrawCommand", typeof(Command<GraphicsDevice>), typeof(XnaControl), new PropertyMetadata(default(Command<GraphicsDevice>)));

		/// <summary>
		/// Invoked when the XnaControll needs to be redrawn.
		/// </summary>
		public Command<GraphicsDevice> DrawCommand
		{
			get
			{
				return (Command<GraphicsDevice>)GetValue(DrawCommandProperty);
			}
			set
			{
				SetValue(DrawCommandProperty, value);
			}
		}


		public XnaControl()
		{
			InitializeComponent();

			// hook up an event to fire when the control has finished loading
			Loaded += new RoutedEventHandler(XnaControl_Loaded);
		}

		~XnaControl()
		{
			imageSource.Dispose();

			// release on finalizer to clean up the graphics device
			if (graphicsService != null)
				graphicsService.Release();
		}

		void XnaControl_Loaded(object sender, RoutedEventArgs e)
		{
			// if we're not in design mode, initialize the graphics device
			if (DesignerProperties.GetIsInDesignMode(this) == false)
			{
				InitializeGraphicsDevice();
			}
		}

		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
		{
			// if we're not in design mode, recreate the 
			// image source for the new size
			if (DesignerProperties.GetIsInDesignMode(this) == false &&
				graphicsService != null)
			{
				// recreate the image source
				imageSource.Dispose();
				imageSource = new XnaImageSource(
					GraphicsDevice, (int)ActualWidth, (int)ActualHeight);
				rootImage.Source = imageSource.WriteableBitmap;
			}

			base.OnRenderSizeChanged(sizeInfo);
		}

		private void InitializeGraphicsDevice()
		{
			if (graphicsService == null)
			{
				// add a reference to the graphics device
				graphicsService = GraphicsDeviceService.AddRef(
					(PresentationSource.FromVisual(this) as HwndSource).Handle);

				// create the image source
				imageSource = new XnaImageSource(
					GraphicsDevice, (int)ActualWidth, (int)ActualHeight);
				rootImage.Source = imageSource.WriteableBitmap;

				// hook the rendering event
				CompositionTarget.Rendering += CompositionTarget_Rendering;
			}
		}

		/// <summary>
		/// Draws the control and allows subclasses to override 
		/// the default behavior of delegating the rendering.
		/// </summary>
		protected virtual void Render()
		{
			// invoke the draw delegate so someone will draw something pretty
			if (DrawFunction != null)
				DrawFunction(GraphicsDevice);

			if (DrawCommand != null)
				if (DrawCommand.CanExecute(GraphicsDevice))
					DrawCommand.Execute(GraphicsDevice);
		}

		void CompositionTarget_Rendering(object sender, EventArgs e)
		{
			// set the image source render target
			GraphicsDevice.SetRenderTarget(imageSource.RenderTarget);

			// allow the control to draw
			Render();

			// unset the render target
			GraphicsDevice.SetRenderTarget(null);

			// commit the changes to the image source
			imageSource.Commit();
		}
	}
}
