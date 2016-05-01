using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Spewnity;

public class Game : MonoBehaviour
{
	public GameObject enemyPrefab;
	public GameObject playerPrefab;
	public List<EnemyStat> enemyStats;
	public float enemyPlacementOffset;
	private Vector3 enemyPlaceholder;
	private Cameraman cameraman;
	private PlayerController playerController;
	private Transform reset;
	private ActionQueue script;

	public void Awake()
	{
		enemyPlaceholder = transform.Find("EnemyPlaceholder").position;
		cameraman = Camera.main.GetComponent<Cameraman>();
		playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
		reset = transform.Find("/Game/Reset");
		script = gameObject.AddComponent<ActionQueue>();
	}

	public void skipIntro()
	{
		GameObject titling = GameObject.Find("/Titling");
		if(titling != null) titling.SetActive(false);

		script.Clear();
		CancelInvoke();
		SoundManager.instance.Stop("theme");
		StopAllCoroutines();

		removeEnemies();
		addEnemies();
		initGame();
	}

	public void Start()
	{
		cameraman.disableTracking();
		playerController.enabled = false;
		SoundManager.instance.Play("theme");
	}

	public void startGame()
	{	
		Button button = GameObject.Find("/Titling/Button").GetComponent<Button>();
		if(!button.interactable) return;
		
		button.interactable = false;
		CanvasGroup titling = GameObject.Find("/Titling").GetComponent<CanvasGroup>();
		StartCoroutine(titling.alpha.LerpFloat(0f, 2f, (float a) => titling.alpha = a, null, startScript));
	}

	public void startScript()
	{
		EnemyBehavior.changeRunMode.Invoke(EnemyRunMode.RunScript, null);
		addEnemies();
		cameraman.dollyTo(7f, GameObject.Find("MacReady").transform.position.x, null, null, null);
		script
			.Add(() => EnemyBehavior.changeRunMode.Invoke(EnemyRunMode.StartScript, "burp"))
			.Delay(8f)
			.Add(() => SoundManager.instance.GetSource("theme").volume *= 0.4f)
			.PlaySound("macready1")
			.Delay(2f)
			.PlaySound("macready2")
			.Delay(2f)
			.PlaySound("macready3")
			.Delay(3f)
			.PlaySound("macready4")
			.Delay(2f)
			.Add(() => SoundManager.instance.FadeOut("theme", 1f))
			.Add(initGame)
			.Run();
	}

	public void initGame()
	{
		playerController.enabled = true;
		cameraman.enableTracking();
		EnemyBehavior.changeRunMode.Invoke(EnemyRunMode.StopScript, null);
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
		// Shuffle enemies
		// Force MacReady to the front
		EnemyStat? macReady = null;
		foreach(EnemyStat enemy in enemyStats)
		{
			if(enemy.name == "MacReady")
			{
				macReady = (EnemyStat) enemy;
				enemyStats.Remove(enemy);
				break;
			}
		}
		Debug.Assert(macReady != null);
		EnemyStat[] shuffleStats = enemyStats.ToArray().Shuffle();
		enemyStats.Clear();
		enemyStats.Add((EnemyStat) macReady);
		foreach(EnemyStat enemy in shuffleStats) enemyStats.Add(enemy);
			
		// Place all enemies
		Vector3 pos = enemyPlaceholder;
		foreach(EnemyStat stat in enemyStats)
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
	