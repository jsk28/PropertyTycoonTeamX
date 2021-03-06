using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace View
{
    /// <summary>
    /// Script attached to 'manage property' popups.
    /// </summary>
public class ManagePurchasable : MonoBehaviour
{
    public Button mortgageBtn;
    public Button sellBtn;
    public Button buyHouseBtn;
    public Button sellHouseBtn;
    private SoundManager soundManager;

    void Awake()
    {
        soundManager = GameObject.FindWithTag("GameMusic").GetComponent<SoundManager>();
    }

    public static ManagePurchasable Create(Transform parent, Model.Space.Purchasable property)
    {
        ManagePurchasable PopUp;
        if(property is Model.Space.Property)
            {
                PopUp = Instantiate(Asset.ManagePropertyPopUpPrefab,parent).GetComponent<ManagePurchasable>();
                game_controller controller = FindObjectOfType<game_controller>(); 
                PopUp.sellBtn.onClick.AddListener(() => PopUp.sellPropertyOption(property.owner.SellProperty(property,controller.board_model),controller.board_view.squares[property.position-1]));
                PopUp.buyHouseBtn.onClick.AddListener(() => PopUp.buyHouseOption(((Model.Space.Property)(property)).buyHouse(controller.board_model) ,(Model.Space.Property)property,((View.PropertySquare)(controller.board_view.squares[property.position-1]))));
                PopUp.sellHouseBtn.onClick.AddListener(() => PopUp.sellHouseOption(((Model.Space.Property)(property)).sellHouse(controller.board_model) ,(Model.Space.Property)property,((View.PropertySquare)(controller.board_view.squares[property.position-1]))));
                if(property.isMortgaged) {
                    PopUp.mortgageBtn.onClick.AddListener(() => PopUp.mortgagePropertyOption(property.pay_off_mortgage(),property));
                    PopUp.mortgageBtn.GetComponentInChildren<TMPro.TMP_Text>().SetText("Pay Off Mortgage");
                } else {
                    PopUp.mortgageBtn.onClick.AddListener(() => PopUp.mortgagePropertyOption(property.mortgage(),property));
                    PopUp.mortgageBtn.GetComponentInChildren<TMPro.TMP_Text>().SetText("Mortgage");
                }
            } else {
                PopUp = Instantiate(Asset.ManageUtilityPopUpPrefab,parent).GetComponent<ManagePurchasable>();
                game_controller controller = FindObjectOfType<game_controller>(); 
                PopUp.sellBtn.onClick.AddListener(() => PopUp.sellPropertyOption(property.owner.SellProperty(property,controller.board_model),((View.UtilitySquare)(controller.board_view.squares[property.position-1]))));
                if(property.isMortgaged) {
                    PopUp.mortgageBtn.onClick.AddListener(() => PopUp.mortgagePropertyOption(property.pay_off_mortgage(),property));
                    PopUp.mortgageBtn.GetComponentInChildren<TMPro.TMP_Text>().SetText("Pay Off Mortgage");
                } else {
                    PopUp.mortgageBtn.onClick.AddListener(() => PopUp.mortgagePropertyOption(property.mortgage(),property));
                    PopUp.mortgageBtn.GetComponentInChildren<TMPro.TMP_Text>().SetText("Mortgage");
                }
            }
        return PopUp;
    }

    public void buyHouseOption(Model.Decision_outcome decision, Model.Space.Property space, View.PropertySquare square)
    {
        switch(decision) //((Model.Space.Property)(card.property)).buyHouse(FindObjectOfType<temp_contr>().board_model)
        {
            case Model.Decision_outcome.NOT_ALL_PROPERTIES_IN_GROUP:
                MessagePopUp.Create(transform.parent, "You need to own all the properties of this colour first!");
            break;
            case Model.Decision_outcome.DIFFERENCE_IN_HOUSES:
                MessagePopUp.Create(transform.parent, "The difference in number of houses on properties of the same colour cannot be bigger than one! Develop other properties of this colour!");
            break;
            case Model.Decision_outcome.MAX_HOUSES:
                MessagePopUp.Create(transform.parent, "Maximum number of houses reached!");
                
            break;
            case Model.Decision_outcome.NOT_ENOUGH_MONEY:
                MessagePopUp.Create(transform.parent, "You have not enough money! Sell or mortgage your properties to get some cash!");
            break;
            case Model.Decision_outcome.SUCCESSFUL:
                transform.parent.GetComponent<View.PropertyCard>().showHouse(space.noOfHouses);
                square.addHouse();
                MessagePopUp.Create(transform.parent, "House bought!");
                soundManager.PlayPurchaseSound();
            break;
        }
        Destroy(gameObject);
    }
    public void sellHouseOption(Model.Decision_outcome decision, Model.Space.Property space, View.PropertySquare square)
    {
        switch(decision)
        {
            case Model.Decision_outcome.DIFFERENCE_IN_HOUSES:
                MessagePopUp.Create(transform.parent, "The difference in number of houses on properties of the same colour cannot be bigger than one! Sell houses on other properties of this colour first!");
            break;
            case Model.Decision_outcome.NO_HOUSES:
                MessagePopUp.Create(transform.parent, "There are no more houses to sell!");
            break;
            case Model.Decision_outcome.SUCCESSFUL:
                transform.parent.GetComponent<View.PropertyCard>().showHouse(space.noOfHouses);
                square.removeHouse();
                MessagePopUp.Create(transform.parent, "House sold!");
                soundManager.PlayIncomeSound();
            break;
        }
        Destroy(gameObject);
    }
    public void sellPropertyOption(Model.Decision_outcome desicision, View.Square square)
    {
        switch(desicision)
        {
            case Model.Decision_outcome.DIFFERENCE_IN_HOUSES:
                MessagePopUp.Create(transform.parent, "First sell all the houses on the properties of this colour!");
            break;
            case Model.Decision_outcome.SUCCESSFUL:
                if(square is View.PropertySquare) { ((View.PropertySquare)(square)).removeRibbon(); } else { ((View.UtilitySquare)(square)).removeRibbon(); }
                transform.parent.gameObject.SetActive(false);
                MessagePopUp.Create(transform.parent.parent, "Property sold!");
                soundManager.PlayIncomeSound();
            break;
        }
        Destroy(gameObject);
    }
    public void mortgagePropertyOption(Model.Decision_outcome decision, Model.Space.Purchasable property)
    {
        switch (decision)
        {
            case Model.Decision_outcome.NO_HOUSES:
                MessagePopUp.Create(transform.parent, "You can't mortgage property with houses on it!");
            break;
            case Model.Decision_outcome.NOT_ENOUGH_MONEY:
                MessagePopUp.Create(transform.parent, "You have not enough money! Sell or mortgage your properties to get some cash!");
            break;
            case Model.Decision_outcome.SUCCESSFUL:
                if (property.isMortgaged)
                {
                    MessagePopUp.Create(transform.parent, "Property mortgaged!"); 
                    soundManager.PlayIncomeSound();
                }
                else
                {
                    MessagePopUp.Create(transform.parent, "Property paid off!");
                    soundManager.PlayPurchaseSound();
                }
                transform.parent.Find("Mortgaged").gameObject.SetActive(property.isMortgaged);
            break;
        }
        
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        FindObjectOfType<View.HUD>().UpdateInfo(FindObjectOfType<game_controller>());
    }
}
}
