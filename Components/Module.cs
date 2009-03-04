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
///		<revision>$Rev: 60 $</revision>
///		<author>$Author: zeroflag $</author>
///		<id>$Id: Module.cs 60 2008-12-05 20:21:08Z zeroflag $</id>
///	</file>
#endregion SVN Version Information

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace zeroflag.Components
{
	public abstract class Module : Component, IComparable<Module>
	{
		#region Properties

		#region Name
		private string _Name;

		/// <summary>
		/// The module's name.
		/// </summary>
		public string Name
		{
			get { return _Name ?? ( _Name = this.NameCreate ); }
			set { _Name = value; }
		}

		/// <summary>
		/// Creates the default/initial value for Name.
		/// The module's name.
		/// </summary>
		protected virtual string NameCreate
		{
			get { return this.GetType().FullName; }
		}

		#endregion Name


		#region Log
		private Logging.Log _Log;

		/// <summary>
		/// This module's log.
		/// </summary>
		[zeroflag.Serialization.SerializerIgnore]
		public Logging.Log Log
		{
			get { return _Log ?? ( _Log = this.LogCreate ); }
			//set { _Log = value; }
		}

		/// <summary>
		/// Creates the default/initial value for Log.
		/// This module's log.
		/// </summary>
		protected virtual Logging.Log LogCreate
		{
			get
			{
				Logging.Log log = null;
				try
				{
					Logging.LogModule logmod = this.CoreBase.Modules.Find( m => m is Logging.LogModule ) as Logging.LogModule;
					log = new zeroflag.Components.Logging.Log( logmod, this.Name );
				}
				catch ( NullReferenceException exc )
				{
					log = new zeroflag.Components.Logging.Log( this.Name );
				}
				return log;
			}
		}

		#endregion Log

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
				}
			}
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
			this.Log.Verbose( newvalue + " ( previously " + oldvalue + " )" );
			if ( (int)newvalue - (int)oldvalue > 1 )
				this.Log.Warning( newvalue + " ( previously " + oldvalue + " ) out of order." );
			// if there are event subscribers...
			if ( this._StateChanged != null )
			{
				// call them...
				this._StateChanged( this, oldvalue, newvalue );
			}
		}
		#endregion StateChanged event
		#endregion State

		#endregion

		protected override void OnInitialize()
		{
			this.Log.Message( "OnInitialize()..." );
			base.OnInitialize();
			this.OnInitializing();
		}

		#region event Initializing
		public delegate void InitializingHandler();

		private event InitializingHandler _Initializing;
		/// <summary>
		/// While this module is initializing...
		/// </summary>
		public event InitializingHandler Initializing
		{
			add { this._Initializing += value; }
			remove { this._Initializing -= value; }
		}
		/// <summary>
		/// Call to raise the Initializing event:
		/// While this module is initializing...
		/// </summary>
		protected virtual bool OnInitializing()
		{
			// if there are event subscribers...
			if ( this._Initializing != null )
			{
				// call them...
				this._Initializing();
			}
			return ( this.State = ModuleStates.Running ) == ModuleStates.Running;
		}
		#endregion event Initializing

		public bool Run( TimeSpan timeSinceLastUpdate )
		{
			if ( this.State == ModuleStates.Initializing )
				if ( !this.OnInitializing() )
					this.State = ModuleStates.Initializing;

			if ( this.State == ModuleStates.Running )
				if ( !this.Update( timeSinceLastUpdate ) )
					this.State = ModuleStates.Shutdown;

			if ( this.State == ModuleStates.Shutdown )
				this.OnDispose();

			return this.State != ModuleStates.Disposed;
		}

		public bool Update( TimeSpan timeSinceLastUpdate )
		{
			return this.OnUpdating( timeSinceLastUpdate );
		}

		#region event Updating
		public delegate void UpdatingHandler( TimeSpan timeSinceLastUpdate );

		private event UpdatingHandler _Updating;
		/// <summary>
		/// While this module is updating...
		/// </summary>
		public event UpdatingHandler Updating
		{
			add { this._Updating += value; }
			remove { this._Updating -= value; }
		}
		/// <summary>
		/// Call to raise the Updating event:
		/// While this module is updating...
		/// </summary>
		protected virtual bool OnUpdating( TimeSpan timeSinceLastUpdate )
		{
			// if there are event subscribers...
			if ( this._Updating != null )
			{
				// call them...
				this._Updating( timeSinceLastUpdate );
			}
			return this.State == ModuleStates.Running;
		}
		#endregion event Updating

		protected internal virtual bool OnShutdown()
		{
			return ( this.State = ModuleStates.Disposed ) == ModuleStates.Disposed;
		}

		protected override void OnDispose()
		{
			if ( this.State != ModuleStates.Disposed )
			{
#if DISPOSING
				this.OnDisposing();
#endif
				if ( this.OnShutdown() )
					this.State = ModuleStates.Disposed;
			}
		}

		#region event Disposing
#if DISPOSING
		public delegate void DisposingHandler();

		private event DisposingHandler _Disposing;
		/// <summary>
		/// While this module is being disposed...
		/// </summary>
		public event DisposingHandler Disposing
		{
			add { this._Disposing += value; }
			remove { this._Disposing -= value; }
		}
		/// <summary>
		/// Call to raise the Disposing event:
		/// While this module is being disposed...
		/// </summary>
		protected virtual void OnDisposing()
		{
			// if there are event subscribers...
			if ( this._Disposing != null )
			{
				// call them...
				this._Disposing();
			}
		}
#endif
		#endregion event Disposing

		#region LastRun

		private DateTime _LastRun = DateTime.Now;

		/// <summary>
		/// When the module was last updated...
		/// </summary>
		public DateTime LastRun
		{
			get { return _LastRun; }
			set
			{
				if ( _LastRun != value )
				{
					_LastRun = value;
				}
			}
		}

		#endregion LastRun

		public virtual bool Run()
		{
			DateTime now = DateTime.Now;
			try
			{
				return this.Run( this.LastRun - now );
			}
			finally
			{
				this.LastRun = now;
			}

		}

		#region IComparable<Module> Members
		/// <summary>
		/// Compare two modules for order.
		/// This represents the execution order and can be used for dependency handling.
		/// e.g. the rendering module (should run last) will return 1 (greater than) for any other module.
		/// </summary>
		/// <param name="other"></param>
		/// <returns>
		/// Less than zero 
		///  This Module is less than the other Module. This module has a lower priority.
		/// Zero 
		///  This Module is equal to other. Equal priority.
		/// Greater than zero 
		///  This Module is greater than Module. Higher priority, executed first.
		/// </returns>
		public virtual int CompareTo( Module other )
		{
			return 0;
		}

		#endregion
	}
}
