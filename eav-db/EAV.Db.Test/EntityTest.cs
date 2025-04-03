using EAV.Db.Client.Model;

namespace EAV.Db.Test;

[EntityTable("kuku")]
public class EntityTest : Entity
{
    [EntityField(101)]
    public string Text
    {
        get { return ValuesText[nameof(Text)]; }
        set { ValuesText[nameof(Text)] = value; }
    }
}
