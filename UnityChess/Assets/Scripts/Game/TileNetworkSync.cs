using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class TileNetworkSync : NetworkBehaviour
{
    public NetworkVariable<FixedString64Bytes> tileName = new NetworkVariable<FixedString64Bytes>();

    public void SetTileName(string name)
    {
        if (IsServer)
        {
            tileName.Value = name;
        }
    }

    private void OnEnable()
    {
        tileName.OnValueChanged += HandleNameChanged;
    }

    private void OnDisable()
    {
        tileName.OnValueChanged -= HandleNameChanged;
    }

    private void HandleNameChanged(FixedString64Bytes previous, FixedString64Bytes current)
    {
        gameObject.name = current.ToString();
    }
}
