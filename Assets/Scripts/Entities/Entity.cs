﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Entity : MonoBehaviour {

    public string Name = "You";
    public Room location;
    public float atkSpeed = 2.0f;
	private float health = 90.0f, mana = 99.0f, stamina = 99.0f, strength = 10.0f, intel = 10.0f;
    private float maxHealth = 90.0f, maxMana = 99.0f, maxStamina = 99.0f;
	private int level = 1;
    private bool exiting = false;
    private Entity target;
    public bool isPlayer = false;
    public bool attacking = false;
    public bool resting = false;
    public int regen = 2;
    public int restRegen = 5;
    private bool dead = false;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
            dead = true;
        if (dead)
        {
            gameObject.SetActive(false);
        }
    }

    public float Mana() { return mana+(intel/10.0f); }

    public float Health() { return health+(level*10); }

    public float Stamina() { return stamina+(strength/10.0f); }

    public float MaxMana() { return maxMana + (intel / 10.0f); }

    public float MaxHealth() { return maxHealth + (level * 10); }

    public float MaxStamina() { return maxStamina + (strength / 10.0f); }

    public int Level() { return level; }

    public float Strength() { return strength; }

    public float Intel() { return intel; }

    public Room Location() { return location; }

    public bool IsPlayer() { return isPlayer; }
    
    public Entity Target() { return target; }
   
    public void SetStamina(float dec)
    {
        if (stamina + dec > MaxStamina())
            stamina = MaxStamina();
        else if (stamina + dec <= 0)
            stamina = 0;
        else
            stamina = stamina + dec;
    }

    public void SetHealth(float dec)
    {
        if (health + dec >= 100) // MaxHealth())
            health = MaxHealth();
        else if (health + dec <= 0)
            health = 0;
        else
            health = health + dec;
    }

    public void SetMana(float dec)
    {
        if (mana + dec > MaxMana())
            mana = MaxMana();
        else if (mana + dec <= 0)
            mana = 0;
        else
            mana = stamina + dec;
    }

    void LeaveCombat()
    {
        if (isPlayer)
        {
            GameObject chat = GameObject.Find("PlayerControl").gameObject;
            chat.transform.Find("ScrollView").gameObject.transform.Find("Viewport").gameObject.transform.Find("Content").gameObject.GetComponent<Text>().color = new Color(0, 0, 1);
            chat.GetComponent<UpdateChatText>().UpdateChat("<color=#ff0000ff>> Leaving Combat</color>");
            chat.transform.Find("ScrollView").gameObject.transform.Find("Viewport").gameObject.transform.Find("Content").gameObject.GetComponent<Text>().color = new Color(1, 1, 1);
            chat.transform.Find("ScrollView").gameObject.GetComponent<ScrollRect>().gameObject.transform.Find("Scrollbar Vertical").gameObject.GetComponent<Scrollbar>().value = 0.0f;
        }
        attacking = false;
        target = null;
    }

    public void TakeDamage(float damage, Entity e)
    {
        if (Health()-damage > 0)
        {
            if (isPlayer)
            {
                GameObject chat = GameObject.Find("PlayerControl").gameObject;
                chat.transform.Find("ScrollView").gameObject.transform.Find("Viewport").gameObject.transform.Find("Content").gameObject.GetComponent<Text>().color = new Color(0, 0, 1);
                chat.GetComponent<UpdateChatText>().UpdateChat("<color=#ff0000ff>> Enemy hit you for "+ damage +" damage!</color>");
                chat.transform.Find("ScrollView").gameObject.transform.Find("Viewport").gameObject.transform.Find("Content").gameObject.GetComponent<Text>().color = new Color(1, 1, 1);
                chat.transform.Find("ScrollView").gameObject.GetComponent<ScrollRect>().gameObject.transform.Find("Scrollbar Vertical").gameObject.GetComponent<Scrollbar>().value = 0.0f;
            }
            SetHealth(-damage);
            Attack(e);
        }
        else
        {
            LeaveCombat();
            e.LeaveCombat();
        }
    }

    public void DealDamage()
    {
        float damage = strength + Random.Range(-5, 5);
        if (this.isPlayer)
        {
            GameObject chat = GameObject.Find("PlayerControl").gameObject;
            chat.transform.Find("ScrollView").gameObject.transform.Find("Viewport").gameObject.transform.Find("Content").gameObject.GetComponent<Text>().color = new Color(0, 0, 1);
            chat.GetComponent<UpdateChatText>().UpdateChat("<color=#ff0000ff>>" + " Attacking " + target.Name + " for " + damage + " damage! " + target.Name + " has " + (target.Health() - damage) + " health left!</color>");
            chat.transform.Find("ScrollView").gameObject.transform.Find("Viewport").gameObject.transform.Find("Content").gameObject.GetComponent<Text>().color = new Color(1, 1, 1);
            chat.transform.Find("ScrollView").gameObject.GetComponent<ScrollRect>().gameObject.transform.Find("Scrollbar Vertical").gameObject.GetComponent<Scrollbar>().value = 0.0f;
        }
        target.TakeDamage(damage, this);
    }

    public void Attack(Entity e)
    {
        if (e.location == location)
        {
            target = e;
            attacking = true;
            Invoke("DealDamage", atkSpeed);
        } else
        {
            LeaveCombat();
            e.LeaveCombat();
        }
    }

    public void SetLocation(Room newLoc)
    {
        if (location != null)
			location.transform.position = location.startLoc;
        newLoc.transform.position = new Vector2(0, 2);
        location = newLoc;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Exit")
        {
            exiting = !exiting;
            if (exiting)
            {
				location.RemoveEntity (this);
                SetLocation(other.gameObject.GetComponent<Exit>().goesTo.GetParent().GetComponent<Room>());
                location.AddEntity(this);
                transform.position = other.gameObject.GetComponent<Exit>().goesTo.transform.position;
                GetComponent<Movement>().newPos = location.transform;
            }
        }
    }
}
