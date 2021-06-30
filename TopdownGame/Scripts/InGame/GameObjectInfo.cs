

public class GameObjectInfo
{
    static string fourChar = "GOBJ";
    public uint classId = 0;

    public GameObjectInfo()
    {
        classId = GetClassId();
    }

    virtual public string GetFourChar()
    { return fourChar; }

    static public uint GetClassId()
    { 
        return GetClassIdWrapper( fourChar );
    }

    static protected uint GetClassIdWrapper( string fourChar )
    {
        char[] literal = fourChar.ToCharArray();
        int[] literal_arr = new int[4];
        literal_arr[0] = (int)literal[0];
        literal_arr[1] = (int)literal[1];
        literal_arr[2] = (int)literal[2];
        literal_arr[3] = (int)literal[3];

        int result = (literal_arr[0] << 24) +
                        (literal_arr[1] << 16) +
                        (literal_arr[2] << 8) +
                        literal_arr[3];

        return (uint)result;
    }

    virtual public void Read( InputByteStream ibstream )
    {
    }
    virtual public void Write( OutputByteStream obstream )
    {
    }
}
