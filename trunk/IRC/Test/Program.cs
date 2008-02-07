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
using zeroflag.IRC;
using zeroflag.IRC.Messages;

namespace Test
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{

			}
			catch (Exception exc)
			{
				Console.WriteLine(exc);
			}
			finally
			{ }
		}
	}
}
//Peer peer = new Peer();

//string input = ":zeroflag!n=zeroflag@unaffiliated/zeroflag";
//peer.Parse(input);
//Console.WriteLine("input:  \t" + input + "\nresult: \t" + peer.ToString());

//input = ":echelon3!n=tsogna@241.c.006.mel.iprimus.net.au";
//peer.Parse(input);
//Console.WriteLine("input:  \t" + input + "\nresult: \t" + peer.ToString());

//input = "simmons.freenode.net";
//peer.Parse(input);
//Console.WriteLine("input:  \t" + input + "\nresult: \t" + peer.ToString());

//input = ":rTi!n=rti@dslb-088-073-002-139.pools.arcor-ip.net";
//peer.Parse(input);
//Console.WriteLine("input:  \t" + input + "\nresult: \t" + peer.ToString());


//Console.WriteLine("Message:");
//Message msg = new Message();

//input = ":rTi_!n=rti@dslb-088-073-002-139.pools.arcor-ip.net NICK :rTi\n";
//msg.Parse(input);
//Console.WriteLine("input:  \t" + input.Replace("\n", "") + "\nresult: \t" + msg.ToString().Replace("\n", ""));

//msg.Command = new zeroflag.IRC.Messages.Commands.Nick("test");
//Console.WriteLine("generate\t" + msg.ToString().Replace("\n", "") + "\nresult: \t" + msg.Generate().Replace("\n", ""));

//input = ":ircd.gimp.org 353 zeroflag_ = #mono :zeroflag_ whitemice ptlo evarlast Wagoo lluis_ ";
//msg.Parse(input);
//Console.WriteLine("input:  \t" + input.Replace("\n", "") + "\nresult: \t" + msg.ToString().Replace("\n", ""));