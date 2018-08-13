using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {
	public Text scoreText;
	public AudioSource clickSound;

	public void Start(){	
		scoreText.text = "Best Score " + PlayerPrefs.GetInt("score").ToString();
		SoundManager.instance.PlayMenuBGSound();	
	}
	
	public void Play(){
		Sound.PlayClickSound(clickSound);
		SoundManager.instance.PlayGameBGSound();
		SceneManager.LoadScene("Main");
	}

	public void Exit()
	{
		Sound.PlayClickSound(clickSound);
		Application.Quit();
	}

}
