using System;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundManager : MonoBehaviour
{
    private const string PLAYER_PREFS_SOUND_EFFECT_VOLUME = "SoundEffectsVolume";
    public static SoundManager Instance { get; private set; }
    
    [SerializeField] private AudioClipRefsSo audioClipRefsSo;

    private float volume = 1f;

    private void Awake()
    {
        Instance = this;

       volume = PlayerPrefs.GetFloat(PLAYER_PREFS_SOUND_EFFECT_VOLUME, 1f);
    }

    private void Start()
    {
        DeliveryManager.Instance.OnRecipeSuccess += DeliveryManager_OnRecipeSuccess;
        DeliveryManager.Instance.OnRecipeFailed += DeliveryManager_OnRecipeFailed;
        CuttingCounter.OnAnyCut += CuttingCounter_OnAnyCut;
        CharacterController.OnAnyPickedSomething += CharacterController_OnPickedSomething;
        BaseCounter.OnAnyObjectPlacedHere += BaseCounter_OnAnyObjectPlacedHere;
        BinCounter.OnAnyObjectTrashed += TrashCounter_OnAnyObjectTrashed;
    }

    private void TrashCounter_OnAnyObjectTrashed(object sender, System.EventArgs e)
    {
        BinCounter binCounter = sender as BinCounter;
        PlaySound(audioClipRefsSo.Trash, binCounter.transform.position);
    }
    
    private void BaseCounter_OnAnyObjectPlacedHere(object sender, System.EventArgs e)
    {
        BaseCounter baseCounter = sender as BaseCounter;
        PlaySound(audioClipRefsSo.ObjectDrop, baseCounter.transform.position);
    }

    private void CharacterController_OnPickedSomething(object sender, System.EventArgs e)
    {
        CharacterController player = sender as CharacterController;
        PlaySound(audioClipRefsSo.ObjectPickUp, player.transform.position);
    }
    

    private void CuttingCounter_OnAnyCut(object sender, System.EventArgs e)
    {
        CuttingCounter cuttingCounter = sender as CuttingCounter;
        PlaySound(audioClipRefsSo.chop, cuttingCounter.transform.position);
    }

    private void DeliveryManager_OnRecipeSuccess(object sender, System.EventArgs e)
    {
        DeliveryManager deliveryCounter = DeliveryManager.Instance;
        PlaySound(audioClipRefsSo.DeliverySuccess,deliveryCounter.transform.position);
    }
    
    private void DeliveryManager_OnRecipeFailed(object sender, System.EventArgs e)
    {
        DeliveryManager deliveryCounter = DeliveryManager.Instance;
        PlaySound(audioClipRefsSo.DeliveryFail,deliveryCounter.transform.position);
    }

    private void PlaySound(AudioClip[] audioClipArray, Vector3 position, float VolumeMultiplier = 1f)
    {
        PlaySound(audioClipArray[Random.Range(0,audioClipArray.Length)],position, VolumeMultiplier*volume);
    }

    private void PlaySound(AudioClip audioClip,Vector3 position, float VolumeMultiplier = 1f)
    {
        AudioSource.PlayClipAtPoint(audioClip,position,VolumeMultiplier*volume);
    }

    public void PlayFootstepsSound(Vector3 pos, float volume)
    {
        PlaySound(audioClipRefsSo.Footstep,pos,volume);
    }

    public void ChangeVolume()
    {
        volume += .1f;
        if (volume>1f)
        {
            volume = 0f;
        }
        
        PlayerPrefs.SetFloat(PLAYER_PREFS_SOUND_EFFECT_VOLUME,volume);
        PlayerPrefs.Save();
    }

    public float GetVolume()
    {
        return volume;
    }
}
