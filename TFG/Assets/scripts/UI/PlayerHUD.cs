using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    LifeSystem playerLifeStatus;
    PlayerDodge playerDodge;

    [SerializeField] private Slider lifeSlider;
    [SerializeField] private Slider dashSlider;
    Animation shakeLifeBarAnim;

    [SerializeField] bool lifeHUD;
    [SerializeField] bool dashHUD;

    private void Start()
    {
        shakeLifeBarAnim = GetComponent<Animation>();

        GameObject player = GameObject.Find("Player");

        if(lifeHUD)
            playerLifeStatus = player.GetComponent<LifeSystem>();
        if (dashHUD)
            playerDodge = player.GetComponent<PlayerDodge>();
    }

    // Update is called once per frame
    void Update()
    {
        if (lifeHUD)
            lifeSlider.value = playerLifeStatus.currLife / playerLifeStatus.maxLife;
        if (dashHUD)
            dashSlider.value = 1 - (playerDodge.dodgeRechargeTimer / playerDodge.dodgeRechargeDelay);
    }

    public void ShakeBar()
    {
        shakeLifeBarAnim.Play();
    }
}
