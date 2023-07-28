using System.Collections.Immutable;

namespace TLCS {
	public enum NodeType {
		IntegerLiteral,
		RealLiteral,
		Identifier,
		BinaryOperation,
		UnaryOperation,
		ParenthesizedOperation
	}

	public abstract record AstNode(NodeType NodeType);

	public abstract record NumberLiteral(NodeType NodeType) : AstNode(NodeType);

	public sealed record IntegerLiteralNode(int Value) : NumberLiteral(NodeType.IntegerLiteral);

	public sealed record RealLiteralNode(double Value) : NumberLiteral(NodeType.RealLiteral);

	public sealed record IdentifierNode(string Name) : AstNode(NodeType.Identifier);

	public sealed record BinaryOperationNode(Token OperatorToken, AstNode Left, AstNode Right) : AstNode(NodeType.BinaryOperation);

	public sealed record UnaryOperationNode(Token OperatorToken, AstNode Operand) : AstNode(NodeType.UnaryOperation);

	public sealed record ParenthesizedOperationNode(AstNode Expression) : AstNode(NodeType.ParenthesizedOperation);

	public sealed class ASTGenerator : IDisposable {
		private readonly List<Token> tokens;
		private int currentTokenIndex;


		private static readonly Dictionary<string, int> OperatorPrecedence = new Dictionary<string, int>
		{
			{ "+", 1 },
			{ "-", 1 },
			{ "*", 2 },
			{ "/", 2 },
			{ "^", 3 },
		};

		public ASTGenerator(string input) {
			this.tokens = new Lexer(input).GetTokens();
			currentTokenIndex = 0;
		}

		public ASTGenerator(string input, string i) {
			this.tokens = new Lexer(input).GetTokensM();
			currentTokenIndex = 0;
		}

		public void Dispose() {
			tokens.Clear();
			GC.SuppressFinalize(this);
			// Dispose Lexer instance if necessary
		}

		private Token Peek() => currentTokenIndex < tokens.Count ? tokens[currentTokenIndex] : Token.CreateEndOfFileToken(1, 0);

		private Token Consume(TokenType tokenType) => Peek().Tipo != tokenType
			? throw new InvalidOperationException($"Expected token type: {tokenType}, found: {Peek().Tipo}")
			: tokens[currentTokenIndex++];

		private Token Consume(TokenType tokenType, string value) => (Peek().Tipo, Peek().Valore) == (tokenType, value)
			? tokens[currentTokenIndex++]
			: throw new InvalidOperationException($"Expected token type: {tokenType}, found: {Peek().Tipo}");

		private AstNode Factor() {
			var token = Peek();
			return token.Tipo switch {
				TokenType.Intero => new IntegerLiteralNode(int.Parse(Consume(TokenType.Intero).Valore)),
				TokenType.Reale => new RealLiteralNode(double.Parse(Consume(TokenType.Reale).Valore)),
				TokenType.Identificatore => new IdentifierNode(Consume(TokenType.Identificatore).Valore),
				TokenType.Operatore when token.Valore == "(" => ParenthesizedOperation(),
				_ => throw new InvalidOperationException($"Unexpected token: {token.Tipo}")
			};
		}

		private AstNode ParenthesizedOperation() {
			Consume(TokenType.Operatore); // Opening parenthesis
			var node = Expression();
			Consume(TokenType.Operatore, ")"); // Closing parenthesis
			return new ParenthesizedOperationNode(node);
		}

		private AstNode UnaryFactor() {
			var token = Peek();
			return (token.Tipo, token.Valore) switch {
				(TokenType.Operatore, "+") or (TokenType.Operatore, "-") => new UnaryOperationNode(Consume(TokenType.Operatore), UnaryFactor()),
				_ => Factor()
			};
		}

		private AstNode Exponentiation() {
			var node = UnaryFactor();
			while (OperatorPrecedence.TryGetValue(Peek().Valore, out var precedence) && precedence == 3) {
				var operatorToken = Consume(TokenType.Operatore);
				node = new BinaryOperationNode(operatorToken, node, UnaryFactor());
			}
			return node;
		}

		private AstNode Term() {
			var node = Exponentiation();
			while (OperatorPrecedence.TryGetValue(Peek().Valore, out var precedence) && precedence == 2) {
				var operatorToken = Consume(TokenType.Operatore);
				node = new BinaryOperationNode(operatorToken, node, Exponentiation());
			}
			return node;
		}

		private AstNode Expression() {
			var node = Term();
			while (OperatorPrecedence.TryGetValue(Peek().Valore, out var precedence) && precedence == 1) {
				var operatorToken = Consume(TokenType.Operatore);
				node = new BinaryOperationNode(operatorToken, node, Term());
			}
			return node;
		}

		public AstNode Parse() => Expression();
	}
}
