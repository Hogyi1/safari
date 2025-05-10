using System;
using System.Collections.Generic;
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
    /// <param name="Type">Az állat típusa (enum).</param>
    /// <param name="position">Világpozíció, ahová az állat kerül.</param>
    /// <param name="ID">Egyedi azonosító.</param>
    /// <param name="Age">Életkor.</param>
    /// <returns>Az elkészült Animal példány, vagy null, ha nem található adat hozzá.</returns>
    public Animal CreateAnimal(AnimalType Type, Vector3 position, int ID, int Age)
    {
        AnimalData data = FindAnimalData(Type);

        if (data == null) return null;

        GameObject go = Instantiate(data.AnimalPrefab, position, Quaternion.identity);

        AnimalStateMachine sm = go.GetComponent<AnimalStateMachine>();
        AnimalView view = go.GetComponent<AnimalView>();
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
}
