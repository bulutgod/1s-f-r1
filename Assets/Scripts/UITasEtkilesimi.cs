using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UITasEtkilesimi : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Tas temsilEttigiTas;
    [HideInInspector]
    public UIManager uiManagerReferansi;
    [HideInInspector]
    public Transform orijinalPanel; // Taþýn ait olduðu el paneli (geri dönmek için)

    private RectTransform _rectTransform;
    private Canvas _anaCanvas;
    private CanvasGroup _canvasGroup;
    private Vector3 _orijinalLocalPozisyon;
    // private Transform _suruklemeUstObjesi; // Canvas'ý _anaCanvas.transform olarak kullanacaðýz

    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _anaCanvas = GetComponentInParent<Canvas>();
        _canvasGroup = GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
        {
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    public void Ayarla(Tas tasVerisi, UIManager uiManager, Transform aitOlduguPanel)
    {
        temsilEttigiTas = tasVerisi;
        uiManagerReferansi = uiManager;
        orijinalPanel = aitOlduguPanel;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (temsilEttigiTas == null || uiManagerReferansi == null || uiManagerReferansi.gameManager == null) return;

        // Sadece sýrasý gelen oyuncu kendi taþlarýný sürükleyebilsin
        Oyuncu aktifOyuncu = uiManagerReferansi.gameManager.oyuncular[uiManagerReferansi.gameManager.AktifOyuncuIndex];
        // Bu UI taþýnýn oyuncusu aktif oyuncu mu? (Þimdilik hep Oyuncu 1'in eli olduðu için bu kontrol basitleþtirilebilir)
        // Daha genel bir yapý için, bu UITasEtkilesimi'nin hangi oyuncuya ait olduðunu bilmesi gerekir.
        // Þimdilik, Oyuncu 1'in sýrasýysa sürüklemeye izin verelim (aktifOyuncuIndex == 0).
        if (uiManagerReferansi.gameManager.AktifOyuncuIndex != 0) // Sadece Oyuncu 1'in sýrasýysa
        {
            // eventData.pointerDrag = null; // Sürüklemeyi engelle (bu doðrudan çalýþmayabilir)
            Debug.Log("Sýra sizde deðil, taþ sürüklenemez!");
            return; // Sürüklemeyi baþlatma
        }


        _orijinalLocalPozisyon = _rectTransform.localPosition;
        transform.SetParent(_anaCanvas.transform); // En üste çizilmesi ve layout'tan çýkmasý için
        transform.SetAsLastSibling();

        _canvasGroup.alpha = 0.7f;
        _canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (temsilEttigiTas == null || !_canvasGroup.blocksRaycasts == false) return; // Sadece sürükleme baþladýysa hareket et

        _rectTransform.anchoredPosition += eventData.delta / _anaCanvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (temsilEttigiTas == null || _canvasGroup.blocksRaycasts == true) // Sürükleme baþlamadýysa bir þey yapma
        {
            if (_canvasGroup.blocksRaycasts == true && temsilEttigiTas != null)
            {
                // Bu durum, sürükleme baþlamadan bittiyse (örn: sadece týklama)
                // veya sürükleme engellendiyse olabilir.
            }
            else if (temsilEttigiTas == null) return;
        }


        _canvasGroup.alpha = 1f;
        _canvasGroup.blocksRaycasts = true;

        TasAtmaAlani birakmaAlani = (eventData.pointerEnter == null) ? null : eventData.pointerEnter.GetComponent<TasAtmaAlani>();

        if (birakmaAlani == null || !birakmaAlani.GecerliBirakmaAlaniMi(temsilEttigiTas, null)) // Oyuncu parametresi þimdilik null
        {
            // Geçerli bir alana býrakýlmadý, orijinal paneline ve pozisyonuna geri dön
            transform.SetParent(orijinalPanel);
            _rectTransform.localPosition = _orijinalLocalPozisyon;
            // Layout grubunun eli yeniden düzenlemesi için UIManager'dan eli tekrar çizdirmesi gerekebilir.
            // Veya sadece aktif oyuncunun elini tekrar çizdir:
            if (uiManagerReferansi != null && uiManagerReferansi.gameManager != null && uiManagerReferansi.gameManager.oyuncular.Count > uiManagerReferansi.gameManager.AktifOyuncuIndex)
            {
                if (uiManagerReferansi.gameManager.AktifOyuncuIndex == 0) // Þimdilik sadece Oyuncu 1'in UI'ýný güncelliyoruz
                    uiManagerReferansi.Oyuncu1EliniGoster(uiManagerReferansi.gameManager.oyuncular[uiManagerReferansi.gameManager.AktifOyuncuIndex]);
            }
        }
        // Eðer geçerli bir alana býrakýldýysa, TasAtmaAlani.OnDrop metodu zaten UIManager aracýlýðýyla GameManager'ý tetiklemiþ olacak
        // ve GameManager eli güncelleyip UIManager'a yeniden çizdirecek.
    }
}