using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Felelős az állatok létrehozásáért a játékban.
/// Betölti az összes állat adatát (ScriptableObject),
/// és ezek alapján példányosítja az állatot, hozzárendelve:
/// - modellt (AnimalModel),
/// - nézetet (AnimalView),
/// - állapotgépet (AnimalStateMachine).
/// </summary>
public class AnimalFactory : MonoBehaviour
{
    // Az állat adatbázis (ScriptableObject-ből betöltve)
    private List<AnimalData> animalDatabase = new List<AnimalData>();
    [SerializeField] private GameObject animalParent;

    /// <summary>
    /// Inicializáláskor betölti az összes állatadatot a Resources/Animals mappából.
    /// </summary>
    void Start()
    {
        LoadAllAnimals();
    }

    /// <summary>
    /// Létrehoz egy új állatot a megadott típus, pozíció, ID és életkor alapján.
    /// Visszaad egy teljesen felépített Animal objektumot, ami tartalmazza a View, Model és StateMachine komponenseket.
    /// </summary>
    /// <param name="animalID">Az állat típusa.</param>
    /// <param name="position">Világpozíció, ahová az állat kerül.</param>
    /// <param name="ID">Egyedi azonosító.</param>
    /// <param name="Age">Életkor.</param>
    /// <returns>Az elkészült Animal példány, vagy null, ha nem található adat hozzá.</returns>
    public Animal CreateAnimal(AnimalType type, Vector3 position, int ID, int Age)
    {
        AnimalData data = FindAnimalData(type);
        if (data == null) return null;

        GameObject instance = Instantiate(data.AnimalPrefab, position, Quaternion.identity);
        instance.transform.SetParent(animalParent.transform, true);

        AnimalStateMachine sm = instance.GetComponent<AnimalStateMachine>();
        AnimalView view = instance.GetComponent<AnimalView>();
        AnimalModel model = new AnimalModel(data, ID, Age);

        return new Animal(ID, model, view, sm);
    }

    /// <summary>
    /// Megkeresi az állat típusához tartozó AnimalData-t az adatbázisban.
    /// </summary>
    private AnimalData FindAnimalData(AnimalType type)
    {
        return animalDatabase.Find(t => t.Type == type);
    }

    /// <summary>
    /// Betölti az összes AnimalData ScriptableObject-et a Resources/Animals mappából.
    /// </summary>
    private void LoadAllAnimals()
    {
        animalDatabase = new List<AnimalData>(Resources.LoadAll<AnimalData>("Animals"));
        Debug.Log($"Betöltve {animalDatabase.Count} állat.");
    }

    public AnimalType GetAnimalTypeByID(int ID)
    {
        return animalDatabase.Find(t => t.animalID == ID).Type;
    }
}
