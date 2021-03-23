using System.IO;
using UnityEngine;

public partial class SROptions
{
    public void ClearSave()
    {
        File.Delete(Application.persistentDataPath + "/save.bin");
    } 

    public void GiveCoins()
    {
        PlayerData.instance.coins += 1000000;
        PlayerData.instance.premium += 1000;
        PlayerData.instance.Save();
    }

    public void AddConsumables()
    {
        for(int i = 0; i < ShopItemList.s_ConsumablesTypes.Length; ++i)
        {
            Consumable c = ConsumableDatabase.GetConsumbale(ShopItemList.s_ConsumablesTypes[i]);
            if(c != null)
            {
                PlayerData.instance.consumables[c.GetConsumableType()] = 10;
            }
        }

        PlayerData.instance.Save();
    }
}