using System.Drawing;
using System.Windows.Forms;

namespace Sokoban
{
    public partial class Main : Form
    {
        private int _currentLevelIndex;

        private Level _level;
        private Graphics _graphics;

        public Main()
        {
            InitializeComponent();
            timer.Start();

            LoadCurrentLevel();
        }

        private void LoadCurrentLevel()
        {
            _level = LevelLoader.Load(Data.Levels[_currentLevelIndex]);
            pictureBox.Image = new Bitmap(_level.Width * Level.TileSize, _level.Height * Level.TileSize);
            _graphics = Graphics.FromImage(pictureBox.Image);
        }

        private void OnLevelCompleted()
        {
            _currentLevelIndex++;

            if (_currentLevelIndex < Data.Levels.Length)
            {
                LoadCurrentLevel();
            }
            else
            {
                timer.Stop();
                MessageBox.Show("Well done, you completed all levels!");
                Application.Exit();
            }
        }

        private void timer_Tick(object sender, System.EventArgs e)
        {
            if (_level.IsCompleted())
            {
                OnLevelCompleted();
            }

            if (TryGetDirection(out var direction))
            {
                _level.HandleMovement(direction);
            }

            _level.Step();
            _level.Draw(_graphics);
            pictureBox.Refresh();
        }

        private bool _up, _down, _left, _right;

        private bool TryGetDirection(out Direction direction)
        {
            if (_up)
            {
                direction = Direction.Up;
                return true;
            }

            if (_down)
            {
                direction = Direction.Down;
                return true;
            }

            if (_left)
            {
                direction = Direction.Left;
                return true;
            }

            if (_right)
            {
                direction = Direction.Right;
                return true;
            }

            direction = (Direction)(-1);
            return false;
        }

        private void Main_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    _up = true;
                    break;
                case Keys.Down:
                    _down = true;
                    break;
                case Keys.Left:
                    _left = true;
                    break;
                case Keys.Right:
                    _right = true;
                    break;
                case Keys.R:
                    LoadCurrentLevel();
                    break;
            }
        }

        private void Main_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    _up = false;
                    break;
                case Keys.Down:
                    _down = false;
                    break;
                case Keys.Left:
                    _left = false;
                    break;
                case Keys.Right:
                    _right = false;
                    break;
            }
        }
    }
}
