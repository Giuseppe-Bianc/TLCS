namespace TLCS {
	public enum NodeType {
		IntegerLiteral,
		RealLiteral,
		Identifier,
		BinaryOperation,
		UnaryOperation,
		ParenertesiedOperation
	}

	public abstract class AstNode {
		public NodeType NodeType { get; protected set; }
	}

	public abstract class NumberLiteral : AstNode { }

	public class IntegerLiteralNode : NumberLiteral {
		public int Value { get; set; }

		public IntegerLiteralNode(int value) {
			NodeType = NodeType.IntegerLiteral;
			Value = value;
		}
	}

	public class RealLiteralNode : NumberLiteral {
		public double Value { get; set; }

		public RealLiteralNode(double value) {
			NodeType = NodeType.RealLiteral;
			Value = value;
		}
	}

	public class IdentifierNode : AstNode {
		public string Name { get; set; }

		public IdentifierNode(string name) {
			NodeType = NodeType.Identifier;
			Name = name;
		}

	}

	public class BinaryOperationNode : AstNode {
		public Token OperatorToken { get; set; }
		public AstNode Left { get; set; }
		public AstNode Right { get; set; }

		public BinaryOperationNode(Token operatorToken, AstNode left, AstNode right) {
			NodeType = NodeType.BinaryOperation;
			OperatorToken = operatorToken;
			Left = left;
			Right = right;
		}
	}

	public class UnaryOperationNode : AstNode {
		public Token OperatorToken { get; set; }
		public AstNode Operand { get; set; }

		public UnaryOperationNode(Token operatorToken, AstNode operand) {
			NodeType = NodeType.UnaryOperation;
			OperatorToken = operatorToken;
			Operand = operand;
		}
	}

	public class ParenertesiedOperationNode : AstNode {
		public AstNode Expression { get; set; }

		public ParenertesiedOperationNode(AstNode operand) {
			NodeType = NodeType.ParenertesiedOperation;
			Expression = operand;
		}
	}

	public class ASTGenerator {
		private readonly Lexer lexer;
		private readonly List<Token> tokens;
		private int currentTokenIndex;

		private static readonly Dictionary<string, int> OperatorPrecedence = new() {
			{ "+", 1 },
			{ "-", 1 },
			{ "*", 2 },
			{ "/", 2 },
			{ "^", 3 },
		};

		public ASTGenerator(string input) {
			lexer = new Lexer(input);
			this.tokens = lexer.GetTokens();
			currentTokenIndex = 0;
		}

		private Token Peek() {
			return currentTokenIndex < tokens.Count ? tokens[currentTokenIndex] : Token.CreateEndOfFileToken(1, 0);
		}

		private Token Consume(TokenType tokenType) {
			Token currentToken = Peek();
			if (currentToken.Tipo != tokenType) {
				throw new InvalidOperationException($"Expected token type: {tokenType}, found: {currentToken.Tipo}");
			}
			currentTokenIndex++;
			return currentToken;
		}

		private Token Consume(TokenType tokenType, string value) {
			Token currentToken = Peek();
			if (currentToken.Tipo == tokenType || currentToken.Valore.Equals(value)) {
				currentTokenIndex++;
				return currentToken;
			}
			throw new InvalidOperationException($"Expected token type: {tokenType}, found: {currentToken.Tipo}");
		}

		private AstNode Factor() {
			var token = Peek();
			switch (token.Tipo) {
				case TokenType.Intero:
					var integerToken = Consume(TokenType.Intero);
					return new IntegerLiteralNode(int.Parse(integerToken.Valore));
				case TokenType.Reale:
					var realToken = Consume(TokenType.Reale);
					return new RealLiteralNode(double.Parse(realToken.Valore));
				case TokenType.Identificatore:
					var identifierToken = Consume(TokenType.Identificatore);
					return new IdentifierNode(identifierToken.Valore);
				case TokenType.Operatore when token.Valore == "(":
					Consume(TokenType.Operatore);
					var node = Expression();
					Consume(TokenType.Operatore, ")"); // Closing parenthesis
					return new ParenertesiedOperationNode(node);
				default:
					throw new InvalidOperationException($"Unexpected token: {token.Tipo}");
			}
		}

		private AstNode UnaryFactor() {
			var token = Peek();
			if (token.Tipo == TokenType.Operatore && (token.Valore == "+" || token.Valore == "-")) {
				var operatorToken = Consume(TokenType.Operatore);
				return new UnaryOperationNode(operatorToken, UnaryFactor());
			}
			return Factor();
		}

		private AstNode Exponentiation() {
			var node = UnaryFactor();
			while (Peek().Tipo == TokenType.Operatore && OperatorPrecedence.ContainsKey(Peek().Valore) && OperatorPrecedence[Peek().Valore] == 3) {
				var operatorToken = Consume(TokenType.Operatore);
				node = new BinaryOperationNode(operatorToken, node, UnaryFactor());
			}
			return node;
		}

		private AstNode Term() {
			var node = Exponentiation();
			while (Peek().Tipo == TokenType.Operatore && OperatorPrecedence.ContainsKey(Peek().Valore) && OperatorPrecedence[Peek().Valore] == 2) {
				var operatorToken = Consume(TokenType.Operatore);
				node = new BinaryOperationNode(operatorToken, node, Exponentiation());
			}
			return node;
		}

		private AstNode Expression() {
			var node = Term();
			while (Peek().Tipo == TokenType.Operatore && OperatorPrecedence.ContainsKey(Peek().Valore) && OperatorPrecedence[Peek().Valore] == 1) {
				var operatorToken = Consume(TokenType.Operatore);
				node = new BinaryOperationNode(operatorToken, node, Term());
			}
			return node;
		}

		public AstNode Parse() {
			return Expression();
		}
	}
}
