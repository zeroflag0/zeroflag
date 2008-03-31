using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace zeroflag.Parsing.Structure
{
	public class Builder
	{
		private ParserContext _Source;
		private Item _Result;

		public Item Result
		{
			get { return _Result; }
			set { _Result = value; }
		}

		public ParserContext Source
		{
			get { return _Source; }
			set { _Source = value; }
		}

		public Item Build()
		{
			return this.Build(this.Source, new ItemNode() { Name = "root", Index = 0 });
		}

		protected virtual Item Build(ParserContext context, ItemNode parent)
		{
			if (context == null || context.Rule == null || context.Rule.Ignore)
				return null;

			Rule rule = context.Rule;
			//ParserContext context = source.Context;
			string name = null;

			if (context.Result != null)
			{
				Token source = context.Result;
				name = source.Name;
				if (rule is Terminal)
				{
					var result = new ItemLeaf();
					result.Value = source.Value;
					return result;
				}
			}
			if (rule is Or)
			{
				Item result = null;
				foreach (ParserContext inner in context.Inner)
				{
					if (null != (result = this.Build(inner, parent)))
						break;
				}
				return result;
			}
			{
				ItemNode result = null;

				if (name != null)
				{
					result = new ItemNode();
					result.Name = name;
				}
				result = result ?? parent;
				{
					Item sub;
					foreach (ParserContext inner in context.Inner)
					{
						if (null != (sub = this.Build(inner, result)))
						{
							result.Inner.Add(sub);
						}
					}
				}

				return result;
			}
		}
	}
}
