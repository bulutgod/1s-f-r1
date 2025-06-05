using UnityEngine;
using UnityEngine.EventSystems; 

public class TasAtmaAlani : MonoBehaviour, IDropHandler
{
    public UIManager uiManagerReferansi; 

    public void OnDrop(PointerEventData eventData)
    {
        
        if (eventData.pointerDrag != null) 
        {
            UITasEtkilesimi suruklenenTasScripti = eventData.pointerDrag.GetComponent<UITasEtkilesimi>();
            if (suruklenenTasScripti != null && uiManagerReferansi != null)
            {
                
                uiManagerReferansi.OyuncuTasAtti(suruklenenTasScripti.temsilEttigiTas);

                
            }
        }
    }
}