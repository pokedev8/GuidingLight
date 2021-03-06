﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinePath : MonoBehaviour 
{
	[SerializeField]
	PlayerController m_PlayerController;

	[SerializeField]
	LineRenderer m_Line;

	Vector2 m_CurrentTilePos;

	GameObject m_CurrentTile;
	GameObject m_NextTile;

	bool m_CanDraw;

	float m_MaxLength = 1.2f;

	void Awake()
	{
		
		m_PlayerController = FindObjectOfType<PlayerController>();
		m_CurrentTile = m_PlayerController.ReturnTile();
		m_CurrentTilePos = new Vector2(m_CurrentTile.transform.position.x, m_CurrentTile.transform.position.y);
		m_CanDraw = true;
		StartCoroutine("OverOtherCandle");
	}

	void Update () 
	{	
		if (m_CanDraw)
		{
			Vector2 linePos = (m_PlayerController.ReturnMousePosition() - m_CurrentTilePos);
			m_Line.SetPosition(0, linePos);
			float lineX = m_Line.GetPosition(0).x;
			float lineY = m_Line.GetPosition(0).y;

			if (lineX > m_MaxLength || lineY > m_MaxLength || lineX < -m_MaxLength || lineY < -m_MaxLength)
			{
				DestroyLine();
			}
		}
	}

	IEnumerator OverOtherCandle()
	{
		yield return new WaitUntil(() => new Vector2 (m_PlayerController.ReturnTile().transform.position.x, m_PlayerController.ReturnTile().transform.position.y) != m_CurrentTilePos);
		if (m_PlayerController.ReturnTile().GetComponent<TileScript>().TileCheck(m_CurrentTilePos, m_PlayerController.ReturnTile().GetComponent<TileScript>().ReturnTilePos()))
		{
			m_CurrentTile.GetComponent<TileScript>().DisableCollider();
			m_NextTile = m_PlayerController.ReturnTile();
			//if line position is not equal to entrance position && 
			//line positioni s not equal to exit position
			//add the positions to the list
			Vector2 linePos = m_Line.transform.position;
			if(linePos != GameController.instance.ReturnEntrancePos() && linePos != GameController.instance.ReturnExitPos())
			{
				GameController.instance.AddList((int)linePos.x, (int)linePos.y);
			}
			m_NextTile.GetComponent<TileScript>().Light(true);
			m_PlayerController.LineDrawn(false);
			m_Line.SetPosition(0, m_NextTile.GetComponent<TileScript>().ReturnTilePos() - m_CurrentTilePos);
			m_CanDraw = false;
		}
		else
		{
			DestroyLine();
		}
	}

	public void DestroyLine()
	{
		m_PlayerController.LineDrawn(false);
		m_PlayerController.PopLineFromStack();
		Destroy(gameObject);
	}
}
