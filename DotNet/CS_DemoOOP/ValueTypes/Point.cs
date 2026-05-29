using System;

namespace CS_DemoOOP.ValueTypes
{
    struct Point
    {
        //public int x;
        //public int y;

        private int x;
        private int y;

        public Point() // Default constructor is required for struct, but we can also define our own parameterized constructor
        {
            this.X = 0;
            this.Y = 0;
        }

        public Point(int x, int y) // Parameterized constructor to initialize the point with specific values
        {
            this.X = x; // Using property setter
            this.Y = y; // Using property setter
        }

        public int X
        {
            get
            {
                // Auth check can be done here before returning the value of x 
                return x;
            }
            set
            {
                // Auth check can be done here before setting the value of x 
                if (value < 0)
                {
                    Console.WriteLine("X cannot be negative. Setting to 0.");
                    x = 0;
                }
                else
                {
                    x = value;
                }
            }
        }

        public int Y
        {
            get
            {
                // Auth check can be done here before returning the value of y 
                return y;
            }
            set
            {
                // Auth check can be done here before setting the value of y 
                if (value < 0)
                {
                    Console.WriteLine("Y cannot be negative. Setting to 0.");
                    y = 0;
                }
                else
                {
                    y = value;
                }
            }
        }

        override public string ToString()
        {
            return $"Point: ({X}, {Y})";
        }
    }
}