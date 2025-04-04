using System.Data;
using EAV.Db.Client.Model;
using Npgsql;

namespace EAV.Db.Client;

public class DbClient : IDisposable
{
    private EntityRegistry registry;
    private string connStr;

    public DbClient(string connStr)
    {
        registry = EntityRegistry.Instance;
        this.connStr = connStr;
    }

    public EntityRegistry Registry => registry;

    public string ConnStr => connStr;

    public void Dispose()
    {
        if (t != null)
        {
            t.Dispose();
            t = null;
        }

        if (db != null)
        {
            db.Close();
            db.Dispose();
            db = null;
        }
    }

    NpgsqlConnection db;
    NpgsqlTransaction t;

    public IDbConnection Connect(bool startTransaction = false)
    {
        db = new NpgsqlConnection(ConnStr);
        db.Open();

        if (startTransaction)
            db.BeginTransaction();

        return db;
    }

    public void Commit()
    {
        if (t != null)
        {
            t.Commit();
        }
    }
}
