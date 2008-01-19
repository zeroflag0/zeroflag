#region LGPL License
//********************************************************************
//	author:         Thomas "zeroflag" Kraemer
//	author email:   zeroflag@zeroflag.de
//	
//	Copyright (C) 2006-2008  Thomas "zeroflag" Kraemer
//	
//	license:	(LGPL)
//	
//		This library is free software; you can redistribute it and/or
//		modify it under the terms of the GNU Lesser General Public
//		License as published by the Free Software Foundation; either
//		version 2.1 of the License, or (at your option) any later version.
//
//		This library is distributed in the hope that it will be useful,
//		but WITHOUT ANY WARRANTY; without even the implied warranty of
//		MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//		Lesser General Public License for more details.
//
//		You should have received a copy of the GNU Lesser General Public
//		License along with this library; if not, write to the Free Software
//		Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
//
//		http://www.gnu.org/licenses/lgpl.html#TOC1
//
//*********************************************************************
#endregion LGPL License

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
//using System.Drawing;
using System.Text;
using System.Windows.Forms;
using zeroflag.Imaging;

namespace Test
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();

			this.pictureBox3.Image = this.ResultBmp;

			this.Result =
				//new Edge(new Edge(this.Root));
				//new AreaFinder
				//(new Change(new Edge(this.Root, new System.Drawing.Rectangle(20, 10, 160, 40)))
				//);
				new StrategyFilter(this.Root).Do(
				new zeroflag.Imaging.Strategies.Edge2()
				.Then(new zeroflag.Imaging.Strategies.Change())
				).Then(new AreaFinder());
			//new Change(new Edge(this.Root));

			//new LineFinder(new Edge(new Edge(this.Root)));
			//new LineFinder(this.Root);
			//new Change(this.Root, 1f);
		}

		Filter _Result;

		public Filter Result
		{
			get { return _Result; }
			set { _Result = value; }
		}

		Image _Root = new Image(200, 100);

		public Image Root
		{
			get { return _Root; }
			set { _Root = value; }
		}

		System.Drawing.Bitmap _ResultBmp = new System.Drawing.Bitmap(800, 800);

		public System.Drawing.Bitmap ResultBmp
		{
			get { return _ResultBmp; }
			set { _ResultBmp = value; }
		}

		#region Image

		private System.Drawing.Image m_Image;

		/// <summary>
		/// Image
		/// </summary>
		public System.Drawing.Image Image
		{
			get { return m_Image; }
			set
			{
				if (m_Image != value)
				{
					this.OnImageChanged(m_Image, m_Image = value);
				}
			}
		}

		#region ImageChanged event
		public delegate void ImageChangedHandler(object sender, System.Drawing.Image oldvalue, System.Drawing.Image newvalue);

		private event ImageChangedHandler m_ImageChanged;
		/// <summary>
		/// Occurs when Image changes.
		/// </summary>
		public event ImageChangedHandler ImageChanged
		{
			add { this.m_ImageChanged += value; }
			remove { this.m_ImageChanged -= value; }
		}

		/// <summary>
		/// Raises the ImageChanged event.
		/// </summary>
		protected virtual void OnImageChanged(System.Drawing.Image oldvalue, System.Drawing.Image newvalue)
		{
			//this.pictureBox2.Image = this.pictureBox1.Image = null;
			//this.pictureBox2.Invalidate();
			//this.pictureBox1.Invalidate();
			//this.pictureBox2.Update();
			//this.pictureBox1.Update();

			this.pictureBox2.Image = oldvalue;
			this.pictureBox1.Image = newvalue;
			System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
			sw.Start();
			this.Root.Source = new System.Drawing.Bitmap(newvalue);
			this.Root.Update();
			sw.Stop();
			TimeSpan update = sw.Elapsed;
			this.Text = "Update=" + update;
			sw.Reset();
			sw.Start();
			this.Root.Render(this.ResultBmp);
			this.pictureBox3.Image = this.ResultBmp;
			sw.Stop();
			this.Text = "Update=" + update + " Rendering=" + sw.Elapsed;

			//this.ResultBmp.Save("result.bmp", System.Drawing.Imaging.ImageFormat.Bmp);

			//this.pictureBox2.Invalidate();
			//this.pictureBox1.Invalidate();
			//this.Invalidate();
			//this.Update();
			//Application.DoEvents();

			// if there are event subscribers...
			if (this.m_ImageChanged != null)
			{
				// call them...
				this.m_ImageChanged(this, oldvalue, newvalue);
			}
		}
		#endregion ImageChanged event
		#endregion Image


		#region Capturing
		#region Target

		private zeroflag.Windows.Window m_Target = null;

		/// <summary>
		/// The target window to be controlled/observed.
		/// </summary>
		public zeroflag.Windows.Window Target
		{
			get { return m_Target; }
			set
			{
				if (m_Target != value)
				{
					this.OnTargetChanged(m_Target, m_Target = value);
				}
			}
		}

		#region TargetChanged event
		public delegate void TargetChangedHandler(object sender, zeroflag.Windows.Window oldvalue, zeroflag.Windows.Window newvalue);

		private event TargetChangedHandler m_TargetChanged;
		/// <summary>
		/// Occurs when Target changes.
		/// </summary>
		public event TargetChangedHandler TargetChanged
		{
			add { this.m_TargetChanged += value; }
			remove { this.m_TargetChanged -= value; }
		}

		/// <summary>
		/// Raises the TargetChanged event.
		/// </summary>
		protected virtual void OnTargetChanged(zeroflag.Windows.Window oldvalue, zeroflag.Windows.Window newvalue)
		{
			// if there are event subscribers...
			if (this.m_TargetChanged != null)
			{
				// call them...
				this.m_TargetChanged(this, oldvalue, newvalue);
			}
		}
		#endregion TargetChanged event

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			this.Invalidate();
		}
		#endregion Target

		private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				this.Image = this.CaptureTarget();
			}
			else if (e.Button != MouseButtons.Right)
			{
				this.Image = this.CaptureTargetClean();
			}
		}

		private void pictureBox1_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				this.Image = this.SelectTarget();
			}
		}

		public System.Drawing.Bitmap SelectTarget()
		{
			System.Drawing.Bitmap bmp = null;

			bool timerEnabled = this.timer.Enabled;
			try
			{
				this.timer.Stop();
				this.FindForm().Hide();
				SelectWindowDialog dia = new SelectWindowDialog();

				if (dia.ShowDialog(this) == DialogResult.OK && dia.Target != this.Handle && dia.Target != IntPtr.Zero)
				{
					this.Target = null;
					this.Target = new zeroflag.Windows.Window(dia.Target);
					bmp = this.CaptureTargetClean();
				}

				return bmp;
			}
			finally
			{
				this.FindForm().Show();
				this.FindForm().Activate();
				Application.DoEvents();
				this.FindForm().Update();
				this.FindForm().Select();
				this.FindForm().Activate();
				this.timer.Enabled = timerEnabled;
			}
		}
		public System.Drawing.Bitmap CaptureTargetClean()
		{
			System.Drawing.Bitmap bmp = null;

			if (this.Target != null)
			{
				Form form = this.FindForm();
				FormWindowState oldstate = form.WindowState;
				form.WindowState = FormWindowState.Minimized;
				form.Hide();
				Application.DoEvents();
				this.Target.Activate(true);
				bmp = this.CaptureTarget();
				Application.DoEvents();
				form.Show();
				form.WindowState = oldstate;
				form.Activate();
				Application.DoEvents();
				form.Update();
				form.Select();
				form.Activate();
			}
			return bmp;
		}

		public System.Drawing.Bitmap CaptureTarget()
		{
			if (this.Target != null)
			{
				System.Drawing.Bitmap bmp = null;
				try
				{
					bmp = new System.Drawing.Bitmap(this.Target.Capture());
				}
				catch (Exception exc)
				{
					Console.WriteLine(exc);
					this.Target = null;
				}
				return bmp;
			}
			else
				return null;
		}
		#endregion Capturing

		#region Handle save/load
		const string LastHandle = "current.handle";
		protected override void OnClosed(EventArgs e)
		{
			if (this.Target != null)
				try
				{
					System.IO.File.WriteAllText(LastHandle, this.Target.Handle.ToInt64().ToString());
				}
				catch (Exception exc)
				{
					MessageBox.Show(exc.ToString());
				}
			base.OnClosed(e);
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			try
			{
				this.Target = new zeroflag.Windows.Window(new IntPtr(long.Parse(System.IO.File.ReadAllText(LastHandle))));
				//this.Image = this.CaptureTargetClean();
			}
			catch (System.IO.FileNotFoundException) { }
			catch (Exception exc)
			{
				MessageBox.Show(exc.ToString());
			}
		}
		#endregion Handle save/load

	}
}