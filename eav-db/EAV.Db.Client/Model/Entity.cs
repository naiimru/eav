namespace EAV.Db.Client.Model;

public class Entity
{
    private static Dictionary<Type, string> tables = new();
    private static Dictionary<(string, string), short> fields = new();

    protected static void Register(Type type)
    {
        var attribute = (EntityTableAttribute)
            Attribute.GetCustomAttribute(type, typeof(EntityTableAttribute));

        if (attribute == null)
        {
            throw new InvalidOperationException(
                $"The class {type.Name} does not have an EntityTable attribute."
            );
        }

        var tableName = EntityTableAttribute.GetTableName(type);

        if (tables.ContainsKey(type))
            return;

        tables.Add(type, tableName);

        var props = type.GetProperties();

        foreach (var prop in props)
        {
            var fieldIdAttribute = (EntityFieldAttribute)
                Attribute.GetCustomAttribute(prop, typeof(EntityFieldAttribute));

            if (fieldIdAttribute == null)
                continue;

            if (fields.ContainsKey((tableName, prop.Name)))
            {
                throw new InvalidOperationException(
                    $"The field {prop.Name} has conflicting Field Id."
                );
            }

            fields.Add((tableName, prop.Name), fieldIdAttribute.Id);
        }
    }

    protected static void Register<T>()
        where T : Entity
    {
        var type = typeof(T);
        Register(type);
    }

    protected static string GetTableName(Entity entity)
    {
        return tables[entity.GetType()];
    }

    protected static short GetFieldId(Entity entity, string name)
    {
        var tableName = GetTableName(entity);
        return fields[(tableName, name)];
    }

    protected Entity()
    {
        Register(this.GetType());
    }

    public long Id { get; set; }

    public DateTime Created { get; set; } = DateTime.UtcNow;

    public DateTime? Updated { get; set; } = null;

    public DateTime? Deleted { get; set; } = null;

    public string UId { get; set; }

    protected internal virtual string TableName
    {
        get { return GetTableName(this); }
    }

    protected internal virtual short GetFieldId(string name)
    {
        return GetFieldId(this, name);
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
