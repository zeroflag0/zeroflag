﻿using System;
using System.Collections.Generic;
using zeroflag.Parsing;
using System.Text;
using zeroflag.Parsing.ObjectOriented;

namespace Test
{
	class CppParser : zeroflag.Parsing.Parser
	{
		public override zeroflag.Parsing.Rule CreateRules()
		{
			Rule newline = @"\n" %
				( (Rule)"\n" | "\0" );//"\r\n" | 
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
				( "/*" & ~+!(Rule)"*/" & "*/" ) |
				( "//" & ~+!newline ^ newline );

			// preprocessor...
			Rule preprocessor = "preprocessor" %
				(
				"#" & ~+!newline ^ newline
				);

			this.WhiteSpace = ( preprocessor | comment | this.WhiteSpace.Inner );
			space = this.WhiteSpace;
			space.Ignore = true;

			Rule expression = new Rule() { Name = "expression", StructureType = typeof( Expression ) };
			Rule functionCall = new Rule() { Name = "call", StructureType = typeof( FunctionCall ) };
			Rule functionCallTail = new Rule( "functionCallT" );
			Rule assign = new Rule( "assign" ) { StructureType = typeof( Operation ) };
			Rule root = new Rule( "root" ) { StructureType = typeof( Root ) };
			Rule rootElement = new Rule( "rootel" );
			Rule type = new Rule() { Name = "type", StructureType = typeof( zeroflag.Parsing.ObjectOriented.Type ) };
			Rule classDefinition = new Rule( "class" ) { StructureType = typeof( Class ) };
			Rule value = new Rule( "value" ) { StructureType = typeof( Value ) };
			Rule structDefinition = new Rule( "struct" ) { StructureType = typeof( Struct ) };
			Rule statement = new Rule( "statement" ) { StructureType = typeof( Expression ) };
			Rule functionParameterDecs = new Rule() { Name = "functionParameterDecs" };
			Rule instance = new Rule( "instance" ) { StructureType = typeof( Value ) };


			#region Values
			// strings...
			Rule esc = "\\";
			Rule quote2 = "\"" % (Rule)'"';
			Rule quote = "'" % (Rule)'\'';
			Rule nonescapedQuote = ( !esc & quote2 );
			//Rule stringValue = "string" %
			//    (
			//    quote2 & ~+!(nonescapedQuote | nl) & ~!(quote2 | nl) & quote2
			//    );
			Rule stringValue = "string" %
				( quote2 & ~+( ( esc & !newline ) | !( esc | quote2 ) | ( quote2 & quote2 ) ) & quote2 );
			//new RegexTerminal(@"");

			// char...
			Rule charValue = "char" %
				( quote & ( ( esc & any ) | !quote ) & quote );

			Rule digit = "digit" %
				( (Rule)"0123456789".ToCharArray() );

			// int...
			Rule intValue = "int" %
				+digit;

			// real...
			Rule realValue = "real" %
				( +digit & "." & +digit & ~( (Rule)"f" | "F" ) );

			Rule hexDigit = "hexdigit" %
				( digit | "abcdef".ToCharArray() | "abcdef".ToUpper().ToCharArray() );

			// hex...
			Rule hexValue = "hex" %
				( "0x" & +hexDigit );

			#endregion Values


			#region Operators
			Rule primaryOperators = "primaryOp" %
				( ( (Rule)"++" | "--" ) );

			Rule indexer = "indexer" %
				+( ~space ^ (
				( "[" & +( ( value | instance ) ^ ~( ~space & "," & ~space ) ) & "]" ) |
				( "[" & +( expression ^ ~( ~space & "," & ~space ) ) & "]" )
				) );

			Rule newOp = "new" %
				( "new" & ( functionCall | ( type & indexer ) ) );
			Rule deleteOp = "delete" %
				( "delete" & ~( (Rule)"[" & "]" ) & instance );
			Rule typeofOp = "typeof" %
				( (Rule)"typeof" & "(" & type & ")" );

			Rule unaryOperators = "unaryOp" %
				( ( (Rule)"!" | "~" | "++" | "--" | "+" | "-" | "*" ) );

			Rule mpyOperators = "mpyOp" %
				( (Rule)"*" | "/" | "%" );
			Rule addOperators = "addOp" %
				( (Rule)"+" | "-" );
			Rule shiftOperators = "shiftOp" %
				( (Rule)">>" | "<<" );

			Rule compareOperators = "compareOp" %
				( (Rule)">=" | ">" | "<=" | "<" );
			Rule equalOperators = "equalOp" %
				( (Rule)"==" | "!=" );

			Rule assignOperator = "assignOp" %
				( ( (Rule)"=" | "+=" | "-=" | "*=" | "/=" | "^=" | "&=" | "|=" | ">>=" | "<<=" ) );

			#endregion Operators

			#region Type Usage
			Rule idHead = letter | "_";
			Rule idTail = idHead | digit;

			Rule id = "id" %
				new RegexTerminal( @"([a-zA-Z_][a-zA-Z0-9_]*)" );
			//(idHead ^ ~+idTail);


			Rule baseType = "typeBasic" %
				(
				//(Rule)"char" | "bool" | "short" | "int" | 
				( "__int" ^ ~+digit ) |
				( "long" ^ ~( space & ( (Rule)"long" | "double" ) ) ) |
				( "unsigned" ^ ~( space & ( (Rule)"int" | "short" | "long" | "char" ) ) ) | "float" | "double" | "char" |
				( "signed" ^ ~( space & ( (Rule)"char" ) ) ) |
				( "enum" ^ ~( space & type ) ) |
				( "struct" ^ ~( space & type ) ) );

			Rule scopeType = "::";
			Rule types = "typeList" %
				( type & ~+( "," & type ) );
			Rule pointers = "pointer" %
				+( (Rule)"*" | "[]" | "&" );
			Rule typeTail = pointers;
			//"typeTail" % ( pointers );

			Rule functionPointerTail = "funcPtrTail" %
				( (Rule)"(" & "*" & ")" & "(" & types & ")" );

			Rule genericTypeName = "typeGeneric" %
				( id & ~( "<" & types & ">" ) );

			Rule typeCast = "cast" %
				( "(" & type & ")" );
			Rule typeSimple = "typeSimple" %
				( baseType | genericTypeName );

			Rule staticInstance = "static" %
				( typeSimple & ~+( scopeType & typeSimple ) );

			type.Inner = ( ( ~( "const" ^ space ) ^ ~( scopeType ) & staticInstance ^ ~( ~space ^ typeTail ) ) ^ ~( ~space ^ functionPointerTail ) );

			Rule typedef = "typedef" %
				( "typedef" ^ space ^ ( ( classDefinition | type ) ^ +( ~( ~space & "," ) ^ space & type ^ ~( ~space & type ) ) ) & ";" );
			#endregion

			#region Variables
			Rule varialbePrototypeVar = new Rule() { StructureType = typeof( Variable ), Inner = id };
			Rule variablePrototype = "variablePrototype" %
				( type ^ ~space & varialbePrototypeVar );

			Rule instancePart = "instancePart" %
				( ( staticInstance | type | id ) ^ ~+( ( ~space ^ functionCallTail ) | ( ~space ^ indexer ) ) );
			Rule instances = "instances" %
				( instancePart & ~+( ~space & ( (Rule)"." | "->" ) & instancePart ) );
			instance.Inner = (
				//(id & ~+(((Rule)"." | "->") & id)) |
				( ~( typeCast ^ ~space ) ^
				(
				instances |
				staticInstance |
				id
				)
				^ ~( ~space ^ indexer )
				) );
			instance.StructureType = typeof( Expression );

			Rule variableDeclaration = "variable" %
				( type ^ ~space ^ +( ~space & ( assign | instance ) & ~indexer & ~( functionCallTail ) & ~(Rule)"," & ~space ) & ";" );

			#endregion

			#region Functions
			//Rule functionParameterDecs = new Rule() { Name = "functionParameterDecs" };
			Rule functionParameterPrototype = new Rule() { Name = "param", StructureType = typeof( Parameter ), Inner = variablePrototype };
			functionParameterDecs.Inner = ( ( functionParameterPrototype & ~+( "," & functionParameterPrototype ) ) | types );

			#region Operator Overloads
			// bool operator() (const KeyFrame* kf, const KeyFrame* kf2) const

			Rule operatorOverload = "operatorOl" %
				( "operator" & (
				( (Rule)"(" & ")" ) |
				( (Rule)"[" & "]" ) |
				assignOperator | equalOperators | compareOperators | shiftOperators | addOperators | mpyOperators | unaryOperators | primaryOperators
				) );
			#endregion Operator Overloads

			Rule constructorPrototype = "constructorPrototype" %
				//(~(type & scopeType) & (id | type) & "(" & ~(functionParameterDecs) & ")");
				( (
					( operatorOverload | ( ( instance | type ) & ~scopeType & operatorOverload ) ) |
					( ~type & ( ~scopeType & ~(Rule)"~" & id ) ) |
					type
				) &
				"(" & ~( functionParameterDecs ) & ")" ^ ~( ~space & "const" ) );
			Rule functionPrototype = "functionPrototype" %
				( type ^ space ^ constructorPrototype );
			Rule functionDeclaration = "functionDeclaration" %
				( functionPrototype & ";" );

			Rule functionBody = new Rule( "functionBody" );
			functionBody.Inner =
				( "{" &
				~+( space |
				statement |
				variableDeclaration |
				functionBody
				)
				& new FailedBefore( "}" ) );

			Rule functionCallParameters = "parameters" %
				( ( expression | value ) & ~+( ',' & ( expression | value ) ) );
			functionCallTail.Inner = ( '(' & ~functionCallParameters & ')' );
			functionCall.Inner = ( ( instance & functionCallTail | ( type & ~instance ) & functionCallTail | id & functionCallTail ) );

			Rule functionDefinition = "functionDefinition" %
				( functionPrototype & functionBody );
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

			value.Inner = ( ~( typeCast ^ ~space ) ^ ~+( ( (Rule)"&" | "*" ) ^ ~space ) ^ ( hexValue | realValue | intValue | stringValue | charValue | instance | id | ( "(" & expression & ")" ) ) ^ ~( ~space ^ indexer ) );

			Rule ops = new Rule( "op" );

			Rule primary = "primary" %
				~( typeCast & ~space ) ^ ( newOp | deleteOp | typeofOp | ( "(" & expression & ")" ) | value );
			ops = primary;

			Rule unary = "unary" %
				( ~unaryOperators & ops );
			ops = ops | ( unary );

			Rule reference = "ref" %
				( unary & ~+( ~space ^ ( (Rule)"." | "->" ) & unary ) );
			ops = ops | ( reference );

			Rule mpy = "mpy" %
				( ( reference & ~+( ~space ^ mpyOperators & reference ) ) );//| unary);
			ops = ops | ( mpy );
			Rule add = "add" %
				( ( mpy & ~+( ~space ^ addOperators & mpy ) ) );//| mpy);
			ops = ops | ( add );

			Rule shift = "shift" %
				( ( add & ~+( ~space ^ shiftOperators & add ) ) );//| add);
			ops = ops | ( shift );

			Rule compare = "compare" %
				( ( shift & ~+( ~space ^ compareOperators & shift ) ) );//| shift);
			ops = ops | ( compare );

			Rule equal = "equal" %
				( ( compare & ~+( ~space ^ equalOperators & compare ) ) );//| compare);
			ops = ops | ( equal );

			Rule logicalAND = "logicAND" %
				( ( equal & ~+( ~space ^ ( "&" ^ -!(Rule)"&" ) & equal ) ) );//| equal);
			ops = ops | ( logicalAND );
			Rule logicalXOR = "logicXOR" %
				( ( logicalAND & ~+( ~space ^ "^" & logicalAND ) ) );//| logicalAND);
			ops = ops | ( logicalXOR );
			Rule logicalOR = "logicOR" %
				( ( logicalXOR & ~+( ~space ^ ( "|" ^ -!(Rule)"|" ) & logicalXOR ) ) );//| logicalXOR);
			ops = ops | ( logicalOR );

			Rule conditionalAND = "AND" %
				( ( logicalOR & ~+( ~space ^ "&&" & logicalOR ) ) );// | logicalOR);
			ops = ops | ( conditionalAND );
			Rule conditionalOR = "OR" %
				( ( conditionalAND & ~+( ~space ^ "||" & conditionalAND ) ) );// | conditionalAND);
			ops = ops | ( conditionalOR );

			Rule conditional = "?:" %
				( conditionalAND & ~( "?" & value & ":" & value ) );// | conditionalOR);

			ops = "ops" %
				( conditional );

			assign.Inner =
				( value & assignOperator & expression );

			expression.Inner = ( assign | ops | functionCall );

			//ops.StructureType = primary.StructureType = unary.StructureType = reference.StructureType = mpy.StructureType
			//    = add.StructureType = shift.StructureType = compare.StructureType = equal.StructureType = logicalAND.StructureType = logicalOR.StructureType
			//    = logicalXOR.StructureType = conditionalOR.StructureType = conditionalAND.StructureType = conditional.StructureType = assign.StructureType = typeof( Operation );

			expression.StructureType = typeof( Expression );
			#endregion


			#region Statements
			//Rule returnStatement = "return" %
			//    ("return" ^ ~(+space & expression) & ";");
			#region if/else
			Rule ifStatement = new Rule( "if" );

			Rule elseStatement = "else" %
				( "else" & ( ifStatement | functionBody | statement ) );

			ifStatement.Inner = ( (Rule)"if" & "(" & expression & ")" & ( functionBody | statement ) ^ ~( ~space & elseStatement ) );
			#endregion if/else

			#region for
			Rule forStatement = "for" %
				( (Rule)"for" & "(" & ( variableDeclaration | ( expression & ";" ) | ";" ) & expression & ";" & expression & ")" & ( functionBody | statement ) );
			#endregion for

			#region while
			Rule whileHeadStatement = "whileHead" %
				( (Rule)"while" & "(" & expression & ")" );

			Rule whileStatement = "while" %
				( whileHeadStatement & ( functionBody | statement ) );

			Rule dowhileStatement = "dowhile" %
				( "do" & ( functionBody | statement ) & whileHeadStatement & ";" );
			#endregion while

			statement.Inner = ( ( ~( (Rule)"return" ^ ~space ) ^ expression & ";" ) | typedef | variableDeclaration | ifStatement | forStatement | whileStatement | dowhileStatement );

			#endregion

			#region Classes
			#region Inheritance
			Rule inheritItem = "inheritItem" %
				( functionCall | ( type & ~functionCall ) | id );
			Rule inherit = "inherit" %
				( ":" & inheritItem ^ ~+( ~space & "," & inheritItem ) );
			#endregion

			Rule classPrototype = "classProt" %
				( ( (Rule)"class" | "struct" ) ^ ~( space & id ) );


			Rule classDeclaration = "classDec" %
				( classPrototype & ~inherit & ";" );

			Rule classVisibility = "visiblity" %
				( ( (Rule)"public" | "protected" | "private" ) & ":" );

			Rule declaration = "decl" %
				~( ( (Rule)"virtual" | "explicit" | "implicit" | "static" ) ^ space );

			classPrototype.StructureType = typeof( Class );
			classDeclaration.StructureType = typeof( Class );

			#region Constructor
			Rule constructorDeclaration = "constructorDeclaration" %
				( constructorPrototype & ~inherit & ";" );
			Rule constructorDefinition = "constructor" %
				( constructorPrototype & ~inherit & functionBody );
			Rule constructorDefinitionExtern = "constructorExt" %
				( constructorPrototype & ~inherit & functionBody );


			constructorDeclaration.StructureType = constructorDefinition.StructureType = constructorDefinitionExtern.StructureType = typeof( Constructor );
			#endregion Constructor

			#region Destructor
			Rule destructorDeclaration = "destructorDeclaration" %
				( "~" & constructorDeclaration );
			Rule destructorDefinition = "destructorDefinition" %
				( "~" & constructorDefinition );

			destructorDeclaration.StructureType = destructorDefinition.StructureType = typeof( Destructor );
			#endregion Destructor

			#region Class Body
			Rule classBody = "classBody" %
				( space |
				( declaration & (
				functionDeclaration | functionDefinition |
				constructorDeclaration | constructorDefinition |
				destructorDeclaration | destructorDefinition ) ) |
				variableDeclaration | classVisibility | root );
			#endregion Class Body

			structDefinition.Inner = ( (Rule)"struct" & "{" & +classBody & "}" ^ ~( ~space & instance ) );

			Rule structDef = "structdef" %
				( structDefinition ^ ~( ~space & ";" ) );
			classDefinition.Inner =
				( structDefinition | ( classPrototype & ~inherit & "{" & new FailedBefore( +classBody & "}" ) ) );// ^ ~(~space & instance)));
			Rule classDef = "class" %
				( classDefinition ^ ~(Rule)";" );

			structDef.StructureType = typeof( Struct );
			classDef.StructureType = typeof( Class );

			#endregion

			#region Enums
			Rule enumHead = "enumHead" %
				( "enum" ^ ~( space & id ) );
			Rule enumItem = "enumItem" %
				( ( assign | id ) );
			Rule enumBody = "enumBody" %
				( +( ~space & enumItem & ~space & ~(Rule)"," & ~space ) );
			Rule enumDef = "enum" %
				( enumHead & "{" & enumBody & "}" & ~(Rule)";" );
			#endregion Enums

			#region Namespaces
			Rule namespaceBody = "namespaceBody" %
				( +( rootElement ) );
			Rule namespaceDefinition = "namespace" %
				( "namespace" ^ ~( space & id ) & "{" & namespaceBody & "}" & ~(Rule)";" );
			namespaceDefinition.StructureType = typeof( Namespace );
			#endregion Namespaces

			#region Compiler Hacks
			// some stupid C/C++ compiler(-specific) stuff to ignore...

			// it seems msvc has picked up [attributes]... just ignore them.
			Rule attribute = "attribute" %
				( "[" & ( ( functionCall | type ) ^ ~+( ":" & ( functionCall | type ) ) ) & "]" );

			// __declspec... whatever...
			Rule declspec = "__declspec" %
				( (Rule)"__declspec" & "(" & value & ")" );

			Rule callspec = "__call" %
				( (Rule)"__thiscall" | "__cdecl" | "__clrcall" | "__stdcall" | "__fastcall" );

			Rule hacks = "hacks" %
				( callspec | attribute | declspec );
			hacks.Ignore = true;
			hacks.Primitive = true;

			this.WhiteSpace = new Whitespace( hacks | this.WhiteSpace.Inner ) { Primitive = false };

			#endregion Compiler Hacks

			#region Extern "C"
			Rule externalC = "externC" %
				( "extern" & quote2 & ( (Rule)"c" | "C" ) & quote2 & "{" & +( rootElement | space ) & "}" );
			#endregion Extern "C"

			rootElement.Inner = ( space |
				namespaceDefinition | classDef | classDefinition | typedef | enumDef | functionDefinition |
				classDeclaration | variableDeclaration | functionDeclaration | constructorDefinitionExtern |
				externalC );

			Rule eof = "EOF" %
				( (Rule)"\0" );
			root.Inner = ( +rootElement );// ^ eof;
			root.StructureType = typeof( Root );

			return root;
		}

		protected override string Preprocess( string content )
		{
			content = base.Preprocess( content );
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

			content = content.Replace( "??=", "#" ).Replace( "??/", "\\" ).Replace( "??'", "^" ).Replace( "??(", "[" ).Replace( "??)", "]" )
				.Replace( "??!", "|" ).Replace( "??<", "{" ).Replace( "??>", "}" ).Replace( "??-", "~" );

			content = content.Replace( "\\\n", "" );
			return content;
		}
	}
}
