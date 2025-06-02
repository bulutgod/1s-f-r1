using UnityEngine;
using System.Collections.Generic; 
using System.Linq; 

public class DesteManager : MonoBehaviour
{
    [Header("Veri Kayna��")]
    public TasVeritabani tasVeritabani; 

    private List<Tas> aktifDeste = new List<Tas>(); 

    void Start()
    {
        if (tasVeritabani == null)
        {
            Debug.LogError("DesteManager: TasVeritabani atanmam��!");
            return;
        }
        DesteOlusturVeKaristir();
        TestDeste();
    }

    public void DesteOlusturVeKaristir()
    {
        aktifDeste.Clear(); 

        foreach (Tas tasSO in tasVeritabani.tumTaslar)
        {
            aktifDeste.Add(tasSO);
        }

        
        System.Random rng = new System.Random();
        int n = aktifDeste.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            Tas deger = aktifDeste[k];
            aktifDeste[k] = aktifDeste[n];
            aktifDeste[n] = deger;
        }

        Debug.Log("Deste olu�turuldu ve kar��t�r�ld�. Toplam ta�: " + aktifDeste.Count);
    }

    public Tas TasCek()
    {
        if (aktifDeste.Count == 0)
        {
            Debug.LogWarning("Destede �ekilecek ta� kalmad�!");
            return null; 
        }

        
        Tas cekilenTas = aktifDeste[aktifDeste.Count - 1];
        aktifDeste.RemoveAt(aktifDeste.Count - 1); 

        return cekilenTas;
    }

    void TestDeste()
    {
        if (aktifDeste.Count > 0)
        {
            Debug.Log("Destenin en �st�ndeki ilk 5 ta� (kar��t�r�ld�ktan sonra):");
            for (int i = 0; i < Mathf.Min(5, aktifDeste.Count); i++)
            {
                
                Debug.Log((i + 1) + ". �ekilecek Ta�: " + aktifDeste[aktifDeste.Count - 1 - i].ToString());
            }
        }
    }
}
