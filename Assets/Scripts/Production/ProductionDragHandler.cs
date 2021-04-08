using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ProductionDragHandler : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    GameObject icon;


    GameObject copyIcon;
    ProductionUI productionUI;


    private void Start()
    {
        productionUI = ProductionUI.instance;
        icon = GetComponent<ProductionSlot>().image.gameObject;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(GetComponent<ProductionSlot>().item != null)
        {
            StationController station = productionUI.station;
            ProductionSlotType slotType = GetComponent<ProductionSlot>().slotType;
            switch (slotType)
            {
                case ProductionSlotType.Input:
                    station.draggingInputIndex = transform.GetSiblingIndex();
                    break;
                case ProductionSlotType.Output:
                    station.draggingOutputIndex = transform.GetSiblingIndex();
                    break;
                default:
                    break;
            }
            productionUI.copyIcon = Instantiate(icon, transform.parent.parent);
            copyIcon = productionUI.copyIcon;
            copyIcon.GetComponent<Image>().color = new Color(1, 1, 1, 0.7f);
            copyIcon.GetComponent<Image>().raycastTarget = false;
            copyIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(icon.GetComponent<RectTransform>().rect.width, icon.GetComponent<RectTransform>().rect.height);
            copyIcon.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
            copyIcon.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
        }
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(productionUI.station.draggingInputIndex!=-1 || productionUI.station.draggingOutputIndex != -1)
        {
            copyIcon.transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (productionUI.station.draggingInputIndex!=-1 || productionUI.station.draggingOutputIndex != -1)
        {
            productionUI.station.draggingInputIndex = -1;
            productionUI.station.draggingOutputIndex = -1;
            Destroy(productionUI.copyIcon);
            productionUI.copyIcon = null;
            copyIcon = null;
        }
    }

}


