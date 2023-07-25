using Ardalis.SmartEnum;

namespace TLCS {
	public enum TokenType {
		Intero,
		Reale,
		Identificatore,
		Operatore,
		FineRiga
	}

	public class TokenTypeE : Enumeration<TokenTypeE> {
		public TokenTypeE(int value, string name) : base(value, name) {}

		public static readonly TokenTypeE Intero = new(1, nameof(Intero));
		public static readonly TokenTypeE Reale = new(2, nameof(Reale));
		public static readonly TokenTypeE Identificatore = new(3, nameof(Identificatore));
		public static readonly TokenTypeE Operatore = new(4, nameof(Operatore));
		public static readonly TokenTypeE FineRiga = new(5, nameof(FineRiga));
	}

	public class TokenTypeS : SmartEnum<TokenTypeS> {
		public TokenTypeS(string name, int value) : base(name,value) {}

		public static readonly TokenTypeS Intero = new(nameof(Intero),1);
		public static readonly TokenTypeS Reale = new(nameof(Reale),2);
		public static readonly TokenTypeS Identificatore = new(nameof(Identificatore),3);
		public static readonly TokenTypeS Operatore = new(nameof(Operatore),4);
		public static readonly TokenTypeS FineRiga = new(nameof(FineRiga),5);
	}

	/// <summary>
	/// Rappresenta un token nel codice di input con le relative informazioni.
	/// </summary>
	[Serializable]
	public sealed class Token : IEquatable<Token> {
		public TokenType Tipo { get; init; }
		public string Valore { get; init; }
		public int Linea { get; init; }
		public int Colonna { get; init; }

		/// <summary>
		/// Inizializza una nuova istanza della classe <see cref="Token"/>.
		/// </summary>
		/// <param name="tipo">Il tipo di token.</param>
		/// <param name="valore">Il valore del token.</param>
		/// <param name="linea">Il numero di riga in cui è stato trovato il token.</param>
		/// <param name="colonna">Il numero di colonna in cui inizia il token.</param>
		/// <exception cref="ArgumentOutOfRangeException">Viene generata quando i valori di riga o colonna forniti non sono validi.</exception>
		public Token(TokenType tipo, string valore, int linea, int colonna) {
			if (linea <= 0) {
				throw new ArgumentOutOfRangeException(nameof(linea), "Il numero di riga deve essere maggiore di zero.");
			}

			if (colonna < 0) {
				throw new ArgumentOutOfRangeException(nameof(colonna), "Il numero di colonna deve essere maggiore o uguale a zero.");
			}

			Tipo = tipo;
			Valore = valore;
			Linea = linea;
			Colonna = colonna;
		}

		/// <summary>
		/// Inizializza una nuova istanza della classe <see cref="Token"/>.
		/// </summary>
		/// <param name="tipo">Il tipo di token.</param>
		/// <param name="linea">Il numero di riga in cui è stato trovato il token.</param>
		/// <param name="colonna">Il numero di colonna in cui inizia il token.</param>
		/// <exception cref="ArgumentOutOfRangeException">Viene generata quando i valori di riga o colonna forniti non sono validi.</exception>
		public Token(TokenType tipo, int linea, int colonna) {
			if (linea <= 0) {
				throw new ArgumentOutOfRangeException(nameof(linea), "Il numero di riga deve essere maggiore di zero.");
			}

			if (colonna < 0) {
				throw new ArgumentOutOfRangeException(nameof(colonna), "Il numero di colonna deve essere maggiore o uguale a zero.");
			}

			Tipo = tipo;
			Valore = "";
			Linea = linea;
			Colonna = colonna;
		}

		/// <summary>
		/// Crea un token di fine file.
		/// </summary>
		/// <param name="linea">Il numero di riga in cui è stato trovato il token.</param>
		/// <param name="colonna">Il numero di colonna in cui inizia il token.</param>
		/// <returns>Il token di fine file.</returns>
		public static Token CreateEndOfFileToken(int linea, int colonna) => new(TokenType.FineRiga, linea, colonna);

		/// <summary>
		/// Restituisce una rappresentazione testuale del token.
		/// </summary>
		/// <returns>Una stringa che rappresenta il token con le informazioni sul tipo, il valore, la riga e la colonna.</returns>
		public override string ToString() => Tipo == TokenType.FineRiga
			? $"Token<{Tipo}, EOF> (Linea: {Linea}, Colonna: {Colonna})"
			: $"Token<{Tipo}, '{Valore}'> (Linea: {Linea}, Colonna: {Colonna})";

