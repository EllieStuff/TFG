using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenCardSelectMenu : MonoBehaviour
{
    [SerializeField] GameObject pasiveSkillUI;
    bool opened = false;
    bool destroying = false;

    const float ANIMATION_SPEED = 0.5f;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag.Equals("Player") && !opened)
        {
            pasiveSkillUI.SetActive(true);
            opened = true;
        }
    }

    private void Update()
    {
        if(pasiveSkillUI == null)
            pasiveSkillUI = GameObject.FindGameObjectWithTag("EnemyManager").transform.GetChild(0).GetComponent<RoomEnemyManager>().elementChoose;
        
        if(opened && !destroying && !pasiveSkillUI.activeSelf)
        {
            StartCoroutine(CloseCircleCorroutine());
            destroying = true;
        }
    }

    IEnumerator CloseCircleCorroutine()
    {
        while(transform.localScale.x > 1)
        {
            float deltatime = Time.deltaTime;
            transform.localScale -= new Vector3(deltatime, deltatime, deltatime) * ANIMATION_SPEED;
        }

        Destroy(gameObject);

        yield return null;
    }
}
