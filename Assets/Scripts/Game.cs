using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Spewnity;

public class Game : MonoBehaviour
{
	public GameObject enemyPrefab;
	public GameObject playerPrefab;
	public EnemyStat[] enemyStats;
	public float enemyPlacementOffset;
	private Vector3 enemyPlaceholder;
	private Cameraman cameraman;
	private PlayerController playerController;
	private Transform reset;

	public void Awake()
	{
		enemyPlaceholder = transform.Find("EnemyPlaceholder").position;
		cameraman = Camera.main.GetComponent<Cameraman>();
		playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
		reset = transform.Find("/Game/Reset");
	}

	public void Start()
	{
		cameraman.disableTracking();
		playerController.enabled = false;
		SoundManager.instance.play("theme");
	}

	public void skipIntro()
	{
		GameObject titling = GameObject.Find("/Titling");
		if(titling != null) titling.SetActive(false);
		removeEnemies();
		addEnemies();
		initGame();
	}

	public void startGame(Button startButton)
	{	
		startButton.interactable = false;
		CanvasGroup titling = GameObject.Find("/Titling").GetComponent<CanvasGroup>();
		StartCoroutine(titling.alpha.LerpFloat(0f, 2f, (float a) => titling.alpha = a, null, startScript));
	}

	public void startScript()
	{
		addEnemies();
		cameraman.dollyTo(7f, playerController.transform.position.x, null, null, null);
		Invoke("initGame", 7f);
	}

	public void initGame()
	{
		playerController.enabled = true;
		cameraman.enableTracking();
		EnemyBehavior.runMode = EnemyRunMode.StopScript;
	}

	public void removeEnemies()
	{
		foreach(Transform child in reset)
		{
			GameObject.Destroy(child.gameObject);
		}
	}

	public void addEnemies()
	{
		enemyStats.Shuffle();
		Vector3 pos = enemyPlaceholder;
		foreach (EnemyStat stat in enemyStats)
		{
			GameObject go = (GameObject) Instantiate(enemyPrefab, pos, Quaternion.identity);
			Debug.Assert(go != null);
			go.name = stat.name;
			go.transform.parent = reset;

			EnemyView view = go.GetComponent<EnemyView>();
			Debug.Assert(view != null);
			view.setGarment(stat.garment);

			EnemyBehavior behavior = go.GetComponent<EnemyBehavior>();
			Debug.Assert(behavior != null);
			behavior.armed = stat.armed;
			view.armed = stat.armed;
			behavior.speedModifier = stat.speed;

			// Move placeholder to next position
			pos.x += enemyPlacementOffset;
		}
	}
}

[System.Serializable]
public struct EnemyStat
{
	public string name;
	public bool armed;
	public GarmentType garment;
	public float speed;
}