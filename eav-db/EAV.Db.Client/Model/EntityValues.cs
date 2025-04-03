using System.Collections;

namespace EAV.Db.Client.Model;

public class EntityValues<T> : IEnumerable<KeyValuePair<short, T>>
{
    private Entity entity;
    private Dictionary<short, T> values = new();
    private string tableName;

    public EntityValues(Entity entity)
    {
        this.entity = entity;
    }

    public EntityValues(Entity entity, string tableName)
        : this(entity)
    {
        this.tableName = tableName;
    }

    public IEnumerator<KeyValuePair<short, T>> GetEnumerator()
    {
        return values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    internal string TableName => tableName;

    public T this[string name]
    {
        get
        {
            var key = entity.GetFieldId(name);
            return Get(key);
        }
        set
        {
            var key = entity.GetFieldId(name);
            Set(key, value);
        }
    }

    public T this[short key]
    {
        get { return Get(key); }
        set { Set(key, value); }
    }

    public T Get(short key)
    {
        if (values.TryGetValue(key, out T value))
            return value;

        return default(T);
    }

    public void Set(short key, T value)
    {
        if (!values.TryAdd(key, value))
            values[key] = value;
    }
}
