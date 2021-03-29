using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Inventory : MonoBehaviour
{

    private PlayerScript player;
    private Image inventoryUI;

    public List<Item> ListInventory = new List<Item>();
    public List<Slot> ListSlots = new List<Slot>();
    
    GameObject backpack;
    GameObject teddybackpack;
    bool readyToPickUpBackpack;
    bool GotTeddy;
    bool GotBackPack;

    void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // called second
    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        player = GameObject.Find("Player").GetComponent<PlayerScript>();
        inventoryUI = GameObject.Find("InventoryUI").GetComponent<Image>();

        inventoryUI.gameObject.SetActive(false);

        backpack = GameObject.Find("Backpack1");
        teddybackpack = GameObject.Find("backpack2");

        initializeSlots();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        backpackTeddy();
        toggleInventory();
        updateSlots();
        updateItems();
    }

    //initialise the inventory with all 12 slots
    private void initializeSlots() {
        ListSlots.Clear();
        for (int i = 0; i < inventoryUI.transform.childCount; i++) {
            ListSlots.Add(inventoryUI.transform.GetChild(i).GetComponent<Slot>());
            ListSlots[i].setIndex(i);
        }     
    }

    private void updateSlots() {
        //insert the item in the inventory slot
        

        for(int i = 0; i < ListInventory.Count; i++) {
            if (ListSlots[i].getItem() == null) {
                ListSlots[i].setItem(ListInventory[i]);
            }
        }
        //remove the item sprite from the inventory slot when removing item
        for (int i = 0; i < ListSlots.Count; i++) {
            if (ListSlots[i].getItem() == null) {
                ListSlots[i].setSpriteRender(null);
            }
        }
    }

    
    private void updateItems() {
        //if using consumable item, remove it
        for(int i = 0; i < ListInventory.Count; i++) {
            if(ListInventory[i].getItemUsed() && ListInventory[i].getIsConsumable()) {
                removeItem(i);
            }
        }
    }
    
    //add item to the inventory
    public void addItem(Item item) {
        ListInventory.Add(item);
    }

    //remove item from inventory
    public void removeItem(int index) {
        ListInventory.RemoveAt(index);
        ListSlots[index].setItem(null);
        for(int i = 0; i < ListSlots.Count-1; i++) {
            if(ListSlots[i].getItem() == null && ListSlots[i+1].getItem() != null) {
                ListSlots[i].setItem(ListSlots[i + 1].getItem());
                ListSlots[i + 1].setItem(null);
            }
        }
    }


    //toggle inventory
    private void toggleInventory() {
        //if inventory is open, pause game and show mouse 
        if (player.getOpenInventory()) {
            Cursor.lockState = CursorLockMode.Confined;
            inventoryUI.gameObject.SetActive(true);
            Time.timeScale = 0f;
        }
        else if (!player.getOpenInventory()) {
            inventoryUI.gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1;
        }
    }



    private void backpackTeddy() {
        if (backpack == null) {
            return;
        }
        if (teddybackpack == null) {
            return;
        }

        if (getGotBackPack()) {
            backpack.SetActive(true);
            if (getGotTeddy()) {
                backpack.SetActive(false);
                teddybackpack.SetActive(true);

            }
        }
        else {
            backpack.SetActive(false);
            teddybackpack.SetActive(false);
        }
    }


    //Getters and Setters
    public bool getGotTeddy() {
        return GotTeddy;
    }
    public void setGotTeddy(bool value) {
        GotTeddy = value;
    }
    public bool getGotBackPack() {
        return GotBackPack;
    }
    public void setGotBackPack(bool value) {
        GotBackPack = value;
    }
    public bool getReadyToPickUpBackpack() {
        return readyToPickUpBackpack;
    }
    public void setReadyToPickUpBackpack(bool value) {
        readyToPickUpBackpack = value;
    }
}
