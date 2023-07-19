using System.Text;

namespace TLCS {
	public static class CharExtensions {
		public static string ASCIIUnicode(this char c) {
			int asciiValue = c;
			return $" {c} ASCII: {asciiValue} Unicode: U-{asciiValue:X4}";
		}
	}
	public class Lexer {
		private readonly string _input;
		private readonly int _inputLen;
		private ReadOnlySpan<char> _inputSpan => _input.AsSpan();
		private int _pos;
		private int _linea;
		private int _colonna;
		private StringBuilder builder = new();
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

		private char CarattereCorrente() {
			return _pos >= _inputLen ? '\0' : _inputSpan[_pos];
		}

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
				return new Token(TokenType.FineRiga, string.Empty, _linea, _colonna);
			}

			char currentChar = _inputSpan[_pos];
			Avanza();


			if (char.IsDigit(currentChar)) {
				builder.Clear();
				builder.Append(currentChar);

				while (_pos < _inputLen && (char.IsDigit(_inputSpan[_pos]) || _inputSpan[_pos] == '.')) {
					if (_inputSpan[_pos] == '.') {
						if (_pos + 1 < _inputLen && char.IsDigit(_inputSpan[_pos + 1])) {
							builder.Append(_inputSpan[_pos]);
							Avanza();
						} else {
							break;
						}
					}

					builder.Append(_inputSpan[_pos]);
					Avanza();
				}

				if (_pos < _inputLen && (_inputSpan[_pos] == 'E' || _inputSpan[_pos] == 'e')) {
					builder.Append(_inputSpan[_pos]);
					Avanza();

					if (_pos < _inputLen && (_inputSpan[_pos] == '+' || _inputSpan[_pos] == '-')) {
						builder.Append(_inputSpan[_pos]);
						Avanza();
					}

					while (_pos < _inputLen && char.IsDigit(_inputSpan[_pos])) {
						builder.Append(_inputSpan[_pos]);
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

				while (_pos < _inputLen && (char.IsLetterOrDigit(_inputSpan[_pos]) || _inputSpan[_pos] == '_')) {
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
	}
}