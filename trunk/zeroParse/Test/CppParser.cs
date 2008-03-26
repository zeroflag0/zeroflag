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
				((Rule)"\n" | "\0");//"\r\n" | 
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
			Rule root = new Rule("root");
			Rule rootElement = new Rule("rootel");
			Rule type = new Rule() { Name = "type" };
			Rule classDefinition = new Rule("class");
			Rule value = new Rule("value");
			Rule structDefinition = new Rule("struct");
			Rule statement = new Rule("statement");


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
			Rule stringValue = "string" %
				(quote2 & ~+((esc & !newline) | !(esc | quote2) | (quote2 & quote2)) & quote2);
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


			Rule baseType = "typeB" %
				(
				//(Rule)"char" | "bool" | "short" | "int" | 
				("__int" ^ ~+digit) |
				("long" ^ ~(space & ((Rule)"long" | "double"))) |
				("unsigned" ^ ~(space & ((Rule)"int" | "short" | "long" | "char"))) | "float" | "double" | "char" |
				("signed" ^ ~(space & ((Rule)"char"))) |
				("enum" ^ ~(space & type)) |
				("struct" ^ ~(space & type)));

			Rule scopeType = "::";
			Rule types = "typeList" %
				(type & ~+("," & type));
			Rule pointers = "pointer" %
				+((Rule)"*" | "[]");
			Rule typeTail = "typeTail" %
				(pointers);

			Rule functionPointerTail = "func*tail" %
				((Rule)"(" & "*" & ")" & "(" & types & ")");

			Rule genericTypeName = "generic" %
				(id & ~("<" & types & ">"));

			Rule typeCast = "cast" %
				("(" & type & ")");
			Rule typeSimple = "simpleType" %
				(baseType | genericTypeName);

			type.Inner = ((~("const" ^ space) ^ ~+(~space ^ ((Rule)"*") ^ ~space) ^ ~(scopeType) & typeSimple & ~+(scopeType & typeSimple) ^ ~(~space ^ typeTail)) ^ ~(~space ^ functionPointerTail));

			Rule typedef = "typedef" %
				("typedef" ^ space ^ ((classDefinition | type) ^ +(~(~space & ",") ^ space & type ^ ~(~space & type))) & ";");
			#endregion

			#region Variables
			Rule indexer = "indexer" %
				(~space ^ "[" & expression & "]");

			Rule variablePrototype = "variablePrototype" %
				(type ^ ~space & id);
			Rule variableDeclaration = "variable" %
				(type ^ ~space ^ (assign | id) ^ ~indexer & ";");

			Rule instance = "instance" % (
				//(id & ~+(((Rule)"." | "->") & id)) |
				(~(typeCast ^ ~space) ^
				(
				(type | id) & ~+(((Rule)"." | "->") & id) ^ ~indexer)
				));
			#endregion

			#region Functions
			Rule functionParameterDecs = new Rule() { Name = "functionParameterDecs" };
			functionParameterDecs.Inner = ((variablePrototype & ~+("," & variablePrototype)) | types);

			Rule constructorPrototype = "constructorPrototype" %
				//(~(type & scopeType) & (id | type) & "(" & ~(functionParameterDecs) & ")");
				(type & "(" & ~(functionParameterDecs) & ")");
			Rule functionPrototype = "functionPrototype" %
				(type ^ space ^ constructorPrototype);
			Rule functionDeclaration = "functionDeclaration" %
				(functionPrototype & ";");

			Rule functionBody = new Rule("functionBody");
			functionBody.Inner =
				("{" & 
				~+(space |
				statement |
				variableDeclaration |
				functionBody
				)
				& "}");

			Rule functionCallParameters = "parameters" %
				((expression | value) & ~+(',' & (expression | value)));
			Rule functionCallTail = "functionCallT" %
				('(' & (functionCallParameters | nothing) & ')');
			functionCall.Inner = ((instance | (type & ~instance)) & functionCallTail);

			Rule functionDefinition = "functionDefinition" %
				(functionPrototype &  functionBody );
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

			value.Inner = (~(typeCast ^ ~space) ^ (hexValue | realValue | intValue | stringValue | charValue | functionCall | instance | id | ("(" & expression & ")")));

			#region Operators
			Rule primaryOperators = "primaryOp" %
				(((Rule)"++" | "--"));

			Rule newOp = "new" %
				("new" & (functionCall | (type & indexer)));
			Rule typeofOp = "typeof" %
				((Rule)"typeof" & "(" & type & ")");

			Rule unaryOperators = "unaryOp" %
				(((Rule)"!" | "~" | "+" | "-" | "*" | "++" | "--"));

			Rule mpyOperators = "mpyOp" %
				((Rule)"*" | "/" | "%");
			Rule addOperators = "addOp" %
				((Rule)"+" | "-");
			Rule shiftOperators = "shiftOp" %
				((Rule)">>" | "<<");

			Rule compareOperators = "compareOp" %
				((Rule)">=" | ">" | "<=" | "<");
			Rule equalOperators = "equalOp" %
				((Rule)"==" | "!=");

			Rule assignOperator = "assignOp" %
				(((Rule)"=" | "+=" | "-=" | "*=" | "/=" | "^=" | "&=" | "|=" | ">>=" | "<<="));

			#endregion Operators


			Rule primary = "primary" %
				~(typeCast & ~space) ^ (newOp | typeofOp | value);

			Rule unary = "unary" %
				(~unaryOperators & primary);

			Rule mpy = "mpy" %
				((unary & ~+(~space ^ mpyOperators & unary)));//| unary);
			Rule add = "add" %
				((mpy & ~+(~space ^ addOperators & mpy)));//| mpy);

			Rule shift = "shift" %
				((add & ~+(~space ^ shiftOperators & add)));//| add);

			Rule compare = "compare" %
				((shift & ~+(~space ^ compareOperators & shift)));//| shift);

			Rule equal = "equal" %
				((compare & ~+(~space ^ equalOperators & compare)));//| compare);

			Rule logicalAND = "logicAND" %
				((equal & ~+(~space ^ ("&" ^ -(Rule)"&") & equal)));//| equal);
			Rule logicalXOR = "logicXOR" %
				((logicalAND & ~+(~space ^ "^" & logicalAND)));//| logicalAND);
			Rule logicalOR = "logicOR" %
				((logicalXOR & ~+(~space ^ "|" & logicalXOR)));//| logicalXOR);

			Rule conditionalAND = "AND" %
				((logicalOR & ~+(~space ^ "&&" & logicalOR)));// | logicalOR);
			Rule conditionalOR = "OR" %
				((conditionalAND & ~+(~space ^ "||" & conditionalAND)));// | conditionalAND);

			Rule conditional = "?:" %
				(conditionalOR & ~("?" & value & ":" & value));// | conditionalOR);

			Rule ops = "op" %
				(conditional);

			assign.Inner =
				(instance & assignOperator & expression);

			expression.Inner = (assign | ops);

			#endregion

			#region Statements
			//Rule returnStatement = "return" %
			//    ("return" ^ ~(+space & expression) & ";");
			statement.Inner = ((~((Rule)"return" ^ ~space) ^ expression & ";") | typedef | variableDeclaration);

			#endregion

			#region Classes
			#region Inheritance
			Rule inheritItem = "inheritItem" %
				(functionCall | (type & ~functionCall) | id);
			Rule inherit = "inherit" %
				(":" & inheritItem & ~+("," & inheritItem));
			#endregion

			Rule classPrototype = "classProt" %
				(((Rule)"class" | "struct") ^ ~(space & id));


			Rule classDeclaration = "classDec" %
				(classPrototype & ~inherit & ";");

			Rule classVisibility = "visiblity" %
				(((Rule)"public" | "protected" | "private") & ":");

			Rule declaration = "decl" %
				~(((Rule)"virtual" | "explicit" | "implicit" | "static") ^ space);

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

			#region Class Body
			Rule classBody = "classBody" %
				(space |
				(declaration & (
				functionDeclaration | functionDefinition |
				constructorDeclaration | constructorDefinition |
				destructorDeclaration | destructorDefinition)) |
				variableDeclaration | classVisibility | root);
			#endregion Class Body

			structDefinition.Inner = ((Rule)"struct" & "{" & +classBody & "}" ^ ~(~space & instance));

			Rule structDef = "structdef" %
				(structDefinition ^ ~(~space & ";"));
			classDefinition.Inner =
				(structDefinition | (classPrototype & ~inherit & "{" & +classBody & "}"));// ^ ~(~space & instance)));
			Rule classDef = "class" %
				(classDefinition ^ ~(Rule)";");
			#endregion

			#region Enums
			Rule enumHead = "enumHead" %
				("enum" ^ ~(space & id));
			Rule enumItem = "enumItem" %
				((assign | id));
			Rule enumBody = "enumBody" %
				(+(~space & enumItem & ~space & ~(Rule)"," & ~space));
			Rule enumDef = "enum" %
				(enumHead & "{" & enumBody & "}" & ~(Rule)";");
			#endregion Enums

			#region Namespaces
			Rule namespaceBody = "namespaceBody" %
				(+(rootElement));
			Rule namespaceDefinition = "namespace" %
				("namespace" ^ ~(space & id) & "{" & namespaceBody & "}" & ~(Rule)";");
			#endregion Namespaces

			#region Compiler Hacks
			// some stupid C/C++ compiler(-specific) stuff to ignore...

			// it seems msvc has picked up [attributes]... just ignore them.
			Rule attribute = "attribute" %
				("[" & ((functionCall | type) ^ ~+(":" & (functionCall | type))) & "]");

			// __declspec... whatever...
			Rule declspec = "__declspec" %
				((Rule)"__declspec" & "(" & value & ")");

			Rule callspec = "__call" %
				((Rule)"__thiscall" | "__cdecl" | "__clrcall" | "__stdcall" | "__fastcall");

			Rule hacks = "hacks" %
				(callspec | attribute | declspec);
			//hacks.Ignore = true;
			//hacks.Primitive = true;

			this.WhiteSpace = new Whitespace(hacks | this.WhiteSpace.Inner);

			#endregion Compiler Hacks

			#region Extern "C"
			Rule externalC = "externC" %
				("extern" & quote2 & ((Rule)"c" | "C") & quote2 & "{" & +(rootElement | space) & "}");
			#endregion Extern "C"

			rootElement.Inner = (space |
				namespaceDefinition | classDef | classDefinition | typedef | enumDef | functionDefinition |
				classDeclaration | variableDeclaration | functionDeclaration | constructorDefinitionExtern |
				externalC);
			root.Inner = (+rootElement);


			return root;
		}

		protected override string Preprocess(string content)
		{
			content = base.Preprocess(content);
			//Trigraph     Equivalent
			//========     ==========
			//  ??=            #
			//  ??/            \
			//  ??'            ^
			//  ??(            [
			//  ??)            ]
			//  ??!            |
			//  ??<            {
			//  ??>            }
			//  ??-            ~

			content = content.Replace("??=", "#").Replace("??/", "\\").Replace("??'", "^").Replace("??(", "[").Replace("??)", "]")
				.Replace("??!", "|").Replace("??<", "{").Replace("??>", "}").Replace("??-", "~");

			content = content.Replace("\\\n", "");
			return content;
		}
	}
}
