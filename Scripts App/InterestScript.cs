using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// In diesem Script werden die Interessen des Nutzers heruntergeladen und als InterestContainer instanziiert.
public class InterestScript : MonoBehaviour
{
    public GameObject containerPrefab;
    public Transform parent;
    public GameObject scrollview;
    RestAPI restAPI;
    public Text textField;
    public GameObject parentGameObject;
    public GameObject arrow;

    // Start is called before the first frame update
    void Start()
    {
        restAPI = this.GetComponent<RestAPI>();
        ShowInterests();
        textField.text = "Dies sind Interessen, Hobbies und Lieblingsgesprächsthemen von " + RestAPI.FirstNameGlobal + " " + RestAPI.SurnameGlobal + ".";
    }

    // Update is called once per frame
    void Update()
    {
        scrollview.SetActive(true);
    }

    // Lädt die Interessen des Nutzers von der Datenbank herunter und ruft den InterestCallback auf.
    public void ShowInterests()
    {
        StartCoroutine(restAPI.GetInterests(InterestCallback));
    }

    // Instanziiert ein Prefab des InterestContainer mit den mitgegebenen Daten
    private void InstantiateContainer(string designation, string description, int yPos, int id)
    {
        GameObject prefabInstanz = Instantiate(containerPrefab, new Vector3(0, yPos, 0), Quaternion.identity);
        prefabInstanz.transform.SetParent(parent);
        var interesse = prefabInstanz.GetComponentInChildren<InterestContainer>();
        interesse.designation.text = designation;
        interesse.description.text = description;
        interesse.parent = parent;
        interesse.id = id;
        // erst in der Coroutine test.cover = yourRawImage;
        //SongContainerSearch klassenInstanz;
        //klassenInstanz    = prefabInstanz.GetComponent<SongContainerSearch>();
        
        //klassenInstanz.songname.text = "noodles";
    }

    // Verkürzt die Strings der designation und der description, falls diese zu lang sind und ruft anschließend die Funktion InstantiateContainer für diese auf.
    private void InterestCallback(RestAPI.Interest[] interests)
    {
        var padding = parentGameObject.GetComponent<VerticalLayoutGroup>().padding;
        int yPos = 0;
        Debug.Log($"Found {interests.Length} interests!");
        foreach (var i in interests)
        {
            Debug.Log(i.bezeichnung);
            string designation = i.bezeichnung;
            string description = i.beschreibung;

            if (i.bezeichnung.Length > 20)
            {
                designation = i.bezeichnung.Remove(20, i.bezeichnung.Length - 20) + "...";
            }
            if (i.beschreibung.Length > 25)
            {
                description = i.beschreibung.Remove(25, i.beschreibung.Length-25) + "...";
            }
            InstantiateContainer(designation, description, yPos, i.id);
            yPos -= 400;
        }
        if (parent.childCount > 5)
        {
            scrollview.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 230);
            padding.left = 15;
        }
        if (parent.childCount == 0)
        {
            arrow.SetActive(true);
        }
    }

}
