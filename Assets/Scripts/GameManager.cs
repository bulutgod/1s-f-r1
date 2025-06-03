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

    void Start()
    {
        if (desteManager == null)
        {
            Debug.LogError("GameManager: DesteManager atanmamis");
            return;
        }
        // Multiplayer'da oyun baþlatma kullanýcýdan tetiklenmeli.
    }

    // Oyun baþlatma, taþ daðýtýmý, hamle yönetimi gibi gerekli fonksiyonlar burada olmalý.
    // Test, otomatik demo, sabit ve örnek test bloklarý kaldýrýldý.
    // Multiplayer’da oyuncu hamleleri UI veya network mesajlarýyla tetiklenmelidir.

    // Oda/oyun sonu, çip iþlemleri, ceza gibi ek mekanikler buraya eklenmeli.
    public void OyuncuCiftAcmakIstedi(Oyuncu oyuncu)
    {
        if (oyuncu.CiftAcilabilirMi())
        {
            oyuncu.ciftAcilmisMi = true;
            // UI’da bilgilendirme: “Çift açtýn, puan 2x olacak”
        }
        else
        {
            // UI’da hata: “5 çiftin yok!”
        }
    }
}