using UnityEngine;
using System.Collections;

public class Cameraman : MonoBehaviour
{
	public float CAMERA_SPEED = 5.0f;
	public float playerOffset = 0;
	public PlayerView player;
	public Transform altTarget = null;

	void Start()
	{
		player = GameObject.Find("Player").GetComponent<PlayerView>();
		Debug.Assert(player != null);

		Camera cam = Camera.main;
		float height = 2f * cam.orthographicSize;
		float width = height * cam.aspect;
		playerOffset = width / 4;
	}

	void LateUpdate()
	{
		if(altTarget == null)
		{
			Vector3 pos = transform.position;
			pos.x = player.transform.position.x + playerOffset * Mathf.Sign(player.facing);
			transform.position = Vector3.Lerp(transform.position, pos, CAMERA_SPEED * Time.deltaTime);
		}
		else
		{
			Vector3 pos = transform.position;
			pos.x = altTarget.position.x;
			transform.position = Vector3.Lerp(transform.position, pos, CAMERA_SPEED * Time.deltaTime);
		}
	}
}
