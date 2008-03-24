using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Test
{
	public class ListEditor : System.ComponentModel.Design.CollectionEditor
	{
		public ListEditor(Type type)
			: base(type)
		{
			//MessageBox.Show("type=" + (type ?? (object)"<null>"));
		}

		protected override object CreateInstance(Type itemType)
		{
			object ret = base.CreateInstance(itemType);
			//MessageBox.Show(ret + " = CreateInstance(itemType=" + (itemType ?? (object)"<null>") + ")");
			return ret;
		}

		protected override object SetItems(object editValue, object[] value)
		{
			object ret = base.SetItems(editValue, value);
			//MessageBox.Show(ret + " = SetItems(editValue=" + (editValue ?? (object)"<null>") + ", value=" + (Print(value) ?? (object)"<null>") + ")");
			new zeroflag.Forms.DebugForm("SetItems()", editValue, value, ret);
			//new zeroflag.Forms.DebugForm("SetItems(value)", value);
			//new zeroflag.Forms.DebugForm("return SetItems()", ret);
			//if (editValue is zeroflag.Collections.Collection<TestData>)
			//{
			//    zeroflag.Collections.Collection<TestData> collection = (zeroflag.Collections.Collection<TestData>)editValue;
			//    foreach (object o in value)
			//        collection.Add((TestData)o);
			//    return collection;
			//}
			return ret;
		}

		static string Print(object[] items)
		{
			string value = "{ ";
			foreach (object o in items)
			{
				value += o + ", ";
			}
			value += " }";
			return value;
		}

		protected override object[] GetItems(object editValue)
		{
			object[] ret = base.GetItems(editValue);
			//MessageBox.Show(ret + " = GetItems(editValue=" + (editValue ?? (object)"<null>") + ")");
			return ret;
		}

		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			return base.EditValue(context, provider, value);
		}
	}
	public partial class TestList2 : ListView
	{
		public TestList2()
		{
			InitializeComponent();
		}

		zeroflag.Collections.Collection<TestData> _TestItems = new zeroflag.Collections.Collection<TestData>();

		[MergableProperty(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[System.ComponentModel.EditorAttribute(typeof(ListEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public zeroflag.Collections.Collection<TestData> TestItems
		{
			get { return _TestItems; }
		}

		[MergableProperty(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[System.ComponentModel.EditorAttribute(typeof(ListEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public ListViewItemCollection MyItems
		{
			get { return base.Items; }
		}
	}
}
