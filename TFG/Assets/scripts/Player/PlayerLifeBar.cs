using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLifeBar : MonoBehaviour
{
    LifeSystem playerLifeStatus;
    Slider lifeSlider;
    Animation shakeLifeBarAnim;


    // Start is called before the first frame update
    void Start()
    {
        playerLifeStatus = GameObject.FindGameObjectWithTag("Player").GetComponent<LifeSystem>();
        lifeSlider = GetComponent<Slider>();
        shakeLifeBarAnim = GetComponent<Animation>();
    }

    private void Update()
    {
        lifeSlider.value = playerLifeStatus.CurrLife / playerLifeStatus.MaxLife;
    }

    public void Damage()
    {
        shakeLifeBarAnim.Play();
    }

}
