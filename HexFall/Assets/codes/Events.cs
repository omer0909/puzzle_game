using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Events : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler
{
	[SerializeField]
	LevelManager m;
	bool drag;

	public void OnPointerDown(PointerEventData eventData)
	{
		if (!LevelManager.lockGame)
			drag = false;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (drag == false && !LevelManager.lockGame)
			m.SelectIcon(eventData.position);
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		drag = true;
		if (!LevelManager.lockGame)
			m.BeginDrag(eventData.position);
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (!LevelManager.lockGame)
			m.Draging(eventData.position);
	}
	void Start()
	{

	}

	void Update()
	{

	}
}
