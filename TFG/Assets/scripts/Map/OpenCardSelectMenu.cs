using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenCardSelectMenu : MonoBehaviour
{
    [SerializeField] GameObject pasiveSkillUI;
    [SerializeField] ParticleSystem infiniteParticle;
    bool opened = false;
    bool destroying = false;

    const float ANIMATION_SPEED = 2f;

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

        DestroyCircleAnimation();
    }

    void StopParticles()
    {
        GetComponent<ParticleSystem>().Stop();

        foreach (Transform child in transform)
            child.GetComponent<ParticleSystem>().Stop();

        infiniteParticle.Play();
    }

    void DestroyCircleAnimation()
    {
        if (opened && !destroying && !pasiveSkillUI.activeSelf)
        {
            destroying = true;
            StopParticles();
        }

        if (destroying && transform.localScale.x > 0)
        {
            float deltatime = Time.deltaTime;
            transform.localScale -= new Vector3(deltatime, deltatime, deltatime) * ANIMATION_SPEED;
        }
    }

}
