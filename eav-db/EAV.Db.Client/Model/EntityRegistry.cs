using System.Reflection;

namespace EAV.Db.Client.Model;

public class EntityRegistry
{
    public static readonly EntityRegistry Instance = new EntityRegistry();

    private HashSet<Type> types = new();

    private Dictionary<Type, IList<(PropertyInfo, Type, string)>> values = new();

    private Dictionary<Type, string> tables = new();

    private Dictionary<(string, string), short> fields = new();

    public virtual void Register(Type type)
    {
        if (types.Contains(type))
            return;

        RegisterValues(type);

        var tableName = RegisterTable(type);

        RegisterFields(type, tableName);

        types.Add(type);
    }

    protected virtual void RegisterValues(Type type)
    {
        var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        var props = type.GetProperties(flags);

        foreach (var prop in props)
        {
            var valuesAttr = (EntityValuesAttribute)
                Attribute.GetCustomAttribute(prop, typeof(EntityValuesAttribute));

            if (valuesAttr == null)
                continue;

            if (!values.ContainsKey(type))
                values.Add(type, new List<(PropertyInfo, Type, string)>());

            var valuesType = prop.PropertyType.GetGenericArguments()[0];
            var valuesName = valuesAttr.Name;

            values[type].Add((prop, valuesType, valuesName));
        }

        //foreach (PropertyInfo prop in props)
        //{
        //    if (!prop.PropertyType.IsGenericType)
        //        continue;

        //    if (prop.PropertyType.GetGenericTypeDefinition() != typeof(EntityValues<>))
        //        continue;

        //    var valuesType = prop.PropertyType.GetGenericArguments()[0];

        //    Console.WriteLine($"Property: {prop.Name}, Generic Argument: {valuesType}");
        //}
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

    protected virtual void RegisterFields(Type type, string tableName)
    {
        var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        var props = type.GetProperties(flags);

        foreach (var prop in props)
        {
            var fieldIdAttr = (EntityFieldAttribute)
                Attribute.GetCustomAttribute(prop, typeof(EntityFieldAttribute));

            if (fieldIdAttr == null)
                continue;

            if (fields.ContainsKey((tableName, prop.Name)))
            {
                throw new ApplicationException($"The field {prop.Name} has conflicting Field Id.");
            }

            fields.Add((tableName, prop.Name), fieldIdAttr.Id);
        }
    }

    public virtual IEnumerable<(PropertyInfo Prop, Type Type, string Name)> GetValues(Type type)
    {
        return values[type];
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
