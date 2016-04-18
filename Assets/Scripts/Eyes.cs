using UnityEngine;
using System.Collections;

public class Eyes : MonoBehaviour
{
	public Sprite closed;
	public Sprite calm;
	public Sprite squint;
	public Sprite open;
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
			case EyeType.None:
				sprite = null;
				break;
			case EyeType.Calm:
				sprite = calm;
				break;
			case EyeType.Closed:
				sprite = closed;
				break;
			case EyeType.Open:
				sprite = open;
				break;
			case EyeType.Squint:
				sprite = squint;
				break;
			default:
				throw new UnityException("WTF");
		}

		Debug.Assert(sprite != null);
		sr.sprite = sprite;
	}

	public void setFlipped(bool flipped)
	{
		sr.flipX = flipped;
	}

	public void setLayer(string sortingLayer, int sortingOrder)
	{
		sr.sortingLayerName = sortingLayer;
		sr.sortingOrder = sortingOrder;
	}
}

public enum EyeType
{
	None,
	Closed,
	Calm,
	Squint,
	Open
}
