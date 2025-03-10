using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MenuEventHandler : MonoBehaviour
{
    [Header("References")]
    public List<Selectable> Selectables = new List<Selectable>();
    [SerializeField] protected Selectable _firstSelected;
    protected Selectable _lastSelected;

    [Header("Controls")]
    [SerializeField] protected InputActionReference _navigateReference;

    public virtual void Awake()
    {
        foreach(var s in Selectables)
        {
            AddSelectionListener(s);
        }
    }

    public virtual void OnEnable()
    {
        _navigateReference.action.performed += OnNavigate;
        EventSystem.current.SetSelectedGameObject(_firstSelected.gameObject);
    }

    public virtual void OnDisable()
    {
        _navigateReference.action.performed -= OnNavigate;
    }

    protected virtual void AddSelectionListener(Selectable selectable)
    {
        // add listener
        EventTrigger trigger = selectable.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = selectable.gameObject.AddComponent<EventTrigger>();
        }

        // add select event
        EventTrigger.Entry SelectEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.Select
        };
        SelectEntry.callback.AddListener(OnSelect);
        trigger.triggers.Add(SelectEntry);

        // add Deselect event
        EventTrigger.Entry DeselectEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.Deselect
        };
        DeselectEntry.callback.AddListener(OnDeselect);
        trigger.triggers.Add(DeselectEntry);

        // select onPointerEnter events
        EventTrigger.Entry PointerEnter = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };
        PointerEnter.callback.AddListener(OnPointerEnter);
        trigger.triggers.Add(PointerEnter);

        // select onPointerExit events
        EventTrigger.Entry PointerExit = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerExit
        };
        PointerExit.callback.AddListener(OnPointerExit);
        trigger.triggers.Add(PointerExit);
    }

    public void OnSelect(BaseEventData eventData)
    {
        // handle animations here
    }

    public void OnDeselect(BaseEventData eventData)
    {
        _lastSelected = eventData.selectedObject.GetComponent<Selectable>();
        // handle animations here
    }

    public void OnPointerEnter(BaseEventData eventData)
    {
        // Set things to selected
        PointerEventData pointerEventData = eventData as PointerEventData;
        if (pointerEventData != null)
        {
            // override raycast from text on buttons
            Selectable sel = pointerEventData.pointerEnter.GetComponentInParent<Selectable>();
            if (sel == null)
            {
                sel = pointerEventData.pointerEnter.GetComponentInChildren<Selectable>();
            }
            pointerEventData.selectedObject = sel.gameObject;
        }
    }

    public void OnPointerExit(BaseEventData eventData)
    {
        // Unselect things
        PointerEventData pointerEventData = eventData as PointerEventData;
        if (pointerEventData != null)
        {
            pointerEventData.selectedObject = null;
        }

    }

    // handle navigation with keyboard or other methods
    protected virtual void OnNavigate(InputAction.CallbackContext context)
    {
        if (EventSystem.current.currentSelectedGameObject == null && _lastSelected != null)
        {
            EventSystem.current.SetSelectedGameObject(_lastSelected.gameObject);
        }
    }
}
