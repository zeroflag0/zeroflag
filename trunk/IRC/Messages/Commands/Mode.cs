#region LGPL License
//********************************************************************
//	author:         Thomas "zeroflag" Kraemer
//	author email:   zeroflag@zeroflag.de
//	
//	Copyright (C) 2006-2008  Thomas "zeroflag" Kraemer
//	
//	license:	(LGPL)
//	
//		This library is free software; you can redistribute it and/or
//		modify it under the terms of the GNU Lesser General Public
//		License as published by the Free Software Foundation; either
//		version 2.1 of the License, or (at your option) any later version.
//
//		This library is distributed in the hope that it will be useful,
//		but WITHOUT ANY WARRANTY; without even the implied warranty of
//		MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//		Lesser General Public License for more details.
//
//		You should have received a copy of the GNU Lesser General Public
//		License along with this library; if not, write to the Free Software
//		Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
//
//		http://www.gnu.org/licenses/lgpl.html#TOC1
//
//*********************************************************************
#endregion LGPL License

using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.IRC.Messages.Commands
{
	public class Mode : Command
	{
		string _Target;

		public string Target
		{
			get { return _Target; }
			set { _Target = value; }
		}

		string _Modification;

		public string Modification
		{
			get { return _Modification; }
			set { _Modification = value; }
		}

		public override void ParseAssign(string[] parameters)
		{
			this.Target = parameters[0];
			this.Modification = parameters[1];
		}

		protected override StringBuilder GenerateParams(StringBuilder builder)
		{
			return builder.Append(this.Target).Append(" ").Append(this.Modification);
		}
	}
}
