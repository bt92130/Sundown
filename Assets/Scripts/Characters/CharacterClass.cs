﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//CONSIDER MAKING THIS CLASS NON MONOBEHAVIOUR SO WE CAN USE A NORMAL CONSTRUCTOR INSTEAD OF THE AWAKE FNC
public class CharacterClass : MonoBehaviour
{
	public readonly float BASE_SPEED = 20f;
	public int nodeID {get; private set;}
	public int health {get; private set;}
	public float speed {get; private set;}
	public float floorHeight {get; private set;}
	public bool isLeft {get; private set;}
	public GameState gameState {get; private set;}
	public Transform trans {get; private set;}
	public Rigidbody2D rb {get; private set;}
	public SpriteRenderer rend {get; private set;}

	public void SetNodeID(int id){nodeID = id;}
	public void SetIsLeft(bool isLeft){this.isLeft = isLeft;}
	public void SetHealth(int health){this.health = health;}
	public void SetSpeed(int speed){this.speed = speed;}

	public virtual void Awake()
	{
		gameState = GameObject.Find("GameState").GetComponent<GameState>();
		trans = GetComponent<Transform>();
		rb = GetComponent<Rigidbody2D>();
		rend = GetComponent<SpriteRenderer>();
	}

	public virtual void Reset()
	{
		speed = BASE_SPEED;
	}
	
	//sets position so that floor position is at target
	public Vector2 SetFloorPosition(Vector2 target)
	{
		float yOffset = rend.bounds.size.y/2;
		return new Vector2(target.x, target.y+yOffset);
	}

	//sets floor height (bottom of sprite), used for collisions and rendering orders
	protected void setFloorHeight()
	{
		floorHeight = trans.position.y-(rend.bounds.size.y/2);
		trans.position = new Vector3(trans.position.x, trans.position.y, floorHeight);
	}

}
