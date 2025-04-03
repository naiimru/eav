namespace EAV.Db.Client.Model;

[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public class EntityFieldAttribute : Attribute
{
    public short Id { get; private set; }

    public EntityFieldAttribute(short id)
    {
        Id = id;
    }
}
