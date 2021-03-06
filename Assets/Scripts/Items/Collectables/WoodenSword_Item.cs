using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodenSword_Item : Item
{
    public Weapon weapon;

    // Start is called before the first frame update
    void Awake()
    {
        initStart("Wooden Sword",false, true, "My old Wooden Sword, Cool! Damage increase: +2");
        weapon = GameObject.Find("woodenSword_weapon").GetComponent<Weapon>();
    }

    // Update is called once per frame
    void Update()
    {
        initUpdate();
    }


    public override void useItem() {
        toggleWeapon(weapon);
    }
}
