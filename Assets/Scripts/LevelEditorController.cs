using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

[ExecuteInEditMode]
public class LevelEditorController : MonoBehaviour
{
    public GameObject BasicTile;
    public int TileSize = 10;
    public int Length;
    public int Width;
    public bool GenerateTiles = false;
    public bool SaveTiles = false;
    public bool DeleteTiles = false;

    [SerializeField]
    public GameObject[,] LoadedTiles;

    private void Update()
    {
        if(GenerateTiles)
        {
            GameObject root = new GameObject();
            root.name = "root";
            root.transform.parent = transform;
            LoadedTiles = new GameObject[Length, Width];

            for (int x = 0; x < Length; x++)
            {
                for (int y = 0; y < Width; y++)
                {
                    GameObject newlyInstantiatedTile = Instantiate(BasicTile, new Vector3(x * TileSize, 0, y * TileSize), new Quaternion(), root.transform);
                    newlyInstantiatedTile.name = string.Format("{0},{1}", x, y);
                    newlyInstantiatedTile.transform.parent = root.transform;
                    TileData tileData = ScriptableObject.CreateInstance<TileData>();
                    tileData.X = x;
                    tileData.Y = y;
                    tileData.Entities = new GameEntityData[0];
                    newlyInstantiatedTile.GetComponent<GameTile>().TileData = tileData;
                    LoadedTiles[x, y] = newlyInstantiatedTile;
                }
            }

            GenerateTiles = false;
        }

        if (SaveTiles)
        {
            AssetDatabase.CreateFolder("Assets/Data", "NewMapData");
            string guid = AssetDatabase.CreateFolder("Assets/Data/NewMapData", "TileData");
            string newFolderPath = AssetDatabase.GUIDToAssetPath(guid);

            MapData newMapData = ScriptableObject.CreateInstance<MapData>();
            AssetDatabase.CreateAsset(newMapData, "Assets/Data/NewMapData/NewMapData.asset");

            List<TileData> newTiles = new List<TileData>();
            //newMapData.MapTiles = new TileData[LoadedTiles.GetLength(0) & LoadedTiles.GetLength(1)];
            for (int x = 0; x < LoadedTiles.GetLength(0); x++)
            {
                for (int y = 0; y < LoadedTiles.GetLength(1); y++)
                {
                    TileData data = LoadedTiles[x, y].GetComponent<GameTile>().TileData;
                    AssetDatabase.CreateAsset(data, newFolderPath + "/tile" + x + y + ".asset");
                    newTiles.Add(data);
                }
            }

            newMapData.MapTiles = newTiles.ToArray();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = newMapData;

            SaveTiles = false;
        }

        if (DeleteTiles)
        {
            if(LoadedTiles != null)
            {
                foreach (GameObject g in LoadedTiles)
                {
                    DestroyImmediate(g);
                }
            }

            DeleteTiles = false;
        }
    }
}
