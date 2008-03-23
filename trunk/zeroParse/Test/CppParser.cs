using System;
using System.Collections.Generic;
using zeroParse;
using System.Text;

namespace Test
{
	class CppParser : zeroParse.Parser
	{
		public override zeroParse.Rule CreateRules()
		{
			Rule newline = @"\n" %
				(Rule)"\r\n" | "\n" | "\0";
			newline.Ignore = true;
			this.WhiteSpace |= newline;// | (Rule)'\r';
			this.WhiteSpace = +this.WhiteSpace;
			this.WhiteSpace.Ignore = true;

			//this.WhiteSpace.Name = "\\s";

			Rule nl = newline,
				space = this.WhiteSpace,
				letter = this.Letter;

			// comments...
			Rule comment = "comment" %
				("/*" & ~+!(Rule)"*/" & "*/") |
				("//" & +!nl & nl);

			// preprocessor...
			Rule preprocessor = "preprocessor" %
				(
				"#" & +!nl & nl
				);

			Rule fill = "fill" % +(preprocessor | comment | space);
			fill.Ignore = true;
			space = fill;

			#region Values
			// strings...
			Rule esc = "\\";
			Rule quote2 = "\"" % (Rule)'"';
			Rule quote = "'" % (Rule)'\'';
			Rule nonescapedQuote = (!esc & quote2);
			Rule stringValue = "string" %
				(
				quote2 & ~+!(nonescapedQuote | nl) & ~!(quote2 | nl) & quote2
				);

			// char...
			Rule charValue = "char" %
				(quote & ((esc & !nl) | !quote) & quote);

			Rule digit = "digit" %
				((Rule)"0123456789".ToCharArray());

			// int...
			Rule intValue = "int" %
				+digit;

			// real...
			Rule realValue = "real" %
				(+digit & "." & +digit & ~((Rule)"f" | "F"));

			Rule hexDigit = "hexdigit" %
				(digit | "abcdef".ToCharArray() | "abcdef".ToUpper().ToCharArray());

			// hex...
			Rule hexValue = "hex" %
				("0x" & +hexDigit);
			#endregion Values

			#region Type Usage
			Rule idHead = letter | "_";
			Rule idTail = idHead | digit;

			Rule id = "id" %
				(idHead & ~+idTail);

			Rule scopeType = "::";
			Rule type = new Rule() { Name = "type" };
			Rule typeTail = "typeTail" %
				(~space & +((Rule)"*" | "&" | "[]"));
			Rule genericTypeName = "generic" %
				(~space & +id & ~(~space & "<" & type & ~+("," & type) & ">"));

			type.Inner = (~space & genericTypeName & ~+(scopeType & genericTypeName) & ~typeTail);
			#endregion

			#region Variables
			Rule variablePrototype = "variablePrototype" %
				(type & space & id);
			Rule variableDeclaration = "variable" %
				(variablePrototype & ~space & ";");

			#endregion

			#region Functions
			Rule functionParameterDecs = new Rule() { Name = "functionParameterDecs" };
			functionParameterDecs.Inner = (variablePrototype & ~(~space & "," & ~space & functionParameterDecs));
			Rule functionPrototype = "functionPrototype" %
				(type & space & id & ~space & "(" & ~functionParameterDecs & ")");
			Rule functionDeclaration = "functionDeclaration" %
				(functionPrototype & ~space & ";");

			Rule functionBody = "functionBody" %
				~+(
				variableDeclaration |
				fill);

			Rule function = "function" %
				(functionPrototype & ~space & "{" & functionBody);
			#endregion

			Rule rootElement =
				(function | variableDeclaration | functionDeclaration);
			Rule root = "root" %
				+(rootElement | (fill));
			//(charValue | stringValue) |
			//(realValue | hexValue | intValue)


			return root;
		}
	}
}
