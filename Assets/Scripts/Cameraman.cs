using UnityEngine;
using System.Collections;

public class Cameraman : MonoBehaviour
{
	public float cameraSpeed = 5.0f;
	public Transform target;
	public Vector3 velocity = Vector3.zero;
	// 0.0 x 0.0 position is target at center of camera.
	// To shift camera focus point, set offset as percentage -0.5 to 0.5.
	// To prevent this shift for any axis, leave as null.
	public float? offsetX, offsetY;
	private float width, height;

	void Start()
	{
		Camera cam = Camera.main;
		height = 2f * cam.orthographicSize;
		width = height * cam.aspect;
	}

	public void setTarget(Transform target, float? speed = null, float? offsetX = null, float? offsetY = null)
	{
		this.target = target;
		this.offsetX = offsetX;
		this.offsetY = offsetY;
		if(speed != null) cameraSpeed = (float) speed;
	}
		
	public void jumpTo(float x, float y)
	{
		target = null;
		Vector3 pos = transform.position;
		pos.x = x;
		pos.y = y;
		transform.position = pos;
	}

	void LateUpdate()
	{		
		if(target == null)
			return;

		Vector3 pos = transform.position;
		pos.x = target.transform.position.x + (offsetX == null ? 0 : width * (float) offsetX);
		pos.y = target.transform.position.y + (offsetY == null ? 0 : height * (float) offsetY);
		transform.position = Vector3.SmoothDamp(transform.position, pos, ref velocity, cameraSpeed);
		Debug.Log("Moving camera to " + transform.position.x + "," + transform.position.y + " speed:" + cameraSpeed);
	}
}