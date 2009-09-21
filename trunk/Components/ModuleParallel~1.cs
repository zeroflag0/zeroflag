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
	public abstract class ModuleParallel<CoreT> : Module<CoreT>, IComponent<CoreT>
		where CoreT : Core<CoreT>
	{
		#region Processor

		private zeroflag.Components.ITaskProcessor _Processor;

		/// <summary>
		/// The task processor used by this module.
		/// </summary>
		public zeroflag.Components.ITaskProcessor Processor
		{
			get { return _Processor ?? (_Processor = this.ProcessorCreate); }
			set
			{
				if (_Processor != value)
				{
					//if (_Processor != null) { }
					_Processor = value;
					//if (_Processor != null) { }
				}
			}
		}

		/// <summary>
		/// Creates the default/initial value for Processor.
		/// The task processor used by this module.
		/// </summary>
		protected virtual zeroflag.Components.ITaskProcessor ProcessorCreate
		{
			get
			{
				zeroflag.Components.ITaskProcessor value = _Processor = new zeroflag.Components.TaskProcessor(this);
				return value;
			}
		}

		#endregion Processor

		#region Interval

		private double? _Interval;

		/// <summary>
		/// The interval between updates in milliseconds.
		/// </summary>
		public double Interval
		{
			get { return _Interval ?? (_Interval = this.IntervalCreate) ?? 20; }
			//protected set
			//{
			//	if (_Interval != value)
			//	{
			//		//if (_Interval != null) { }
			//		_Interval = value;
			//		//if (_Interval != null) { }
			//	}
			//}
		}

		/// <summary>
		/// Creates the default/initial value for Interval.
		/// The interval between updates in milliseconds.
		/// </summary>
		protected virtual double? IntervalCreate
		{
			get
			{
				double? value = _Interval = 20;
				return value;
			}
		}

		#endregion Interval

		protected override void OnStateChanged(ModuleStates oldvalue, ModuleStates newvalue)
		{
			base.OnStateChanged(oldvalue, newvalue);
			if (newvalue == ModuleStates.Running)
			{
				this.OnUpdatePost(TimeSpan.Zero);
			}
			else if (newvalue == ModuleStates.Shutdown || newvalue == ModuleStates.Disposed)
			{
				this.Core.Shutdown();
			}
		}

		protected override void OnUpdate(TimeSpan timeSinceLastUpdate)
		{
			//base.OnUpdate( timeSinceLastUpdate );
		}

		protected override void OnUpdatePost(TimeSpan timeSinceLastUpdate)
		{
			this.Processor.Add(this.LastUpdate.AddMilliseconds(this.Interval), this.Update);
			base.OnUpdatePost(timeSinceLastUpdate);
		}

		public override void Initialize()
		{
			this.Processor.Add(base.Initialize);
			while (this.State != ModuleStates.Ready || this.State == ModuleStates.Running)
			{
				System.Threading.Thread.Sleep(1);
			}
		}

		protected override void Dispose(bool disposing)
		{
			this.Processor.Add(() => base.Dispose(disposing));
			//base.Dispose( disposing );
		}
	}
}