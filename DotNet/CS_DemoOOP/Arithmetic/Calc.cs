using System;
using System.Collections.Generic;
using System.Text;

namespace CS_DemoOOP.Arithmetic
{
    internal static class Calc
    {
        // Method to add two numbers
        public static int Add(int a, int b)
        {
            return a + b;
        }

        // Method to add three numbers
        public static int Add(int a, int b, int c) // Method overloading: same method name with different number of parameters
        {
            return a + b + c;
        }

        // Method to add two strings (concatenation)
        public static string Add(string str1, string str2) // Method overloading: same method name with different type of parameters
        {
            return $"{str1}-{str2}";
        }

        // Method to subtract two numbers
        public static int Subtract(int a, int b)
        {
            return a - b;
        }


    }
}
