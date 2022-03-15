using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // helps us work with UI elements

/// <summary>
/// This script is used for all bars and help us change the values in a convenient way.
/// </summary>
/// <param name="slider"> The slider used in the bar </param>
public class BarScript : MonoBehaviour
{
    public Slider slider;
    public void SetCurrent(int current)
    {
        slider.value = current;
    }

    public void SetMax(int max)
    {
        if (max < slider.value)
            slider.value = max;
        slider.maxValue = max;
    }
}
