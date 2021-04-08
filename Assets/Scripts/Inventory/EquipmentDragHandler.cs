using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EquipmentDragHandler : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public GameObject icon;
    EquipmentManager equipmentManager;
    GameObject copyIcon;

    private void Start()
    {
        equipmentManager = EquipmentManager.instance;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (GetComponent<EquipmentSlot>() != null)
        {
            equipmentManager.draggingEquipmentIndex = (int)GetComponent<EquipmentSlot>().slotType;
        }
        else
        {
            equipmentManager.draggingConsumable = GetComponent<ConsumableSlot>().GetItem();
        }
        
        copyIcon = Instantiate(icon, transform.parent.parent.parent);
        copyIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(icon.GetComponent<RectTransform>().rect.width, icon.GetComponent<RectTransform>().rect.height);
        copyIcon.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
        copyIcon.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);

        copyIcon.GetComponent<Image>().color = new Color(1, 1, 1, 0.7f);
        copyIcon.GetComponent<Image>().raycastTarget = false;

    }

    public void OnDrag(PointerEventData eventData)
    {
        if(equipmentManager.draggingEquipmentIndex != -1 || equipmentManager.draggingConsumable != null)
        {
            copyIcon.transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (equipmentManager.draggingEquipmentIndex != -1)
        {
            equipmentManager.draggingEquipmentIndex = -1;
        }
        if(equipmentManager.draggingConsumable != null)
        {
            equipmentManager.draggingConsumable = null;
        }
        Destroy(copyIcon);


    }
}
