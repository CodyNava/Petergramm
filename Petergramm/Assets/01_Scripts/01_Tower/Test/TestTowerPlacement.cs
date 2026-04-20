using System.Collections.Generic;
using _01_Scripts._02_Grid.GridData;
using NaughtyAttributes;
using UnityEngine;

namespace _01_Scripts._01_Tower.Test
{
   public class TestTowerPlacement : MonoBehaviour
   {
    
      [SerializeField] private List<GameObject> towerPrefab = new();
      [SerializeField] private GridData gridData;

      
      //TODO Change logik to click on tile and place x tower 
      //(gridData.TryGetValue(COORD, out GridTileData tile) && !tile.isOccupied)
      //would be how you get the right tile - COORD needs to be the location you clicked on
      //or use a drag and drop methode where you dont need to click on a tile rather
      //add the tower to the dict by the coord where the tower was placed.
      
      [Button]
      private void TestTowerPlacing()
      {
         Dictionary<Vector3, GridTileData> gridCoords = gridData.PlacementCoords;

         foreach (KeyValuePair<Vector3, GridTileData> pair in gridCoords)
         {
            if (pair.Value.isOccupied) return;
            GameObject spawnedTower = Instantiate(towerPrefab[0], pair.Key, Quaternion.identity);   
            
            pair.Value.isOccupied = true;
            pair.Value.occupant = spawnedTower;
         } 
      }

      //TODO Change logik to click on tile and Sell/destroy x tower 
      [Button]
      private void DeleteAllTower()
      {
         
         Dictionary<Vector3, GridTileData> gridCoords = gridData.PlacementCoords;

         foreach (KeyValuePair<Vector3, GridTileData> pair in gridCoords)
         {
            if (!pair.Value.isOccupied) return;
            
            Destroy(pair.Value.occupant.gameObject);
            pair.Value.isOccupied = false;
            pair.Value.occupant = null;
         } 
      }
      
      
   }
}
