namespace NpWebApiApp
{
    public class Employee
    {
        public static int idGlobal = 0;
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }

        public Employee(string name, int age)
        {
            Id = ++idGlobal;
            Name = name;
            Age = age;
        }
    }
}
