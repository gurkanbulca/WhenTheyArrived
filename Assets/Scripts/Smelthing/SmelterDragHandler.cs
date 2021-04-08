using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SmelterDragHandler : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    GameObject icon;
    
    GameObject copyIcon;

    SmelterUI smelthingUI;

    private void Start()
    {
        smelthingUI = SmelterUI.instance;
        icon = GetComponent<SmelterSlot>().image.gameObject;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (GetComponent<SmelterSlot>().item != null)
        {
            SmelterController smelter = smelthingUI.smelter;
            SmelterSlotType slotType = GetComponent<SmelterSlot>().slotType;
            switch (slotType)
            {
                case SmelterSlotType.Input:
                    smelter.draggingInputIndex = transform.GetSiblingIndex();
                    break;
                case SmelterSlotType.Output:
                    smelter.draggingOutputIndex = transform.GetSiblingIndex();
                    break;
                case SmelterSlotType.Fuel:
                    smelter.draggingFuelIndex = transform.GetSiblingIndex();
                    break;
                default:
                    break;
            }
            smelthingUI.copyIcon = Instantiate(icon, transform.parent.parent);
            copyIcon = smelthingUI.copyIcon;
            copyIcon.GetComponent<Image>().color = new Color(1, 1, 1, 0.7f);
            copyIcon.GetComponent<Image>().raycastTarget = false;
            copyIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(icon.GetComponent<RectTransform>().rect.width, icon.GetComponent<RectTransform>().rect.height);
            copyIcon.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
            copyIcon.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (smelthingUI.smelter.draggingInputIndex != -1 
            || smelthingUI.smelter.draggingOutputIndex != -1
            || smelthingUI.smelter.draggingFuelIndex != -1)
        {
            copyIcon.transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        print("test");
        smelthingUI.smelter.draggingInputIndex = -1;
        smelthingUI.smelter.draggingOutputIndex = -1;
        smelthingUI.smelter.draggingFuelIndex = -1;
        Destroy(smelthingUI.copyIcon);
        smelthingUI.copyIcon = null;
        copyIcon = null;
    }


}
