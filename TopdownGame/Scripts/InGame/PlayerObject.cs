using UnityEngine;


public class PlayerObject : GameObjectInfo
{
    static string fourChar = "POBJ";

    public Vector2 position;
    public Vector2 velocity;

    public PlayerObject()
    {
        classId = GetClassId();
        position = Vector2.zero;
        velocity = Vector2.zero;
    }

    override public string GetFourChar()
    { return fourChar; }

    static public new uint GetClassId()
    { 
        return GetClassIdWrapper( fourChar );
    }

    override public void Read( InputByteStream ibstream ) 
    {
        float x,y;

        ibstream.Read( out x );
        ibstream.Read( out y );
        position.x = x;
        position.y = y;

        ibstream.Read( out x );
        ibstream.Read( out y );
        velocity.x = x;
        velocity.y = y;
        
    }
    override public void Write( OutputByteStream obstream )
    {
        obstream.Write( position.x );
        obstream.Write( position.y );

        obstream.Write( velocity.x );
        obstream.Write( velocity.y );
    
    }
}
