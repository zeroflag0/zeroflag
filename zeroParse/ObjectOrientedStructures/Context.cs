using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Parsing.ObjectOriented
{
	public class Context
	{

		#region AvailableItems
		private List<System.Type> _AvailableItems;

		/// <summary>
		/// StructureType types available for structure.
		/// </summary>
		public List<System.Type> AvailableItems
		{
			get { return _AvailableItems ?? ( _AvailableItems = this.AvailableItemsCreate ); }
		}

		/// <summary>
		/// Creates the default/initial value for AvailableItems.
		/// StructureType types available for structure.
		/// </summary>
		protected virtual List<System.Type> AvailableItemsCreate
		{
			get
			{
				return zeroflag.Reflection.TypeHelper.GetDerived( typeof( ItemType ) );
			}
		}

		#endregion AvailableItems

		public ItemType Parse( zeroflag.Parsing.ParserContext context, ItemType outer )
		{
			ItemType current = outer;
			if ( context != null )
			{
				//Console.WriteLine( "Parsing " + context + "..." );
				if ( context.Rule != null )
				{
					System.Type itemtype = context.Rule.StructureType;

					if ( itemtype != null )
					{
						current = (ItemType)zeroflag.Reflection.TypeHelper.CreateInstance( itemtype );
					}
					if ( current != null )
					{
						if ( current.Name == null )
							current.Name = context.Rule.Name ?? ( itemtype ?? typeof( ItemType ) ).Name;


						if ( context.Rule is zeroflag.Parsing.Terminal && context.Result != null )
						{
							if ( current.Value == null )
								current.Value = context.Result.Value;
							else
								current.Value += context.Result.Value;
						}
						if ( current.Index == null )
							current.Index = context.Result.Index;

						if ( current != null && outer != current )
						{
							current.Snippet = context.ToString();
							
							if ( outer != null )
							{
								outer.Inner.Add( current );
							}
						}
					}
				}

				foreach ( var inner in context.Inner )
				{
					this.Parse( inner, current );
				}
				//Console.WriteLine( "Finished " + context + "." );
				this.OnParsed( context );
			}
			return current;
		}


		#region event Parsed
		public delegate void ParsedHandler( zeroflag.Parsing.ParserContext value );

		private event ParsedHandler _Parsed;
		/// <summary>
		/// When an item finished parsing.
		/// </summary>
		public event ParsedHandler Parsed
		{
			add { this._Parsed += value; }
			remove { this._Parsed -= value; }
		}
		/// <summary>
		/// Call to raise the Parsed event:
		/// When an item finished parsing.
		/// </summary>
		protected virtual void OnParsed( zeroflag.Parsing.ParserContext value )
		{
			// if there are event subscribers...
			if ( this._Parsed != null )
			{
				// call them...
				this._Parsed( value );
			}
		}
		#endregion event Parsed

		public ItemType Parse( ParserContext context )
		{
			return this.Parse( context, null );
		}
	}
}
