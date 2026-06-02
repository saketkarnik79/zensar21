namespace CS_DemoLINQ
{
    class Student
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public int Marks { get; set; }
    }

    internal class Program
    {
        private static List<Student> GetStudents()
        {
            return new List<Student>()
            {
                new Student { Id = 1, Name = "Alice", Age = 20, Marks = 85 },
                new Student { Id = 2, Name = "Bob", Age = 22, Marks = 90 },
                new Student { Id = 3, Name = "Charlie", Age = 19, Marks = 78 },
                new Student { Id = 4, Name = "David", Age = 21, Marks = 92 },
                new Student { Id = 5, Name = "Eve", Age = 20, Marks = 89 },
                new Student { Id = 6, Name = "Frank", Age = 23, Marks = 80 },
                new Student { Id = 7, Name = "Grace", Age = 19, Marks = 95 },
                new Student { Id = 8, Name = "Heidi", Age = 22, Marks = 82 },
                new Student { Id = 9, Name = "Ivan", Age = 21, Marks = 89 },
                new Student { Id = 10, Name = "Judy", Age = 20, Marks = 91 }
            };
        }

        // Filter students with marks above a certain threshold using LINQ Method Syntax
        private static IEnumerable<Student> StudentsWithMarksAboveMethodSyntax(int threshold)
        {
            var students = GetStudents();
            // Using LINQ Method syntax to filter students based on marks
            return students.Where(s => s.Marks > threshold);
        }

        // Filter students with marks above a certain threshold using LINQ Query Syntax
        private static IEnumerable<Student> StudentsWithMarksAboveQuerySyntax(int threshold)
        {
            var students = GetStudents();
            // Using LINQ Query syntax to filter students based on marks
            return from s in students
                   where s.Marks > threshold
                   select s;
        }

        // Filter students with marks above a certain threshold using LINQ Method Syntax with Projection
        private static IEnumerable<(string Name, int Marks)> StudentsWithMarksAboveMethodSyntaxProjection(int threshold)
        {
            var students = GetStudents();
            // Using LINQ Method syntax to filter and project students based on marks
            return students.Where(s => s.Marks > threshold)
                           .Select(s => (s.Name, s.Marks));
        }

        // Filter students with marks above a certain threshold using LINQ Query Syntax with Projection
        private static IEnumerable<(string Name, int Marks)> StudentsWithMarksAboveQuerySyntaxProjection(int threshold)
        {
            var students = GetStudents();
            // Using LINQ Query syntax to filter and project students based on marks
            return from s in students
                   where s.Marks > threshold
                   select (s.Name, s.Marks);
        }

        // Filter students with marks above a certain threshold using LINQ Method Syntax with Projection and ordered by marks in descending order
        private static IEnumerable<(string Name, int Marks)> StudentsWithMarksAboveMethodSyntaxProjectionOrdered(int threshold)
        {
            var students = GetStudents();
            // Using LINQ Method syntax to filter, project, and order students based on marks
            return students.Where(s => s.Marks > threshold)
                           .OrderByDescending(s => s.Marks)
                           .ThenBy(s => s.Name)
                           .Select(s => (s.Name, s.Marks));
        }

        // Filter students with marks above a certain threshold using LINQ Query Syntax with Projection and ordered by marks in descending order
        private static IEnumerable<(string Name, int Marks)> StudentsWithMarksAboveQuerySyntaxProjectionOrdered(int threshold)
        {
            var students = GetStudents();
            // Using LINQ Query syntax to filter, project, and order students based on marks
            return from s in students
                   where s.Marks > threshold
                   select (s.Name, s.Marks)
                   into student
                   orderby student.Marks descending, student.Name
                   select student;
        }

        // Get students grouped by age using LINQ Method Syntax
        private static IEnumerable<IGrouping<int, Student>> StudentsGroupedByAgeMethodSyntax()
        {
            var students = GetStudents();
            // Using LINQ Method syntax to group students by age
            return students.GroupBy(s => s.Age);
        }

        // Get students grouped by age using LINQ Query Syntax
        private static IEnumerable<IGrouping<int, Student>> StudentsGroupedByAgeQuerySyntax()
        {
            var students = GetStudents();
            // Using LINQ Query syntax to group students by age
            return from s in students
                   group s by s.Age into g
                   select g;
        }

        static void Main(string[] args)
        {
            //var students = StudentsWithMarksAboveMethodSyntax(85);
            //var students = StudentsWithMarksAboveQuerySyntax(85);
            //foreach (var student in students)
            //{
            //    Console.WriteLine($"Name: {student.Name}, Marks: {student.Marks}");
            //}

            //var students = StudentsWithMarksAboveMethodSyntaxProjection(85);
            //var students = StudentsWithMarksAboveQuerySyntaxProjection(85);
            //foreach (var student in students)
            //{
            //    Console.WriteLine($"Name: {student.Name}, Marks: {student.Marks}");
            //}
            //var students = StudentsWithMarksAboveMethodSyntaxProjectionOrdered(85);
            //var students = StudentsWithMarksAboveQuerySyntaxProjectionOrdered(85);
            //foreach (var student in students)
            //{
            //    Console.WriteLine($"Name: {student.Name}, Marks: {student.Marks}");
            //}

            //var groupedStudents = StudentsGroupedByAgeMethodSyntax();
            //var groupedStudents = StudentsGroupedByAgeQuerySyntax();
            //foreach (var group in groupedStudents)
            //{
            //    Console.WriteLine($"Age: {group.Key}, Count: {group.Count()}, Total Marks: {group.Sum(s => s.Marks)}, Average Marks: {group.Average(s => s.Marks)}, Minimum Marks: {group.Min(s => s.Marks)}, Maximum Marks: {group.Max(s => s.Marks)}");
            //    foreach (var student in group)
            //    {
            //        Console.WriteLine($"\tName: {student.Name}, Marks: {student.Marks}");
            //    }
            //}
            var students = GetStudents();
            //var first=students.First();
            //Console.WriteLine($"First student: {first.Name}, Marks: {first.Marks}");
            //var last = students.Last();
            //Console.WriteLine($"Last student: {last.Name}, Marks: {last.Marks}");
            //var single = students.Single(s=> s.Id == 5);
            //Console.WriteLine($"Single student with Id 5: {single.Name}, Marks: {single.Marks}");
            //var singleOrDefault = students.SingleOrDefault(s => s.Id == 11);
            //if (singleOrDefault != null)
            //{
            //    Console.WriteLine($"Single student with Id 11: {singleOrDefault.Name}, Marks: {singleOrDefault.Marks}");
            //}
            //else
            //{
            //    Console.WriteLine("No student found with Id 11.");
            //}
            //bool hasTopper = students.Any(s => s.Marks >= 95);
            //string result= hasTopper ? "Yes" : "No";
            //Console.WriteLine($"Has topper: {result}");
            bool allPassed = students.All(s => s.Marks >= 50);
            string result = allPassed ? "Yes" : "No";
            Console.WriteLine($"All students passed: {result}");

            Console.WriteLine("Program completed. Press any key to exit...");
            Console.ReadKey();
        }
    }
}
