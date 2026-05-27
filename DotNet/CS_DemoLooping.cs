using System;

namespace CS_DemoLooping
{
	class Program
	{
		static void Main()
		{
			// DemoFor();
			// DemoWhile();
			// DemoDoWhile();
			// DemoForEach();
			// DemoBreak();
			DemoContinue();
		}
		
		static void DemoFor()
		{
			for(int i = 1; i <= 5; i++)
			{
				Console.WriteLine("Count: {0}", i);
			}
		}
		
		static void DemoWhile()
		{
			int i = 1;
			while(i <= 5)
			{
				Console.WriteLine("Value: {0}", i++);
				//i++;
			}
		}
		
		static void DemoDoWhile()
		{
			int i = 6;
			do
			{
				Console.WriteLine("Value: {0}", i++);
				//i++;
			} while(i <= 5);
		}
		
		static void DemoForEach()
		{
			int[] numbers={10, 20, 30, 40, 50};
			
			foreach(int num in numbers)
			{
				Console.WriteLine(num);
			}
		}
		
		static void DemoBreak()
		{
			for (int i = 1; i <= 5; i++)
			{
				if (i == 3)
					break;

				Console.WriteLine(i);
			}
		}
		
		static void DemoContinue()
		{
			for (int i = 1; i <= 5; i++)
			{
				if (i == 3)
					continue;

				Console.WriteLine(i);
			}
		}
	}
}