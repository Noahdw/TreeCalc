using System;
using System.Collections.Generic;
using System.Linq;

namespace TreeCalc
{
    class Tree
    {
        public Tree Left;
        public Tree Right;
        public Tree Previous;
        public string Value;
    }
    class Calculator
    {
        private Dictionary<string, int> map = new Dictionary<string, int>()
        {
            {"(",-1},
            {")",-1},
            {"+",0 },
            {"-",0 },
            {"*",1 },
            {"/",1 },
            {"^",2 },
            {"sin",2 },
            {"cos",2 },
            {"tan",2 },
            {"sec",2 },
            {"csc",2 },
            {"cot",2 }
        };

        //Uses the Shunting-yard algorithm
        public double Calculate(string input)
        {
            Stack<string> stack = new Stack<string>();
            Queue<string> queue = new Queue<string>();
           
            if(input.Count(x => x == '(') != input.Count(x => x == ')'))
            {
                InvladUserInputException e = new InvladUserInputException("Uneven number of brackets");
                throw e;
            }
            char c;
            for (var n = 0; n < input.Length; ++n )
            {   // 0 - 9 in ascii
                c = input[n];
                char tempC = c;
                string snumber = "";
                while (tempC >= 48 && tempC <= 57 && n < input.Length)
                {
                    snumber += tempC.ToString();
                    if (!(++n < input.Length))
                    {
                        continue;
                    }
                    tempC = input[n];
                }
                if (snumber != "")
                {
                    queue.Enqueue(snumber);
                    --n;
                    continue;
                }
         
              
                switch (c)
                {
                    case '+':
                    case '-':
                    case '*':
                    case '/':
                    case '^':
                        while (stack.Count > 0 && HasGreaterPrecedence(stack.Peek(),c.ToString()))
                        {
                            queue.Enqueue(stack.Pop());
                        }
                        stack.Push(c.ToString());
                        break;
                    case '(':
                    case ')':
                        if (c =='(')
                        {
                            stack.Push(c.ToString());
                        }
                        else if (c == ')')
                        {
                            while (stack.Peek() != "(")
                            {
                                queue.Enqueue(stack.Pop());
                            }
                            stack.Pop();
                        }
                        continue;
                }
                string trigChar = c.ToString().ToLower();
                if (trigChar == "s" || trigChar == "c" || trigChar == "t")
                {
                    trigChar = input.Substring(n, 3).ToLower();
                    n += 2;
                    switch (trigChar)
                    {
                        case "sin":
                            stack.Push(trigChar);
                            break;
                        case "cos":
                            stack.Push(trigChar);
                            break;
                        case "tan":
                            stack.Push(trigChar);
                            break;
                        case "sec":
                            stack.Push(trigChar);
                            break;
                        case "csc":
                            stack.Push(trigChar);
                            break;
                        case "cot":
                            stack.Push(trigChar);
                            break;
                        default:
                            InvladUserInputException e = new InvladUserInputException("Likely an invalid trig function");
                            throw e;
                    }
                }
            }
            while (stack.Count > 0)
            {
                queue.Enqueue(stack.Pop());
            }
            foreach (var variable in queue)
            {
             //Console.WriteLine(variable);
                stack.Push(variable);
            }
           
            Tree root = new Tree();
            Tree cursor = root;
            root.Value = stack.Pop();
            foreach (var v in stack)
            {
                if (cursor.Right == null)
                {
                    cursor.Right = new Tree {Value = v};

                    if (IsTrigFunction(v))
                    {
                        Tree t = cursor;
                        cursor = cursor.Right;
                        cursor.Previous = t;
                        cursor.Left = new Tree {Value = "N/A"};
                        continue;
                        
                    }
                    else if(IsOperator(v))
                    {
                        Tree temp = cursor;
                        cursor = cursor.Right;
                        cursor.Previous = temp;
                    }

                }
                else if (cursor.Left == null)
                {
                    cursor.Left = new Tree {Value = v};
                    if (IsTrigFunction(v))
                    {
                        Tree t = cursor;
                        cursor = cursor.Left;
                        cursor.Previous = t;
                        cursor.Left = new Tree { Value = "N/A" };
                        continue;

                    }
                   else if (IsOperator(v))
                    {
                        Tree temp = cursor;
                        cursor = cursor.Left;
                        cursor.Previous = temp;
                    }
                 
                }
                while (cursor.Previous != null && cursor.Left != null)
                {
                    cursor = cursor.Previous;
                }
             
            }
            //Console.ReadLine();
            return PostOrderTraversal(root);
        }

        private bool HasGreaterPrecedence(string left, string right)
        {
            if (map[left] > map[right])
            {
                return true;
            }
            if (map[left] == map[right] && left != "^")
            {
                return true;
            }
            return false;
        }

        private bool IsOperator(string c)
        {
            return (c == "+" || c == "-" || c == "*" || c == "/" || c == "^");
        }
        private bool IsTrigFunction(string c)
        {
            return (c == "sin" || c == "tan" || c == "cos" || c == "csc" || c == "sec" || c == "cot");
        }
        private double PostOrderTraversal(Tree root)
        {
            if (IsOperator(root.Value) || IsTrigFunction(root.Value))
            {
                switch (root.Value)
                {
                    case "+":
                        return PostOrderTraversal(root.Left) + PostOrderTraversal(root.Right);
                    case "-":
                        return PostOrderTraversal(root.Left) - PostOrderTraversal(root.Right);
                    case "*":
                        return PostOrderTraversal(root.Left) * PostOrderTraversal(root.Right);
                    case "/":
                        return PostOrderTraversal(root.Left) / PostOrderTraversal(root.Right);
                    case "^":
                        return Math.Pow(PostOrderTraversal(root.Left), PostOrderTraversal(root.Right));
                    case "sin":
                        return Math.Sin(PostOrderTraversal(root.Right));
                    case "cos":
                        return Math.Cos(PostOrderTraversal(root.Right));
                    case "tan":
                        return Math.Tan(PostOrderTraversal(root.Right));
                    case "csc":
                        return 1 / Math.Sin(PostOrderTraversal(root.Right));
                    case "sec":
                        return 1 / Math.Cos(PostOrderTraversal(root.Right));
                    case "cot":
                        return 1 / Math.Tan(PostOrderTraversal(root.Right));
                    default:
                        InvladUserInputException e = new InvladUserInputException("Unexpected value in PostOrderTraversal");
                        throw e; ;
                }
            }
            return Convert.ToDouble(root.Value);
        }
    }


    public class InvladUserInputException : Exception
    {
        public InvladUserInputException(string error)
            : base(error)
        { 
        }
    }
}
