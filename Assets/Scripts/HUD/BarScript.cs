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
    private Slider _slider;

    private Slider Sldr
    {
        get
        {
            if (this._slider == null)
                this._slider = GetComponent<Slider>();

            return this._slider;
        }
    }

    private TMP_Text _tmp_Text;

    private TMP_Text Tmp_Txt
    {
        get
        {
            if (this._tmp_Text == null)
                this._tmp_Text = GetComponentInChildren<TMP_Text>();

            return _tmp_Text;
        }
    }

    public void SetCurrent(int current)
    {
        this.Sldr.value = current;
        int percentile = (int)(current * 100 / this.Sldr.maxValue);
        if (percentile < 0)
        {
            percentile = 0;
        }
        this.Tmp_Txt.text = percentile + "%";
    }

    public void SetMax(int max)
    {
        if (max < this.Sldr.value)
            this.Sldr.value = max;
        this.Sldr.maxValue = max;
    }
}
