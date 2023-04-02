using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchViews : MonoBehaviour
{
    public void ChangeView()
    {
        GameManager.Instance.SwitchView();
    }
}
