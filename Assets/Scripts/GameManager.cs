using UnityEngine;
using System.Collections.Generic;
using System.Linq; 

public class GameManager : MonoBehaviour
{
    [Header("Baglantilar")]
    public DesteManager desteManager;

    [Header("Oyun Ayarlari")]
    public int oyuncuSayisi = 4;
    public List<Oyuncu> oyuncular = new List<Oyuncu>();

    [Header("Oyun Durumu")]
    public Tas gostergeTasi;
    public Tas okeyTasi;
    private int aktifOyuncuIndex = 0;
    public List<Tas> atilanTaslarYigini = new List<Tas>();
    private int oynananElSayisiBuTurda = 0;

    [Header("UI Yonetimi")]
    public UIManager uiManager; 

    void Start()
    {
        if (desteManager == null)
        {
            Debug.LogError("GameManager: DesteManager atanmamis!");
            return;
        }
        if (uiManager == null)
        {
            Debug.LogError("GameManager: UIManager atanmamýþ! Lütfen Inspector üzerinden atayýn.");
            return;
        }

      
        OyunuBaslat();
    }

    public void OyunuBaslat() 
    {
        Debug.Log("Oyun Baþlatýlýyor...");

    

        oyuncular.Clear();
        atilanTaslarYigini.Clear(); 
        oynananElSayisiBuTurda = 0;
        aktifOyuncuIndex = 0;

        for (int i = 0; i < oyuncuSayisi; i++)
        {
           
            Oyuncu yeniOyuncu = new Oyuncu("Oyuncu " + (i + 1), i == 0, 1000); 
            oyuncular.Add(yeniOyuncu);
        }
        Debug.Log(oyuncuSayisi + " adet oyuncu oluþturuldu.");

        Debug.Log("Taþlar daðýtýlýyor...");
        for (int i = 0; i < oyuncular.Count; i++)
        {
            int dagitilacakTasSayisi = oyuncular[i].ilkOyuncuMu ? 22 : 21;
            for (int j = 0; j < dagitilacakTasSayisi; j++)
            {
                Tas cekilenTas = desteManager.TasCek();
                if (cekilenTas != null)
                {
                    oyuncular[i].TasaEkle(cekilenTas);
                }
                else
                {
                    Debug.LogError("HATA: Deste bitti ancak hala taþ daðýtýlmaya çalýþýlýyor! Oyuncu: " + oyuncular[i].oyuncuAdi);
                    break;
                }
            }
        }

        Debug.Log("Taþ daðýtýmý tamamlandý. Oyuncu elleri (Konsol):");
        foreach (Oyuncu oyuncu in oyuncular)
        {
           
            Debug.Log(oyuncu.oyuncuAdi + " elindeki taþ sayýsý: " + oyuncu.eli.Count);
        }

        GostergeVeOkeyBelirle();

        if (desteManager != null)
        {
            Debug.Log("Destede kalan taþ sayýsý (Gösterge sonrasý): " + desteManager.KalanTasSayisi());
        }

        if (uiManager != null && oyuncular.Count > 0)
        {
            uiManager.Oyuncu1EliniGoster(oyuncular[0]); 
        }

        
        if (oyuncular.Count > 0 && oyuncular[aktifOyuncuIndex].ilkOyuncuMu)
        {
            
        }
        else if (oyuncular.Count > 0)
        {
           
        }
       
        Debug.Log("Oyun kurulumu tamamlandý. UI'da Oyuncu 1'in eli gösteriliyor olmalý.");
        Debug.Log("Tur simülasyonu þimdilik baþlatýlmadý.");
    }

    void GostergeVeOkeyBelirle()
    {
        Tas potansiyelGosterge = null;
        int denemeSayaci = 0;
        const int MAKSIMUM_GOSTERGE_DENEMESI = 110;

        do
        {
            if (desteManager.KalanTasSayisi() == 0)
            {
                Debug.LogError("GÖSTERGE BELÝRLENEMEDÝ: Gösterge çekmek için destede hiç taþ kalmadý!");
                this.gostergeTasi = null;
                this.okeyTasi = null;
                return;
            }

            potansiyelGosterge = desteManager.TasCek();
            denemeSayaci++;

            if (potansiyelGosterge == null && desteManager.KalanTasSayisi() > 0)
            {
                Debug.LogWarning("Potansiyel gösterge null geldi ama destede hala taþ var. Tekrar deneniyor. Deneme: " + denemeSayaci);
                continue;
            }

            if (potansiyelGosterge != null && potansiyelGosterge.tip == TasTipi.SahteOkey)
            {
                Debug.Log("Gösterge için Sahte Okey çekildi (" + potansiyelGosterge.ToString() + "). Bu taþ geçerli gösterge deðil, yeni gösterge çekilecek. Deneme: " + denemeSayaci);
                potansiyelGosterge = null;
            }

            if (denemeSayaci > MAKSIMUM_GOSTERGE_DENEMESI)
            {
                Debug.LogError("GÖSTERGE BELÝRLENEMEDÝ: Maksimum deneme sayýsýna ulaþýldý (" + MAKSIMUM_GOSTERGE_DENEMESI + "). Destede uygun gösterge bulunamadý.");
                this.gostergeTasi = null;
                this.okeyTasi = null;
                return;
            }

        } while (potansiyelGosterge == null || potansiyelGosterge.tip == TasTipi.SahteOkey);

        this.gostergeTasi = potansiyelGosterge;

        if (this.gostergeTasi == null)
        {
            Debug.LogError("GÖSTERGE TAÞI HALA NULL! OKEY BELÝRLENEMEYECEK.");
            this.okeyTasi = null;
            return;
        }

        Debug.Log("Gösterge Taþý: " + this.gostergeTasi.ToString());

        TasRengi okeyRengi = this.gostergeTasi.renk;
        int okeyNumarasi;

        if (this.gostergeTasi.sayi == 13)
        {
            okeyNumarasi = 1;
        }
        else
        {
            okeyNumarasi = this.gostergeTasi.sayi + 1;
        }

        this.okeyTasi = null;

        if (desteManager.TasVeritabani != null) 
        {
            this.okeyTasi = desteManager.TasVeritabani.tumTaslar.FirstOrDefault(t =>
                t != null &&
                t.tip == TasTipi.Sayi &&
                t.renk == okeyRengi &&
                t.sayi == okeyNumarasi);

            if (this.okeyTasi != null)
            {
                Debug.Log("Okey Taþý: " + this.okeyTasi.ToString());
            }
            else
            {
                Debug.LogError("Belirlenen okey taþý (" + okeyRengi + " " + okeyNumarasi + ") TasVeritabani'nda bulunamadý!");
            }
        }
        else
        {
            Debug.LogError("Okey taþýný bulmak için TasVeritabani referansý eksik!");
        }
    }

    
    public void OyuncuCiftAcmakIstedi(Oyuncu oyuncu)
    {
        if (oyuncu == null) return;

        if (oyuncu.CiftAcilabilirMi())
        {
            oyuncu.ciftAcilmisMi = true;
            Debug.Log(oyuncu.oyuncuAdi + " çift açtý!");
            
        }
        else
        {
            Debug.Log(oyuncu.oyuncuAdi + " çift açmayý denedi ama yeterli çifti yok.");
            
        }
    }

   
}