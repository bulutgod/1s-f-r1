using UnityEngine;
using UnityEngine.EventSystems; 

public class TasAtmaAlani : MonoBehaviour, IDropHandler
{
    public UIManager uiManagerReferansi; 

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("--- TasAtmaAlani.OnDrop �A�RILDI --- eventData.pointerDrag: " + (eventData.pointerDrag != null ? eventData.pointerDrag.name : "null"));
        if (eventData.pointerDrag != null)
        {
            UITasEtkilesimi suruklenenTasScripti = eventData.pointerDrag.GetComponent<UITasEtkilesimi>();
            if (suruklenenTasScripti != null && suruklenenTasScripti.uiManagerReferansi != null)
            {
                Debug.Log(suruklenenTasScripti.temsilEttigiTas.ToString() + " ta�� AtmaAlani'na b�rak�ld�. UIManager'a bildiriliyor...");
                suruklenenTasScripti.uiManagerReferansi.OyuncuTasAttiYerineBirakti(suruklenenTasScripti);
            }
            else if (suruklenenTasScripti == null)
            {
                Debug.LogError("TasAtmaAlani.OnDrop: S�r�klenen objenin �zerinde UITasEtkilesimi script'i bulunamad�!");
            }
            else if (suruklenenTasScripti.uiManagerReferansi == null)
            {
                Debug.LogError("TasAtmaAlani.OnDrop: suruklenenTasScripti.uiManagerReferansi null!");
            }
        }
        else
        {
            Debug.LogWarning("TasAtmaAlani.OnDrop: eventData.pointerDrag null geldi!");
        }
        Debug.Log("--- TasAtmaAlani.OnDrop B�TT� ---");
    }

    public bool GecerliBirakmaAlaniMi(Tas tas, Oyuncu oyuncu)
    {
        // Bu metot �u an UITasEtkilesimi.OnEndDrag i�inde �a�r�l�yor.
        // �imdilik her zaman true d�ns�n ki OnEndDrag'de ta�� geri atmas�n (e�er OnDrop �al���yorsa).
        return true;
    }
}