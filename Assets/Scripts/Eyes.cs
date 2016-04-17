using UnityEngine;
using System.Collections;

public class Eyes : MonoBehaviour
{
	public Sprite eyesClosed;
	public Sprite eyesCalm;
	public Sprite eyesSquint;
	public Sprite eyesOpen;
	private SpriteRenderer sr;

	void Awake()
	{	
		sr = gameObject.GetComponent<SpriteRenderer>();
	}
	
	public void setEyes(EyeType type)
	{
		Sprite sprite = null;
		switch(type)
		{
			case EyeType.Calm:
				sprite = eyesCalm;
				break;
			case EyeType.Closed:
				sprite = eyesClosed;
				break;
			case EyeType.Open:
				sprite = eyesOpen;
				break;
			case EyeType.Squint:
				sprite = eyesSquint;
				break;
			default:
				throw new UnityException("WTF");
		}

		sr.sprite = sprite;
	}

	public void setFlipped(bool flipped)
	{
		sr.flipX = flipped;
	}

	public void setSortingOrder(int order)
	{
		sr.sortingOrder = order;
	}
}

public enum EyeType
{
	Closed,
	Calm,
	Squint,
	Open
}
