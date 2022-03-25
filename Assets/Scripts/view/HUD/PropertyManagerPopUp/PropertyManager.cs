using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace View 
{
    public class PropertyManager : MonoBehaviour
    {
    // Start is called before the first frame update
    [SerializeField] public DictionaryWrapper<int,PurchasableCard> cards;

    Model.Player player;
    public Text ownedByText;
    public Button GoBunkruptBtn; 

    public static PropertyManager Create(Transform parent, Model.Player player, Dictionary<int,PurchasableCard> propertyCards,bool canManage)
    {
        PropertyManager manager = Instantiate(Asset.propertyManager(),parent).GetComponent<PropertyManager>();
        manager.player = player;
        manager.ownedByText.text = "Propierties owned by " + player.name;
        manager.setUpCards(player,propertyCards,canManage);
        return manager;
    }

    void setUpCards(Model.Player player, Dictionary<int,PurchasableCard> propertyCards,bool canManage)
    {
        foreach(KeyValuePair<int, PurchasableCard> entry in propertyCards)
        {
            if(Random.Range(0,2) == 1)
            //if(!player.owned_spaces.Contains(entry.Value.property))
            {
                cards.getValue(entry.Key).gameObject.SetActive(false);
            } else {
                cards.getValue(entry.Key).gameObject.SetActive(true);
                //change color or whatever
                switch(entry.Value.property.type)
                {
                    case SqType.PROPERTY:
                    ((PropertyCard)(cards.getValue(entry.Key))).setUpCard((PropertyCard)entry.Value);
                    //((PropertyCard)(cards.getValue(entry.Key))).showHouse(((Model.Space.Property)(cards.getValue(entry.Key).property)).noOfHouses);
                    ((PropertyCard)(cards.getValue(entry.Key))).showHouse(Random.Range(0,6));
                    if(canManage)
                    {
                        cards.getValue(entry.Key).gameObject.AddComponent<ManagePropertyController>();
                    }
                    break;
                    case SqType.STATION:
                    ((StationCard)(cards.getValue(entry.Key))).setUpCard((StationCard)entry.Value);
                    if(canManage)
                    {
                        cards.getValue(entry.Key).gameObject.AddComponent<ManageUtilityController>();
                    }
                    break;
                    case SqType.UTILITY:
                    ((UtilityCard)(cards.getValue(entry.Key))).setUpCard((UtilityCard)entry.Value);
                    if(canManage)
                    {
                        cards.getValue(entry.Key).gameObject.AddComponent<ManageUtilityController>();
                    }
                    break;
                }
            }
        }
    }

    public void GoBankrupt()
    {
        Debug.Log("Player " + player.name + " goes bankrupt!");
        Destroy(this.gameObject);
    }

    public void destroy()
    {
        Destroy(this.gameObject);
    }
}
}
