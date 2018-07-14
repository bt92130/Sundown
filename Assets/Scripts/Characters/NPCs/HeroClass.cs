﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroClass : CharacterClass
{
	private float tracking; //used to determine how likely a clue will be discovered
	private float leash = 20f;
	private PlayerClass player;

	public override void Awake()
	{
		base.Awake();
		player = GameObject.Find("Player").GetComponent<PlayerClass>();
	}

	public virtual void Track()
	{
		Debug.Log("Default Track!");
	}

	// private void follow()
	// {
	// 	if(nodeID != player.nodeID)
	// 		return;
	// 	float distance = Vector2.Distance(player.trans.position, trans.position);
	// 	if(distance > leash)
	// 	{
	// 		rb.velocity = getVelocityTowardPlayer();
	// 	}
	// 	else
	// 	{
	// 		rb.velocity = Vector2.zero;
	// 	}
	// }

	// private Vector2 getVelocityTowardPlayer()
	// {
	// 	float distance;
	// 	Vector2 direction;
	// 	Vector2 velocity;
	// 	distance = Vector2.Distance(player.trans.position, trans.position);
	// 	direction = player.trans.position - trans.position;
	// 	velocity = new Vector2((direction.x*speed)/distance, (direction.y*speed)/distance);
	// 	return velocity;
	// }

	//called by gamestate in masterreset
	public override void Reset()
	{
		base.Reset();
		base.SetNodeID(0);//Random.Range(0, World.WORLD_SIZE-1));
		tracking = 0;
	}
}
