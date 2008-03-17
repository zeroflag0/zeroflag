using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace zeroflag.Zml.Factories
{
	public class ImageFactory : zeroflag.Zml.Factory<Image>
	{

		#region File

		private string _File = default(string);

		public string File
		{
			get { return _File; }
			set
			{
				if (_File != value)
				{
					_File = value;
				}
			}
		}
		#endregion File

		#region Resource

		private string _Resource = default(string);

		public string Resource
		{
			get { return _Resource; }
			set
			{
				if (_Resource != value)
				{
					_Resource = value;
				}
			}
		}
		#endregion Resource

		public override Image Create()
		{
			if (this.Resource != null)
			{
				foreach (Type type in TypeHelper.Types)
				{
					try
					{
						return new Bitmap(type, this.Resource);
					}
					catch
					//(Exception exc)
					{
						//Console.WriteLine(exc);
					}
				}
			}
			if (this.File != null)
				return Bitmap.FromFile(this.File);
			return null;
		}
	}
}
