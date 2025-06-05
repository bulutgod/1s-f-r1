using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;


public class UIManager : MonoBehaviour
{
    private UITasEtkilesimi _aktifSeciliTasUI = null;
    [Header("Bağlantılar")]
    public GameManager gameManager; 
    
    [Header("Oyuncu eli UI Baglantilar")]
    public GameObject tasUIPrefab;
    public Transform oyuncu1ElPaneliTransform;

    void Start()
    {
       
    }

    public void Oyuncu1EliniGoster(Oyuncu oyuncu)
    {
        if (oyuncu1ElPaneliTransform == null)
        {
            Debug.LogError("UIManager: oyuncu1ElPaneliTransform atanmamis, Oyuncu 1'in eli gösterilemiyor!");
            return;
        }
        if (tasUIPrefab == null)
        {
            Debug.LogError("UIManager: tasUIPrefab atanmamış, taşlar oluşturulamıyor!");
            return;
        }

        TemizleOyuncuEliUI(oyuncu1ElPaneliTransform);

        if (oyuncu == null)
        {
            Debug.LogWarning("UIManager: Oyuncu1EliniGoster çağrıldı ama oyuncu referansı null! Panel temizlendi.");
            return;
        }

        if (oyuncu.eli != null)
        {
            foreach (Tas tasVerisi in oyuncu.eli)
            {
                if (tasVerisi == null)
                {
                    Debug.LogWarning("Oyuncunun elinde null bir taş verisi bulundu, UI'da gösterilmeyecek.");
                    continue;
                }

                GameObject yeniTasUIObjesi = Instantiate(tasUIPrefab, oyuncu1ElPaneliTransform);
                yeniTasUIObjesi.name = tasVerisi.ToString();
                UITasEtkilesimi tasEtkilesimScripti = yeniTasUIObjesi.GetComponent<UITasEtkilesimi>();
                if (tasEtkilesimScripti != null)
                {
                    
                    tasEtkilesimScripti.Ayarla(tasVerisi, this, oyuncu1ElPaneliTransform);
                    
                }

                Image tasResmi = yeniTasUIObjesi.GetComponent<Image>();
                TextMeshProUGUI sayiText = yeniTasUIObjesi.GetComponentInChildren<TextMeshProUGUI>();
                if (tasEtkilesimScripti != null && tasEtkilesimScripti == _aktifSeciliTasUI)
                {
                    
                    if (tasResmi != null)
                    {
                        
                        tasResmi.color = Color.Lerp(tasResmi.color, Color.yellow, 0.3f);
                    }
                }

                bool buTasOElinOkeyiMi = false;
                if (gameManager != null && gameManager.okeyTasi != null && tasVerisi == gameManager.okeyTasi)
                {
                    buTasOElinOkeyiMi = true;
                }

                
                if (tasResmi != null)
                {
                    if (tasVerisi.tasGorseli != null) 
                    {
                        tasResmi.sprite = tasVerisi.tasGorseli;
                        tasResmi.color = Color.white; 
                    }
                    else 
                    {
                        tasResmi.sprite = null;
                        tasResmi.color = Color.white;
                    }

                 
                    if (buTasOElinOkeyiMi)
                    {
                        
                    }
                }

                
                if (sayiText != null)
                {
                    if (buTasOElinOkeyiMi)
                    {
                        sayiText.text = ""; 
                        
                    }
                    else if (tasVerisi.tip == TasTipi.SahteOkey)
                    {
                        sayiText.text = "★"; 
                        sayiText.color = Color.green;
                    }
                    else 
                    {
                        sayiText.text = tasVerisi.sayi.ToString();
                        switch (tasVerisi.renk)
                        {
                            case TasRengi.Kirmizi: sayiText.color = new Color(0.9f, 0.1f, 0.1f); break;
                            case TasRengi.Sari: sayiText.color = new Color(0.8f, 0.6f, 0.0f); break;
                            case TasRengi.Siyah: sayiText.color = Color.black; break;
                            case TasRengi.Mavi: sayiText.color = new Color(0.1f, 0.2f, 0.8f); break;
                            default: sayiText.color = Color.grey; break;
                        }
                    }
                }
            }
        }
    }

    void TemizleOyuncuEliUI(Transform elPaneli)
    {
        if (elPaneli == null) return;
        foreach (Transform cocukTas in elPaneli)
        {
            Destroy(cocukTas.gameObject);
        }
    }
    public void OyuncuTasAtti(Tas atilanTasDetayi)
    {
        if (gameManager != null)
        {
            bool basariliMi = gameManager.SiraKimdeyseOAtar(atilanTasDetayi); 
            if (basariliMi)
            {
                
                Debug.Log("UIManager: Oyuncu " + atilanTasDetayi.ToString() + " taşını attı (GameManager'a iletildi).");
            }
            else
            {
                
                Debug.LogWarning("UIManager: Taş atma işlemi GameManager tarafından onaylanmadı.");
            }
        }
    }
    public void OyuncuTasAttiYerineBirakti(UITasEtkilesimi atilanTasUIScripti)
    {
        if (atilanTasUIScripti == null || atilanTasUIScripti.temsilEttigiTas == null) return;

        Tas atilanTasDetayi = atilanTasUIScripti.temsilEttigiTas;
        Debug.Log("UIManager: " + atilanTasDetayi.ToString() + " taşı atılmak üzere GameManager'a bildiriliyor.");

        if (gameManager != null)
        {
            
            bool atmaBasarili = gameManager.SiraKimdeyseOAtar(atilanTasDetayi); 
            if (atmaBasarili)
            {
                
            }
            else
            {
                
                Debug.LogWarning("UIManager: Taş atma işlemi GameManager tarafından onaylanmadı. Taş geri yerine konulmalı.");
                
                if (gameManager != null && gameManager.oyuncular.Count > gameManager.AktifOyuncuIndex) 
                    Oyuncu1EliniGoster(gameManager.oyuncular[gameManager.AktifOyuncuIndex]);
            }
        }
    }
}