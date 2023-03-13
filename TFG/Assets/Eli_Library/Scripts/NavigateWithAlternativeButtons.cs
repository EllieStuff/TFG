using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NavigateWithAlternativeButtons : MonoBehaviour
{
    enum Direction { RIGHT, LEFT, UP, DOWN }

    [SerializeField] string
        moveRightBttn = "",
        moveLeftBttn = "",
        moveUpBttn = "",
        moveDownBttn = "";
    [SerializeField] float
        initDelay = 0.5f,
        continuePressedDelay = 0.2f;

    Menu_Manager menuManager;
    Selectable currOption;
    bool
        movingRight = false,
        movingLeft = false,
        movingUp = false,
        movingDown = false;

    // Start is called before the first frame update
    void Start()
    {
        menuManager = GetComponent<Menu_Manager>();
        currOption = menuManager.startSelectedOption.GetComponent<Selectable>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckButtonsDown();
        CheckButtonsUp();

    }

    void CheckButtonsDown()
    {
        if (moveRightBttn != "" && Input.GetButtonDown(moveRightBttn))
        {
            if (currOption.navigation.selectOnRight == null) return;
            movingRight = true;
            currOption = currOption.navigation.selectOnRight;
            menuManager.SetCurrentEventSystemSelection(currOption.gameObject);
            StartCoroutine(MoveSelectedButton(Direction.RIGHT, initDelay));
        }
        if (moveLeftBttn != "" && Input.GetButtonDown(moveLeftBttn))
        {
            if (currOption.navigation.selectOnLeft == null) return;
            movingLeft = true;
            currOption = currOption.navigation.selectOnLeft;
            menuManager.SetCurrentEventSystemSelection(currOption.gameObject);
            StartCoroutine(MoveSelectedButton(Direction.LEFT, initDelay));
        }
        if (moveUpBttn != "" && Input.GetButtonDown(moveUpBttn))
        {
            if (currOption.navigation.selectOnUp == null) return;
            movingUp = true;
            currOption = currOption.navigation.selectOnUp;
            menuManager.SetCurrentEventSystemSelection(currOption.gameObject);
            StartCoroutine(MoveSelectedButton(Direction.UP, initDelay));
        }
        else if (moveDownBttn != "" && Input.GetButtonDown(moveDownBttn))
        {
            if (currOption.navigation.selectOnDown == null) return;
            movingDown = true;
            currOption = currOption.navigation.selectOnDown;
            menuManager.SetCurrentEventSystemSelection(currOption.gameObject);
            StartCoroutine(MoveSelectedButton(Direction.DOWN, initDelay));
        }

    }
    
    void CheckButtonsUp()
    {
        if (moveRightBttn != "" && Input.GetButtonUp(moveRightBttn))
        {
            movingRight = false;
        }
        if (moveLeftBttn != "" && Input.GetButtonUp(moveLeftBttn))
        {
            movingLeft = false;
        }
        if (moveUpBttn != "" && Input.GetButtonUp(moveUpBttn))
        {
            movingUp = false;
        }
        if (moveDownBttn != "" && Input.GetButtonUp(moveDownBttn))
        {
            movingDown = false;
        }

    }


    IEnumerator MoveSelectedButton(Direction _dir, float _delay = 0.5f)
    {
        yield return new WaitForSeconds(_delay);

        switch (_dir)
        {
            case Direction.RIGHT:
                if (!movingRight || currOption.navigation.selectOnRight == null) yield break;
                currOption = currOption.navigation.selectOnRight;
                break;

            case Direction.LEFT:
                if (!movingLeft || currOption.navigation.selectOnLeft == null) yield break;
                currOption = currOption.navigation.selectOnLeft;
                break;

            case Direction.UP:
                if (!movingUp || currOption.navigation.selectOnUp == null) yield break;
                currOption = currOption.navigation.selectOnUp;
                break;

            case Direction.DOWN:
                if (!movingDown || currOption.navigation.selectOnDown == null) yield break;
                currOption = currOption.navigation.selectOnDown;
                break;


            default:
                break;
        }

        menuManager.SetCurrentEventSystemSelection(currOption.gameObject);
        StartCoroutine(MoveSelectedButton(_dir, continuePressedDelay));
    }

}
