using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Random = UnityEngine.Random;
using static SwipeMerge.Constants;

namespace SwipeMerge
{
    public partial class GameManager : MonoBehaviour
    {
        private static Transform root;

        // 생성될 타일이 가질 값의 범위를 지정합니다. (사용되는 값이 2를 밑으로 하는 수이므로, 지수의 범위로 표현합니다)
        private const int spawnValueExpMin = 1;
        private const int spawnValueExpMax = 2;
        
        // 게임이 시작되었을 때 생성할 타일의 수입니다.
        [SerializeField] private int spawnOnInit = 2;

        private Tile[] tiles = new Tile[CellPerLine * CellPerLine];
        private int shiftedTiles;
        
        private void UpdateInput()
        {
            if (!this.isPlaying) return;

            var hasShiftTried = false;
            this.shiftedTiles = 0;
            
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                this.ShiftAll();
                this.UpdatePosition();
                hasShiftTried = true;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                this.Rotate(2);
                this.ShiftAll();
                this.Rotate(2);
                this.UpdatePosition();
                hasShiftTried = true;
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                this.Rotate(1);
                this.ShiftAll();
                this.Rotate(3);
                this.UpdatePosition();
                hasShiftTried = true;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                this.Rotate(3);
                this.ShiftAll();
                this.Rotate(1);
                this.UpdatePosition();
                hasShiftTried = true;
            }

            // 키 입력을 했더라도 빈 공간이 있는 상태에서 고의로 옮겨진 타일이 없도록 움직이려고 했다면 타일을 만들지 않습니다.
            if (!hasShiftTried || (TryGetEmptySpace(out _, out _) && this.shiftedTiles == 0))
            {
                return;
            }
            this.Spawn();
        }

        private void UpdatePosition()
        {
            for (var y = 0; y < CellPerLine; y++)
            {
                for (var x = 0; x < CellPerLine; x++)
                {
                    var idx = ToIndex(x, y);
                    if (!this.tiles[idx]) continue;
                    this.tiles[idx].transform.position = CellManager.Instance.GetPosition(x, y);
                }
            }
        }

        // 타일을 밀어내는 함수를 방향벌로 작성하는 방법도 있지만, 저는 배열을 회전하면서 한 방향으로만 밀어내는 방식을 선택했습니다.
        // 성능이 더 좋더라도 비슷하게 생긴 코드가 여러 벌 존재하는 게 제 스타일이 아니기 때문입니다.
        private void Rotate(int count)
        {
            var i = 0;
            
            while (i < count)
            {
                i++;
                var clone = this.tiles.ToArray();

                for (var y = 0; y < CellPerLine; y++)
                {
                    for (var x = 0; x < CellPerLine; x++)
                    {
                        this.tiles[ToIndex(x, y)] = clone[ToIndex(CellPerLine - 1 - y, x)];
                    }
                }
            }
        }

        // 왼쪽에서 오른쪽으로, 위에서 아래로 탐색하면서 타일을 왼쪽으로 밀어냅니다.
        // 밀어내다 막히는 곳 (벽)과 밀어낼 타일의 숫자가 같다면 하나의 타일로 합칩니다.
        private void ShiftAll()
        {
            var result = new Tile[CellPerLine * CellPerLine];
            var mergedTiles = new HashSet<int>();
            
            for (var y = 0; y < CellPerLine; y++)
            {
                var wallX = -1;
                for (var x = 0; x < CellPerLine; x++)
                {
                    if (!this.tiles[ToIndex(x, y)]) continue;
                    var tile = this.tiles[ToIndex(x, y)];

                    // 타일을 밀었을 때 같은 숫자와 부딪힌다면 합칩니다. (합쳐질 때 이번에 옮긴 타일은 사라지니 배열에 반영하지 않고 넘어갑니다) 
                    if (-1 < wallX)
                    {
                        var mergeTarget = result[ToIndex(wallX, y)];
                        if (mergeTarget != null
                            &&!mergedTiles.Contains(mergeTarget.GetInstanceID())
                            && mergeTarget.Value == tile.Value)
                        {
                            mergeTarget.SetValue(tile.Value * 2);
                            Destroy(tile.gameObject);
                            mergedTiles.Add(mergeTarget.GetInstanceID());
                            this.shiftedTiles++;
                            continue;
                        }
                    }

                    // 합쳐지지 않았다면 배열에 타일을 반영합니다.
                    result[ToIndex(++wallX, y)] = tile;

                    // 이전 좌표와 현재 좌표가 다르면 이동한 타일로 카운트합니다.
                    if (x != wallX)
                    {
                        this.shiftedTiles++;
                    }
                }
            }

            this.tiles = result;
        }

        // 빈 곳에 타일을 새로 만듭니다.
        // 빈 곳이 없어 새 타일을 만들 수 없다면 게임 오버입니다.
        private void Spawn()
        {
            if (!TryGetEmptySpace(out var x, out var y))
            {
                this.gameOverPanel.SetActive(true);
                this.isPlaying = false;
                return;
            }

            root ??= GameObject.FindWithTag("Root").transform;
            var tile = Instantiate(Resources.Load<GameObject>("SwipeMerge/Tile"), root).GetComponent<Tile>();
            var value = (int)Math.Pow(2, Random.Range(spawnValueExpMin, spawnValueExpMax + 1));
            tile.transform.position = CellManager.Instance.GetPosition(x, y);
            this.tiles[ToIndex(x, y)] = tile;
            tile.SetValue(value);
        }
        
        // LINQ를 사용해 빈 곳의 인덱스를 찾고 그 중 무작위로 하나를 전달합니다.
        // 더 좋은 방법도 많겠지만, LINQ는 C#에 내장된 유용한 기능이므로 보여드리고 싶어서 이 방법으로 진행했습니다.
        private bool TryGetEmptySpace(out int x, out int y)
        {
            var emptyIndices = Enumerable.Range(0, CellPerLine * CellPerLine)
                .Where(idx => !this.tiles[idx])
                .OrderBy(_ => Random.Range(0, 100))
                .ToArray();

            if (emptyIndices.Length > 0)
            {
                if (this.tiles[emptyIndices[0]])
                {
                    Debug.LogError("????");
                }
                (x,y) = ToPoint(emptyIndices[0]);
                return true;
            }
            
            x = 0;
            y = 0;
            return false;
        }
    }
}
