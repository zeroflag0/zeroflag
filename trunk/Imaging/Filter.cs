using System;
using zeroflag.Collections;
using Point = System.Drawing.Point;
using PointF = System.Drawing.PointF;
using Size = System.Drawing.Size;
using SizeF = System.Drawing.SizeF;
using Rectangle = System.Drawing.Rectangle;

namespace zeroflag.Imaging
{
	//TODO: EVERY region is a filter -> rename, redesign (scaleing, padding).
	public class Filter : TreeNode<Filter>, IComparable<Filter>, zeroflag.Imaging.IPixelSource
	{
		#region Constructors
		public Filter()
		{
		}

		public Filter(Filter parent)
			: base(parent)
		{
			this.Size = parent.Size;
		}

		public Filter(Filter parent, float step)
			: this(parent, step, step)
		{
		}

		public Filter(Filter parent, float stepx, float stepy)
			: this(parent, new System.Drawing.SizeF(stepx, stepy))
		{
		}

		public Filter(Filter parent, System.Drawing.SizeF step)
			: this(parent)
		{
			this.Step = step;
		}

		public Filter(Filter parent, System.Drawing.Rectangle region)
			: base(parent)
		{
			this.Location = region.Location;
			this.Size = region.Size;
		}
		#endregion Constructors

		#region Pixels
		#region PixelBuffer

		private Color[,] _PixelBuffer = null;

		protected Color[,] PixelBuffer
		{
			get { return _PixelBuffer ?? (_PixelBuffer = this.CreatePixelBuffer()); }
			set
			{
				if (_PixelBuffer != value)
				{
					_PixelBuffer = value;
				}
			}
		}

		protected virtual Color[,] CreatePixelBuffer()
		{
			return new Color[this.Width, this.Height];
		}
		#endregion PixelBuffer

		public virtual Color this[int x, int y]
		{
			get
			{
				//TODO: Change: Filter into self but use parent for intput.
				if (this.NeedsUpdate && this.ApplyMode == ApplyModeType.OnDemand)
					this.Update();
				else if (this.PixelBuffer[x, y] == null && this.ApplyMode == ApplyModeType.OnDemandPixel)
					this.ApplyPixel(x, y);

				if (this.PixelBuffer[x, y] != null)
					return this.PixelBuffer[x, y];
				else if (this.Parent != null)
					return this.Parent[this.X + (int)((float)x * this.Step.Width), this.Y + (int)((float)y * this.Step.Height)];
				else
					return new Color();

				//return this.PixelBuffer[x, y] != null ?
				//    (this.Parent != null
				//    ? this.Parent[this.X + (int)((float)x * this.Step.Width), this.Y + (int)((float)y * this.Step.Height)]
				//    : new Color()) :
				//    new Color();
			}
			set
			{
				this.PixelBuffer[x, y] = value;
			}
		}
		#endregion Pixels

		#region TreeNode
		public override Filter Value
		{
			get
			{
				return this;
			}
			set
			{
			}
		}
		#endregion TreeNode

		#region Location
		Point _Location = new Point(0, 0);
		[System.ComponentModel.Category("Location")]
		public virtual Point Location
		{
			get { return _Location; }
			set { _Location = value; }
		}
		[System.Xml.Serialization.XmlIgnore]
		[System.ComponentModel.Category("Borders")]
		public int Left
		{
			get { return X; }
			set { X = value; }
		}
		[System.Xml.Serialization.XmlIgnore]
		[System.ComponentModel.Category("Location")]
		public int X
		{
			get { return this.Location.X; }
			set { this.Location = new Point(value, this.Y); }
		}
		[System.Xml.Serialization.XmlIgnore]
		[System.ComponentModel.Category("Borders")]
		public int Top
		{
			get { return Y; }
			set { Y = value; }
		}
		[System.Xml.Serialization.XmlIgnore]
		[System.ComponentModel.Category("Location")]
		public int Y
		{
			get { return this.Location.Y; }
			set { this.Location = new Point(this.X, value); }
		}
		#endregion Location

		#region Size

		Size _Size = new Size(100, 100);
		[System.ComponentModel.Category("Size")]
		public virtual Size Size
		{
			get { return _Size; }
			set { _Size = value; }
		}
		[System.Xml.Serialization.XmlIgnore]
		[System.ComponentModel.Category("Size")]
		public int Width
		{
			get { return this.Size.Width; }
			set { this.Size = new Size(value, this.Height); }
		}
		[System.Xml.Serialization.XmlIgnore]
		[System.ComponentModel.Category("Borders")]
		public int Right
		{
			get { return this.Left + this.Width; }
			set { this.Width = value - this.Left; }
		}
		[System.Xml.Serialization.XmlIgnore]
		[System.ComponentModel.Category("Size")]
		public int Height
		{
			get { return this.Size.Height; }
			set { this.Size = new Size(this.Width, value); }
		}
		[System.Xml.Serialization.XmlIgnore]
		[System.ComponentModel.Category("Borders")]
		public int Bottom
		{
			get { return this.Top + this.Height; }
			set { this.Height = value - this.Top; }
		}
		[System.Xml.Serialization.XmlIgnore]
		[System.ComponentModel.Category("Size")]
		public int Volume
		{
			get
			{
				return this.Width * this.Height;
			}
		}

