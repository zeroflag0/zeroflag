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
	class Unknown : Command
	{
		string[] _Parameters;

		public string[] Parameters
		{
			get { return _Parameters; }
			set { _Parameters = value; }
		}

		string _Name;
		public override string Name
		{
			get
			{
				return _Name ?? base.Name;
			}
			set
			{
				_Name = value;
			}
		}

		public override void ParseAssign(string[] parameters)
		{
			this.Parameters = parameters;
		}

		protected override StringBuilder GenerateParams(StringBuilder builder)
		{
			foreach (string param in Parameters)
			{
				builder.Append(param).Append(" ");
			}
			return builder;
		}

		public override string ToString()
		{
			string value = this.GetType().Name + "[name='" + (this.Name ?? "<null>") + "', ";
			if (this.Parameters != null)
				for (int i = 0; i < this.Parameters.Length; i++)
				{
					value += i + "='" + this.Parameters[i] + "', ";
				}
			else
				value += "<null>";
			value = value.TrimEnd(',', ' ') + "]";

			return value;
		}
	}
}
