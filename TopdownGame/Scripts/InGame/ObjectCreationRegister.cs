using System.Collections.Generic;

public class ObjectCreationRegistry
{
    static ObjectCreationRegistry Instance;
    Dictionary<uint , System.Func<GameObjectInfo>> creationFuncTable;


    ObjectCreationRegistry()
    {
        creationFuncTable = new Dictionary<uint, System.Func<GameObjectInfo>>();

        RegisterCreationFunc<GameObjectInfo>( GameObjectInfo.GetClassId() );
        RegisterCreationFunc<PlayerObject>( PlayerObject.GetClassId() );
    }
    public static ObjectCreationRegistry GetInstance()
    { 
        if( Instance == null )
           Instance = new ObjectCreationRegistry();

        return Instance;
    }

    public GameObjectInfo CreateObject( uint classId )
    {
        GameObjectInfo result = null;

        if( creationFuncTable.ContainsKey( classId ) == true )
            result = creationFuncTable[classId].Invoke();

        return result;
    }

    public void RegisterCreationFunc<T>( uint classId ) where T : GameObjectInfo , new()
    {
        creationFuncTable.Add( classId , CreateInstance<T> );
    }

    static T CreateInstance<T>() where T : new()
    {
        return new T();
    }
}
