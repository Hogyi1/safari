using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ActionBarManager : MonoBehaviour
{
    [Header("TextFieldsForBottomBar")]
    public TMP_Text mony;
    public Image monyUpOrDownIcon;
    public TMP_Text visitorCount;
    public Image visitorsUpOrDownIcon;
    public TMP_Text Date;

    public TMP_Text animalCount;
    public Image animalUpOrDownIcon;
    [Header("ButtonsForBottomBar")]
    public Button inventory;
    public Button shop;
    public Button services;
    public Button stopTime;
    public Button startTime;
    public Button ForwardTime;

    [Header("SpritesForBottomBar")]
    public Sprite upImage;
    public Sprite downImage;
    public Sprite forwardImage1;
    public Sprite forwardImage2;
    public Sprite forwardImage3;


}