		public Rectangle Rectangle
		{
			get { return new Rectangle(this.Location, this.Size); }
			set { this.Size = value.Size; this.Location = value.Location; }
		}
		#endregion Size

		#region Scale

		#region Step

		private SizeF _Step = new SizeF(1, 1);

		public virtual SizeF Step
		{
			get { return _Step; }
			set
			{
				if (_Step != value)
				{
					_Step = value;
					if (this.Parent != null)
					{
						this.Size = new Size((int)((float)this.Parent.Width / (float)value.Width), (int)((float)this.Parent.Height / (float)value.Height));
					}
				}
			}
		}
		#endregion Step

		#region Padding

		private Rectangle _Padding = default(Rectangle);

		public virtual Rectangle Padding
		{
			get { return _Padding; }
			set
			{
				if (_Padding != value)
				{
					_Padding = value;
				}
			}
		}
		#endregion Padding

		#endregion Scale

		#region IComparable<Filter> Members

		public int CompareTo(Filter other)
		{
			if (other != null)
				return this.Volume.CompareTo(other.Volume);
			else
				return -1;
		}

		#endregion

		#region Filter
		public bool Contains(int x, int y)
		{
			return x > this.X && y > this.Y && x < this.Right && y < this.Bottom;
		}

		public static implicit operator Rectangle(Filter region)
		{
			return region != null ? region.Rectangle : new Rectangle();
		}

		public Filter Then(Filter next)
		{
			next.Parent = this;
			next.Location = new Point(0, 0);
			next.Size = this.Size;
			return next;
		}
		#endregion Filter

		#region ToString
		public override string ToString()
		{
			return this.GetType().Name + " " + this.Location.ToString() + " " + this.Size.ToString();
		}
		#endregion ToString

		#region Update
		#region NeedsUpdate

		private bool _NeedsUpdate = false;

		public bool NeedsUpdate
		{
			get { return _NeedsUpdate || this.Parent != null && this.Parent.NeedsUpdate; }
			set
			{
				if (_NeedsUpdate != value)
				{
					_NeedsUpdate = value;
				}
			}
		}
		#endregion NeedsUpdate

		public void Update()
		{
			if (this.NeedsUpdate)
			{
				Console.WriteLine(this + " updating...");
				this.PixelBuffer = this.CreatePixelBuffer();
				this.DoUpdate();
				Console.WriteLine(this + " updated.");
			}

			foreach (Filter child in this.Children)
			{
				child.Update();
			}

			this.NeedsUpdate = false;
		}

		protected virtual void DoUpdate()
		{
			if (this.ApplyMode == ApplyModeType.OnUpdate)
				this.Apply();
		}

		protected virtual void Apply()
		{
			for (int y = this.Padding.Y; y < this.Height - this.Padding.Height; y++)
			{
				for (int x = this.Padding.X; x < this.Width - this.Padding.Width; x++)
				{
					this.ApplyPixel(x, y);
				}
			}
		}

		protected virtual void ApplyPixel(int x, int y)
		{
		}

		#endregion Update

		#region Apply
		public enum ApplyModeType
		{
			Inherit,
			OnUpdate,
			OnChange,
			OnRender,
			OnDemand,
			OnDemandPixel,
		}

		ApplyModeType _ApplyMode = ApplyModeType.Inherit;

		public ApplyModeType ApplyMode
		{
			get
			{
				return _ApplyMode == ApplyModeType.Inherit ?
					this.Parent != null ? this.Parent.ApplyMode : ApplyModeType.OnUpdate
					: _ApplyMode;
			}
			set { _ApplyMode = value; }
		}
		#endregion Apply

		#region Rendering
		bool _AllowRendering = true;

		public bool AllowRendering
		{
			get { return _AllowRendering; }
			set { _AllowRendering = value; }
		}

		public void Render(System.Drawing.Image img)
		{
			System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(img);
			g.ScaleTransform((float)img.Width / (float)this.Width, (float)img.Height / (float)this.Height);

			this.Render(g);
		}

		public void Render(System.Drawing.Graphics g)
		{
			g.ScaleTransform((float)this.Step.Width, (float)this.Step.Height);
			g.TranslateTransform(this.X, this.Y);

			if (this.ApplyMode == ApplyModeType.OnRender)
				this.Apply();

			if (AllowRendering)
				this.DoRender(g);


			foreach (Filter child in this.Children)
			{
				child.Render(g);
			}
		}

		protected virtual void DoRender(System.Drawing.Graphics g)
		{
			for (int y = this.Padding.Y; y < this.Height - this.Padding.Height; y++)
			{
				for (int x = this.Padding.X; x < this.Width - this.Padding.Width; x++)
				{
					Color color = this[x, y];
					g.FillRectangle(new System.Drawing.SolidBrush(color), x, y, 1, 1);
				}
			}
		}
		#endregion Rendering
	}
}
