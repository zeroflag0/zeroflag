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
	public class Core<Self> : Core
		where Self : Core<Self>
	{
		#region Properties

		#region Modules
		/// <summary>
		/// Creates the default/initial value for Modules.
		/// All modules used by this core.
		/// </summary>
		protected override ComponentCollection<Module> ModulesCreate
		{
			get
			{
				var modules = new ComponentCollection<Module, Self>( this ) { Core = this as Self };
				//modules.ItemAdded += item => item.Core = this;
				//modules.ItemRemoved += item =>
				//{
				//};
				//modules.ItemChanged += ModuleChanged;
				//modules.ItemChanged += ( sender, oldModule, newModule ) =>
				//{
				//    modules.Sort( ( mod1, mod2 ) => object.ReferenceEquals( mod1, mod2 ) ? 0 : mod2.CompareTo( mod1 ) );
				//};
				return modules;
			}
		}

		void ModuleChanged( object sender, Module oldvalue, Module newvalue )
		{
			Modules.Sort( ModuleComparison );
		}

		int ModuleComparison( Module a, Module b )
		{
			return object.ReferenceEquals( a, b ) ? 0 : b.CompareTo( a );
		}

		#endregion Modules

		#endregion

	}
}
