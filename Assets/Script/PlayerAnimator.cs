using System;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
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
        _animator.SetBool(IsWalking,player.GetIsWalking());
    }
}
