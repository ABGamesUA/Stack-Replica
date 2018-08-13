using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {	
	public static PauseMenu instance; 
	public GameObject ui;
	public Stack stack;	
	public AudioSource click;


private void Awake()
{
	instance = this;
}
private void Start()
{
	if(Sound.isOff) click.enabled = false;	
}

	void Update () {
		if(Input.GetKeyDown(KeyCode.Escape)||Input.GetKeyDown(KeyCode.P)){
			Tougle();
		}
	}		

	public void Tougle(){			
		stack.enabled = !stack.enabled;		
		ui.SetActive(!ui.activeSelf);
		if(ui.activeSelf){
			Time.timeScale = 0f;
		} else
		{
			if(click.enabled == true)click.Play();
			Time.timeScale = 1f;			
		} 
	}

	public void Retry (string sceneName){
		if(click.enabled == true)click.Play();
		Tougle();
		SceneManager.LoadScene(sceneName);
	}

	public void Menu (){
		if(click.enabled == true)click.Play();
		//Tougle();
		SoundManager.instance.PlayMenuBGSound();
		SceneManager.LoadScene("Menu");
	}
}
