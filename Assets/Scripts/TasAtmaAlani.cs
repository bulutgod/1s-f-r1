using UnityEngine;
using UnityEngine.EventSystems; 

public class TasAtmaAlani : MonoBehaviour, IDropHandler
{
    public UIManager uiManagerReferansi; 

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("--- TasAtmaAlani.OnDrop ÇAÐRILDI --- eventData.pointerDrag: " + (eventData.pointerDrag != null ? eventData.pointerDrag.name : "null"));
        if (eventData.pointerDrag != null)
        {
            UITasEtkilesimi suruklenenTasScripti = eventData.pointerDrag.GetComponent<UITasEtkilesimi>();
            if (suruklenenTasScripti != null && suruklenenTasScripti.uiManagerReferansi != null)
            {
                Debug.Log(suruklenenTasScripti.temsilEttigiTas.ToString() + " taþý AtmaAlani'na býrakýldý. UIManager'a bildiriliyor...");
                suruklenenTasScripti.uiManagerReferansi.OyuncuTasAttiYerineBirakti(suruklenenTasScripti);
            }
            else if (suruklenenTasScripti == null)
            {
                Debug.LogError("TasAtmaAlani.OnDrop: Sürüklenen objenin üzerinde UITasEtkilesimi script'i bulunamadý!");
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
        Debug.Log("--- TasAtmaAlani.OnDrop BÝTTÝ ---");
    }

    public bool GecerliBirakmaAlaniMi(Tas tas, Oyuncu oyuncu)
    {
        // Bu metot þu an UITasEtkilesimi.OnEndDrag içinde çaðrýlýyor.
        // Þimdilik her zaman true dönsün ki OnEndDrag'de taþý geri atmasýn (eðer OnDrop çalýþýyorsa).
        return true;
    }
}