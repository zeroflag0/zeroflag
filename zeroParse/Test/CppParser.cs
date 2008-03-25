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
			this.WhiteSpace = this.WhiteSpace.Inner | newline;// | (Rule)'\r';

			//this.WhiteSpace.Name = "\\s";
			Rule nothing = new Nothing();
			Rule any = new Anything();

			Rule
				space = this.WhiteSpace,
				letter = this.Letter;

			// comments...
			Rule comment = "comment" %
				("/*" & ~+!(Rule)"*/" & "*/") |
				("//" & ~+!newline ^ newline);

			// preprocessor...
			Rule preprocessor = "preprocessor" %
				(
				"#" & ~+!newline ^ newline
				);

			this.WhiteSpace = (preprocessor | comment | this.WhiteSpace.Inner);
			space = this.WhiteSpace;
			space.Ignore = true;

			Rule expression = new Rule() { Name = "expression" };
			Rule functionCall = new Rule() { Name = "call" };
			Rule assign = new Rule("assign");

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
			Rule stringValue = "string" % (quote2 & ~+((esc & !newline) | !(esc | quote2)) & quote2);
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
				(idHead ^ ~+idTail);

			Rule scopeType = "::";
			Rule type = new Rule() { Name = "type" };
			Rule types = "typeList" %
				(type & ~+("," & type));
			Rule pointers = "pointer" %
				+((Rule)"*" | "&" | "[]");
			Rule typeTail = "typeTail" %
				(pointers);
			Rule genericTypeName = "generic" %
				(id & ~("<" & types & ">"));

			Rule typeCast = "cast" %
				("(" & type & ")");
			Rule typeSimple = "simpleType" %
				(genericTypeName | id);

			type.Inner = (typeSimple & ~+(scopeType & typeSimple) ^ ~(~space ^ typeTail));
			#endregion

			#region Variables
			Rule variablePrototype = "variablePrototype" %
				(type ^ space & id);
			Rule variableDeclaration = "variable" %
				(type ^ space ^ (assign | id) & ";");

			Rule instance = "instance" % (
				//(id & ~+(((Rule)"." | "->") & id)) |
				(~(typeCast ^ ~space) ^
				(
				(type | id) & ~+(((Rule)"." | "->") & id))
				));
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

			#region Old
