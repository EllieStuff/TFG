using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasModeManager : MonoBehaviour
{
    enum SwitchMode { OVERLAY, CAMERA }
    [SerializeField] Canvas miscCanvas;
    [SerializeField] GameObject radialMenu;
    [SerializeField] GameObject UIShortcutsKeys;
    [SerializeField] GameObject cardGrid;
    [SerializeField] GameObject gameOverMenu;
    [SerializeField] GameObject pauseMenu;

    SwitchMode mode = SwitchMode.CAMERA;

    float timer = 1;

    private void Update()
    {
        if (timer > 0)
            timer -= Time.deltaTime;
        else
        {
            if ((pauseMenu.activeSelf || gameOverMenu.activeSelf) && !mode.Equals(SwitchMode.OVERLAY))
                SwitchUI(SwitchMode.OVERLAY);
            if (!pauseMenu.activeSelf && !gameOverMenu.activeSelf && !mode.Equals(SwitchMode.CAMERA))
                SwitchUI(SwitchMode.CAMERA);
        }
    }

    void SwitchUI(SwitchMode _mode)
    {
        mode = _mode;

        if (_mode.Equals(SwitchMode.OVERLAY))
        {
            radialMenu.SetActive(false);
            UIShortcutsKeys.SetActive(false);
            cardGrid.SetActive(false);
        }
        else
        {
            radialMenu.SetActive(true);
            UIShortcutsKeys.SetActive(true);
            cardGrid.SetActive(true);
        }

        if (_mode.Equals(SwitchMode.OVERLAY))
            miscCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        if (_mode.Equals(SwitchMode.CAMERA))
            miscCanvas.renderMode = RenderMode.ScreenSpaceCamera;
    }
}
