namespace TLCS {
	public enum TokenType {
		Intero,
		Reale,
		Identificatore,
		Operatore,
		ParentesiAperta,
		ParentesiChiusa,
		FineRiga
	}

	public class Token {
		public TokenType Tipo { get; }
		public string Valore { get; }
		public int Linea { get; }
		public int Colonna { get; }

		public Token(TokenType tipo, string valore, int linea, int colonna) {
			Tipo = tipo;
			Valore = valore;
			Linea = linea;
			Colonna = colonna;
		}

		public override string ToString() {
			return Tipo == TokenType.FineRiga ? $"Token<{Tipo}, EOF> (Linea: {Linea}, Colonna: {Colonna})" 
				: $"Token<{Tipo}, '{Valore}'> (Linea: {Linea}, Colonna: {Colonna})";
		}
	}
}