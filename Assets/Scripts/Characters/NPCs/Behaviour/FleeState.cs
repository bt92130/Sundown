using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleeState : BaseState
{
    private StateMachine stateMachine;
    private List<Vector2> path;

    public FleeState(CharacterClass ch) : base(ch)
    {
        stateMachine = ch.stateMachine;
        path = new List<Vector2>();

        stateMachine.OnStateChanged += GetPath;
    }

    public override Type Tick()
    {
        rb.velocity = Vector2.zero;
        //if wildlife, run away from player
        if(character is Wildlife)
        {
            rb.velocity = new Vector2(-1f, -1f)*PathFinding.GetVelocity(character.floorPosition, player.floorPosition, character.speed*2);
        }
        //if townie, run home
        else
        {
            if(path.Count > 0)
            {
                rb.velocity = PathFinding.GetVelocity(character.floorPosition, path[0], character.speed);
                if((character.floorPosition - path[0]).sqrMagnitude <= PathFinding.TOLERANCE) 
                    path.RemoveAt(0);
            }
            else
            {
                // once path is done, switch to a different state
                return typeof(RestState);
            }
        }
        return null;
    }

    //subscribed to OnStateChanged event
    private void GetPath(BaseState state)
    {
        if(state is FleeState && character is TownspersonClass)
		    path = PathFinding.AStarJump(character.floorPosition, ((TownspersonClass)character).building.entrance.transform.position, character.nodeMap, character.nodeID);
    }
}