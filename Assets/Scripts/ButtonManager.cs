using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour 
{
	public void LoadScene (string Scene)
	{
		SceneManager.LoadScene (Scene);
	}

    public void CloseGame()
    {
        Application.Quit();
    }

}
