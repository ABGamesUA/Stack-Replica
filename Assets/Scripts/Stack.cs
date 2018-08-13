using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Stack : MonoBehaviour {
	public Color32[] gameColors = new Color32[4];
	public Material stackMat;
	private PauseMenu pm;
	public Text scoreText;
	public Text currentScore;
	public GameObject endPanel;
	public GameObject buildEffect;	
	public GameObject comboEffect;
	public AudioSource failPutDown;
	public AudioSource sucseessPutDown;
	public AudioSource ComboPutDown;
	public AudioSource endGame;	

	private const float BOUNDS_SIZE = 3f;
	private const float STACK_MOVING_SPEED = 5f;
	private const float ERROR_MARGIN = .1f;
	private const float STACK_BOUNDS_GAIN = .25f;
	private const int COMBO_START_GAIN = 1;

	private GameObject[] theStack;
	private Vector2 stackBounds = new Vector2(BOUNDS_SIZE, BOUNDS_SIZE);

	private int scoreCount = 0;
	private int stackIndex;
	private int combo = 0;

	private float tileTransition = 0.0f;
	private float tileSpeed = 2.5f;
	private float secondaryPosition;

	private bool isMovingOnX = true;
	private bool gameOver = false;
	
	private Vector3 desiredposition;
	private Vector3 lastTilePosition;
	
	private void Start () {			
		/*if(Sound.isOff)
		{
			failPutDown.enabled = false;
			sucseessPutDown.enabled = false;
			ComboPutDown.enabled = false;
			endGame.enabled = false;
		}
		else
		{
			failPutDown.enabled = true;
			sucseessPutDown.enabled = true;
			ComboPutDown.enabled = true;
			endGame.enabled = true;	
		}*/

		this.enabled = true;
		Time.timeScale = 1f;
		theStack = new GameObject[transform.childCount];
		for (int i = 0; i < transform.childCount; i++)
			{
				theStack[i] = transform.GetChild (i).gameObject;
				ColorMesh(theStack[i].GetComponent<MeshFilter>().mesh);
			}
		stackIndex = transform.childCount - 1;		
	}
	
	private void CreateRubble(Vector3 pos, Vector3 scale)
	{
		GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
		go.transform.localPosition = pos;
		go.transform.localScale = scale;
		go.AddComponent<Rigidbody>();		
		go.GetComponent<MeshRenderer>().material = stackMat;
		ColorMesh(go.GetComponent<MeshFilter>().mesh);		
		Destroy(go, 2f);
	}


	private void Update () {
		if(gameOver)return;			
				if(Input.GetMouseButtonDown(0)){		
						if(PlaceTile())
						{
							SpawnTile();
							scoreCount++;
							scoreText.text = scoreCount.ToString();
							if(failPutDown.enabled == true)failPutDown.Play();	
							
						}
						else {
							EndGame();
						}
				}
			
		MoveTile();
		transform.position = Vector3.Lerp(transform.position,desiredposition,STACK_MOVING_SPEED*Time.deltaTime);
		}


	private void SpawnTile()
	{
		ColorMesh(theStack[stackIndex].GetComponent<MeshFilter>().mesh);
		lastTilePosition = theStack[stackIndex].transform.localPosition;
		stackIndex--;
		if(stackIndex < 0) stackIndex = transform.childCount - 1;
		desiredposition = (Vector3.down) * scoreCount;
		theStack [stackIndex].transform.localPosition = new Vector3 (0, scoreCount, 0);
		theStack[stackIndex].transform.localScale = new Vector3(stackBounds.x, 1 ,stackBounds.y);
		ColorMesh(theStack[stackIndex].GetComponent<MeshFilter>().mesh);		
	}

	public bool PlaceTile(){
		Transform t = theStack[stackIndex].transform;
		if(isMovingOnX){
			float deltaX = lastTilePosition.x - t.position.x;
			if(Mathf.Abs(deltaX) > ERROR_MARGIN){
				
				combo = 0;
				stackBounds.x -= Mathf.Abs(deltaX);
				if(stackBounds.x <=0) return false;
				float middle = lastTilePosition.x + t.localPosition.x / 2;
				t.localScale = new Vector3(stackBounds.x, 1 ,stackBounds.y);
				CreateRubble
					(
					new Vector3((t.position.x > 0)
					? t.position.x + (t.localScale.x/2)
					: t.position.x - (t.localScale.x/2)
					,t.position.y
					,t.position.z),
					new Vector3(Mathf.Abs(deltaX),1,t.localScale.z)
					);
				t.localPosition = new Vector3(middle - (lastTilePosition.x/2), scoreCount, lastTilePosition.z);
			}
			else {
				if(combo > COMBO_START_GAIN){
					if(ComboPutDown.enabled == true)ComboPutDown.Play();
					stackBounds.x += STACK_BOUNDS_GAIN;
					if(stackBounds.x > BOUNDS_SIZE) stackBounds.x = BOUNDS_SIZE;
					float middle = lastTilePosition.x + t.localPosition.x / 2;
					t.localScale = new Vector3(stackBounds.x, 1 ,stackBounds.y);
					t.localPosition = new Vector3(middle - (lastTilePosition.x/2), scoreCount, lastTilePosition.z);
					GameObject cEffect = (GameObject) Instantiate(comboEffect, t.transform.position, Quaternion.identity);	
				Destroy(cEffect, 2f);
				}
				else if(sucseessPutDown.enabled == true)sucseessPutDown.Play();
				
				combo++;
				t.localPosition = new Vector3(lastTilePosition.x, scoreCount, lastTilePosition.z);
				GameObject effect = (GameObject) Instantiate(buildEffect, t.transform.position, Quaternion.identity);	
				Destroy(effect, 2f);			
			}
		}
		else 
		{
			float deltaZ = lastTilePosition.z - t.position.z;
			if(Mathf.Abs(deltaZ) > ERROR_MARGIN){			
				combo = 0;
				stackBounds.y -= Mathf.Abs(deltaZ);
				if(stackBounds.y <=0) return false;
				float middle = lastTilePosition.z + t.localPosition.z / 2;
				t.localScale = new Vector3(stackBounds.x, 1 ,stackBounds.y);
				CreateRubble
					(
					new Vector3(t.position.x
					,t.position.y,
					(t.position.z > 0)
					? t.position.z + (t.localScale.z/2)
					: t.position.z - (t.localScale.z/2)),					
					new Vector3(t.localScale.x,1,Mathf.Abs(deltaZ))
					);
				t.localPosition = new Vector3(lastTilePosition.x, scoreCount,  middle-(lastTilePosition.z/2));
			}
			else {
				if(combo > COMBO_START_GAIN){
					if(ComboPutDown.enabled == true)ComboPutDown.Play();
					stackBounds.y += STACK_BOUNDS_GAIN;
					if(stackBounds.y > BOUNDS_SIZE) stackBounds.y = BOUNDS_SIZE;
					float middle = lastTilePosition.z + t.localPosition.z / 2;
					t.localScale = new Vector3(stackBounds.x, 1 ,stackBounds.y);
					t.localPosition = new Vector3(lastTilePosition.x, scoreCount,  middle-(lastTilePosition.z));
					GameObject cEffect = (GameObject) Instantiate(comboEffect, t.transform.position, Quaternion.identity);	
				Destroy(cEffect, 2f);		
				}
				else if(sucseessPutDown.enabled == true) sucseessPutDown.Play();				
				combo++;
				t.localPosition = new Vector3(lastTilePosition.x, scoreCount, lastTilePosition.z);
				GameObject effect = (GameObject) Instantiate(buildEffect, t.transform.position, Quaternion.identity);	
				Destroy(effect, 2f);
			}
		}
		secondaryPosition = (isMovingOnX)
		? t.localPosition.x
		: t.localPosition.z;
		isMovingOnX = !isMovingOnX;
		return true;}

	private void MoveTile()
	{		
		tileTransition += Time.deltaTime * tileSpeed;
		if(isMovingOnX)	theStack[stackIndex].transform.localPosition = new Vector3(Mathf.Sin(tileTransition) * BOUNDS_SIZE, scoreCount, secondaryPosition);
		else theStack[stackIndex].transform.localPosition = new Vector3(secondaryPosition, scoreCount, Mathf.Sin(tileTransition) * BOUNDS_SIZE);}
	
	private Color32 Lerp4(Color32 a, Color32 b, Color32 c, Color32 d, float t){
		if (t < .33f) return Color.Lerp(a, b, t/.33f);
		else if (t<.66f) return Color.Lerp(b,c, (t-0.33f)/.33f);
		else return Color.Lerp(c,d, (t-0.66f)/.66f);
	}
	private void ColorMesh(Mesh mesh)
	{
		Vector3[] vertices = mesh.vertices;
		Color32[] colors = new Color32[vertices.Length];
		float f = Mathf.Sin(scoreCount * .25f);
		for (int i = 0; i < vertices.Length; i++)
		{
			colors[i] = Lerp4(gameColors[0], gameColors[1], gameColors[2], gameColors[3], f);
		}
		mesh.colors32 = colors;
	}
	
	
	private void EndGame(){
		if(endGame.enabled == true)endGame.Play();
		if(PlayerPrefs.GetInt("score") < scoreCount) PlayerPrefs.SetInt("score", scoreCount);
		gameOver = true;
		currentScore.text = "YOUR SCORE " + scoreCount.ToString();		
		theStack[stackIndex].AddComponent<Rigidbody>();		
		endPanel.SetActive(true);
		PauseMenu.instance.enabled = false;
		}
	
	
}
