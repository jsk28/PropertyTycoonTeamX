using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace View 
{
    public class PropertyManager : MonoBehaviour
    {
    [SerializeField] public DictionaryWrapper<int,PurchasableCard> cards;

    public Model.Player player;
    public Text ownedByText;

    public static PropertyManager Create(Transform parent, Model.Player player, Dictionary<int,PurchasableCard> propertyCards,bool canManage)
    {
        PropertyManager manager = Instantiate(Asset.PropertyManagerPrefab,parent).GetComponent<PropertyManager>();
        manager.player = player;
        manager.ownedByText.text = "Properties owned by " + player.name;
        manager.setUpCards(player,propertyCards,canManage);
        return manager;
    }

    public void setUpCards(Model.Player player, Dictionary<int,PurchasableCard> propertyCards,bool canManage)
    {
        foreach(KeyValuePair<int, PurchasableCard> entry in propertyCards)
        {
            if(!player.owned_spaces.Contains(entry.Value.property))
            {
                cards.getValue(entry.Key).gameObject.SetActive(false);
            } else {
                cards.getValue(entry.Key).gameObject.AddComponent<ManagableCard>(); 
                cards.getValue(entry.Key).gameObject.GetComponent<ManagableCard>().canManage = canManage;
                cards.getValue(entry.Key).gameObject.SetActive(true);
                cards.getValue(entry.Key).gameObject.transform.Find("Mortgaged").gameObject.SetActive(entry.Value.property.isMortgaged);
                //change color or whatever
                if(entry.Value.property.type == SqType.PROPERTY)
                {
                    ((PropertyCard)(cards.getValue(entry.Key))).setUpCard((PropertyCard)entry.Value);
                    ((PropertyCard)(cards.getValue(entry.Key))).showHouse(((Model.Space.Property)(cards.getValue(entry.Key).property)).noOfHouses);
                } 
                else if (entry.Value.property.type == SqType.UTILITY) {
                    ((UtilityCard)(cards.getValue(entry.Key))).setUpCard((UtilityCard)entry.Value);
                } else {
                    ((StationCard)(cards.getValue(entry.Key))).setUpCard((StationCard)entry.Value);
                }
            }
        }
    }

    public void destroy()
    {
        Destroy(this.gameObject);
    }
}
}
