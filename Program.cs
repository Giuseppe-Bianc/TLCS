using BenchmarkDotNet.Running;
using System.Text;

namespace TLCS {
	public static class Program {
		static void Main(string[] args) {
			Lexer lexer = new( Costanti.input);
			List<Token> tokens = lexer.GetTokens();

			// Use string.Join to optimize printing
			string tokensString = string.Join(Environment.NewLine, tokens);

			// Print all tokens at once
			Console.WriteLine(tokensString);
			ASTGenerator astG = new (Costanti.input);
			AstNode root = astG.Parse();
			AstPrinter astPrinter = new();
			string ast = astPrinter.PrintAst(root);
			Console.WriteLine(ast);
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