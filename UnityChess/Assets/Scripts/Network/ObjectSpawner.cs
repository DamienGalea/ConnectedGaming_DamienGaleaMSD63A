using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [SerializeField] private GameObject networkImageApplyPrefab;

    // Start is called before the first frame update
    void Start()
    {

        NetworkManager.Singleton.OnServerStarted += SpawnObject;

        
    }

    public void SpawnObject()
    {

        if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost)
        {
            GameObject obj = Instantiate(networkImageApplyPrefab);
            obj.GetComponent<NetworkObject>().Spawn(true);

            Debug.Log("Spawned NetworkImageApply prefab.");
        }
        NetworkManager.Singleton.OnServerStarted -= SpawnObject;
    }

    
}
