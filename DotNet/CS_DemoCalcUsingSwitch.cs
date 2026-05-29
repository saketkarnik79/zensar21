using System;

namespace CS_DemoCalcUsingSwitch
{
	class Program
	{
		static void Main()
		{
			int x;
			int y;
			char op;
			float result = 0;
			bool error = false;
			
			Console.Write("Enter First Number: ");
			x = int.Parse(Console.ReadLine());
			
			Console.Write("Enter Second Number: ");
			y = int.Parse(Console.ReadLine());
			
			Console.Write("Enter the operator <+, -, *, /, %>: ");
			op = char.Parse(Console.ReadLine());
			
			switch(op)
			{
				case '+':
					result = x + y;
					break;					
				case '-':
					result = x - y;
					break;
				case '*':
					result = x * y;
					break;
				case '/':
					try
					{
						result = (float)x / y;
					}
					catch(DivideByZeroException ex)
					{
						Console.WriteLine("Can't divide by zero");
					}
					finally
					{
						Console.WriteLine("Execution completed. Cleaning up resources.");
					}
					break;
				case '%':
					result = x % y;
					break;
				default:
					Console.WriteLine("Invalid operator.");
					break;
			}	
			Console.WriteLine();
			
			if(!error)
			{
				Console.WriteLine("{0} {1} {2} = {3}", x, op, y, result);
			}
		}
	}
}