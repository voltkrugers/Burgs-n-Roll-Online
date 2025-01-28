using System;
using UnityEngine;

public class BinCounter : BaseCounter
{
    public static event EventHandler OnAnyObjectTrashed; 
    new public static void ResetStaticData()
    {
        OnAnyObjectTrashed = null;
    }
    
    public override void Interact(CharacterController player)
    {
        if (player.HasKitchenObj())
        {
            player.GetKitchenObj().DestroySelf();
            
            OnAnyObjectTrashed?.Invoke(this,EventArgs.Empty);
        }
    }
}
