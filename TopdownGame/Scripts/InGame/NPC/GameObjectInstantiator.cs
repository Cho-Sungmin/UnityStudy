
using UnityEngine;

public class GameObjectInstantiator : MonoBehaviour
{
    static public GameObject InstantiateObject( GameObjectInfo obj , uint objectId )
    {
        string fourChar = obj.GetFourChar();

        GameObject newObj = (GameObject) Instantiate( Resources.Load( "Prefabs/" + fourChar ) );
        newObj.name = System.Convert.ToString( objectId );
        
        return newObj;
    }
}
