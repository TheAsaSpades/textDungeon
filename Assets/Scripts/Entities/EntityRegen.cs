﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityRegen : MonoBehaviour {

    public int delay = 2;

    private Entity ent;
	// Use this for initialization
	void Start () {
        ent = this.gameObject.GetComponent<Entity>();
        StartCoroutine(Regen());
	}


    IEnumerator Regen()
    {
        for(; ; ) { 
            if (ent.Stamina() < ent.MaxStamina())
                ent.SetStamina(delay);
            if (ent.Health() < ent.MaxHealth())
                ent.SetHealth(delay);
            if (ent.Mana() < ent.MaxMana())
                ent.SetMana(delay);
            yield return new WaitForSeconds(delay);
         }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
