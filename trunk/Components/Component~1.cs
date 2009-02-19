#region BSD license
/*
 * Copyright (c) 2008, Thomas "zeroflag" Kraemer. All rights reserved.
 * Copyright (c) 2008, Anders "anonimasu" Helin. All rights reserved.
 * Copyright (c) 2008, The zeroflag.Components.NET Team. All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without 
 * modification, are permitted provided that the following conditions are met:
 * 
 * Redistributions of source code must retain the above copyright notice, this
 * list of conditions and the following disclaimer.
 * Redistributions in binary form must reproduce the above copyright notice, 
 * this list of conditions and the following disclaimer in the documentation 
 * and/or other materials provided with the distribution.
 * Neither the name of the zeroflag.Components.NET Team nor the names of its contributors may 
 * be used to endorse or promote products derived from this software without 
 * specific prior written permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF
 * THE POSSIBILITY OF SUCH DAMAGE.
 */
#endregion BSD license

#region SVN Version Information
///	<file>
///		<!-- Last modification of this file: -->
///		<revision>$Rev$</revision>
///		<author>$Author$</author>
///		<id>$Id$</id>
///	</file>
#endregion SVN Version Information

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace zeroflag.Components
{
	public abstract class Component<CoreT> : Component, zeroflag.Components.IComponent<CoreT>
		where CoreT : Core
	{
		public delegate void CoreChangedHandler( object sender, CoreT oldvalue, CoreT newvalue );

		public override Core CoreBase
		{
			get
			{
				return this.Core;
			}
			set
			{
				this.Core = (CoreT)value;
			}
		}
		#region Core

		private CoreT _Core;

		/// <summary>
		/// The core this module belongs to.
		/// </summary>
		[SerializerIgnore]
		public CoreT Core
		{
			get { return _Core ?? this as CoreT ?? ( this.Outer != null ? this.Outer.CoreBase as CoreT : null ); }
			set
			{
				if ( _Core != value )
				{
					this.OnCoreChanged( _Core, _Core = value );
				}
			}
		}


		#region CoreChanged event

		private event Component<CoreT>.CoreChangedHandler _CoreChanged;
		/// <summary>
		/// Occurs when Core changes.
		/// </summary>
		public event Component<CoreT>.CoreChangedHandler CoreChanged
		{
			add { this._CoreChanged += value; }
			remove { this._CoreChanged -= value; }
		}

		/// <summary>
		/// Raises the CoreChanged event.
		/// </summary>
		protected virtual void OnCoreChanged( CoreT oldvalue, CoreT newvalue )
		{
			if ( newvalue != null )
			{
				if ( this.HasInner )
				{
					foreach ( var comp in this.Inner )
					{
						comp.CoreBase = newvalue;
					}
				}
				if ( this.Outer != null && this.Outer.CoreBase == null )
					this.Outer.CoreBase = newvalue;
			}
			// if there are event subscribers...
			if ( this._CoreChanged != null )
			{
				// call them...
				this._CoreChanged( this, oldvalue, newvalue );
			}
		}
		#endregion CoreChanged event
		#endregion Core

		protected override void InnerItemAdded( Component item )
		{
			if ( item != null )
			{
				if ( item is Component<CoreT> )
					( item as Component<CoreT> ).Core = this.Core;
				item.Outer = this;
			}
		}

	}
}
