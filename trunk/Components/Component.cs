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
	public abstract class Component
		: IDisposable
		, IComponent<Core>
		, IEquatable<Component>
#if !SILVERLIGHT
, System.ComponentModel.IComponent
#endif
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
			this.RegisterOuter( oldvalue, newvalue );
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
			if ( item != null && item.Outer == null )
			{
				item.Outer = this;
			}
		}

		protected virtual void InnerItemRemoved( Component item )
		{
			if ( item != null && item.Outer == this )
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

		#region State

		public enum ModuleStates
		{
			/// <summary>
			/// The module hasn't been initialized yet.
			/// </summary>
			Initializing,
			/// <summary>
			/// The module is ready to run.
			/// </summary>
			Ready,
			/// <summary>
			/// The module is running.
			/// </summary>
			Running,
			/// <summary>
			/// The module is shutting down.
			/// </summary>
			Shutdown,
			/// <summary>
			/// The module has completely shut down.
			/// </summary>
			Disposed,
		}

		private ModuleStates _State;

		/// <summary>
		/// This module's state.
		/// </summary>
		public ModuleStates State
		{
			get { return _State; }
			set
			{
				if ( _State != value )
				{
					this.OnStateChanged( _State, _State = value );
					this.StateChangeInner( value );
				}
			}
		}

		protected virtual void StateChangeInner( ModuleStates value )
		{
			//foreach ( Component comp in this.Inner )
			//{
			//    comp.State = value;
			//}
		}

		#region StateChanged event
		public delegate void StateChangedHandler( object sender, ModuleStates oldvalue, ModuleStates newvalue );

		private event StateChangedHandler _StateChanged;
		/// <summary>
		/// Occurs when State changes.
		/// </summary>
		public event StateChangedHandler StateChanged
		{
			add { this._StateChanged += value; }
			remove { this._StateChanged -= value; }
		}

		/// <summary>
		/// Raises the StateChanged event.
		/// </summary>
		protected virtual void OnStateChanged( ModuleStates oldvalue, ModuleStates newvalue )
		{
			// if there are event subscribers...
			if ( this._StateChanged != null )
			{
				// call them...
				this._StateChanged( this, oldvalue, newvalue );
			}
		}
		#endregion StateChanged event
		#endregion State

		#region Initialize
		public virtual void Initialize()
		{
			if ( this.State == ModuleStates.Ready || this.State == ModuleStates.Running )
				return;
			this.OnInitialize();

			this.OnInitializeInner();

			this.OnInitializePost();

			this.OnInitialized( this );

			this.State = ModuleStates.Ready;
		}

		protected virtual void OnInitializeInner()
		{
			bool all;
			List<Component> done = new List<Component>();
			do
			{
				all = true;

				var components = this.Inner.ToArray();
				foreach ( var comp in components )
				{
					comp.Initialize();
					done.Add( comp );
				}
				foreach ( var comp in this.Inner )
					if ( !done.Contains( comp ) )
					{
						all = false;
						break;
					}
			}
			while ( !all );
		}
		/// <summary>
		/// Initializing. (before any inner components are initalized. (before any inner components are initialized; use <see cref="PostInitialize"/> if you need to wait for inner components to initialize)
		/// </summary>
		protected virtual void OnInitialize() { }
		/// <summary>
		/// After this and any inner componets are initialized. (after any inner components were initialized; use <see cref="OnInitialize"/> if you need to act earlier)
		/// </summary>
		protected virtual void OnInitializePost() { }

		#region event Initialized
		private event Action<Component> _Initialized;
		/// <summary>
		/// After the component finished initialization.
		/// </summary>
		public event Action<Component> Initialized
		{
			add { this._Initialized += value; }
			remove { this._Initialized -= value; }
		}
		/// <summary>
		/// Call to raise the Initialized event:
		/// After the component finished initialization.
		/// </summary>
		protected virtual void OnInitialized( Component component )
		{
			// if there are event subscribers...
			if ( this._Initialized != null )
			{
				// call them...
				this._Initialized( component );
			}
		}
		#endregion event Initialized
		#endregion Initialize

		#region Update
		#region LastRun

		private DateTime _LastUpdate = DateTime.Now;

		/// <summary>
		/// When the module was last updated...
		/// </summary>
		public DateTime LastUpdate
		{
			get { return _LastUpdate; }
			set
			{
				if ( _LastUpdate != value )
				{
					_LastUpdate = value;
				}
			}
		}

		#endregion LastRun

		public virtual void Update()
		{
			this.Update( DateTime.Now - this.LastUpdate );
		}

		public virtual void Update( TimeSpan timeSinceLastUpdate )
		{
			DateTime now = DateTime.Now;
			try
			{
				this.OnUpdate( timeSinceLastUpdate );
				this.OnUpdateInner( timeSinceLastUpdate );
			}
			finally
			{
				this.LastUpdate = now;
			}
			this.OnUpdatePost( timeSinceLastUpdate );
			this.OnUpdated( this, timeSinceLastUpdate );
		}

		protected virtual void OnUpdateInner( TimeSpan timeSinceLastUpdate )
		{
			foreach ( var comp in this.Inner.ToArray() )
			{
				//if ( comp.State != ModuleStates.Disposed )
				comp.Update( timeSinceLastUpdate );
			}
		}
		/// <summary>
		/// Updating. (before any inner components are initalized. (before any inner components are updated; use <see cref="PostUpdate"/> if you need to wait for inner components to update)
		/// </summary>
		protected virtual void OnUpdate( TimeSpan timeSinceLastUpdate ) { }
		/// <summary>
		/// After this and any inner componets are updated. (after any inner components were updated; use <see cref="OnUpdate"/> if you need to act earlier)
		/// </summary>
		protected virtual void OnUpdatePost( TimeSpan timeSinceLastUpdate ) { }

		#region event Updated
		private event Action<Component, TimeSpan> _Updated;
		/// <summary>
		/// After the component completes an update.
		/// </summary>
		public event Action<Component, TimeSpan> Updated
		{
			add { this._Updated += value; }
			remove { this._Updated -= value; }
		}
		/// <summary>
		/// Call to raise the Updated event:
		/// After the component completes an update.
		/// </summary>
		protected virtual void OnUpdated( Component component, TimeSpan timeSinceLastUpdate )
		{
			// if there are event subscribers...
			if ( this._Updated != null )
			{
				// call them...
				this._Updated( component, timeSinceLastUpdate );
			}
		}
		#endregion event Updated
		#endregion Update

		#region Dispose

		/// <summary>
		/// Disposing! (after any inner components were disposed; use <see cref="PreDispose"/> if you need to act earlier)
		/// </summary>
		protected virtual void OnDisposePost() { }
		/// <summary>
		/// Before this or any inner components get disposed.
		/// </summary>
		protected virtual void OnDispose() { }

		public virtual void Dispose()
		{
			if ( this.State == ModuleStates.Disposed )
				return;
			this.OnDispose();
			this.Dispose( true );
			this.OnDisposeInner();
			this.OnDisposePost();
			this.State = ModuleStates.Disposed;
			this.IsDisposed = true;
		}

		protected virtual void OnDisposeInner()
		{
			foreach ( var comp in this.Inner )
			{
				comp.Dispose();
			}
		}

		/// <summary>
		/// Dummy dispose to make components compatible to System.ComponentModel.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose( bool disposing )
		{
		}


		#region IsDisposed

		private bool _IsDisposed = false;

		/// <summary>
		/// Whether this component is disposed or not.
		/// </summary>
		public bool IsDisposed
		{
			get { return _IsDisposed; }
			set
			{
				if ( _IsDisposed != value )
				{
					this.OnHasDisposed( _IsDisposed, _IsDisposed = value );
				}
			}
		}

		#region Disposed event
		public delegate void DisposedHandler( object sender, bool oldvalue, bool newvalue );

		private event DisposedHandler _HasDisposed;
		/// <summary>
		/// Occurs when IsDisposed changes.
		/// </summary>
		public event DisposedHandler HasDisposed
		{
			add { this._HasDisposed += value; }
			remove { this._HasDisposed -= value; }
		}

		/// <summary>
		/// Raises the Disposed event.
		/// </summary>
		protected virtual void OnHasDisposed( bool oldvalue, bool newvalue )
		{
			// if there are event subscribers...
			if ( this._HasDisposed != null )
			{
				// call them...
				this._HasDisposed( this, oldvalue, newvalue );
			}
		}


		#endregion Disposed event
		#endregion IsDisposed

		#endregion Dispose

		#region IComponent<ICore> Members

		#region Core

		public virtual Core CoreBase
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

		#region IEquatable<Component> Members

		public bool Equals( Component other )
		{
			return object.ReferenceEquals( this, other );
		}

		#endregion

		#region IComponent Members
#if !SILVERLIGHT
		public event EventHandler Disposed
		{
			add { }
			remove { }
		}

		#region Site
		private System.ComponentModel.ISite _Site;

		/// <summary>
		/// Site
		/// </summary>
		public System.ComponentModel.ISite Site
		{
			get { return _Site; }
			set
			{
				if ( _Site != value )
				{
					_Site = value;
				}
			}
		}

		#endregion Site

#endif
		#endregion

	}
}
