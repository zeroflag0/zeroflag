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
///		<id>$Id: Core.cs 60 2008-12-05 20:21:08Z zeroflag $</id>
///	</file>
#endregion SVN Version Information

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace zeroflag.Components
{
	public abstract class Core : Module, zeroflag.Components.ICore
	{
		#region Properties

		#region Modules
		private ComponentCollection<Module> _Modules;

		/// <summary>
		/// All modules used by this core.
		/// </summary>
		public ComponentCollection<Module> Modules
		{
			get { return _Modules ?? ( _Modules = this.ModulesCreate ); }
			protected set { _Modules = value; }
		}

		/// <summary>
		/// Creates the default/initial value for Modules.
		/// All modules used by this core.
		/// </summary>
		protected virtual ComponentCollection<Module> ModulesCreate
		{
			get
			{
				var modules = _Modules = new ComponentCollection<Module>( this ) { CoreBase = this };
				//modules.ItemAdded += item => item.Core = this;
				//modules.ItemRemoved += item =>
				//{
				//};
				modules.ItemChanged += ( sender, oldModule, newModule ) => modules.Sort( ( mod1, mod2 ) => object.ReferenceEquals( mod1, mod2 ) ? 0 : mod2.CompareTo( mod1 ) );
				//this.LogModule = new zeroflag.Components.Logging.LogModule();
				return modules;
			}
		}

		#endregion Modules

		#region LogModule
		private Logging.LogModule _LogModule;

		/// <summary>
		/// The core's logging module.
		/// </summary>
		public Logging.LogModule LogModule
		{
			get { return _LogModule ?? ( _LogModule = this.LogModuleCreate ); }
			set
			{
				var current = _LogModule ?? this.LogModuleFind;
				if ( value != current )
				{
					if ( current != null )
						this.Modules.Remove( current );
					if ( value != null )
						this.Modules.Add( value );
				}
			}
		}

		/// <summary>
		/// Searches the default/initial value for LogModule.
		/// The core's logging module.
		/// </summary>
		protected virtual Logging.LogModule LogModuleFind
		{
			get
			{
				return this.Modules.Find( m => m != null && typeof( zeroflag.Components.Logging.LogModule ).IsAssignableFrom( m.GetType() ) ) as zeroflag.Components.Logging.LogModule;
			}
		}

		/// <summary>
		/// Creates the default/initial value for LogModule.
		/// The core's logging module.
		/// </summary>
		protected virtual Logging.LogModule LogModuleCreate
		{
			get
			{
				return this.LogModuleFind ?? ( this.LogModule = new zeroflag.Components.Logging.LogModule() );
			}
		}

		#endregion LogModule


		#endregion

		#region Run
		public void Run()
		{
			try
			{
				try
				{
					if ( this.State == ModuleStates.Initializing )
					{
						this.Initialize();

						this.State = ModuleStates.Ready;
					}
					try
					{
						if ( this.State == ModuleStates.Ready || this.State == ModuleStates.Running )
							this.OnRun();
					}
					catch ( Exception exc )
					{
						this.Log.Error( exc );
					}
					this.State = ModuleStates.Shutdown;

				}
				catch ( Exception exc )
				{
					this.Log.Error( exc );
				}

				this.Dispose();
			}
			catch ( Exception exc )
			{
				this.Log.Error( exc );
			}
			this.State = ModuleStates.Disposed;
		}

		/// <summary>
		/// Run this core's main loop.
		/// </summary>
		protected virtual void OnRun()
		{
			DateTime last = DateTime.Now;
			DateTime now;
			System.Threading.AutoResetEvent wait = new System.Threading.AutoResetEvent( false );
			while ( this.State == ModuleStates.Running || this.State == ModuleStates.Ready )
			{
				now = DateTime.Now;
				this.Update( last - now );
				last = now;
				wait.WaitOne( 1 );
			}
		}

		#endregion Run



	}
}
