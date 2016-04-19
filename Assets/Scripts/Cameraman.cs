using UnityEngine;
using System.Collections;

public class Cameraman : MonoBehaviour
{
	public float cameraSpeed = 5.0f;
	public CameramanTarget goal = CameramanTarget.Main;
	public Transform mainTarget;
	public Transform altTarget = null;
	[Tooltip("Offset makes camera shift to the left or right of the target. Optimal values are between -0.4f and 0.4f.")]
	public float mainOffset = -0.25f; 
	public float altOffset = 0.0f; 
	private float width;

	void Start()
	{
		Camera cam = Camera.main;
		float height = 2f * cam.orthographicSize;
		width = height * cam.aspect;
//		mainTargetOffset = width / 4;
	}

	public void setGoal(CameramanTarget goal, Transform altTarget = null)
	{
		this.goal = goal;
		if(altTarget != null) this.altTarget = altTarget;
	}

	void LateUpdate()
	{		
		if(goal == CameramanTarget.None)
			return;

		Transform target = (goal == CameramanTarget.Main ? mainTarget : altTarget);
		float offset = (goal == CameramanTarget.Main ? mainOffset : altOffset);

		if(target == null)
		{
			Debug.Log(goal + " camera target is missing");
			return;
		}

		Vector3 pos = transform.position;
		pos.x = target.transform.position.x + (width * offset);
		transform.position = Vector3.Lerp(transform.position, pos, cameraSpeed * Time.deltaTime);
	}
}

public enum CameramanTarget
{
	None,
	Main,
	Alt
}