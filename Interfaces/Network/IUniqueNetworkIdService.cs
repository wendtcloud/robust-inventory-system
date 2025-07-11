using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public interface IUniqueNetworkIdService
{
    public NetworkVariable<FixedString128Bytes> UniqueNetworkId { get; }

    void InitializeUniqueNetworkId();

    bool IsSameNetworkId(FixedString128Bytes otherUniqueNetworkId)
    {
        return UniqueNetworkId.Value.Equals(otherUniqueNetworkId);
    }
}