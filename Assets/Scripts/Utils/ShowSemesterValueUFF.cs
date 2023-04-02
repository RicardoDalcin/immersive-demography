// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Microsoft.MixedReality.Toolkit.UI;
using TMPro;
using UnityEngine;

[AddComponentMenu("Scripts/MRTK/Examples/ShowSliderValue")]
public class ShowSemesterValueUFF : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro textMesh = null;

    public void OnSliderUpdated(SliderEventData eventData)
    {
        if (textMesh == null)
        {
            textMesh = GetComponent<TextMeshPro>();
        }

        if (textMesh != null)
        {
            var foobar = float.Parse($"{eventData.NewValue:F2}");
            var value = foobar * 10;

            var isEven = (value % 2) == 0;

            var year = 2017 + (int)(value / 2);
            var semester = isEven ? 1 : 2;

            textMesh.text = year.ToString() + "/" + semester.ToString();
        }
    }
}
