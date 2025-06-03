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
        Debug.Log("--- GecerliPerMi (Detaylý Joker) Kontrol Baþladý ---");
        Debug.Log("Kontrol Edilen Orjinal Per: " + perStrOriginal + " | Okey Taþý: " + (okeyTasiGercek != null ? okeyTasiGercek.ToString() : "YOK"));

        if (potansiyelPer == null || potansiyelPer.Count < 3 || potansiyelPer.Count > 4)
        {
            Debug.Log("[PER_DJ_TEST] Hata: Taþ sayýsý 3 veya 4 olmalý.");
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
            if (tas == null) { Debug.Log("[PER_DJ_TEST] Hata: Perde null taþ var."); return false; }

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
                Debug.Log("[PER_DJ_TEST] Hata: Perde geçersiz tipte bir taþ var: " + tas.ToString());
                return false;
            }
        }

        Debug.Log("[PER_DJ_TEST] Wildcard (Gerçek Okey) Sayýsý: " + wildcardSayisi);
        Debug.Log("[PER_DJ_TEST] Sayýsal Taþlar (Sahte Okeyler Okey'e dönüþmüþ haliyle): " + string.Join(", ", sayisalTaslar.Select(p => p.ToString())));




        if (sayisalTaslar.Count == 0)
        {

            if (wildcardSayisi >= 3 && wildcardSayisi <= 4 && sayisalTaslar.Count == 0)
            {
                Debug.Log("[PER_DJ_TEST] Per tamamen wildcard'lardan oluþuyor. Geçerli.");
                return true;
            }

        }


        if (sayisalTaslar.Count == 0 && wildcardSayisi < (potansiyelPer.Count > 0 ? potansiyelPer.Count : 3))
        {
            Debug.Log("[PER_DJ_TEST] Hata: Wildcard sayýsý yetersiz ve hiç sayýsal taþ yok.");
            return false;
        }




        int referansSayi = sayisalTaslar[0].sayi;
        foreach (Tas tas in sayisalTaslar)
        {
            if (tas.sayi != referansSayi)
            {
                Debug.Log("[PER_DJ_TEST] Hata: Sayýsal taþlarýn sayýlarý ayný deðil. Referans: " + referansSayi + ", Bulunan: " + tas.sayi);
                return false;
            }
        }
        Debug.Log("[PER_DJ_TEST] Sayýsal taþlarýn sayýlarý ayný: " + referansSayi);

        List<TasRengi> kullanilanRenkler = new List<TasRengi>();
        foreach (Tas tas in sayisalTaslar)
        {
            if (kullanilanRenkler.Contains(tas.renk))
            {
                Debug.Log("[PER_DJ_TEST] Hata: Sayýsal taþlar arasýnda ayný renk tekrarý var: " + tas.renk);
                return false;
            }
            kullanilanRenkler.Add(tas.renk);
        }
        Debug.Log("[PER_DJ_TEST] Sayýsal taþlarýn renkleri kendi içinde farklý. Kullanýlan renk adedi: " + kullanilanRenkler.Count);


        Debug.Log("[PER_DJ_TEST] Tüm kontroller baþarýlý. Bu geçerli bir per (jokerli).");
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
        Debug.Log("--- Elin Puaný Hesaplama Baþladý ---");
        if (oyuncuEli == null || oyuncuEli.Count == 0)
        {
            Debug.Log("[PUAN_HESAPLAMA] El boþ, puan: 0");
            return 0;
        }
        if (okeyTasiGercek == null)
        {
            Debug.LogError("[PUAN_HESAPLAMA] Okey Taþý Gerçek null, puan hesaplanamýyor.");
            return 0;
        }

        // Oyuncunun elindeki taþlarý kopyalayalým ki orijinal listeyi bozmayalým
        List<Tas> kalanTaslar = new List<Tas>(oyuncuEli);
        int toplamPuan = 0;
        int sahteOkeySayisi = kalanTaslar.Count(t => t.tip == TasTipi.SahteOkey);
        int gercekOkeySayisi = kalanTaslar.Count(t => t == okeyTasiGercek);

        Debug.Log("[PUAN_HESAPLAMA] El: " + string.Join(", ", kalanTaslar.Select(t => t.ToString())));
        Debug.Log("[PUAN_HESAPLAMA] Sahte Okey Sayýsý: " + sahteOkeySayisi + ", Gerçek Okey Sayýsý: " + gercekOkeySayisi);

        // Olasý tüm per ve seri kombinasyonlarýný bulmak daha karmaþýk bir algoritma gerektirir.
        // Þimdilik basitçe, jokerleri kullanarak oluþturulabilecek per ve serilerin puanýný alacaðýz.
        // Daha geliþmiþ bir puanlama sistemi için "maksimum set bulma" problemi çözülmeli.
        // Bu metodun amacý "açýlabilecek en yüksek puaný" bulmak deðil,
        // verilen bir eldeki per ve serilerden geçerli olanlarýn puanýný hesaplamaktýr.
        // Gerçek bir 101 oyununda, oyuncu elindeki tüm per ve serileri gösterir.

        // Basit bir yaklaþým: Tüm üçlü/dörtlü kombinasyonlarý dene ve geçerli olanlarý puanla.
        // Bu Brute-Force yöntemi büyük ellerde performans sorunlarý yaratabilir,
        // ancak baþlangýç için yeterli olacaktýr. Daha sonra optimize edilebilir.

        List<List<Tas>> bulunanGecerliGruplar = new List<List<Tas>>();
        List<Tas> kullanilanTaslar = new List<Tas>();

        // Per ve Seri algoritmalarý oldukça karmaþýktýr.
        // Burada basitçe, eldeki tüm 3'lü ve 4'lü kombinasyonlarý deneriz.
        // Bu test amaçlý bir yaklaþýmdýr. Gerçek bir oyunda, eldeki en iyi per/seri setini bulmak için
        // daha sofistike bir algoritmaya ihtiyaç vardýr (örneðin, backtracking).

        // Þimdilik, sadece PER ve SERÝ kombinasyonlarýný ayrý ayrý deneyeceðiz.
        // Eðer bir taþ bir kere kullanýldýysa, baþka bir grupta tekrar kullanýlamaz.

        // Elimizdeki tüm taþlarý tekil olarak kopyalayalým
        List<Tas> mevcutTaslar = new List<Tas>(oyuncuEli);
        bool degisiklikYapildi = true;

        while (degisiklikYapildi)
        {
            degisiklikYapildi = false;
            // Ýlk olarak 4'lü gruplarý (Per veya Seri) ara
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
                                Debug.Log("[PUAN_HESAPLAMA] 4'lü Geçerli Grup Bulundu: " + string.Join(", ", potansiyelGrup.Select(t => t.ToString())));
                                // Bu grubu puanla ve kullanýlan taþlarý listeden çýkar
                                foreach (Tas tas in potansiyelGrup)
                                {
                                    toplamPuan += tas.sayi; // Okey taþý sayýsýný sayýlarýnda kullanacaðýz
                                    // Sahte okey veya gerçek okey yerine geçtiði sayýya göre puanlanýr
                                    if (tas.tip == TasTipi.SahteOkey || tas == okeyTasiGercek)
                                    {
                                        toplamPuan += okeyTasiGercek.sayi; // Okey/Sahte Okey'in yerine geçtiði taþýn sayýsýný ekle
                                    }
                                }

                                // Kullanýlan taþlarý kaldýr
                                // Bu kýsým dikkatli yazýlmalý, RemoveAt indeksleri kaydýrabilir.
                                // En iyisi önce listeyi oluþturup sonra kaldýrmak.
                                mevcutTaslar.Remove(potansiyelGrup[0]);
                                mevcutTaslar.Remove(potansiyelGrup[1]);
                                mevcutTaslar.Remove(potansiyelGrup[2]);
                                mevcutTaslar.Remove(potansiyelGrup[3]);

                                degisiklikYapildi = true;
                                goto NextIteration; // Ýç içe döngülerden çýkmak için
                            }
                        }
                    }
                }
            }

            // Ardýndan 3'lü gruplarý ara
            for (int i = 0; i < mevcutTaslar.Count; i++)
            {
                for (int j = i + 1; j < mevcutTaslar.Count; j++)
                {
                    for (int k = j + 1; k < mevcutTaslar.Count; k++)
                    {
                        List<Tas> potansiyelGrup = new List<Tas> { mevcutTaslar[i], mevcutTaslar[j], mevcutTaslar[k] };
                        if (GecerliPerMi(potansiyelGrup, okeyTasiGercek) || GecerliSeriMi(potansiyelGrup, okeyTasiGercek))
                        {
                            Debug.Log("[PUAN_HESAPLAMA] 3'lü Geçerli Grup Bulundu: " + string.Join(", ", potansiyelGrup.Select(t => t.ToString())));
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
        NextIteration:; // goto için etiket
        }


        // Kalan taþlarýn puaný (açýlmamýþ taþlar) genellikle sayýlmaz.
        // Ancak açma esnasýnda bazen çiftler (2 adet ayný taþ) puanýn üzerine etki edebilir.
        // Þimdilik sadece per ve serilerin toplam puanýný alýyoruz.
        // Tek kalan taþlar (eþleþmeyenler) ve çiftler normalde puana dahil edilmez.

        Debug.Log("[PUAN_HESAPLAMA] Toplam Hesaplanan Puan: " + toplamPuan);
        return toplamPuan;
    }

}