namespace TLCS {
	public static class Evaluator {
		public static double Evaluate(AstNode node) {
			return node switch {
				IntegerLiteralNode intLiteralNode => intLiteralNode.Value,
				RealLiteralNode realLiteralNode => realLiteralNode.Value,
				IdentifierNode identifierNode => throw new InvalidOperationException($"Identifier '{identifierNode.Name}' not supported in evaluation."),
				BinaryOperationNode binaryOpNode => EvaluateBinaryOperation(binaryOpNode.OperatorToken.Valore, Evaluate(binaryOpNode.Left), Evaluate(binaryOpNode.Right)),
				UnaryOperationNode unaryOpNode => EvaluateUnaryOperation(unaryOpNode.OperatorToken.Valore, Evaluate(unaryOpNode.Operand)),
				ParenthesizedOperationNode parenOpNode => Evaluate(parenOpNode.Expression),
				_ => throw new InvalidOperationException($"Unsupported node type: {node.NodeType}")
			};
		}

		private static double EvaluateBinaryOperation(string op, double left, double right) {
			return op switch {
				"+" => left + right,
				"-" => left - right,
				"*" => left * right,
				"/" => left / right,
				"^" => Math.Pow(left, right),
				_ => throw new InvalidOperationException($"Unsupported binary operator: {op}")
			};
		}

		private static double EvaluateUnaryOperation(string op, double operand) {
			return op switch {
				"+" => operand,
				"-" => -operand,
				_ => throw new InvalidOperationException($"Unsupported unary operator: {op}")
			};
		}
	}
}
