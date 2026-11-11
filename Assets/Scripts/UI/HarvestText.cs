using System;
using TMPro;
using UnityEngine;
using LitMotion;
using LitMotion.Extensions;

public class HarvestText : MonoBehaviour
{

    public TextMeshProUGUI harvestText;
    [SerializeField]private int harvestAmount;
    public TextMeshProUGUI maxHarvestText;
    [SerializeField]private int maxHarvestAmount;
    
    [Header("LitMotion Settings")]
    public float textAnimationDuration;

    private void Start()
    {
        UpdateHarvestText(0);
        UpdateMaxHarverstText(GameManager.Instance.getHarvestObjective());
    }

    private void OnEnable()
    {
        //sub to event here : 
        GameManager.OnHarvestChange += UpdateHarvestText;
    }

    private void OnDisable()
    {
        //unsub to event here : 
        GameManager.OnHarvestChange -= UpdateHarvestText;
    }

    public void UpdateHarvestText(int newAmount)
    {
        LMotion.Create(harvestAmount, newAmount, textAnimationDuration)
            .BindToText(harvestText, "{0}");
        harvestAmount = newAmount;
    }
    
    public void UpdateMaxHarverstText(int newMaxAmount)
    {
        maxHarvestText.text = "/" + newMaxAmount.ToString();
        maxHarvestAmount = newMaxAmount;
    }
    
}
