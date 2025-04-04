using System.Data;
using Dapper;
using EAV.Db.Client.Model;
using Npgsql;

namespace EAV.Db.Client;

public partial class DbWriter : IDisposable
{
    DbClient client;

    public DbWriter(DbClient client)
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

    public void Save(Entity entity)
    {
        using var db = client.Connect();

        if (entity.Id == 0)
        {
            InsertEntity(db, entity);
        }
        else
        {
            entity.Updated = DateTime.UtcNow;
            UpdateEntity(db, entity);
        }
    }

    private void InsertEntity(IDbConnection db, Entity entity)
    {
        var sql = $"""

            INSERT INTO {entity.TableName} (created, uid)
            VALUES (@Created, @Uid)
            RETURNING id;
            
            """;

        entity.Id = db.QuerySingle<long>(sql, entity);

        //SaveEntityValues(db, entity);
    }

    private void UpdateEntity(IDbConnection db, Entity entity)
    {
        var sql = $"""

            UPDATE {entity.TableName}
            SET updated = @Updated
            WHERE id = @Id;
            
            """;

        db.Execute(sql, entity);

        //SaveEntityValues(db, entity);
    }

    //protected virtual void SaveEntityValues(IDbConnection db, Entity entity)
    //{
    //    if (entity.ValuesDateTimeDirty)
    //        SaveEntityValues(db, entity.ValuesDateTime.TableName, entity.Id, entity.ValuesDateTime);

    //    if (entity.ValuesIntDirty)
    //        SaveEntityValues(db, entity.ValuesInt.TableName, entity.Id, entity.ValuesInt);

    //    if (entity.ValuesStringDirty)
    //        SaveEntityValues(db, entity.ValuesString.TableName, entity.Id, entity.ValuesString);

    //    if (entity.ValuesTextDirty)
    //        SaveEntityValues(db, entity.ValuesText.TableName, entity.Id, entity.ValuesText);
    //}

    public void SaveEntityValues<T>(
        IDbConnection db,
        string table,
        long id,
        IEnumerable<KeyValuePair<short, T>> values
    )
    {
        var selectQuery = $"SELECT aid FROM {table} WHERE id = @Id";
        var existingAids = db.Query<short>(selectQuery, new { Id = id }).ToHashSet();

        var insertsOrUpdates = values.Select(kvp => new
        {
            Id = id,
            Aid = kvp.Key,
            Data = kvp.Value,
        });

        var deletes = existingAids.Except(insertsOrUpdates.Select(item => item.Aid));

        if (insertsOrUpdates.Any())
        {
            var upsertQuery =
                $@"
            INSERT INTO {table} (id, aid, data)
            VALUES (@Id, @Aid, @Data)
            ON CONFLICT (id, aid) DO UPDATE
            SET data = EXCLUDED.data";

            db.Execute(upsertQuery, insertsOrUpdates);
        }

        if (deletes.Any())
        {
            var deleteQuery = $"DELETE FROM {table} WHERE id = @Id AND aid = ANY(@Aids)";

            db.Execute(deleteQuery, new { Id = id, Aids = deletes.ToArray() });
        }
    }
}
