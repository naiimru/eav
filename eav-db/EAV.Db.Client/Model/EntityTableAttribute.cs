using System;

namespace EAV.Db.Client.Model;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class EntityTableAttribute : Attribute
{
    public static string GetTableName(Type type)
    {
        var attribute = (EntityTableAttribute)
            Attribute.GetCustomAttribute(type, typeof(EntityTableAttribute));

        if (attribute == null)
        {
            throw new InvalidOperationException(
                $"The class {type.Name} does not have an EntityTable attribute."
            );
        }

        return attribute.TableName;
    }

    public static string GetTableName<T>()
    {
        var type = typeof(T);
        return GetTableName(type);
    }

    public string TableName { get; private set; }

    public EntityTableAttribute(string tableName)
    {
        if (string.IsNullOrWhiteSpace(tableName))
        {
            throw new ArgumentException("Table name cannot be null or empty.", nameof(tableName));
        }

        TableName = tableName;
    }
}
