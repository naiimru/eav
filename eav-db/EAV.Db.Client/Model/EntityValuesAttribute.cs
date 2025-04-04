namespace EAV.Db.Client.Model;

[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public class EntityValuesAttribute : Attribute
{
    public string Name { get; private set; }

    public EntityValuesAttribute(string name)
    {
        Name = name;
    }
}
