using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



public class InputManager : MonoBehaviour

{
    public GameObject Gamefield_hard;
    public GameObject Gamefield_medium;
    public GameObject Gamefield_easy;
    public GameObject Feedbackfield;
    public GameObject Feedback_billboard;

    public GameObject cameraRig;
    public GameObject cameraEye;
    public GameObject cameraEye_EmptyObject;

    public GameObject rightController;
    public GameObject leftController;

    public Transform PerfectPosition;

    // for resetting the camera position
    public float movementSpeed;
    public float ySpeed;
    public float rotationSpeed;

    private GameManager gameManager_hard;
    private GameManager gameManager_medium;
    private GameManager gameManager_easy;
    //private GameManager gameManager_feedback; //??


    private string gameType;
    private GameObject gameField;
    private ControllerGrabObject rightControllerScript;
    private ControllerGrabObject leftControllerScript;


    private void Awake()
    {
        //Feedbackfield.SetActive(false);
        //Feedback_billboard.SetActive(false);

        gameManager_hard = Gamefield_hard.GetComponent<GameManager>();
        gameManager_medium = Gamefield_medium.GetComponent<GameManager>();
        gameManager_easy = Gamefield_easy.GetComponent<GameManager>();
        //gameManager_feedback = Feedbackfield.GetComponent<GameManager>(); //?

        rightControllerScript = rightController.GetComponent<ControllerGrabObject>();
        leftControllerScript = leftController.GetComponent<ControllerGrabObject>();
    }

    public void Start()
    {


        if (gameType == null || gameType == "easy")
        {
            gameType = "easy";
            InitializeGamefield();
            gameField = Gamefield_easy;
        }
        else if (gameType == "medium")
        {
            InitializeGamefield();
            gameField = Gamefield_medium;
        }
        else
        {
            InitializeGamefield();
            gameField = Gamefield_hard;
        }

    }

    void Update()
    {

        // move the camera rig up or down
        if (Input.GetKey(KeyCode.UpArrow))
        {
            cameraRig.transform.position += new Vector3(0, ySpeed, 0);
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            cameraRig.transform.position -=
                new Vector3(0, ySpeed, 0);
        }

        // rotate the camera rig
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            cameraRig.transform.RotateAround(cameraEye.transform.position, Vector3.up, -rotationSpeed);
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            cameraRig.transform.RotateAround(cameraEye.transform.position, Vector3.up, rotationSpeed);
        }


        if (Input.GetKeyDown(KeyCode.P))
        {
            var d = PerfectPosition.position - cameraEye.transform.position;
            d.y = 0;
            cameraRig.transform.position += d;
            var lookDirection = cameraEye.transform.forward;
            lookDirection.y = 0;
            var rot = Vector3.SignedAngle(lookDirection, PerfectPosition.forward, Vector3.up);
            cameraRig.transform.RotateAround(cameraEye.transform.position, Vector3.up, rot);
        }


        // load new game
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            Debug.Log("game type " + gameType);
            InitializeGamefield();
            /*
            if (gameType == "easy")
            {
                gameType = "easy";
                InitializeGamefield();
            }
            else if (gameType == "medium")
            {
                gameType = "medium";
                InitializeGamefield();
            }
            else
            {
                InitializeGamefield();
            }*/

        }


        // set difficulty to easy
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("l key pressed");
            gameType = "easy";
            InitializeGamefield();
        }

        // set difficulty to medium
        if (Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log("m key pressed");
            gameType = "medium";
            InitializeGamefield();
        }

        // set difficulty to hard
        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("s key pressed");
            gameType = "hard";
            InitializeGamefield();
        }

        // cheat for debugging
        if (Input.GetKeyDown(KeyCode.W))
        {
            GameObject[] cards = gameManager_hard.cards_hard;
            if (gameType == "easy")
            {
                cards = gameManager_easy.cards_easy;
                gameManager_easy.GameWon();
        
            }
            else if (gameType == "medium")
            {
                cards = gameManager_medium.cards_medium;
                gameManager_medium.GameWon();
            }
            else if (gameType == "hard")
            {
                cards = gameManager_hard.cards_hard;
                gameManager_hard.GameWon();
            }

            for (int i = 0; i < cards.Length; i++)
            {
                GameCard card = cards[i].GetComponent<GameCard>();
                card.turnToImage();
            }
        }

        // toggle ball grabbing and releasing

        if (Input.GetKeyDown(KeyCode.T))
        {
            leftControllerScript.controllerMode = "t";
            rightControllerScript.controllerMode = "t";

        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            leftControllerScript.controllerMode = "r";
            rightControllerScript.controllerMode = "r";
        }
    }

    private void RotateCards()
    {
        GameObject[] cards = gameManager_hard.cards_hard;
        if (gameType == "easy")
        {
            cards = gameManager_easy.cards_easy;
        }
        else if (gameType == "medium")
        {
            cards = gameManager_medium.cards_medium;
        }
        else if (gameType == "hard")
        {
            cards = gameManager_hard.cards_hard;
        }

        for (int i = 0; i < cards.Length; i++)
        {
            GameCard card = cards[i].GetComponent<GameCard>();
            card.turnToImage();
        }

    }

    public void InitializeGamefield()
    {
        Debug.Log("new game initialized");
        Feedback_billboard.SetActive(false);
        Feedbackfield.SetActive(false);
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        if (gameType == "easy")
        {
            Gamefield_easy.SetActive(true);
            Gamefield_medium.SetActive(false);
            Gamefield_hard.SetActive(false);
            gameManager_easy.initBoard();

        }
        else if (gameType == "medium")
        {
            Gamefield_easy.SetActive(false);
            Gamefield_medium.SetActive(true);
            Gamefield_hard.SetActive(false);
            gameManager_medium.initBoard();
        }
        else if (gameType == "hard")
        {
            Gamefield_easy.SetActive(false);
            Gamefield_medium.SetActive(false);
            Gamefield_hard.SetActive(true);
            gameManager_hard.initBoard();
        }
    }

    
    public void WaitForFeedbackField()
    {

        //yield return new WaitForSeconds(3.0f);
        /*Debug.Log("Active Works!");
        Feedbackfield.SetActive(true);
        Gamefield_easy.SetActive(false);
        Gamefield_medium.SetActive(false);
        Gamefield_hard.SetActive(false);
        Feedback_billboard.SetActive(true);
        Feedbackfield.SetActive(true);
        Debug.Log("Active2 Works!");*/

        StartCoroutine(ShowFeedbackField());
    }

    IEnumerator ShowFeedbackField()
    {
        yield return new WaitForSeconds(5.0f);
        Debug.Log("Active Works!");
        Feedbackfield.SetActive(true);
        Gamefield_easy.SetActive(false);
        Gamefield_medium.SetActive(false);
        Gamefield_hard.SetActive(false);
        Feedback_billboard.SetActive(true);
        Feedbackfield.SetActive(true);

        Debug.Log("Active2 Works!");
    }
    
    


    /*IEnumerator RotateToStart()
    {
        Quaternion oldRotation = cameraRig.transform.rotation;
        cameraRig.transform.Rotate(Vector3.up * 3);
        Quaternion newRotation = cameraRig.transform.rotation;

        for (var t = 0f; t < 1; t += Time.deltaTime / 0.5f)
        {
            cameraRig.transform.rotation = Quaternion.Slerp(oldRotation, newRotation, t);
            yield return null;
        }

        cameraRig.transform.rotation = newRotation;
        yield return null;
    }*/
}
