using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponRotator : MonoBehaviour
{
    public Transform weaponTransform;
    public PlayerController player;

    private void Start()
    {
        player = GetComponentInParent<PlayerController>();
    }
    void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePos - weaponTransform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if (!player.isRight)
            angle -= 180;
        weaponTransform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
