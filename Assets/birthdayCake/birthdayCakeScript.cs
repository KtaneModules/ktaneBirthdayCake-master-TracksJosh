using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KModkit;

public class birthdayCakeScript : MonoBehaviour {

    public GameObject[] candles;
    public Renderer[] candleRender;
    public KMSelectable cake;
    public Material[] candleColors;
    public KMBombInfo Bomb;
    public KMBombModule bombModule;
    public KMAudio audio;

    private int moduleId;
    private static int moduleIdCounter = 1;
    private int[] candleScore = { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
    private int candleTotal = 0;
    private bool playSound;
    private bool isSolved = false;
    DateTime b = DateTime.Today;




    // Use this for initialization
    void Start() {
        cake.OnInteract += delegate { Cake(); return false; };
        GetComponent<KMBombModule>().OnActivate += HappyBirthday;
        int years = 0;
        if ((b.Day >= 30 && b.Month == 7) || (b.Month >= 8))
        {
            years = b.Year - 2003;
        }
        else { years = b.Year - 2003 - 1; }
        for (int i = 0; i < candles.Length; i++)
        {
            candleScore[i] = 0;
            candles[i].SetActive(false);
        }
        moduleId = moduleIdCounter++;
        if(years > 33) { years=33; }
        if(years <= 0) { years=1; }
        if (years > 0)
        {
            for (int i = 0; i < years; i++)
            {
                candles[i].SetActive(true);
                int color = UnityEngine.Random.Range(0, 8);
                candleScore[i] = color;
                candleRender[i].material = candleColors[color];
                candleTotal += candleScore[i];
            }
            candleTotal = candleTotal % 60;
            Debug.LogFormat("[Birthday Cake #{0}] I am {1} years old!", moduleId, years);
            Debug.LogFormat("[Birthday Cake #{0}] Candle Total is {1}", moduleId, candleTotal);
        }
        
	}



    void Cake()
    {
        int eat = Mathf.FloorToInt(Bomb.GetTime() % 60);
        if (eat == candleTotal)
        {
            bombModule.HandlePass();
            audio.PlaySoundAtTransform("win", transform);
            isSolved = true;
        }
        else
        {
            bombModule.HandleStrike();
            audio.PlaySoundAtTransform("strik", transform);
            Debug.LogFormat("[Birthday Cake #{0}] Pressed {1}", moduleId, eat);
        }
    }

    void HappyBirthday()
    {
        if (!playSound)
        {
            playSound = true;
            audio.PlaySoundAtTransform("birf", transform);
        }
    }
}
