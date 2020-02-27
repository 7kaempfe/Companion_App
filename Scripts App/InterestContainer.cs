using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;

// Dieses Script liegt auf dem InterestContainer Prefab. Die Werte werden im InterestScript zugewiesen.
public class InterestContainer : MonoBehaviour
{
    public Text designation, description;
    public Button openButton;
    public Transform parent;
    public int id;

    void Start()
    {

    }

    // Sendet die id des Containers an das InterestDescriptionScript, so dass man eine genaue Beschreibung des Inhalts einsehen kann.
    public void CloserDescription()
    {
        InterestDescriptionScript.ID = id;
    }


}
