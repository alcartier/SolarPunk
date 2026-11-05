using System;
using UnityEngine;
using UnityEngine.UI;
using LitMotion;
using LitMotion.Extensions;

public class CropSelectionUI : MonoBehaviour
{
    [Header("LitMotionParam")]
    public Ease easeType = Ease.OutBounce;
    public float smoothTime = 0.3f;
    
    public Image cropImage;
    
    private MotionHandle currentMotion;
    
    private void OnEnable()
    {
        DropSeed.OnCropChange += UpdateUI;
    }

    private void OnDisable()
    {
        DropSeed.OnCropChange -= UpdateUI;
    }

    private void UpdateUI(Sprite sprite)
    {
        if (currentMotion.IsActive()) currentMotion.Cancel();
        
        cropImage.sprite = sprite;
        
        currentMotion = LMotion.Create(transform.localScale * 1.2f, transform.localScale, smoothTime)
            .WithEase(easeType)
            .BindToLocalScale(cropImage.GetComponent<Transform>())
            .AddTo(gameObject);
    }
}
