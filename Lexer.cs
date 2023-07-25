using System.Runtime.InteropServices;
using System.Text;

namespace TLCS {
	public static class Extensions {
		public static string ASCIIUnicode(this char c) {
			int asciiValue = c;
			return $" {c} ASCII: {asciiValue} Unicode: U-{asciiValue:X4}";
		}

		public static bool IsLetterOrDigitOrUnderscore(this char c) {
			return char.IsLetterOrDigit(c) || c == '_';
		}

		public static bool IsNumberOrPoint(this char c) {
			return char.IsDigit(c) || c == '.';
		}
		public static bool EqualsIgnoreCase(this string str1, string str2) {
			return string.Equals(str1, str2, StringComparison.OrdinalIgnoreCase);
		}

		public static bool EqualsIgnoreCase(this char c1, char c2) {
			return char.ToUpperInvariant(c1).Equals(char.ToUpperInvariant(c2));
		}
	}
	public class Lexer {
		private readonly string _input;
		private readonly int _inputLen;
		private readonly StringBuilder builder = new();
		private ReadOnlySpan<char> InputSpan => _input.AsSpan();
		private int _pos;
		private int _linea;
		private int _colonna;
		public Lexer(string input) {
			_input = input;
			_inputLen = input.Length;
			_pos = 0;
			_linea = 1;
			_colonna = 1;
		}

		private void Avanza() {
			_pos++;
			_colonna++;
		}

		private char CarattereCorrente() => _pos >= _inputLen ? '\0' : InputSpan[_pos];

		private void IgnoraSpazi() {
			while (char.IsWhiteSpace(CarattereCorrente())) {
				if (CarattereCorrente() == '\n') {
					_linea++;
					_colonna = 1;
				}

				Avanza();
			}
		}

		public Token GetNextToken() {
			IgnoraSpazi();

			if (_pos >= _inputLen) {
				return Token.CreateEndOfFileToken(_linea, _colonna);
			}

			char currentChar = InputSpan[_pos];
			Avanza();

			return currentChar switch {
				var digit when char.IsDigit(digit) => ProcessNumberToken(currentChar),
				var letter when char.IsLetter(letter) => ProcessIdentifierToken(currentChar),
				'+' or '-' or '*' or '/' or '(' or ')' or '^' or '#' => new Token(TokenType.Operatore, currentChar.ToString(), _linea, _colonna - 1),
				_ => throw new InvalidOperationException($"Carattere non riconosciuto:{currentChar.ASCIIUnicode()}"),
			};
		}

		public TokenE GetNextTokenE() {
			IgnoraSpazi();

			if (_pos >= _inputLen) {
				return TokenE.CreateEndOfFileToken(_linea, _colonna);
			}

			char currentChar = InputSpan[_pos];
			Avanza();

			return currentChar switch {
				var digit when char.IsDigit(digit) => ProcessNumberTokenE(currentChar),
				var letter when char.IsLetter(letter) => ProcessIdentifierTokenE(currentChar),
				'+' or '-' or '*' or '/' or '(' or ')' or '^' or '#' => new TokenE(TokenTypeE.Operatore, currentChar.ToString(), _linea, _colonna - 1),
				_ => throw new InvalidOperationException($"Carattere non riconosciuto:{currentChar.ASCIIUnicode()}"),
			};
		}

		public TokenS GetNextTokenS() {
			IgnoraSpazi();

			if (_pos >= _inputLen) {
				return TokenS.CreateEndOfFileToken(_linea, _colonna);
			}

			char currentChar = InputSpan[_pos];
			Avanza();

			return currentChar switch {
				var digit when char.IsDigit(digit) => ProcessNumberTokenS(currentChar),
				var letter when char.IsLetter(letter) => ProcessIdentifierTokenS(currentChar),
				'+' or '-' or '*' or '/' or '(' or ')' or '^' or '#' => new TokenS(TokenTypeS.Operatore, currentChar.ToString(), _linea, _colonna - 1),
				_ => throw new InvalidOperationException($"Carattere non riconosciuto:{currentChar.ASCIIUnicode()}"),
			};
		}

		private Token ProcessNumberToken(char startChar) {
			builder.Clear();
			builder.Append(startChar);

			while (_pos < _inputLen && InputSpan[_pos].IsNumberOrPoint()) {
				if (InputSpan[_pos] == '.') {
					if (_pos + 1 < _inputLen && char.IsDigit(InputSpan[_pos + 1])) {
						builder.Append(InputSpan[_pos]);
						Avanza();
					} else {
						break;
					}
				}

				builder.Append(InputSpan[_pos]);
				Avanza();
			}

			if (_pos < _inputLen && (InputSpan[_pos].EqualsIgnoreCase('e'))) {
				builder.Append(InputSpan[_pos]);
				Avanza();

				if (_pos < _inputLen && (InputSpan[_pos] == '+' || InputSpan[_pos] == '-')) {
					builder.Append(InputSpan[_pos]);
					Avanza();
				}

				while (_pos < _inputLen && char.IsDigit(InputSpan[_pos])) {
					builder.Append(InputSpan[_pos]);
					Avanza();
				}
			}

			string numero = builder.ToString();

			return numero.Contains('.') ?
				new Token(TokenType.Reale, numero, _linea, _colonna - numero.Length) :
				new Token(TokenType.Intero, numero, _linea, _colonna - numero.Length);
		}

