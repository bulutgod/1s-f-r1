using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class DesteManager : MonoBehaviour
{
    [Header("Veri Kaynaðý")]
    public TasVeritabani TasVeritabani;

    private List<Tas> aktifDeste = new List<Tas>();

    void Start()
    {
        if (TasVeritabani == null)
        {
            Debug.LogError("DesteManager: TasVeritabani atanmamýþ!");
            return;
        }
        DesteOlusturVeKaristir();
    }

    public void DesteOlusturVeKaristir()
    {
        aktifDeste.Clear();

        foreach (Tas tasSO in TasVeritabani.tumTaslar)
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
    }

    public Tas TasCek()
    {
        if (aktifDeste.Count == 0)
        {
            return null;
        }

        Tas cekilenTas = aktifDeste[aktifDeste.Count - 1];
        aktifDeste.RemoveAt(aktifDeste.Count - 1);
        return cekilenTas;
    }

    public int KalanTasSayisi()
    {
        return aktifDeste.Count;
    }
}