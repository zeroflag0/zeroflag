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
using zeroflag.Collections;
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
		[SerializerIgnore]
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

		#region Modules

		protected override Collection<Component> InnerCreate
		{
			get
			{
				var inner = base.InnerCreate;
				inner.ItemChanged += ( s, o, n ) => this.Modules = null;
				return inner;
			}
		}
		private List<Module> _Modules;

		/// <summary>
		/// Modules contained in this module.
		/// </summary>
		public List<Module> Modules
		{
			get { return _Modules ?? ( _Modules = this.ModulesCreate ); }
			protected set
			{
				if ( _Modules != value )
				{
					//if (_Modules != null) { }
					_Modules = value;
					//if (_Modules != null) { }
				}
			}
		}

		/// <summary>
		/// Creates the default/initial value for Modules.
		/// Modules contained in this module.
		/// </summary>
		protected virtual List<Module> ModulesCreate
		{
			get
			{
				return base.Inner.FindAll<Module>();
			}
		}

		#endregion Modules


		#endregion

		protected override void OnStateChanged( Component.ModuleStates oldvalue, Component.ModuleStates newvalue )
		{
			this.Log.Message( newvalue + " ( previously " + oldvalue + " )" );
			if ( (int)newvalue - (int)oldvalue > 1 || (int)newvalue < (int)oldvalue )
				this.Log.Warning( newvalue + " ( previously " + oldvalue + " ) out of order." );

			base.OnStateChanged( oldvalue, newvalue );
		}
		public void Shutdown()
		{
			if ( this.State != ModuleStates.Disposed )
			{
				this.Log.Message( "Requesting shutdown..." );
				this.State = ModuleStates.Shutdown;
			}
		}

#if VERBOSE
		System.Diagnostics.StackTrace _InitializeTrace;
		int _InitializeThread;
#endif
		public override void Initialize()
		{
			if ( this.State == ModuleStates.Ready || this.State == ModuleStates.Running )
			{
#if VERBOSE
				this.Log.Warning( "Already initialized. " + System.Threading.Thread.CurrentThread.ManagedThreadId + "\n" + new System.Diagnostics.StackTrace() + " [" + this.State + "] from " + _InitializeThread + "\n" + _InitializeTrace );
#else
				this.Log.Warning( "Already initialized. [" + this.State + "]" );
#endif
				return;
			}
#if VERBOSE
			else
			{
				_InitializeTrace = new System.Diagnostics.StackTrace();
				_InitializeThread = System.Threading.Thread.CurrentThread.ManagedThreadId;
			}
#endif
			this.Log.Message( "Initializing..." );
			for ( int retry = 0; ; retry++ )
			{
				try
				{
					this.Resort();
					base.Initialize();
					this.Resort();
					break;
				}
				catch ( Exception exc )
				{
					if ( retry < 4 )
						this.Log.Warning( exc );
					else
					{
						this.Log.Error( exc );
						throw;
					}
				}
			}
			this.Log.Message( "Initialized." );
		}

		protected virtual bool Resort()
		{
			if ( this.Modules.Count > 0 )
			{
				this.Modules.Sort();
				this.ResortOut();
			}
			return true;
		}
		[System.Diagnostics.Conditional( "VERBOSE" )]
		protected void ResortOut()
		{
			string dbg = "Resort()\n";

			foreach ( Module module in this.Modules )
				dbg += "\t" + module.Name + "\n";
			this.Log.Verbose( dbg );
		}

		protected override void OnInitializeInner()
		{
			Console.WriteLine( this + ".OnInitializeInner()" ); 
			bool all;
			List<Component> done = new List<Component>();
			int retry = 0;
			do
			{
				all = true;

				var components = this.Inner.ToArray();
				foreach ( var comp in components )
				{
					try
					{
						this.Log.Verbose( "Initializing " + comp + "..." );
						comp.Initialize();
						done.Add( comp );
					}
					catch ( Exception exc )
					{
						if ( retry < 4 )
							this.Log.Warning( exc );
						else
						{
							this.Log.Error( exc );
							throw;
						}
					}
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

		protected override void OnInitializePost()
		{
			base.OnInitializePost();
			this.Resort();
			this.State = ModuleStates.Ready;
		}

		public override void Update( TimeSpan timeSinceLastUpdate )
		{
			if ( this.State == ModuleStates.Running )
				try
				{
					base.Update( timeSinceLastUpdate );
				}
				catch ( Exception exc )
				{
					this.Log.Error( exc );
					this.State = ModuleStates.Shutdown;
				}
			else if ( this.State == ModuleStates.Ready )
				this.State = ModuleStates.Running;
			else if ( this.State == ModuleStates.Shutdown )
				this.Dispose();
			else
				this.Log.Warning( "Running in invalid state " + this.State + "." );
		}

		public override void Dispose()
		{
			if ( this.State == ModuleStates.Disposed )
			{
				this.Log.Warning( "Already disposed." );
				return;
			}
			this.Log.Message( "Disposing..." );
			this.Resort();
			base.Dispose();
			this.Log.Message( "Disposed." );
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
