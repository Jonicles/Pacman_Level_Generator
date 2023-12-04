using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorTracker : MonoBehaviour
{
    List<Color> colors = new List<Color>
    {
        new Color(0.1294118f, 0.1294118f, 1),
        new Color(0.1294118f,1, 0.3911492f),
        new Color(1,0.2079782f, 0.1294118f),
        new Color(1, 0.1294118f, 1),
        new Color(0.1294118f, 1, 1),
        new Color(0.5398438f, 0.1294118f, 1),
        new Color(1, 1, 0.1294118f),
        new Color(1, 0.6587149f, 0.1294118f),
        new Color(0.1294118f, 1, 1),
    };
    List<Color> availableColors = new List<Color>();
    Color previousColor;

    public Color GenerateNewColor()
    {
        if (availableColors.Count == 0)
            PopulateAvailableColors();

        Color newColor = previousColor;
        int index = 0; 
        
        while (newColor == previousColor)
        {
            index = UnityEngine.Random.Range(0, availableColors.Count - 1);
            newColor = availableColors[index];
        }

        availableColors.RemoveAt(index);

        previousColor = newColor;
        return newColor;
    }

    private void PopulateAvailableColors()
    {
        foreach (var color in colors)
            availableColors.Add(color);
    }
}
