using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    [HideInInspector] public GridObjectID ID;
    
    [HideInInspector] public bool Selected { get; private set; }
    [HideInInspector] public Button ShopItemButton;

    [SerializeField] private Image BackGround;
    [SerializeField] private Sprite NormalBackGround, SelectedBackGround;    
    [SerializeField] private Image PreviewImage;

    private IDManager IDmanager;
    private BuildManager buildManager;

    public void OnClick()
    {
        buildManager.SetObjectToBuild(IDmanager.GetData(ID));
    }


    public void UpdateSelected()
    {
        SetSelect(buildManager.GridObjectToBuild.ID == ID);
    }


    void SetSelect(bool value)
    {
        Selected = value;
        if (Selected)
            BackGround.sprite = SelectedBackGround;
        else
            BackGround.sprite = NormalBackGround;
    }


    void Start()
    {
        IDmanager = IDManager.Instance;
        buildManager = BuildManager.Instance;

        PreviewImage.sprite = IDmanager.GetData(ID).SpritePreview;

        ShopItemButton.onClick.AddListener( delegate {  OnClick(); });
        buildManager.OnGridObjectToBuildChanged += delegate { UpdateSelected(); };
    }

    public ShopItem(GridObjectID _ID)
    {
        ID = _ID;
    }





}
