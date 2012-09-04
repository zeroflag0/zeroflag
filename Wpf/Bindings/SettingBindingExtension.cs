using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace zeroflag.Wpf.Bindings
{
	public class SettingBindingExtension : Binding
	{
		public static object Settings;

		public SettingBindingExtension()
		{
			Initialize();
		}

		public SettingBindingExtension(string path)
			: base(path)
		{
			Initialize();
		}

		private void Initialize()
		{
			this.Source = Settings;
			this.Mode = BindingMode.TwoWay;
		}
	}
}
