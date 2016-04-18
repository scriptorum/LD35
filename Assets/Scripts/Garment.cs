using UnityEngine;
using System.Collections;

public class Garment : MonoBehaviour
{
	public Sprite bandana;
	public Sprite beard;
	public Sprite bigHair;
	public Sprite curlyHair;
	public Sprite eyebrows;
	public Sprite glasses;
	public Sprite goggles;
	public Sprite greenHat;
	public Sprite headLamp;
	public Sprite stache;
	public Sprite tenGallonHat;
	private SpriteRenderer sr;

	void Awake()
	{	
		sr = gameObject.GetComponent<SpriteRenderer>();
	}

	public void setGarment(GarmentType type)
	{
		Sprite sprite = null;
		switch(type)
		{
			case GarmentType.None:
				sprite = null;
				break;
			case GarmentType.Bandana:
				sprite = bandana;
				break;
			case GarmentType.Beard:
				sprite = beard;
				break;
			case GarmentType.BigHair:
				sprite = bigHair;
				break;
			case GarmentType.CurlyHair:
				sprite = curlyHair;
				break;
			case GarmentType.Eyebrows:
				sprite = eyebrows;
				break;
			case GarmentType.Glasses:
				sprite = glasses;
				break;
			case GarmentType.Goggles:
				sprite = goggles;
				break;
			case GarmentType.GreenHat:
				sprite = greenHat;
				break;
			case GarmentType.HeadLamp:
				sprite = headLamp;
				break;
			case GarmentType.Stache:
				sprite = stache;
				break;
			case GarmentType.TenGallonHat:
				sprite = tenGallonHat;
				break;
			default:
				throw new UnityException("WTF");
		}

		sr.sprite = sprite;
		Debug.Assert(sprite != null);
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

public enum GarmentType
{
	None,
	Bandana,
	Beard,
	BigHair,
	CurlyHair,
	Eyebrows,
	Glasses,
	Goggles,
	GreenHat,
	HeadLamp,
	Stache,
	TenGallonHat
}



