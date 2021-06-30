
public enum ReplicationAction : byte {
		CREATE,
		UPDATE,
		DESTROY
	}
public class ReplicationHeader
{
	public ReplicationAction action;
	public uint objectId;
	public uint classId;

	public ReplicationHeader(){
	}
	public ReplicationHeader( ReplicationAction _action , uint _objectId , uint _classId )
	{
		action = _action;
		objectId = _objectId;
		classId = _classId;
	}

	public void Read( InputByteStream ibstream )
	{
		byte act;
		ibstream.Read( out act );
		action = (ReplicationAction)act;
		ibstream.Read( out objectId );

		if( action != ReplicationAction.DESTROY )
			ibstream.Read( out classId );
	}

	public void Write( OutputByteStream obstream )
	{
		obstream.Write( (byte)action );
		obstream.Write( objectId );

		if( action != ReplicationAction.DESTROY )
			obstream.Write( classId );
	}

	
}