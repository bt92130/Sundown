using System;
using System.Collections.Generic;
using UnityEngine;

public class RangerChaseState : BaseState
{
    private static readonly float VISION_RANGE = 100f;
    private Ranger ranger;
    private StateMachine stateMachine;
    private List<Vector2> path;

    public RangerChaseState(Ranger ch) : base(ch)
    {
        ranger = ch;
        stateMachine = ch.stateMachine;
        path = new List<Vector2>();
    }

    public override Type Tick()
    {
        rb.velocity = Vector2.zero;

        //if valid LOS, attack
        if(HasLineOfSight())
        {
            path.Clear();
            return typeof(RangerAttackState);
        }

        //else move towards player while checking
        if(path.Count == 0)
            path = PathFinding.AStarJump(ranger.floorPosition, player.floorPosition, ranger.nodeMap, character.nodeID);
        else
        {
            rb.velocity = PathFinding.GetVelocity(ranger.floorPosition, path[0], ranger.speed);
            if((ranger.floorPosition - path[0]).sqrMagnitude <= PathFinding.TOLERANCE)
                path.RemoveAt(0);

            //if we reach where the end of path without LOS, go to idle
            if(path.Count == 0)
            {
                ranger.SetIsAlarmed(false);
                return typeof(RangerIdleState);
            }
        }
        return null;
    }

    private bool HasLineOfSight()
    {
        Vector2 direction = player.floorPosition-ranger.floorPosition;
        if((ranger.isLeft && Mathf.Sign(direction.x) < 0) || (!ranger.isLeft && Mathf.Sign(direction.x) > 0))
        {
            Debug.DrawRay(ranger.floorPosition, player.floorPosition-ranger.floorPosition, Color.red, 2f);
            RaycastHit2D vision = Physics2D.Raycast(ranger.floorPosition, player.floorPosition - ranger.floorPosition, VISION_RANGE, 1 << LayerMask.NameToLayer("Default"));
			if(vision.collider != null)
			{
				if(vision.collider.gameObject.name == "Player")
					return true;
			}
        }
        return false;
    }
}