		private TokenE ProcessNumberTokenE(char startChar) {
			builder.Clear();
			builder.Append(startChar);

			while (_pos < _inputLen && InputSpan[_pos].IsNumberOrPoint()) {
				if (InputSpan[_pos] == '.') {
					if (_pos + 1 < _inputLen && char.IsDigit(InputSpan[_pos + 1])) {
						builder.Append(InputSpan[_pos]);
						Avanza();
					} else {
						break;
					}
				}

				builder.Append(InputSpan[_pos]);
				Avanza();
			}

			if (_pos < _inputLen && (InputSpan[_pos].EqualsIgnoreCase('e'))) {
				builder.Append(InputSpan[_pos]);
				Avanza();

				if (_pos < _inputLen && (InputSpan[_pos] == '+' || InputSpan[_pos] == '-')) {
					builder.Append(InputSpan[_pos]);
					Avanza();
				}

				while (_pos < _inputLen && char.IsDigit(InputSpan[_pos])) {
					builder.Append(InputSpan[_pos]);
					Avanza();
				}
			}

			string numero = builder.ToString();

			return numero.Contains('.') ?
				new TokenE(TokenTypeE.Reale, numero, _linea, _colonna - numero.Length) :
				new TokenE(TokenTypeE.Intero, numero, _linea, _colonna - numero.Length);
		}

		private TokenS ProcessNumberTokenS(char startChar) {
			builder.Clear();
			builder.Append(startChar);

			while (_pos < _inputLen && InputSpan[_pos].IsNumberOrPoint()) {
				if (InputSpan[_pos] == '.') {
					if (_pos + 1 < _inputLen && char.IsDigit(InputSpan[_pos + 1])) {
						builder.Append(InputSpan[_pos]);
						Avanza();
					} else {
						break;
					}
				}

				builder.Append(InputSpan[_pos]);
				Avanza();
			}

			if (_pos < _inputLen && (InputSpan[_pos].EqualsIgnoreCase('e'))) {
				builder.Append(InputSpan[_pos]);
				Avanza();

				if (_pos < _inputLen && (InputSpan[_pos] == '+' || InputSpan[_pos] == '-')) {
					builder.Append(InputSpan[_pos]);
					Avanza();
				}

				while (_pos < _inputLen && char.IsDigit(InputSpan[_pos])) {
					builder.Append(InputSpan[_pos]);
					Avanza();
				}
			}

			string numero = builder.ToString();

			return numero.Contains('.') ?
				new TokenS(TokenTypeS.Reale, numero, _linea, _colonna - numero.Length) :
				new TokenS(TokenTypeS.Intero, numero, _linea, _colonna - numero.Length);
		}

		private Token ProcessIdentifierToken(char startChar) {
			builder.Clear();
			builder.Append(startChar);

			while (_pos < _inputLen && InputSpan[_pos].IsLetterOrDigitOrUnderscore()) {
				builder.Append(InputSpan[_pos]);
				Avanza();
			}

			string identificatore = builder.ToString();

			return new Token(TokenType.Identificatore, identificatore, _linea, _colonna - identificatore.Length);
		}

		private TokenE ProcessIdentifierTokenE(char startChar) {
			builder.Clear();
			builder.Append(startChar);

			while (_pos < _inputLen && InputSpan[_pos].IsLetterOrDigitOrUnderscore()) {
				builder.Append(InputSpan[_pos]);
				Avanza();
			}

			string identificatore = builder.ToString();

			return new TokenE(TokenTypeE.Identificatore, identificatore, _linea, _colonna - identificatore.Length);
		}


		private TokenS ProcessIdentifierTokenS(char startChar) {
			builder.Clear();
			builder.Append(startChar);

			while (_pos < _inputLen && InputSpan[_pos].IsLetterOrDigitOrUnderscore()) {
				builder.Append(InputSpan[_pos]);
				Avanza();
			}

			string identificatore = builder.ToString();

			return new TokenS(TokenTypeS.Identificatore, identificatore, _linea, _colonna - identificatore.Length);
		}

		public List<Token> GetTokens() {
			List<Token> tokens = new();

			Token token;
			while ((token = GetNextToken()).Tipo != TokenType.FineRiga) {
				tokens.Add(token);
			}

			return tokens;

		}

		public List<TokenE> GetTokensE() {
			List<TokenE> tokens = new();

			TokenE token;
			while ((token = GetNextTokenE()).Tipo != TokenTypeE.FineRiga) {
				tokens.Add(token);
			}

			return tokens;

		}

		public List<TokenS> GetTokensS() {
			List<TokenS> tokens = new();

			TokenS token;
			while ((token = GetNextTokenS()).Tipo != TokenTypeS.FineRiga) {
				tokens.Add(token);
			}

			return tokens;

		}
	}
}