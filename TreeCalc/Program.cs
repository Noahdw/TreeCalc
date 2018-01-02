using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeCalc
{
    class Program
    {
        static void Main(string[] args)
        {

            while (true)
            {
                var inputString = Console.ReadLine();
                Calculator calculator = new Calculator();
                try
                {
                    Console.WriteLine(calculator.Calculate(inputString));
                    //Console.ReadLine();
                }
                catch (InvladUserInputException e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
           
        }
    }
}
