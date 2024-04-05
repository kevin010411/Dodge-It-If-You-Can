using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using DevUtils;

public class ScoreManager : MonoBehaviour
{

    private int PlayerMaximumHealth = 1000;

    public int MaximumStars;
    public int CurrentStarsCount;
    public int PlayerHealth;
    public int TotalHits;

    // Start is called before the first frame update
    void Start()
    {
        MaximumStars = 0;
        CurrentStarsCount = 0;
        TotalHits = 0;
        PlayerHealth = PlayerMaximumHealth;
    }

    // Update is called once per frame
    void Update()
    {

        if(PlayerHealth <= 0)
        {
            SendMessage(FunctionNames.TriggerPlayerDead);
        }

    }

    void ApplyDamageToPlayer(int DamageValue)
    {
        if(PlayerHealth - DamageValue < 0)
        {
            PlayerHealth = 0;
        }
        else
        {
            PlayerHealth -= DamageValue;
        }

        TotalHits++;
        Debug.Log("Applied ==> [" + DamageValue + "] damage to player.");
    }

    void ApplyMultiTimesDamageToPlayer(int DamageValue, int Times)
    {
        if(PlayerHealth - DamageValue * Times < 0)
        {
            PlayerHealth = 0;
        }
        else
        {
            PlayerHealth -= DamageValue * Times;
        }

        TotalHits++;
        Debug.Log("Applied ==> [" + (DamageValue * Times) + "] damage to player.");
    }

    void ApplyHealEffectToPlayer(int HealthRegainValue)
    {
        if(PlayerHealth + HealthRegainValue > PlayerMaximumHealth) 
        {
            PlayerHealth = PlayerMaximumHealth;
        }
        else
        {
            PlayerHealth += HealthRegainValue;
        }
    }

    void GetStar()
    {
        if(CurrentStarsCount < MaximumStars)
        {
            CurrentStarsCount++;
        }
    }
}
