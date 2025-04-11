using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkPiece : NetworkBehaviour
{
    public VisualPiece piece;   

    public NetworkVariable<Vector3> syncedPosition = new(
         writePerm: NetworkVariableWritePermission.Server
     );

    void Update()
    {
       
    }

    [ServerRpc(RequireOwnership = false)]
    public void MovePieceServerRpc(Vector3 newPosition)
    {
        syncedPosition.Value = newPosition;
        transform.position = newPosition;
    }

    

}

