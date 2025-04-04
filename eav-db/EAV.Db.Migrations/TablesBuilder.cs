using EAV.Db.Client.Model;
using FluentMigrator.Builders.Create;

namespace EAV.Db.Migrations;

public class TablesBuilder<T>
    where T : Entity
{
    private readonly EntityRegistry registry;

    public TablesBuilder(EntityRegistry registry)
    {
        this.registry = registry;
        registry.Register(typeof(T));
    }

    public virtual void Run(ICreateExpressionRoot create)
    {
        var type = typeof(T);

        var schemaName = "public";
        var tableName = registry.GetTableName(type);

        if (tableName.IndexOf(".") > 0)
        {
            schemaName = tableName.Split(".")[0];
            tableName = tableName.Split(".")[1];
        }

        var table = create.Table(tableName).InSchema(schemaName);

        table.WithColumn("id").AsInt64().PrimaryKey().Identity();
        table.WithColumn("created").AsDateTime2().NotNullable();
        table.WithColumn("updated").AsDateTime2().Nullable();
        table.WithColumn("deleted").AsDateTime2().Nullable();
        table.WithColumn("uid").AsString(300).NotNullable();

        create.Index().OnTable(tableName).InSchema(schemaName).OnColumn("uid");
    }
}
