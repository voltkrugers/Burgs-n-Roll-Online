using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public struct PlayerData : IEquatable<PlayerData> , INetworkSerializable
{
    public ulong clientId;
    public int colorId;
    public FixedString64Bytes PlayerName;
    public FixedString64Bytes PlayerId;
    
    public bool Equals(PlayerData other)
    {
        return clientId == other.clientId &&
               colorId == other.colorId &&
               PlayerName == other.PlayerName &&
               PlayerId == other.PlayerId;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
       serializer.SerializeValue(ref clientId);
       serializer.SerializeValue(ref colorId);
       serializer.SerializeValue(ref PlayerName);
       serializer.SerializeValue(ref PlayerId);
    }
}
