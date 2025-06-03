using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class OkeyRulesEngine
{
    public static bool GecerliPerMi(List<Tas> potansiyelPer, Tas okeyTasiGercek)
    {
        string perStrOriginal = "null_liste";
        if (potansiyelPer != null)
        {
            perStrOriginal = string.Join(", ", potansiyelPer.Select(p => p == null ? "NULL_TAS" : p.ToString()));
        }
        Debug.Log("--- GecerliPerMi (Detayl� Joker) Kontrol Ba�lad� ---");
        Debug.Log("Kontrol Edilen Orjinal Per: " + perStrOriginal + " | Okey Ta��: " + (okeyTasiGercek != null ? okeyTasiGercek.ToString() : "YOK"));

        if (potansiyelPer == null || potansiyelPer.Count < 3 || potansiyelPer.Count > 4)
        {
            Debug.Log("[PER_DJ_TEST] Hata: Ta� say�s� 3 veya 4 olmal�.");
            return false;
        }


        List<Tas> sayisalTaslar = new List<Tas>();
        int wildcardSayisi = 0;

        if (okeyTasiGercek == null)
        {
            Debug.LogError("[PER_DJ_TEST] Hata: okeyTasiGercek parametresi null geldi!");
            return false;
        }

        foreach (Tas tas in potansiyelPer)
        {
            if (tas == null) { Debug.Log("[PER_DJ_TEST] Hata: Perde null ta� var."); return false; }

            if (tas.tip == TasTipi.SahteOkey)
            {

                sayisalTaslar.Add(okeyTasiGercek);
            }
            else if (tas == okeyTasiGercek)
            {
                wildcardSayisi++;
            }
            else if (tas.tip == TasTipi.Sayi)
            {
                sayisalTaslar.Add(tas);
            }
            else
            {
                Debug.Log("[PER_DJ_TEST] Hata: Perde ge�ersiz tipte bir ta� var: " + tas.ToString());
                return false;
            }
        }

        Debug.Log("[PER_DJ_TEST] Wildcard (Ger�ek Okey) Say�s�: " + wildcardSayisi);
        Debug.Log("[PER_DJ_TEST] Say�sal Ta�lar (Sahte Okeyler Okey'e d�n��m�� haliyle): " + string.Join(", ", sayisalTaslar.Select(p => p.ToString())));




        if (sayisalTaslar.Count == 0)
        {

            if (wildcardSayisi >= 3 && wildcardSayisi <= 4 && sayisalTaslar.Count == 0)
            {
                Debug.Log("[PER_DJ_TEST] Per tamamen wildcard'lardan olu�uyor. Ge�erli.");
                return true;
            }

        }


        if (sayisalTaslar.Count == 0 && wildcardSayisi < (potansiyelPer.Count > 0 ? potansiyelPer.Count : 3))
        {
            Debug.Log("[PER_DJ_TEST] Hata: Wildcard say�s� yetersiz ve hi� say�sal ta� yok.");
            return false;
        }




        int referansSayi = sayisalTaslar[0].sayi;
        foreach (Tas tas in sayisalTaslar)
        {
            if (tas.sayi != referansSayi)
            {
                Debug.Log("[PER_DJ_TEST] Hata: Say�sal ta�lar�n say�lar� ayn� de�il. Referans: " + referansSayi + ", Bulunan: " + tas.sayi);
                return false;
            }
        }
        Debug.Log("[PER_DJ_TEST] Say�sal ta�lar�n say�lar� ayn�: " + referansSayi);

        List<TasRengi> kullanilanRenkler = new List<TasRengi>();
        foreach (Tas tas in sayisalTaslar)
        {
            if (kullanilanRenkler.Contains(tas.renk))
            {
                Debug.Log("[PER_DJ_TEST] Hata: Say�sal ta�lar aras�nda ayn� renk tekrar� var: " + tas.renk);
                return false;
            }
            kullanilanRenkler.Add(tas.renk);
        }
        Debug.Log("[PER_DJ_TEST] Say�sal ta�lar�n renkleri kendi i�inde farkl�. Kullan�lan renk adedi: " + kullanilanRenkler.Count);


        Debug.Log("[PER_DJ_TEST] T�m kontroller ba�ar�l�. Bu ge�erli bir per (jokerli).");
        return true;
    }


    public static bool GecerliSeriMi(List<Tas> potansiyelSeri, Tas okeyTasiGercek)
    {
        string seriStrOriginal = "null_liste";
        if (potansiyelSeri != null)
        {
            seriStrOriginal = string.Join(", ", potansiyelSeri.Select(p => p == null ? "NULL_TAS" : p.ToString()));
        }
        

        if (potansiyelSeri == null || potansiyelSeri.Count < 3)
        {
            return false;
        }

        if (okeyTasiGercek == null || okeyTasiGercek.tip != TasTipi.Sayi)
        {
            
            List<Tas> siraliSafTaslar = new List<Tas>(potansiyelSeri);
            foreach (Tas tst in siraliSafTaslar)
            {
                if (tst == null || tst.tip != TasTipi.Sayi)
                {
                    
                    return false;
                }
            }
            if (siraliSafTaslar.Count == 0 && potansiyelSeri.Count >= 3) {  return false; }
            if (siraliSafTaslar.Count == 0 && potansiyelSeri.Count < 3) return false; 

            if (siraliSafTaslar.Count > 0)
            {
                TasRengi r = siraliSafTaslar[0].renk;
                if (!siraliSafTaslar.All(t => t.renk == r))
                {
                    
                    return false;
                }
                siraliSafTaslar.Sort((a, b) => a.sayi.CompareTo(b.sayi));
                for (int i = 0; i < siraliSafTaslar.Count - 1; i++)
                {
                    if (siraliSafTaslar[i + 1].sayi != siraliSafTaslar[i].sayi + 1)
                    {
                        
                        return false;
                    }
                }
                
                return true;
            }
            
            return false;
        }

        List<Tas> islenecekTaslar = new List<Tas>(); 
        int wildcardAdedi = 0;                 

        foreach (Tas tas in potansiyelSeri)
        {
            if (tas == null) {  return false; }

            if (tas.tip == TasTipi.SahteOkey)
            {
                islenecekTaslar.Add(okeyTasiGercek); 
            }
            else if (tas == okeyTasiGercek)
            {
                wildcardAdedi++; 
            }
            else if (tas.tip == TasTipi.Sayi)
            {
                islenecekTaslar.Add(tas);
            }
            else
            {
               
                return false;
            }
        }

        

        if (islenecekTaslar.Count == 0) 
        {
            
            bool sonuc = wildcardAdedi >= 3 && wildcardAdedi == potansiyelSeri.Count;
           
            return sonuc;
        }

       
        TasRengi seriRengi = islenecekTaslar[0].renk; 
        foreach (Tas tas in islenecekTaslar)
        {
            if (tas.renk != seriRengi)
            {
                
                return false;
            }
        }
  
        islenecekTaslar.Sort((a, b) => a.sayi.CompareTo(b.sayi));

        List<int> benzersizSayilar = new List<int>();
        if (islenecekTaslar.Count > 0)
        {
            benzersizSayilar.Add(islenecekTaslar[0].sayi);
            for (int i = 1; i < islenecekTaslar.Count; i++)
            {
                if (islenecekTaslar[i].sayi > islenecekTaslar[i - 1].sayi) 
                {
                    benzersizSayilar.Add(islenecekTaslar[i].sayi);
                }
                else if (islenecekTaslar[i].sayi == islenecekTaslar[i - 1].sayi)
                {
                    
                    return false;
                }
            }
        }

        if (benzersizSayilar.Count == 0)
        { 
            return wildcardAdedi >= 3 && wildcardAdedi == potansiyelSeri.Count;
        }

  
        int minSayi = benzersizSayilar[0];
        int maxSayi = benzersizSayilar[benzersizSayilar.Count - 1];

        int teorikSeriUzunluguBoylesiSayilarla = maxSayi - minSayi + 1;
        int gerekenWildcardSayisi = teorikSeriUzunluguBoylesiSayilarla - benzersizSayilar.Count;

        if (gerekenWildcardSayisi < 0) { gerekenWildcardSayisi = 0; } 

      

        if (wildcardAdedi >= gerekenWildcardSayisi)
        {
            
            int olusanTamSeriUzunlugu = benzersizSayilar.Count + gerekenWildcardSayisi; 

            
            if (olusanTamSeriUzunlugu == potansiyelSeri.Count)
            {
                
                return true;
            }
            else
            {
                
                return false;
            }
        }
        else
        {
            
            return false;
        }
    }
    public static int ElinPuaniniHesapla(List<Tas> oyuncuEli, Tas okeyTasiGercek)
    {
        Debug.Log("--- Elin Puan� Hesaplama Ba�lad� ---");
        if (oyuncuEli == null || oyuncuEli.Count == 0)
        {
            Debug.Log("[PUAN_HESAPLAMA] El bo�, puan: 0");
            return 0;
        }
        if (okeyTasiGercek == null)
        {
            Debug.LogError("[PUAN_HESAPLAMA] Okey Ta�� Ger�ek null, puan hesaplanam�yor.");
            return 0;
        }

        // Oyuncunun elindeki ta�lar� kopyalayal�m ki orijinal listeyi bozmayal�m
        List<Tas> kalanTaslar = new List<Tas>(oyuncuEli);
        int toplamPuan = 0;
        int sahteOkeySayisi = kalanTaslar.Count(t => t.tip == TasTipi.SahteOkey);
        int gercekOkeySayisi = kalanTaslar.Count(t => t == okeyTasiGercek);

        Debug.Log("[PUAN_HESAPLAMA] El: " + string.Join(", ", kalanTaslar.Select(t => t.ToString())));
        Debug.Log("[PUAN_HESAPLAMA] Sahte Okey Say�s�: " + sahteOkeySayisi + ", Ger�ek Okey Say�s�: " + gercekOkeySayisi);

        // Olas� t�m per ve seri kombinasyonlar�n� bulmak daha karma��k bir algoritma gerektirir.
        // �imdilik basit�e, jokerleri kullanarak olu�turulabilecek per ve serilerin puan�n� alaca��z.
        // Daha geli�mi� bir puanlama sistemi i�in "maksimum set bulma" problemi ��z�lmeli.
        // Bu metodun amac� "a��labilecek en y�ksek puan�" bulmak de�il,
        // verilen bir eldeki per ve serilerden ge�erli olanlar�n puan�n� hesaplamakt�r.
        // Ger�ek bir 101 oyununda, oyuncu elindeki t�m per ve serileri g�sterir.

        // Basit bir yakla��m: T�m ��l�/d�rtl� kombinasyonlar� dene ve ge�erli olanlar� puanla.
        // Bu Brute-Force y�ntemi b�y�k ellerde performans sorunlar� yaratabilir,
        // ancak ba�lang�� i�in yeterli olacakt�r. Daha sonra optimize edilebilir.

        List<List<Tas>> bulunanGecerliGruplar = new List<List<Tas>>();
        List<Tas> kullanilanTaslar = new List<Tas>();

        // Per ve Seri algoritmalar� olduk�a karma��kt�r.
        // Burada basit�e, eldeki t�m 3'l� ve 4'l� kombinasyonlar� deneriz.
        // Bu test ama�l� bir yakla��md�r. Ger�ek bir oyunda, eldeki en iyi per/seri setini bulmak i�in
        // daha sofistike bir algoritmaya ihtiya� vard�r (�rne�in, backtracking).

        // �imdilik, sadece PER ve SER� kombinasyonlar�n� ayr� ayr� deneyece�iz.
        // E�er bir ta� bir kere kullan�ld�ysa, ba�ka bir grupta tekrar kullan�lamaz.

        // Elimizdeki t�m ta�lar� tekil olarak kopyalayal�m
        List<Tas> mevcutTaslar = new List<Tas>(oyuncuEli);
        bool degisiklikYapildi = true;

        while (degisiklikYapildi)
        {
            degisiklikYapildi = false;
            // �lk olarak 4'l� gruplar� (Per veya Seri) ara
            for (int i = 0; i < mevcutTaslar.Count; i++)
            {
                for (int j = i + 1; j < mevcutTaslar.Count; j++)
                {
                    for (int k = j + 1; k < mevcutTaslar.Count; k++)
                    {
                        for (int l = k + 1; l < mevcutTaslar.Count; l++)
                        {
                            List<Tas> potansiyelGrup = new List<Tas> { mevcutTaslar[i], mevcutTaslar[j], mevcutTaslar[k], mevcutTaslar[l] };
                            if (GecerliPerMi(potansiyelGrup, okeyTasiGercek) || GecerliSeriMi(potansiyelGrup, okeyTasiGercek))
                            {
                                Debug.Log("[PUAN_HESAPLAMA] 4'l� Ge�erli Grup Bulundu: " + string.Join(", ", potansiyelGrup.Select(t => t.ToString())));
                                // Bu grubu puanla ve kullan�lan ta�lar� listeden ��kar
                                foreach (Tas tas in potansiyelGrup)
                                {
                                    toplamPuan += tas.sayi; // Okey ta�� say�s�n� say�lar�nda kullanaca��z
                                    // Sahte okey veya ger�ek okey yerine ge�ti�i say�ya g�re puanlan�r
                                    if (tas.tip == TasTipi.SahteOkey || tas == okeyTasiGercek)
                                    {
                                        toplamPuan += okeyTasiGercek.sayi; // Okey/Sahte Okey'in yerine ge�ti�i ta��n say�s�n� ekle
                                    }
                                }

                                // Kullan�lan ta�lar� kald�r
                                // Bu k�s�m dikkatli yaz�lmal�, RemoveAt indeksleri kayd�rabilir.
                                // En iyisi �nce listeyi olu�turup sonra kald�rmak.
                                mevcutTaslar.Remove(potansiyelGrup[0]);
                                mevcutTaslar.Remove(potansiyelGrup[1]);
                                mevcutTaslar.Remove(potansiyelGrup[2]);
                                mevcutTaslar.Remove(potansiyelGrup[3]);

                                degisiklikYapildi = true;
                                goto NextIteration; // �� i�e d�ng�lerden ��kmak i�in
                            }
                        }
                    }
                }
            }

            // Ard�ndan 3'l� gruplar� ara
            for (int i = 0; i < mevcutTaslar.Count; i++)
            {
                for (int j = i + 1; j < mevcutTaslar.Count; j++)
                {
                    for (int k = j + 1; k < mevcutTaslar.Count; k++)
                    {
                        List<Tas> potansiyelGrup = new List<Tas> { mevcutTaslar[i], mevcutTaslar[j], mevcutTaslar[k] };
                        if (GecerliPerMi(potansiyelGrup, okeyTasiGercek) || GecerliSeriMi(potansiyelGrup, okeyTasiGercek))
                        {
                            Debug.Log("[PUAN_HESAPLAMA] 3'l� Ge�erli Grup Bulundu: " + string.Join(", ", potansiyelGrup.Select(t => t.ToString())));
                            foreach (Tas tas in potansiyelGrup)
                            {
                                toplamPuan += tas.sayi;
                                if (tas.tip == TasTipi.SahteOkey || tas == okeyTasiGercek)
                                {
                                    toplamPuan += okeyTasiGercek.sayi;
                                }
                            }
                            mevcutTaslar.Remove(potansiyelGrup[0]);
                            mevcutTaslar.Remove(potansiyelGrup[1]);
                            mevcutTaslar.Remove(potansiyelGrup[2]);

                            degisiklikYapildi = true;
                            goto NextIteration;
                        }
                    }
                }
            }
        NextIteration:; // goto i�in etiket
        }


        // Kalan ta�lar�n puan� (a��lmam�� ta�lar) genellikle say�lmaz.
        // Ancak a�ma esnas�nda bazen �iftler (2 adet ayn� ta�) puan�n �zerine etki edebilir.
        // �imdilik sadece per ve serilerin toplam puan�n� al�yoruz.
        // Tek kalan ta�lar (e�le�meyenler) ve �iftler normalde puana dahil edilmez.

        Debug.Log("[PUAN_HESAPLAMA] Toplam Hesaplanan Puan: " + toplamPuan);
        return toplamPuan;
    }

}