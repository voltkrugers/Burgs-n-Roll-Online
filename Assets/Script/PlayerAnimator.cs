using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerAnimator : NetworkBehaviour
{
    private const string IsWalking = "IsWalking";
    [SerializeField] private CharacterController player;
    
    private Animator _animator;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!IsOwner)
        {
            return;
        }
        _animator.SetBool(IsWalking,player.GetIsWalking());
    }
}
