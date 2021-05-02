using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class build : MonoBehaviour
{
    public GameObject sceneCamera;
    public Tilemap buildTiles;
    public Tilemap wallTiles;
    public Tilemap highlightTiles;
    public Tilemap defencePreviewTiles;
    public Tilemap defenceTiles;
    public Tilemap unbuildableTiles;
    public TileBase highlightTileGreen;
    public TileBase highlightTileRed;
    public bool buildModeEnabled = false;
    public bool sellModeEnabled = false;
    private Vector3Int currentTile;
    private bool canBuildOnTile;
    private Defence currentDefenceType;
    public Dictionary<Vector3, dynamic> globalGameInstances = new Dictionary<Vector3, dynamic>();
    public static build Controller { get; set; }

    void Awake()
    {
        if (Controller != null) {
            return;
        }

        Controller = this;
    }

    void Update() {
        if(buildModeEnabled == true){
            checkTile();

            if(Input.GetMouseButtonDown(0) && canBuildOnTile){
                defenceTiles.SetTile(currentTile, currentDefenceType.tile);
                var newGameInstance = currentDefenceType._onBuild(currentTile, defenceTiles.CellToWorld(currentTile), wallTiles.HasTile(currentTile));
                globalGameInstances.Add(newGameInstance.worldPos, newGameInstance);
            }
        }
        if(sellModeEnabled == true){

        }
    }

    public void removeDefenceTile(Vector3 destroyedPos){
        var destroyedTile = buildTiles.WorldToCell(destroyedPos);
        defenceTiles.SetTile(destroyedTile, null);
        globalGameInstances.Remove(destroyedPos);
    }

    public void toggleBuildMode(Defence defenceType){
        if(defenceType != currentDefenceType){
            currentDefenceType = defenceType;
            buildModeEnabled = true;
        } else {
            buildModeEnabled = !buildModeEnabled;
        }
        if(buildModeEnabled == false){
            defencePreviewTiles.SetTile(currentTile, null);
            highlightTiles.SetTile(currentTile, null);
        }
        // Just big and far away so it will update if we select a different defence
        currentTile = new Vector3Int(999,999,999);
    }

    bool tileUnoccupied(Vector3Int tilePos){
        bool canBuild = !defenceTiles.HasTile(tilePos);
        return canBuild;
    }

    bool tileBuildable(Vector3Int tilePos){
        bool canBuild;
        if(currentDefenceType.buildsOnWalls == true){
            canBuild = !(wallTiles.HasTile(tilePos) || unbuildableTiles.HasTile(tilePos)) || buildTiles.HasTile(tilePos);
        } else {
            canBuild = !(wallTiles.HasTile(tilePos) || unbuildableTiles.HasTile(tilePos));
        }
        return canBuild;
    }

    // Checks if a tile can be built on
    void checkTile(){
        var mousePos = Input.mousePosition;
        mousePos.z = -(sceneCamera.transform.position.z);
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePos);
        // mouseWorldPos.x += 0.5f;
        // mouseWorldPos.y += 0.5f;
        Vector3Int tilePos = buildTiles.WorldToCell(mouseWorldPos);
        if(tilePos != currentTile){
            // Check again as is new tile
            highlightTiles.SetTile(currentTile, null);
            if((tileBuildable(tilePos)) && (tileUnoccupied(tilePos))){
                // Can build
                canBuildOnTile = true;
                highlightTiles.SetTile(tilePos,highlightTileGreen);
            } else {
                canBuildOnTile = false;
                highlightTiles.SetTile(tilePos,highlightTileRed);
            }
            defencePreviewTiles.SetTile(currentTile, null);
            defencePreviewTiles.SetTile(tilePos, currentDefenceType.tile);
        }
        currentTile = tilePos;
    }
}