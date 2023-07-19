using BenchmarkDotNet.Running;

namespace TLCS {
	public static class Program {
		static void Main(string[] args) {
			string input = "3.133  + 3.133E-1 + 3.133e-1 + 3.133E+2 + 3.133e-2 + 4 * (2 - 1)^2";
			//string input = "#a222 + #1a.22a";
			Lexer lexer = new(input);
			var tokens = lexer.GetTokens();

			foreach (var token in tokens) {
				Console.WriteLine(token);
			}

			/*NewLexer lexere = new(input);
			var tokense = lexer.GetTokens();

			foreach (var token in tokense) {
				Console.WriteLine(token);
			}*/
			/*LexerMod lexer = new (input);
			var tokens = lexer.Tokenize();
			tokens.ToList().ForEach(t => Console.WriteLine(t));*/
			Console.WriteLine("Premi Invio per continuare...");
			if (Console.ReadLine().Equals("e")) {
				Environment.Exit(0);
			} else {
				Console.Clear();
			}
			var summary = BenchmarkRunner.Run<Bencmarks>();
		}
	}
}