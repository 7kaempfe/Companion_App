using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleKeyShortcutVisibility : MonoBehaviour
{
    public GameObject keyShortcutText;
    void Start()
    {
        keyShortcutText.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown("h"))
        {
            if (keyShortcutText.activeInHierarchy)
            {
                keyShortcutText.SetActive(false);
            }
            else
            {
                keyShortcutText.SetActive(true);
            }
        }
    }
}
