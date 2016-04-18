using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour
{
	private SpriteRenderer sr;

	void Awake()
	{	
		sr = gameObject.GetComponent<SpriteRenderer>();
	}

	public void setLayer(string sortingLayer, int sortingOrder)
	{
		sr.sortingLayerName = sortingLayer;
		sr.sortingOrder = sortingOrder;
	}
}
