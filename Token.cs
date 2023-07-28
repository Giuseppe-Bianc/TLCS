using System.Diagnostics;

namespace TLCS {
	public enum TokenType {
		Intero,
		Reale,
		Identificatore,
		Operatore,
		FineRiga
	}

	public interface ITokenIterator {
		Token? Next();
		bool HasNext();
	}

	public sealed class TokenIterator : ITokenIterator {
		private static TokenIterator? instance;
		private readonly List<Token> tokens;
		private int index;

		private TokenIterator(List<Token> tokens) {
			this.tokens = tokens;
			this.index = 0;
		}

		public static TokenIterator GetInstance(List<Token> tokens) {
			instance ??= new TokenIterator(tokens);
			return instance;
		}


		public Token? Next() => HasNext() ? tokens[index++] : null;

		public bool HasNext() => index < tokens.Count;
	}

	[Serializable]
	[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
	public sealed record Token(TokenType Tipo, string Valore, int Linea, int Colonna) {
		public Token(TokenType tipo, int linea, int colonna) : this(tipo, "", linea, colonna) { }

		public static Token CreateEndOfFileToken(int linea, int colonna) => new(TokenType.FineRiga, linea, colonna);
		public static Token CreateOperatorToken(string valore, int linea, int colonna) => new(TokenType.Operatore, valore, linea, colonna);
		public static Token CreateIntToken(string valore, int linea, int colonna) => new(TokenType.Intero, valore, linea, colonna);
		public static Token CreateRealToken(string valore, int linea, int colonna) => new(TokenType.Reale, valore, linea, colonna);
		public static Token CreateIdentifierToken(string valore, int linea, int colonna) => new(TokenType.Identificatore, valore, linea, colonna);


		public override string ToString() => Tipo == TokenType.FineRiga
			? $"Token<{Tipo}, EOF> (Linea: {Linea}, Colonna: {Colonna})"
			: $"Token<{Tipo}, '{Valore}'> (Linea: {Linea}, Colonna: {Colonna})";

		public static ITokenIterator GetIterator(List<Token> tokens) {
			return TokenIterator.GetInstance(tokens);
		}

		public override int GetHashCode() {
			return HashCode.Combine(Tipo,Valore,Linea,Colonna);
		}

		private string GetDebuggerDisplay() {
			return ToString();
		}
	}

}