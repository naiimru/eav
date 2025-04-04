using EAV.Db.Client.Model;

namespace EAV.Db.Test;

[EntityTable("kuku")]
public class EntityTest : Entity
{
    private EntityValues<DateTime> valuesDateTime = null;

    protected internal bool ValuesDateTimeDirty => valuesDateTime != null;

    [EntityValues("dt")]
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

    [EntityField(101)]
    public string Text
    {
        get { return ValuesText[nameof(Text)]; }
        set { ValuesText[nameof(Text)] = value; }
    }
}
