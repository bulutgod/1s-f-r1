using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UITasEtkilesimi : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Tas temsilEttigiTas;
    [HideInInspector]
    public UIManager uiManagerReferansi;
    [HideInInspector]
    public Transform orijinalPanel; // Ta��n ait oldu�u el paneli (geri d�nmek i�in)

    private RectTransform _rectTransform;
    private Canvas _anaCanvas;
    private CanvasGroup _canvasGroup;
    private Vector3 _orijinalLocalPozisyon;
    // private Transform _suruklemeUstObjesi; // Canvas'� _anaCanvas.transform olarak kullanaca��z

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

        // Sadece s�ras� gelen oyuncu kendi ta�lar�n� s�r�kleyebilsin
        Oyuncu aktifOyuncu = uiManagerReferansi.gameManager.oyuncular[uiManagerReferansi.gameManager.AktifOyuncuIndex];
        // Bu UI ta��n�n oyuncusu aktif oyuncu mu? (�imdilik hep Oyuncu 1'in eli oldu�u i�in bu kontrol basitle�tirilebilir)
        // Daha genel bir yap� i�in, bu UITasEtkilesimi'nin hangi oyuncuya ait oldu�unu bilmesi gerekir.
        // �imdilik, Oyuncu 1'in s�ras�ysa s�r�klemeye izin verelim (aktifOyuncuIndex == 0).
        if (uiManagerReferansi.gameManager.AktifOyuncuIndex != 0) // Sadece Oyuncu 1'in s�ras�ysa
        {
            // eventData.pointerDrag = null; // S�r�klemeyi engelle (bu do�rudan �al��mayabilir)
            Debug.Log("S�ra sizde de�il, ta� s�r�klenemez!");
            return; // S�r�klemeyi ba�latma
        }


        _orijinalLocalPozisyon = _rectTransform.localPosition;
        transform.SetParent(_anaCanvas.transform); // En �ste �izilmesi ve layout'tan ��kmas� i�in
        transform.SetAsLastSibling();

        _canvasGroup.alpha = 0.7f;
        _canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (temsilEttigiTas == null || !_canvasGroup.blocksRaycasts == false) return; // Sadece s�r�kleme ba�lad�ysa hareket et

        _rectTransform.anchoredPosition += eventData.delta / _anaCanvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (temsilEttigiTas == null || _canvasGroup.blocksRaycasts == true) // S�r�kleme ba�lamad�ysa bir �ey yapma
        {
            if (_canvasGroup.blocksRaycasts == true && temsilEttigiTas != null)
            {
                // Bu durum, s�r�kleme ba�lamadan bittiyse (�rn: sadece t�klama)
                // veya s�r�kleme engellendiyse olabilir.
            }
            else if (temsilEttigiTas == null) return;
        }


        _canvasGroup.alpha = 1f;
        _canvasGroup.blocksRaycasts = true;

        TasAtmaAlani birakmaAlani = (eventData.pointerEnter == null) ? null : eventData.pointerEnter.GetComponent<TasAtmaAlani>();

        if (birakmaAlani == null || !birakmaAlani.GecerliBirakmaAlaniMi(temsilEttigiTas, null)) // Oyuncu parametresi �imdilik null
        {
            // Ge�erli bir alana b�rak�lmad�, orijinal paneline ve pozisyonuna geri d�n
            transform.SetParent(orijinalPanel);
            _rectTransform.localPosition = _orijinalLocalPozisyon;
            // Layout grubunun eli yeniden d�zenlemesi i�in UIManager'dan eli tekrar �izdirmesi gerekebilir.
            // Veya sadece aktif oyuncunun elini tekrar �izdir:
            if (uiManagerReferansi != null && uiManagerReferansi.gameManager != null && uiManagerReferansi.gameManager.oyuncular.Count > uiManagerReferansi.gameManager.AktifOyuncuIndex)
            {
                if (uiManagerReferansi.gameManager.AktifOyuncuIndex == 0) // �imdilik sadece Oyuncu 1'in UI'�n� g�ncelliyoruz
                    uiManagerReferansi.Oyuncu1EliniGoster(uiManagerReferansi.gameManager.oyuncular[uiManagerReferansi.gameManager.AktifOyuncuIndex]);
            }
        }
        // E�er ge�erli bir alana b�rak�ld�ysa, TasAtmaAlani.OnDrop metodu zaten UIManager arac�l���yla GameManager'� tetiklemi� olacak
        // ve GameManager eli g�ncelleyip UIManager'a yeniden �izdirecek.
    }
}