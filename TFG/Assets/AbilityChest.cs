using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityChest : MonoBehaviour
{
    [SerializeField] PassiveSkill_Base.SkillType chestAbility;
    [SerializeField] Transform chestTop;
    bool interacting = false, canInteract = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(canInteract) interacting = Input.GetKey(KeyCode.E);
    }


    private void OnCollisionStay(Collision col)
    {
        if (col.transform.CompareTag("Player") && interacting)
        {
            canInteract = interacting = false;
            //PassiveSkills_Manager skillsManager = col.transform.GetComponent<PassiveSkills_Manager>();
            //skillsManager.AddSkill(PassiveSkills_Manager.GetSkillByType(chestAbility));
            StartCoroutine(OpenChest(col.transform));
        }
    }


    IEnumerator OpenChest(Transform _playerRef)
    {
        float timeLerpSpeed = 0.2f, chestLerpSpeed = 1f;
        float timer = 0, maxTime = timeLerpSpeed;
        while (timer < maxTime)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Lerp(1, 0, timer / maxTime);
        }
        yield return new WaitForSecondsRealtime(0.1f);

        timer = 0; maxTime = chestLerpSpeed;
        float rotSpeed = -40;
        Time.timeScale = 0;
        while(timer < maxTime)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.unscaledDeltaTime;
            chestTop.Rotate(transform.forward, rotSpeed * Time.unscaledDeltaTime);
        }
        yield return new WaitForSecondsRealtime(0.1f);

        timer = 0; maxTime = timeLerpSpeed;
        while (timer < maxTime)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Lerp(0, 1, timer / maxTime);
        }

        PassiveSkills_Manager skillsManager = _playerRef.GetComponent<PassiveSkills_Manager>();
        skillsManager.AddSkill(PassiveSkills_Manager.GetSkillByType(chestAbility));
    }

}
