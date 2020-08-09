/* Corbin Lutsch & Matthew Lord
 *     Z1837389  & Z1848456
 *  CSCI - 473
 *  Due: 04/11/19
 *  Assignment: Let's Play a Game ლ(ಠ益ಠლ)
 * 
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mandc_Assign5
{
    public partial class Form1 : Form
    {
        private static Pen myPen;
        private static SolidBrush myBrush;

        //keep track of the correct col, rows, and diagonal sum totals 
        private static int col1, col2, col3, col4, col5, col6, col7;
        private static int row1, row2, row3, row4, row5, row6, row7;
        private static int diag1, diag2;

        //current col, rows, and diagonal sum totals
        private static int r1, r2, r3, r4, r5, r6, r7;
        private static int c1, c2, c3, c4, c5, c6, c7;
        private static int d1, d2;

        private static int[] array2; //current text box values
        private static int[] answers; //answers for each text box
        private static string[] original; //original text box values

        private static TextBoxWithoutCaret[] array; //array of textboxes
        public Timer time;
        private int count = 0; //holds the total time for the puzzle 
        private static bool flag = true; //used to keep track of pause or resume
        private static bool cheat = false; //checks if cheat button was clicked or not
        private static int num = 0; //current puzzle, ranges from 1 to 3 
        private static string saveFile = ""; //the name of the save file if we save
        private static string completeFile = ""; //the name of the complete file if we reset
        private static string currentFile = ""; //name of the current file 

        private static bool EASY = false; //flags to determine which puzzle we are on
        private static bool MEDIUM = false;
        private static bool HARD = false;

        private static int completionTime = 0; //various variables to keep track of averages
        private static int puzzleTime1 = 0;
        private static int puzzleTime2 = 0;
        private static int puzzleTime3 = 0;
        private static int fastest = 0;
        private static int totalCompleted = 0;

        public Form1()
        {
            InitializeComponent();
            myPen = new Pen(Color.White);
            myBrush = new SolidBrush(Color.Gray);
            time = new Timer();
            time.Tick += timer_Tick;
        }

        /***************************************************************
        private void btnReset_Click(object sender, EventArgs e)

        Use: resets the current puzzle and deletes any completed puzzles
     
        Parameters: 1. object sender - the calling object/button
                    2. EventArgs e - the calling event
			
        Returns: nothing
        ***************************************************************/
        private void btnReset_Click(object sender, EventArgs e)
        {
            if (currentFile == "..\\..\\hard/h" + num.ToString() + ".txt" ||
                currentFile == "..\\..\\medium/m" + num.ToString() + ".txt" ||
                currentFile == "..\\..\\easy/e" + num.ToString() + ".txt")
            {
                for(int i = 0; i < array.Length; i++)
                {
                    if (array[i].ReadOnly == false) //clear the current text boxes 
                        array[i].Text = "";
                }
                count = 0;
                if (File.Exists(completeFile)) //delete the complete file
                    File.Delete(completeFile);

                if (File.Exists(saveFile)) //delete the complete file
                    File.Delete(saveFile);
            }
            else
            {
                File.Delete(currentFile); //otherwise delete the save file/complete file
                num--;
                if (EASY) //recall to open the file back up
                    btnEasy_Click(sender, e);
                else if (MEDIUM)
                    btnMedium_Click(sender, e);
                else if (HARD)
                    btnHard_Click(sender, e);
            }
            cheat = false;
            count = 0;
            time.Start();
        }

        /***************************************************************
        private void complete()

        Use: checks if a puzzle is complete or not
     
        Parameters: none
			
        Returns: nothing
        ***************************************************************/
        private void complete()
        {
            int total = 0;
            for(int i = 0; i < array.Length; i++)
            {
                if (array[i].Text == answers[i].ToString()) //check if answers match text boxes
                    total++;
            }

            if (total == answers.Length) //if all answers match 
            {
                time.Stop();
                totalCompleted++;
                completionTime += count;

                if (num == 1) //puzzle 1
                    puzzleTime1 = count;
                else if (num == 2) //puzzle 2
                    puzzleTime2 = count;
                else if (num == 3) //puzzle 3
                    puzzleTime3 = count;

                //sort the puzzles
                int[] sortedPuzzles = new int[3];
                sortedPuzzles[0] = puzzleTime1;
                sortedPuzzles[1] = puzzleTime2;
                sortedPuzzles[2] = puzzleTime3;
                Array.Sort(sortedPuzzles);

                //check for fastest puzzle
                if (sortedPuzzles[0] != 0)
                    fastest = sortedPuzzles[0];
                else if (sortedPuzzles[1] != 0)
                    fastest = sortedPuzzles[1];
                else
                    fastest = sortedPuzzles[2];
                
                //write the contents to the outfile 
                using (StreamWriter outFile = new StreamWriter(completeFile))
                {
                    for (int i = 0; i < original.Length; i++) //write original contents 
                    {
                        outFile.Write(original[i]);
                    }

                    for (int i = 0; i < answers.Length; i++) //write the answers
                    {
                        outFile.Write(answers[i].ToString());
                    }

                    for (int i = 0; i < array.Length; i++) //write the input
                    {
                        if (array[i].Text == "")
                            outFile.Write("0");
                        else
                            outFile.Write(array[i].Text);
                    }

                    if (!cheat)
                        outFile.Write(count.ToString());
                }
                //display averages to the user
                TimeSpan count1 = TimeSpan.FromMilliseconds(count * 100);
                TimeSpan fastest1 = TimeSpan.FromMilliseconds(fastest * 100);
                TimeSpan avg1 = TimeSpan.FromMilliseconds(completionTime / totalCompleted * 100);
                MessageBox.Show(String.Format("\nCompletion time: {0:mm\\:ss\\.f}\nFastest for this difficulty : {1:mm\\:ss\\.f}\nAverage completion: {2:mm\\:ss\\.f}\n", count1, fastest1, avg1));

            }

        }

        /***************************************************************
        private void btnSave_Click(object sender, EventArgs e)

        Use: saves the current puzzle 
     
        Parameters: 1. object sender - the calling object/button
                    2. EventArgs e - the calling event
			
        Returns: nothing
        ***************************************************************/
        private void btnSave_Click(object sender, EventArgs e)
        {

            using (StreamWriter outFile = new StreamWriter(saveFile))
            {
                for (int i = 0; i < original.Length; i++) //write original contents 
                {
                    outFile.Write(original[i]);
                }

                for (int i = 0; i < answers.Length; i++) //write the answers
                {
                    outFile.Write(answers[i].ToString());
                }

                for (int i = 0; i < array.Length; i++) //write the input
                {
                    if (array[i].Text == "")
                        outFile.Write("0");
                    else
                        outFile.Write(array[i].Text);
                }

                if (!cheat)
                    outFile.Write(count.ToString()); //write the timer
            }
        }

        /***************************************************************
        private void btnCheat_Click(object sender, EventArgs e)

        Use: if the puzzle is completely filled, it will fix the first
        incorrect answer in the first position starting in row 1, col1
        and working its way down the rows. If not all text boxes are filled
        it will fill a random text box with the correct answer
     
        Parameters: 1. object sender - the calling object/button
                    2. EventArgs e - the calling event
			
        Returns: nothing
        ***************************************************************/
        private void btnCheat_Click(object sender, EventArgs e)
        {
            Button ch = sender as Button;

            if (ch != null)
            {
                cheat = true; //update the cheat flag
                int total = 0;
                for (int i = 0; i < array.Length; i++) 
                {
                    if (array[i].Text != "")
                        total++;
                }

                if (total == array.Length) //all textboxes are filled
                {
                    for (int i = 0; i < array.Length; i++)
                    {
                        if (Convert.ToInt32(array[i].Text) != answers[i]) //fix them sequentially 
                        {
                            array[i].Text = answers[i].ToString();
                            break;
                        }
                    }
                }
                else
                {
                    Random random = new Random(); 
                    int random1 = random.Next(0, array.Length);
                    int temp = 0;

                    //fill a random text box with the answer
                    while (Int32.TryParse(array[random1].Text, out temp))
                    {
                        if (temp != answers[random1])
                            break;
                        else
                            random1 = random.Next(0, array.Length);
                    }

                    array[random1].Text = answers[random1].ToString();

                }
            }


        }

        /***************************************************************
       public static void highlight3()

       Use: highlights a row, column, or diagonal to the user indiciating 
       a mistake was made. (if all textboxes of that row, col, diagonal,
       are non-empty and at least one is invalid it will highlight that
       row, col, diagonal)

       Parameters: none

       Returns: nothing
       ***************************************************************/
        public static void highlight3()
        {
            bool change;

            //check all the rows
            for (int k = 0; k < array.Length; k += 7)
            {
                change = false;

                for (int i = 0; i < 7; i++)
                {
                    if (array[k].Text != "" && array[k + 1].Text != "" && array[k + 2].Text != "" && array[k + 3].Text != "" && array[k + 4].Text != "" && array[k + 5].Text != "" && array[k + 6].Text != "")
                    {
                        if (array[k + i].Text != answers[k + i].ToString())
                            change = true;
                    }
                }

                if (change)
                {
                    for (int l = 0; l < 7; l++)
                        array[l + k].BackColor = Color.FromArgb(90, 0, 0);
                }
                else
                {
                    for (int l = 0; l < 7; l++)
                        array[l + k].BackColor = Color.Black;
                }
            }

            //check all the columns
            for (int k = 0; k < 7; k++)
            {
                change = false;

                for (int i = 0; i < array.Length; i += 7)
                {
                    if (array[k].Text != "" && array[k + 7].Text != "" && array[k + 14].Text != "" && array[k + 21].Text != "" && array[k + 28].Text != "" && array[k + 35].Text != "" && array[k + 42].Text != "")
                    {
                        if (array[k + i].Text != answers[k + i].ToString())
                            change = true;
                    }
                }

                if (change)
                {
                    for (int l = 0; l < array.Length; l += 7)
                        array[l + k].BackColor = Color.FromArgb(90, 0, 0);
                }

            }


            //first diagonal
            change = false;
            for (int i = 0; i < array.Length; i += 8)
            {
                if (array[0].Text != "" && array[8].Text != "" && array[16].Text != "" && array[24].Text != "" && array[32].Text != "" && array[40].Text != "" && array[48].Text != "")
                {
                    if (array[i].Text != answers[i].ToString())
                        change = true;
                }
            }

            if (change)
            {
                for (int l = 0; l < array.Length; l += 8)
                    array[l].BackColor = Color.FromArgb(90, 0, 0);
            }

            //second diagonal
            change = false;
            for (int i = 6; i < 43; i += 6)
            {
                if (array[6].Text != "" && array[12].Text != "" && array[18].Text != "" && array[24].Text != "" && array[30].Text != "" && array[36].Text != "" && array[42].Text != "")
                {
                    if (array[i].Text != answers[i].ToString())
                        change = true;
                }
            }

            if (change)
            {
                for (int l = 6; l < 43; l += 6)
                    array[l].BackColor = Color.FromArgb(90, 0, 0);
            }

        }

        /***************************************************************
      public static void highlight2()

      Use: highlights a row, column, or diagonal to the user indiciating 
      a mistake was made. (if all textboxes of that row, col, diagonal,
      are non-empty and at least one is invalid it will highlight that
      row, col, diagonal)

      Parameters: none

      Returns: nothing
      ***************************************************************/
        public static void highlight2()
        {
            bool change;

            //check the rows
            for (int k = 0; k < array.Length; k += 5)
            {
                change = false;

                for (int i = 0; i < 5; i++)
                {
                    if (array[k].Text != "" && array[k + 1].Text != "" && array[k + 2].Text != "" && array[k + 3].Text != "" && array[k + 4].Text != "")
                    {
                        if (array[k + i].Text != answers[k + i].ToString())
                            change = true;
                    }
                }

                if (change)
                {
                    for (int l = 0; l < 5; l++)
                        array[l + k].BackColor = Color.FromArgb(90, 0, 0);
                }
                else
                {
                    for (int l = 0; l < 5; l++)
                        array[l + k].BackColor = Color.Black;
                }
            }

            //check the columns
            for (int k = 0; k < 5; k++)
            {
                change = false;

                for (int i = 0; i < array.Length; i += 5)
                {
                    if (array[k].Text != "" && array[k + 5].Text != "" && array[k + 10].Text != "" && array[k + 15].Text != "" && array[k + 20].Text != "")
                    {
                        if (array[k + i].Text != answers[k + i].ToString())
                            change = true;
                    }
                }

                if (change)
                {
                    for (int l = 0; l < array.Length; l += 5)
                        array[l + k].BackColor = Color.FromArgb(90, 0, 0);
                }

            }


            //first diagonal
            change = false;
            for (int i = 0; i < array.Length; i += 6)
            {
                if (array[0].Text != "" && array[6].Text != "" && array[12].Text != "" && array[18].Text != "" && array[24].Text != "")
                {
                    if (array[i].Text != answers[i].ToString())
                        change = true;
                }
            }

            if (change)
            {
                for (int l = 0; l < array.Length; l += 6)
                    array[l].BackColor = Color.FromArgb(90, 0, 0);
            }

            //second diagonal
            change = false;
            for (int i = 4; i < 21; i += 4)
            {
                if (array[4].Text != "" && array[8].Text != "" && array[12].Text != "" && array[16].Text != "" && array[20].Text != "")
                {
                    if (array[i].Text != answers[i].ToString())
                        change = true;
                }
            }

            if (change)
            {
                for (int l = 4; l < 21; l += 4)
                    array[l].BackColor = Color.FromArgb(90, 0, 0);
            }


        }

        /***************************************************************
      public static void highlight()

      Use: highlights a row, column, or diagonal to the user indiciating 
      a mistake was made. (if all textboxes of that row, col, diagonal,
      are non-empty and at least one is invalid it will highlight that
      row, col, diagonal)

      Parameters: none

      Returns: nothing
      ***************************************************************/
        public static void highlight()
        {
            bool change;
            //check the rows
            for (int k = 0; k < 9; k += 3)
            {
                change = false;

                for (int i = 0; i < 3; i++)
                {
                    if (array[k].Text != "" && array[k + 1].Text != "" && array[k + 2].Text != "")
                    {
                        if (array[k + i].Text != answers[k + i].ToString())
                            change = true;
                    }
                }

                if (change)
                {
                    for (int l = 0; l < 3; l++)
                        array[l + k].BackColor = Color.FromArgb(90, 0, 0);
                }
                else
                {
                    for (int l = 0; l < 3; l++)
                        array[l + k].BackColor = Color.Black;
                }
            }
            //check the columns
            for (int k = 0; k < 3; k ++)
            {
                change = false;

                for (int i = 0; i < 9; i+=3)
                {
                    if (array[k].Text != "" && array[k + 3].Text != "" && array[k + 6].Text != "")
                    {
                        if (array[k + i].Text != answers[k + i].ToString())
                            change = true;
                    }
                  
                }

                if (change)
                {
                    for (int l = 0; l < 9; l+=3)
                        array[l + k].BackColor = Color.FromArgb(90, 0, 0);
                }
              
            }

            //first diagonal
            change = false;
            for (int i = 0; i < 9; i += 4)
            {
                if (array[0].Text != "" && array[4].Text != "" && array[8].Text != "")
                {
                    if (array[i].Text != answers[i].ToString())
                        change = true;
                }
            }

            if (change)
            {
                for (int l = 0; l < 9; l += 4)
                    array[l].BackColor = Color.FromArgb(90, 0, 0);
            }

            //second diagonal
            change = false;
            for (int i = 2; i < 7; i += 2)
            {
                if (array[2].Text != "" && array[4].Text != "" && array[6].Text != "")
                {
                    if (array[i].Text != answers[i].ToString())
                        change = true;
                }
            }

            if (change)
            {
                for (int l = 2; l < 7; l += 2)
                    array[l].BackColor = Color.FromArgb(90, 0, 0);
            }


        }

        /***************************************************************
        private void btnPause_Click(object sender, EventArgs e)

        Use: pauses the puzzle by calling hide to hide the puzzle
        and stopping the timer
     
        Parameters: 1. object sender - the calling object/button
                    2. EventArgs e - the calling event
			
        Returns: nothing
        ***************************************************************/
        private void btnPause_Click(object sender, EventArgs e)
        {
            Button pause = sender as Button;
            
            if (pause != null)
            {
               
                if (flag)
                {
                    time.Stop();

                    if (EASY)
                        hide();
                    else if (MEDIUM)
                        hide2();
                    else if (HARD)
                        hide3();

                    pbHide.BringToFront(); //black image to cover textboxes
                    btnPause.Text = "Resume";
                    flag = false;
                }
                else
                {
                    time.Start(); //restore the time 

                    if (EASY)
                        printAxis();
                    else if (MEDIUM)
                        printAxis2();
                    else if (HARD)
                        printAxis3();

                    pbHide.SendToBack(); //show the textboxes again
                    btnPause.Text = "Pause";
                    flag = true;
                }
                
            }
        }

        /***************************************************************
        private void timer_Tick(object sender, EventArgs e)

        Use: updates the label with the timer as it displays
     
        Parameters: 1. object sender - the calling object/button
                    2. EventArgs e - the calling event
			
        Returns: nothing
        ***************************************************************/
        private void timer_Tick(object sender, EventArgs e)
        {
            count++;
            TimeSpan result = TimeSpan.FromMilliseconds(count*100);
            lblTimer.Text = String.Format("{0:mm\\:ss\\.f}", result);
        }

        /***************************************************************
        private void btnHard_Click(object sender, EventArgs e)

        Use: gets a new hard puzzle, defaults to uncompleted,
        then saved, then completed 
     
        Parameters: 1. object sender - the calling object/button
                    2. EventArgs e - the calling event
			
        Returns: nothing
        ***************************************************************/
        private void btnHard_Click(object sender, EventArgs e)
        {
            Button hard = sender as Button;

            if (hard != null)
            {
                if (EASY || MEDIUM || HARD)
                {
                    for (int i = 0; i < array.Length; i++)
                    {
                        this.Controls.Remove(array[i]);
                        array[i].Dispose();
                        cheat = false;
                    }
                }

                if (EASY || MEDIUM)
                {
                    fastest = 0;
                    puzzleTime1 = puzzleTime2 = puzzleTime3 = 0;
                    totalCompleted = 0;
                    completionTime = 0;
                }

                EASY = false;
                MEDIUM = false;
                HARD = true;

                count = 0;
                time.Start();

                if (!flag)
                    btnPause_Click(sender, e); 

                //draw the playing field
                Graphics g = graph.CreateGraphics();
                SolidBrush blackBrush = new SolidBrush(Color.Black);
                g.FillRectangle(blackBrush, 0, 0, graph.Width, graph.Height);

                //draw the vertical lines
                g.DrawLine(myPen, graph.Width / 7, 0, graph.Width / 7, graph.Height);
                g.DrawLine(myPen, 2 * graph.Width / 7, 0, 2 * graph.Width / 7, graph.Height);
                g.DrawLine(myPen, 3 * graph.Width / 7, 0, 3 * graph.Width / 7, graph.Height);
                g.DrawLine(myPen, 4 * graph.Width / 7, 0, 4 * graph.Width / 7, graph.Height);
                g.DrawLine(myPen, 5 * graph.Width / 7, 0, 5 * graph.Width / 7, graph.Height);
                g.DrawLine(myPen, 6 * graph.Width / 7, 0, 6 * graph.Width / 7, graph.Height);

                //draw the horizontal lines
                g.DrawLine(myPen, 0, graph.Height / 7, graph.Width, graph.Height / 7);
                g.DrawLine(myPen, 0, 2 * graph.Height / 7, graph.Width, 2 * graph.Height / 7);
                g.DrawLine(myPen, 0, 3 * graph.Height / 7, graph.Width, 3 * graph.Height / 7);
                g.DrawLine(myPen, 0, 4 * graph.Height / 7, graph.Width, 4 * graph.Height / 7);
                g.DrawLine(myPen, 0, 5 * graph.Height / 7, graph.Width, 5 * graph.Height / 7);
                g.DrawLine(myPen, 0, 6 * graph.Height / 7, graph.Width, 6 * graph.Height / 7);


                num = (num % 3) + 1; //increment num

                string file_c = "..\\..\\hard/h" + num.ToString() + "c.txt";
                string file_s = "..\\..\\hard/h" + num.ToString() + "s.txt";
                string file = "..\\..\\hard/h" + num.ToString() + ".txt";

           
                if (File.Exists(file_s))
                    file = file_s;
                else if (File.Exists(file_c))
                    file = file_c;

                saveFile = "..\\..\\hard/h" + num.ToString() + "s.txt";
                completeFile = "..\\..\\hard/h" + num.ToString() + "c.txt";
                currentFile = file;

                string line;
                string wholeLine = "";
                // string[] tokens;
                using (StreamReader inFile = new StreamReader(file))
                {
                    line = inFile.ReadLine();
                    while (line != null)
                    {
                        wholeLine += line;
                        line = inFile.ReadLine();
                    }
                }


                //get the saved clock time 
                if (wholeLine.Length > 148)
                {
                    string clock = "";
                    for (int i = 147; i < wholeLine.Length; i++)
                        clock += wholeLine[i];

                    count = Convert.ToInt32(clock);
                }

                int x = 94;
                int y = 73;
                int j = 0;

                array = new TextBoxWithoutCaret[49];
                for (int i = 0; i < 49; i++)
                {
                    array[i] = new TextBoxWithoutCaret();
                    array[i].MaxLength = 1;
                    array[i].BackColor = Color.Black;

                    array[i].Font = new Font("Arial", 37);
                    array[i].Width = (graph.Width / 7);
                    array[i].Location = new Point(x, y);
                    array[i].TextAlign = HorizontalAlignment.Center;

                    if (wholeLine[i] != '0')
                    {
                        array[i].ForeColor = Color.FromArgb(75, 75, 75);
                        array[i].Text = wholeLine[i].ToString();
                        array[i].ReadOnly = true;
                    }
                    else
                        array[i].ForeColor = Color.White;

                    this.Controls.Add(array[i]);
                    array[i].BringToFront();
                    j++;
                    if (j % 7 == 0)
                    {
                        x = 95;
                        y += 64;
                    }
                    else
                        x += 64;

                    array[i].TextChanged += tb_TextChanged;
                    array[i].KeyPress += tb_KeyPress;
                }
                
                //if its greater than 18, then it is a save file
                if (wholeLine.Length > 99)
                {
                    for (int i = 98; i < 147; i++)
                    {
                        if (array[i - 98].ReadOnly == false)
                        {
                            if (wholeLine[i].CompareTo('0') != 0)
                            {
                                array[i - 98].TextChanged -= tb_TextChanged;
                                array[i - 98].Text = wholeLine[i].ToString();
                                array[i - 98].TextChanged += tb_TextChanged;
                            }
                        }

                    }
                }

                //populate the answers array
                answers = new int[49];
                for (int i = 0; i < 49; i++)
                    answers[i] = Convert.ToInt32(wholeLine[i + 49].ToString());

                original = new string[49];
                for (int i = 0; i < 49; i++)
                    original[i] = (wholeLine[i].ToString());


                diag1 = answers[0] + answers[8] + answers[16] + answers[24] + answers[32] + answers[40] + answers[48];
                diag2 = answers[6] + answers[12] + answers[18] + answers[24] + answers[30] + answers[36] + answers[42];

                row1 = answers[0] + answers[1] + answers[2] + answers[3] + answers[4] + answers[5] + answers[6];
                row2 = answers[7] + answers[8] + answers[9] + answers[10] + answers[11] + answers[12] + answers[13];
                row3 = answers[14] + answers[15] + answers[16] + answers[17] + answers[18] + answers[19] + answers[20];
                row4 = answers[21] + answers[22] + answers[23] + answers[24] + answers[25] + answers[26] + answers[27];
                row5 = answers[28] + answers[29] + answers[30] + answers[31] + answers[32] + answers[33] + answers[34];
                row6 = answers[35] + answers[36] + answers[37] + answers[38] + answers[39] + answers[40] + answers[41];
                row7 = answers[42] + answers[43] + answers[44] + answers[45] + answers[46] + answers[47] + answers[48];

                col1 = answers[0] + answers[7] + answers[14] + answers[21] + answers[28] + answers[35] + answers[42];
                col2 = answers[1] + answers[8] + answers[15] + answers[22] + answers[29] + answers[36] + answers[43];
                col3 = answers[2] + answers[9] + answers[16] + answers[23] + answers[30] + answers[37] + answers[44];
                col4 = answers[3] + answers[10] + answers[17] + answers[24] + answers[31] + answers[38] + answers[45];
                col5 = answers[4] + answers[11] + answers[18] + answers[25] + answers[32] + answers[39] + answers[46];
                col6 = answers[5] + answers[12] + answers[19] + answers[26] + answers[33] + answers[40] + answers[47];
                col7 = answers[6] + answers[13] + answers[20] + answers[27] + answers[34] + answers[41] + answers[48];
                //print the current input 
                printAxis3();

            }
        }

        /***************************************************************
        private void btnMedium_Click(object sender, EventArgs e)

        Use: gets a new medium puzzle, defaults to uncompleted,
        then saved, then completed
     
        Parameters: 1. object sender - the calling object/button
                    2. EventArgs e - the calling event
			
        Returns: nothing
        ***************************************************************/
        private void btnMedium_Click(object sender, EventArgs e)
        {
            Button medium = sender as Button;

            if (medium != null)
            {
                if (EASY || MEDIUM || HARD)
                {
                    for (int i = 0; i < array.Length; i++)
                    {
                        this.Controls.Remove(array[i]);
                        array[i].Dispose();
                        cheat = false;
                    }
                }

                if (EASY || HARD)
                {
                    fastest = 0;
                    puzzleTime1 = puzzleTime2 = puzzleTime3 = 0;
                    totalCompleted = 0;
                    completionTime = 0;
                }

                EASY = false;
                MEDIUM = true;
                HARD = false;

                count = 0;
                time.Start();

                if (!flag)
                    btnPause_Click(sender, e); //need to define for medium

                //draw the playing field
                Graphics g = graph.CreateGraphics();
                SolidBrush blackBrush = new SolidBrush(Color.Black);
                g.FillRectangle(blackBrush, 0, 0, graph.Width, graph.Height);

                //draw the vertical lines
                g.DrawLine(myPen, graph.Width / 5, 0, graph.Width / 5, graph.Height);
                g.DrawLine(myPen, 2 * graph.Width / 5, 0, 2 * graph.Width / 5, graph.Height);
                g.DrawLine(myPen, 3*graph.Width / 5, 0, 3* graph.Width / 5, graph.Height);
                g.DrawLine(myPen, 4 * graph.Width / 5, 0, 4 * graph.Width / 5, graph.Height);

                //draw the horizontal lines
                g.DrawLine(myPen, 0, graph.Height / 5, graph.Width, graph.Height / 5);
                g.DrawLine(myPen, 0, 2 * graph.Height / 5, graph.Width, 2 * graph.Height / 5);
                g.DrawLine(myPen, 0, 3 * graph.Height / 5, graph.Width, 3 * graph.Height / 5);
                g.DrawLine(myPen, 0, 4 * graph.Height / 5, graph.Width, 4 * graph.Height / 5);


                num = (num % 3) + 1; //increment num

                string file_c = "..\\..\\medium/m" + num.ToString() + "c.txt";
                string file_s = "..\\..\\medium/m" + num.ToString() + "s.txt";
                string file = "..\\..\\medium/m" + num.ToString() + ".txt";

                if (File.Exists(file_s))
                    file = file_s;
                else if (File.Exists(file_c))
                    file = file_c;

                saveFile = "..\\..\\medium/m" + num.ToString() + "s.txt";
                completeFile = "..\\..\\medium/m" + num.ToString() + "c.txt";
                currentFile = file;

                string line;
                string wholeLine = "";
                // string[] tokens;
                using (StreamReader inFile = new StreamReader(file))
                {
                    line = inFile.ReadLine();
                    while (line != null)
                    {
                        wholeLine += line;
                        line = inFile.ReadLine();
                    }
                }


                //get the saved clock time 
                if (wholeLine.Length > 76)
                {
                    string clock = "";
                    for (int i = 75; i < wholeLine.Length; i++)
                        clock += wholeLine[i];

                    count = Convert.ToInt32(clock);
                }

                int x = 94;
                int y = 73;
                int j = 0;

                array = new TextBoxWithoutCaret[25];
                for (int i = 0; i < 25; i++)
                {
                    array[i] = new TextBoxWithoutCaret();
                    array[i].MaxLength = 1;
                    array[i].BackColor = Color.Black;

                    array[i].Font = new Font("Arial", 55);
                    array[i].Width = (graph.Width / 5);
                    array[i].Location = new Point(x, y);
                    array[i].TextAlign = HorizontalAlignment.Center;

                    if (wholeLine[i] != '0')
                    {
                        array[i].ForeColor = Color.FromArgb(75, 75, 75);
                        array[i].Text = wholeLine[i].ToString();
                        array[i].ReadOnly = true;
                    }
                    else
                        array[i].ForeColor = Color.White;

                    this.Controls.Add(array[i]);
                    array[i].BringToFront();
                    j++;
                    if (j % 5 == 0)
                    {
                        x = 95;
                        y += 90;
                    }
                    else
                        x += 89;

                    array[i].TextChanged += tb_TextChanged;
                    array[i].KeyPress += tb_KeyPress;
                }

                //if its greater than 18, then it is a save file
                if (wholeLine.Length > 51)
                {
                    for (int i = 50; i < 75; i++)
                    {
                        if (array[i-50].ReadOnly == false)
                        {
                            if (wholeLine[i].CompareTo('0') != 0)
                            {
                                array[i - 50].TextChanged -= tb_TextChanged;
                                array[i - 50].Text = wholeLine[i].ToString();
                                array[i - 50].TextChanged += tb_TextChanged;
                            }
                        }

                    }
                }

                //populate the answers array
                answers = new int[25];
                for (int i = 0; i < 25; i++)
                    answers[i] = Convert.ToInt32(wholeLine[i + 25].ToString());

                original = new string[25];
                for (int i = 0; i < 25; i++)
                    original[i] = (wholeLine[i].ToString());


                diag1 = answers[0] + answers[6] + answers[12] + answers[18] + answers[24];
                diag2 = answers[4] + answers[8] + answers[12] + answers[16] + answers[20];

                row1 = answers[0] + answers[1] + answers[2] + answers[3] + answers[4];
                row2 = answers[5] + answers[6] + answers[7] + answers[8] + answers[9];
                row3 = answers[10] + answers[11] + answers[12] + answers[13] + answers[14];
                row4 = answers[15] + answers[16] + answers[17] + answers[18] + answers[19];
                row5 = answers[20] + answers[21] + answers[22] + answers[23] + answers[24];

                col1 = answers[0] + answers[5] + answers[10] + answers[15] + answers[20];
                col2 = answers[1] + answers[6] + answers[11] + answers[16] + answers[21];
                col3 = answers[2] + answers[7] + answers[12] + answers[17] + answers[22];
                col4 = answers[3] + answers[8] + answers[13] + answers[18] + answers[23];
                col5 = answers[4] + answers[9] + answers[14] + answers[19] + answers[24];

                //print the current input 
                printAxis2();


            }
        }

        /***************************************************************
        private void btnEasy_Click(object sender, EventArgs e)

        Use: gets a new easy puzzle, defaults to uncompleted,
        then saved, then completed
     
        Parameters: 1. object sender - the calling object/button
                    2. EventArgs e - the calling event
			
        Returns: nothing
        ***************************************************************/
        private void btnEasy_Click(object sender, EventArgs e)
        {
            Button easy = sender as Button;

            if (easy != null)
            {
                if (EASY || MEDIUM || HARD)
                {
                    for (int i = 0; i < array.Length; i++)
                    {
                        this.Controls.Remove(array[i]);
                        array[i].Dispose();
                        cheat = false;
                    }
                }

                if (MEDIUM || HARD)
                {
                    fastest = 0;
                    puzzleTime1 = puzzleTime2 = puzzleTime3 = 0;
                    totalCompleted = 0;
                    completionTime = 0;
                }

                EASY = true;
                MEDIUM = false;
                HARD = false;

                count = 0;
                time.Start();

                if (!flag)
                    btnPause_Click(sender, e);

                //draw the playing field
                Graphics g = graph.CreateGraphics();
                SolidBrush blackBrush = new SolidBrush(Color.Black);
                g.FillRectangle(blackBrush, 0, 0, graph.Width, graph.Height);

                //draw the vertical lines
                g.DrawLine(myPen, graph.Width / 3, 0, graph.Width / 3, graph.Height);
                g.DrawLine(myPen, 2 * graph.Width / 3, 0, 2 * graph.Width / 3, graph.Height);

                //draw the horizontal lines
                g.DrawLine(myPen, 0, graph.Height / 3, graph.Width, graph.Height / 3);
                g.DrawLine(myPen, 0, 2 * graph.Height / 3, graph.Width, 2 * graph.Height / 3);


                // int fileCount = Directory.GetFiles("..\\..\\easy", "*", SearchOption.TopDirectoryOnly).Length;

                num = (num % 3) + 1; //increment num

                string file_c = "..\\..\\easy/e" + num.ToString() + "c.txt";
                string file_s = "..\\..\\easy/e" + num.ToString() + "s.txt";
                string file = "..\\..\\easy/e" + num.ToString() + ".txt";

                if (File.Exists(file_s))
                    file = file_s;
                else if (File.Exists(file_c))
                    file = file_c;
                
                saveFile = "..\\..\\easy/e" + num.ToString() + "s.txt";
                completeFile = "..\\..\\easy/e" + num.ToString() + "c.txt";
                currentFile = file;
             
                string line;
                string wholeLine = "";
               // string[] tokens;
                using (StreamReader inFile = new StreamReader(file))
                {
                    line = inFile.ReadLine();
                    while (line != null)
                    {
                        wholeLine += line;
                        line = inFile.ReadLine();
                    }
                }

               
                //get the saved clock time 
                if (wholeLine.Length > 27)
                {
                    string clock = "";
                    for (int i = 27; i < wholeLine.Length; i++)
                        clock += wholeLine[i];

                    count = Convert.ToInt32(clock);
                }

                int x = 94;
                int y = 73;
                int j = 0;

                array = new TextBoxWithoutCaret[9];
                for (int i = 0; i < 9; i++)
                {
                    array[i] = new TextBoxWithoutCaret();
                    array[i].MaxLength = 1;
                    array[i].BackColor = Color.Black;

                    array[i].Font = new Font("Arial", 95);
                    array[i].Width = (graph.Width / 3);
                    array[i].Location = new Point(x, y);
                    array[i].TextAlign = HorizontalAlignment.Center;

                    if (wholeLine[i] != '0')
                    {
                        array[i].ForeColor = Color.FromArgb(75, 75, 75);
                        array[i].Text = wholeLine[i].ToString();
                        array[i].ReadOnly = true;
                    }
                    else
                        array[i].ForeColor = Color.White;
                  


                    this.Controls.Add(array[i]);
                    array[i].BringToFront();
                    j++;
                    if (j % 3 == 0)
                    {
                        x = 95;
                        y += 150;
                    }
                    else
                        x += 150;

                    array[i].TextChanged += tb_TextChanged;
                    array[i].KeyPress += tb_KeyPress;
                }

                //if its greater than 18, then it is a save file
                if (wholeLine.Length > 18) 
                {
                    for (int i = 18; i < 27; i++)
                    {
                        if (array[i-18].ReadOnly == false)
                        {
                            if (wholeLine[i].CompareTo('0') != 0)
                            {
                                array[i - 18].TextChanged -= tb_TextChanged;
                                array[i - 18].Text = wholeLine[i].ToString();
                                array[i - 18].TextChanged += tb_TextChanged;
                            }
                        }
                            
                    }       
                }

                //populate the answers array
                answers = new int[9];
                for (int i = 0; i < 9; i++)
                    answers[i] = Convert.ToInt32(wholeLine[i + 9].ToString());

                original = new string[9];
                for (int i = 0; i < 9; i++)
                    original[i] = (wholeLine[i].ToString());


                diag1 = Convert.ToInt32(wholeLine[9].ToString()) + Convert.ToInt32(wholeLine[13].ToString()) + Convert.ToInt32(wholeLine[17].ToString());
                diag2 = Convert.ToInt32(wholeLine[11].ToString()) + Convert.ToInt32(wholeLine[13].ToString()) + Convert.ToInt32(wholeLine[15].ToString());

                row1 = Convert.ToInt32(wholeLine[9].ToString()) + Convert.ToInt32(wholeLine[10].ToString()) + Convert.ToInt32(wholeLine[11].ToString());
                row2 = Convert.ToInt32(wholeLine[12].ToString()) + Convert.ToInt32(wholeLine[13].ToString()) + Convert.ToInt32(wholeLine[14].ToString());
                row3 = Convert.ToInt32(wholeLine[15].ToString()) + Convert.ToInt32(wholeLine[16].ToString()) + Convert.ToInt32(wholeLine[17].ToString());

                col1 = Convert.ToInt32(wholeLine[9].ToString()) + Convert.ToInt32(wholeLine[12].ToString()) + Convert.ToInt32(wholeLine[15].ToString());
                col2 = Convert.ToInt32(wholeLine[10].ToString()) + Convert.ToInt32(wholeLine[13].ToString()) + Convert.ToInt32(wholeLine[16].ToString());
                col3 = Convert.ToInt32(wholeLine[11].ToString()) + Convert.ToInt32(wholeLine[14].ToString()) + Convert.ToInt32(wholeLine[17].ToString());

                //print the current input 
                printAxis();
           
           
            }

        }

        /***************************************************************
        private void tb_TextChanged(object sender, EventArgs e)

        Use: if text is changed, we check if something needs to be highlighted
        by calling the respective highlight method. Then we check for puzzle
        completion. We also display to the user if they are doing well so far
        (all answers are currently correct) or should fix something
        (might wanna get that checked out)

        Parameters: 1. object sender - the calling object/button
                    2. EventArgs e - the calling event

        Returns: nothing
        ***************************************************************/
        private void tb_TextChanged(object sender, EventArgs e)
        {
            TextBox tb = sender as TextBox;

            if (tb != null)
            {
                if (EASY)
                {
                    highlight();
                    printAxis();
                }
                else if (MEDIUM)
                {
                    highlight2();
                    printAxis2();
                }
                else if (HARD)
                {
                    highlight3();
                    printAxis3();
                }
  
                int total = 0;
                int zeros = 0;
                for (int i = 0; i < answers.Length; i++)
                {
                    if (array2[i] == 0)
                        zeros++;
                    else if (answers[i] == array2[i])
                        total++;
                }

                if ((total + zeros) == answers.Length)
                    r.Text = "You're doing well so far!";
                else
                    r.Text = "Might wanna get that checked out";

                complete();

            }
        }

        /***************************************************************
          private void tb_KeyPress(object sender, KeyPressEventArgs e)

          Use: accepts only digits 1-9 from the keyboard and backspaces

          Parameters: 1. object sender - the calling object/button
                      2. EventArgs e - the calling event

          Returns: nothing
          ***************************************************************/
        private void tb_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(e.KeyChar == (char)Keys.Back || (e.KeyChar > '0' && e.KeyChar <= '9') ))
            {
                e.Handled = true;
            }
        }

        /***************************************************************
        private void hide3()

        Use: hides the current sums and correct sums to the user for the
        hard puzzle variant

        Parameters: none 

        Returns: nothing
        ***************************************************************/
        private void hide3()
        {
            //draw the sum lines up top
            Graphics g1 = top.CreateGraphics();
            g1.FillRectangle(new SolidBrush(Color.Black), 0, 0, top.Width, top.Height);
            g1.DrawLine(myPen, top.Width / 7, 0, top.Width / 7, top.Height);
            g1.DrawLine(myPen, 2 * top.Width / 7, 0, 2 * top.Width / 7, top.Height);
            g1.DrawLine(myPen, 3 * top.Width / 7, 0, 3 * top.Width / 7, top.Height);
            g1.DrawLine(myPen, 4 * top.Width / 7, 0, 4 * top.Width / 7, top.Height);
            g1.DrawLine(myPen, 5 * top.Width / 7, 0, 5 * top.Width / 7, top.Height);
            g1.DrawLine(myPen, 6 * top.Width / 7, 0, 6 * top.Width / 7, top.Height);

            //draw the left side axis lines 
            Graphics g2 = left.CreateGraphics();
            g2.FillRectangle(new SolidBrush(Color.Black), 0, 0, left.Width, left.Height);
            g2.DrawLine(myPen, 0, graph.Height / 6.5f, graph.Width, graph.Height / 6.5f);
            g2.DrawLine(myPen, 0, graph.Height / 6.5f + array[0].Height, graph.Width, graph.Height / 6.5f + array[0].Height);
            g2.DrawLine(myPen, 0, graph.Height / 6.5f + 2 * array[0].Height, graph.Width, graph.Height / 6.5f + 2 * array[0].Height);
            g2.DrawLine(myPen, 0, graph.Height / 6.5f + 3 * array[0].Height, graph.Width, graph.Height / 6.5f + 3 * array[0].Height);
            g2.DrawLine(myPen, 0, graph.Height / 6.5f + 4 * array[0].Height, graph.Width, graph.Height / 6.5f + 4 * array[0].Height);
            g2.DrawLine(myPen, 0, graph.Height / 6.5f + 5 * array[0].Height, graph.Width, graph.Height / 6.5f + 5 * array[0].Height);
            g2.DrawLine(myPen, 0, graph.Height / 6.5f + 6 * array[0].Height, graph.Width, graph.Height / 6.5f + 6 * array[0].Height);
            g2.DrawLine(myPen, 0, (3 * graph.Height / 2.05f - 2 * graph.Height / 6.5f), graph.Width, (3 * graph.Height / 2.05f - 2 * graph.Height / 6.5f));

            //draw the bot axis lines 
            Graphics g3 = bot.CreateGraphics();
            g3.FillRectangle(new SolidBrush(Color.Black), 0, 0, bot.Width, bot.Height);
            g3.DrawLine(myPen, top.Width / 7, 0, top.Width / 7, top.Height);
            g3.DrawLine(myPen, 2 * top.Width / 7, 0, 2 * top.Width / 7, top.Height);
            g3.DrawLine(myPen, 3 * top.Width / 7, 0, 3 * top.Width / 7, top.Height);
            g3.DrawLine(myPen, 4 * top.Width / 7, 0, 4 * top.Width / 7, top.Height);
            g3.DrawLine(myPen, 5 * top.Width / 7, 0, 5 * top.Width / 7, top.Height);
            g3.DrawLine(myPen, 6 * top.Width / 7, 0, 6 * top.Width / 7, top.Height);

            //draw the right side axis lines 
            Graphics g4 = right.CreateGraphics();
            g4.FillRectangle(new SolidBrush(Color.Black), 0, 0, right.Width, right.Height);
            g4.DrawLine(myPen, 0, graph.Height / 6.5f, graph.Width, graph.Height / 6.5f);
            g4.DrawLine(myPen, 0, graph.Height / 6.5f + array[0].Height, graph.Width, graph.Height / 6.5f + array[0].Height);
            g4.DrawLine(myPen, 0, graph.Height / 6.5f + 2 * array[0].Height, graph.Width, graph.Height / 6.5f + 2 * array[0].Height);
            g4.DrawLine(myPen, 0, graph.Height / 6.5f + 3 * array[0].Height, graph.Width, graph.Height / 6.5f + 3 * array[0].Height);
            g4.DrawLine(myPen, 0, graph.Height / 6.5f + 4 * array[0].Height, graph.Width, graph.Height / 6.5f + 4 * array[0].Height);
            g4.DrawLine(myPen, 0, graph.Height / 6.5f + 5 * array[0].Height, graph.Width, graph.Height / 6.5f + 5 * array[0].Height);
            g4.DrawLine(myPen, 0, graph.Height / 6.5f + 6 * array[0].Height, graph.Width, graph.Height / 6.5f + 6 * array[0].Height);
            g4.DrawLine(myPen, 0, (3 * graph.Height / 2.05f - 2 * graph.Height / 6.5f), graph.Width, (3 * graph.Height / 2.05f - 2 * graph.Height / 6.5f));

            g1.DrawString("?", new Font("Arial", 30), myBrush, 0, top.Height / 6);
            g1.DrawString("?", new Font("Arial", 30), myBrush, graph.Width / 7 + graph.Width / 100, top.Height / 6);
            g1.DrawString("?", new Font("Arial", 30), myBrush, 2 * graph.Width / 7 + graph.Width / 100, top.Height / 6);
            g1.DrawString("?", new Font("Arial", 30), myBrush, 3 * graph.Width / 7 + graph.Width / 100, top.Height / 6);
            g1.DrawString("?", new Font("Arial", 30), myBrush, 4 * graph.Width / 7 + graph.Width / 100, top.Height / 6);
            g1.DrawString("?", new Font("Arial", 30), myBrush, 5 * graph.Width / 7 + graph.Width / 100, top.Height / 6);
            g1.DrawString("?", new Font("Arial", 30), myBrush, 6 * graph.Width / 7 + graph.Width / 100, top.Height / 6);

            //row1 sums and diags
            g2.DrawString("?", new Font("Arial", 30), myBrush, 0, graph.Height / 30);
            g2.DrawString("?", new Font("Arial", 30), myBrush, 0, graph.Height / 7 + graph.Height / 30);
            g2.DrawString("?", new Font("Arial", 30), myBrush, 0, 2 * graph.Height / 7 + graph.Height / 30);
            g2.DrawString("?", new Font("Arial", 30), myBrush, 0, 3 * graph.Height / 7 + graph.Height / 30);
            g2.DrawString("?", new Font("Arial", 30), myBrush, 0, 4 * graph.Height / 7 + graph.Height / 30);
            g2.DrawString("?", new Font("Arial", 30), myBrush, 0, 5 * graph.Height / 7 + graph.Height / 30);
            g2.DrawString("?", new Font("Arial", 30), myBrush, 0, 6 * graph.Height / 7 + graph.Height / 30);
            g2.DrawString("?", new Font("Arial", 30), myBrush, 0, graph.Height + graph.Height / 30);
            g2.DrawString("?", new Font("Arial", 30), myBrush, 0, graph.Height / 0.85f);

            g3.DrawString("?", new Font("Arial", 30), myBrush, 0, bot.Height / 12);
            g3.DrawString("?", new Font("Arial", 30), myBrush, array[0].Width, bot.Height / 12);
            g3.DrawString("?", new Font("Arial", 30), myBrush, 2 * array[0].Width, bot.Height / 12);
            g3.DrawString("?", new Font("Arial", 30), myBrush, 3 * array[0].Width, bot.Height / 12);
            g3.DrawString("?", new Font("Arial", 30), myBrush, 4 * array[0].Width, bot.Height / 12);
            g3.DrawString("?", new Font("Arial", 30), myBrush, 5 * array[0].Width, bot.Height / 12);
            g3.DrawString("?", new Font("Arial", 30), myBrush, 6 * array[0].Width, bot.Height / 12);
            g4.DrawString("?", new Font("Arial", 30), myBrush, 0, right.Height / 50);
            g4.DrawString("?", new Font("Arial", 30), myBrush, 0, right.Height / 50 + array[0].Height);
            g4.DrawString("?", new Font("Arial", 30), myBrush, 0, right.Height / 50 + 2 * array[0].Height);
            g4.DrawString("?", new Font("Arial", 30), myBrush, 0, right.Height / 50 + 3 * array[0].Height);
            g4.DrawString("?", new Font("Arial", 30), myBrush, 0, right.Height / 50 + 4 * array[0].Height);
            g4.DrawString("?", new Font("Arial", 30), myBrush, 0, right.Height / 50 + 5 * array[0].Height);
            g4.DrawString("?", new Font("Arial", 30), myBrush, 0, right.Height / 50 + 6 * array[0].Height);
            g4.DrawString("?", new Font("Arial", 30), myBrush, 0, right.Height / 50 + 7 * array[0].Height);
            g4.DrawString("?", new Font("Arial", 30), myBrush, 0, right.Height / 30 + 8 * array[0].Height);
        }

        /***************************************************************
        private void hide2()

        Use: hides the current sums and correct sums to the user for the
        medium puzzle variant

        Parameters: none 

        Returns: nothing
        ***************************************************************/
        private void hide2()
        {
            //draw the sum lines up top
            Graphics g1 = top.CreateGraphics();
            g1.FillRectangle(new SolidBrush(Color.Black), 0, 0, top.Width, top.Height);
            g1.DrawLine(myPen, top.Width / 5, 0, top.Width / 5, top.Height);
            g1.DrawLine(myPen, 2 * top.Width / 5, 0, 2 * top.Width / 5, top.Height);
            g1.DrawLine(myPen, 3 * top.Width / 5, 0, 3 * top.Width / 5, top.Height);
            g1.DrawLine(myPen, 4 * top.Width / 5, 0, 4 * top.Width / 5, top.Height);

            //draw the left side axis lines 
            Graphics g2 = left.CreateGraphics();
            g2.FillRectangle(new SolidBrush(Color.Black), 0, 0, left.Width, left.Height);
            g2.DrawLine(myPen, 0, graph.Height / 6.5f, graph.Width, graph.Height / 6.5f);
            g2.DrawLine(myPen, 0, left.Height / 5 + graph.Height / 10.8f, graph.Width, left.Height / 5 + graph.Height / 10.8f);
            g2.DrawLine(myPen, 0, 2 * left.Height / 5 + graph.Height / 29.8f, graph.Width, 2 * left.Height / 5 + graph.Height / 29.8f);
            g2.DrawLine(myPen, 0, 3 * left.Height / 5 - graph.Height / 29.8f, graph.Width, 3 * left.Height / 5 - graph.Height / 29.8f);
            g2.DrawLine(myPen, 0, 4 * left.Height / 5 - graph.Height / 10.8f, graph.Width, 4 * left.Height / 5 - graph.Height / 10.8f);
            g2.DrawLine(myPen, 0, (3 * graph.Height / 2.05f - 2 * graph.Height / 6.5f), graph.Width, (3 * graph.Height / 2.05f - 2 * graph.Height / 6.5f));

            //draw the bot axis lines 
            Graphics g3 = bot.CreateGraphics();
            g3.FillRectangle(new SolidBrush(Color.Black), 0, 0, bot.Width, bot.Height);
            g3.DrawLine(myPen, top.Width / 5, 0, top.Width / 5, top.Height);
            g3.DrawLine(myPen, 2 * top.Width / 5, 0, 2 * top.Width / 5, top.Height);
            g3.DrawLine(myPen, 3 * top.Width / 5, 0, 3 * top.Width / 5, top.Height);
            g3.DrawLine(myPen, 4 * top.Width / 5, 0, 4 * top.Width / 5, top.Height);

            //draw the right side axis lines 
            Graphics g4 = right.CreateGraphics();
            g4.FillRectangle(new SolidBrush(Color.Black), 0, 0, right.Width, right.Height);
            g4.DrawLine(myPen, 0, graph.Height / 6.5f, graph.Width, graph.Height / 6.5f);
            g4.DrawLine(myPen, 0, left.Height / 5 + graph.Height / 10.8f, graph.Width, left.Height / 5 + graph.Height / 10.8f);
            g4.DrawLine(myPen, 0, 2 * left.Height / 5 + graph.Height / 29.8f, graph.Width, 2 * left.Height / 5 + graph.Height / 29.8f);
            g4.DrawLine(myPen, 0, 3 * left.Height / 5 - graph.Height / 29.8f, graph.Width, 3 * left.Height / 5 - graph.Height / 29.8f);
            g4.DrawLine(myPen, 0, 4 * left.Height / 5 - graph.Height / 10.8f, graph.Width, 4 * left.Height / 5 - graph.Height / 10.8f);
            g4.DrawLine(myPen, 0, (3 * graph.Height / 2.05f - 2 * graph.Height / 6.5f), graph.Width, (3 * graph.Height / 2.05f - 2 * graph.Height / 6.5f));


            //col1 sums
            g1.DrawString("?", new Font("Arial", 40), myBrush, 0, top.Height / 6);
            g1.DrawString("?", new Font("Arial", 40), myBrush, graph.Width / 5 + graph.Width / 45, top.Height / 6);
            g1.DrawString("?", new Font("Arial", 40), myBrush, 2 * graph.Width / 5 + graph.Width / 45, top.Height / 6);
            g1.DrawString("?", new Font("Arial", 40), myBrush, 3 * graph.Width / 5 + graph.Width / 45, top.Height / 6);
            g1.DrawString("?", new Font("Arial", 40), myBrush, 4 * graph.Width / 5 + graph.Width / 45, top.Height / 6);

            //row1 sums and diags
            g2.DrawString("?", new Font("Arial", 40), myBrush, 0, graph.Height / 100);
            g2.DrawString("?", new Font("Arial", 40), myBrush, 0, graph.Height / 5 + graph.Height / 100);
            g2.DrawString("?", new Font("Arial", 40), myBrush, 0, 2 * graph.Height / 5 + graph.Height / 100);
            g2.DrawString("?", new Font("Arial", 40), myBrush, 0, 3 * graph.Height / 5 + graph.Height / 100);
            g2.DrawString("?", new Font("Arial", 40), myBrush, 0, 4 * graph.Height / 5 + graph.Height / 100);
            g2.DrawString("?", new Font("Arial", 40), myBrush, 0, graph.Height + graph.Height / 100);
            g2.DrawString("?", new Font("Arial", 40), myBrush, 0, graph.Height / 0.85f);

            g3.DrawString("?", new Font("Arial", 40), myBrush, bot.Width / 50, bot.Height / 12);
            g3.DrawString("?", new Font("Arial", 40), myBrush, bot.Width / 50 + array[0].Width, bot.Height / 12);
            g3.DrawString("?", new Font("Arial", 40), myBrush, bot.Width / 50 + 2 * array[0].Width, bot.Height / 12);
            g3.DrawString("?", new Font("Arial", 40), myBrush, bot.Width / 50 + 3 * array[0].Width, bot.Height / 12);
            g3.DrawString("?", new Font("Arial", 40), myBrush, bot.Width / 50 + 4 * array[0].Width, bot.Height / 12);
            g4.DrawString("?", new Font("Arial", 40), myBrush, 0, right.Height / 50);
            g4.DrawString("?", new Font("Arial", 40), myBrush, 0, array[0].Height);
            g4.DrawString("?", new Font("Arial", 40), myBrush, 0, 2 * array[0].Height);
            g4.DrawString("?", new Font("Arial", 40), myBrush, 0, 3 * array[0].Height);
            g4.DrawString("?", new Font("Arial", 40), myBrush, 0, 4 * array[0].Height);
            g4.DrawString("?", new Font("Arial", 40), myBrush, 0, 5 * array[0].Height);
            g4.DrawString("?", new Font("Arial", 40), myBrush, 0, right.Height - right.Height / 10);
        }

        /***************************************************************
        private void hide()

        Use: hides the current sums and correct sums to the user for the
        easy puzzle variant

        Parameters: none 

        Returns: nothing
        ***************************************************************/
        private void hide()
        {

            myBrush.Color = Color.Gray;

            //draw the sum lines up top
            Graphics g1 = top.CreateGraphics();
            g1.FillRectangle(new SolidBrush(Color.Black), 0, 0, top.Width, top.Height);
            g1.DrawLine(myPen, graph.Width / 3, 0, graph.Width / 3, graph.Height);
            g1.DrawLine(myPen, 2 * graph.Width / 3, 0, 2 * graph.Width / 3, graph.Height);

            //draw the left side axis lines 
            Graphics g2 = left.CreateGraphics();
            g2.FillRectangle(new SolidBrush(Color.Black), 0, 0, left.Width, left.Height);
            g2.DrawLine(myPen, 0, graph.Height / 6.5f, graph.Width, graph.Height / 6.5f);
            g2.DrawLine(myPen, 0, graph.Height / 2.05f, graph.Width, graph.Height / 2.05f);
            g2.DrawLine(myPen, 0, (2 * graph.Height / 2.05f - graph.Height / 6.5f), graph.Width, (2 * graph.Height / 2.05f - graph.Height / 6.5f));
            g2.DrawLine(myPen, 0, (3 * graph.Height / 2.05f - 2 * graph.Height / 6.5f), graph.Width, (3 * graph.Height / 2.05f - 2 * graph.Height / 6.5f));

            //col1 sums
            g1.DrawString("?", new Font("Arial", 40), myBrush, graph.Width / 11, top.Height / 6);
            g1.DrawString("?", new Font("Arial", 40), myBrush, graph.Width / 2 - (graph.Width / 14), top.Height / 6);
            g1.DrawString("?", new Font("Arial", 40), myBrush, graph.Width - (graph.Width / 4), top.Height / 6);

            //row1 sums and diags
            g2.DrawString("?", new Font("Arial", 40), myBrush, 0, graph.Height / 100);
            g2.DrawString("?", new Font("Arial", 40), myBrush, 0, graph.Height / 4);
            g2.DrawString("?", new Font("Arial", 40), myBrush, 0, graph.Height / 2 + (graph.Height / 10));
            g2.DrawString("?", new Font("Arial", 40), myBrush, 0, graph.Height / 1.1f);
            g2.DrawString("?", new Font("Arial", 40), myBrush, 0, graph.Height / 0.85f);

            //draw the bot axis lines 
            Graphics g3 = bot.CreateGraphics();
            g3.FillRectangle(new SolidBrush(Color.Black), 0, 0, bot.Width, bot.Height);
            g3.DrawLine(myPen, graph.Width / 3, 0, graph.Width / 3, graph.Height);
            g3.DrawLine(myPen, 2 * graph.Width / 3, 0, 2 * graph.Width / 3, graph.Height);

            //draw the right side axis lines 
            Graphics g4 = right.CreateGraphics();
            g4.FillRectangle(new SolidBrush(Color.Black), 0, 0, right.Width, right.Height);
            g4.DrawLine(myPen, 0, graph.Height / 6.5f, graph.Width, graph.Height / 6.5f);
            g4.DrawLine(myPen, 0, graph.Height / 2.05f, graph.Width, graph.Height / 2.05f);
            g4.DrawLine(myPen, 0, (2 * graph.Height / 2.05f - graph.Height / 6.5f), graph.Width, (2 * graph.Height / 2.05f - graph.Height / 6.5f));
            g4.DrawLine(myPen, 0, (3 * graph.Height / 2.05f - 2 * graph.Height / 6.5f), graph.Width, (3 * graph.Height / 2.05f - 2 * graph.Height / 6.5f));

            g3.DrawString("?", new Font("Arial", 40), myBrush, graph.Width / 11, top.Height / 6);
            g3.DrawString("?", new Font("Arial", 40), myBrush, graph.Width / 2 - (graph.Width / 14), top.Height / 6);
            g3.DrawString("?", new Font("Arial", 40), myBrush, graph.Width - (graph.Width / 4), top.Height / 6);
            g4.DrawString("?", new Font("Arial", 40), myBrush, 0, graph.Height / 0.85f);
            g4.DrawString("?", new Font("Arial", 40), myBrush, 0, graph.Height / 4);
            g4.DrawString("?", new Font("Arial", 40), myBrush, 0, graph.Height / 2 + (graph.Height / 10));
            g4.DrawString("?", new Font("Arial", 40), myBrush, 0, graph.Height / 1.1f);
            g4.DrawString("?", new Font("Arial", 40), myBrush, 0, graph.Height / 100);
        }

        /***************************************************************
        private void printAxis3()

        Use: prints the 'axis' or grid lines for each cell for the
        hard puzzles and displays the correct sums and current sums. 
        Current sums are displayed in various colors depending on if 
        a row is a filled and correct = green. If a row is filled and 
        incorrect = red. And if a row is not filled = gainsboro

        Parameters: none 

        Returns: nothing
        ***************************************************************/
        private void printAxis3()
        {
            myBrush.Color = Color.Gray;

            //draw the sum lines up top
            Graphics g1 = top.CreateGraphics();
            g1.FillRectangle(new SolidBrush(Color.Black), 0, 0, top.Width, top.Height);
            g1.DrawLine(myPen, top.Width / 7, 0, top.Width / 7, top.Height);
            g1.DrawLine(myPen, 2 * top.Width / 7, 0, 2 * top.Width / 7, top.Height);
            g1.DrawLine(myPen, 3 * top.Width / 7, 0, 3 * top.Width / 7, top.Height);
            g1.DrawLine(myPen, 4 * top.Width / 7, 0, 4 * top.Width / 7, top.Height);
            g1.DrawLine(myPen, 5 * top.Width / 7, 0, 5 * top.Width / 7, top.Height);
            g1.DrawLine(myPen, 6 * top.Width / 7, 0, 6 * top.Width / 7, top.Height);

            //draw the left side axis lines 
            Graphics g2 = left.CreateGraphics();
            g2.FillRectangle(new SolidBrush(Color.Black), 0, 0, left.Width, left.Height);
            g2.DrawLine(myPen, 0, graph.Height / 6.5f, graph.Width, graph.Height / 6.5f);
            g2.DrawLine(myPen, 0, graph.Height / 6.5f + array[0].Height, graph.Width, graph.Height / 6.5f + array[0].Height);
            g2.DrawLine(myPen, 0, graph.Height / 6.5f + 2*array[0].Height, graph.Width, graph.Height / 6.5f + 2*array[0].Height);
            g2.DrawLine(myPen, 0, graph.Height / 6.5f + 3*array[0].Height, graph.Width, graph.Height / 6.5f + 3*array[0].Height);
            g2.DrawLine(myPen, 0, graph.Height / 6.5f + 4*array[0].Height, graph.Width, graph.Height / 6.5f + 4*array[0].Height);
            g2.DrawLine(myPen, 0, graph.Height / 6.5f + 5 * array[0].Height, graph.Width, graph.Height / 6.5f + 5 * array[0].Height);
            g2.DrawLine(myPen, 0, graph.Height / 6.5f + 6 * array[0].Height, graph.Width, graph.Height / 6.5f + 6 * array[0].Height);
            g2.DrawLine(myPen, 0, (3 * graph.Height / 2.05f - 2 * graph.Height / 6.5f), graph.Width, (3 * graph.Height / 2.05f - 2 * graph.Height / 6.5f));

            //draw the bot axis lines 
            Graphics g3 = bot.CreateGraphics();
            g3.FillRectangle(new SolidBrush(Color.Black), 0, 0, bot.Width, bot.Height);
            g3.DrawLine(myPen, top.Width / 7, 0, top.Width / 7, top.Height);
            g3.DrawLine(myPen, 2 * top.Width / 7, 0, 2 * top.Width / 7, top.Height);
            g3.DrawLine(myPen, 3 * top.Width / 7, 0, 3 * top.Width / 7, top.Height);
            g3.DrawLine(myPen, 4 * top.Width / 7, 0, 4 * top.Width / 7, top.Height);
            g3.DrawLine(myPen, 5 * top.Width / 7, 0, 5 * top.Width / 7, top.Height);
            g3.DrawLine(myPen, 6 * top.Width / 7, 0, 6 * top.Width / 7, top.Height);

            //draw the right side axis lines 
            Graphics g4 = right.CreateGraphics();
            g4.FillRectangle(new SolidBrush(Color.Black), 0, 0, right.Width, right.Height);
            g4.DrawLine(myPen, 0, graph.Height / 6.5f, graph.Width, graph.Height / 6.5f);
            g4.DrawLine(myPen, 0, graph.Height / 6.5f + array[0].Height, graph.Width, graph.Height / 6.5f + array[0].Height);
            g4.DrawLine(myPen, 0, graph.Height / 6.5f + 2 * array[0].Height, graph.Width, graph.Height / 6.5f + 2 * array[0].Height);
            g4.DrawLine(myPen, 0, graph.Height / 6.5f + 3 * array[0].Height, graph.Width, graph.Height / 6.5f + 3 * array[0].Height);
            g4.DrawLine(myPen, 0, graph.Height / 6.5f + 4 * array[0].Height, graph.Width, graph.Height / 6.5f + 4 * array[0].Height);
            g4.DrawLine(myPen, 0, graph.Height / 6.5f + 5 * array[0].Height, graph.Width, graph.Height / 6.5f + 5 * array[0].Height);
            g4.DrawLine(myPen, 0, graph.Height / 6.5f + 6 * array[0].Height, graph.Width, graph.Height / 6.5f + 6 * array[0].Height);
            g4.DrawLine(myPen, 0, (3 * graph.Height / 2.05f - 2 * graph.Height / 6.5f), graph.Width, (3 * graph.Height / 2.05f - 2 * graph.Height / 6.5f));


            //col1 sums
            
            g1.DrawString(col1.ToString(), new Font("Arial", 30), myBrush, 0, top.Height / 6);
            g1.DrawString(col2.ToString(), new Font("Arial", 30), myBrush, graph.Width / 7 + graph.Width / 100, top.Height / 6);
            g1.DrawString(col3.ToString(), new Font("Arial", 30), myBrush, 2 * graph.Width / 7 + graph.Width / 100, top.Height / 6);
            g1.DrawString(col4.ToString(), new Font("Arial", 30), myBrush, 3 * graph.Width / 7 + graph.Width / 100, top.Height / 6);
            g1.DrawString(col5.ToString(), new Font("Arial", 30), myBrush, 4 * graph.Width / 7 + graph.Width / 100, top.Height / 6);
            g1.DrawString(col6.ToString(), new Font("Arial", 30), myBrush, 5 * graph.Width / 7 + graph.Width / 100, top.Height / 6);
            g1.DrawString(col7.ToString(), new Font("Arial", 30), myBrush, 6 * graph.Width / 7 + graph.Width / 100, top.Height / 6);

            //row1 sums and diags
            g2.DrawString(diag1.ToString(), new Font("Arial", 30), myBrush, 0, graph.Height / 30);
            g2.DrawString(row1.ToString(), new Font("Arial", 30), myBrush, 0, graph.Height / 7 + graph.Height / 30);
            g2.DrawString(row2.ToString(), new Font("Arial", 30), myBrush, 0, 2 * graph.Height / 7 + graph.Height / 30);
            g2.DrawString(row3.ToString(), new Font("Arial", 30), myBrush, 0, 3 * graph.Height / 7 + graph.Height / 30);
            g2.DrawString(row4.ToString(), new Font("Arial", 30), myBrush, 0, 4 * graph.Height / 7 + graph.Height / 30);
            g2.DrawString(row5.ToString(), new Font("Arial", 30), myBrush, 0, 5 * graph.Height / 7 + graph.Height / 30);
            g2.DrawString(row6.ToString(), new Font("Arial", 30), myBrush, 0, 6 * graph.Height / 7 + graph.Height / 30);
            g2.DrawString(row7.ToString(), new Font("Arial", 30), myBrush, 0, graph.Height + graph.Height / 30);
            g2.DrawString(diag2.ToString(), new Font("Arial", 30), myBrush, 0, graph.Height / 0.85f);


            int temp = 0;

            array2 = new int[49];
            for (int i = 0; i < 49; i++)
            {
                if (Int32.TryParse(array[i].Text, out temp))
                    array2[i] = temp;
                else
                    array2[i] = 0;

                temp = 0;
            }

            //current sums inputted by the user
            r1 = array2[0] + array2[1] + array2[2] + array2[3] + array2[4] + array2[5] + array2[6];
            r2 = array2[7] + array2[8] + array2[9] + array2[10] + array2[11] + array2[12] + array2[13];
            r3 = array2[14] + array2[15] + array2[16] + array2[17] + array2[18] + array2[19] + array2[20];
            r4 = array2[21] + array2[22] + array2[23] + array2[24] + array2[25] + array2[26] + array2[27];
            r5 = array2[28] + array2[29] + array2[30] + array2[31] + array2[32] + array2[33] + array2[34];
            r6 = array2[35] + array2[36] + array2[37] + array2[38] + array2[39] + array2[40] + array2[41];
            r7 = array2[42] + array2[43] + array2[44] + array2[45] + array2[46] + array2[47] + array2[48];

            c1 = array2[0] + array2[7] + array2[14] + array2[21] + array2[28] + array2[35] + array2[42];
            c2 = array2[1] + array2[8] + array2[15] + array2[22] + array2[29] + array2[36] + array2[43];
            c3 = array2[2] + array2[9] + array2[16] + array2[23] + array2[30] + array2[37] + array2[44];
            c4 = array2[3] + array2[10] + array2[17] + array2[24] + array2[31] + array2[38] + array2[45];
            c5 = array2[4] + array2[11] + array2[18] + array2[25] + array2[32] + array2[39] + array2[46];
            c6 = array2[5] + array2[12] + array2[19] + array2[26] + array2[33] + array2[40] + array2[47];
            c7 = array2[6] + array2[13] + array2[20] + array2[27] + array2[34] + array2[41] + array2[48];

            d1 = array2[0] + array2[8] + array2[16] + array2[24] + array2[32] + array2[40] + array2[48];
            d2 = array2[6] + array2[12] + array2[18] + array2[24] + array2[30] + array2[36] + array2[42];

            myBrush.Color = Color.Gainsboro;

            //col1 sums
            if (c1 == col1 && array[0].Text != "" && array[7].Text != "" && array[14].Text != "" && array[21].Text != "" && array[28].Text != "" && array[35].Text != "" && array[42].Text != "")
                myBrush.Color = Color.Green;
            else if (array[0].Text != "" && array[7].Text != "" && array[14].Text != "" && array[21].Text != "" && array[28].Text != "" && array[35].Text != "" && array[42].Text != "")
                myBrush.Color = Color.Red;
            else
                myBrush.Color = Color.Gainsboro;
            g3.DrawString(c1.ToString(), new Font("Arial", 30), myBrush, 0, bot.Height / 12);

            if (c2 == col2 && array[1].Text != "" && array[8].Text != "" && array[15].Text != "" && array[22].Text != "" && array[29].Text != "" && array[36].Text != "" && array[43].Text != "")
                myBrush.Color = Color.Green;
            else if (array[1].Text != "" && array[8].Text != "" && array[15].Text != "" && array[22].Text != "" && array[29].Text != "" && array[36].Text != "" && array[43].Text != "")
                myBrush.Color = Color.Red;
            else
                myBrush.Color = Color.Gainsboro;
            g3.DrawString(c2.ToString(), new Font("Arial", 30), myBrush, array[0].Width, bot.Height / 12);

            if (c3 == col3 && array[2].Text != "" && array[9].Text != "" && array[16].Text != "" && array[23].Text != "" && array[30].Text != "" && array[37].Text != "" && array[44].Text != "")
                myBrush.Color = Color.Green;
            else if (array[2].Text != "" && array[9].Text != "" && array[16].Text != "" && array[23].Text != "" && array[30].Text != "" && array[37].Text != "" && array[44].Text != "")
                myBrush.Color = Color.Red;
            else
                myBrush.Color = Color.Gainsboro;
            g3.DrawString(c3.ToString(), new Font("Arial", 30), myBrush, 2*array[0].Width, bot.Height / 12);

            if (c4 == col4 && array[3].Text != "" && array[10].Text != "" && array[17].Text != "" && array[24].Text != "" && array[31].Text != "" && array[38].Text != "" && array[45].Text != "")
                myBrush.Color = Color.Green;
            else if (array[3].Text != "" && array[10].Text != "" && array[17].Text != "" && array[24].Text != "" && array[31].Text != "" && array[38].Text != "" && array[45].Text != "")
                myBrush.Color = Color.Red;
            else
                myBrush.Color = Color.Gainsboro;
            g3.DrawString(c4.ToString(), new Font("Arial", 30), myBrush, 3 * array[0].Width, bot.Height / 12);

            if (c5 == col5 && array[4].Text != "" && array[11].Text != "" && array[18].Text != "" && array[25].Text != "" && array[32].Text != "" && array[39].Text != "" && array[46].Text != "")
                myBrush.Color = Color.Green;
            else if (array[4].Text != "" && array[11].Text != "" && array[18].Text != "" && array[25].Text != "" && array[32].Text != "" && array[39].Text != "" && array[46].Text != "")
                myBrush.Color = Color.Red;
            else
                myBrush.Color = Color.Gainsboro;
            g3.DrawString(c5.ToString(), new Font("Arial", 30), myBrush, 4 * array[0].Width, bot.Height / 12);

            if (c6 == col6 && array[5].Text != "" && array[12].Text != "" && array[19].Text != "" && array[26].Text != "" && array[33].Text != "" && array[40].Text != "" && array[47].Text != "")
                myBrush.Color = Color.Green;
            else if (array[5].Text != "" && array[12].Text != "" && array[19].Text != "" && array[26].Text != "" && array[33].Text != "" && array[40].Text != "" && array[47].Text != "")
                myBrush.Color = Color.Red;
            else
                myBrush.Color = Color.Gainsboro;
            g3.DrawString(c6.ToString(), new Font("Arial", 30), myBrush, 5 * array[0].Width, bot.Height / 12);

            if (c7 == col7 && array[6].Text != "" && array[13].Text != "" && array[20].Text != "" && array[27].Text != "" && array[34].Text != "" && array[41].Text != "" && array[48].Text != "")
                myBrush.Color = Color.Green;
            else if (array[6].Text != "" && array[13].Text != "" && array[20].Text != "" && array[27].Text != "" && array[34].Text != "" && array[41].Text != "" && array[48].Text != "")
                myBrush.Color = Color.Red;
            else
                myBrush.Color = Color.Gainsboro;
            g3.DrawString(c7.ToString(), new Font("Arial", 30), myBrush, 6 * array[0].Width, bot.Height / 12);

            //row1 sums and diags
            if (d1 == diag1 && array[0].Text != "" && array[8].Text != "" && array[16].Text != "" && array[24].Text != "" && array[32].Text != "" && array[40].Text != "" && array[48].Text != "")
                myBrush.Color = Color.Green;
            else if (array[0].Text != "" && array[8].Text != "" && array[16].Text != "" && array[24].Text != "" && array[32].Text != "" && array[40].Text != "" && array[48].Text != "")
                myBrush.Color = Color.Red;
            else
                myBrush.Color = Color.Gainsboro;
            g4.DrawString(d1.ToString(), new Font("Arial", 30), myBrush, 0, right.Height / 50);


            if (r1 == row1 && array[0].Text != "" && array[1].Text != "" && array[2].Text != "" && array[3].Text != "" && array[4].Text != "" && array[5].Text != "" && array[6].Text != "")
                myBrush.Color = Color.Green;
            else if (array[0].Text != "" && array[1].Text != "" && array[2].Text != "" && array[3].Text != "" && array[4].Text != "" && array[5].Text != "" && array[6].Text != "")
                myBrush.Color = Color.Red;
            else
                myBrush.Color = Color.Gainsboro;
            g4.DrawString(r1.ToString(), new Font("Arial", 30), myBrush, 0, right.Height/50 + array[0].Height);

            if (r2 == row2 && array[7].Text != "" && array[8].Text != "" && array[9].Text != "" && array[10].Text != "" && array[11].Text != "" && array[12].Text != "" && array[13].Text != "")
                myBrush.Color = Color.Green;
            else if (array[7].Text != "" && array[8].Text != "" && array[9].Text != "" && array[10].Text != "" && array[11].Text != "" && array[12].Text != "" && array[13].Text != "")
                myBrush.Color = Color.Red;
            else
                myBrush.Color = Color.Gainsboro;
            g4.DrawString(r2.ToString(), new Font("Arial", 30), myBrush, 0, right.Height / 50 + 2* array[0].Height);

            if (r3 == row3 && array[14].Text != "" && array[15].Text != "" && array[16].Text != "" && array[17].Text != "" && array[18].Text != "" && array[19].Text != "" && array[20].Text != "")
                myBrush.Color = Color.Green;
            else if (array[14].Text != "" && array[15].Text != "" && array[16].Text != "" && array[17].Text != "" && array[18].Text != "" && array[19].Text != "" && array[20].Text != "")
                myBrush.Color = Color.Red;
            else
                myBrush.Color = Color.Gainsboro;
            g4.DrawString(r3.ToString(), new Font("Arial", 30), myBrush, 0, right.Height / 50 + 3 * array[0].Height);

            if (r4 == row4 && array[21].Text != "" && array[22].Text != "" && array[23].Text != "" && array[24].Text != "" && array[25].Text != "" && array[26].Text != "" && array[27].Text != "")
                myBrush.Color = Color.Green;
            else if (array[21].Text != "" && array[22].Text != "" && array[23].Text != "" && array[24].Text != "" && array[25].Text != "" && array[26].Text != "" && array[27].Text != "")
                myBrush.Color = Color.Red;
            else
                myBrush.Color = Color.Gainsboro;
            g4.DrawString(r4.ToString(), new Font("Arial", 30), myBrush, 0, right.Height / 50 + 4 * array[0].Height);

            if (r5 == row5 && array[28].Text != "" && array[29].Text != "" && array[30].Text != "" && array[31].Text != "" && array[32].Text != "" && array[33].Text != "" && array[34].Text != "")
                myBrush.Color = Color.Green;
            else if (array[28].Text != "" && array[29].Text != "" && array[30].Text != "" && array[31].Text != "" && array[32].Text != "" && array[33].Text != "" && array[34].Text != "")
                myBrush.Color = Color.Red;
            else
                myBrush.Color = Color.Gainsboro;
            g4.DrawString(r5.ToString(), new Font("Arial", 30), myBrush, 0, right.Height / 50 + 5 * array[0].Height);

            if (r6 == row6 && array[35].Text != "" && array[36].Text != "" && array[37].Text != "" && array[38].Text != "" && array[39].Text != "" && array[40].Text != "" && array[41].Text != "")
                myBrush.Color = Color.Green;
            else if (array[35].Text != "" && array[36].Text != "" && array[37].Text != "" && array[38].Text != "" && array[39].Text != "" && array[40].Text != "" && array[41].Text != "")
                myBrush.Color = Color.Red;
            else
                myBrush.Color = Color.Gainsboro;
            g4.DrawString(r6.ToString(), new Font("Arial", 30), myBrush, 0, right.Height / 50 + 6 * array[0].Height);

            if (r7 == row7 && array[42].Text != "" && array[43].Text != "" && array[44].Text != "" && array[45].Text != "" && array[46].Text != "" && array[47].Text != "" && array[48].Text != "")
                myBrush.Color = Color.Green;
            else if (array[42].Text != "" && array[43].Text != "" && array[44].Text != "" && array[45].Text != "" && array[46].Text != "" && array[47].Text != "" && array[48].Text != "")
                myBrush.Color = Color.Red;
            else
                myBrush.Color = Color.Gainsboro;
            g4.DrawString(r7.ToString(), new Font("Arial", 30), myBrush, 0, right.Height / 50 + 7 * array[0].Height);

            if (d2 == diag2 && array[6].Text != "" && array[12].Text != "" && array[18].Text != "" && array[24].Text != "" && array[30].Text != "" && array[36].Text != "" && array[42].Text != "")
                myBrush.Color = Color.Green;
            else if (array[6].Text != "" && array[12].Text != "" && array[18].Text != "" && array[24].Text != "" && array[30].Text != "" && array[36].Text != "" && array[42].Text != "")
                myBrush.Color = Color.Red;
            else
                myBrush.Color = Color.Gainsboro;
            g4.DrawString(d2.ToString(), new Font("Arial", 30), myBrush, 0, right.Height / 30 + 8 * array[0].Height);

            /*if I were smart I might have come up with the following code instead
             *even still I would have to reimplement this for rows and diagonals 

             * 
             * int[] current = new int[16];
             * int[] totals = new int[16];
             * current is populated with current rows, cols, diags
             * current[0] to current[6] == r1 to r7
             * totals[0] to totals[6] == row1 to row7
             * int j = 0;
             * for (int i = 0; i < 7; i++)
             * { 
             *          myBrush.Color = Color.Gainsboro;
             *  if (array[j].Text != "" + array[j+1].Text != "" + array[j+2].Text != "" + array[j+3].Text != "" + array[j+4].Text != "" + array[j+5].Text != "" + array[j+6].Text)
             *      if (current[i] == totals[i]) //r1 == row1
             *          myBrush.Color = Color.Green;
             *      else
             *          myBrush.Color = Color.Red;
             *          
             * g4.DrawString(current[i].ToString(), new Font("Arial", 30), myBrush, 0, right.Height/50 + i * array[0].Height);
             * 
             * j +=7;
             * }
             * 
             */
        }

        /***************************************************************
       private void printAxis2()

       Use: prints the 'axis' or grid lines for each cell for the
       medium puzzles and displays the correct sums and current sums. 
       Current sums are displayed in various colors depending on if 
       a row is a filled and correct = green. If a row is filled and 
       incorrect = red. And if a row is not filled = gainsboro

       Parameters: none 

       Returns: nothing
       ***************************************************************/
        private void printAxis2()
        {

            myBrush.Color = Color.Gray;

            //draw the sum lines up top
            Graphics g1 = top.CreateGraphics();
            g1.FillRectangle(new SolidBrush(Color.Black), 0, 0, top.Width, top.Height);
            g1.DrawLine(myPen, top.Width / 5, 0, top.Width / 5, top.Height);
            g1.DrawLine(myPen, 2 * top.Width / 5, 0, 2 * top.Width / 5, top.Height);
            g1.DrawLine(myPen, 3 * top.Width / 5, 0, 3 * top.Width / 5, top.Height);
            g1.DrawLine(myPen, 4 * top.Width / 5, 0, 4 * top.Width / 5, top.Height);

            //draw the left side axis lines 
            Graphics g2 = left.CreateGraphics();
            g2.FillRectangle(new SolidBrush(Color.Black), 0, 0, left.Width, left.Height);
            g2.DrawLine(myPen, 0, graph.Height / 6.5f, graph.Width, graph.Height / 6.5f);
            g2.DrawLine(myPen, 0, left.Height / 5 + graph.Height/10.8f, graph.Width, left.Height / 5 + graph.Height / 10.8f);
            g2.DrawLine(myPen, 0, 2*left.Height / 5 + graph.Height / 29.8f, graph.Width, 2*left.Height / 5 + graph.Height / 29.8f);
            g2.DrawLine(myPen, 0, 3 * left.Height / 5 - graph.Height / 29.8f, graph.Width, 3 * left.Height / 5 - graph.Height / 29.8f);
            g2.DrawLine(myPen, 0, 4 * left.Height / 5 - graph.Height / 10.8f, graph.Width, 4 * left.Height / 5 - graph.Height / 10.8f);
            g2.DrawLine(myPen, 0, (3 * graph.Height / 2.05f - 2 * graph.Height / 6.5f), graph.Width, (3 * graph.Height / 2.05f - 2 * graph.Height / 6.5f));

            //draw the bot axis lines 
            Graphics g3 = bot.CreateGraphics();
            g3.FillRectangle(new SolidBrush(Color.Black), 0, 0, bot.Width, bot.Height);
            g3.DrawLine(myPen, top.Width / 5, 0, top.Width / 5, top.Height);
            g3.DrawLine(myPen, 2 * top.Width / 5, 0, 2 * top.Width / 5, top.Height);
            g3.DrawLine(myPen, 3 * top.Width / 5, 0, 3 * top.Width / 5, top.Height);
            g3.DrawLine(myPen, 4 * top.Width / 5, 0, 4 * top.Width / 5, top.Height);

            //draw the right side axis lines 
            Graphics g4 = right.CreateGraphics();
            g4.FillRectangle(new SolidBrush(Color.Black), 0, 0, right.Width, right.Height);
            g4.DrawLine(myPen, 0, graph.Height / 6.5f, graph.Width, graph.Height / 6.5f);
            g4.DrawLine(myPen, 0, left.Height / 5 + graph.Height / 10.8f, graph.Width, left.Height / 5 + graph.Height / 10.8f);
            g4.DrawLine(myPen, 0, 2 * left.Height / 5 + graph.Height / 29.8f, graph.Width, 2 * left.Height / 5 + graph.Height / 29.8f);
            g4.DrawLine(myPen, 0, 3 * left.Height / 5 - graph.Height / 29.8f, graph.Width, 3 * left.Height / 5 - graph.Height / 29.8f);
            g4.DrawLine(myPen, 0, 4 * left.Height / 5 - graph.Height / 10.8f, graph.Width, 4 * left.Height / 5 - graph.Height / 10.8f);
            g4.DrawLine(myPen, 0, (3 * graph.Height / 2.05f - 2 * graph.Height / 6.5f), graph.Width, (3 * graph.Height / 2.05f - 2 * graph.Height / 6.5f));

            
            //col1 sums
            g1.DrawString(col1.ToString(), new Font("Arial", 40), myBrush, 0, top.Height / 6);
            g1.DrawString(col2.ToString(), new Font("Arial", 40), myBrush, graph.Width / 5 + graph.Width / 45, top.Height / 6);
            g1.DrawString(col3.ToString(), new Font("Arial", 40), myBrush, 2 * graph.Width / 5 + graph.Width / 45, top.Height / 6);
            g1.DrawString(col4.ToString(), new Font("Arial", 40), myBrush, 3 * graph.Width / 5 + graph.Width / 45, top.Height / 6);
            g1.DrawString(col5.ToString(), new Font("Arial", 40), myBrush, 4 * graph.Width / 5 + graph.Width / 45, top.Height / 6);

            //row1 sums and diags
            g2.DrawString(diag1.ToString(), new Font("Arial", 40), myBrush, 0, graph.Height / 100);
            g2.DrawString(row1.ToString(), new Font("Arial", 40), myBrush, 0, graph.Height / 5 + graph.Height / 100);
            g2.DrawString(row2.ToString(), new Font("Arial", 40), myBrush, 0, 2*graph.Height / 5 + graph.Height / 100);
            g2.DrawString(row3.ToString(), new Font("Arial", 40), myBrush, 0, 3*graph.Height / 5 + graph.Height / 100);
            g2.DrawString(row4.ToString(), new Font("Arial", 40), myBrush, 0, 4*graph.Height / 5 + graph.Height / 100);
            g2.DrawString(row5.ToString(), new Font("Arial", 40), myBrush, 0, graph.Height + graph.Height / 100);
            g2.DrawString(diag2.ToString(), new Font("Arial", 40), myBrush, 0, graph.Height / 0.85f);
            

            int temp = 0;
            
            array2 = new int[25];
            for (int i = 0; i < 25; i++)
            {
                if (Int32.TryParse(array[i].Text, out temp))
                    array2[i] = temp;
                else
                    array2[i] = 0;

                temp = 0;
            }

            //current sums inputted by the user
            r1 = array2[0] + array2[1] + array2[2] + array2[3] + array2[4];
            r2 = array2[5] + array2[6] + array2[7] + array2[8] + array2[9];
            r3 = array2[10] + array2[11] + array2[12] + array2[13] + array2[14];
            r4 = array2[15] + array2[16] + array2[17] + array2[18] + array2[19];
            r5 = array2[20] + array2[21] + array2[22] + array2[23] + array2[24];

            c1 = array2[0] + array2[5] + array2[10] + array2[15] + array2[20];
            c2 = array2[1] + array2[6] + array2[11] + array2[16] + array2[21];
            c3 = array2[2] + array2[7] + array2[12] + array2[17] + array2[22];
            c4 = array2[3] + array2[8] + array2[13] + array2[18] + array2[23];
            c5 = array2[4] + array2[9] + array2[14] + array2[19] + array2[24];

            d1 = array2[0] + array2[6] + array2[12] + array2[18] + array2[24];
            d2 = array2[4] + array2[8] + array2[12] + array2[16] + array2[20];

            myBrush.Color = Color.Gainsboro;

            //col1 sums
            if (c1 == col1 && array[0].Text != "" && array[5].Text != "" && array[10].Text != "" && array[15].Text != "" && array[20].Text != "")
                myBrush.Color = Color.Green;
            else if (array[0].Text != "" && array[5].Text != "" && array[10].Text != "" && array[15].Text != "" && array[20].Text != "")
                myBrush.Color = Color.Red;
            else
                myBrush.Color = Color.Gainsboro;
            g3.DrawString(c1.ToString(), new Font("Arial", 40), myBrush, bot.Width / 50, bot.Height / 12);

            if (c2 == col2 && array[1].Text != "" && array[6].Text != "" && array[11].Text != "" && array[16].Text != "" && array[21].Text != "")
                myBrush.Color = Color.Green;
            else if (array[1].Text != "" && array[6].Text != "" && array[11].Text != "" && array[16].Text != "" && array[21].Text != "")
                myBrush.Color = Color.Red;
            else
                myBrush.Color = Color.Gainsboro;
            g3.DrawString(c2.ToString(), new Font("Arial", 40), myBrush, bot.Width / 50 + array[0].Width, bot.Height / 12);

            if (c3 == col3 && array[2].Text != "" && array[7].Text != "" && array[12].Text != "" && array[17].Text != "" && array[22].Text != "")
                myBrush.Color = Color.Green;
            else if (array[2].Text != "" && array[7].Text != "" && array[12].Text != "" && array[17].Text != "" && array[22].Text != "")
                myBrush.Color = Color.Red;
            else
                myBrush.Color = Color.Gainsboro;
            g3.DrawString(c3.ToString(), new Font("Arial", 40), myBrush, bot.Width / 50 + 2*array[0].Width, bot.Height / 12);

            if (c4 == col4 && array[3].Text != "" && array[8].Text != "" && array[13].Text != "" && array[18].Text != "" && array[23].Text != "")
                myBrush.Color = Color.Green;
            else if (array[3].Text != "" && array[8].Text != "" && array[13].Text != "" && array[18].Text != "" && array[23].Text != "")
                myBrush.Color = Color.Red;
            else
                myBrush.Color = Color.Gainsboro;
            g3.DrawString(c4.ToString(), new Font("Arial", 40), myBrush, bot.Width / 50 + 3*array[0].Width, bot.Height / 12);

            if (c5 == col5 && array[4].Text != "" && array[9].Text != "" && array[14].Text != "" && array[19].Text != "" && array[24].Text != "")
                myBrush.Color = Color.Green;
            else if (array[4].Text != "" && array[9].Text != "" && array[14].Text != "" && array[19].Text != "" && array[24].Text != "")
                myBrush.Color = Color.Red;
            else
                myBrush.Color = Color.Gainsboro;
            g3.DrawString(c5.ToString(), new Font("Arial", 40), myBrush, bot.Width / 50 + 4*array[0].Width, bot.Height / 12);


            //row1 sums and diags
            if (d1 == diag1 && array[0].Text != "" && array[6].Text != "" && array[12].Text != "" && array[18].Text != "" && array[24].Text != "")
                myBrush.Color = Color.Green;
            else if (array[0].Text != "" && array[6].Text != "" && array[12].Text != "" && array[18].Text != "" && array[24].Text != "")
                myBrush.Color = Color.Red;
            else
                myBrush.Color = Color.Gainsboro;
            g4.DrawString(d1.ToString(), new Font("Arial", 40), myBrush, 0, right.Height / 50);
            

            if (r1 == row1 && array[0].Text != "" && array[1].Text != "" && array[2].Text != "" && array[3].Text != "" && array[4].Text != "")
                myBrush.Color = Color.Green;
            else if (array[0].Text != "" && array[1].Text != "" && array[2].Text != "" && array[3].Text != "" && array[4].Text != "")
                myBrush.Color = Color.Red;
            else
                myBrush.Color = Color.Gainsboro;
            g4.DrawString(r1.ToString(), new Font("Arial", 40), myBrush, 0, array[0].Height);

            if (r2 == row2 && array[5].Text != "" && array[6].Text != "" && array[7].Text != "" && array[8].Text != "" && array[9].Text != "")
                myBrush.Color = Color.Green;
            else if (array[5].Text != "" && array[6].Text != "" && array[7].Text != "" && array[8].Text != "" && array[9].Text != "")
                myBrush.Color = Color.Red;
            else
                myBrush.Color = Color.Gainsboro;
            g4.DrawString(r2.ToString(), new Font("Arial", 40), myBrush, 0, 2*array[0].Height);

            if (r3 == row3 && array[10].Text != "" && array[11].Text != "" && array[12].Text != "" && array[13].Text != "" && array[14].Text != "")
                myBrush.Color = Color.Green;
            else if (array[10].Text != "" && array[11].Text != "" && array[12].Text != "" && array[13].Text != "" && array[14].Text != "")
                myBrush.Color = Color.Red;
            else
                myBrush.Color = Color.Gainsboro;
            g4.DrawString(r3.ToString(), new Font("Arial", 40), myBrush, 0, 3 * array[0].Height);

            if (r4 == row4 && array[15].Text != "" && array[16].Text != "" && array[17].Text != "" && array[18].Text != "" && array[19].Text != "")
                myBrush.Color = Color.Green;
            else if (array[15].Text != "" && array[16].Text != "" && array[17].Text != "" && array[18].Text != "" && array[19].Text != "")
                myBrush.Color = Color.Red;
            else
                myBrush.Color = Color.Gainsboro;
            g4.DrawString(r4.ToString(), new Font("Arial", 40), myBrush, 0, 4 * array[0].Height);

            if (r5 == row5 && array[20].Text != "" && array[21].Text != "" && array[22].Text != "" && array[23].Text != "" && array[24].Text != "")
                myBrush.Color = Color.Green;
            else if (array[20].Text != "" && array[21].Text != "" && array[22].Text != "" && array[23].Text != "" && array[24].Text != "")
                myBrush.Color = Color.Red;
            else
                myBrush.Color = Color.Gainsboro;
            g4.DrawString(r5.ToString(), new Font("Arial", 40), myBrush, 0, 5 * array[0].Height);
            
            if (d2 == diag2 && array[4].Text != "" && array[8].Text != "" && array[12].Text != "" && array[16].Text != "" && array[20].Text != "")
                myBrush.Color = Color.Green;
            else if (array[4].Text != "" && array[8].Text != "" && array[12].Text != "" && array[16].Text != "" && array[20].Text != "")
                myBrush.Color = Color.Red;
            else
                myBrush.Color = Color.Gainsboro;
            g4.DrawString(d2.ToString(), new Font("Arial", 40), myBrush, 0, right.Height - right.Height/10);
            
        }

        /***************************************************************
       private void printAxis()

       Use: prints the 'axis' or grid lines for each cell for the
       easy puzzles and displays the correct sums and current sums. 
       Current sums are displayed in various colors depending on if 
       a row is a filled and correct = green. If a row is filled and 
       incorrect = red. And if a row is not filled = gainsboro

       Parameters: none 

       Returns: nothing
       ***************************************************************/
        private void printAxis()
        {

            myBrush.Color = Color.Gray;
            //draw the sum lines up top
            Graphics g1 = top.CreateGraphics();
            g1.FillRectangle(new SolidBrush(Color.Black), 0, 0, top.Width, top.Height);
            g1.DrawLine(myPen, graph.Width / 3, 0, graph.Width / 3, graph.Height);
            g1.DrawLine(myPen, 2 * graph.Width / 3, 0, 2 * graph.Width / 3, graph.Height);

            //draw the left side axis lines 
            Graphics g2 = left.CreateGraphics();
            g2.FillRectangle(new SolidBrush(Color.Black), 0, 0, left.Width, left.Height);
            g2.DrawLine(myPen, 0, graph.Height / 6.5f, graph.Width, graph.Height / 6.5f);
            g2.DrawLine(myPen, 0, graph.Height / 2.05f, graph.Width, graph.Height / 2.05f);
            g2.DrawLine(myPen, 0, (2 * graph.Height / 2.05f - graph.Height / 6.5f), graph.Width, (2 * graph.Height / 2.05f - graph.Height / 6.5f));
            g2.DrawLine(myPen, 0, (3 * graph.Height / 2.05f - 2 * graph.Height / 6.5f), graph.Width, (3 * graph.Height / 2.05f - 2 * graph.Height / 6.5f));

            //col1 sums
            g1.DrawString(col1.ToString(), new Font("Arial", 40), myBrush, graph.Width / 11, top.Height / 6);
            g1.DrawString(col2.ToString(), new Font("Arial", 40), myBrush, graph.Width / 2 - (graph.Width / 14), top.Height / 6);
            g1.DrawString(col3.ToString(), new Font("Arial", 40), myBrush, graph.Width - (graph.Width / 4), top.Height / 6);

            //row1 sums and diags
            g2.DrawString(diag1.ToString(), new Font("Arial", 40), myBrush, 0, graph.Height / 100);
            g2.DrawString(row1.ToString(), new Font("Arial", 40), myBrush, 0, graph.Height / 4);
            g2.DrawString(row2.ToString(), new Font("Arial", 40), myBrush, 0, graph.Height / 2 + (graph.Height / 10));
            g2.DrawString(row3.ToString(), new Font("Arial", 40), myBrush, 0, graph.Height / 1.1f);
            g2.DrawString(diag2.ToString(), new Font("Arial", 40), myBrush, 0, graph.Height / 0.85f);

            //draw the bot axis lines 
            Graphics g3 = bot.CreateGraphics();
            g3.FillRectangle(new SolidBrush(Color.Black), 0, 0, bot.Width, bot.Height);
            g3.DrawLine(myPen, graph.Width / 3, 0, graph.Width / 3, graph.Height);
            g3.DrawLine(myPen, 2 * graph.Width / 3, 0, 2 * graph.Width / 3, graph.Height);

            //draw the right side axis lines 
            Graphics g4 = right.CreateGraphics();
            g4.FillRectangle(new SolidBrush(Color.Black), 0, 0, right.Width, right.Height);
            g4.DrawLine(myPen, 0, graph.Height / 6.5f, graph.Width, graph.Height / 6.5f);
            g4.DrawLine(myPen, 0, graph.Height / 2.05f, graph.Width, graph.Height / 2.05f);
            g4.DrawLine(myPen, 0, (2 * graph.Height / 2.05f - graph.Height / 6.5f), graph.Width, (2 * graph.Height / 2.05f - graph.Height / 6.5f));
            g4.DrawLine(myPen, 0, (3 * graph.Height / 2.05f - 2 * graph.Height / 6.5f), graph.Width, (3 * graph.Height / 2.05f - 2 * graph.Height / 6.5f));

            int temp = 0;

            array2 = new int[9];
            for (int i = 0; i < 9; i++)
            {
                if (Int32.TryParse(array[i].Text, out temp))
                    array2[i] = temp;
                else
                    array2[i] = 0;

                temp = 0;
            }

            //current sum inputed by user
            r1 = array2[0] + array2[1] + array2[2];
            r2 = array2[3] + array2[4] + array2[5];
            r3 = array2[6] + array2[7] + array2[8];

            c1 = array2[0] + array2[3] + array2[6];
            c2 = array2[1] + array2[4] + array2[7];
            c3 = array2[2] + array2[5] + array2[8];

            d1 = array2[0] + array2[4] + array2[8];
            d2 = array2[2] + array2[4] + array2[6];

            myBrush.Color = Color.Gainsboro;

            //col1 sums
            if (c1 == col1 && array[0].Text != "" && array[3].Text != "" && array[6].Text != "")
                myBrush.Color = Color.Green;
            else if (array[0].Text != "" && array[3].Text != "" && array[6].Text != "")
                myBrush.Color = Color.Red;
            else
                myBrush.Color = Color.Gainsboro;

            g3.DrawString(c1.ToString(), new Font("Arial", 40), myBrush, graph.Width / 11, top.Height / 6);


            if (c2 == col2 && array[1].Text != "" && array[4].Text != "" && array[7].Text != "")
                myBrush.Color = Color.Green;
            else if (array[1].Text != "" && array[4].Text != "" && array[7].Text != "")
                myBrush.Color = Color.Red;
            else
                myBrush.Color = Color.Gainsboro;
            g3.DrawString(c2.ToString(), new Font("Arial", 40), myBrush, graph.Width / 2 - (graph.Width / 14), top.Height / 6);


            if (c3 == col3 && array[2].Text != "" && array[5].Text != "" && array[8].Text != "")
                myBrush.Color = Color.Green;
            else if (array[2].Text != "" && array[5].Text != "" && array[8].Text != "")
                myBrush.Color = Color.Red;
            else
                myBrush.Color = Color.Gainsboro;
            g3.DrawString(c3.ToString(), new Font("Arial", 40), myBrush, graph.Width - (graph.Width / 4), top.Height / 6);

            //row1 sums and diags
            if (d1 == diag1 && array[0].Text != "" && array[4].Text != "" && array[8].Text != "")
                myBrush.Color = Color.Green;
            else if (array[0].Text != "" && array[4].Text != "" && array[8].Text != "")
                myBrush.Color = Color.Red;
            else
                myBrush.Color = Color.Gainsboro;

            g4.DrawString(d1.ToString(), new Font("Arial", 40), myBrush, 0, graph.Height / 0.85f);


            if (r1 == row1 && array[0].Text != "" && array[1].Text != "" && array[2].Text != "")
                myBrush.Color = Color.Green;
            else if (array[0].Text != "" && array[1].Text != "" && array[2].Text != "")
                myBrush.Color = Color.Red;
            else
                myBrush.Color = Color.Gainsboro;

            g4.DrawString(r1.ToString(), new Font("Arial", 40), myBrush, 0, graph.Height / 4);

            if (r2 == row2 && array[3].Text != "" && array[4].Text != "" && array[5].Text != "")
                myBrush.Color = Color.Green;
            else if (array[3].Text != "" && array[4].Text != "" && array[5].Text != "")
                myBrush.Color = Color.Red;
            else
                myBrush.Color = Color.Gainsboro;
            g4.DrawString(r2.ToString(), new Font("Arial", 40), myBrush, 0, graph.Height / 2 + (graph.Height / 10));


            if (r3 == row3 && array[6].Text != "" && array[7].Text != "" && array[8].Text != "")
                myBrush.Color = Color.Green;
            else if (array[6].Text != "" && array[7].Text != "" && array[8].Text != "")
                myBrush.Color = Color.Red;
            else
                myBrush.Color = Color.Gainsboro;
            g4.DrawString(r3.ToString(), new Font("Arial", 40), myBrush, 0, graph.Height / 1.1f);

            if (d2 == diag2 && array[2].Text != "" && array[4].Text != "" && array[6].Text != "")
                myBrush.Color = Color.Green;
            else if (array[2].Text != "" && array[4].Text != "" && array[6].Text != "")
                myBrush.Color = Color.Red;
            else
                myBrush.Color = Color.Gainsboro;
      
            g4.DrawString(d2.ToString(), new Font("Arial", 40), myBrush, 0, graph.Height / 100);
        }

        //the class below is used to override Focus method for the textboxes
        //to be able to hide the caret
        class TextBoxWithoutCaret : TextBox
        {
            [DllImport("user32")]
            static extern bool HideCaret(IntPtr hwnd);

            [DllImport("user32")]
            static extern bool ShowCaret(IntPtr hwnd);


            /***************************************************************
            protected override void OnGotFocus(EventArgs e)

            Use: hides the caret and highlights the textbox 

            Parameters: EventArgs e - the event triggered

            Returns: nothing
            ***************************************************************/
            protected override void OnGotFocus(EventArgs e)
            {
                base.OnGotFocus(e);
                base.BackColor = Color.FromArgb(1, 4, 7);
                HideCaret(Handle);
            }

            /***************************************************************
            protected override void OnLostFocus(EventArgs e)

            Use: hides the caret and un-highlights the previously selected textbox.
            then highlight() is called to determine if a row,column, or box needs to
            be highlighted again. 

            Parameters: EventArgs e - the event triggered

            Returns: nothing
            ***************************************************************/
            protected override void OnLostFocus(EventArgs e)
            {
                base.OnLostFocus(e);
                base.BackColor = Color.Black;
                if (EASY)
                    highlight();
                else if (MEDIUM)
                    highlight2();
                else if (HARD)
                    highlight3();

                ShowCaret(Handle);
            }

           

        }



    }

    
}
