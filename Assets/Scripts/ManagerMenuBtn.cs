using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ManagerMenuBtn : MonoBehaviour 
{
	public void StartGameBtn (string FirstWave)
	{
		SceneManager.LoadScene (FirstWave);
	}

    public void CloseGameBtn ()
    {
        Application.Quit();
    }

}
