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
	public abstract class Component : IDisposable, IComponent<Core>
	{

		#region Outer

		private Component _Outer;

		/// <summary>
		/// The parent component containing this component.
		/// </summary>
		[SerializerIgnore]
		public Component Outer
		{
			get { return _Outer; }
			set
			{
				if ( _Outer != value )
				{
					this.OnOuterChanged( _Outer, value );
					this.OnOuterChanged( _Outer, _Outer = value );
				}
			}
		}

		#region OuterChanged event
		public delegate void OuterChangedHandler( object sender, Component oldvalue, Component newvalue );

		private event OuterChangedHandler _OuterChanged;
		/// <summary>
		/// Occurs when Outer changes.
		/// </summary>
		public event OuterChangedHandler OuterChanged
		{
			add { this._OuterChanged += value; }
			remove { this._OuterChanged -= value; }
		}

		protected virtual void RegisterOuter( Component oldvalue, Component newvalue )
		{
			if ( oldvalue != null )
			{
				oldvalue.Inner.Remove( this );
			}
			if ( newvalue != null )
			{
				if ( !newvalue.Inner.Contains( this ) )
					newvalue.Inner.Add( this );
			}
		}

		/// <summary>
		/// Raises the OuterChanged event.
		/// </summary>
		protected virtual void OnOuterChanged( Component oldvalue, Component newvalue )
		{
			// if there are event subscribers...
			if ( this._OuterChanged != null )
			{
				// call them...
				this._OuterChanged( this, oldvalue, newvalue );
			}
		}
		#endregion OuterChanged event

		protected O OuterSearch<O>()
			where O : Component
		{
			return this.Outer as O ?? ( this.Outer != null ? this.Outer.OuterSearch<O>() : null );
		}
		#endregion Outer


		#region Inner
		private zeroflag.Collections.Collection<Component> _Inner;

		/// <summary>
		/// Any components contained in this component.
		/// </summary>
		public zeroflag.Collections.Collection<Component> Inner
		{
			get { return _Inner ?? ( _Inner = this.InnerCreate ); }
			//set { _Inner = value; }
		}

		/// <summary>
		/// Creates the default/initial value for Inner.
		/// Any components contained in this component.
		/// </summary>
		protected virtual zeroflag.Collections.Collection<Component> InnerCreate
		{
			get
			{
				var inner = _Inner = new zeroflag.Collections.Collection<Component>();
				inner.ItemAdded += InnerItemAdded;
				inner.ItemRemoved += InnerItemRemoved;
				return inner;
			}
		}

		protected virtual void InnerItemAdded( Component item )
		{
			if ( item != null )
			{
				item.Outer = this;
			}
		}

		protected virtual void InnerItemRemoved( Component item )
		{
			if ( item != null )
			{
				item.Outer = null;
			}
		}

		/// <summary>
		/// Shortcut to check whether this component has any inner components - does not trigger lazy initialization.
		/// </summary>
		protected bool HasInner
		{
			get
			{
				return _Inner != null && this.Inner.Count > 0;
			}
		}

		#endregion Inner

		public void Initialize()
		{
			this.OnInitialize();
			foreach ( var comp in this.Inner )
			{
				comp.Initialize();
			}
			this.PostInitialize();
		}
		/// <summary>
		/// After this and any inner componets are initialized. (after any inner components were initialized; use <see cref="OnInitialize"/> if you need to act earlier)
		/// </summary>
		protected virtual void PostInitialize() { }
		/// <summary>
		/// Initializing. (before any inner components are initalized. (before any inner components were initialized; use <see cref="PostInitialize"/> if you need to wait for inner components to initialize)
		/// </summary>
		protected virtual void OnInitialize() { }

		#region IDisposable Members

		/// <summary>
		/// Disposing! (after any inner components were disposed; use <see cref="PreDispose"/> if you need to act earlier)
		/// </summary>
		protected virtual void OnDispose() { }
		/// <summary>
		/// Before this or any inner components get disposed.
		/// </summary>
		protected virtual void PreDispose() { }

		public void Dispose()
		{
			this.PreDispose();
			foreach ( var comp in this.Inner )
			{
				comp.Dispose();
			}
			this.OnDispose();
		}

		#endregion

		#region IComponent<ICore> Members

		#region Core

		protected internal virtual Core CoreBase
		{
			get
			{
				return ( (IComponent<Core>)this ).Core;
			}
			set
			{
				( (IComponent<Core>)this ).Core = value;
			}
		}

		private Core _Core;

		/// <summary>
		/// The core this module belongs to.
		/// </summary>
		[SerializerIgnore]
		Core IComponent<Core>.Core
		{
			get { return _Core ?? this as Core ?? ( this.Outer != null ? this.Outer.CoreBase : null ); }
			set
			{
				if ( _Core != value )
				{
					this.OnCoreChanged( _Core, _Core = value );
				}
			}
		}


		#region CoreChanged event

		private event Component<Core>.CoreChangedHandler _CoreChanged;

		/// <summary>
		/// Occurs when Core changes.
		/// </summary>
		event Component<Core>.CoreChangedHandler IComponent<Core>.CoreChanged
		{
			add { this._CoreChanged += value; }
			remove { this._CoreChanged -= value; }
		}

		/// <summary>
		/// Raises the CoreChanged event.
		/// </summary>
		protected virtual void OnCoreChanged( Core oldvalue, Core newvalue )
		{
			if ( newvalue != null )
			{
				if ( this.HasInner )
				{
					foreach ( var comp in this.Inner )
					{
						if ( comp is IComponent<Core> )
							( comp as IComponent<Core> ).Core = newvalue;
					}
				}
				if ( this.Outer != null && ( this.Outer is IComponent<Core> ) && ( this.Outer as IComponent<Core> ).Core == null )
					( this.Outer as IComponent<Core> ).Core = newvalue;
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


		#endregion
	}
}
