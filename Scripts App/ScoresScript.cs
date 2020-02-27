using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Dieses Script lädt die Scores des Nutzers herunter und liest die Daten aus um die Daten des letzten Spiels, die besten Daten und die durchschnittlichen anzuzeigen.
public class ScoresScript : MonoBehaviour
{
    RestAPI restAPI;
    public Text scoreLastGame, scoreBestGame, scoreAverageGame, lengthLastGame, lengthBestGame, lengthAverageGame, dateLastGame, dateBestGame, hintField;

    // Start is called before the first frame update
    void Start()
    {
        restAPI = this.GetComponent<RestAPI>();
        StartCoroutine(restAPI.GetScores(RestAPI.FirstNameGlobal, RestAPI.SurnameGlobal, RestAPI.PasswordGlobal, ScoreCallback));
        hintField.text = "Hier können Sie den Fortschritt im Memoryspiel von " + RestAPI.FirstNameGlobal + " " + RestAPI.SurnameGlobal + " einsehen.";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //macht aus "1:24"-String --> 84-int (wird nur im Spiel benutzt)
    private int minToSecs(string s)
    {

        //U+033A
        string[] zahlen = s.Split('\u003A');
        int mins = Int32.Parse(zahlen[0]) * 60;
        int secs = Int32.Parse(zahlen[1]);
        Debug.Log(mins + secs);
        return mins + secs;
    }


    //macht aus 84-int "1:24"-String
    private String secsToMin(int secs)
    {
        string result = "";
        int m = (secs / 60);
        int s = (secs % 60);
        if (s > 9)
        {
            result = (m + ":" + s);
        }
        else
        {
            result = (m + ":" +  0 + s);
        }
        Debug.Log(result);
        return result;
    }

    // In diesem Script werden den Textfeldern die entsprechenden Daten zugewiesen, um diese lesbar zu machen.
    public void ScoreCallback(RestAPI.Score[] scores)
    {
        int ID = -1;
        int Würfe = Convert.ToInt32(scores[0].score);
        int gametime = Convert.ToInt32(scores[0].date);
        int AlleWürfe=0;
        int AlleSpieldauern=0;
        int DurchschnittWürfe = 0;
        int DurchschnittSpieldauern = 0;
        scoreBestGame.text = scores[0].score;
        lengthBestGame.text = secsToMin(Convert.ToInt32(scores[0].date)) + " min";

        Debug.Log($"Found {scores.Length} scores!");
        foreach (var s in scores)
        {
            if (s.id > ID)
            {
                ID = s.id;
                scoreLastGame.text = s.score;
                lengthLastGame.text = secsToMin(Convert.ToInt32(s.date)) + " min";
            }
            if (Convert.ToInt32(s.score) < Würfe)
            {
                Würfe = Convert.ToInt32(s.score);
                scoreBestGame.text = s.score;
            }
            if (Convert.ToInt32(s.date) < gametime)
            {
                gametime = Convert.ToInt32(s.date);
                lengthBestGame.text = secsToMin(Convert.ToInt32(s.date)) + " min";
            }
            AlleWürfe += Convert.ToInt32(s.score);
            AlleSpieldauern += Convert.ToInt32(s.date);

            Debug.Log(s.date);
            Debug.Log(s.score);
        }
        if (scores.Length != 0)
        {
            DurchschnittWürfe = AlleWürfe / scores.Length;
            DurchschnittSpieldauern = AlleSpieldauern / scores.Length;
        }
        scoreAverageGame.text = DurchschnittWürfe.ToString();
        lengthAverageGame.text = secsToMin(DurchschnittSpieldauern) + " min";
    }
}
