using System.Collections.Generic;
using UnityEngine;
using LitMotion;
using LitMotion.Extensions;


public enum CropState
{
    Young,
    Mature,
    Dead
}


public class Crop : MonoBehaviour
{
    public float scaleMultiplier;
    
    public Vector3 maxScale;
    
    public float maxCooldown = 2f;
    public float matureCooldown = 3f;
    private float currentCooldown = 0f;

    [Header("Watering Settings")]
    public bool watered;
    [SerializeField] private float wateringMultiplier;
    
    public CropState currentState = CropState.Young;
    
    public Material deadMat;
    
    [Header("Growth Motion Settings")]
    public Ease easeType;
    public float smoothTime;

    public int scoreProvided;
    
    private MeshRenderer meshRenderer;
    
    private void Start()
    {
        currentCooldown = maxCooldown;
        meshRenderer = GetComponent<MeshRenderer>();
    }
    
    void Update()
    {
        if (currentCooldown <= 0)
        { 
            if (currentState == CropState.Dead)
            {
                Destroy(this.gameObject);
            }
            if (!ScaleMultiply(scaleMultiplier))
            {
                currentState = CropState.Dead;
                meshRenderer.material = deadMat;
            }
            currentCooldown = maxCooldown;
        }
        else
        {
            if (watered)
            {
                currentCooldown -= (Time.deltaTime * wateringMultiplier);
            }
            else
            {
                currentCooldown -= Time.deltaTime;
            }

        }
    }

    private bool ScaleMultiply(float multiplier)
    {
        if (transform.localScale != maxScale)
        {
            var newScale = transform.localScale * multiplier;

            if (newScale.x > maxScale.x)
            {
                newScale = new Vector3(maxScale.x, maxScale.y, maxScale.z);
                currentState = CropState.Mature;
                currentCooldown = matureCooldown;
            }
            LMotion.Create(transform.localScale, newScale, smoothTime)
                .WithEase(easeType)
                .BindToLocalScale(transform)
                .AddTo(gameObject);
            return true;
        }
        return false;
    }

    public int getCropScore()
    {
        return scoreProvided;
    }
}
