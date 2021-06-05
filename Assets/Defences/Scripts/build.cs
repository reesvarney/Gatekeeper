using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;

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
    public TileBase sellTileRed;
    public TileBase sellTileGreen;
    public TileBase upgradeTileRed;
    public TileBase upgradeTileGreen;
    public TileBase repairTileRed;
    public TileBase repairTileGreen;
    public enum Mode{
        Build,
        Sell,
        Upgrade,
        Repair,
        None
    }
    public Mode currentMode = Mode.None;
    private Vector3Int currentTile;
    private bool canBuildOnTile;
    private Defence currentDefenceType;
    public Dictionary<Vector3, dynamic> globalGameInstances = new Dictionary<Vector3, dynamic>();
    private Dictionary<Mode, TileBase> modeHighlightsGreen = new Dictionary<Mode, TileBase>();
    private Dictionary<Mode, TileBase> modeHighlightsRed = new Dictionary<Mode, TileBase>();
    public static build Controller { get; set; }

    public GameObject costOverlay;
    public TextMeshProUGUI costText;

    public int upgradeCost = 100;
    public int repairCost = 50;

    void Awake()
    {
        modeHighlightsGreen.Add(Mode.Sell, sellTileGreen);
        modeHighlightsGreen.Add(Mode.Upgrade, upgradeTileGreen);
        modeHighlightsGreen.Add(Mode.Repair, repairTileGreen);

        modeHighlightsRed.Add(Mode.Sell, sellTileRed);
        modeHighlightsRed.Add(Mode.Upgrade, upgradeTileRed);
        modeHighlightsRed.Add(Mode.Repair, repairTileRed);

        costOverlay.SetActive(false);

        if (Controller != null) {
            return;
        }

        Controller = this;
    }

    void Update() {
        if(!gameController.isPaused){
        if(currentMode != Mode.None){
            checkTile();

            if(currentMode == Mode.Build){
                if(Input.GetMouseButtonDown(0) && canBuildOnTile && !globalGameInstances.ContainsKey(currentTile)){
                    defenceTiles.SetTile(currentTile, currentDefenceType.tile);
                    var magicLevel = GetComponent<playerMagic>().spend(currentDefenceType.cost);
                    var newGameInstance = currentDefenceType._onBuild(currentTile, defenceTiles.CellToWorld(currentTile), wallTiles.HasTile(currentTile));
                    globalGameInstances.Add(currentTile, newGameInstance);
                    if(magicLevel < currentDefenceType.cost){
                        currentMode = Mode.None;
                        setBuildMode();
                    }
                }
            } else if(currentMode == Mode.Sell){
                if(Input.GetMouseButtonDown(0) && globalGameInstances.ContainsKey(currentTile)){
                    defenceTiles.SetTile(currentTile, null);
                    var currentInstance = globalGameInstances[currentTile];
                    currentInstance.sell();
                    highlightTiles.SetTile(currentTile, null);
                    currentTile = new Vector3Int(999,999,999);
                }
            } else if(currentMode == Mode.Upgrade){
                if(Input.GetMouseButtonDown(0) && globalGameInstances.ContainsKey(currentTile) && GetComponent<playerMagic>().level > upgradeCost){
                    GetComponent<playerMagic>().spend(upgradeCost);
                    var currentInstance = globalGameInstances[currentTile];
                    currentInstance.upgrade();
                    highlightTiles.SetTile(currentTile, null);
                    currentTile = new Vector3Int(999,999,999);
                }
            } else if(currentMode == Mode.Repair){
                if(Input.GetMouseButtonDown(0) && globalGameInstances.ContainsKey(currentTile) && GetComponent<playerMagic>().level > repairCost){
                    GetComponent<playerMagic>().spend(repairCost);
                    var currentInstance = globalGameInstances[currentTile];
                    currentInstance.repair();
                    highlightTiles.SetTile(currentTile, null);
                    currentTile = new Vector3Int(999,999,999);
                }
            }
        }


        if (Input.GetKeyDown("r")){
            toggleSellMode();
        }

        if (Input.GetKeyDown("e")){
            toggleUpgradeMode();
        }

        if (Input.GetKeyDown("q")){
            toggleRepairMode();
        }
        }
    }

    public void removeDefenceTile(Vector3 destroyedPos){
        var destroyedTile = buildTiles.WorldToCell(destroyedPos);
        defenceTiles.SetTile(destroyedTile, null);
        globalGameInstances.Remove(destroyedPos);
    }

    public void toggleSellMode(){
        if(currentMode == Mode.Sell){
            currentMode = Mode.None;
        } else {
            currentMode = Mode.Sell;
        }
        setBuildMode();
    }

    public void toggleRepairMode(){
        if(currentMode == Mode.Repair){
            currentMode = Mode.None;
        } else {
            currentMode = Mode.Repair;
        }
        setBuildMode();
    }

    public void toggleUpgradeMode(){
        if(currentMode == Mode.Upgrade){
            currentMode = Mode.None;
        } else {
            currentMode = Mode.Upgrade;
        }
        setBuildMode();
    }

    public void toggleBuildMode(Defence defenceType){
        if(GetComponent<playerMagic>().canAfford(defenceType.cost)){
            if(defenceType != currentDefenceType){
                currentDefenceType = defenceType;
                currentMode = Mode.Build;
            } else {
                if(currentMode == Mode.Build){
                    currentMode = Mode.None;
                } else {
                    currentMode = Mode.Build;
                }
            }
        } else {
            currentMode = Mode.None;
        }

        setBuildMode();
    }

    public void setBuildMode(){
        if(currentMode == Mode.None){
            defencePreviewTiles.SetTile(currentTile, null);
            highlightTiles.SetTile(currentTile, null);
            costOverlay.SetActive(false);
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
        Vector3Int tilePos = buildTiles.WorldToCell(mouseWorldPos);
        if(tilePos != currentTile){
            if(currentMode == Mode.Build){
                costOverlay.SetActive(false);
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
            } else if(currentMode == Mode.Sell || currentMode == Mode.Repair || currentMode == Mode.Upgrade) {
                defencePreviewTiles.SetTile(currentTile, null);
                highlightTiles.SetTile(currentTile, null);
                var modeCost = 0;
                var otherFactor = false;
                if(currentMode == Mode.Repair){
                    modeCost = repairCost;
                    if(globalGameInstances.ContainsKey(tilePos) && !(globalGameInstances[tilePos].health < globalGameInstances[tilePos].levelHealth)){
                        otherFactor = true;
                    }
                } else if(currentMode == Mode.Upgrade){
                    modeCost = upgradeCost;
                }
                if(!tileUnoccupied(tilePos) && currentMode != Mode.Sell){
                    costOverlay.SetActive(true);
                    costOverlay.transform.position = tilePos;
                    costText.text = modeCost.ToString();
                } else {
                    costOverlay.SetActive(false);
                }
                if(!tileUnoccupied(tilePos) && GetComponent<playerMagic>().canAfford(modeCost) && !otherFactor){
                    highlightTiles.SetTile(tilePos, modeHighlightsGreen[currentMode]);
                } else {
                    highlightTiles.SetTile(tilePos, modeHighlightsRed[currentMode]);
                }
            }
        }
        currentTile = tilePos;
    }
}