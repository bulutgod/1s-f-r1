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
            Debug.LogError("GameManager: UIManager atanmam��! L�tfen Inspector �zerinden atay�n.");
            return;
        }

      
        OyunuBaslat();
    }

    public void OyunuBaslat() 
    {
        Debug.Log("Oyun Ba�lat�l�yor...");

    

        oyuncular.Clear();
        atilanTaslarYigini.Clear(); 
        oynananElSayisiBuTurda = 0;
        aktifOyuncuIndex = 0;

        for (int i = 0; i < oyuncuSayisi; i++)
        {
           
            Oyuncu yeniOyuncu = new Oyuncu("Oyuncu " + (i + 1), i == 0, 1000); 
            oyuncular.Add(yeniOyuncu);
        }
        Debug.Log(oyuncuSayisi + " adet oyuncu olu�turuldu.");

        Debug.Log("Ta�lar da��t�l�yor...");
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
                    Debug.LogError("HATA: Deste bitti ancak hala ta� da��t�lmaya �al���l�yor! Oyuncu: " + oyuncular[i].oyuncuAdi);
                    break;
                }
            }
        }

        Debug.Log("Ta� da��t�m� tamamland�. Oyuncu elleri (Konsol):");
        foreach (Oyuncu oyuncu in oyuncular)
        {
           
            Debug.Log(oyuncu.oyuncuAdi + " elindeki ta� say�s�: " + oyuncu.eli.Count);
        }

        GostergeVeOkeyBelirle();

        if (desteManager != null)
        {
            Debug.Log("Destede kalan ta� say�s� (G�sterge sonras�): " + desteManager.KalanTasSayisi());
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
       
        Debug.Log("Oyun kurulumu tamamland�. UI'da Oyuncu 1'in eli g�steriliyor olmal�.");
        Debug.Log("Tur sim�lasyonu �imdilik ba�lat�lmad�.");
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
                Debug.LogError("G�STERGE BEL�RLENEMED�: G�sterge �ekmek i�in destede hi� ta� kalmad�!");
                this.gostergeTasi = null;
                this.okeyTasi = null;
                return;
            }

            potansiyelGosterge = desteManager.TasCek();
            denemeSayaci++;

            if (potansiyelGosterge == null && desteManager.KalanTasSayisi() > 0)
            {
                Debug.LogWarning("Potansiyel g�sterge null geldi ama destede hala ta� var. Tekrar deneniyor. Deneme: " + denemeSayaci);
                continue;
            }

            if (potansiyelGosterge != null && potansiyelGosterge.tip == TasTipi.SahteOkey)
            {
                Debug.Log("G�sterge i�in Sahte Okey �ekildi (" + potansiyelGosterge.ToString() + "). Bu ta� ge�erli g�sterge de�il, yeni g�sterge �ekilecek. Deneme: " + denemeSayaci);
                potansiyelGosterge = null;
            }

            if (denemeSayaci > MAKSIMUM_GOSTERGE_DENEMESI)
            {
                Debug.LogError("G�STERGE BEL�RLENEMED�: Maksimum deneme say�s�na ula��ld� (" + MAKSIMUM_GOSTERGE_DENEMESI + "). Destede uygun g�sterge bulunamad�.");
                this.gostergeTasi = null;
                this.okeyTasi = null;
                return;
            }

        } while (potansiyelGosterge == null || potansiyelGosterge.tip == TasTipi.SahteOkey);

        this.gostergeTasi = potansiyelGosterge;

        if (this.gostergeTasi == null)
        {
            Debug.LogError("G�STERGE TA�I HALA NULL! OKEY BEL�RLENEMEYECEK.");
            this.okeyTasi = null;
            return;
        }

        Debug.Log("G�sterge Ta��: " + this.gostergeTasi.ToString());

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
                Debug.Log("Okey Ta��: " + this.okeyTasi.ToString());
            }
            else
            {
                Debug.LogError("Belirlenen okey ta�� (" + okeyRengi + " " + okeyNumarasi + ") TasVeritabani'nda bulunamad�!");
            }
        }
        else
        {
            Debug.LogError("Okey ta��n� bulmak i�in TasVeritabani referans� eksik!");
        }
    }

    
    public void OyuncuCiftAcmakIstedi(Oyuncu oyuncu)
    {
        if (oyuncu == null) return;

        if (oyuncu.CiftAcilabilirMi())
        {
            oyuncu.ciftAcilmisMi = true;
            Debug.Log(oyuncu.oyuncuAdi + " �ift a�t�!");
            
        }
        else
        {
            Debug.Log(oyuncu.oyuncuAdi + " �ift a�may� denedi ama yeterli �ifti yok.");
            
        }
    }

   
}