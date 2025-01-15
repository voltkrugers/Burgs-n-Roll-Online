using UnityEngine;

[CreateAssetMenu(fileName = "AudioClipRefsSo", menuName = "Scriptable Objects/AudioClipRefsSo")]
public class AudioClipRefsSo : ScriptableObject
{
    public AudioClip[] chop;
    public AudioClip[] DeliveryFail;
    public AudioClip[] DeliverySuccess;
    public AudioClip[] Footstep;
    public AudioClip[] ObjectDrop;
    public AudioClip[] ObjectPickUp;
    public AudioClip StoveSizzle;
    public AudioClip[] Trash;
    public AudioClip[] Warning;
}
