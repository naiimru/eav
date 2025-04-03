namespace EAV.Db.Test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var e = new EntityTest();
            e.Text = "bla bla";

            Console.WriteLine(e.Text);
        }
    }
}
