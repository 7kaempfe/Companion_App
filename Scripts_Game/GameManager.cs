using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour
{
    public string SET_NAME;
    public int SET_SIZE = 16;
    //public bool easyGame;

    public GameObject[] cards_hard = new GameObject[16];
    public GameObject[] cards_medium = new GameObject[9];
    public GameObject[] cards_easy = new GameObject[6];
    public GameObject[] cards_feedback = new GameObject[3];
    private GameObject[] cards;
    private InputManager inputManager;
    public GameObject Componentholder;

    // alternative card array for easy game
    //public GameObject[] cardsEasyGame;
    public Material[] cardMaterials_animals;
    public Material[] cardMaterials_hamburg;
    public Material[] cardMaterials_personal;


    public List<int> thrownBalls;

    public scri clock;
    public GameObject victoryMessage;
    public ControllerGrabObject grabController;
    public GameObject glitter;
    public GameObject billboardCanvas;
    //public GameObject feedbackCanvas;
    private int numberOfCards;

    // used to start the timer only when player throws first ball
    public static bool startTimer;


    // the card that was previously hit (turned around)
    private GameCard activatedCard;
    private int finishedSets = 0;


    //private bool waitingForMoment;


    // waiting for two non-matching cards to be turned back
    private bool waitingForReset;
    // to prevent the same ball from hitting more than one tile
    private bool waitingForBallToFall;
    private float yCoordBillboard;
    private const float WINNING_YCOORD_BILLBOARD = 1.28f;

    // URL info text
    public GameObject url;
    // Random number for randomizing card sets
    private System.Random random;
    private bool cardsetHamburg;
    public Texture2D texture;
    public Texture2D test5;
    public Texture test4;
    private Boolean downloaded;


    private void Awake()
    {

        inputManager = Componentholder.GetComponent<InputManager>();
        inputManager.Feedbackfield.SetActive(false);
        inputManager.Feedback_billboard.SetActive(false);


        Debug.Log("Inactive works");

        random = new System.Random();

        victoryMessage.SetActive(false);
        waitingForReset = false;
        startTimer = false;
        waitingForBallToFall = false;
        thrownBalls = new List<int>();
    
        glitter.SetActive(false);
        yCoordBillboard = billboardCanvas.transform.position.y;

        cardsetHamburg = false;

        // shuffle materials
        if (this.name == "GameField_easy")
        {
            SET_SIZE = 6;
            Debug.Log("set size is 6");
            cards = cards_easy;
        }
        else if (this.name == "GameField_medium")
        {
            SET_SIZE = 8;
            Debug.Log("set size is 8");
            cards = cards_medium;
        }
        else
        {
            SET_SIZE = 16;
            Debug.Log("set size is 16");
            cards = cards_hard;
        }
    }


    public void initBoard()
    {
        //StartCoroutine(waitForAMoment());
        Debug.Log("initBoard geht weiter");
        Tutorial.newGame = true;
        numberOfCards = SET_SIZE;

        Material[] cardMaterials;

        StartCoroutine(GetImages(BildCallback));
        cardMaterials = cardMaterials_personal;
        /*
        int next = random.Next();
        Debug.Log("Next random: " + next.ToString());
        if (next % 2 == 0)
        {
            cardMaterials = cardMaterials_animals;
            Debug.Log("Animals selected");
            cardsetHamburg = false;

        }
        else
        {
            cardMaterials = cardMaterials_hamburg;
            Debug.Log("Hamburg selected");
            cardsetHamburg = true;
        }
        */
        // shuffle card materials
       /* for (int i = 0; i < cardMaterials.Length; i++)
        {
            Material tmp = cardMaterials[i];
            int r = UnityEngine.Random.Range(i, cardMaterials.Length);
            cardMaterials[i] = cardMaterials[r];
            cardMaterials[r] = tmp;
        }

        // shuffle cards

        Debug.Log("Number of cards: " + numberOfCards.ToString());
        Debug.Log("lengh of card array: " + cards.Length);
        for (int i = 0; i < numberOfCards; i++)
        {
            GameObject tmp = cards[i];
            int r = UnityEngine.Random.Range(i, numberOfCards);
            cards[i] = cards[r];
            cards[r] = tmp;
        }
        // apply materials to pairs
        int cardIndex = 0;
        for (int i = 0; i < numberOfCards / 2; i++)
        {
            cards[cardIndex].GetComponent<GameCard>().setCardMaterial(cardMaterials[i]);
            cards[cardIndex].GetComponent<GameCard>().pairNumber = i;
            cards[cardIndex + 1].GetComponent<GameCard>().setCardMaterial(cardMaterials[i]);
            cards[cardIndex + 1].GetComponent<GameCard>().pairNumber = i;
            cardIndex += 2;
        }*/
    }

    public void cardActivated(GameCard currentCard)
    {
        if (startTimer && Counter.wuerfe == 1)
        {
            // Debug.Log("clock -> game started");
            clock.gameStarted = true;
        }

        if (!waitingForReset && !waitingForBallToFall)
        {
            currentCard.turnToImage();
            StartCoroutine("waitForBallToFall");
            if (activatedCard != null)
            {
                if (activatedCard.pairNumber == currentCard.pairNumber)
                {
                    finishedSets++;
                    Debug.Log("Correct Pair!");
                    activatedCard.isFinished = true;
                    activatedCard.isFinished = true;
                    //currentCard.tiltForward();
                    //activatedCard.tiltForward();

                    if (finishedSets >= numberOfCards / 2)
                    {

                        
                        GameWon();
                        

                        //StartCoroutine(waitForAMoment());

                    }
                    activatedCard = null;
                }
                else
                {
                    Debug.Log("before waitforReset");
                    StartCoroutine("waitForReset");
                    activatedCard.turnToBack();
                    currentCard.turnToBack();
                    activatedCard = null;
                }
            }
            else
            {
                activatedCard = currentCard;
            }
        }
    }

    public void GameWon()
    {
        Debug.Log("Victory!");
        /*
        inputManager.Feedbackfield.SetActive(true);
        inputManager.Gamefield_easy.SetActive(false);
        inputManager.Gamefield_medium.SetActive(false);
        inputManager.Gamefield_hard.SetActive(false);
        inputManager.Feedback_billboard.SetActive(true);
        inputManager.Feedbackfield.SetActive(true);
        */
        inputManager.WaitForFeedbackField();

        Debug.Log("Victory! 2");

        clock.gameFinished = true;
        victoryMessage.SetActive(true);
        ControllerGrabObject.gewonnen = true;
       
        if (cardsetHamburg)
        {
            url.SetActive(true);
        }
        else
        {
            url.SetActive(false);
           
        }
        glitter.SetActive(true);
        
        billboardCanvas.transform.position = new Vector4(billboardCanvas.transform.position.x, WINNING_YCOORD_BILLBOARD, billboardCanvas.transform.position.z);
        Debug.Log("Victory!6");
        Debug.Log(inputManager.movementSpeed);
        
        Debug.Log("Victory!7");


        //StartCoroutine(waitForAMoment());


    }
    


    /*public IEnumerator waitForAMoment()
    {
        //waitingForMoment = true;
        yield return new WaitForSeconds(3);
        //inputManager.ShowFeedbackField();


        // waitingForMoment = false;
        //yield return null;


    }
    */

    IEnumerator waitForReset()
    {
        waitingForReset = true;
        yield return new WaitForSeconds(1.5f);

        waitingForReset = false;
        yield return null;
    }

    IEnumerator waitForBallToFall()
    {
        waitingForBallToFall = true;
        yield return new WaitForSeconds(0.5f);

        waitingForBallToFall = false;
        yield return null;
    }

    public void GetImage()
    {
        StartCoroutine(GetImages(BildCallback));
    }

    public IEnumerator GetImages(System.Action<Bild[]> callback)
    {
        WWWForm form = new WWWForm();

        using (UnityWebRequest www = UnityWebRequest.Get($"http://vitalab.informatik.uni-hamburg.de:3050/images?vorname={SongScript.vorname}&name={SongScript.nachname}&passwort={SongScript.passwort}"))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                Debug.Log("Ungültige Einlogdaten");
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                var image = JsonHelper.getJsonArray<Bild>(www.downloadHandler.text);
                callback(image);
            }
        }       
        
    }

    public void BildCallback(Bild[] bilder)
    {
        int zähler = 0;
        Debug.Log($"Found {bilder.Length} images!");
        foreach (var b in bilder)
        {
            if (zähler < 15)
            {
                //cardMaterials_personal[zähler] = new Material(cardMaterials_personal[0]);
                StartCoroutine(WaitForASecond(b.id, zähler, bilder.Length));
                Debug.Log(zähler);
                Debug.Log(b.id);
                zähler++;
            }
        }
        
        
    }

    public IEnumerator WaitForASecond(int id, int zähler, int länge)
    {
        Debug.Log("Started Coroutine at timestamp : " + Time.time);
        yield return new WaitForSeconds(zähler/5);
        Debug.Log("Finished Coroutine at timestamp : " + Time.time);
        StartCoroutine(DownloadImage(id, zähler, länge));

    }

    public IEnumerator DownloadImage(int imageid, int zähler, int länge)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture($"http://vitalab.informatik.uni-hamburg.de:3050/images?vorname={SongScript.vorname}&name={SongScript.nachname}&passwort={SongScript.passwort}&imageid={imageid}");
        yield return request.SendWebRequest();
        Texture tex = ((DownloadHandlerTexture)request.downloadHandler).texture as Texture;
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else
        {
            cardMaterials_personal[zähler].mainTexture = tex;
            /*cardMaterials_personal[zähler].EnableKeyword("_mainTexture");
            cardMaterials_personal[zähler].SetTexture("_mainTexture", tex);
            cardMaterials_personal[zähler].EnableKeyword("_mainTexture");*/
            Debug.Log(zähler + 1);
            Debug.Log(länge);
            if (zähler + 1 == länge)
            {
                Material[] cardMaterials;

                cardMaterials = cardMaterials_personal;


                for (int i = 0; i < länge; i++)
                {
                    Material tmp = cardMaterials[i];
                    int r = UnityEngine.Random.Range(i, länge);
                    if (cardMaterials[i].mainTexture == null)
                    {
                        r = länge - 1;
                        länge = länge - 1;
                        Debug.Log("AHA");
                    }
                    if (cardMaterials[r].mainTexture == null)
                    {
                        Debug.Log("Whyyyyyyy");
                    }
                    else
                    {
                        Debug.Log("OKAY");
                        cardMaterials[i] = cardMaterials[r];
                        cardMaterials[r] = tmp;
                    }

                }

                // shuffle cards

                Debug.Log("Number of cards: " + numberOfCards.ToString());
                Debug.Log("lengh of card array: " + cards.Length);
                for (int i = 0; i < numberOfCards; i++)
                {
                    GameObject tmp = cards[i];
                    int r = UnityEngine.Random.Range(i, numberOfCards);
                    cards[i] = cards[r];
                    cards[r] = tmp;
                }
                // apply materials to pairs
                int cardIndex = 0;
                for (int i = 0; i < numberOfCards / 2; i++)
                {
                    cards[cardIndex].GetComponent<GameCard>().setCardMaterial(cardMaterials[i]);
                    cards[cardIndex].GetComponent<GameCard>().pairNumber = i;
                    cards[cardIndex + 1].GetComponent<GameCard>().setCardMaterial(cardMaterials[i]);
                    cards[cardIndex + 1].GetComponent<GameCard>().pairNumber = i;
                    cardIndex += 2;
                }
            }
        }

    }

    public Texture2D byteArrayToImage(byte[] byteArrayIn)
    {
        Debug.Log(byteArrayIn.Length);
        texture.LoadImage(byteArrayIn);
        return texture;
    }

    [System.Serializable]
    public class Bild
    {
        public int id;
        public byte[] image;
        public int userid;
    }

    public class JsonHelper
    {
        public static T[] getJsonArray<T>(string json)
        {
            string newJson = "{ \"array\": " + json + "}";
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
            return wrapper.array;
        }

        [System.Serializable]
        private class Wrapper<T>
        {
            public T[] array;
        }
    }
}
