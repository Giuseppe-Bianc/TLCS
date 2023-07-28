using System.Text;

namespace TLCS {
	public sealed class AstPrinter {
		public string PrintAst(AstNode node, int indentation = 0) {
			if (node is null)
				return string.Empty;

			StringBuilder sb = new();
			string indent = new(' ', indentation * 2);

			switch (node.NodeType) {
				case NodeType.IntegerLiteral:
					var intNode = (IntegerLiteralNode)node;
					sb.AppendLine($"{indent}IntegerLiteral: {intNode.Value}");
					break;

				case NodeType.RealLiteral:
					var realNode = (RealLiteralNode)node;
					sb.AppendLine($"{indent}RealLiteral: {realNode.Value}");
					break;

				case NodeType.Identifier:
					var idNode = (IdentifierNode)node;
					sb.AppendLine($"{indent}Identifier: {idNode.Name}");
					break;

				case NodeType.BinaryOperation:
					var binOpNode = (BinaryOperationNode)node;
					sb.AppendLine($"{indent}BinaryOperation: {binOpNode.OperatorToken.Valore}");
					sb.Append(PrintAst(binOpNode.Left, indentation + 1));
					sb.Append(PrintAst(binOpNode.Right, indentation + 1));
					break;

				case NodeType.UnaryOperation:
					var unaryOpNode = (UnaryOperationNode)node;
					sb.AppendLine($"{indent}UnaryOperation: {unaryOpNode.OperatorToken.Valore}");
					sb.Append(PrintAst(unaryOpNode.Operand, indentation + 1));
					break;

				case NodeType.ParenthesizedOperation:
					var parenNode = (ParenthesizedOperationNode)node;
					sb.AppendLine($"{indent}ParenertesiedOperation:");
					sb.Append(PrintAst(parenNode.Expression, indentation + 1));
					break;

				default:
					sb.AppendLine($"{indent}Unknown node type");
					break;
			}

			return sb.ToString();
		}
	}
}
