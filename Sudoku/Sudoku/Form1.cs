using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sudoku
{
    public partial class Form1 : Form
    {
        private List<Button> levels; //gumbi za odabir osnovnih težina
        private DataGridView grid; //sudoku

        private int cellwidth;
        private int cellheight;
        private int cellnumber;

        //gumbi i labeli za vrijeme, bilješke, zvuk, hint i kraj igre
        private Button notesb;
        private Label notesl;
        private Label time;
        private Timer t;
        private int timeElapsed;
        private bool on;
        private Label congratsl;
        private Label congratst;
        private PictureBox congratsp;
        private Button hintb;
        private SoundPlayer yay;
        private SoundPlayer music;
 

        private List<List<string>> matrica; //matrica igre
        private List<List<string>> gmatrica; // generirana matrica (rješeni sudoku)
        private List<List<string>> pgmatrica; // početna matrica sudokua
        private List<string> values;

        private string biljeske;

        public Form1()
        {
            levels = new List<Button>();

            t = new Timer();

            music = new SoundPlayer(Properties.Resources.funnysong);
            music.PlayLooping();

            cellwidth = 45;
            cellheight = 45;
            cellnumber = 9;
            timeElapsed = 0;

            matrica = new List<List<string>>();
            gmatrica = new List<List<string>>();
            pgmatrica = new List<List<string>>();
            values = new List<string>();


            InitializeComponent();
            DoubleBuffered = true; //za smanjenje grafičkih smetnji
        }

        private DataGridView initialize_NewGrid(string ime) //svaki sudoku će imati ova svojstva
        {
            grid = new DataGridView();
            grid.Name = ime;
            grid.AllowUserToResizeColumns = false;
            grid.AllowUserToResizeRows = false;
            grid.AllowUserToAddRows = false;
            grid.RowHeadersVisible = false;
            grid.ColumnHeadersVisible = false;
            grid.MultiSelect = false;
            grid.GridColor = Color.DarkRed;
            grid.DefaultCellStyle.BackColor = Color.WhiteSmoke;
            grid.DefaultCellStyle.SelectionBackColor = Color.Crimson;
            grid.ScrollBars = ScrollBars.None;
            grid.Font = new Font("Calibri", 16F, FontStyle.Bold);
            grid.ForeColor = Color.DarkRed;
            grid.SelectionMode = DataGridViewSelectionMode.CellSelect;
            grid.EditingControlShowing += new DataGridViewEditingControlShowingEventHandler(grid_EditingControlShowing);
            grid.CellValueChanged += new DataGridViewCellEventHandler(cell_CellValueChanged);
            grid.CellClick += new DataGridViewCellEventHandler(cell_CellClick);
            return grid;
        }

        private void clear_All() //čisti gumbe i grid za ponovni početak igre
        {
            for (int i = 0; i < levels.Count; ++i)
            {
                Button btn1 = levels[i];

                if (this.Controls.Contains(btn1))
                {
                    this.Controls.Remove(btn1);
                }
            }

            levels.Clear();

            this.Controls.Remove(grid);

            this.Controls.Remove(notesb);
            this.Controls.Remove(notesl);
            this.Controls.Remove(hintb);
            on = false;

            if(t.Enabled == true) t.Stop();
            this.Controls.Remove(time);

            matrica.Clear();
            gmatrica.Clear();
            pgmatrica.Clear();
            values.Clear();
            matrica.TrimExcess();
            gmatrica.TrimExcess();
            pgmatrica.TrimExcess();
            values.TrimExcess();

            this.label1.Visible = true;
            this.Controls.Remove(congratsl);
            this.Controls.Remove(congratst);
            this.Controls.Remove(congratsp);

            biljeske = String.Empty;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            clear_All();
            timeElapsed = 0;

            int x = 0;
            for (int i = 0; i < 5; ++i)
            {
                Button gumb = new Button();
                levels.Add(gumb);
                gumb.Size = this.button1.Size;
                if (i < 3) gumb.Location = new Point(600 + x, this.button1.Location.Y);
                else if( i == 3) gumb.Location = new Point(levels[1].Location.X - (gumb.Size.Width + 10)/2, this.button2.Location.Y);
                else gumb.Location = new Point(levels[1].Location.X + (gumb.Size.Width + 10)*1/2, this.button2.Location.Y);
                x += gumb.Size.Width + 10;
                gumb.AutoSize = false;
                gumb.BackColor = Color.Black;
                gumb.ForeColor = Color.DarkRed;
                gumb.FlatAppearance.BorderColor = Color.DarkRed;
                gumb.FlatAppearance.BorderSize = 5;
                gumb.FlatStyle = FlatStyle.Flat;
                gumb.Font = new Font("Algerian", 22);

                if (i == 0)
                {
                    gumb.Text = "EASY";
                    gumb.Name = "easy";
                }

                else if (i == 1)
                {
                    gumb.Text = "MEDIUM";
                    gumb.Name = "medium";            
                }

                else if (i == 2)
                {
                    gumb.Text = "HARD";
                    gumb.Name = "hard";                   
                }

                else if (i == 3)
                {
                    gumb.Text = "16x16";
                    gumb.Name = "16";                   
                }

                else
                {
                    gumb.Text = "25x25";
                    gumb.Name = "25";                  
                }
                
                gumb.Click += new EventHandler(gumb_Click);
                gumb.MouseEnter += new EventHandler(gumb_MouseEnter);
                gumb.MouseLeave += new EventHandler(gumb_MouseLeave);
                this.Controls.Add(gumb);
            }
        }

        private void gumb_MouseEnter(object sender, EventArgs e)
        {
            Button gumb = (sender as Button);
            gumb.BackColor = Color.DarkRed;
            gumb.ForeColor = Color.Black;
            gumb.FlatAppearance.BorderColor = Color.Black;
        }

        private void gumb_MouseLeave(object sender, EventArgs e)
        {
            Button gumb = (sender as Button);
            gumb.BackColor = Color.Black;
            gumb.ForeColor = Color.DarkRed;
            gumb.FlatAppearance.BorderColor = Color.DarkRed;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void gumb_Click(object sender, EventArgs e)
        {
            clear_All();
            start_GameNew(sender);
        }

        private void initialize_Notes() //stvara label i gumb za bilješke
        {
            notesb = new Button();
            notesl = new Label();

            notesl.Name = "notesl";
            notesl.Text = "TOGGLE NOTES:";
            notesl.Size = this.button1.Size;
            notesl.AutoSize = true;
            notesl.BackColor = Color.Black;
            notesl.ForeColor = Color.DarkRed;
            notesl.Font = new Font("Algerian", 22);
            notesl.TextAlign = ContentAlignment.MiddleCenter;

            notesb.Name = "notesb";
            notesb.Text = "OFF";
            notesb.Size = new Size((int)this.button1.Size.Width / 3, (int)this.button1.Size.Height / 3);
            notesb.AutoSize = false;
            notesb.BackColor = Color.Black;
            notesb.ForeColor = Color.DarkRed;
            notesb.FlatAppearance.BorderColor = Color.DarkRed;
            notesb.FlatAppearance.BorderSize = 2;
            notesb.FlatStyle = FlatStyle.Flat;
            notesb.TextAlign = ContentAlignment.MiddleCenter;
            notesb.Font = new Font("Algerian", 20);
            notesb.Click += new EventHandler(notes_Click);
            notesb.MouseEnter += new EventHandler(gumb_MouseEnter);
            notesb.MouseLeave += new EventHandler(gumb_MouseLeave);

            this.Controls.Add(notesl);
            this.Controls.Add(notesb);
        }

        private void initialize_Time() //stvara label za prikaz vremena
        {
            time = new Label();
            time.Name = "time";
            TimeSpan timespan = TimeSpan.FromMilliseconds(timeElapsed);
            time.Text = "Time: " + timespan.ToString(@"mm\:ss");
            time.Size = this.button1.Size;
            time.AutoSize = true;
            time.BackColor = Color.Black;
            time.ForeColor = Color.DarkRed;
            time.Font = new Font("Algerian", 22);
            time.TextAlign = ContentAlignment.MiddleCenter;

            t = new Timer();
            t.Tick += new EventHandler(t_Tick);
            t.Interval = 1000;
            t.Start();

            this.Controls.Add(time);
        }

        private void initialize_Hint() //stvara gumb za hint
        {
            hintb = new Button();

            hintb.Name = "hintb";
            hintb.Text = "HINT";
            hintb.Size = this.button1.Size;

            hintb.AutoSize = false;
            hintb.BackColor = Color.Black;
            hintb.ForeColor = Color.DarkRed;
            hintb.FlatAppearance.BorderColor = Color.DarkRed;
            hintb.FlatAppearance.BorderSize = 5;
            hintb.FlatStyle = FlatStyle.Flat;
            hintb.TextAlign = ContentAlignment.MiddleCenter;
            hintb.Font = new Font("Algerian", 22);
            hintb.Click += new EventHandler(hint_Click);
            hintb.MouseEnter += new EventHandler(gumb_MouseEnter);
            hintb.MouseLeave += new EventHandler(gumb_MouseLeave);
            this.Controls.Add(hintb);
        }


        private void start_GameNew(object sender) //pokreće igru
        {
            initialize_Notes();
            initialize_Time();
            initialize_Hint();
            for (int i = 1; i < 10; ++i) values.Add(i.ToString());
            Button gumb = (sender as Button);


            if (gumb.Name == "easy" || gumb.Name == "medium" || gumb.Name == "hard") //one sve imaju tablicu 9x9
            {
                Console.WriteLine("9x9");
                for (int j = 0; j < 9; ++j)
                {
                    List<string> pom1 = new List<string>();
                    List<string> pom2 = new List<string>();
                    List<string> pom3 = new List<string>();
                    for (int k = 0; k < 9; ++k)
                    {
                        pom1.Add(" ");
                        pom2.Add(" ");
                        pom3.Add(" ");
                    }
                    matrica.Add(pom1);
                    gmatrica.Add(pom2);
                    pgmatrica.Add(pom3);
                }
                generate_sudoku(sender, 9); //generiramo sudoku igru 9x9 ovisno o težini
                initialize_NewGrid("grid");
                cellwidth = 45;
                cellheight = 45;
                cellnumber = 9;
                grid.Location = new Point(this.label1.Location.X, this.button1.Location.Y - 150);
                
                start_NormalGameNew();
            }

            else if (gumb.Name == "16") //tablica 16x16
            {
                for (int j = 0; j < 16; ++j)
                {
                    List<string> pom1 = new List<string>();
                    List<string> pom2 = new List<string>();
                    List<string> pom3 = new List<string>();
                    for (int k = 0; k < 16; ++k)
                    {
                        pom1.Add(" ");
                        pom2.Add(" ");
                        pom3.Add(" ");
                    }
                    matrica.Add(pom1);
                    gmatrica.Add(pom2);
                    pgmatrica.Add(pom3);
                }
                for( char c = 'A'; c<= 'G'; ++c) values.Add(c.ToString());
                generate_sudoku(sender, 16); //generiramo sudoku igru 16x16
                initialize_NewGrid("grid");
                cellwidth = 42;
                cellheight = 42;
                cellnumber = 16;
                this.label1.Visible = false;
                grid.Location = new Point(this.label1.Location.X - 120, this.button1.Location.Y - 350);
                
                start_NormalGameNew();
            }

            else if (gumb.Name == "25")
            {
                Console.WriteLine("25");
                for (int j = 0; j < 25; ++j)
                {
                    List<string> pom1 = new List<string>();
                    List<string> pom2 = new List<string>();
                    List<string> pom3 = new List<string>();
                    for (int k = 0; k < 25; ++k)
                    {
                        pom1.Add(" ");
                        pom2.Add(" ");
                        pom3.Add(" ");
                    }

                    matrica.Add(pom1);
                    gmatrica.Add(pom2);
                    pgmatrica.Add(pom3);
                }
                for (char c = 'A'; c <= 'P'; ++c) values.Add(c.ToString());
                generate_sudoku(sender, 25);
                initialize_NewGrid("grid");
                cellwidth = 25;
                cellheight = 25;
                cellnumber = 25;
                this.label1.Visible = false;
                grid.Location = new Point(this.label1.Location.X - 120, this.button1.Location.Y - 350);
                
                start_NormalGameNew();
            }
        }


        private void start_NormalGameNew()
        {
            grid.Size = new Size(cellwidth * cellnumber + 3, cellwidth * cellnumber + 3);
            notesl.Location = new Point(this.label1.Location.X, this.grid.Location.Y + grid.Size.Height + 20);
            notesb.Location = new Point(notesl.Location.X + notesl.Size.Width + 5, notesl.Location.Y);
            time.Location = new Point(notesl.Location.X, notesl.Location.Y + notesl.Size.Height + 5);
            hintb.Location = new Point(button1.Location.X, button1.Location.Y - 200);

            for (int i = 0; i < cellnumber; ++i)
            {
                DataGridViewTextBoxColumn text = new DataGridViewTextBoxColumn();
                text.MaxInputLength = 1;
                grid.Columns.Add(text);
                grid.Columns[i].Name = (i + 1).ToString();
                grid.Columns[i].Width = cellwidth;
                grid.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                DataGridViewRow row = new DataGridViewRow();
                row.Height = cellheight;
                grid.Rows.Add(row);
            }
           
            for (int i = 0; i < cellnumber; ++i)
                for (int j = 0; j < cellnumber; ++j)
                {
                    if( pgmatrica[i][j] != " ")
                    {
                        grid.Rows[i].Cells[j].Value = pgmatrica[i][j];
                        grid.Rows[i].Cells[j].ReadOnly = true;
                        grid.Rows[i].Cells[j].Style.ForeColor = Color.Black;
                    }
                }
            //podebljanje 3x3 odnosno 4x4  odnosno 5x5 podtablica u tablici
          
            int sqrt = (int)Math.Sqrt(cellnumber);

            for (int i = 0; i < cellnumber - sqrt; ++i)
            {
                if (i % sqrt == sqrt - 1)
                {
                    grid.Columns[i].DividerWidth = 2;
                    grid.Rows[i].DividerHeight = 2;
                }
            }

            Controls.Add(grid);
        }

        //ne smijemo dopustiti unos nekih slova
        private void grid_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress -= new KeyPressEventHandler(cell_KeyPress);
            TextBox tb = e.Control as TextBox;
            if (tb != null) tb.KeyPress += new KeyPressEventHandler(cell_KeyPress);
        }

        private void cell_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (cellnumber == 9 && (!char.IsDigit(e.KeyChar) || (char.IsDigit(e.KeyChar) && char.Equals(e.KeyChar, '0'))) && e.KeyChar != (char)8)
            {
                e.Handled = true;
                return;
            }
            else if (cellnumber == 16)
            {
                if ((!char.IsDigit(e.KeyChar) && !char.Equals(e.KeyChar, 'A') && !char.Equals(e.KeyChar, 'B') && !char.Equals(e.KeyChar, 'C') && !char.Equals(e.KeyChar, 'D') && !char.Equals(e.KeyChar, 'E') && !char.Equals(e.KeyChar, 'F') && !char.Equals(e.KeyChar, 'G')) || (char.IsDigit(e.KeyChar) && char.Equals(e.KeyChar, '0')))
                {
                    e.Handled = true;
                    return;
                }
            }
            else if (cellnumber == 25)
            {
                if ((!char.IsDigit(e.KeyChar) && !char.Equals(e.KeyChar, 'B') && !char.Equals(e.KeyChar, 'B') && !char.Equals(e.KeyChar, 'C') && !char.Equals(e.KeyChar, 'D') && !char.Equals(e.KeyChar, 'E') && !char.Equals(e.KeyChar, 'F') && !char.Equals(e.KeyChar, 'G') && !char.Equals(e.KeyChar, 'H') && !char.Equals(e.KeyChar, 'I') &&
                    !char.Equals(e.KeyChar, 'J') && !char.Equals(e.KeyChar, 'K') && !char.Equals(e.KeyChar, 'L') && !char.Equals(e.KeyChar, 'M') && !char.Equals(e.KeyChar, 'N') && !char.Equals(e.KeyChar, 'P')) || (char.IsDigit(e.KeyChar) && char.Equals(e.KeyChar, '0')))
                {
                    e.Handled = true;
                    return;
                }
            }
  

            if (on) //ako su uključene bilješke, onda samo želimo zapisati moguće vrijednosti, ne spremamo u matricu
            {
                foreach (DataGridViewTextBoxColumn column in grid.Columns)
                {
                    column.MaxInputLength = cellnumber;
                }

                biljeske += e.KeyChar.ToString();
                if (biljeske.Length != 5 && biljeske.Length != 11) biljeske += " ";
                else biljeske += "\n";
            }
            else if (!on)
            {
                grid.Font = new Font("Calibri", 16F, FontStyle.Bold);
                grid.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                foreach (DataGridViewTextBoxColumn column in grid.Columns)
                {
                    column.MaxInputLength = 1;
                }
                int r = grid.CurrentCell.RowIndex;
                int c = grid.CurrentCell.ColumnIndex;
                string val = e.KeyChar.ToString();
                if (!val.Equals(gmatrica[r][c]) && val!= ((char)8).ToString())
                {
                    MessageBox.Show("Wrong input");
                }
            }
        }

        //spremanje unešenih podataka u matrice i provjera je li igra gotova
        private void cell_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView grid = (sender as DataGridView);
            DataGridViewCell cell = grid.Rows[e.RowIndex].Cells[e.ColumnIndex];

            if (!on) //odlučili smo se za vrijednost, spremamo je u matricu
            {
                Console.WriteLine("mjenjam cell");
                cell.Style.Font = new Font("Calibri", 16F, FontStyle.Bold);
                cell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

                matrica[e.RowIndex][e.ColumnIndex] = cell.Value.ToString();

                bool gotovo = check_IfDone();
                if (gotovo) show_Congratulations();
                
            }

            else if (on) //ne spremamo
            {
                cell.Style.Font = new Font("Calibri", 9.5F);
                cell.Style.Alignment = DataGridViewContentAlignment.TopLeft;
                cell.Style.WrapMode = DataGridViewTriState.True;
                cell.Value = biljeske;
                biljeske = String.Empty;
            }
        }

        private void cell_CellClick(object sender, DataGridViewCellEventArgs e) //promjena boje retka i stupca s obzirom na odabranu ćeliju
        {
            DataGridView grid = (sender as DataGridView);
            int r = e.RowIndex;
            int c = e.ColumnIndex;

            for (int i = 0; i < cellnumber; ++i)
            {
                grid.Rows[i].DefaultCellStyle.BackColor = Color.White;
            }
            grid.Rows[r].DefaultCellStyle.BackColor = Color.Salmon;

            for( int i = 0; i < cellnumber; ++i)
            {
                grid.Rows[i].Cells[c].Style.ApplyStyle(this.grid.Rows[r].DefaultCellStyle);
            }

        }

        //promijeni se stil unosa kada je gumb notes kliknut
        private void notes_Click(object sender, EventArgs e)
        {
            if (notesb.Text == "OFF")
            {
                notesb.Text = "ON";
                on = true;
            }
            else if (notesb.Text == "ON")
            {
                notesb.Text = "OFF";
                on = false;
            }
        }

        private void hint_Click(object sender, EventArgs e) //klik na gumb hint, upisuje se jedna vrijednost u prazno polje na sudoku
        {
            int r, c;

            do
            {
                Random rnd = new Random();
                r = rnd.Next(0, cellnumber);
                c = rnd.Next(0, cellnumber);
            }
            while (matrica[r][c] != " ");

            matrica[r][c] = gmatrica[r][c];
            grid.Rows[r].Cells[c].Value = matrica[r][c];
        }

        //računanje vremena
        void t_Tick(object sender, EventArgs e)
        {
            timeElapsed += t.Interval;
            TimeSpan timespan = TimeSpan.FromMilliseconds(timeElapsed);
            time.Text = "Time: " + timespan.ToString(@"mm\:ss");
        }

        //uspoređuje generiranu matricu i matricu iz igre
        private bool check_IfDone()
        {
            for (int i = 0; i < cellnumber; i++)
                for (int j = 0; j < cellnumber; j++)
                    if (matrica[i][j] != gmatrica[i][j]) return false;
            return true;
        }

        //čestitke ako korisnik pobijedi
        private void show_Congratulations()
        {
            clear_All();
            this.label1.Visible = false;

            congratsp = new PictureBox();
            congratsp.Name = "congratsp";
            congratsp.Size = new Size(800, 533);
            congratsp.Location = new Point(this.label1.Location.X - 200, this.label1.Location.Y + 50);
            congratsp.Image = Properties.Resources.fireworks;
            this.Controls.Add(congratsp);
            if(button3.Text == "On")
            {
                yay = new SoundPlayer(Properties.Resources.yay);
                yay.Play();
            }
            

            congratsl = new Label();
            congratsl.Name = "congratsl";
            congratsl.Text = "CONGRATULATIONS,\nYOU WON!";
            congratsl.Location = new Point(this.label1.Location.X, 70);
            congratsl.Size = this.button1.Size;
            congratsl.AutoSize = true;
            congratsl.BackColor = Color.Black;
            congratsl.ForeColor = Color.DarkRed;
            congratsl.Font = new Font("Algerian", 32);
            congratsl.TextAlign = ContentAlignment.MiddleCenter;
            this.Controls.Add(congratsl);

            congratst = new Label();
            congratst.Name = "congratst";
            TimeSpan timespan = TimeSpan.FromMilliseconds(timeElapsed);
            congratst.Text = "YOUR TIME: " + timespan.ToString(@"mm\:ss");
            congratst.Location = new Point(congratsp.Location.X, congratsp.Location.Y + congratsp.Size.Height + 10);
            congratst.Size = this.button1.Size;
            congratst.AutoSize = true;
            congratst.BackColor = Color.Black;
            congratst.ForeColor = Color.DarkRed;
            congratst.Font = new Font("Algerian", 32);
            congratst.TextAlign = ContentAlignment.MiddleCenter;
            
            this.Controls.Add(congratst);
        }

        private void generate_sudoku(object sender, int dim)
        {
            Console.WriteLine("generating sudoku");
            generateDiagonals(dim);
            generateRemaining(dim, 0, (int) Math.Sqrt(dim));
            Console.WriteLine("generated solved");

            string difficulty = (sender as Button).Text;

            if (difficulty == "EASY")
                generateUnsolvedSudoku(dim, 40);
            if (difficulty == "MEDIUM")
                generateUnsolvedSudoku(dim, 50);
            if (difficulty == "HARD")
                generateUnsolvedSudoku(dim, 60);
            if (difficulty == "16x16")
                generateUnsolvedSudoku(dim, 100);
            if (difficulty == "25x25")
                generateUnsolvedSudoku(dim, 250);
            Console.WriteLine("generated unsolved");
        }

        private void generateUnsolvedSudoku(int dim, int blanks)
        {
            int sqrt = (int)Math.Sqrt(dim);
            for( int i = 0; i < dim; ++i)
            {
                for( int j = 0; j < dim; ++j)
                {
                    pgmatrica[i][j] = gmatrica[i][j];
                }
            }

            int r, c;
            for (int i = 0; i < blanks; ++i)
            {
                do
                {
                    Random rnd = new Random();
                    
                    r = rnd.Next(0, dim);
                    c = rnd.Next(0, dim);

                }
                while (pgmatrica[r][c] == " ");

                pgmatrica[r][c] = " ";
            }
        }


        private void generateDiagonals(int dim)
        {
            int sqrt = (int)Math.Sqrt(dim);
            for (int i = 0; i < dim; i = i + sqrt)
                fillSquare(dim, i, i);
        }

        private void fillSquare(int dim, int row, int column)
        {
            int num;
            int sqrt = (int)Math.Sqrt(dim);
            for( int i = 0; i < sqrt; ++i)
            {
                for( int j = 0; j < sqrt; ++j)
                {
                    do
                    {
                        Random rnd = new Random();
                        num = rnd.Next(0, dim);
                    }
                    while (valueInSquare(dim, values[num], row, column, gmatrica));
                    gmatrica[row + i][column + j] = values[num];
                }
                
            }

        }

        private bool generateRemaining(int dim, int i, int j)
        {
            int sqrt = (int)Math.Sqrt(dim);
            if( j>= dim && i < dim - 1)
            {
                ++i;
                j = 0;
            }
            if (i >= dim && j >= dim) return true;

            if (i < sqrt)
            {
                if (j < sqrt)
                    j = sqrt;
            }
            else if (i < dim - sqrt)
            {
                if (j == (int)(i / sqrt) * sqrt)
                    j += sqrt;
            }
            else
            {
                if (j == dim - sqrt)
                {
                    i++;
                    j = 0;
                    if (i >= dim)
                        return true;
                }
            }


            for (int num = 0; num < dim; ++num)
            {
                
                if (!valueInRow(dim, values[num], i, gmatrica) && !valueInColumn(dim, values[num], j, gmatrica) && !valueInSquare(dim, values[num], i, j, gmatrica))
                {
                    Console.WriteLine("value je " + values[num].ToString());
                    gmatrica[i][j] = values[num];
                    Console.WriteLine("generate remaining " + gmatrica[i][j]);
                    if (generateRemaining(dim, i, j + 1))
                        return true;

                    gmatrica[i][j] = " ";
                }
            }

            return false;
        }


        private bool valueInRow(int dim, string val, int r, List<List<string>> listMatrica) //provjera postoji li val u zadanom retku
        {
            int column;

            for (column = 0; column < dim; ++column)
                if (val.Equals(listMatrica[r][column]))
                    return true;

            return false;
        }


        private bool valueInColumn(int dim, string val, int c, List<List<string>> listMatrica) //provjera postoji li val u zadanom stupcu
        {
            int row;

            for (row = 0; row < dim; ++row)
                if (val.Equals(listMatrica[row][c]))
                    return true;

            return false;
        }

        private bool valueInSquare(int dim, string val, int r, int c, List<List<string>> listMatrica) //provjera postoji li val u zadanom kvadratu
        {
            int i, j;
            int sqrt = (int)Math.Sqrt(dim);
            for (i = r - r % sqrt; i < r - r % sqrt + sqrt; ++i)
                for (j = c - c % sqrt; j < c - c % sqrt + sqrt; ++j)
                    if (val.Equals(listMatrica[i][j]))
                        return true;

            return false;
        }


        private bool matrixFull9(int[,] matrica)
        {
            for (int i = 0; i < 9; ++i)
                for (int j = 0; j < 9; ++j)
                    if (matrica[i, j] == 0)
                        return false;

            return true;
        }

        private void button3_Click(object sender, EventArgs e) //uključivanje/isključivanje glazbe
        {
            if( button3.Text == "On")
            {
                button3.Text = "Off";
                music.Stop();
            }
            else
            {
                button3.Text = "On";
                music.PlayLooping();
            }

        }

        private void button3_MouseEnter(object sender, EventArgs e)
        {
            Button gumb = (Button)sender;
            gumb.BackColor = Color.DarkRed;
            gumb.ForeColor = Color.Black;
            gumb.FlatAppearance.BorderColor = Color.Black;
        }

        private void button3_MouseLeave(object sender, EventArgs e)
        {
            Button gumb = (Button)sender;
            gumb.BackColor = Color.Black;
            gumb.ForeColor = Color.DarkRed;
            gumb.FlatAppearance.BorderColor = Color.DarkRed;
        }
    }
}
