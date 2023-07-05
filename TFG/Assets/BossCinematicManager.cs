using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class BossCinematicManager : MonoBehaviour
{
    const float ANIM_LEFTOVER_TIME = 0.7f;
    const float WAIT_BEFORE_FIGHT = 1f;

    [SerializeField] AnimationClip camAnimClip;
    [SerializeField] VideoPlayer videoPlayer;
    [SerializeField] GameObject animCam, originalCam, videoImg;
    [SerializeField] Image fader;
    [SerializeField] BatBossEnemy bossScript;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GetComponent<Collider>().enabled = false;
            StartCoroutine(BossAnimation_Cor(other.GetComponent<PlayerAttack>()));
        }
    }


    IEnumerator BossAnimation_Cor(PlayerAttack _player)
    {
        AudioManager.instance.StopMusic(AudioManager.instance.playMusicInstance);
        _player.canAttack = false;
        bossScript.canAttack = false;
        fader.gameObject.SetActive(true);

        yield return LerpImageColor_Cor(fader, Color.clear, Color.black);
        AudioManager.instance.PlayMusic(FMODEvents.instance.bossMusic);
        animCam.SetActive(true);
        originalCam.SetActive(false);
        GameObject playerModel = _player.transform.Find("Modelo Raton").gameObject;
        playerModel.SetActive(false);
        GameObject playerCanvas = _player.transform.parent.Find("PlayerCanvas").gameObject;
        playerCanvas.SetActive(false);
        yield return LerpImageColor_Cor(fader, Color.black, Color.clear);

        yield return new WaitForSeconds(camAnimClip.length - ANIM_LEFTOVER_TIME);

        yield return LerpImageColor_Cor(fader, Color.clear, Color.black);
        animCam.SetActive(false);
        originalCam.SetActive(true);
        videoPlayer.gameObject.SetActive(true);
        videoImg.SetActive(true);
        yield return LerpImageColor_Cor(fader, Color.black, Color.clear);

        yield return new WaitForSeconds((float)videoPlayer.clip.length);

        yield return LerpImageColor_Cor(fader, Color.clear, Color.black);
        videoPlayer.gameObject.SetActive(false);
        videoImg.SetActive(false);
        playerModel.SetActive(true);
        playerCanvas.SetActive(true);
        yield return LerpImageColor_Cor(fader, Color.black, Color.clear);

        fader.gameObject.SetActive(false);
        yield return new WaitForSeconds(WAIT_BEFORE_FIGHT);

        _player.canAttack = true;
        bossScript.canAttack = true;
    }

    IEnumerator LerpImageColor_Cor(Image _image, Color _initColor, Color _targetColor, float _lerpTime = 0.4f)
    {
        _image.color = _initColor;
        float timer = 0f;
        while (timer < _lerpTime)
        {
            yield return null;
            timer += Time.unscaledDeltaTime;
            _image.color = Color.Lerp(_initColor, _targetColor, timer / _lerpTime);
        }
    }

}
