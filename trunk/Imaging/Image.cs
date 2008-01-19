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
using Base = System.Drawing.Bitmap;
using Point = System.Drawing.Point;
using PointF = System.Drawing.PointF;
using Size = System.Drawing.Size;
using SizeF = System.Drawing.SizeF;
using Rectangle = System.Drawing.Rectangle;


namespace zeroflag.Imaging
{
	public class Image : Filter
	{
		#region Source

		private Base _Source = default(Base);

		public Base Source
		{
			get { return _Source; }
			set
			{
				if (_Source != value)
				{
					_Source = value;
					this.NeedsUpdate = true;
				}
			}
		}
		#endregion Source

		public override Color this[int x, int y]
		{
			get
			{
				return this.PixelBuffer[x, y] ?? base[x, y];
			}
			set
			{
				base[x, y] = value;
			}
		}

		//public override SizeF Step
		//{
		//    get
		//    {
		//        if (this.Source != null)
		//        {
		//            lock (this.Source)
		//            {
		//                return new SizeF((float)this.Width / (float)this.Source.Width, (float)this.Height / (float)this.Source.Height);
		//            }
		//        }
		//        else
		//            return base.Step;
		//    }
		//    set
		//    {
		//        base.Step = value;
		//    }
		//}

		protected override Color[,] CreatePixelBuffer()
		{
			Color[,] buffer = this.PixelBuffer = base.CreatePixelBuffer();
			lock (buffer)
			{
				Base bmp = this.Source;
				int width = 0, height = 0;
				System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
				sw.Start();
				if (bmp != null)
				{
					byte[] data = new byte[0];
					int bpp = 1;
					lock (bmp)
					{
						System.Drawing.Imaging.BitmapData lockBmp = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp.PixelFormat);
						try
						{
							data = new byte[lockBmp.Stride * lockBmp.Height];

							System.Runtime.InteropServices.Marshal.Copy(lockBmp.Scan0, data, 0, data.Length);

							width = bmp.Width;
							height = bmp.Height;
							bpp = Base.GetPixelFormatSize(bmp.PixelFormat) / 8;
						}
						catch (Exception exc)
						{
							Console.WriteLine(exc);
							throw;
						}
						finally
						{
							bmp.UnlockBits(lockBmp);
						}
					}
					int a = 0xff, r, g, b, ptr;
					Color color = System.Drawing.Color.Transparent;
					int donex = 0, doney = 0;
					double internx, interny = 0;
					double stepx = (double)this.Width / (double)width;
					double stepy = (double)this.Height / (double)height;
					for (int y = 0; y < height; y++, interny += stepy)
					{
						if (doney <= interny)
						{
							internx = 0;
							donex = 0;
							for (int x = 0; x < width; x++, internx += stepx)
							{
								if (donex <= internx)
								{
									try
									{
										ptr = (y * width + x) * bpp;
										//if (bpp == 4)
										//{
										//    ptr++;
										//}
										b = data[ptr++];
										g = data[ptr++];
										r = data[ptr++];
										color = new Color(a, r, g, b);
									}
									catch (Exception exc)
									{
										Console.WriteLine(exc);
									}
									try
									{
										for (int ty = doney; ty <= interny; ty++)
											for (; donex <= internx; donex++)
												buffer[donex, ty] = color;
									}
									catch (Exception exc)
									{
										Console.WriteLine(exc);
									}
								}
							}
							doney++;
						}
					}

					//double bx = 0, by = 0, dx, dy, ix, iy;
					//Color color = System.Drawing.Color.Transparent;
					//for (int y = 0; y < height; y++)
					//{
					//    dy = (y * this.Height) / height - by;
					//    by += dy;
					//    dx = 0;
					//    for (int x = 0; x < width; x++)
					//    {
					//        dx = (x * this.Width) / width - bx;
					//        bx += dx;
					//        if (by - dy <= by && bx - dx <= bx)
					//        {
					//            try
					//            {
					//                ptr = (y * width + x) * bpp;
					//                //if (bpp == 4)
					//                //{
					//                //    ptr++;
					//                //}
					//                b = data[ptr++];
					//                g = data[ptr++];
					//                r = data[ptr++];
					//                color = new Color(a, r, g, b);
					//            }
					//            catch (Exception exc)
					//            {
					//                Console.WriteLine(exc);
					//            }
					//            try
					//            {
					//                for (iy = by - dy; iy <= by; iy++)
					//                    for (ix = bx - dx; ix <= bx; ix++)
					//                        buffer[(int)ix, (int)iy] = color;
					//            }
					//            catch (Exception exc)
					//            {
					//                Console.WriteLine(exc);
					//            }
					//        }
					//    }
					//}
				}
				//else
				//{
				//    for (int y = 0; y < this.Height; y++)
				//    {
				//        for (int x = 0; x < this.Width; x++)
				//        {
				//            buffer[x, y] = System.Drawing.Color.Transparent;
				//        }
				//    }
				//}
				sw.Stop();
				Console.WriteLine(this + " created pixelbuffer in " + sw.Elapsed);
				return buffer;
			}
		}

		public Image(int width, int height)
			: base()
		{
			this.Size = new Size(width, height);
		}

		public Image(Filter parent, System.Drawing.Rectangle region)
			: base(parent, region)
		{
		}

	}
}
