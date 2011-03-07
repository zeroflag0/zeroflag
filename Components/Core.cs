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
		public Core()
		{
			this.LogModule = new zeroflag.Components.Logging.LogModule();

			//for ( int i = 0; i < 10; i++ )
			//{
			//    using ( this.PerformanceLog["Timing10ms"].Record )
			//    {
			//        DateTime start = DateTime.Now;
			//        DateTime end = start.AddMilliseconds( 10 );
			//        while ( DateTime.Now < end ) ;
			//    }
			//}
		}

		#region Properties

		#region Modules

		private ComponentCollection<Module> _Modules;

		/// <summary>
		/// All modules used by this core.
		/// </summary>
		public ComponentCollection<Module> Modules
		{
			//get { return _Modules ?? ( ModulesInitialize( _Modules = this.ModulesCreate ) ); }
			get
			{
				if (_Modules != null)
				{
					return _Modules;
				}
				else
				{
					return (ModulesInitialize(_Modules = this.ModulesCreate));
				}
			}
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
				var modules = _Modules = new ComponentCollection<Module>(this) {CoreBase = this};
				//this.LogModule = new zeroflag.Components.Logging.LogModule();
				return modules;
			}
		}

		protected virtual ComponentCollection<Module> ModulesInitialize(ComponentCollection<Module> modules)
		{
			modules.ItemAdded += item =>
			                     {
			                     	this.Log.Message("Modules.Add(" + item + ")");
			                     	if (!this.Inner.Contains(item))
			                     	{
			                     		this.Inner.Add(item);
			                     	}
			                     };
			modules.ItemRemoved += item =>
			                       {
			                       	this.Log.Verbose("Modules.Remove(" + item + ")");
			                       	while (this.Inner.Contains(item))
			                       	{
			                       		this.Inner.Remove(item);
			                       	}
			                       };
			modules.ItemChanged += (sender, oldModule, newModule) => modules.Sort((mod1, mod2) => object.ReferenceEquals(mod1, mod2) ? 0 : mod2.CompareTo(mod1));
			return modules;
		}

		protected override void InnerItemAdded(Component item)
		{
			base.InnerItemAdded(item);
			Module mod = item as Module;
			if (mod != null)
			{
				if (!this.Modules.Contains(mod))
				{
					this.Modules.Add(mod);
				}
			}
		}

		protected override void InnerItemRemoved(Component item)
		{
			base.InnerItemRemoved(item);

			Module mod = item as Module;
			if (mod != null)
			{
				while (this.Modules.Contains(mod))
				{
					this.Modules.Remove(mod);
				}
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
			get
			{
				//if ( _LogModule != null )
				return _LogModule;
				//else
				//    return _LogModule = this.LogModuleCreate;
				//return _LogModule ?? ( _LogModule = this.LogModuleCreate );
			}
			set
			{
				var current = _LogModule; // ?? this.LogModuleFind;
				if (value != current)
				{
					_LogModule = value;
					//Console.WriteLine( "New log module. (" + current + " => " + value + ")\n" + new System.Diagnostics.StackTrace() );
					//this.Log.Verbose( "New log module. (" + current + " => " + value + ")" );
					if (current != null)
					{
						this.Modules.Remove(current);
					}
					if (value != null)
					{
						this.Modules.Add(value);
					}
				}
			}
		}

		/// <summary>
		/// Searches the default/initial value for LogModule.
		/// The core's logging module.
		/// </summary>
		protected virtual Logging.LogModule LogModuleFind
		{
			get { return this.Modules.Find(m => m != null && typeof (zeroflag.Components.Logging.LogModule).IsAssignableFrom(m.GetType())) as zeroflag.Components.Logging.LogModule; }
		}

		/// <summary>
		/// Creates the default/initial value for LogModule.
		/// The core's logging module.
		/// </summary>
		protected virtual Logging.LogModule LogModuleCreate
		{
			get
			{
				//Console.WriteLine( "Creating log module." );
				//this.Log.Verbose( "Creating log module." );
				//var mod = this.LogModuleFind;
				//if ( mod == null )
				var mod = _LogModule = new zeroflag.Components.Logging.LogModule();
				return mod;
				//return this.LogModuleFind ?? ( this.LogModule = new zeroflag.Components.Logging.LogModule() );
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
					if (this.State == ModuleStates.Initializing)
					{
						this.Initialize();

						this.State = ModuleStates.Ready;
					}
					try
					{
						if (this.State == ModuleStates.Ready || this.State == ModuleStates.Running)
						{
							this.OnRun();
						}
					}
					catch (Exception exc)
					{
						this.Log.Error(exc);
					}

					if (!this.RecoverFromExceptions)
					{
						this.State = ModuleStates.Shutdown;
					}
				}
				catch (Exception exc)
				{
					this.Log.Error(exc);
				}

				this.Dispose();
			}
			catch (Exception exc)
			{
				this.Log.Error(exc);
			}
			this.State = ModuleStates.Disposed;
		}

		DateTime last = DateTime.Now;
		/// <summary>
		/// Run this core's main loop.
		/// </summary>
		protected virtual void OnRun()
		{
			System.Threading.AutoResetEvent wait = new System.Threading.AutoResetEvent(false);
			while (this.State == ModuleStates.Running || this.State == ModuleStates.Ready)
			{
				this.OnRunStep();
				wait.WaitOne(1);
			}
		}

		protected virtual void OnRunStep()
		{
				DateTime now = DateTime.Now;
				//using ( this.PerformanceLog["Core"].Record )
				this.Update( last - now );
				last = now;
		}

		#endregion Run

		#region PerformanceLog

		private zeroflag.Components.Logging.PerformanceLog _PerformanceLog;

		/// <summary>
		/// Loghelper for performance data.
		/// </summary>
		public zeroflag.Components.Logging.PerformanceLog PerformanceLog
		{
			get { return _PerformanceLog ?? (_PerformanceLog = this.PerformanceLogCreate); }
			//protected set
			//{
			//	if (_PerformanceLog != value)
			//	{
			//		//if (_PerformanceLog != null) { }
			//		_PerformanceLog = value;
			//		//if (_PerformanceLog != null) { }
			//	}
			//}
		}

		/// <summary>
		/// Creates the default/initial value for PerformanceLog.
		/// Loghelper for performance data.
		/// </summary>
		protected virtual zeroflag.Components.Logging.PerformanceLog PerformanceLogCreate
		{
			get
			{
				zeroflag.Components.Logging.PerformanceLog value = _PerformanceLog = new zeroflag.Components.Logging.PerformanceLog() {Log = this.Log, Outer = this};
				return value;
			}
		}

		#endregion PerformanceLog
	}
}