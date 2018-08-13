using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {
	public static SoundManager instance;
	public AudioSource menuBG;
	public AudioSource gameBG;

	// Use this for initialization
	private void Awake()
	{
		if(instance == null) instance = this;
		else if(instance != this) Destroy(gameObject);
		DontDestroyOnLoad(gameObject);
	}

	public void PlayMenuBGSound()
	{
		gameBG.Stop();
		menuBG.Play();
	}

	public void PlayGameBGSound()
	{
		menuBG.Stop();
		gameBG.Play();
	}
	
	
}
