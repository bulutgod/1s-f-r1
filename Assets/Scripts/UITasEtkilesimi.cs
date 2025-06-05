using UnityEngine;
using UnityEngine.EventSystems; 
using UnityEngine.UI;         
public class UITasEtkilesimi : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Tas temsilEttigiTas;
    public UIManager uiManagerReferansi; 

    private RectTransform _rectTransform;
    private Canvas _anaCanvas; 
    private CanvasGroup _canvasGroup; 
    private Vector3 _orijinalPozisyon; 

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

    public void Ayarla(Tas tasVerisi, UIManager uiManager)
    {
        temsilEttigiTas = tasVerisi;
        uiManagerReferansi = uiManager;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        
        _orijinalPozisyon = _rectTransform.localPosition; 
        _canvasGroup.alpha = 0.6f; 
        _canvasGroup.blocksRaycasts = false;

        
    }

    public void OnDrag(PointerEventData eventData)
    {
        
        _rectTransform.anchoredPosition += eventData.delta / _anaCanvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        
        _canvasGroup.alpha = 1f; 
        _canvasGroup.blocksRaycasts = true; 

        
        if (eventData.pointerEnter == null || eventData.pointerEnter.GetComponent<TasAtmaAlani>() == null) // TasAtmaAlani adýnda bir script'imiz olduðunu varsayalým
        {
            _rectTransform.localPosition = _orijinalPozisyon; // Geri dön
        }
        
    }
}