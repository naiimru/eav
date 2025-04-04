namespace EAV.Db.Client.Model;

public class Entity
{
    private EntityRegistry registry;

    protected Entity()
    {
        registry = EntityRegistry.Instance;
        registry.Register(this.GetType());
    }

    public long Id { get; set; }

    public DateTime Created { get; set; } = DateTime.UtcNow;

    public DateTime? Updated { get; set; } = null;

    public DateTime? Deleted { get; set; } = null;

    public string UId { get; set; }

    public EntityRegistry Registry => registry;

    public virtual string TableName
    {
        get { return Registry.GetTableName(this.GetType()); }
    }

    public virtual short GetFieldId(string name)
    {
        return Registry.GetFieldId(this.GetType(), name);
    }

    protected virtual EntityValues<T> CreateValues<T>()
    {
        var typeName = typeof(T).Name.ToLower();
        var tableName = $"{TableName}_values_{typeName}";
        return new EntityValues<T>(this, tableName);
    }
}
