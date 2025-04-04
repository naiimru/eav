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

    protected internal virtual EntityValues<T> CreateValues<T>()
    {
        var typeName = typeof(T).Name.ToLower();
        var tableName = $"{TableName}_values_{typeName}";
        return new EntityValues<T>(this, tableName);
    }

    private EntityValues<DateTime> valuesDateTime = null;

    protected internal bool ValuesDateTimeDirty => valuesDateTime != null;

    protected internal EntityValues<DateTime> ValuesDateTime
    {
        get
        {
            if (valuesDateTime == null)
                valuesDateTime = CreateValues<DateTime>();
            return valuesDateTime;
        }
    }

    private EntityValues<int> valuesInt = null;

    protected internal bool ValuesIntDirty => valuesInt != null;

    protected internal EntityValues<int> ValuesInt
    {
        get
        {
            if (valuesInt == null)
                valuesInt = CreateValues<int>();
            return valuesInt;
        }
    }

    private EntityValues<string> valuesString = null;

    protected internal bool ValuesStringDirty => valuesString != null;

    protected internal EntityValues<string> ValuesString
    {
        get
        {
            if (valuesString == null)
                valuesString = CreateValues<string>();
            return valuesString;
        }
    }

    private EntityValues<string> valuesText = null;

    protected internal bool ValuesTextDirty => valuesText != null;

    protected internal EntityValues<string> ValuesText
    {
        get
        {
            if (valuesText == null)
                valuesText = CreateValues<string>();
            return valuesText;
        }
    }
}
