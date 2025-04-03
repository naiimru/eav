using System.Data;
using Dapper;
using EAV.Db.Client.Model;
using Npgsql;

namespace EAV.Db.Client;

public class DbWriter : IDisposable
{
    DbClient client;

    public DbWriter(DbClient client)
    {
        this.client = client;
    }

    public void Dispose()
    {
        client.Dispose();
    }

    public void Save(Entity entity)
    {
        using var db = client.Connect(true);

        if (entity.Id == 0)
        {
            InsertEntity(entity, db);
        }
        else
        {
            UpdateEntity(entity, db);
        }

        client.Commit();
    }

    private void InsertEntity(Entity entity, IDbConnection db)
    {
        var sql = $"""

            INSERT INTO {entity.TableName} (created, uid)
            VALUES (@Created, @Uid)
            RETURNING id;
            
            """;

        entity.Id = db.QuerySingle<long>(sql, entity);

        SetEntityValues(entity.Id, entity.ValuesDateTime!, db, "values_dt");
        SetEntityValues(entity.Id, entity.ValuesInt!, db, "values_int");
        SetEntityValues(entity.Id, entity.ValuesString!, db, "values_string");
        SetEntityValues(entity.Id, entity.ValuesText!, db, "values_text");
    }

    private void UpdateEntity(Entity entity, IDbConnection db)
    {
        var sql =
            @"
            UPDATE grabs
            SET source = @Source,
                type = @Type,
                d = @D,
                t = @T,
                uid = @Uid
            WHERE id = @Id;";

        db.Execute(sql, entity);

        SetEntityValues(entity.Id, entity.ValuesDateTime!, db, "values_dt");
        SetEntityValues(entity.Id, entity.ValuesInt!, db, "values_int");
        SetEntityValues(entity.Id, entity.ValuesString!, db, "values_string");
        SetEntityValues(entity.Id, entity.ValuesText!, db, "values_text");
    }

    public void SetEntityValues<T>(
        long id,
        IEnumerable<KeyValuePair<short, T>> values,
        IDbConnection db,
        string table
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
