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