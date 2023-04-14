// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.MixedReality.Toolkit.UI;
using TMPro;
using UnityEngine;

[AddComponentMenu("Scripts/MRTK/Examples/ShowValue")]
public class ShowPeopleLabel : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro textMesh = null;

    void Start() {
  
    }

    public void SetText(PeopleView foo)
    {
        if (textMesh == null)
        {
            textMesh = GetComponent<TextMeshPro>();
        }

        if (textMesh != null)
        {
            var str = "<size=300><b>Legenda Pessoas</b></size>\n\n" + foo.GetLabel();
            textMesh.text = str;
        }
    }
}
