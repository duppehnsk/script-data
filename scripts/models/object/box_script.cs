using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class box_script : MonoBehaviour
{
    
    public GameObject normal;
    public GameObject destroy;
    public int healthbox;
    public bool _random;
    public GameObject[] RandomDrop;
    public List<BoxDrop> _specialDrop = new List<BoxDrop>();
    private int r;
    private int count;


    void FixedUpdate()
    {
        if(healthbox <= 0)
        {         
            Instantiate(destroy, new Vector3(normal.transform.position.x, normal.transform.position.y, normal.transform.position.z), Quaternion.identity);
            GetDrop();
            Destroy(normal);
        }
    }

    void GetDrop()
    {
        if(_random)
        {
            count = Random.Range(0,3);
            for(int i = 0; i < count; i++)
            {
            r = Random.Range(0,RandomDrop.Length);
            Instantiate(RandomDrop[r], new Vector3(normal.transform.position.x, normal.transform.position.y+0.2f, normal.transform.position.z), Quaternion.identity);
            }
        }
        else
        {
            for(int i=0; i<_specialDrop.Count;i++)
            {
                for(int j=0; j<_specialDrop[i]._count;j++)
                {
                Instantiate(_specialDrop[i].ObjectItem, new Vector3(normal.transform.position.x, normal.transform.position.y+0.2f, normal.transform.position.z), Quaternion.identity);   
                }
            }
            
        }
    }
}
[System.Serializable]
public class BoxDrop
{
    [Header("Предмет")]
    public GameObject ObjectItem;
    [Header("Количество")]
    public int _count;
}
