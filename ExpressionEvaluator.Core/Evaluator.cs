using System.Globalization;

namespace ExpressionEvaluator.Core;

public class Evaluator
{
    public static double Evaluate(string infix)
    {
        var postfix = InfixToPostfix(infix);
        return EvaluatePostfix(postfix);
    }

    private static string InfixToPostfix(string infix)
    {
        var stack = new Stack<char>(100);
        var postfix = string.Empty;
        var numberBuffer = string.Empty;

        for (int i = 0; i < infix.Length; i++)
        {
            if (infix[i] == ' ')
                continue;

            if (char.IsDigit(infix[i]) || infix[i] == '.')
            {
                numberBuffer += infix[i];
            }
            else
            {
                if (!string.IsNullOrEmpty(numberBuffer))
                {
                    postfix += numberBuffer + ' ';
                    numberBuffer = string.Empty;
                }

                if (IsOperator(infix[i]))
                {
                    if (stack.Count == 0)
                    {
                        stack.Push(infix[i]);
                    }
                    else
                    {
                        if (infix[i] == ')')
                        {
                            do
                            {
                                postfix += stack.Pop();
                            }
                            while (stack.Peek() != '(');
                            stack.Pop();
                        }
                        else
                        {
                            if (PriorityInfix(infix[i]) > PriorityStack(stack.Peek()))
                            {
                                stack.Push(infix[i]);
                            }
                            else
                            {
                                postfix += stack.Pop();
                                stack.Push(infix[i]);
                            }
                        }
                    }
                }
            }
        }
        if (!string.IsNullOrEmpty(numberBuffer))
        {
            postfix += numberBuffer + " ";
        }
        while (stack.Count != 0)
        {
            postfix += stack.Pop() + " ";
        }
        return postfix;
    }

    private static bool IsOperator(char item) => "+-*/^()".Contains(item);

    private static int PriorityStack(char item) => item switch
    {
        '^' => 3,
        '*' or '/' or '%' => 2,
        '+' or '-' => 1,
        '(' => 0,
        _ => throw new Exception("Sintax error."),
    };

    private static int PriorityInfix(char item) => item switch
    {
        '^' => 4,
        '*' or '/' or '%' => 2,
        '+' or '-' => 1,
        '(' => 5,
        _ => throw new Exception("Sintax error."),
    };

    private static double EvaluatePostfix(string postfix)
    {
        var stack = new Stack<double>(100);
        var numberBuffer = string.Empty;

        for (int i = 0; i < postfix.Length; i++)
        {
            if (char.IsDigit(postfix[i]) || postfix[i] == '.')
            {
                numberBuffer += postfix[i];
            }
            else if (postfix[i] == ' ')
            {
                if (!string.IsNullOrEmpty(numberBuffer))
                {
                    stack.Push(double.Parse(numberBuffer, CultureInfo.InvariantCulture));
                    numberBuffer = string.Empty;
                }
            }
            else if (IsOperator(postfix[i]))
            {
                var number2 = stack.Pop();
                var number1 = stack.Pop();
                var result = Calculate(number1, postfix[i], number2);
                stack.Push(result);
            }
        }
        if (!string.IsNullOrEmpty(numberBuffer))
        {
            stack.Push(double.Parse(numberBuffer, CultureInfo.InvariantCulture));
        }
        return stack.Pop();
    }

    private static double Calculate(double number1, char item, double number2) => item switch
    {
        '^' => Math.Pow(number1, number2),
        '*' => number1 * number2,
        '/' => number1 / number2,
        '+' => number1 + number2,
        '-' => number1 - number2,
        _ => throw new Exception("Invalid Operator"),
    };
}
