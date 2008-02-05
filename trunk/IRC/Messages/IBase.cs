using System;
namespace zeroflag.IRC.Messages
{
	public interface IBase
	{
		System.Text.StringBuilder Generate(System.Text.StringBuilder builder);
		string Generate();
		void Parse(string value);
	}
}
