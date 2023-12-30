using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Clock : MonoBehaviour
{
    public TMP_Text dateText;
    void Update()
    {
        DateTime now = DateTime.Now;
        dateText.text = now.ToString("ddd, hh:mmtt").ToUpper();
    }
}