		/// <summary>
		/// Determina se l'oggetto specificato è uguale al token corrente.
		/// </summary>
		/// <param name="obj">L'oggetto da confrontare con il token corrente.</param>
		/// <returns><c>true</c> se l'oggetto specificato è uguale al token corrente; in caso contrario, <c>false</c>.</returns>
		public override bool Equals(object? obj) => Equals(obj as Token);

		/// <summary>
		/// Determina se il token specificato è uguale al token corrente.
		/// </summary>
		/// <param name="other">Il token da confrontare con il token corrente.</param>
		/// <returns><c>true</c> se il token specificato è uguale al token corrente; in caso contrario, <c>false</c>.</returns>
		public bool Equals(Token? other) {
			if (ReferenceEquals(this, other))
				return true;

			if (other is null)
				return false;

			return Tipo.Equals(other.Tipo) &&
				Valore.Equals(other.Valore, StringComparison.Ordinal) && // Usiamo String.Equals con StringComparison.Ordinal
				Linea == other.Linea &&
				Colonna == other.Colonna;
		}

		/// <summary>
		/// Serve come funzione hash per un tipo particolare, adatta per l'uso in algoritmi di hash e strutture dati come una tabella hash.
		/// </summary>
		/// <returns>Un codice hash per il token corrente.</returns>
		public override int GetHashCode() {
			unchecked {
				int hash = 17;
				hash = hash * 23 + Tipo.GetHashCode();
				hash = hash * 23 + (Valore?.GetHashCode() ?? 0);
				hash = hash * 23 + Linea.GetHashCode();
				hash = hash * 23 + Colonna.GetHashCode();
				return hash;
			}
		}


		/// <summary>
		/// Checks whether two tokens are equal.
		/// </summary>
		/// <param name="token1">The first token to compare.</param>
		/// <param name="token2">The second token to compare.</param>
		/// <returns><c>true</c> if the tokens are equal; otherwise, <c>false</c>.</returns>
		public static bool operator ==(Token? token1, Token? token2) {
			if (ReferenceEquals(token1, token2))
				return true;

			if (token1 is null || token2 is null)
				return false;

			return token1.Equals(token2);
		}

		/// <summary>
		/// Checks whether two tokens are not equal.
		/// </summary>
		/// <param name="token1">The first token to compare.</param>
		/// <param name="token2">The second token to compare.</param>
		/// <returns><c>true</c> if the tokens are not equal; otherwise, <c>false</c>.</returns>
		public static bool operator !=(Token? token1, Token? token2) => !(token1 == token2);
	}

	/// <summary> Rappresenta un token nel codice di input con le relative informazioni. </summary>
	[Serializable]
	public sealed class TokenE : IEquatable<TokenE> {
		public TokenTypeE Tipo { get; init; }
		public string Valore { get; init; }
		public int Linea { get; init; }
		public int Colonna { get; init; }

		/// <summary>
		/// Inizializza una nuova istanza della classe <see cref="Token"/>.
		/// </summary>
		/// <param name="tipo">Il tipo di token.</param>
		/// <param name="valore">Il valore del token.</param>
		/// <param name="linea">Il numero di riga in cui è stato trovato il token.</param>
		/// <param name="colonna">Il numero di colonna in cui inizia il token.</param>
		/// <exception cref="ArgumentOutOfRangeException">Viene generata quando i valori di riga o colonna forniti non sono validi.</exception>
		public TokenE(TokenTypeE tipo, string valore, int linea, int colonna) {
			if (linea <= 0) {
				throw new ArgumentOutOfRangeException(nameof(linea), "Il numero di riga deve essere maggiore di zero.");
			}

			if (colonna < 0) {
				throw new ArgumentOutOfRangeException(nameof(colonna), "Il numero di colonna deve essere maggiore o uguale a zero.");
			}

			Tipo = tipo;
			Valore = valore;
			Linea = linea;
			Colonna = colonna;
		}

		/// <summary>
		/// Inizializza una nuova istanza della classe <see cref="TokenE"/>.
		/// </summary>
		/// <param name="tipo">Il tipo di token.</param>
		/// <param name="linea">Il numero di riga in cui è stato trovato il token.</param>
		/// <param name="colonna">Il numero di colonna in cui inizia il token.</param>
		/// <exception cref="ArgumentOutOfRangeException">Viene generata quando i valori di riga o colonna forniti non sono validi.</exception>
		public TokenE(TokenTypeE tipo, int linea, int colonna) {
			if (linea <= 0) {
				throw new ArgumentOutOfRangeException(nameof(linea), "Il numero di riga deve essere maggiore di zero.");
			}

			if (colonna < 0) {
				throw new ArgumentOutOfRangeException(nameof(colonna), "Il numero di colonna deve essere maggiore o uguale a zero.");
			}

			Tipo = tipo;
			Valore = "";
			Linea = linea;
			Colonna = colonna;
		}

