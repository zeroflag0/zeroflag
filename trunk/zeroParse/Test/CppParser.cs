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
			Rule nothing = new Nothing();
			Rule any = new Anything();

			Rule nl = newline,
				space = this.WhiteSpace,
				letter = this.Letter;

			// comments...
			Rule comment = "comment" %
				("/*" & ~+!(Rule)"*/" & "*/") |
				("//" & ~+!nl & nl);

			// preprocessor...
			Rule preprocessor = "preprocessor" %
				(
				"#" & ~+!nl & nl
				);

			Rule fill = "fill" % +(preprocessor | comment | space);
			fill.Ignore = true;
			space = fill;

			Rule expression = new Rule() { Name = "expression" };
			Rule functionCall = new Rule() { Name = "call" };

			#region Values
			// strings...
			Rule esc = "\\";
			Rule quote2 = "\"" % (Rule)'"';
			Rule quote = "'" % (Rule)'\'';
			Rule nonescapedQuote = (!esc & quote2);
			//Rule stringValue = "string" %
			//    (
			//    quote2 & ~+!(nonescapedQuote | nl) & ~!(quote2 | nl) & quote2
			//    );
			Rule stringValue = "string" % (quote2 & ~+((esc & !nl) | !(esc | quote2)) & quote2);
			//new RegexTerminal(@"");

			// char...
			Rule charValue = "char" %
				(quote & ((esc & any) | !quote) & quote);

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
			Rule types = "typeList" %
				(type & ~+("," & ~space & type));
			Rule pointers = "pointer" %
				+((Rule)"*" | "&" | "[]");
			Rule typeTail = "typeTail" %
				(~space & pointers);
			Rule genericTypeName = "generic" %
				(~space & id & ~(~space & "<" & types & ">"));

			Rule typeCast = "cast" %
				("(" & ~space & type & ~space & ")");
			Rule typeSimple = "simpleType" %
				(genericTypeName | id);
			type.Inner = (typeSimple & ~+(~space & scopeType & typeSimple) & ~(~space & typeTail));
			#endregion

			#region Variables
			Rule variablePrototype = "variablePrototype" %
				(type & space & id);
			Rule variableDeclaration = "variable" %
				(variablePrototype & ~space & ";");

			Rule instance = "instance" % (
				//(id & ~+(((Rule)"." | "->") & id)) |
				(+((id | type) & ~+((Rule)"." | "->")) & id)
				);
			#endregion

			#region Expressions
			//see: http://msdn2.microsoft.com/en-us/library/6a71f45d.aspx

			//<Value>      ::= OctLiteral
			//               | HexLiteral
			//               | DecLiteral
			//               | StringLiteral
			//               | CharLiteral
			//               | FloatLiteral
			//               | Id '(' <Expr> ')'
			//               | Id '(' ')'
			//               | Id
			//               | '(' <Expr> ')'
			Rule assign = new Rule("assign");
			Rule value = "value" %
				(~(typeCast & ~space) & (hexValue | realValue | intValue | stringValue | charValue | functionCall | instance | type | id | ("(" & ~space & expression & ~space & ")")));

			//<Op Pointer> ::= <Op Pointer> '.' <Value>
			//               | <Op Pointer> '->' <Value>
			//               | <Op Pointer> '[' <Expr> ']'
			//               | <Value>
			Rule opPointer = new Rule("opPointer");
			opPointer.Inner =
				(type & (
				(opPointer & ~space & "." & ~space & value) |
				(opPointer & ~space & "->" & ~space & value) |
				(opPointer & ~space & "[" & ~space & expression & ~space & "]"))
				) | value;

			Rule primaryOperators = "primaryOps" %
				(~space & ((Rule)"++" | "--") & ~space);

			Rule opPrimary = new Rule("opPrimary");
			opPrimary.Inner =
				(
				(opPointer & primaryOperators) |
				("new" & space & ((type & ~functionCall) | (~type & functionCall))) |
				opPointer
				);
			//<Op Unary>   ::= '!'    <Op Unary>
			//               | '~'    <Op Unary>
			//               | '-'    <Op Unary>
			//               | '*'    <Op Unary>
			//               | '&'    <Op Unary>
			//               | '++'   <Op Unary>
			//               | '--'   <Op Unary>
			//               | <Op Pointer> '++'
			//               | <Op Pointer> '--'
			//               | '(' <Type> ')' <Op Unary>   !CAST
			//               | sizeof '(' <Type> ')'
			//               | sizeof '(' ID <Pointers> ')'
			//               | <Op Pointer>
			Rule unaryOperatorsPre = "unaryOps" %
				(~space & ((Rule)"!" | "~" | "+" | "-" | "*" | "&" | "++" | "--") & ~space);

			Rule opUnary = new Rule("opUnary");
			opUnary.Inner = 
				((unaryOperatorsPre & opPrimary) |
				("sizeof" & ~space & "(" & ~space & type & ~space & ")") |
				("sizeof" & ~space & "(" & ~space & id & ~space & pointers & ~space & ")") |
				(opPrimary));



			//<Op Mult>    ::= <Op Mult> '*' <Op Unary>
			//               | <Op Mult> '/' <Op Unary>
			//               | <Op Mult> '%' <Op Unary>
			//               | <Op Unary>
			Rule opMpy = "opMpy" %
				((+(opUnary & ~space & ((Rule)"*" | "/" | "%") & ~space) & opUnary) | opUnary);

			//<Op Add>     ::= <Op Add> '+' <Op Mult>
			//               | <Op Add> '-' <Op Mult>
			//               | <Op Mult>
			Rule opAdd = "opAdd" %  //any |
				((+(opMpy & ~space & ((Rule)"+" | "-") & ~space) & opMpy) | opMpy);


			// Shift operators << >>
			Rule opShift = "opShift" %
				((+(opAdd & ~space & ((Rule)"<<" | ">>") & ~space) & opAdd) | opAdd);


			Rule compareOperator = "compareOperator" %
				(~space & ((Rule)"<" | ">" | "<=" | ">=" | "==" | "!=") & ~space);
			Rule compare = "compare" %
				((value & compareOperator & value) | opShift);


			Rule opLogAND = "opLogAND" %
				((+(compare & ~space & ((Rule)"&") & ~space) & compare) | compare);

			Rule opLogXOR = "opLogXOR" %
				((+(opLogAND & ~space & ((Rule)"&") & ~space) & opLogAND) | opLogAND);

			Rule opLogOR = "opLogOR" %
				((+(opLogXOR & ~space & ((Rule)"&") & ~space) & opLogXOR) | opLogXOR);

			Rule opCondAND = "opCondAND" %
				((+(opLogOR & ~space & ((Rule)"&") & ~space) & opLogOR) | opLogOR);
			Rule opCondOR = "opCondOR" %
				((+(opCondAND & ~space & ((Rule)"&") & ~space) & opCondAND) | opCondAND);

			Rule opConditional = "opConditional" %
				((+(opCondOR & ~space & (Rule)"?" & ~space & value & ":" & value)) | opCondOR);


			Rule ops = "ops" %
				(opConditional);

			
			//<Op Assign>  ::= <Op If> '='   <Op Assign>
			//               | <Op If> '+='  <Op Assign>
			//               | <Op If> '-='  <Op Assign>
			//               | <Op If> '*='  <Op Assign>
			//               | <Op If> '/='  <Op Assign>
			//               | <Op If> '^='  <Op Assign>
			//               | <Op If> '&='  <Op Assign>
			//               | <Op If> '|='  <Op Assign>
			//               | <Op If> '>>=' <Op Assign>
			//               | <Op If> '<<=' <Op Assign>
			//               | <Op If>
			Rule assignOperator = "assignOperator" %
				(~space & ((Rule)"=" | "+=" | "-=" | "*=" | "/=" | "^=" | "&=" | "|=" | ">>=" | "<<=") & ~space);
			assign.Inner =
				(instance & ~space & assignOperator & ~space & (assign | value));

			//<Expr>       ::= <Expr> ',' <Op Assign>
			//               | <Op Assign>
			expression.Inner = ops | assign | functionCall;

			//<Op If>      ::= <Op Or> '?' <Op If> ':' <Op If>
			//               | <Op Or>

			//<Op Or>      ::= <Op Or> '||' <Op And>
			//               | <Op And>

			//<Op And>     ::= <Op And> '&&' <Op BinOR>
			//               | <Op BinOR>

			//<Op BinOR>   ::= <Op BinOr> '|' <Op BinXOR>
			//               | <Op BinXOR>

			//<Op BinXOR>  ::= <Op BinXOR> '^' <Op BinAND>
			//               | <Op BinAND>

			//<Op BinAND>  ::= <Op BinAND> '&' <Op Equate>
			//               | <Op Equate>

			//<Op Equate>  ::= <Op Equate> '==' <Op Compare>
			//               | <Op Equate> '!=' <Op Compare>
			//               | <Op Compare>

			//<Op Compare> ::= <Op Compare> '<'  <Op Shift>
			//               | <Op Compare> '>'  <Op Shift>
			//               | <Op Compare> '<=' <Op Shift>
			//               | <Op Compare> '>=' <Op Shift>
			//               | <Op Shift>

			//<Op Shift>   ::= <Op Shift> '<<' <Op Add>
			//               | <Op Shift> '>>' <Op Add>
			//               | <Op Add>



			#endregion

			#region Statements
			Rule returnStatement = "return" %
				("return" & ~(+space & value) & ~space & ";");
			Rule statement = "statement" %
				(expression & ~space & ";") | returnStatement;

			Rule processingBlock = "block" %
				(nothing);
			#endregion

			#region Functions
			Rule functionParameterDecs = new Rule() { Name = "functionParameterDecs" };
			functionParameterDecs.Inner = (variablePrototype & ~+(~space & "," & ~space & variablePrototype));

			Rule constructorPrototype = "constructorPrototype" %
				(~(type & scopeType) & id & ~space & "(" & ~space & ~(functionParameterDecs & ~space) & ")");
			Rule functionPrototype = "functionPrototype" %
				(type & space & constructorPrototype);
			Rule functionDeclaration = "functionDeclaration" %
				(functionPrototype & ~space & ";");

			Rule functionBody = "functionBody" %
				~+(
				variableDeclaration |
				statement |
				fill);

			Rule functionCallParameters = "parameters" %
				((expression | value) & ~space & ~+(',' & ~space & (expression | value) & ~space));
			functionCall.Inner = (instance & ~space & '(' & ~space & (functionCallParameters | nothing) & ~space & ')');

			Rule functionDefinition = "function" %
				(functionPrototype & ~space & "{" & functionBody & "}");
			#endregion

			#region Classes
			Rule classPrototype = "classProt" %
				(((Rule)"class" | "struct") & space & id);

			Rule classDeclaration = "classDec" %
				(classPrototype & ~space & ";");

			Rule classVisibility = "visiblity" %
				(((Rule)"public" | "protected" | "private") & ~space & ":");

			Rule virtua = "virtual" %
				~("virtual" & space);
			Rule constructorDeclaration = "constructorDeclaration" %
				(constructorPrototype & ~space & ";");
			Rule constructorDefinition = "constructor" %
				(constructorPrototype & ~space & "{" & functionBody & "}");

			Rule destructorDeclaration = "destructorDeclaration" %
				("~" & ~space & constructorDeclaration);
			Rule destructorDefinition = "destructorDefinition" %
				("~" & ~space & constructorDefinition);

			Rule classBody = "classBody" %
				((virtua & (
				functionDeclaration | functionDefinition |
				constructorDeclaration | constructorDefinition |
				destructorDeclaration | destructorDefinition)) |
				variableDeclaration | classVisibility | fill);

			Rule classDefinition = "class" %
				(classPrototype & ~space & "{" & +classBody & "}");
			#endregion

			Rule rootElement =
				(classDefinition | functionDefinition | variableDeclaration | functionDeclaration);
			Rule root = "root" %
				+((fill) | statement);//rootElement);


			return root;
		}
	}
}
