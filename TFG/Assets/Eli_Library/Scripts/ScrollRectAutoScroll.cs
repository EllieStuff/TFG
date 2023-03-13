using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class ScrollRectAutoScroll : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float scrollSpeed = 10f;
    private bool mouseOver = false;

    private List<Selectable> selectables = new List<Selectable>();
    private ScrollRect scrollRect;

    private Vector2 nextScrollPosition = Vector2.up;

    void OnEnable()
    {
        if (scrollRect)
        {
            scrollRect.content.GetComponentsInChildren(selectables);
        }
    }

    void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
    }

    void Start()
    {
        if (scrollRect)
        {
            scrollRect.content.GetComponentsInChildren(selectables);
        }
        ScrollToSelected(true);
    }


    void Update()
    {
        InputScroll();

        if (!mouseOver)
        {
            scrollRect.normalizedPosition = Vector2.Lerp(scrollRect.normalizedPosition, nextScrollPosition, scrollSpeed * Time.unscaledDeltaTime);
        }
        else
        {
            nextScrollPosition = scrollRect.normalizedPosition;
        }
    }


    void InputScroll()
    {
        if (selectables.Count > 0)
        {
            if (Input.GetAxis("Vertical") != 0.0f || Input.GetAxis("Horizontal") != 0.0f || Input.GetButtonDown("Horizontal")
                || Input.GetButtonDown("Vertical") || Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
            {
                ScrollToSelected(false);
            }
        }
    }

    void ScrollToSelected(bool quickScroll)
    {
        int selectedIndex = -1;
        Selectable selectedElement = EventSystem.current.currentSelectedGameObject ? EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>() : null;

        if (selectedElement)
        {
            selectedIndex = selectables.IndexOf(selectedElement);
        }
        if (selectedIndex > -1)
        {
            if (quickScroll)
            {
                scrollRect.normalizedPosition = new Vector2(0, 1 - (selectedIndex / ((float)selectables.Count - 1)));
                nextScrollPosition = scrollRect.normalizedPosition;
            }
            else
            {
                nextScrollPosition = new Vector2(0, 1 - (selectedIndex / ((float)selectables.Count - 1)));
            }
        }
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseOver = false;
        ScrollToSelected(false);
    }

}