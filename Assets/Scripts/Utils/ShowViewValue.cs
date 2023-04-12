// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Microsoft.MixedReality.Toolkit.UI;
using TMPro;
using UnityEngine;

[AddComponentMenu("Scripts/MRTK/Examples/ShowSliderValue")]
public class ShowViewValue : MonoBehaviour
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
            var value = foobar * 4;

            switch (value)
            {
                case 0:
                    textMesh.text = "Gênero";
                    break;
                case 1:
                    textMesh.text = "Etnia";
                    break;
                case 2:
                    textMesh.text = "Admissão";
                    break;
                case 3:
                    textMesh.text = "Nacionalidade";
                    break;
                case 4:
                    textMesh.text = "Geral";
                    break;
            }
        }
    }
}
