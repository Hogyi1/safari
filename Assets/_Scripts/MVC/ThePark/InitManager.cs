using UnityEngine;

public class InitManager : MonoBehaviour
{
    public static InitManager Instance { get; private set; }

    [SerializeField]
    private string filename;
    public Inventory inventory;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            inventory = new Inventory();
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
