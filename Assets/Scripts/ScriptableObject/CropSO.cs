using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "CropSO", menuName = "Scriptable Objects/CropSO")]
public class CropSO : ScriptableObject
{
    public string cropName;

    public Sprite cropSprite;
    
    public GameObject cropPrefab;
}
