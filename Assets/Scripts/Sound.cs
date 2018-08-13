using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Sound {	
	public static bool isOff = false;
	
	public static void PlayClickSound(AudioSource clickSound)
	{
		if(clickSound.enabled == true) clickSound.Play();
		else return;
	}	
}
