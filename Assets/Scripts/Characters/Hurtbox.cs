using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurtbox : MonoBehaviour
{
    private CharacterClass character;

    public void Awake()
    {
        character = GetComponent<CharacterClass>();
    }

    public void Hurt(int damage)
    {
        Debug.Log(this.tag+" has been hit!");
        character.SetHealth(character.health-damage);
    }

    public void Hurt(int damage, PlayerClass player)
    {
        player.SetHealth(player.health-damage);
    }
}