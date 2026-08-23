using UnityEngine;

public class FastSpinPowerUp : PowerUp
{
    public override void Interact(GameObject gameObject)
    {
        PlayerController.Instance.SetFastSpin(true);
        PlayerController.Instance.ResetFastSpinTimer();
        Destroy(this.gameObject);

    }

    private void Update()
    {
       
    }

}