		/// <summary>
		/// Crea un token di fine file.
		/// </summary>
		/// <param name="linea">Il numero di riga in cui è stato trovato il token.</param>
		/// <param name="colonna">Il numero di colonna in cui inizia il token.</param>
		/// <returns>Il token di fine file.</returns>
		public static TokenE CreateEndOfFileToken(int linea, int colonna) => new(TokenTypeE.FineRiga, linea, colonna);

		/// <summary>
		/// Restituisce una rappresentazione testuale del token.
		/// </summary>
		/// <returns>Una stringa che rappresenta il token con le informazioni sul tipo, il valore, la riga e la colonna.</returns>
		public override string ToString() => Tipo == TokenTypeE.FineRiga
			? $"Token<{Tipo}, EOF> (Linea: {Linea}, Colonna: {Colonna})"
			: $"Token<{Tipo}, '{Valore}'> (Linea: {Linea}, Colonna: {Colonna})";

		/// <summary>
		/// Determina se l'oggetto specificato è uguale al token corrente.
		/// </summary>
		/// <param name="obj">L'oggetto da confrontare con il token corrente.</param>
		/// <returns><c>true</c> se l'oggetto specificato è uguale al token corrente; in caso contrario, <c>false</c>.</returns>
		public override bool Equals(object? obj) => Equals(obj as TokenE);

		/// <summary>
		/// Determina se il token specificato è uguale al token corrente.
		/// </summary>
		/// <param name="other">Il token da confrontare con il token corrente.</param>
		/// <returns><c>true</c> se il token specificato è uguale al token corrente; in caso contrario, <c>false</c>.</returns>
		public bool Equals(TokenE? other) {
			if (ReferenceEquals(this, other))
				return true;

			if (other is null)
				return false;

			return Tipo.Equals(other.Tipo) &&
				Valore.Equals(other.Valore, StringComparison.Ordinal) && // Usiamo String.Equals con StringComparison.Ordinal
				Linea == other.Linea &&
				Colonna == other.Colonna;
		}

		/// <summary>
		/// Serve come funzione hash per un tipo particolare, adatta per l'uso in algoritmi di hash e strutture dati come una tabella hash.
		/// </summary>
		/// <returns>Un codice hash per il token corrente.</returns>
		public override int GetHashCode() {
			unchecked {
				int hash = 17;
				hash = hash * 23 + Tipo.GetHashCode();
				hash = hash * 23 + (Valore?.GetHashCode() ?? 0);
				hash = hash * 23 + Linea.GetHashCode();
				hash = hash * 23 + Colonna.GetHashCode();
				return hash;
			}
		}


		/// <summary>
		/// Checks whether two tokens are equal.
		/// </summary>
		/// <param name="token1">The first token to compare.</param>
		/// <param name="token2">The second token to compare.</param>
		/// <returns><c>true</c> if the tokens are equal; otherwise, <c>false</c>.</returns>
		public static bool operator ==(TokenE? token1, TokenE? token2) {
			if (ReferenceEquals(token1, token2))
				return true;

			if (token1 is null || token2 is null)
				return false;

			return token1.Equals(token2);
		}

		/// <summary>
		/// Checks whether two tokens are not equal.
		/// </summary>
		/// <param name="token1">The first token to compare.</param>
		/// <param name="token2">The second token to compare.</param>
		/// <returns><c>true</c> if the tokens are not equal; otherwise, <c>false</c>.</returns>
		public static bool operator !=(TokenE? token1, TokenE? token2) => !(token1 == token2);
	}

	/// <summary> Rappresenta un token nel codice di input con le relative informazioni. </summary>
	[Serializable]
	public sealed class TokenS : IEquatable<TokenS> {
		public TokenTypeS Tipo { get; init; }
		public string Valore { get; init; }
		public int Linea { get; init; }
		public int Colonna { get; init; }

		/// <summary>
		/// Inizializza una nuova istanza della classe <see cref="TokenS"/>.
		/// </summary>
		/// <param name="tipo">Il tipo di token.</param>
		/// <param name="valore">Il valore del token.</param>
		/// <param name="linea">Il numero di riga in cui è stato trovato il token.</param>
		/// <param name="colonna">Il numero di colonna in cui inizia il token.</param>
		/// <exception cref="ArgumentOutOfRangeException">Viene generata quando i valori di riga o colonna forniti non sono validi.</exception>
		public TokenS(TokenTypeS tipo, string valore, int linea, int colonna) {
			if (linea <= 0) {
				throw new ArgumentOutOfRangeException(nameof(linea), "Il numero di riga deve essere maggiore di zero.");
			}

			if (colonna < 0) {
				throw new ArgumentOutOfRangeException(nameof(colonna), "Il numero di colonna deve essere maggiore o uguale a zero.");
			}

			Tipo = tipo;
			Valore = valore;
			Linea = linea;
			Colonna = colonna;
		}

