using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Basics.Components
{
	public interface IContent : ISavable, IDisposable
	{
		/// <summary>
		/// Short name of the content
		/// </summary>
		string Name { get; }
		/// <summary>
		/// Description of the content
		/// </summary>
		string Description { get; }
		/// <summary>
		/// whether this content is selected
		/// </summary>
		bool IsSelected { get; set; }
	}
}
