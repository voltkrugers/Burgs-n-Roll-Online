using System;
using UnityEngine;

public class ContainerCounterVisual : MonoBehaviour
{
    private Animator _animator;
    private const string OpenClosed = "OpenClose";
    [SerializeField] private ContainerCounter _containerCounter;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _containerCounter.OnPlayerGrabbedObject += OnPlayerGrabbedObject;
    }

    private void OnPlayerGrabbedObject(object sender, System.EventArgs e)
    {
        _animator.SetTrigger(OpenClosed);
    }
}
