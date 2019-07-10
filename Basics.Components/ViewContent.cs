using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Basics.Components
{
	public class ViewContent : Base
	{
		private string _ContentUri;
		/// <summary>
		/// uri to the content page
		/// </summary>
		public string ContentUri
		{
			get
			{
				return _ContentUri;
			}
			set
			{
				if (_ContentUri != value)
				{
					_ContentUri = value;
					this.OnPropertyChanged("ContentUri");
				}
			}
		}


		private IContentView _DataContext;
		/// <summary>
		/// the data to be provided to the page
		/// </summary>
		public IContentView DataContext
		{
			get
			{
				return _DataContext;
			}
			set
			{
				if (_DataContext != value)
				{
					_DataContext = value;
					this.OnPropertyChanged("DataContext");
				}
			}
		}

		public Command CloseView
		{
			get
			{
				return new Command(p =>
					{
						this.Project.CloseView(this);
					});
			}
		}

		public Project Project
		{
			get { return this.DataContext.Project; }
			set { this.DataContext.Project = value; }
		}

		public override string ToString()
		{
			return this.GetType().Name + "[" + (this.DataContext != null ? this.DataContext.Name : "<null>") + "]";
		}
	}
}
