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
    
    private MotionHandle currentMotion;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentMotion.IsActive()) currentMotion.Cancel();
        
        AudioSource.PlayClipAtPoint(hoverSound, Camera.main.transform.position, 1f);
        
        currentMotion = LMotion.Create(transform.localScale, Vector3.one * hoverScale, smoothTime)
            .WithEase(easeType)
            .BindToLocalScale(transform);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (currentMotion.IsActive()) currentMotion.Cancel();
        
        currentMotion = LMotion.Create(transform.localScale, Vector3.one, smoothTime)
            .WithEase(easeType)
            .BindToLocalScale(transform);
    }

    public void PlayClickSound()
    {
        if (currentMotion.IsActive()) currentMotion.Cancel();
        AudioSource.PlayClipAtPoint(clickSound, Camera.main.transform.position, 1f);
    }
}
