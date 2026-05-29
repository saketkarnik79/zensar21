using System;
using System.Collections.Generic;
using System.Text;

namespace CS_DemoOOP.ReferenceTypes
{
    internal class Person: IDisposable
    {
        private bool isCleanedUp = false; // Flag to track if cleanup has been done

        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime DateOfBirth { get; set; }

        // Initializers
        public Person() // Default constructor
        {
            Id= 0;
            Name=string.Empty;
            DateOfBirth= DateTime.MinValue;
        }

        public Person(int id, string name, DateTime dateOfBirth) // Parameterized constructor
        {
            Id = id;
            Name = name;
            DateOfBirth = dateOfBirth;
        }

        public int Age
        {
            get
            {
                DateTime today = DateTime.Today;
                int age = today.Year - DateOfBirth.Year;
                if (DateOfBirth.Date > today.AddYears(-age)) age--;
                return age;
            }
        }

        public virtual string Display()
        {
            return $"Person: Id={Id}, Name={Name}, DateOfBirth={DateOfBirth.ToShortDateString()}, Age={Age}";
        }

        public void Dispose()
        {
            // Cleanup code if needed (e.g., releasing unmanaged resources)
            Console.WriteLine($"Person with Id={Id} is being disposed.");
            GC.SuppressFinalize( this ); // Prevent the finalizer from being called since we've already cleaned up
            isCleanedUp = true; // Set the flag to indicate cleanup is done
        }

        // Finalizer (destructor) - called when the object is being garbage collected
        ~Person()
        {
            // Cleanup code if needed (not commonly used in C#)
            if (!isCleanedUp)
            {
                Console.WriteLine($"Person with Id={Id} is being finalized.");
            }
        }
    }
}
