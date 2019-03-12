namespace Genus2D.Utililities
{
    /// <summary>
    /// A cursor within a grid structure. Restricted to the grid.
    /// Top left = (0,0)
    /// Bottom Right = (Grid Width, Grid Height)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GridCounter
    {
        private int _cursorX;
        private int _cursorY;
        private int _cursorStartX;
        private int _cursorStartY;
        private int _gridWidth;
        private int _gridHeight;
        private bool _shouldWrap;

        public GridCounter(int pCursorStartX, int pCursorStartY, int pGridWidth, int pGridHeight, bool pShouldWrap = false)
        {
            this._cursorStartX = pCursorStartX;
            this._cursorStartY = pCursorStartY;

            this._cursorX = pCursorStartX;
            this._cursorY = pCursorStartY;

            this._gridWidth = pGridWidth;
            this._gridHeight = pGridHeight;

            this._shouldWrap = pShouldWrap;
        }

        public void MoveCursorRight()
        {
            _cursorX++;

            if (_cursorX >= _gridWidth)
            {
                if (_shouldWrap)
                {
                    _cursorX = 0;
                }
                else
                {
                    _cursorX = _gridWidth - 1;
                }
            }
        }

        public void MoveCursorLeft()
        {
            _cursorX--;

            if (_cursorX < 0)
            {
                if (_shouldWrap)
                {
                    _cursorX = _gridWidth;
                }
                else
                {
                    _cursorX = 0;
                }
            }
        }

        public void MoveCursorUp()
        {
            _cursorY--;

            if (_cursorY < 0)
            {
                if (_shouldWrap)
                {
                    _cursorY = _gridHeight;
                }
                else
                {
                    _cursorY = 0;
                }
            }
        }

        public void MoveCursorDown()
        {
            _cursorY++;

            if (_cursorY > _gridHeight)
            {
                if (_shouldWrap)
                {
                    _cursorY = 0;
                }
                else
                {
                    _cursorY = _gridHeight;
                }
            }
        }

        public int[] GetCursorCoordinates()
        {
            return new int[] { _cursorX, _cursorY };
        }

        public void MoveCursorToTopLeft()
        {
            _cursorX = 0;
            _cursorY = 0;
        }

        public void MoveCursorToTopRight()
        {
            _cursorX = _gridWidth;
            _cursorY = 0;
        }

        public void MoveCursorToBottomRight()
        {
            _cursorX = _gridWidth;
            _cursorY = _gridHeight;
        }

        public void MoveCursorToBottomLeft()
        {
            _cursorX = 0;
            _cursorY = _gridHeight;
        }
    }
}