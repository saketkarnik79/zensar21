using System;

namespace CS_DemoPointers
{
    class Program
    {
        static void Main(string[] args)
        {
            int x = 10;

            unsafe
            {
                int* p = &x; // Get the address of x
                Console.WriteLine($"Value of x: {x}"); // Output: 10
                Console.WriteLine($"Address of x: {(long)p:X}"); // Output: Address in hexadecimal
                *p = 20; // Change the value at the address pointed to by p
            }
            Console.WriteLine($"New value of x: {x}"); // Output: 20

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}