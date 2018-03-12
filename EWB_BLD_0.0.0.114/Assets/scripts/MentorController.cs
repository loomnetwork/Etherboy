using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MentorController : MonoBehaviour {

    public GameObject talisman;
    public GameObject sword;

    /*
     * COULD ALL LIVE IN A TOWN SCENE CLASS.  SETS THE MENTOR'S ART ASSETS BASED ON EVENTS
     */
    public void EquipTalisman()
    {
        talisman.SetActive(true);
    }

    public void GiveTalisman()
    {
        talisman.SetActive(false);
    }

    public void EquipSword()
    {
        sword.SetActive(true);
    }

    public void GiveSword()
    {
        sword.SetActive(false);
    }
}
