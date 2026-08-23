using UnityEngine;

public class InvenciblePowerUp : PowerUp
{
    public override void Interact(GameObject gameObject)
    {
        PlayerController.Instance.SetInvencible(true);
        PlayerController.Instance.ResetInvencibleTimer();
        Destroy(this.gameObject);

    }

}