#if OLD
			//<Op Pointer> ::= <Op Pointer> '.' <Value>
			//               | <Op Pointer> '->' <Value>
			//               | <Op Pointer> '[' <Expr> ']'
			//               | <Value>
			Rule opPointer = new Rule("opPointer");
			opPointer.Inner =
				//((type | instance) & (
				//(opPointer & "." & value) |
				//(opPointer & "->" & value) |
				//(opPointer & "[" & expression & "]"))
				//) | 
				instance |value;

			Rule primaryOperators = "primaryOps" %
				(((Rule)"++" | "--"));

			Rule opPrimary = new Rule("opPrimary");
			opPrimary.Inner =
				(
				(opPointer & primaryOperators) |
				("new" ^ space & ((type & ~functionCall) | (~type & functionCall))) |
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
				(((Rule)"!" | "~" | "+" | "-" | "*" | "&" | "++" | "--"));

			Rule opUnary = new Rule("opUnary");
			opUnary.Inner =
				((unaryOperatorsPre & (instance | opPrimary)) |
				((Rule)"sizeof" & "(" & type & ")") |
				((Rule)"sizeof" & "(" & id & pointers & ")") |
				(opPrimary));



			//<Op Mult>    ::= <Op Mult> '*' <Op Unary>
			//               | <Op Mult> '/' <Op Unary>
			//               | <Op Mult> '%' <Op Unary>
			//               | <Op Unary>
			Rule opMpy = "opMpy" %
				(
				(+((instance | opUnary) & ((Rule)"*" | "/" | "%")) & opUnary) |
				opUnary);

			//<Op Add>     ::= <Op Add> '+' <Op Mult>
			//               | <Op Add> '-' <Op Mult>
			//               | <Op Mult>
			Rule opAdd = "opAdd" %  //any |
				(
				(+((instance | opMpy) & ((Rule)"+" | "-")) & opMpy) |
				opMpy);


			// Shift operators << >>
			Rule opShift = "opShift" %
				(
				(+((instance | opAdd) & ((Rule)"<<" | ">>")) & opAdd) |
				opAdd);


			Rule compareOperator = "compareOperator" %
				(((Rule)"<" | ">" | "<=" | ">=" | "==" | "!="));
			Rule compare = "compare" %
				(
				((instance | value) & compareOperator & value) |
				opShift);


			Rule opLogAND = "opLogAND" %
				(
				(+((instance | compare) & ((Rule)"&")) & compare) |
				compare);

			Rule opLogXOR = "opLogXOR" %
				(
				(+((instance | opLogAND) & ((Rule)"&")) & opLogAND) |
				opLogAND);

			Rule opLogOR = "opLogOR" %
				(
				(+((instance | opLogXOR) & ((Rule)"&")) & opLogXOR) |
				opLogXOR);

			Rule opCondAND = "opCondAND" %
				(
				(+((instance | opLogOR) & ((Rule)"&")) & opLogOR) |
				opLogOR);
			Rule opCondOR = "opCondOR" %
				(
				(+((instance | opCondAND) & ((Rule)"&")) & opCondAND) |
				opCondAND);

			Rule opConditional = "opConditional" %
				(
				(((instance | opCondOR) & (Rule)"?" & expression & ":" & expression)) |
				opCondOR);

			Rule ops = "ops" %
				new Debug(opConditional);


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
				(((Rule)"=" | "+=" | "-=" | "*=" | "/=" | "^=" | "&=" | "|=" | ">>=" | "<<="));
			Rule assign = new Rule("assign");
			assign.Inner =
				(instance & assignOperator & (value | assign));

			//<Expr>       ::= <Expr> ',' <Op Assign>
			//               | <Op Assign>
			expression.Inner = (ops | functionCall | assign);

#endif
			#endregion Old
			Rule value = "value" %
				(~(typeCast & space) ^ (hexValue | realValue | intValue | stringValue | charValue | functionCall | instance | id | ("(" & expression & ")")));

			{
				#region Operators
				Rule primaryOperators = "primaryOp" %
					(((Rule)"++" | "--"));

				Rule newOp = "new" %
					("new" & functionCall);
				Rule typeofOp = "typeof" %
					((Rule)"typeof" & "(" & type & ")");

				Rule unaryOperators = "unaryOp" %
					(((Rule)"!" | "~" | "+" | "-" | "*" | "&" | "++" | "--"));

				Rule mpyOperators = "mpyOp" %
					((Rule)"*" | "/" | "%");
				Rule addOperators = "addOp" %
					((Rule)"+" | "-");
				Rule shiftOperators = "shiftOp" %
					((Rule)">>" | "<<");

				Rule compareOperators = "compareOp" %
					((Rule)">" | ">=" | "<" | "<=");
				Rule equalOperators = "equalOp" %
					((Rule)"==" | "!=");

				Rule assignOperator = "assignOp" %
					(((Rule)"=" | "+=" | "-=" | "*=" | "/=" | "^=" | "&=" | "|=" | ">>=" | "<<="));

				#endregion Operators


				Rule primary = "primary" %
					~(typeCast & space) ^ (newOp | typeofOp | new Debug(value));

				Rule unary = "unary" %
					(~unaryOperators & primary);

				Rule mpy = "mpy" %
					(unary ^ ~+(~space ^ mpyOperators & unary));
				Rule add = "add" %
					(mpy ^ ~+(~space ^ addOperators & mpy));

				Rule shift = "shift" %
					(add ^ ~+(~space ^ shiftOperators & add));

				Rule compare = "compare" %
					(shift ^ ~+(~space ^ compareOperators & shift));

				Rule equal = "equal" %
					(compare ^ ~+(~space ^ equalOperators & compare));

				Rule logicalAND = "logicAND" %
					(equal ^ ~+(~space ^ "&" & equal));
				Rule logicalXOR = "logicXOR" %
					(logicalAND ^ ~+(~space ^ "^" & logicalAND));
				Rule logicalOR = "logicOR" %
					(logicalXOR ^ ~+(~space ^ "|" & logicalXOR));

				Rule conditionalAND = "AND" %
					(logicalOR ^ ~+(~space ^ "&&" & logicalOR));
				Rule conditionalOR = "OR" %
					(conditionalAND ^ ~+(~space ^ "&&" & conditionalAND));

				Rule conditional = "?:" %
					(conditionalOR ^ ~+(~space ^ "?" & value & ":" & value));

				Rule ops = "op" %
					(conditional);

				assign.Inner =
					(instance & assignOperator & expression);

				expression.Inner = (functionCall | assign | ops);

			}
			#endregion

			#region Statements
			//Rule returnStatement = "return" %
			//    ("return" ^ ~(+space & expression) & ";");
			Rule statement = "statement" %
				(~((Rule)"return" ^ ~space) ^ expression & ";");

			Rule processingBlock = "block" %
				(nothing);
			#endregion

			#region Functions
			Rule functionParameterDecs = new Rule() { Name = "functionParameterDecs" };
			functionParameterDecs.Inner = (variablePrototype & ~+("," & variablePrototype));

			Rule constructorPrototype = "constructorPrototype" %
				//(~(type & scopeType) & (id | type) & "(" & ~(functionParameterDecs) & ")");
				(type & "(" & ~(functionParameterDecs) & ")");
			Rule functionPrototype = "functionPrototype" %
				(type ^ space ^ constructorPrototype);
			Rule functionDeclaration = "functionDeclaration" %
				(functionPrototype & ";");

			Rule functionBody = "functionBody" %
				~+(space |
				statement |
				variableDeclaration
				);

			Rule functionCallParameters = "parameters" %
				((expression | value) & ~+(',' & (expression | value)));
			functionCall.Inner = (instance & '(' & (functionCallParameters | nothing) & ')');

			Rule functionDefinition = "function" %
				(functionPrototype & "{" & functionBody & "}");
			#endregion

			#region Classes
			#region Inheritance
			Rule inheritItem = "inheritItem" %
				(functionCall | (type & ~functionCall) | id);
			Rule inherit = "inherit" %
				(":" & inheritItem & ~+("," & inheritItem));
			#endregion

			Rule classPrototype = "classProt" %
				(((Rule)"class" | "struct") ^ space & id);

			Rule classDeclaration = "classDec" %
				(classPrototype & ~inherit & ";");

			Rule classVisibility = "visiblity" %
				(((Rule)"public" | "protected" | "private") & ":");

			Rule virtua = "virtual" %
				~("virtual" ^ space);

			#region Constructor
			Rule constructorDeclaration = "constructorDeclaration" %
				(constructorPrototype & ~inherit & ";");
			Rule constructorDefinition = "constructor" %
				(constructorPrototype & ~inherit & "{" & functionBody & "}");
			Rule constructorDefinitionExtern = "constructorExt" %
				(constructorPrototype & ~inherit & "{" & functionBody & "}");
			#endregion Constructor

			#region Destructor
			Rule destructorDeclaration = "destructorDeclaration" %
				("~" & constructorDeclaration);
			Rule destructorDefinition = "destructorDefinition" %
				("~" & constructorDefinition);
			#endregion Destructor

			#region Body
			Rule classBody = "classBody" %
				(space |
				(virtua & (
				functionDeclaration | functionDefinition |
				constructorDeclaration | constructorDefinition |
				destructorDeclaration | destructorDefinition)) |
				variableDeclaration | classVisibility);
			#endregion Body

			Rule classDefinition = "class" %
				(classPrototype & ~inherit & "{" & +classBody & "}" ^ ~(Rule)";");
			#endregion

			Rule rootElement =
				(classDefinition | functionDefinition | variableDeclaration | functionDeclaration | constructorDefinitionExtern);
			Rule root = "root" %
				+((space) | rootElement);


			return root;
		}
	}
}
