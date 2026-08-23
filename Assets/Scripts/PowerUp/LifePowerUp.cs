using UnityEngine;

public class LifePowerUp : PowerUp
{
    public override void Interact(GameObject gameObject)
    {
        gameObject.GetComponent<PlayerHealth>().IncreaseHealth(1);  
        Destroy(this.gameObject);
    }

}
