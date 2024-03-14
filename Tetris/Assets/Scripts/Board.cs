using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class Board : MonoBehaviour
{
    public GameObject restartScene;
    public Tilemap tilemap { get; private set; }
    public Piece activePiece { get; private set; }

    public TetrominoData[] tetrominoes;
    public Vector2Int boardSize = new Vector2Int(10, 20); //10 a 20 lik board
    public Vector3Int spawnPosition = new Vector3Int(-1, 8, 0); //nerede spawnlanacagini secmek icin.

    public RectInt Bounds
    {
        get
        {
            Vector2Int position = new Vector2Int(-boardSize.x / 2, -boardSize.y / 2);
            return new RectInt(position, boardSize);
        }
    }
    public void restartButton()
    {
        SceneManager.LoadScene("Game");
    }
    private void Awake()//bir script etkinleþtirildiði anda cagirilan ozel bir method.
    {
        tilemap = GetComponentInChildren<Tilemap>();
        activePiece = GetComponentInChildren<Piece>();

        for (int i = 0; i < tetrominoes.Length; i++)
        {
            tetrominoes[i].Initialize();
        }
    }

    private void Start()
    {
        SpawnPiece();
    }

    public void SpawnPiece()
    {
        int random = Random.Range(0, tetrominoes.Length);//rastgele index üretmek icin.

        TetrominoData data = tetrominoes[random];

        activePiece.Initialize(this, spawnPosition, data);

        if (IsValidPosition(activePiece, spawnPosition))
        {
            Set(activePiece);
        }
        else
        {
            GameOver();
        }
    }

    public void GameOver()
    {
        tilemap.ClearAllTiles();
        restartScene.SetActive(true);
        
    }

    public void Set(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, piece.data.tile); // tam nerede oldugunu konumlandirdik.

        }
    }

    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, null);
        }
    }

    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        RectInt bounds = Bounds;

        
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + position; // her bagimsiz cell icin tile position



            if (!bounds.Contains((Vector2Int)tilePosition)) //returns true if given position is true;

            {
                return false;
            }

       
            if (tilemap.HasTile(tilePosition)) //invalid position orada zaten tile oldugunu gosterir.

            {
                return false;
            }
        }

        return true;
    }

    public void ClearLines()  //her bir karede tile var ise o yatay koseyi sifirlamak icin.

    {
        RectInt bounds = Bounds;
        int row = bounds.yMin;

        
        while (row < bounds.yMax) //bastan sona kontrol etmemiz icin.

        {


            if (IsLineFull(row))
            {
                LineClear(row);
            }
            else
            {
                row++;
            }
        }
    }

    public bool IsLineFull(int row)
    {
        RectInt bounds = Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);

            
            if (!tilemap.HasTile(position))  //o pozisyonda tile var mi?

            {
                return false;
            }
        }

        return true;
    }

    public void LineClear(int row)
    {
        RectInt bounds = Bounds;

       
        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);
            tilemap.SetTile(position, null);
        }

       
        while (row < bounds.yMax)
        {
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row + 1, 0); //row +1 olmasinin sebebi bir asagisindakine erismek

                TileBase above = tilemap.GetTile(position);//Bir sonraki satýrdaki karoyu above adlý bir deðiþkene kaydeder.


                position = new Vector3Int(col, row, 0);
                tilemap.SetTile(position, above);
            }

            row++;
        }
    }

}