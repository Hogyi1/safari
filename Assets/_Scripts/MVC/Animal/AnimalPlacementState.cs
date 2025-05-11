using UnityEngine;

/// <summary>
/// Handles the logic for placing animals on the terrain.
/// Displays a decal preview and spawns an animal if the location is valid.
/// </summary>
public class AnimalPlacementState : IBuildingState
{
    private AnimalPreviewSystem previewSystem;
    private AnimalType animalType;

    /// <summary>
    /// Initializes the placement state and starts the animal preview.
    /// </summary>
    /// <param name="animalType">The type of animal being placed.</param>
    /// <param name="previewSystem">Reference to the animal preview system.</param>
    public AnimalPlacementState(AnimalType animalType, AnimalPreviewSystem previewSystem)
    {
        this.animalType = animalType;
        this.previewSystem = previewSystem;

        previewSystem.StartShowingPreview();
    }

    /// <summary>
    /// Ends the placement mode and hides the preview.
    /// </summary>
    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

    /// <summary>
    /// Called every frame while in placement mode.
    /// (PreviewSystem already handles position update internally)
    /// </summary>
    /// <param name="mousePosition">World position under the mouse cursor.</param>
    public void UpdateState(Vector3 mousePosition)
    {
        // No need to update manually – previewSystem handles this internally
    }

    /// <summary>
    /// Attempts to place an animal at the selected position.
    /// </summary>
    /// <param name="mousePosition">World position where the player clicked.</param>
    public void OnAction(Vector3 mousePosition)
    {
        float terrainY = Terrain.activeTerrain.SampleHeight(mousePosition);
        Vector3 spawnPosition = new Vector3(mousePosition.x, terrainY, mousePosition.z);

        if (previewSystem == null || !previewSystem.CheckValidity(new Vector3(mousePosition.x, 0, mousePosition.z)))
            return;

        AnimalManager.Instance.SpawnAnimal(animalType, spawnPosition, 5);
    }
}
