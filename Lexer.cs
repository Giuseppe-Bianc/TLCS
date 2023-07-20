using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace TLCS {
	public static class CharExtensions {
		public static string ASCIIUnicode(this char c) {
			int asciiValue = c;
			return $" {c} ASCII: {asciiValue} Unicode: U-{asciiValue:X4}";
		}
	}
	public class Lexer {
		private readonly String _input;
		private readonly int _inputLen;
		private readonly StringBuilder builder = new();
		private ReadOnlySpan<char> InputSpan => _input.AsSpan();
		private int _pos;
		private int _linea;
		private int _colonna;
		public Lexer([ConstantExpected]String input) {
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

		private char CarattereCorrente() {
			return _pos >= _inputLen ? '\0' : InputSpan[_pos];
		}

		private char CarattereCorrenteMod() => _pos >= _inputLen ? '\0' : InputSpan[_pos];

		private void IgnoraSpazi() {
			while (char.IsWhiteSpace(CarattereCorrente())) {
				if (CarattereCorrente() == '\n') {
					_linea++;
					_colonna = 1;
				}

				Avanza();
			}
		}

		private void IgnoraSpaziMod() {
			while (char.IsWhiteSpace(CarattereCorrenteMod())) {
				if (CarattereCorrenteMod() == '\n') {
					_linea++;
					_colonna = 1;
				}

				Avanza();
			}
		}


		public Token GetNextToken() {
			IgnoraSpazi();

			if (_pos >= _inputLen) {
				return new Token(TokenType.FineRiga, string.Empty, _linea, _colonna);
			}

			char currentChar = InputSpan[_pos];
			Avanza();


			if (char.IsDigit(currentChar)) {
				builder.Clear();
				builder.Append(currentChar);

				while (_pos < _inputLen && (char.IsDigit(InputSpan[_pos]) || InputSpan[_pos] == '.')) {
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

				if (_pos < _inputLen && (InputSpan[_pos] == 'E' || InputSpan[_pos] == 'e')) {
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

				if (numero.Contains('.')) {
					return new Token(TokenType.Reale, numero, _linea, _colonna - numero.Length);
				}

				return new Token(TokenType.Intero, numero, _linea, _colonna - numero.Length);
			}

			if (char.IsLetter(currentChar)) {
				builder.Clear();
				builder.Append(currentChar);

				while (_pos < _inputLen && (char.IsLetterOrDigit(InputSpan[_pos]) || InputSpan[_pos] == '_')) {
					builder.Append(currentChar);
					Avanza();
				}

				string identificatore = builder.ToString();

				return new Token(TokenType.Identificatore, identificatore, _linea, _colonna - identificatore.Length);
			}

			return currentChar switch {
				'+' or '-' or '*' or '/' or '(' or ')' or '^' or '#' => new Token(TokenType.Operatore, currentChar.ToString(), _linea, _colonna - 1),
				_ => throw new InvalidOperationException($"Carattere non riconosciuto:{currentChar.ASCIIUnicode()}"),
			};
		}

		public Token GetNextTokenMod() {
			IgnoraSpaziMod();

			if (_pos >= _inputLen) {
				return new Token(TokenType.FineRiga, string.Empty, _linea, _colonna);
			}

			char currentChar = InputSpan[_pos];
			Avanza();


			if (char.IsDigit(currentChar)) {
				builder.Clear();
				builder.Append(currentChar);

				while (_pos < _inputLen && (char.IsDigit(InputSpan[_pos]) || InputSpan[_pos] == '.')) {
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

				if (_pos < _inputLen && (char.ToLower(InputSpan[_pos]).Equals('e'))) {
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

				if (numero.Contains('.')) {
					return new Token(TokenType.Reale, numero, _linea, _colonna - numero.Length);
				}

				return new Token(TokenType.Intero, numero, _linea, _colonna - numero.Length);
			}

			if (char.IsLetter(currentChar)) {
				builder.Clear();
				builder.Append(currentChar);

				while (_pos < _inputLen && (char.IsLetterOrDigit(InputSpan[_pos]) || InputSpan[_pos] == '_')) {
					builder.Append(currentChar);
					Avanza();
				}

				string identificatore = builder.ToString();

				return new Token(TokenType.Identificatore, identificatore, _linea, _colonna - identificatore.Length);
			}

			return currentChar switch {
				'+' or '-' or '*' or '/' or '(' or ')' or '^' or '#' => new Token(TokenType.Operatore, currentChar.ToString(), _linea, _colonna - 1),
				_ => throw new InvalidOperationException($"Carattere non riconosciuto:{currentChar.ASCIIUnicode()}"),
			};
		}


		public List<Token> GetTokens() {
			List<Token> tokens = new();

			Token token;
			while ((token = GetNextToken()).Tipo != TokenType.FineRiga) {
				tokens.Add(token);
			}

			return tokens;
		}

		public List<Token> GetTokensMod() {
			List<Token> tokens = new();

			Token token;
			while ((token = GetNextTokenMod()).Tipo != TokenType.FineRiga) {
				tokens.Add(token);
			}

			return tokens;
		}

	}
}