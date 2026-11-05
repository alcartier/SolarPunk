using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DropSeed : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject seedPrefab;
    [SerializeField] private InputActionReference plantSeedAction;
    
    [SerializeField] private InputActionReference nextCropAction;
    [SerializeField] private InputActionReference previousCropAction;
    
    public List<CropSO> crops;
    
    public int currentCropId = 0;
    
    //event Pour UI
    public static event Action<Sprite> OnCropChange;

    private void Start()
    {
        OnCropChange?.Invoke(crops[currentCropId].cropSprite);
    }

    private void OnEnable()
    {
        OnCropChange?.Invoke(crops[currentCropId].cropSprite);
        
        plantSeedAction.action.performed += PlantSeed;

        nextCropAction.action.performed += GoToNextCrop;
        
        previousCropAction.action.performed += GoToPreviousCrop;
    }

    private void OnDisable()
    {
        OnCropChange?.Invoke(crops[currentCropId].cropSprite);
        
        plantSeedAction.action.performed -= PlantSeed;
        
        nextCropAction.action.performed -= GoToNextCrop;
        
        previousCropAction.action.performed -= GoToPreviousCrop;
    }

    private void PlantSeed(InputAction.CallbackContext context)
    {
        var curSeed = Instantiate(seedPrefab, spawnPoint.position, spawnPoint.rotation);
        curSeed.GetComponent<SeedOnCollision>().cropPrefab = crops[currentCropId].cropPrefab;
    }
    
    private void GoToNextCrop(InputAction.CallbackContext context)
    {
        if (currentCropId >= crops.Count - 1)
        {
            currentCropId = 0;
        }
        else
        {
            currentCropId++;
        }
        OnCropChange?.Invoke(crops[currentCropId].cropSprite);
    }
    
    private void GoToPreviousCrop(InputAction.CallbackContext context)
    {
        if (currentCropId <= 0)
        {
            currentCropId = crops.Count - 1;
        }
        else
        {
            currentCropId--;
        }
        OnCropChange?.Invoke(crops[currentCropId].cropSprite);
    }
}
