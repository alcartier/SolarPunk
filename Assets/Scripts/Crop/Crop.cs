using System.Collections.Generic;
using UnityEngine;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine.UI;


public enum CropState
{
    Young,
    Mature,
    Dead
}


public class Crop : MonoBehaviour
{
    public float scaleMultiplier;
    
    public float maxScaleFactor = 2f;
    
    private Vector3 maxScale;

    public AudioClip destroySound;
    
    [Header("Timings")]
    public float growthCooldown = 2f;
    public float matureCooldown = 3f;
    [SerializeField]private float currentCooldown = 0f;

    [Header("Watering Settings")]
    public bool watered;
    [SerializeField] private float wateringMultiplier;
    
    public CropState currentState = CropState.Young;
    
    public Material deadMat;
    public Material grownMat;
    
    [Header("Growth Motion Settings")]
    public Ease easeType;
    public Ease spawnEaseType;
    public float smoothTime;
    public float spawnTime;
    
    public int scoreProvided;
    public int scoreRemoved;
    
    private MeshRenderer meshRenderer;
    
    public GameObject destroyParticle;
    
    private void Start()
    {
        currentCooldown = growthCooldown;
        meshRenderer = GetComponent<MeshRenderer>();
        maxScale = transform.localScale * maxScaleFactor;
        Vector3 scale = transform.localScale;
        LMotion.Create(new Vector3(0,0,0), scale, 0.25f)
            .WithEase(spawnEaseType)
            .BindToLocalScale(transform)
            .AddTo(gameObject);
        
        
    }

    private void OnEnable()
    {
        GameManager.OnGameEnd2 += DestroyThis;
    }

    private void OnDisable()
    {
        GameManager.OnGameEnd2 -= DestroyThis;

    }

    private void DestroyThis()
    {
        Destroy(this.gameObject);
    }

    void Update()
    {
        if (currentCooldown <= 0)
        { 
            if (currentState == CropState.Dead)
            {
                GameManager.Instance.ChangeScore(-scoreRemoved);
                Instantiate(destroyParticle, transform.position, Quaternion.identity);
                AudioSource.PlayClipAtPoint(destroySound, this.transform.position, 1f);
                DestroyThis();
                return;
            }
            
            currentCooldown = growthCooldown;

            if (!ScaleMultiply(scaleMultiplier))
            {
                currentState = CropState.Dead;
                meshRenderer.material = deadMat;
            }

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
                watered = false;
                meshRenderer.material = grownMat;
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
