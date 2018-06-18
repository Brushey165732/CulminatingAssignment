using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Threading;

namespace FireEmblem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public enum GameState {Title, MapSelect, GameOn, PlayerTurn, EnemyTurn, GameOver, WinScreen}

        public GameState gameState;
        Unit unit;
        Unit unit2;
        Window2 window2;

        MediaPlayer music = new MediaPlayer();
        DispatcherTimer GameTimer = new DispatcherTimer();
        Combat combat;

        public int temp;
        int turn = 0;

        int[,] map_array;

        int x_initial;
        int y_initial;
        int x_previous;
        int y_previous;
        bool cangetdestination = false;
        bool hasclearedmove = false;

        string map1 = "Map1_Village.txt";
        string array1 = "Array_1.txt";
        //for other maps??
        //string map2;
        //string array2;
        //string map3;
        //string array3;
        //string map4;
        //string array4;
        //string map5;
        //string array5;


        Rectangle select_box;
        List<Unit> allied_units = new List<Unit>();
        int alive_allied_units;
        int active_allied_units;

        List<Unit> enemy_units = new List<Unit>();
        int alive_enemy_units;
        int active_enemy_units;



        public MainWindow()
        {
            InitializeComponent();
            this.Title = "Jonathan Brushey - Culminating Assignment";
            window2 = new Window2();
            window2.Show();

            music.Open(new Uri("02 Main Theme.mp3", UriKind.Relative));
            //music.Play();

            GameTimer.Tick += GameTimer_Tick;
            GameTimer.Interval = new TimeSpan(170000);
            GameTimer.Start();

            Map map = new Map(this, canvas);
            map.generateMap(map1);
            map_array = map.mapArray(array1, allied_units, enemy_units);

            combat = new Combat(this, canvas, turn);

            select_box = new Rectangle();
            select_box.Stroke = Brushes.PaleGoldenrod;
            select_box.StrokeThickness = 3;
            select_box.Width = 38;
            select_box.Height = 38;
            canvas.Children.Add(select_box);

            gameState = GameState.PlayerTurn;
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {

            alive_enemy_units = enemy_units.Count();
            alive_allied_units = allied_units.Count();
            for (int i = 0; i < allied_units.Count; i++)
            {
                if (allied_units[i].isalive == false)
                {
                    alive_allied_units -= 1;
                    map_array[allied_units[i].y_position, allied_units[i].x_position] = '-';
                    allied_units[i].remove();
                    allied_units.RemoveAt(i);
                }
            }
            for (int i = 0; i < enemy_units.Count; i++)
            {
                if (enemy_units[i].isalive == false)
                {
                    alive_enemy_units -= 1;
                    map_array[enemy_units[i].y_position, enemy_units[i].x_position] = '-';
                    enemy_units[i].remove();
                    enemy_units.RemoveAt(i);
                }
            }

            if (gameState == GameState.Title)
            {

            }
            if (gameState == GameState.MapSelect)
            {

            }
            if (gameState == GameState.PlayerTurn)
            {
                this.Title = ("Player Phase");
                active_allied_units = allied_units.Count;

                for (int i = 0; i < allied_units.Count; i++)
                {
                    if (!allied_units[i].isactive)
                    {
                        allied_units[i].sprite.Fill = Brushes.Gray;
                        active_allied_units -= 1;
                        //Console.WriteLine(active_allied_units.ToString() + " allied units are active.");
                    }
                    else
                    {
                        if (allied_units[i].type == 0)
                        {
                            allied_units[i].sprite.Fill = Brushes.CornflowerBlue;
                        }
                        else if (allied_units[i].type == 1)
                        {
                            allied_units[i].sprite.Fill = Brushes.Blue;
                        }
                        else if (allied_units[i].type == 2)
                        {
                            allied_units[i].sprite.Fill = Brushes.DarkBlue;
                        }
                    }
                }
                for (int i = 0; i < enemy_units.Count; i++)
                {
                    enemy_units[i].isactive = true;
                    enemy_units[i].hasmoved = false;
                }
                active_enemy_units = enemy_units.Count();
                if (alive_allied_units == 0)
                {
                    gameState = GameState.GameOver;
                }
                if (alive_enemy_units == 0)
                {
                    gameState = GameState.WinScreen;
                }
                if (active_allied_units == 0)
                {
                    turn += 1;
                    gameState = GameState.EnemyTurn;
                }

            }
            if (gameState == GameState.EnemyTurn)
            {
                this.Title = ("Enemy Phase");
                active_enemy_units = enemy_units.Count();
                for (int i = 0; i < enemy_units.Count; i++)
                {
                    if (!enemy_units[i].isactive)
                    {
                        active_enemy_units -= 1;
                        //Console.WriteLine(active_enemy_units.ToString() + " enemy units are active.");
                    }
                }
                for (int i = 0; i < allied_units.Count; i++)
                {
                    allied_units[i].isactive = true;
                    allied_units[i].hasmoved = false;
                }
                active_allied_units = allied_units.Count;
                if (alive_allied_units == 0)
                {
                    gameState = GameState.GameOver;

                }
                if (alive_enemy_units == 0)
                {
                    gameState = GameState.WinScreen;
                }
                if (active_enemy_units == 0)
                {
                    turn += 1;
                    gameState = GameState.PlayerTurn;
                }
                for (int j = 0; j < enemy_units.Count; j++)
                {
                    MessageBox.Show("calling enemymove");
                    enemy_units[j].enemymove(map_array, allied_units, enemy_units, j, enemy_units[j].x_position, enemy_units[j].y_position, turn);
                }
            }
            if (gameState == GameState.GameOver)
            {
                this.Title = "Game Over";
            }
            if (gameState == GameState.WinScreen)
            {
                this.Title = "Congratulations";
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.Enter))
            {
                gameState = GameState.PlayerTurn;
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
            //Console.WriteLine(e.GetPosition(canvas).ToString());
            Point point = e.GetPosition(canvas);
            int x = Convert.ToInt32(point.X / 40);
            int y = Convert.ToInt32(point.Y / 40);
            //Console.WriteLine("(" + x + ", " + y + ")");

            Canvas.SetLeft(select_box, x * 40 + 1);
            Canvas.SetTop(select_box, y * 40 + 1);
            //Console.WriteLine(map_array[y, x].ToString() + ", " + map_array[y_previous, x_previous].ToString());
            if (cangetdestination)
            {
                allied_units[temp].playermove(map_array, allied_units, enemy_units, temp, x_initial, y_initial, x, y, turn);
                cangetdestination = false;
                combat.close_targets(map_array, allied_units, enemy_units, temp);
            }
            if (map_array[y,x] == 2)
            {
                for (int i = 0; i < allied_units.Count; i++)
                {

                    if (allied_units[i].x_position == x && allied_units[i].y_position == y)
                    {
                        if (allied_units[i].hasmoved && hasclearedmove == false)
                        { 
                            int canvasnum = canvas.Children.Count;
                            for (int X = canvasnum - 1; X > canvasnum - 41; X -= 1)
                            {
                                canvas.Children.Remove(canvas.Children[X]);
                            }
                            hasclearedmove = true;
                        }
                        if (!allied_units[i].hasmoved)
                        {
                            window2.stats(0, allied_units[i].type, allied_units[i].hpleft, allied_units[i].hptotal, allied_units[i].str, allied_units[i].skill, allied_units[i].spd, allied_units[i].luck, allied_units[i].def);
                            cangetdestination = true;
                            x_initial = x;
                            y_initial = y;
                            temp = i;
                            int I;
                            int J;
                            for(int X = -4; X <= 4; X++)
                            {
                                I = X;
                                for (int Y = -4; Y <= 4; Y++)
                                {
                                    J = Y;
                                    if (X < 0)
                                    {
                                        I = -X;
                                    }
                                    if (Y < 0)
                                    {
                                        J = -Y;
                                    }
                                    if (I + J <= 4 && I + J > 0)
                                    {
                                        Rectangle rect = new Rectangle();
                                        rect.Height = 40;
                                        rect.Width = 40;
                                        rect.Opacity = 0.2;
                                        if (I+J == 4)
                                        {
                                            rect.Fill = Brushes.Red;
                                        }
                                        else if (I+J <= 3)
                                        {
                                            rect.Fill = Brushes.Blue;
                                        }
                                        Canvas.SetLeft(rect, allied_units[i].x_position * 40 + X * 40);
                                        Canvas.SetTop(rect, allied_units[i].y_position * 40 + Y * 40);
                                        canvas.Children.Add(rect);
                                    }
                                }
                            }
                            hasclearedmove = false;
                        }
                        
                    }
                }
            }

            else if (map_array[y,x] == 3 && map_array[y_previous, x_previous] == 2 && turn % 2 == 0)
            {

                Console.WriteLine((x - x_previous) + ", " + (y - y_previous));
                Console.WriteLine(map_array[y_previous, x_previous] + ",, " + map_array[y, x]);
                for (int i = 0; i < enemy_units.Count; i++)
                {
                    if (enemy_units[i].x_position == x && enemy_units[i].y_position == y)
                    {
                        window2.stats(1, enemy_units[i].type, enemy_units[i].hpleft, enemy_units[i].hptotal, enemy_units[i].str, enemy_units[i].skill, enemy_units[i].spd, enemy_units[i].luck, enemy_units[i].def);
                    }
                }
                for (int i = 0; i < allied_units.Count; i++)
                {
                    if (allied_units[i].x_position == x_previous && allied_units[i].y_position == y_previous && allied_units[i].isactive)
                    {
                        if ((x == x_previous + 1 || x == x_previous -1) && y == y_previous)
                        {
                            for (int j = 0; j < enemy_units.Count; j++)
                            {
                                if (enemy_units[j].x_position == x && enemy_units[j].y_position == y)
                                {
                                    combat.fight(map_array, allied_units, enemy_units, i, j, turn);
                                    break;
                                }
                            }
                        }
                        else if (x == x_previous && (y == y_previous + 1 || y == y_previous - 1))
                        {
                            for (int j = 0; j < enemy_units.Count; j++)
                            {
                                if (enemy_units[j].x_position == x && enemy_units[j].y_position == y)
                                {
                                    combat.fight(map_array, allied_units, enemy_units, i, j, turn);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            else if (map_array[y, x] == 3)
            {
                for (int i = 0; i < enemy_units.Count; i++)
                {
                    //Console.WriteLine(enemy_units[i].x_position);
                    //Console.WriteLine(enemy_units[i].y_position);
                    if (enemy_units[i].x_position == x && enemy_units[i].y_position == y)
                    {
                        window2.stats(1, enemy_units[i].type, enemy_units[i].hpleft, enemy_units[i].hptotal, enemy_units[i].str, enemy_units[i].skill, enemy_units[i].spd, enemy_units[i].luck, enemy_units[i].def);
                    }
                }
            }

            x_previous = x;
            y_previous = y;
            
        }
    }
}
