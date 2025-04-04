using Dapper;
using EAV.Db.Client.Model;

namespace EAV.Db.Client;

public class DbReader : IDisposable
{
    DbClient client;

    public DbReader(DbClient client)
    {
        this.client = client;
    }

    public void Dispose()
    {
        if (client != null)
        {
            client.Dispose();
            client = null;
        }
    }

    public T LoadById<T>(long id)
        where T : Entity
    {
        client.Registry.Register(typeof(T));

        string tableName = client.Registry.GetTableName(typeof(T));

        using var db = client.Connect();

        string sql = $"SELECT * FROM {tableName} WHERE id = @Id";
        var entity = db.QueryFirstOrDefault<T>(sql, new { Id = id });

        return entity;
    }
}
