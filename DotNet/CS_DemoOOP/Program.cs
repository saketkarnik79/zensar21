using System;
using CS_DemoOOP.ValueTypes;
using CS_DemoOOP.ReferenceTypes;
using CS_DemoOOP.Arithmetic;
using CS_DemoOOP.ReferenceTypes.Banking;
using CS_DemoOOP.ReferenceTypes.Insurance;

namespace CS_DemoOOP
{
    class Program
    {
        static void Main(string[] args)
        {
            //Day today = Day.Wednesday;

            // Take input of the day from user interactively
            // Console.Write("Enter the day of the week (1 for Sunday, 2 for Monday, ..., 7 for Saturday): ");
            // string dayInput = Console.ReadLine();
            // Day today = Enum.Parse<Day>(dayInput);

            // Console.WriteLine($"Today is {today}, Numeric value: {(int)today}");

            // Console.WriteLine();
            // // Example of Looping through Enum Values
            // Console.WriteLine("Days of the week:");
            // foreach (Day day in Enum.GetValues<Day>())
            // {
            //     Console.WriteLine($"  {day} (Value: {(int)day})");
            // }

            // Day days = Day.Monday | Day.Wednesday | Day.Friday; // Using bitwise OR to combine multiple days
            //                                                     //Console.WriteLine($"Selected days: {days}"); // Output: Selected days: Monday, Wednesday, Friday
            // Console.WriteLine($"Is Monday selected? {(days.HasFlag(Day.Monday) ? "Yes" : "No")}");
            // Console.WriteLine($"Is Tuesday selected? {(days.HasFlag(Day.Tuesday) ? "Yes" : "No")}");
            // Console.WriteLine($"Is Wednesday selected? {(days.HasFlag(Day.Wednesday) ? "Yes" : "No")}");
            // Console.WriteLine($"Is Thursday selected? {(days.HasFlag(Day.Thursday) ? "Yes" : "No")}");
            // Console.WriteLine($"Is Friday selected? {(days.HasFlag(Day.Friday) ? "Yes" : "No")}");
            // Console.WriteLine($"Is Saturday selected? {(days.HasFlag(Day.Saturday) ? "Yes" : "No")}");

            // // Output:
            // // Is Monday selected? Yes
            // // Is Tuesday selected? No
            // // Is Wednesday selected? Yes
            // // Is Thursday selected? No
            // // Is Friday selected? Yes
            // // Is Saturday selected? No

            //Point point = new Point();
            //// point.x = 10;
            //// point.y = 20;
            //point.X = 10; // Using property setter
            //point.Y = 20; // Using property setter
            //Console.WriteLine(point); // Output: Point: (10, 20)

            //Point anotherPoint = new Point(30, 40); // Using parameterized constructor
            //Console.WriteLine(anotherPoint); // Output: Point: (30, 40) 

            //Person person = new Person() { Id = 1, Name = "John Doe", DateOfBirth = new DateTime(1990, 9, 10) };
            //Console.WriteLine(person.Display());

            //Calc calc = new Calc();
            //Console.WriteLine($"Addition of 5 and 10: {calc.Add(5, 10)}"); // This will call the method that takes two integers
            //Console.WriteLine($"Addition of 5, 10 and 15: {calc.Add(5, 10, 15)}"); // This will call the overloaded method that takes three integers
            //Console.WriteLine($"Addition of 'Hello' and 'World': {calc.Add("Hello", "World")}"); // This will call the overloaded method that takes two strings
            //Console.WriteLine($"Subtraction of 10 and 5: {calc.Subtract(10, 5)}"); // This will call the method that performs subtraction
            //Console.WriteLine($"Addition of 5 and 10: {Calc.Add(5, 10)}"); // This will call the method that takes two integers
            //Console.WriteLine($"Addition of 5, 10 and 15: {Calc.Add(5, 10, 15)}"); // This will call the overloaded method that takes three integers
            //Console.WriteLine($"Addition of 'Hello' and 'World': {Calc.Add("Hello", "World")}"); // This will call the overloaded method that takes two strings
            //Console.WriteLine($"Subtraction of 10 and 5: {Calc.Subtract(10, 5)}"); // This will call the method that performs subtraction

            //Person person1 = new Person(1, "Alice", new DateTime(1995, 5, 15));

            //Console.WriteLine("Person object created");
            //person1.Dispose(); // Explicitly calling Dispose to release resources
            //person1 = null; // Setting the reference to null to make the object eligible for garbage collection
            //GC.Collect(); // Forcing garbage collection to see the finalizer in action
            //GC.WaitForPendingFinalizers(); // Waiting for the finalizer to complete

            //Person employee = new Employee(1, "Bob", new DateTime(1985, 3, 20), 1001, 10, 50000m);
            //Console.WriteLine(employee.Display());

            AccountBase account = new SavingsAccount();
            try
            {
                Console.WriteLine($"Initial balance: {account.Balance}");
                // Deposit some money
                account.Deposit(10000m);
                Console.WriteLine($"Balance after deposit: {account.Balance}");
                // Calculate the insurance premium for the account
                if (account is ILifeInsurance insurance)
                {
                    decimal premium = insurance.CalculatePremium();
                    Console.WriteLine($"Calculated insurance premium: {premium}");
                    // Display the modified balance after deducting the premium from the account
                    Console.WriteLine($"Balance after insurance deduction: {account.Balance}");
                }
                // Withdraw some money
                account.Withdraw(5500m);
                Console.WriteLine($"Balance after withdrawal: {account.Balance}");
                // Attempt to withdraw more than the balance to trigger an exception
                account.Withdraw(5000m);
                Console.WriteLine($"Balance after withdrawal: {account.Balance}");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            catch (Exception ex) 
            { 
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                Console.WriteLine("Transaction completed.");
                Console.WriteLine("Performing cleanup if necessary...");
            }

            Console.WriteLine("\nProgram is completed. Press any key to exit...");
            Console.ReadKey();
        }
    }
}