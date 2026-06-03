namespace CS_DemoExtensionMethods
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string str = "Hello World";
            Console.WriteLine(str);
            str = str.Capitalize();
            //str= StringExtension.Capitalize(str);
            Console.WriteLine(str);

            string? name = null;
            int? x = name?.Length;
            if(name!=null)
            {
                Console.WriteLine(name.Capitalize());
            }
            else
            {
                Console.WriteLine("Name is null.");
            }
            Console.WriteLine("Program complete. Press any key to exit...");
            Console.ReadKey();
        }
    }
}
