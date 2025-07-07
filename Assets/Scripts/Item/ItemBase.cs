using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class ItemBase : MonoBehaviour
{
    public abstract void Apply(PlayerController player);
}
