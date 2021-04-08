using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct LineInfo {
    public string line;
    public Sprite portrait;

    public LineInfo( string _line , Sprite _portrait )
    {
        line = _line;
        portrait = _portrait;
    }

    public LineInfo( ref LineInfo src )
    {
        line = src.line;
        portrait = src.portrait;
    }
};

public class ObjectInfo : MonoBehaviour
{
    public int id;
    public bool isNPC;
    public LineInfo[] lineInfo;
    
    

	public ref LineInfo[] GetLineInfo()
    {
        return ref lineInfo;
    }
 
}
