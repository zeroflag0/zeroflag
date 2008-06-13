using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace zeroflag.Forms.Reflected
{
	public partial class TypeList<T> : ListView<Type>
	{
		public TypeList()
		{
			InitializeComponent();
			AppDomain.CurrentDomain.AssemblyLoad += new AssemblyLoadEventHandler(CurrentDomain_AssemblyLoad);
		}

		void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
		{
			this.Synchronize();
		}

		public override void Synchronize()
		{
			List<Type> types = zeroflag.Reflection.TypeHelper.GetDerived(typeof(T));
			List<Type> removed = new List<Type>();

			foreach (Type type in this.Items)
				if (!types.Contains(type))
					removed.Add(type);

			foreach (Type type in removed)
				this.Items.Remove(type);

			foreach (Type type in types)
				if (!this.Items.Contains(type))
					this.Items.Add(type);
			

			base.Synchronize();
		}
	}
}
