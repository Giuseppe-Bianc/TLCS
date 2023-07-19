using System.Text;


namespace TLCS {

	public class NewLexer {
		private readonly String _input;
		private readonly int _inputLen;
		private int _pos;
		private int _linea;
		private int _colonna;

		public NewLexer(string input) {
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
			return _pos >= _inputLen ? '\0' : _input[_pos];
		}

		private void IgnoraSpazi() {
			while (_pos < _inputLen && char.IsWhiteSpace(CarattereCorrente())) {
				if (CarattereCorrente() == '\n') {
					_linea++;
					_colonna = 1;
				}

				Avanza();
			}
		}

		public Token GetNextToken() {
			ReadOnlySpan<char> inputSpan = _input.AsSpan();
			IgnoraSpazi();

			if (_pos >= _inputLen) {
				return new Token(TokenType.FineRiga, string.Empty, _linea, _colonna);
			}

			char currentChar = inputSpan[_pos];
			Avanza();

			if (char.IsDigit(currentChar)) {
				int start = _pos;

				while (_pos < _inputLen && (char.IsDigit(inputSpan[_pos]) || inputSpan[_pos] == '.')) {
					if (inputSpan[_pos] == '.') {
						if (_pos + 1 < _inputLen && char.IsDigit(inputSpan[_pos + 1])) {
							Avanza();
						} else {
							break;
						}
					}

					Avanza();
				}

				if (_pos < _inputLen && (inputSpan[_pos] == 'E' || inputSpan[_pos] == 'e')) {
					Avanza();

					if (_pos < _inputLen && (inputSpan[_pos] == '+' || inputSpan[_pos] == '-')) {
						Avanza();
					}

					while (_pos < _inputLen && char.IsDigit(inputSpan[_pos])) {
						Avanza();
					}
				}

				string numero = inputSpan[start.._pos].ToString();

				if (numero.Contains('.')) {
					return new Token(TokenType.Reale, numero, _linea, _colonna - numero.Length);
				}

				return new Token(TokenType.Intero, numero, _linea, _colonna - numero.Length);
			}

			if (char.IsLetter(currentChar)) {
				int start = _pos;

				while (_pos < _inputLen && (char.IsLetterOrDigit(inputSpan[_pos]) || inputSpan[_pos] == '_')) {
					Avanza();
				}

				string identificatore = inputSpan[start.._pos].ToString();

				return new Token(TokenType.Identificatore, identificatore, _linea, _colonna - identificatore.Length);
			}

			return currentChar switch {
				'+' or '-' or '*' or '/' or '(' or ')' or '^' => new Token(TokenType.Operatore, currentChar.ToString(), _linea, _colonna - 1),
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
