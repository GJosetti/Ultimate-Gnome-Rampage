using UnityEngine;

public static class GameManager
{
    static public  int room = 0;

    static public Cameras camera;

    static public bool inCombat;

    public static void ResetState()
    {
        room = 0;
        inCombat = false;
        // reseta qualquer outro campo estático aqui
    }

}
