using System.Collections;
using System.Collections.Specialized;

namespace CS_DemoCollections
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //DemoArrayList();
            //DemoNonGenericStack();
            //DemoNonGenericQueue();
            //DemoHashTable();

            //DemoGenericList();
            //DemoGenericStack();
            //DemoGenericQueue();
            //DemoDictionary();
            //DemoHashSet();

            DemoNameValueCollection();

            Console.WriteLine("Program completed. Press any key to exit...");
            Console.ReadKey();
        }

        // Non-Generic Collection Demos
        public static void DemoArrayList()
        {
            ArrayList arrayList = new ArrayList();
            // Add Items of different types to the ArrayList
            arrayList.Add(10);
            arrayList.Add("Hello");
            arrayList.Add(3.14);

            // Insert an item at a specific index
            arrayList.Insert(1, "Inserted");

            //Display the contents of the ArrayList
            Console.WriteLine("ArrayList contents:");
            foreach (var item in arrayList)
            {
                Console.WriteLine(item);
            }

            // Accessing items by index
            Console.WriteLine($"Item at index 2: {arrayList[2]}");

            // Update an item
            arrayList[0] = 20;

            // Remove an item
            arrayList.Remove("Hello");
            arrayList.RemoveAt(1);

            // Check if an item exists
            Console.WriteLine($"Item exists: {arrayList.Contains(20)}");

            // Display Count of items
            Console.WriteLine($"Count of items: {arrayList.Count}");

            // Clear all items
            arrayList.Clear();

            // Display Count of items
            Console.WriteLine($"Count of items: {arrayList.Count}");
        }

        public static void DemoNonGenericStack()
        {
            Stack stack = new Stack();

            // Push items of different types onto the stack
            stack.Push(10);
            stack.Push("Hello");
            stack.Push(3.14);

            // Display the contents of the stack
            Console.WriteLine("Stack contents:");
            foreach (var item in stack)
            {
                Console.WriteLine(item);
            }

            // Pop an item from the stack
            var poppedItem = stack.Pop();
            Console.WriteLine($"Popped item: {poppedItem}");

            // Peek at the top item without removing it
            var topItem = stack.Peek();
            Console.WriteLine($"Top item: {topItem}");

            // Check if an item exists
            Console.WriteLine($"Item exists: {stack.Contains("Hello")}");

            // Display Count of items
            Console.WriteLine($"Count of items: {stack.Count}");

            // Clear all items
            stack.Clear();

            // Display Count of items
            Console.WriteLine($"Count of items: {stack.Count}");
        }

        public static void DemoNonGenericQueue()
        {
            Queue queue = new Queue();

            // Enqueue items of different types into the queue
            queue.Enqueue(10);
            queue.Enqueue("Hello");
            queue.Enqueue(3.14);

            // Display the contents of the queue
            Console.WriteLine("Queue contents:");
            foreach (var item in queue)
            {
                Console.WriteLine(item);
            }

            // Dequeue an item from the queue
            var dequeuedItem = queue.Dequeue();
            Console.WriteLine($"Dequeued item: {dequeuedItem}");

            // Peek at the front item without removing it
            var frontItem = queue.Peek();
            Console.WriteLine($"Front item: {frontItem}");

            // Check if an item exists
            Console.WriteLine($"Item exists: {queue.Contains("Hello")}");

            // Display Count of items
            Console.WriteLine($"Count of items: {queue.Count}");

            // Clear all items
            queue.Clear();

            // Display Count of items
            Console.WriteLine($"Count of items: {queue.Count}");
        }

        public static void DemoHashTable()
        {
            Hashtable hashtable = new Hashtable();
           
            // Add key-value pairs of different types to the Hashtable
            hashtable.Add("Name", "Alice");
            hashtable.Add("Age", 30);
            hashtable.Add("IsStudent", true);
            
            // Display the contents of the Hashtable
            Console.WriteLine("Hashtable contents:");
            foreach (DictionaryEntry entry in hashtable)
            {
                Console.WriteLine($"{entry.Key}: {entry.Value}");
            }

            // Accessing values by key
            Console.WriteLine($"Name: {hashtable["Name"]}");
            Console.WriteLine($"Age: {hashtable["Age"]}");
            
            // Update a value
            hashtable["Age"] = 31;
            
            // Remove a key-value pair
            hashtable.Remove("IsStudent");
            
            // Check if a key exists
            Console.WriteLine($"Key exists: {hashtable.ContainsKey("Name")}");
            
            // Display Count of items
            Console.WriteLine($"Count of items: {hashtable.Count}");
            
            // Clear all items
            hashtable.Clear();
            
            // Display Count of items
            Console.WriteLine($"Count of items: {hashtable.Count}");
        }

        // Generic Collection Demos
        public static void DemoGenericList()
        {
            // Implement a demo for List<T>
            List<int> list = new List<int>();
            // Add Items of different types to the ArrayList
            list.Add(10);
            //list.Add("Hello"); // This will cause a compile-time error because List<int> only accepts integers
            //list.Add(3.14); // This will also cause a compile-time error for the same reason
            list.Add(20);

            // Insert an item at a specific index
            //list.Insert(1, "Inserted"); // This will cause a compile-time error because "Inserted" is not an integer
            list.Insert(1, 15);

            //Display the contents of the ArrayList
            Console.WriteLine("ArrayList contents:");
            foreach (var item in list)
            {
                Console.WriteLine(item);
            }

            // Accessing items by index
            Console.WriteLine($"Item at index 2: {list[2]}");

            // Update an item
            list[0] = 20;

            // Remove an item
            list.Remove(20);
            list.RemoveAt(1);

            // Check if an item exists
            Console.WriteLine($"Item exists: {list.Contains(20)}");

            // Display Count of items
            Console.WriteLine($"Count of items: {list.Count}");

            // Clear all items
            list.Clear();

            // Display Count of items
            Console.WriteLine($"Count of items: {list.Count}");
        }

        public static void DemoGenericStack()
        {
            Stack<int> stack = new Stack<int>();

            // Push items of different types onto the stack
            stack.Push(10);
            stack.Push(20);
            stack.Push(30);

            // Display the contents of the stack
            Console.WriteLine("Stack contents:");
            foreach (var item in stack)
            {
                Console.WriteLine(item);
            }

            // Pop an item from the stack
            var poppedItem = stack.Pop();
            Console.WriteLine($"Popped item: {poppedItem}");

            // Peek at the top item without removing it
            var topItem = stack.Peek();
            Console.WriteLine($"Top item: {topItem}");

            // Check if an item exists
            Console.WriteLine($"Item exists: {stack.Contains(20)}");

            // Display Count of items
            Console.WriteLine($"Count of items: {stack.Count}");

            // Clear all items
            stack.Clear();

            // Display Count of items
            Console.WriteLine($"Count of items: {stack.Count}");
        }

        public static void DemoGenericQueue()
        {
            Queue<int> queue = new Queue<int>();

            // Enqueue items of different types into the queue
            queue.Enqueue(10);
            queue.Enqueue(20);
            queue.Enqueue(30);

            // Display the contents of the queue
            Console.WriteLine("Queue contents:");
            foreach (var item in queue)
            {
                Console.WriteLine(item);
            }

            // Dequeue an item from the queue
            var dequeuedItem = queue.Dequeue();
            Console.WriteLine($"Dequeued item: {dequeuedItem}");

            // Peek at the front item without removing it
            var frontItem = queue.Peek();
            Console.WriteLine($"Front item: {frontItem}");

            // Check if an item exists
            Console.WriteLine($"Item exists: {queue.Contains(20)}");

            // Display Count of items
            Console.WriteLine($"Count of items: {queue.Count}");

            // Clear all items
            queue.Clear();

            // Display Count of items
            Console.WriteLine($"Count of items: {queue.Count}");
        }

        public static void DemoDictionary()
        {
            Dictionary<string, int> dictionary = new Dictionary<string, int>();

            // Add key-value pairs of different types to the Dictionary
            dictionary.Add("Name", 1);
            dictionary.Add("Age", 30);
            dictionary.Add("IsStudent", 1);

            // Display the contents of the Dictionary
            Console.WriteLine("Dictionary contents:");
            foreach (var kvp in dictionary)
            {
                Console.WriteLine($"{kvp.Key}: {kvp.Value}");
            }

            // Accessing values by key
            Console.WriteLine($"Name: {dictionary["Name"]}");
            Console.WriteLine($"Age: {dictionary["Age"]}");

            // Update a value
            dictionary["Age"] = 31;

            // Remove a key-value pair
            dictionary.Remove("IsStudent");

            // Check if a key exists
            Console.WriteLine($"Key exists: {dictionary.ContainsKey("Name")}");

            // Display Count of items
            Console.WriteLine($"Count of items: {dictionary.Count}");

            // Clear all items
            dictionary.Clear();

            // Display Count of items
            Console.WriteLine($"Count of items: {dictionary.Count}");
        }

        public static void DemoHashSet()
        {
            HashSet<int> hashSet = new HashSet<int>();

            // Add items of different types to the HashSet
            hashSet.Add(10);
            hashSet.Add(20);
            hashSet.Add(30);
            hashSet.Add(30); // This will not be added because HashSet does not allow duplicate items

            // Display the contents of the HashSet
            Console.WriteLine("HashSet contents:");
            foreach (var item in hashSet)
            {
                Console.WriteLine(item);
            }

            // Check if an item exists
            Console.WriteLine($"Item exists: {hashSet.Contains(20)}");

            // Display Count of items
            Console.WriteLine($"Count of items: {hashSet.Count}");

            // Clear all items
            hashSet.Clear();

            // Display Count of items
            Console.WriteLine($"Count of items: {hashSet.Count}");
        }

        // Specialized Collections Demos
        public static void DemoNameValueCollection()
        {
            // Implement a demo for NameValueCollection
            NameValueCollection collection = new NameValueCollection();
            
            // Add key-value pairs to the NameValueCollection
            collection.Add("Name", "John");
            collection.Add("Age", "30");
            collection.Add("City", "New York");
            collection.Add("City", "New York");

            // Display the contents of the NameValueCollection
            Console.WriteLine("NameValueCollection contents:");
            foreach (string key in collection.Keys)
            {
                Console.WriteLine($"{key}: {collection[key]}");
            }
        }
    }
}
