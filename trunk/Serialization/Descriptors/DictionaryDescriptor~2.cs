using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Serialization.Descriptors
{
	public class DictionaryDescriptor<T1, T2> : Descriptor<System.Collections.Generic.IDictionary<T1, T2>>
	{
		public const string NameItem = "Item";

		protected override void DoParse()
		{
			if (this.Value != null)
			{

				IDictionary<T1, T2> value = this.GetValue();

				foreach (KeyValuePair<T1, T2> pair in value)
				{
					Helper helper = new Helper(pair.Key, pair.Value);
					Descriptor item = DoParse(helper, this);
					item.Name = NameItem;
				}
			}
		}

		//protected override object DoGenerate()
		//{
		//    if (this.Value == null)
		//        this.Value = this.DoCreateInstance();
		//    IDictionary<T1, T2> value = this.GetValue();

		//    if (value != null && this.Inner.Count > 0)
		//    {
		//        value.Clear();
		//        foreach (Descriptor sub in this.Inner)
		//        {
		//            if (sub.Name == NameItem)
		//            {
		//                Helper helper = (Helper)(/*sub.Value ?? */sub.Generate());
		//                //KeyValuePair<T1, T2> pair = (KeyValuePair<T1, T2>)sub.Generate();
		//                value.Add(helper.Key, helper.Value);
		//            }
		//        }
		//    }
		//    return this.Value;
		//}

		public override object GenerateLink()
		{
			//if (this.Value == null)
			//    this.Value = this.DoCreateInstance();
			IDictionary<T1, T2> value = this.GetValue();

			if (value != null && this.Inner.Count > 0)
			{
				value.Clear();
				foreach (Descriptor sub in this.Inner)
				{
					if (sub.Name == NameItem)
					{
						Helper helper = (Helper)(/*sub.Value ?? */sub.GenerateLink());
						//KeyValuePair<T1, T2> pair = (KeyValuePair<T1, T2>)sub.Generate();
						value.Add(helper.Key, helper.Value);
					}
				}
			}
			return this.Value;
		}
		public struct Helper
		{
			public Helper(T1 key, T2 value)
			{
				_Key = key;
				_Value = value;
			}

			T1 _Key;

			public T1 Key
			{
				get { return _Key; }
				set { _Key = value; }
			}

			T2 _Value;

			public T2 Value
			{
				get { return _Value; }
				set { _Value = value; }
			}
		}
	}
}
