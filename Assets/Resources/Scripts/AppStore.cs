using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AppStore 
{

    
    private static float drawingHaloVanishingTime=0.2f;
    private static float pointerDelay=1f;

    public float DrawingHaloVanishingTime { get => drawingHaloVanishingTime; set => drawingHaloVanishingTime = value; }
    public float PointerDelay { get => pointerDelay; set => pointerDelay = value; }
}
