using System;
using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    private CharacterController Player;
    private float footstepTimer;
    private float footstepTimerMax = .1f;

    private void Awake()
    {
        Player = GetComponent<CharacterController>();
    }

    private void Update()
    {
        footstepTimer -= Time.deltaTime;
        if (footstepTimer<0f)
        {
            footstepTimer = footstepTimerMax;
            if (Player.GetIsWalking())
            {
                SoundManager.Instance.PlayFootstepsSound(Player.transform.position,1f);
            }
        }
    }
}
