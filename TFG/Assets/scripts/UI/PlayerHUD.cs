using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    LifeSystem playerLifeStatus;
    [SerializeField] private Slider lifeSlider;
    Animation shakeLifeBarAnim;

    private void Start()
    {
        shakeLifeBarAnim = GetComponent<Animation>();
        GameObject player = GameObject.Find("Player");
        playerLifeStatus = player.GetComponent<LifeSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        lifeSlider.value = playerLifeStatus.currLife / playerLifeStatus.maxLife;
    }

    public void ShakeLifeBar()
    {
        shakeLifeBarAnim.Play();
    }
}
