using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class FeedbackCard : MonoBehaviour
{
    public GameObject front;
    public GameObject gameManager;
    private Quaternion initialRotation;
    public bool isFinished = false;
    public bool isActivated = false;
    public int pairNumber;
    public const float ROTATION_TIME = 0.4f;
    public static bool bewertet;
    SongScript songScript;
    public GameObject Componentholder;
    public int bewertung;
    public Text Würfe;
    public Text Spieldauer;
    public GameObject dankeText;
    public GameObject frageText;


    // Start is called before the first frame update
    void Start()
    {
        songScript = Componentholder.GetComponent<SongScript>();
        initialRotation = transform.rotation;
        bewertet = false;
        Debug.Log(bewertet);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void turnToImage()
    {
        // Debug.Log("Card turned to image");
        StartCoroutine(RotateToImage());
    }


    public void OnTriggerEnter(Collider other)
    {
        if (!(this.isFinished || this.isActivated) && /*!(gameManager.GetComponent<GameManager>().thrownBalls.Contains(other.GetInstanceID())) &&*/ !bewertet)
        {
            Debug.Log(bewertet);
            Debug.Log(gameObject.name + " field collision!");
            turnToImage();
            SongBewerten();
            frageText.SetActive(false);
            dankeText.SetActive(true);
            ScoresHochladen(Spieldauer.text, Würfe.text);
            

            Debug.Log(bewertet);
        }
    }

    public IEnumerator UpdateSong()
    {
        WWWForm form = new WWWForm();
        form.AddField("vorname", SongScript.vorname);
        form.AddField("name", SongScript.nachname);
        form.AddField("passwort", SongScript.passwort);
        form.AddField("songid", songScript.songID);
        form.AddField("rating", bewertung);

        using (UnityWebRequest www = UnityWebRequest.Post("http://vitalab.informatik.uni-hamburg.de:3050/songs/update", form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Song update complete!");
            }
        }
    }

    public void SongBewerten()
    {
        StartCoroutine(UpdateSong());
    }

    public void ScoresHochladen(string Spieldauer, string Würfe)
    {
        StartCoroutine(PostScores(Spieldauer, Würfe));
    }




    IEnumerator RotateToImage()
    {
        bewertet = true;
        isActivated = true;
        Quaternion oldRotation = transform.rotation;
        transform.Rotate(Vector3.up * 180);
        Quaternion newRotation = transform.rotation;

        for (var t = 0f; t < 1; t += Time.deltaTime / ROTATION_TIME)
        {
            transform.rotation = Quaternion.Slerp(oldRotation, newRotation, t);
            yield return null;
        }

        transform.rotation = newRotation;
        yield return null;
    }

    private int minToSecs(string s)
    {

        //U+033A
        string[] zahlen = s.Split('\u003A');
        int mins = Int32.Parse(zahlen[0]) * 60;
        int secs = Int32.Parse(zahlen[1]);
        Debug.Log(mins + secs);
        return mins + secs;
    }


    public IEnumerator PostScores(string date, string score)
    {
        WWWForm form = new WWWForm();
        form.AddField("vorname", SongScript.vorname);
        form.AddField("name", SongScript.nachname);
        form.AddField("passwort", SongScript.passwort);
        form.AddField("date", minToSecs(date));
        form.AddField("score", score);

        using (UnityWebRequest www = UnityWebRequest.Post("http://vitalab.informatik.uni-hamburg.de:3050/scores", form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
            }
        }
    }


}
