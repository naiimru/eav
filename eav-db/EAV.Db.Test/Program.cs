using EAV.Db.Client;

namespace EAV.Db.Test
{
    internal class Program
    {
        static string ConnStr =
            "Host=localhost; Port=5432; Database=GrDb4; Username=postgres; Password=12; MaxPoolSize=50; Include Error Detail=true;";

        static void Main(string[] args)
        {
            var e = new EntityTest();
            e.Text = "bla bla";

            Console.WriteLine(e.Text);

            using var dbw = new DbWriter(new DbClient(ConnStr));
            dbw.Save(e);
        }
    }
}
