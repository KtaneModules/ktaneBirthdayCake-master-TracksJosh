using System;
using System.Collections;
using UnityEngine;
using System.Text.RegularExpressions;

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
    private bool isSolved = false;
    DateTime b = DateTime.Today;

    // Use this for initialization
    void Start() {
        cake.OnInteract += delegate { Cake(); return false; };
        bombModule.OnActivate += HappyBirthday;
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
            audio.PlaySoundAtTransform("win", transform);
            if (!isSolved)
            {
                isSolved = true;
                bombModule.HandlePass();
            }
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
        if (moduleId == 1)
        {
            audio.PlaySoundAtTransform("birf", transform);
        }
    }

    //twitch plays
    #pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} press <##> [Presses the cake when the timer's seconds digits are '##']";
    #pragma warning restore 414
    bool ZenModeActive;
    IEnumerator ProcessTwitchCommand(string command)
    {
        string[] parameters = command.Split(' ');
        if (Regex.IsMatch(parameters[0], @"^\s*press\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            if (parameters.Length > 2)
            {
                yield return "sendtochaterror Too many parameters!";
            }
            else if (parameters.Length == 2)
            {
                int time = -1;
                if (int.TryParse(parameters[1], out time))
                {
                    if (parameters[1].Length == 2 && time >= 0 && time <= 59)
                    {
                        yield return null;
                        if ((int)Bomb.GetTime() % 60 == time)
                            yield return "waiting music";
                        else if (ZenModeActive)
                        {
                            if ((time > (int)Bomb.GetTime() % 60 && (time - (int)Bomb.GetTime() % 60 > 15)) || (time < (int)Bomb.GetTime() % 60 && (60 - (int)Bomb.GetTime() % 60 + time > 15)))
                                yield return "waiting music";
                        }
                        else
                        {
                            if ((time > (int)Bomb.GetTime() % 60 && (60 - time + (int)Bomb.GetTime() % 60 > 15)) || (time < (int)Bomb.GetTime() % 60 && ((int)Bomb.GetTime() % 60 - time > 15)))
                                yield return "waiting music";
                        }
                        while ((int)Bomb.GetTime() % 60 == time) yield return "trycancel Halted waiting to press the cake due to a cancel request.";
                        while ((int)Bomb.GetTime() % 60 != time) yield return "trycancel Halted waiting to press the cake due to a cancel request.";
                        yield return "end waiting music";
                        cake.OnInteract();
                        yield break;
                    }
                }
                yield return "sendtochaterror!f The specified digits '" + parameters[1] + "' are invalid!";
            }
            else if (parameters.Length == 1)
            {
                yield return "sendtochaterror Please specify the digits to press the cake at!";
            }
        }
    }

    IEnumerator TwitchHandleForcedSolve()
    {
        while ((int)Bomb.GetTime() % 60 != candleTotal) yield return true;
        cake.OnInteract();
    }

}