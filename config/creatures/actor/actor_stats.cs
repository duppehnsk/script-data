using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class actor_stats : MonoBehaviour
{
    [Header("ActorObject")]
    public GameObject Actor;
    [Header("Stats")]
    public string _act_name;
    public Sprite ui_act_img;
    //public GameObject ui_ic_god;
    //public GameObject ui_ic_food;

    //public string ui_group;
    
    public float Health;
    public float RegenerateHealth;
    public float Stalmina;
    public float hungry;

    public float Radiation;
    public float Money = 80f;
    public float _currentMassa = 10f;
    public float MaxItemMassa = 50f;
    private bool _playonesound = true;
    //public bool inTir;
    //public float satiety = 0f; //скорость уменьшения сытости со временем

    //[Header("Cheats")]
    //public int[] achievements = new int[4];

    [Header("Cheats")]
    public bool GodMode;
    // Start is called before the first frame update
    void Start()
    {
        if (GodMode == false)
        {
            //ui_ic_god.SetActive(false);
        }
    }

    // Update is called once per frame

    void FixedUpdate()
    {
        Cheats();
        Regeneraion();
        RadDamage();
        StaminaControl();
    }

    void Cheats()
    {
        if (GodMode == true)
        {
            Health = 99999f;
        }
    }
    void Regeneraion()
    {
        if(Radiation < 10)
        {
            if (Health < 100)
            {
                Health += RegenerateHealth;
            }
        }
        
        if (Radiation > 0)
        {
            Radiation -=0.001f;
        }
    }

    void RadDamage()
    {
        if(Radiation > 0)
        {
            Health-=Radiation/5000f; 
        }
    }

    void StaminaControl()
    {
        if(Actor.GetComponent<actor_controller>().CanWalk == true)
        {
            if(Actor.GetComponent<actor_controller>().Staying == false)
                {
                     if(Actor.GetComponent<actor_controller>().m_IsWalking == false)
                        {
                             Stalmina -= ((_currentMassa/MaxItemMassa));
                         }
                      Stalmina -= (_currentMassa/MaxItemMassa * 0.3f);
                }
                 
        }

    if(Actor.GetComponent<actor_controller>().m_IsWalking == false && Stalmina < 20)
    {
        Actor.GetComponent<actor_controller>().m_IsWalking = true;
    }

        if(Actor.GetComponent<actor_controller>().Staying == true && Stalmina < 100)
                {
                   Stalmina +=0.1f;
                }
        if(Stalmina >100)
        {
            Stalmina = 100f;
        }
        if(Stalmina < 0)
        {
            Stalmina = 0f;
            Actor.GetComponent<actor_controller>().CanWalk = false;
        }
        if(Stalmina > 15)
        {
            _playonesound = true;
            Actor.GetComponent<actor_controller>().CanWalk = true;
        }
        if(Stalmina < 10)
        {
            if(_playonesound == true)
            {
                _playonesound = false;
            }
            
        }
    }
}
