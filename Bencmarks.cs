using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using System.Text;

namespace TLCS {
	[MemoryDiagnoser]
	[Orderer(SummaryOrderPolicy.FastestToSlowest)]
	[RankColumn]
	[StopOnFirstError]
	[SimpleJob(warmupCount: 15, iterationCount: 30)]
	public class Bencmarks {
		[Params(1, 100)]
		public int COUNT { get; set; }
		private Lexer lexer;

		public static void ConstructImput(int n, StringBuilder inp) {
			for (int i = 0; i < n; ++i) {
				inp.AppendLine(Costanti.input);
			}
		}

		[GlobalSetup]
		public void Setup() {
			StringBuilder inp = new();
			switch (COUNT) {
				case 1:
					ConstructImput(1, inp);
					break;
				case 100:
					ConstructImput(100, inp);
					break;
			}

			lexer = new Lexer(inp.ToString());
		}

		[Benchmark(Baseline = true)]
		public void LexerPerf() {
			lexer.GetTokens();
		}

		/*[Benchmark]
		public void LexerModPerf() {
			lexer.GetTokensM();
		}*/

		/*[Benchmark]
		public void AstGeneratorPerf() {
			astPrinter.PrintAst(ast);
		}*/

	}
}
