using EAV.Db.Client;
using EAV.Db.Entities;

namespace EAV.Db.Test
{
    internal class Program
    {
        static string ConnStr =
            "Host=localhost; Port=5432; Database=EAV; Username=postgres; Password=12; MaxPoolSize=50; Include Error Detail=true;";

        static void Main(string[] args)
        {
            var e = new Entity01();
            e.UId = "01";

            using var dbr = new DbReader(new DbClient(ConnStr));
            e = dbr.LoadById<Entity01>(3);
            e.UId = "01.1";

            using var dbw = new DbWriter(new DbClient(ConnStr));
            dbw.Save(e);
        }
    }
}
