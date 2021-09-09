using System.Collections.Generic;

public class GameObjectManager
{
    uint playerObjectId = 0;
    Dictionary<uint , GameObjectInfo> objectTable;
    Dictionary<GameObjectInfo , uint> idTable;
    public SortedDictionary<uint , uint> objectStateTable;

    public GameObjectManager()
    {
        objectTable = new Dictionary<uint, GameObjectInfo>();
        idTable = new Dictionary<GameObjectInfo, uint>();
        objectStateTable = new SortedDictionary<uint, uint>();
    }

    public uint GetObjectId( GameObjectInfo obj )
    {
        uint objectId = 0;

        if( obj != null && idTable.TryGetValue( obj , out objectId )  == false )
        {
            //AddGameObject( obj );
            //objectId = nextObjectId-1;
        }

        return objectId;
    }

    public GameObjectInfo GetGameObject( uint objectId )
    {
        GameObjectInfo result;

        objectTable.TryGetValue( objectId , out result );
        
        return result;
    }
    public List<uint> GetGameObjectIdAll()
    {
        GameObjectInfo result;

        Dictionary<uint,GameObjectInfo>.KeyCollection keys = objectTable.Keys;

        List<uint> results = new List<uint>();

        foreach( uint key in keys )
        {
            results.Add(key);
        }
        
        return results;
    }

    public GameObjectInfo GetPlayerObject()
    {
        GameObjectInfo result;

        objectTable.TryGetValue( playerObjectId , out result );
        
        return result;
    }

     public uint GetPlayerObjectId()
    {   
        return playerObjectId;
    }

    public void AddGameObject( GameObjectInfo obj , uint objectId )
    {
		if (obj != null && objectId != 0)
		{
			objectTable.Add(objectId, obj);
			idTable.Add(obj, objectId);
		}
    }

     public void RemoveGameObject( GameObjectInfo obj )
    {
        uint objectId = GetObjectId( obj );
        if( objectTable.ContainsKey( objectId ) == true )
        {
            objectTable.Remove( objectId );
        }
    }

    public void SetPlayerObject( uint pId )
    { playerObjectId = pId; }

    public void DestroyAllGameObject()
    {
        List<uint> objs = GetGameObjectIdAll();

        foreach( uint id in objs )
        {
            RemoveGameObject( objectTable[id] );
        }
    }
}
