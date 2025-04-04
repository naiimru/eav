using EAV.Db.Client.Model;
using EAV.Db.Entities;
using FluentMigrator;

namespace EAV.Db.Migrations.Runner.Migrations;

[Migration(01)]
public class Migration01 : UpMigration
{
    public override void Up()
    {
        new TablesBuilder<Entity01>(EntityRegistry.Instance).Run(Create);
    }
}