		/// <summary>
		/// Inizializza una nuova istanza della classe <see cref="TokenS"/>.
		/// </summary>
		/// <param name="tipo">Il tipo di token.</param>
		/// <param name="linea">Il numero di riga in cui è stato trovato il token.</param>
		/// <param name="colonna">Il numero di colonna in cui inizia il token.</param>
		/// <exception cref="ArgumentOutOfRangeException">Viene generata quando i valori di riga o colonna forniti non sono validi.</exception>
		public TokenS(TokenTypeS tipo, int linea, int colonna) {
			if (linea <= 0) {
				throw new ArgumentOutOfRangeException(nameof(linea), "Il numero di riga deve essere maggiore di zero.");
			}

			if (colonna < 0) {
				throw new ArgumentOutOfRangeException(nameof(colonna), "Il numero di colonna deve essere maggiore o uguale a zero.");
			}

			Tipo = tipo;
			Valore = "";
			Linea = linea;
			Colonna = colonna;
		}

		/// <summary>
		/// Crea un token di fine file.
		/// </summary>
		/// <param name="linea">Il numero di riga in cui è stato trovato il token.</param>
		/// <param name="colonna">Il numero di colonna in cui inizia il token.</param>
		/// <returns>Il token di fine file.</returns>
		public static TokenS CreateEndOfFileToken(int linea, int colonna) => new(TokenTypeS.FineRiga, linea, colonna);

		/// <summary>
		/// Restituisce una rappresentazione testuale del token.
		/// </summary>
		/// <returns>Una stringa che rappresenta il token con le informazioni sul tipo, il valore, la riga e la colonna.</returns>
		public override string ToString() => Tipo == TokenTypeS.FineRiga
			? $"Token<{Tipo}, EOF> (Linea: {Linea}, Colonna: {Colonna})"
			: $"Token<{Tipo}, '{Valore}'> (Linea: {Linea}, Colonna: {Colonna})";

		/// <summary>
		/// Determina se l'oggetto specificato è uguale al token corrente.
		/// </summary>
		/// <param name="obj">L'oggetto da confrontare con il token corrente.</param>
		/// <returns><c>true</c> se l'oggetto specificato è uguale al token corrente; in caso contrario, <c>false</c>.</returns>
		public override bool Equals(object? obj) => Equals(obj as TokenS);

		/// <summary>
		/// Determina se il token specificato è uguale al token corrente.
		/// </summary>
		/// <param name="other">Il token da confrontare con il token corrente.</param>
		/// <returns><c>true</c> se il token specificato è uguale al token corrente; in caso contrario, <c>false</c>.</returns>
		public bool Equals(TokenS? other) {
			if (ReferenceEquals(this, other))
				return true;

			if (other is null)
				return false;

			return Tipo.Equals(other.Tipo) &&
				Valore.Equals(other.Valore, StringComparison.Ordinal) && // Usiamo String.Equals con StringComparison.Ordinal
				Linea == other.Linea &&
				Colonna == other.Colonna;
		}

		/// <summary>
		/// Serve come funzione hash per un tipo particolare, adatta per l'uso in algoritmi di hash e strutture dati come una tabella hash.
		/// </summary>
		/// <returns>Un codice hash per il token corrente.</returns>
		public override int GetHashCode() {
			unchecked {
				int hash = 17;
				hash = hash * 23 + Tipo.GetHashCode();
				hash = hash * 23 + (Valore?.GetHashCode() ?? 0);
				hash = hash * 23 + Linea.GetHashCode();
				hash = hash * 23 + Colonna.GetHashCode();
				return hash;
			}
		}


		/// <summary>
		/// Checks whether two tokens are equal.
		/// </summary>
		/// <param name="token1">The first token to compare.</param>
		/// <param name="token2">The second token to compare.</param>
		/// <returns><c>true</c> if the tokens are equal; otherwise, <c>false</c>.</returns>
		public static bool operator ==(TokenS? token1, TokenS? token2) {
			if (ReferenceEquals(token1, token2))
				return true;

			if (token1 is null || token2 is null)
				return false;

			return token1.Equals(token2);
		}

		/// <summary>
		/// Checks whether two tokens are not equal.
		/// </summary>
		/// <param name="token1">The first token to compare.</param>
		/// <param name="token2">The second token to compare.</param>
		/// <returns><c>true</c> if the tokens are not equal; otherwise, <c>false</c>.</returns>
		public static bool operator !=(TokenS? token1, TokenS? token2) => !(token1 == token2);
	}

}