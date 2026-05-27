using System;

namespace CS_DemoCalcUsingIf
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
			
			if(op == '+')
			{
				result = x + y;
			}			
			else if(op == '-')
			{
				result = x - y;
			}
			else if(op == '*')
			{
				result = x * y;
			}
			else if(op == '/')
			{
				result = (float)x / y;
			}
			else if(op == '%')
			{
				result = x % y;
			}
			else
			{
				Console.WriteLine("Invalid operator.");
				error = true;
			}	
			Console.WriteLine();
			
			if(!error)
			{
				Console.WriteLine("{0} {1} {2} = {3}", x, op, y, result);
			}
		}
	}
}