using System.Data;
using Npgsql;

namespace EAV.Db.Client;

public class DbClient : IDisposable
{
    public DbClient(string connStr)
    {
        ConnStr = connStr;
    }

    public string ConnStr { get; private set; }

    public void Dispose()
    {
        if (t != null)
        {
            t.Dispose();
        }

        if (db != null)
        {
            db.Close();
            db.Dispose();
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
