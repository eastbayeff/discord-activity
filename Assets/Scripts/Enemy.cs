using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IClickable
{
    public int MaxHealth = 100;

    public void OnClick(Player p)
    {
        Debug.Log("Enemy Selected");
    }
}
