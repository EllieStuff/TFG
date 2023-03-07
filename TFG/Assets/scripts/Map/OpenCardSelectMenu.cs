using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenCardSelectMenu : MonoBehaviour
{
    GameObject pasiveSkillUI;
    bool opened = false;

    private void Start()
    {
        pasiveSkillUI = GameObject.Find("Enemies").GetComponentInChildren<RoomEnemyManager>().elementChoose;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag.Equals("Player") && !opened)
        {
            pasiveSkillUI.SetActive(true);
            opened = true;
        }
    }
}
