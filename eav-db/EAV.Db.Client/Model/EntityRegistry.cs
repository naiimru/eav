using System.Reflection;

namespace EAV.Db.Client.Model;

public class EntityRegistry
{
    public static readonly EntityRegistry Instance = new EntityRegistry();

    private Dictionary<Type, string> tables = new();

    private Dictionary<Type, string> values = new();

    private Dictionary<(string, string), short> fields = new();

    public virtual void Register(Type type)
    {
        var tableName = RegisterTable(type);

        RegisterValues(type, tableName);

        RegisterFields(type, tableName);
    }

    protected virtual string RegisterTable(Type type)
    {
        if (tables.ContainsKey(type))
            return tables[type];

        var attribute = (EntityTableAttribute)
            Attribute.GetCustomAttribute(type, typeof(EntityTableAttribute));

        if (attribute == null)
        {
            throw new InvalidOperationException(
                $"The class {type.Name} does not have an EntityTable attribute."
            );
        }

        var tableName = EntityTableAttribute.GetTableName(type);

        tables.Add(type, tableName);

        return tableName;
    }

    protected virtual void RegisterValues(Type type, string tableName)
    {
        var props = type.GetProperties();

        foreach (PropertyInfo prop in props)
        {
            if (!prop.PropertyType.IsGenericType)
                continue;

            if (prop.PropertyType.GetGenericTypeDefinition() != typeof(EntityValues<>))
                continue;

            var valuesType = prop.PropertyType.GetGenericArguments()[0];

            Console.WriteLine($"Property: {prop.Name}, Generic Argument: {valuesType}");
        }
    }

    protected virtual void RegisterFields(Type type, string tableName)
    {
        var props = type.GetProperties();

        foreach (var prop in props)
        {
            var fieldIdAttribute = (EntityFieldAttribute)
                Attribute.GetCustomAttribute(prop, typeof(EntityFieldAttribute));

            if (fieldIdAttribute == null)
                continue;

            if (fields.ContainsKey((tableName, prop.Name)))
            {
                throw new ApplicationException($"The field {prop.Name} has conflicting Field Id.");
            }

            fields.Add((tableName, prop.Name), fieldIdAttribute.Id);
        }
    }

    public virtual string GetTableName(Type type)
    {
        return tables[type];
    }

    public virtual short GetFieldId(Type type, string name)
    {
        var tableName = GetTableName(type);
        return fields[(tableName, name)];
    }
}
