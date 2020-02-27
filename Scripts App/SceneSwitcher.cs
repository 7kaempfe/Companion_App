using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Dieses Script wird benutzt um den Scene zu wechseln.
public class SceneSwitcher : MonoBehaviour
{
	public void goToAnmeldeScreen()
	{
		SceneManager.LoadScene("Anmelden");
	}
    public void goToMusikScene()
	{
		SceneManager.LoadScene("Musik_Suche");
	}

	public void goToHomeScreen()
	{
		SceneManager.LoadScene("Homescreen");
	}

    public void goEinstellungenScreen()
    {
        SceneManager.LoadScene("Einstellungen");
    }

    public void goToFotoListeScreen()
	{
		SceneManager.LoadScene("Fotos_Liste");
	}

	public void goToFotoAddScreen()
	{
		SceneManager.LoadScene("Fotos_Add");
	}
	public void goToInteressen_AddScreen()
	{
		InterestUploadScript.ID = 0;
		SceneManager.LoadScene("Interessen_Add");
	}
	public void goToInteressen_BeschreibungScreen()
	{
		SceneManager.LoadScene("Interessen_Beschreibung");
	}

	public void goToInteressen_ListeScreen()
	{
		//yield return new WaitForSeconds(5);
		SceneManager.LoadScene("Interessen_Liste");
	}
	public void goToMusik_AddScreen()
	{
		SceneManager.LoadScene("Musik_Add");
	}
	public void goToMusik_ListeScreen()
	{
		SceneManager.LoadScene("Musik_Liste");
	}
	public void goToSpielerfolgScreen()
	{
		SceneManager.LoadScene("Spielerfolg");
	}

    public void goToRegistrierenScreen()
    {
        SceneManager.LoadScene("Registrieren");
    }

    public void goToAnmeldenScene()
    {
        SceneManager.LoadScene("Anmelden");
    }

}
