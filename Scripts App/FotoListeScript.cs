using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

/* 
 * Dieses Script ist zuständig für die Erstellung der FotoContainer und für das Hochladen von neuen Bildern in die Datenbank
*/
public class FotoListeScript : MonoBehaviour
{
    RestAPI restAPI;
    public Text textfield;
    public Transform parent;
    public GameObject containerPrefab;
    public GameObject arrow;
    public RawImage testRawImage;
    public Texture2D testTexture;


    // Start is called before the first frame update
    void Start()
    {
        restAPI = this.GetComponent<RestAPI>();
        textfield.text = "Hier sind die Motive, die im Memoryspiel von " + RestAPI.FirstNameGlobal + " " + RestAPI.SurnameGlobal + " auftauchen.";
        LoadPictures();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Lädt die pictures aus der Datenbank und erstellt diese in einer scrollview. Wird bei Start aufgerufen.
    public void LoadPictures()
    {
        //waitHint.SetActive(true);
        foreach (Transform child in parent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        StartCoroutine(restAPI.GetPictures(PictureCallback));
        //animation
    }


    // Wird in LoadPictures aufgerufen. Ruft die Funktion DownloadPicture für jedes Bild in der Datenbank unter dem angegebenen nutzer auf.
    public void PictureCallback(RestAPI.Picture[] pictures)
    {
        int yPos = 0;
        int xPos = 0;
        int xPos2 = 800;
        int zähler = 0;
        Debug.Log($"Found {pictures.Length} images!");
        foreach (var b in pictures)
        {
            if (zähler == 0)
            {
                zähler = 1;
                StartCoroutine(DownloadPicture(b.id, yPos, xPos));
                //InstantiateContainer(test1, yPos, xPos);
            }
            else
            {
                StartCoroutine(DownloadPicture(b.id, yPos, xPos2));
                //InstantiateContainer(restAPI.ByteArrayToImage(test), yPos, xPos2);
                yPos -= 800;
                zähler = 0;
            }

        }
    }


    // Lädt die Bilder aus der Datenbank runter und ruft die Funktion InstantiateContainer auf.
    public IEnumerator DownloadPicture(int pictureID, int yPos, int xPos)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture($"http://vitalab.informatik.uni-hamburg.de:3050/images?vorname={RestAPI.FirstNameGlobal}&name={RestAPI.SurnameGlobal}&passwort={RestAPI.PasswordGlobal}&imageid={pictureID}");
        yield return request.SendWebRequest();
        Texture2D tex = ((DownloadHandlerTexture)request.downloadHandler).texture;
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else
        {
            InstantiateContainer(tex, yPos, xPos);
        }
    }



    // Instanziiert die Prefabs der Bildcontainer mit der gegebenen Textur und an den gegebenen Koordinaten.
    public void InstantiateContainer(Texture2D texture, int yPos, int xPos)
    {
        GameObject prefabInstanz = Instantiate(containerPrefab, new Vector3(xPos, yPos, 0), Quaternion.identity);
        prefabInstanz.transform.SetParent(parent);
        var Picture = prefabInstanz.GetComponentInChildren<FotoContainerScript>();
        Picture.rawImage.texture = texture;
        arrow.SetActive(false);
        // erst in der Coroutine test.cover = yourRawImage;
        //SongContainerSearch klassenInstanz;
        //klassenInstanz    = prefabInstanz.GetComponent<SongContainerSearch>();


        //klassenInstanz.songname.text = "noodles";
    }

    // Ruft die Gallerie des Handys auf und sobald ein Bild ausgewählt wurde, wird dieses in die Datenbank hochgeladen.
    public void PickImage()
    {
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            Debug.Log("Image path: " + path);
            if (path != null)
            {
                // Create Texture from selected image
                Texture2D texture = NativeGallery.LoadImageAtPath(path, 512);
                if (!texture)
                {
                    Debug.Log("Couldn't load texture from " + path);
                    return;
                }
                else
                {
                    StartCoroutine(restAPI.PostPicture(restAPI.ImageToByteArray(DuplicateTexture(texture))));
                }

            }
        }, "Select an image", "image/*");

        Debug.Log("Permission result: " + permission);
    }

   /* public void test1()
    {
        Debug.Log("test1");
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            Debug.Log("Image path: " + path);
            if (path != null)
            {
                // Create Texture from selected image
                Texture2D texture = NativeGallery.LoadImageAtPath(path, 512);
                if (texture == null)
                {
                    Debug.Log("Couldn't load texture from " + path);
                    return;
                }
                else
                {
                    testRawImage.texture = texture;
                }

            }
        }, "Select an image", "image/*");

        Debug.Log("Permission result: " + permission);
    }

    public void test2()
    {
        StartCoroutine(restAPI.PostPicture(restAPI.ImageToByteArray(DuplicateTexture(testTexture))));
    }*/

    // Wird benutzt um die Bilder aus der Datenbank readable zu machen, damit diese in die Datenbank hochgeladen werden können.
    Texture2D DuplicateTexture(Texture2D source)
    {
        RenderTexture renderTex = RenderTexture.GetTemporary(
                    source.width,
                    source.height,
                    0,
                    RenderTextureFormat.Default,
                    RenderTextureReadWrite.Linear);

        Graphics.Blit(source, renderTex);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTex;
        Texture2D readableText = new Texture2D(source.width, source.height);
        readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        readableText.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTex);
        return readableText;
    }
}
