using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    PlayerLifeSystem playerLifeStatus;
    [SerializeField] private Slider lifeSlider;

    private void Start()
    {
        GameObject player = GameObject.Find("Player");
        playerLifeStatus = player.GetComponent<PlayerLifeSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        lifeSlider.value = playerLifeStatus.life / playerLifeStatus.playerMaxLife;
    }
}
