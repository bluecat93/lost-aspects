using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // helps us work with UI elements
using TMPro;

/// <summary>
/// This script is used for all bars and help us change the values in a convenient way.
/// </summary>
/// <param name="slider"> The slider used in the bar </param>
public class BarScript : MonoBehaviour
{
    private Slider slider;
    private  TMP_Text text;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        text = GetComponentInChildren<TMP_Text>();
    }
    public void SetCurrent(int current)
    {
        slider.value = current;
        int percentile = (int)(current * 100 / slider.maxValue);
        if (percentile < 0)
        {
            percentile = 0;
        }
        text.text = percentile + "%";

    }

    public void SetMax(int max)
    {
        if (max < slider.value)
            slider.value = max;
        slider.maxValue = max;
    }
}
