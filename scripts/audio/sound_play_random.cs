using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sound_play_random : MonoBehaviour
{
public AudioSource _as;
public float minPing;
public float maxPing;
public float cur_ping;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(PlayRandom());
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void GetPause()
    {
        cur_ping = Random.Range(minPing,maxPing);
    }
    IEnumerator PlayRandom()
    {
        while (true)
        {
            GetPause();
            yield return new WaitForSeconds(cur_ping/1000);
            _as.PlayOneShot(_as.clip);
        
        }
    }
}
