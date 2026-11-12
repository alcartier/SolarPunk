using System;
using UnityEngine;
using LitMotion;
using LitMotion.Extensions;
using TMPro;
using UnityEngine.EventSystems;

public class OnButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Ease easeType;
    public float smoothTime = 0.3f;
    public float hoverScale = 1.1f;
    private TextMeshProUGUI button;
    public AudioClip hoverSound;
    public AudioClip clickSound;
    
    
    private AudioSource audioSource;
    private MotionHandle currentMotion;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentMotion.IsActive()) currentMotion.Cancel();
        
        if (hoverSound != null) audioSource.PlayOneShot(hoverSound);
        
        currentMotion = LMotion.Create(transform.localScale, Vector3.one * hoverScale, smoothTime)
            .WithEase(easeType)
            .WithScheduler(MotionScheduler.UpdateIgnoreTimeScale)
            .BindToLocalScale(transform)
            .AddTo(gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (currentMotion.IsActive()) currentMotion.Cancel();
        
        currentMotion = LMotion.Create(transform.localScale, Vector3.one, smoothTime)
            .WithEase(easeType)
            .WithScheduler(MotionScheduler.UpdateIgnoreTimeScale)
            .BindToLocalScale(transform)
            .AddTo(gameObject);
    }

    public void PlayClickSound()
    {
        if (currentMotion.IsActive()) currentMotion.Cancel();
        if (clickSound != null) audioSource.PlayOneShot(clickSound);
    }
}
