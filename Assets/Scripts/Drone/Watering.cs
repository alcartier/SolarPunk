using UnityEngine;
using UnityEngine.InputSystem;

public class Watering : MonoBehaviour
{
    private CropHolder cropHolder;
    
    [SerializeField] private InputActionReference waterCropAction;
    [SerializeField] private ParticleSystem waterParticles;
    
    public AudioClip wateringSound;
    
    private void OnEnable()
    {
        waterCropAction.action.performed += WaterCrop;
    }

    private void OnDisable()
    {
        waterCropAction.action.performed -= WaterCrop;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Dirt"))
        {
            cropHolder =  other.GetComponent<CropHolder>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Dirt") && other.GetComponent<CropHolder>() == cropHolder)
        {
            cropHolder = null;
        }
    }
    
    private void WaterCrop(InputAction.CallbackContext context)
    {
        waterParticles.Play();
        AudioSource.PlayClipAtPoint(wateringSound, this.transform.position, 1f);
        if (cropHolder != null && cropHolder.currentCrop != null)
        {
            cropHolder.currentCrop.watered = true;
        }
    }
}
