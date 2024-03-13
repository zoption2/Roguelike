using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageProvider 
{
    public void DoDamage(GameObject victim);
}
public class DamageProvider : MonoBehaviour, IDamageProvider
{
    private void OnCollisionEnter(Collision collision)
    {
        DoDamage(collision.gameObject);
    }
    public void DoDamage(GameObject victim)
    {
        int damage = 0;
        throw new System.NotImplementedException();
    }
}
