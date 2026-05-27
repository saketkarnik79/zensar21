using System;

namespace CS_DemoAddingTwoNumbers
{
	class Program
	{
		static void Main()
		{
			int x;
			int y;
			int sum;
			
			Console.Write("Enter First Number: ");
			x = int.Parse(Console.ReadLine());
			
			Console.Write("Enter Second Number: ");
			y = int.Parse(Console.ReadLine());
			
			sum = x + y;
			
			Console.WriteLine();
			
			Console.WriteLine("{0} + {1} = {2}", x, y, sum);
		}
	}
}