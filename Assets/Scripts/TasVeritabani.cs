using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "VarsayilanTasVeritabani", menuName = "101 Okey/Tas Veritabani")]
public class TasVeritabani : ScriptableObject
{
    public List<Tas> tumTaslar = new List<Tas>();
}