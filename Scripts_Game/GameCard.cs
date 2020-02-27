using System;
using System.Collections;
using UnityEngine;

public class GameCard : MonoBehaviour
{
    public GameObject gameManager;
    public bool isFinished = false;
    public bool isActivated = false;
    public int pairNumber;
    public GameObject front;

    public const float ROTATION_TIME = 0.4f;

    private Quaternion initialRotation;

    // Start is called before the first frame update
    void Start()
    {
        // set initial rotation
        initialRotation = transform.rotation;
    }

    public void turnToImage()
    {
        // Debug.Log("Card turned to image");
        StartCoroutine(RotateToImage());
    }

    public void turnToBack()
    {
        // Debug.Log("Card turned to back");
        StartCoroutine(RotateToBack());
    }

    // Method to tilt card forward for one moment when pair was found
    // Indicates found pair and makes balls disappear

    public void tiltForward()
    {
        Debug.Log("Card tilted forward");
        StartCoroutine(TiltCardForward());
    }


    public void setCardMaterial(Material mat)
    {
        front.GetComponent<Renderer>().material = mat;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!(this.isFinished || this.isActivated) && !(gameManager.GetComponent<GameManager>().thrownBalls.Contains(other.GetInstanceID())))
        {
            Debug.Log(gameObject.name + " field collision!");
            gameManager.GetComponent<GameManager>().cardActivated(this);
        }
    }

    IEnumerator RotateToImage()
    {
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

    IEnumerator RotateToBack()
    {
        yield return new WaitForSeconds(3);

        Quaternion oldRotation = transform.rotation;
        Quaternion newRotation = initialRotation;

        for (var t = 0f; t < 1; t += Time.deltaTime / ROTATION_TIME)
        {
            transform.rotation = Quaternion.Slerp(oldRotation, newRotation, t);
            yield return null;
        }

        transform.rotation = initialRotation;

        isActivated = false;
        yield return null;
    }

    private IEnumerator TiltCardForward()
    {
        Debug.Log("Tilt Card Forward called");
        yield return new WaitForSeconds(1);

        Quaternion oldRotation = transform.rotation;
        Debug.Log("Old Rotation" + oldRotation.ToString());
        //Quaternion newRotation = Quaternion.Euler(oldRotation.x+20, oldRotation.y, oldRotation.z);
        //Debug.Log("New Rotation" + newRotation.ToString());

        for (var t = 0f; t < 1; t += Time.deltaTime / ROTATION_TIME)
        {
            transform.rotation = Quaternion.Slerp(oldRotation, initialRotation, t);
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);

        for (var t = 0f; t < 1; t += Time.deltaTime / ROTATION_TIME)
        {
            transform.rotation = Quaternion.Slerp(initialRotation, oldRotation, t);
            yield return null;
        }
        transform.rotation = oldRotation;

        yield return null;
    }

}
