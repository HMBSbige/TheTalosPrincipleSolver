using System;
using System.Linq;

namespace TheTalosPrincipleSolver
{
    public class PuzzleSolver
    {
        private const char I = 'i', O = 'o', T = 't', J = 'j', L = 'l', S = 's', Z = 'z';

        /// <summary>
        /// 行 x 列，每个 board 代表属于哪个 block（0 代表不属于任何 block）
        /// </summary>
        private readonly int[,] board;
        /// <summary>
        /// 放在板上的块
        /// </summary>
        private readonly char[] blocks;

        private int blocksPtr = 0;

        private readonly int nPieces;

        /// <summary>
        /// 构造函数，初始化
        /// </summary>
        /// <param name="height">行数</param>
        /// <param name="width">列数</param>
        /// <param name="iBlocks">I 形块的数量</param>
        /// <param name="oBlocks">2x2正方形方块的数量</param>
        /// <param name="tBlocks">T 形块的数量</param>
        /// <param name="jBlocks">J 形块的数量</param>
        /// <param name="lBlocks">L 形块的数量</param>
        /// <param name="sBlocks">S 形块的数量</param>
        /// <param name="zBlocks">Z 形块的数量</param>
        public PuzzleSolver(int height, int width, int iBlocks, int oBlocks, int tBlocks, int jBlocks, int lBlocks, int sBlocks, int zBlocks)
        {
            if (width <= 0 || height <= 0)
            {
                throw new ArgumentException(@"宽度和高度必须大于 0");
                
            }
            if (iBlocks < 0 || oBlocks < 0 || tBlocks < 0 || jBlocks < 0 || lBlocks < 0 || sBlocks < 0 || zBlocks < 0)
            {
                throw new ArgumentException(@"块个数必须大于等于 0");
            }
            board = new int[height,width];
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    board[y,x] = 0;
                }
            }
            nPieces = iBlocks + oBlocks + tBlocks + jBlocks + lBlocks + sBlocks + zBlocks;
            blocks = new char[nPieces];
            for (var i = 0; i < iBlocks; i++)
            {
                blocks[blocksPtr++] = I;
            }
            for (var i = 0; i < oBlocks; i++)
            {
                blocks[blocksPtr++] = O;
            }
            for (var i = 0; i < tBlocks; i++)
            {
                blocks[blocksPtr++] = T;
            }
            for (var i = 0; i < jBlocks; i++)
            {
                blocks[blocksPtr++] = J;
            }
            for (var i = 0; i < lBlocks; i++)
            {
                blocks[blocksPtr++] = L;
            }
            for (var i = 0; i < sBlocks; i++)
            {
                blocks[blocksPtr++] = S;
            }
            for (var i = 0; i < zBlocks; i++)
            {
                blocks[blocksPtr++] = Z;
            }
            blocksPtr = 0;
        }

        /// <summary>
        /// 计算一组相邻的空块有多大
        /// </summary>
        /// <param name="y">行数</param>
        /// <param name="x">列数</param>
        /// <returns>相邻的空块个数</returns>
        private int Group(int y, int x)
        {
            if (y >= 0 && y < board.GetLength(0) && x >= 0 && x < board.GetLength(1) && board[y,x] == 0)
            {
                board[y,x] = -1;
                return 1 + Group(y, x + 1) + Group(y, x - 1) + Group(y + 1, x) + Group(y - 1, x);
            }
            return 0;
        }

        private void ClearGroups()
        {
            for (int y = 0; y < board.GetLength(0); y++)
            {
                for (int x = 0; x < board.GetLength(1); x++)
                {
                    if (board[y,x] == -1)
                    {
                        board[y,x] = 0;
                    }
                }
            }
        }

        private bool IsStupidConfig()
        {
            for (int y = 0; y < board.GetLength(0); y++)
            {
                for (int x = 0; x < board.GetLength(1); x++)
                {
                    if (board[y,x] == 0)
                    {
                        if (Group(y, x) % 4 != 0)
                        {
                            ClearGroups();
                            return true;
                        }
                    }
                }
            }
            ClearGroups();
            return false;
        }

        private long iterations = 0;
        /// <summary>
        /// 返回递归了几次
        /// </summary>
        /// <returns>递归次数</returns>
        public long GetIterations()
        {
            return iterations;
        }

        /// <summary>
        /// 主要算法。暴力、贪心、分治。
        /// </summary>
        /// <param name="p">传入 1 开始解</param>
        /// <returns>是否可解</returns>
        private bool s(int p)
        {
            iterations++;
            if (blocksPtr >= blocks.GetLength(0))
            {
                return true;
            }
            char block = blocks[blocksPtr++];
            if (block == I)
            {
                // I 形块自旋后有2种放置方式
                for (int y = 0; y <= board.GetLength(0) - 4; y++)
                {
                    for (int x = 0; x <= board.GetLength(1) - 1; x++)
                    {
                        if (board[y,x] == 0 && board[y + 1,x] == 0 && board[y + 2,x] == 0 && board[y + 3,x] == 0)
                        {
                            board[y,x] = p;
                            board[y + 1,x] = p;
                            board[y + 2,x] = p;
                            board[y + 3,x] = p;
                            if (!IsStupidConfig() && s(p + 1))
                            {
                                return true;
                            }
                            board[y,x] = 0;
                            board[y + 1,x] = 0;
                            board[y + 2,x] = 0;
                            board[y + 3,x] = 0;
                        }
                    }
                }
                for (int y = 0; y <= board.GetLength(0) - 1; y++)
                {
                    for (int x = 0; x <= board.GetLength(1) - 4; x++)
                    {
                        if (board[y,x] == 0 && board[y,x + 1] == 0 && board[y,x + 2] == 0 && board[y,x + 3] == 0)
                        {
                            board[y,x] = p;
                            board[y,x + 1] = p;
                            board[y,x + 2] = p;
                            board[y,x + 3] = p;
                            if (!IsStupidConfig() && s(p + 1))
                            {
                                return true;
                            }
                            board[y,x] = 0;
                            board[y,x + 1] = 0;
                            board[y,x + 2] = 0;
                            board[y,x + 3] = 0;
                        }
                    }
                }
                --blocksPtr;
                return false;
            }
            if (block == O)
            {
                // 2x2正方形方块只有1种放置方式
                for (int y = 0; y <= board.GetLength(0) - 2; y++)
                {
                    for (int x = 0; x <= board.GetLength(1) - 2; x++)
                    {
                        if (board[y,x] == 0 && board[y + 1,x] == 0 && board[y,x + 1] == 0 && board[y + 1,x + 1] == 0)
                        {
                            
                            board[y,x] = p;
                            board[y + 1,x] = p;
                            board[y,x + 1] = p;
                            board[y + 1,x + 1] = p;
                            if (!IsStupidConfig() && s(p + 1))
                            {
                                return true;
                            }
                            board[y,x] = 0;
                            board[y + 1,x] = 0;
                            board[y,x + 1] = 0;
                            board[y + 1,x + 1] = 0;
                        }
                    }
                }
                
                
                --blocksPtr;
                return false;
            }

            if (block == T)
            {
                // T 形块自旋后有4种放置方式
                for (int y = 0; y <= board.GetLength(0) - 2; y++)
                {
                    for (int x = 0; x <= board.GetLength(1) - 3; x++)
                    {
                        if (board[y,x] == 0 && board[y,x + 1] == 0 && board[y + 1,x + 1] == 0 && board[y,x + 2] == 0)
                        {
                            board[y,x] = p;
                            board[y,x + 1] = p;
                            board[y + 1,x + 1] = p;
                            board[y,x + 2] = p;
                            if (!IsStupidConfig() && s(p + 1))
                            {
                                return true;
                            }
                            board[y,x] = 0;
                            board[y,x + 1] = 0;
                            board[y + 1,x + 1] = 0;
                            board[y,x + 2] = 0;
                        }
                    }
                }
                for (int y = 0; y <= board.GetLength(0) - 3; y++)
                {
                    for (int x = 0; x <= board.GetLength(1) - 2; x++)
                    {
                        if (board[y,x] == 0 && board[y + 1,x] == 0 && board[y + 1,x + 1] == 0 && board[y + 2,x] == 0)
                        {
                            
                            board[y,x] = p;
                            board[y + 1,x] = p;
                            board[y + 1,x + 1] = p;
                            board[y + 2,x] = p;
                            if (!IsStupidConfig() && s(p + 1))
                            {
                                return true;
                            }
                            board[y,x] = 0;
                            board[y + 1,x] = 0;
                            board[y + 1,x + 1] = 0;
                            board[y + 2,x] = 0;
                        }
                    }
                }
                for (int y = 0; y <= board.GetLength(0) - 3; y++)
                {
                    for (int x = 0; x <= board.GetLength(1) - 2; x++)
                    {
                        if (board[y,x + 1] == 0 && board[y + 1,x] == 0 && board[y + 1,x + 1] == 0 && board[y + 2,x + 1] == 0)
                        {
                            
                            board[y,x + 1] = p;
                            board[y + 1,x] = p;
                            board[y + 1,x + 1] = p;
                            board[y + 2,x + 1] = p;
                            if (!IsStupidConfig() && s(p + 1))
                            {
                                return true;
                            }
                            board[y,x + 1] = 0;
                            board[y + 1,x] = 0;
                            board[y + 1,x + 1] = 0;
                            board[y + 2,x + 1] = 0;
                        }
                    }
                }
                for (int y = 0; y <= board.GetLength(0) - 2; y++)
                {
                    for (int x = 0; x <= board.GetLength(1) - 3; x++)
                    {
                        if (board[y + 1,x] == 0 && board[y,x + 1] == 0 && board[y + 1,x + 1] == 0 && board[y + 1,x + 2] == 0)
                        {
                            
                            board[y + 1,x] = p;
                            board[y,x + 1] = p;
                            board[y + 1,x + 1] = p;
                            board[y + 1,x + 2] = p;
                            if (!IsStupidConfig() && s(p + 1))
                            {
                                return true;
                            }
                            board[y + 1,x] = 0;
                            board[y,x + 1] = 0;
                            board[y + 1,x + 1] = 0;
                            board[y + 1,x + 2] = 0;
                        }
                    }
                }
                --blocksPtr;
                return false;
            }

            if (block == J)
            {
                // J 形块自旋后有4种放置方式
                for (int y = 0; y <= board.GetLength(0) - 2; y++)
                {
                    for (int x = 0; x <= board.GetLength(1) - 3; x++)
                    {
                        if (board[y,x] == 0 && board[y,x + 1] == 0 && board[y + 1,x + 2] == 0 && board[y,x + 2] == 0)
                        {
                            
                            board[y,x] = p;
                            board[y,x + 1] = p;
                            board[y + 1,x + 2] = p;
                            board[y,x + 2] = p;
                            if (!IsStupidConfig() && s(p + 1))
                            {
                                return true;
                            }
                            board[y,x] = 0;
                            board[y,x + 1] = 0;
                            board[y + 1,x + 2] = 0;
                            board[y,x + 2] = 0;
                        }
                    }
                }
                for (int y = 0; y <= board.GetLength(0) - 2; y++)
                {
                    for (int x = 0; x <= board.GetLength(1) - 3; x++)
                    {
                        if (board[y + 1,x] == 0 && board[y,x] == 0 && board[y + 1,x + 1] == 0 && board[y + 1,x + 2] == 0)
                        {
                            
                            board[y + 1,x] = p;
                            board[y,x] = p;
                            board[y + 1,x + 1] = p;
                            board[y + 1,x + 2] = p;
                            if (!IsStupidConfig() && s(p + 1))
                            {
                                return true;
                            }
                            board[y + 1,x] = 0;
                            board[y,x] = 0;
                            board[y + 1,x + 1] = 0;
                            board[y + 1,x + 2] = 0;
                        }
                    }
                }
                for (int y = 0; y <= board.GetLength(0) - 3; y++)
                {
                    for (int x = 0; x <= board.GetLength(1) - 2; x++)
                    {
                        if (board[y,x] == 0 && board[y + 1,x] == 0 && board[y,x + 1] == 0 && board[y + 2,x] == 0)
                        {
                            
                            board[y,x] = p;
                            board[y + 1,x] = p;
                            board[y,x + 1] = p;
                            board[y + 2,x] = p;
                            if (!IsStupidConfig() && s(p + 1))
                            {
                                return true;
                            }
                            board[y,x] = 0;
                            board[y + 1,x] = 0;
                            board[y,x + 1] = 0;
                            board[y + 2,x] = 0;
                        }
                    }
                }
                for (int y = 0; y <= board.GetLength(0) - 3; y++)
                {
                    for (int x = 0; x <= board.GetLength(1) - 2; x++)
                    {
                        if (board[y,x + 1] == 0 && board[y + 2,x] == 0 && board[y + 1,x + 1] == 0 && board[y + 2,x + 1] == 0)
                        {
                            
                            board[y,x + 1] = p;
                            board[y + 2,x] = p;
                            board[y + 1,x + 1] = p;
                            board[y + 2,x + 1] = p;
                            if (!IsStupidConfig() && s(p + 1))
                            {
                                return true;
                            }
                            board[y,x + 1] = 0;
                            board[y + 2,x] = 0;
                            board[y + 1,x + 1] = 0;
                            board[y + 2,x + 1] = 0;
                        }
                    }
                }
                --blocksPtr;
                return false;
            }

            if (block == L)
            {
                // L 形块自旋后有4种放置方式
                for (int y = 0; y <= board.GetLength(0) - 2; y++)
                {
                    for (int x = 0; x <= board.GetLength(1) - 3; x++)
                    {
                        if (board[y,x] == 0 && board[y,x + 1] == 0 && board[y + 1,x] == 0 && board[y,x + 2] == 0)
                        {
                            
                            board[y,x] = p;
                            board[y,x + 1] = p;
                            board[y + 1,x] = p;
                            board[y,x + 2] = p;
                            if (!IsStupidConfig() && s(p + 1))
                            {
                                return true;
                            }
                            board[y,x] = 0;
                            board[y,x + 1] = 0;
                            board[y + 1,x] = 0;
                            board[y,x + 2] = 0;
                        }
                    }
                }
                for (int y = 0; y <= board.GetLength(0) - 3; y++)
                {
                    for (int x = 0; x <= board.GetLength(1) - 2; x++)
                    {
                        if (board[y,x] == 0 && board[y + 1,x] == 0 && board[y + 2,x + 1] == 0 && board[y + 2,x] == 0)
                        {
                            
                            board[y,x] = p;
                            board[y + 1,x] = p;
                            board[y + 2,x + 1] = p;
                            board[y + 2,x] = p;
                            if (!IsStupidConfig() && s(p + 1))
                            {
                                return true;
                            }
                            board[y,x] = 0;
                            board[y + 1,x] = 0;
                            board[y + 2,x + 1] = 0;
                            board[y + 2,x] = 0;
                        }
                    }
                }
                for (int y = 0; y <= board.GetLength(0) - 3; y++)
                {
                    for (int x = 0; x <= board.GetLength(1) - 2; x++)
                    {
                        if (board[y,x + 1] == 0 && board[y,x] == 0 && board[y + 1,x + 1] == 0 && board[y + 2,x + 1] == 0)
                        {
                            
                            board[y,x + 1] = p;
                            board[y,x] = p;
                            board[y + 1,x + 1] = p;
                            board[y + 2,x + 1] = p;
                            if (!IsStupidConfig() && s(p + 1))
                            {
                                return true;
                            }
                            board[y,x + 1] = 0;
                            board[y,x] = 0;
                            board[y + 1,x + 1] = 0;
                            board[y + 2,x + 1] = 0;
                        }
                    }
                }
                for (int y = 0; y <= board.GetLength(0) - 2; y++)
                {
                    for (int x = 0; x <= board.GetLength(1) - 3; x++)
                    {
                        if (board[y + 1,x] == 0 && board[y,x + 2] == 0 && board[y + 1,x + 1] == 0 && board[y + 1,x + 2] == 0)
                        {
                            
                            board[y + 1,x] = p;
                            board[y,x + 2] = p;
                            board[y + 1,x + 1] = p;
                            board[y + 1,x + 2] = p;
                            if (!IsStupidConfig() && s(p + 1))
                            {
                                return true;
                            }
                            board[y + 1,x] = 0;
                            board[y,x + 2] = 0;
                            board[y + 1,x + 1] = 0;
                            board[y + 1,x + 2] = 0;
                        }
                    }
                }
                --blocksPtr;
                return false;
            }

            if (block == S)
            {
                // S 形块自旋后有2种放置方式
                for (int y = 0; y <= board.GetLength(0) - 3; y++)
                {
                    for (int x = 0; x <= board.GetLength(1) - 2; x++)
                    {
                        if (board[y,x] == 0 && board[y + 1,x] == 0 && board[y + 1,x + 1] == 0 && board[y + 2,x + 1] == 0)
                        {
                            
                            board[y,x] = p;
                            board[y + 1,x] = p;
                            board[y + 1,x + 1] = p;
                            board[y + 2,x + 1] = p;
                            if (!IsStupidConfig() && s(p + 1))
                            {
                                return true;
                            }
                            board[y,x] = 0;
                            board[y + 1,x] = 0;
                            board[y + 1,x + 1] = 0;
                            board[y + 2,x + 1] = 0;
                        }
                    }
                }
                for (int y = 0; y <= board.GetLength(0) - 2; y++)
                {
                    for (int x = 0; x <= board.GetLength(1) - 3; x++)
                    {
                        if (board[y,x + 1] == 0 && board[y,x + 2] == 0 && board[y + 1,x] == 0 && board[y + 1,x + 1] == 0)
                        {
                            
                            board[y,x + 1] = p;
                            board[y,x + 2] = p;
                            board[y + 1,x] = p;
                            board[y + 1,x + 1] = p;
                            if (!IsStupidConfig() && s(p + 1))
                            {
                                return true;
                            }
                            board[y,x + 1] = 0;
                            board[y,x + 2] = 0;
                            board[y + 1,x] = 0;
                            board[y + 1,x + 1] = 0;
                        }
                    }
                }
                --blocksPtr;
                return false;
            }

            if (block == Z)
            {
                // Z 形块自旋后有2种放置方式
                for (int y = 0; y <= board.GetLength(0) - 2; y++)
                {
                    for (int x = 0; x <= board.GetLength(1) - 3; x++)
                    {
                        if (board[y,x] == 0 && board[y,x + 1] == 0 && board[y + 1,x + 1] == 0 && board[y + 1,x + 2] == 0)
                        {
                            
                            board[y,x] = p;
                            board[y,x + 1] = p;
                            board[y + 1,x + 1] = p;
                            board[y + 1,x + 2] = p;
                            if (!IsStupidConfig() && s(p + 1))
                            {
                                return true;
                            }
                            board[y,x] = 0;
                            board[y,x + 1] = 0;
                            board[y + 1,x + 1] = 0;
                            board[y + 1,x + 2] = 0;
                        }
                    }
                }
                for (int y = 0; y <= board.GetLength(0) - 3; y++)
                {
                    for (int x = 0; x <= board.GetLength(1) - 2; x++)
                    {
                        if (board[y,x + 1] == 0 && board[y + 1,x] == 0 && board[y + 1,x + 1] == 0 && board[y + 2,x] == 0)
                        {
                            
                            board[y,x + 1] = p;
                            board[y + 1,x] = p;
                            board[y + 1,x + 1] = p;
                            board[y + 2,x] = p;
                            if (!IsStupidConfig() && s(p + 1))
                            {
                                return true;
                            }
                            board[y,x + 1] = 0;
                            board[y + 1,x] = 0;
                            board[y + 1,x + 1] = 0;
                            board[y + 2,x] = 0;
                        }
                    }
                }
                --blocksPtr;
                return false;
            }
            throw new InvalidOperationException(@"算法出错！");
        }

        private bool solved = false, solveable = false;

        private readonly object sync=new object();

        /// <summary>
        /// 调用这个函数开始解拼图
        /// </summary>
        /// <returns>是否有解</returns>
        public bool Solve()
        {
            lock (sync)
            {
                if (solved)
                    return solveable;
                solveable = nPieces * 4 == board.GetLength(0) * board.GetLength(1) && s(1);
                solved = true;
                return solveable;
            }
        }

        public int[,] getBoard()
        {
            return board.Clone() as int[,];
        }

        public int getNumberOfPieces()
        {
            return nPieces;
        }

        public int getWidth()
        {
            return board.GetLength(1);
        }

        public int getHeight()
        {
            return board.GetLength(0);
        }

        public bool isSolved()
        {
            return solved;
        }

        public int getIBlocks()
        {
            return blocks.Count(c => c == I);
        }

        public int getOBlocks()
        {
            return blocks.Count(c => c == O);
        }

        public int getTBlocks()
        {
            return blocks.Count(c => c == T);
        }

        public int getJBlocks()
        {
            return blocks.Count(c => c == J);
        }

        public int getLBlocks()
        {
            return blocks.Count(c => c == L);
        }

        public int getSBlocks()
        {
            return blocks.Count(c => c == S);
        }
       
        public int getZBlocks()
        {
            return blocks.Count(c => c == Z);
        }
    }
}