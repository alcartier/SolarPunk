using System;
using UnityEngine;

public class SeedOnCollision : MonoBehaviour
{
    public float timeToLive = 5f;
    
    [Tooltip("The Crop that will be planted")]
    public GameObject cropPrefab;
    
    private void Start()
    {
        Destroy(gameObject, timeToLive);
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Dirt"))
        {
            //Planter la graine
            collision.gameObject.GetComponent<CropHolder>().PlantSeed(cropPrefab);
            Destroy(this.gameObject);
        }
        else if (collision.gameObject.CompareTag("Seed") || collision.gameObject.CompareTag("Player"))
        {
            return;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
