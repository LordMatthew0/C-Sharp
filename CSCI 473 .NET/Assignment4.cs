/* Corbin Lutsch & Matthew Lord
 *     Z1837389  & Z1848456
 *  CSCI - 473
 *  Due: 03/28/19
 *  Assignment 4 -  r = 2a(1 – sin φ)
 * 
 */


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mandc_Assign4
{
    public partial class Form1 : Form
    {
        private static Pen myPen;
        private static float xMin = -100;
        private static float xMax = 100;
        private static float xInterval = 10;
        private static float yMin = -100;
        private static float yMax = 100;
        private static float yInterval = 10;

        public Form1()
        {
            InitializeComponent();

            myPen = new Pen(Color.White);

        }

        /***************************************************************
        private void btnAxis_Click(object sender, EventArgs e)

        Use: updates the axis and prints any equations inputed 
     
        Parameters: 1. object sender - the calling object/button
                    2. EventArgs e - the calling event
			
        Returns: nothing
        ***************************************************************/
        private void btnAxis_Click(object sender, EventArgs e)
        {
            Button update = sender as Button;

            if (update != null)
            {
                //update the axis
                updateAxis();
                //graph any equations entered
                btnGraph_Click(sender, e);
            }
        
        }

        /***************************************************************
        private void updateAxis()

        Use: updates the axis and then calls graph to graph any inputed
        equations

        Parameters: 1. object sender - the calling object/button
                    2. EventArgs e - the calling event

        Returns: nothing
        ***************************************************************/
        private void updateAxis()
        {

            //If valid numbers were entered in all input fields 
            if (float.TryParse(xMinTxt.Text, out xMin) && float.TryParse(xMaxTxt.Text, out xMax) &&
                float.TryParse(yMinTxt.Text, out yMin) && float.TryParse(yMaxTxt.Text, out yMax) &&
                float.TryParse(xIntervalTxt.Text, out xInterval) && float.TryParse(yIntervalTxt.Text, out yInterval))
            {
                //validate the min and max ranges
                if (xMin >= xMax)
                {
                    xMax = xMin + 1;
                    xMaxTxt.Text = xMax.ToString();
                }

                if (yMin >= yMax)
                {
                    yMax = yMin + 1;
                    yMaxTxt.Text = yMax.ToString();
                }


                //Clear the current axis screen
                Graphics g = graph.CreateGraphics();
                SolidBrush blackBrush = new SolidBrush(Color.Black);
                g.FillRectangle(blackBrush, 0, 0, graph.Width, graph.Height);
                myPen.Color = Color.White;

                //print the updated axis screen
                float x1 = 0.0F;
                float y1 = 0.0F;

                //get the remainder of the interval 
                float remainder;
                float interval;

                float xMinAbs = Math.Abs(xMin);
                float yMinAbs = Math.Abs(yMin);

                if (xMin <= 0 && xMax > 0)
                {
                    x1 = graph.Width * (xMinAbs / (xMax + xMinAbs));
                    g.DrawLine(myPen, x1, 0, x1, graph.Height);
                }

                if (yMin <= 0 && yMax > 0)
                {
                    y1 = graph.Height - (graph.Height * (yMinAbs / (yMax + yMinAbs)));
                    //draw axis
                    g.DrawLine(myPen, 0, y1, graph.Width, y1);
                }


                if (xMin <= 0 && xMax > 0) //y-axis is present
                {
                    //reset y1
                    if (yMin <= 0 && yMax > 0)
                        y1 = graph.Height - (graph.Height * (Math.Abs(yMin) / (yMax + Math.Abs(yMin))));
                    else if (yMin < 0 && yMax <= 0)
                        y1 = 0;
                    else //xMin > 0 && xMax > 0
                        y1 = graph.Height;

                    //get the remainder of the interval 
                    remainder = Math.Abs(xMin) % Math.Abs(xInterval);
                    interval = Math.Abs(xInterval) + remainder;

                    //print the first remainder tick
                    x1 = graph.Width * (remainder / (Math.Abs(xMin) + Math.Abs(xMax)));
                    g.DrawLine(myPen, x1, y1, x1, y1 + 4);
                    g.DrawLine(myPen, x1, y1, x1, y1 - 4);

                    //print the first remainder tick label
                    float tempMinX = xMin + remainder;

                    if (yMin <= -5) //we will flip the ticks and numbers once the axis gets close to the origin 
                    {
                        if (tempMinX != 0)
                            g.DrawString(tempMinX.ToString(), new Font("Arial", 6), new SolidBrush(Color.White), x1 - 8, y1 + 4);
                    }
                    else
                    {
                        if (tempMinX != 0)
                            g.DrawString(tempMinX.ToString(), new Font("Arial", 6), new SolidBrush(Color.White), x1 - 6, y1 - 15);
                    }

                    for (int i = 0; i < ((Math.Abs(xMin) + Math.Abs(xMax)) / Math.Abs(xInterval)); i++)
                    {
                        tempMinX += Math.Abs(xInterval);
                        x1 = graph.Width * (interval / (Math.Abs(xMin) + Math.Abs(xMax)));

                        if (yMin <= -5)
                        {
                            if (tempMinX != 0)
                                g.DrawString(tempMinX.ToString(), new Font("Arial", 6), new SolidBrush(Color.White), x1 - 6, y1 + 4); //number

                            g.DrawLine(myPen, x1, y1, x1, y1 + 4); //ticks
                            g.DrawLine(myPen, x1, y1, x1, y1 - 4);
                        }
                        else
                        {
                            g.DrawLine(myPen, x1, y1, x1, y1 + 4); //ticks
                            g.DrawLine(myPen, x1, y1, x1, y1 - 4);

                            if (tempMinX != 0)
                                g.DrawString(tempMinX.ToString(), new Font("Arial", 6), new SolidBrush(Color.White), x1 - 6, y1 - 15); //number
                        }

                        interval += Math.Abs(xInterval);
                    }
                }
                else if (xMin >= 0 && xMax > 0) //y-axis is missing, labels are on left side
                {
                    //reset y1
                    if (yMin <= 0 && yMax > 0)
                        y1 = graph.Height - (graph.Height * (Math.Abs(yMin) / (yMax + Math.Abs(yMin))));
                    else if (yMin < 0 && yMax <= 0)
                        y1 = 0;
                    else //xMin > 0 && xMax > 0
                        y1 = graph.Height;

                    //get the remainder of the interval 
                    remainder = Math.Abs(xMin) % Math.Abs(xInterval);
                    interval = Math.Abs(xInterval) - remainder;

                    //print the first remainder tick
                    x1 = graph.Width * (interval / (Math.Abs(xMax) - Math.Abs(xMin)));
                    g.DrawLine(myPen, x1, y1, x1, y1 + 4);
                    g.DrawLine(myPen, x1, y1, x1, y1 - 4);

                    //print the first remainder tick label
                    float tempMinX = xMin + interval;
                    
                    if (yMin <= -5)
                    {
                        if (tempMinX != 0)
                            g.DrawString(tempMinX.ToString(), new Font("Arial", 6), new SolidBrush(Color.White), x1 - 6, y1 + 4);
                    }
                    else
                    {
                        if (tempMinX != 0)
                            g.DrawString(tempMinX.ToString(), new Font("Arial", 6), new SolidBrush(Color.White), x1 - 6, y1 - 15);
                    }

                    for (int i = 0; i < ((Math.Abs(xMax) - Math.Abs(xMin)) / Math.Abs(xInterval)); i++)
                    {
                        tempMinX += Math.Abs(xInterval);
                        interval += Math.Abs(xInterval);
                        x1 = graph.Width * (interval / (Math.Abs(xMax) - Math.Abs(xMin)));
                  
                        if (yMin <= -5)
                        {
                            if (tempMinX != 0)
                                g.DrawString(tempMinX.ToString(), new Font("Arial", 6), new SolidBrush(Color.White), x1 - 6, y1 + 4); //number

                            g.DrawLine(myPen, x1, y1, x1, y1 + 4); //ticks
                            g.DrawLine(myPen, x1, y1, x1, y1 - 4);
                        }
                        else
                        {

                            g.DrawLine(myPen, x1, y1, x1, y1 + 4); //ticks
                            g.DrawLine(myPen, x1, y1, x1, y1 - 4);


                            if (tempMinX != 0)
                                g.DrawString(tempMinX.ToString(), new Font("Arial", 6), new SolidBrush(Color.White), x1 - 6, y1 - 15); //number
                        }

                    }
                }
                else //xMin < 0 && xMax < 0
                {
                    //reset y1
                    if (yMin <= 0 && yMax > 0)
                        y1 = graph.Height - (graph.Height * (Math.Abs(yMin) / (yMax + Math.Abs(yMin))));
                    else if (yMin < 0 && yMax <= 0)
                        y1 = 0;
                    else //xMin > 0 && xMax > 0
                        y1 = graph.Height;

                    //get the remainder of the interval 
                    remainder = Math.Abs(xMax) % Math.Abs(xInterval);
                    interval = Math.Abs(xInterval) - remainder;

                    //print the first remainder tick
                    x1 = graph.Width - (graph.Width * (interval / (Math.Abs(xMin) - Math.Abs(xMax))));
                    g.DrawLine(myPen, x1, y1, x1, y1 + 4);
                    g.DrawLine(myPen, x1, y1, x1, y1 - 4);

                    //print the first remainder tick label
                    float tempMinX = xMax - interval;
                    if (yMin <= -5)
                    {
                        if (tempMinX != 0)
                            g.DrawString(tempMinX.ToString(), new Font("Arial", 6), new SolidBrush(Color.White), x1 - 6, y1 + 4);
                    }
                    else
                    {
                        if (tempMinX != 0)
                            g.DrawString(tempMinX.ToString(), new Font("Arial", 6), new SolidBrush(Color.White), x1 - 6, y1 - 15); //number
                    }

                    for (int i = 0; i < ((Math.Abs(xMin) - Math.Abs(xMax)) / Math.Abs(xInterval)); i++)
                    {
                        tempMinX -= Math.Abs(xInterval);
                        interval += Math.Abs(xInterval);
                        x1 = graph.Width - (graph.Width * (interval / (Math.Abs(xMin) - Math.Abs(xMax))));
                
                        if (yMin <= -5)
                        {
                            if (tempMinX != 0)
                                g.DrawString(tempMinX.ToString(), new Font("Arial", 6), new SolidBrush(Color.White), x1 - 6, y1 + 4); //number

                            g.DrawLine(myPen, x1, y1, x1, y1 + 4); //ticks
                            g.DrawLine(myPen, x1, y1, x1, y1 - 4);
                        }
                        else
                        {

                            g.DrawLine(myPen, x1, y1, x1, y1 + 4); //ticks
                            g.DrawLine(myPen, x1, y1, x1, y1 - 4);


                            if (tempMinX != 0)
                                g.DrawString(tempMinX.ToString(), new Font("Arial", 6), new SolidBrush(Color.White), x1 - 6, y1 - 15); //number
                        }


                    }
                }




                if (yMin <= 0 && yMax > 0) //now do the same thing but for the other axis
                {
                    //reset x1
                    if (xMin <= 0 && xMax > 0)
                        x1 = graph.Width * (Math.Abs(xMin) / (xMax + Math.Abs(xMin)));
                    else if (xMin < 0 && xMax <= 0)
                        x1 = graph.Width;
                    else //xMin > 0 && xMax > 0
                        x1 = 0;

                    //now do the same for the y-axis
                    remainder = Math.Abs(yMin) % Math.Abs(yInterval);
                    interval = Math.Abs(yInterval) + remainder;

                    //print the first remainder tick
                    y1 = graph.Height - (graph.Height * (remainder / (Math.Abs(yMin) + Math.Abs(yMax))));

                    g.DrawLine(myPen, x1 + 4, y1, x1, y1);
                    g.DrawLine(myPen, x1 - 4, y1, x1, y1);

                    //print the first remainder tick label
                    float tempMinY = yMin + remainder;
                    if (xMin <= -4) //swap the numbers to the right side of the ticks so you can still see as you move closer to origin
                    {
                        if (tempMinY != 0)
                            g.DrawString(tempMinY.ToString(), new Font("Arial", 6), new SolidBrush(Color.White), x1 - 20, y1 - 4);
                    }
                    else
                    {
                        if (tempMinY != 0)
                            g.DrawString(tempMinY.ToString(), new Font("Arial", 6), new SolidBrush(Color.White), x1 + 5, y1 - 4);
                    }
                   

                    for (int i = 0; i < ((Math.Abs(yMin) + Math.Abs(yMax)) / Math.Abs(yInterval)); i++)
                    {
                        tempMinY += Math.Abs(yInterval);
                        y1 = graph.Height - (graph.Height * (interval / (Math.Abs(yMin) + Math.Abs(yMax))));


                        if (xMin <= -4) //swap the numbers to the right side of the ticks so you can still see as you move closer to origin
                        {

                            g.DrawLine(myPen, x1 + 4, y1, x1, y1);
                            g.DrawLine(myPen, x1 - 4, y1, x1, y1);
                            if (tempMinY != 0)
                                g.DrawString(tempMinY.ToString(), new Font("Arial", 6), new SolidBrush(Color.White), x1 - 20, y1 - 4);
                        }
                        else
                        {

                            g.DrawLine(myPen, x1 + 4, y1, x1, y1);
                            g.DrawLine(myPen, x1 - 4, y1, x1, y1);


                            if (tempMinY != 0)
                                g.DrawString(tempMinY.ToString(), new Font("Arial", 6), new SolidBrush(Color.White), x1 + 5, y1 - 4);
                        }

                        interval += Math.Abs(yInterval);
                    }
                }
                else if (yMin >= 0 && yMax > 0)
                {
                    //reset x1
                    if (xMin <= 0 && xMax > 0)
                        x1 = graph.Width * (Math.Abs(xMin) / (xMax + Math.Abs(xMin)));
                    else if (xMin < 0 && xMax <= 0)
                        x1 = graph.Width;
                    else //xMin > 0 && xMax > 0
                        x1 = 0;

                    //now do the same for the y-axis
                    remainder = Math.Abs(yMin) % Math.Abs(yInterval);
                    interval = Math.Abs(yInterval) - remainder;

                    //print the first remainder tick
                    y1 = graph.Height - (graph.Height * (interval / (Math.Abs(yMax) - Math.Abs(yMin))));

                    g.DrawLine(myPen, x1 + 4, y1, x1, y1);
                    g.DrawLine(myPen, x1 - 4, y1, x1, y1);

                    //print the first remainder tick label
                    float tempMinY = yMin + interval;
                    if (xMin <= -4) //swap the numbers to the right side of the ticks so you can still see as you move closer to origin
                    {
                        if (tempMinY != 0)
                            g.DrawString(tempMinY.ToString(), new Font("Arial", 6), new SolidBrush(Color.White), x1 - 20, y1 - 4);
                    }
                    else
                    {
                        if (tempMinY != 0)
                            g.DrawString(tempMinY.ToString(), new Font("Arial", 6), new SolidBrush(Color.White), x1 + 5, y1 - 4);
                    }
                    //g.DrawString(tempMinY.ToString(), new Font("Arial", 6), new SolidBrush(Color.White), x1 - 20, y1 - 4);

                    for (int i = 0; i < ((Math.Abs(yMax) - Math.Abs(yMin)) / Math.Abs(yInterval)); i++)
                    {
                        tempMinY += Math.Abs(yInterval);
                        interval += Math.Abs(yInterval);
                        y1 = graph.Height - (graph.Height * (interval / (Math.Abs(yMax) - Math.Abs(yMin))));
                  
                        if (xMin <= -4) //swap the numbers to the right side of the ticks so you can still see as you move closer to origin
                        {

                            g.DrawLine(myPen, x1 + 4, y1, x1, y1);
                            g.DrawLine(myPen, x1 - 4, y1, x1, y1);
                            if (tempMinY != 0)
                                g.DrawString(tempMinY.ToString(), new Font("Arial", 6), new SolidBrush(Color.White), x1 - 20, y1 - 4);
                        }
                        else
                        {

                            g.DrawLine(myPen, x1 + 4, y1, x1, y1);
                            g.DrawLine(myPen, x1 - 4, y1, x1, y1);


                            if (tempMinY != 0)
                                g.DrawString(tempMinY.ToString(), new Font("Arial", 6), new SolidBrush(Color.White), x1 + 5, y1 - 4);
                        }

                    }
                }
                else //yMin < 0 && yMax < 0
                {
                    //reset x1
                    if (xMin <= 0 && xMax > 0)
                        x1 = graph.Width * (Math.Abs(xMin) / (xMax + Math.Abs(xMin)));
                    else if (xMin < 0 && xMax <= 0)
                        x1 = graph.Width;
                    else //xMin > 0 && xMax > 0
                        x1 = 0;

                    //now do the same for the y-axis
                    remainder = Math.Abs(yMax) % Math.Abs(yInterval);
                    interval = Math.Abs(yInterval) - remainder;

                    //print the first remainder tick
                    y1 = (graph.Height * (interval / (Math.Abs(yMin) - Math.Abs(yMax))));

                    g.DrawLine(myPen, x1 + 4, y1, x1, y1);
                    g.DrawLine(myPen, x1 - 4, y1, x1, y1);

                    //print the first remainder tick label
                    float tempMinY = yMax - interval;
                    if (xMin <= -4) //swap the numbers to the right side of the ticks so you can still see as you move closer to origin
                    {
                        if (tempMinY != 0)
                            g.DrawString(tempMinY.ToString(), new Font("Arial", 6), new SolidBrush(Color.White), x1 - 20, y1 - 4);
                    }
                    else
                    {
                        if (tempMinY != 0)
                            g.DrawString(tempMinY.ToString(), new Font("Arial", 6), new SolidBrush(Color.White), x1 + 5, y1 - 4);
                    }
                 

                    for (int i = 0; i < ((Math.Abs(yMin) - Math.Abs(yMax)) / Math.Abs(yInterval)); i++)
                    {
                        tempMinY -= Math.Abs(yInterval);
                        interval += Math.Abs(yInterval);
                        y1 = (graph.Height * (interval / (Math.Abs(yMin) - Math.Abs(yMax))));
                  
                        //flip the axis 
                        if (xMin <= -4) //swap the numbers to the right side of the ticks so you can still see as you move closer to origin
                        {

                            g.DrawLine(myPen, x1 + 4, y1, x1, y1);
                            g.DrawLine(myPen, x1 - 4, y1, x1, y1);
                            if (tempMinY != 0)
                                g.DrawString(tempMinY.ToString(), new Font("Arial", 6), new SolidBrush(Color.White), x1 - 20, y1 - 4);
                        }
                        else
                        {

                            g.DrawLine(myPen, x1 + 4, y1, x1, y1);
                            g.DrawLine(myPen, x1 - 4, y1, x1, y1);


                            if (tempMinY != 0)
                                g.DrawString(tempMinY.ToString(), new Font("Arial", 6), new SolidBrush(Color.White), x1 + 5, y1 - 4);
                        }

                    }
                }

               
            }
        }

        /***************************************************************
       private void btnReset_Click(object sender, EventArgs e)

      Use: resets the default values for the axis dimensions and input values

      Parameters: 1. object sender - the calling object/button
                  2. EventArgs e - the calling event

      Returns: nothing
      ***************************************************************/
        private void btnReset_Click(object sender, EventArgs e)
        {
            Button reset = sender as Button;

            if (reset != null)
            {
                int i = -100;
                xMinTxt.Text = i.ToString();
                xMaxTxt.Text = 100.ToString();
                yMinTxt.Text = i.ToString();
                yMaxTxt.Text = 100.ToString();
                yIntervalTxt.Text = 10.ToString();
                xIntervalTxt.Text = 10.ToString();
                mTxt.Text = "m";
                bTxt.Text = "b";
                a1Txt.Text = "a";
                b1Txt.Text = "b";
                c1Txt.Text = "c";
                a2Txt.Text = "a";
                b2Txt.Text = "b";
                c2Txt.Text = "c";
                d2Txt.Text = "d";
                rTxt.Text = "r";
                hTxt.Text = "h";
                kTxt.Text = "k";

            }
        }

        /***************************************************************
      private void btnGraph_Click(object sender, EventArgs e)

      Use: graphs the 4 different types of equations 

      Parameters: 1. object sender - the calling object/button
                  2. EventArgs e - the calling event

      Returns: nothing
      ***************************************************************/
        private void btnGraph_Click(object sender, EventArgs e)
        {
            Button btn_graph = sender as Button;

            if (btn_graph != null)
            {

                updateAxis(); //update any changes to the axis
                Graphics g = graph.CreateGraphics();
                float m = 0.0f;
                float b = 0.0f;
                //Y = mx + b
                if (float.TryParse(mTxt.Text, out m) && float.TryParse(bTxt.Text, out b))
                {
                    float y1;
                    float y2;
                    float x1 = yMin;
                    float x2 = yMax;
                    x1 -= b;
                    x1 /= m;
                    x2 -= b;
                    x2 /= m;
                    if (x1 <= xMax && x1 >= xMin)
                    {
                        if (xMin < 0 && xMax >= 0)
                        {
                            if (x1 <= 0)
                                x1 = ((Math.Abs(xMin) - Math.Abs(x1)) / (Math.Abs(xMax) + Math.Abs(xMin))) * graph.Width;
                            else
                                x1 = ((Math.Abs(xMin) + x1) / (Math.Abs(xMax) + Math.Abs(xMin))) * graph.Width;
                        }
                        else if (xMin >= 0 && xMax >= 0)
                        {
                            x1 = ((x1 - xMin) / (xMax + xMin)) * graph.Width;
                        }
                        else
                        {
                            x1 = ((Math.Abs(xMin) - Math.Abs(x1)) / (Math.Abs(xMin) - Math.Abs(xMax))) * graph.Width;
                        }

                        y1 = graph.Height;
                    }
                    else
                    {
                        y1 = xMin * m + b;

                        if (yMin < 0 && yMax >= 0)
                        {
                            if (y1 <= 0)
                                y1 = graph.Height - (((Math.Abs(yMin) - Math.Abs(y1)) / (Math.Abs(yMin) + Math.Abs(yMax))) * graph.Height);
                            else
                                y1 = graph.Height - (((Math.Abs(yMin) + Math.Abs(y1)) / (Math.Abs(yMin) + Math.Abs(yMax))) * graph.Height);
                        }
                        else if (yMin >= 0 && yMax >= 0)
                        {
                            y1 = graph.Height - ((y1 - yMin) / (yMax - yMin) * graph.Height);
                        }
                        else //yMin <= 0 && yMax <= 0
                        {
                            y1 = graph.Height - (((Math.Abs(yMin) - Math.Abs(y1)) / (Math.Abs(yMin) - Math.Abs(yMax))) * graph.Height);
                        }

                        x1 = 0;
                    }


                    if (x2 <= xMax && x2 >= xMin)
                    {
                        if (xMin < 0 && xMax >= 0)
                        {
                            if (x2 <= 0)
                                x2 = ((Math.Abs(xMin) - Math.Abs(x2)) / (Math.Abs(xMax) + Math.Abs(xMin))) * graph.Width;
                            else
                                x2 = ((Math.Abs(xMin) + x2) / (Math.Abs(xMax) + Math.Abs(xMin))) * graph.Width;
                        }
                        else if (xMin >= 0 && xMax >= 0)
                        {
                            x2 = ((x2 - xMin) / (xMax + xMin)) * graph.Width;
                        }
                        else
                        {
                            x2 = ((Math.Abs(xMin) - Math.Abs(x2)) / (Math.Abs(xMin) - Math.Abs(xMax))) * graph.Width;
                        }

                        y2 = 0;
                    }
                    else
                    {
                        y2 = xMax * m + b;

                        if (yMin < 0 && yMax >= 0)
                        {
                            if (y2 <= 0)
                                y2 = graph.Height - (((Math.Abs(yMin) - Math.Abs(y2)) / (Math.Abs(yMin) + Math.Abs(yMax))) * graph.Height);
                            else
                                y2 = graph.Height - (((Math.Abs(yMin) + Math.Abs(y2)) / (Math.Abs(yMin) + Math.Abs(yMax))) * graph.Height);
                        }
                        else if (yMin >= 0 && yMax >= 0)
                        {
                            y2 = graph.Height - ((y2 - yMin) / (yMax - yMin) * graph.Height);
                        }
                        else //yMin <= 0 && yMax <= 0
                        {
                            y2 = graph.Height - (((Math.Abs(yMin) - Math.Abs(y2)) / (Math.Abs(yMin) - Math.Abs(yMax))) * graph.Height);
                        }

                        x2 = graph.Width;
                    }

                    
                    if ((x1 > graph.Width || x1 < 0) && (x2 > graph.Width || x2 < 0) ||
                        (y1 > graph.Height || y1 < 0) && (y2 > graph.Height || y2 < 0))
                    {
                        g.DrawString("Equation 'Y = mx + b' out of scope", new Font("Arial", 16), new SolidBrush(Color.Red), 10, 0);
                    }
                    else
                    {
                        myPen.Color = color1.BackColor;
                        g.DrawLine(myPen, x1, y1, x2, y2);
                    }
                    
                }

                //draw circle
                float h = 0.0f;
                float k = 0.0f;
                float rh = 0.0f;
                float rw = 0.0f;
                if (float.TryParse(hTxt.Text, out h) && float.TryParse(kTxt.Text, out k) && float.TryParse(rTxt.Text, out rh))
                {
                    rw = rh;
                    //in each case xMin and xMax can both be positive, negative, or one positive and one negative
                    //depending on what those values are determines how values are graphed in relation to what is
                    //displayed on screen
                 
                    
                    if (h >= 0) //h > 0 then what is xMin and xMax? 
                    {
                        //if (xMin <= 0)
                       // {
                            if (xMin <= 0 && xMax >= 0)
                                h = (((Math.Abs(xMin) + Math.Abs(h)) / (Math.Abs(xMin) + Math.Abs(xMax))) * graph.Width);
                            else if (xMin >= 0 && xMax >= 0)
                                h = (((Math.Abs(h) - Math.Abs(xMin)) / (Math.Abs(xMax) - Math.Abs(xMin))) * graph.Width);
                            else //xMin <= 0 && xMax <= 0
                                h = ( (Math.Abs(xMin) + Math.Abs(h))  / (Math.Abs(xMin) - Math.Abs(xMax)) ) * graph.Width;
          
                    }
                    else //h < 0
                    {
                        if (xMin <= 0 && xMax >= 0)
                            h = (((Math.Abs(xMin) - Math.Abs(h)) / (Math.Abs(xMin) + Math.Abs(xMax))) * graph.Width);
                        else if (xMin >= 0 && xMax >= 0)
                            h = (((-Math.Abs(xMin) - Math.Abs(h)) / (Math.Abs(xMax) - Math.Abs(xMin))) * graph.Width);
                        else //xMin <= 0 && xMax <= 0
                            h = ((Math.Abs(xMin) - Math.Abs(h)) / (Math.Abs(xMin) - Math.Abs(xMax))) * graph.Width;
   
                    }
                    
                    //y-center
                    
                    if (k >= 0)
                    {
                        if (yMin <= 0 && yMax >= 0)
                            k = graph.Height - (((Math.Abs(yMin) + Math.Abs(k)) / (Math.Abs(yMin) + Math.Abs(yMax))) * graph.Height);
                        else if (yMin >= 0 && yMax >= 0)
                            k = graph.Height - (((Math.Abs(k) - Math.Abs(yMin)) / (Math.Abs(yMax) - Math.Abs(yMin))) * graph.Height);
                        else //yMin <= 0 && yMax <= 0
                            k = graph.Height - ((Math.Abs(yMin) + Math.Abs(k)) / (Math.Abs(yMin) - Math.Abs(yMax)) * graph.Height);
    
                    }
                    else
                    {

                        if (yMin <= 0 && yMax >= 0)
                            k = graph.Height - (((Math.Abs(yMin) - Math.Abs(k)) / (Math.Abs(yMin) + Math.Abs(yMax))) * graph.Height);
                        else if (yMin >= 0 && yMax >= 0)
                            k = graph.Height - (((Math.Abs(yMax) + Math.Abs(k)) / (Math.Abs(yMax) - Math.Abs(yMin))) * graph.Height);
                        else
                            k = graph.Height - (((Math.Abs(yMin) - Math.Abs(k)) / (Math.Abs(yMin) - Math.Abs(yMax))) * graph.Height);

                    }
                    
                    //now get the radius 
                    if (xMin <= 0 && xMax >= 0)
                    {
                        rw = ((Math.Abs(rw) / (Math.Abs(xMin) + Math.Abs(xMax)) * graph.Width));
                    }
                    else if (xMin >= 0 && xMax >= 0)
                    {
                        rw = ((Math.Abs(rw) / (Math.Abs(xMax) - Math.Abs(xMin)) * graph.Width));
                    }
                    else //xMin <=0 && xMax <= 0
                    {
                        rw = ((Math.Abs(rw) / (Math.Abs(xMin) - Math.Abs(xMax)) * graph.Width));
                    }

                    if (yMin <= 0 && yMax >= 0)
                    {
                        rh = ((Math.Abs(rh) / (Math.Abs(yMin) + Math.Abs(yMax)) * graph.Height));
                    }
                    else if (yMin >= 0 && yMax >= 0)
                    {
                        rh = ((Math.Abs(rh) / (Math.Abs(yMax) - Math.Abs(yMin)) * graph.Height));
                    }
                    else
                    {
                        rh = ((Math.Abs(rh) / (Math.Abs(yMin) - Math.Abs(yMax)) * graph.Height));
                    }
          
                    
                    if ( ((h - rw) <= graph.Width && (h - rw) >= 0) || ((h + rw) >= 0 && (h + rw) <= graph.Width) ) 
                    {
                         
                        if ( ((k - rh) <= graph.Height && (k - rh) >= 0) || ((k + rh) >= 0 && (k + rh) <= graph.Height))
                        {
                            myPen.Color = color4.BackColor;
                            g.DrawEllipse(myPen, h - rw, k - rh, rw + rw, rh + rh);
                        }
                        else
                            g.DrawString("Equation 'r^2 = (x-h)^2 + (y-k)^2' out of scope", new Font("Arial", 16), new SolidBrush(Color.Red), 10, 50);
                    }
                    else
                    {
                        g.DrawString("Equation 'r^2 = (x-h)^2 + (y-k)^2' out of scope", new Font("Arial", 16), new SolidBrush(Color.Red), 10, 50);
                    }
                   

                }

                //quadratic 
                float a1 = 0.0f;
                float b1 = 0.0f;
                float c1 = 0.0f;
                bool flag = false;
                //float tension = 0.0f;
                if (float.TryParse(a1Txt.Text, out a1) && float.TryParse(b1Txt.Text, out b1) && float.TryParse(c1Txt.Text, out c1))
                {
                   
                    //use 1000 points
                    PointF[] points = new PointF[1000];
                    float x = xMin;
                    float step = 0.0f;
                    float save = 0.0f;
                    float y = 0.0f;

                    //find the step value of each point 
                    if (xMin >= 0)
                        step = (Math.Abs(xMax) - Math.Abs(xMin)) / 1000;
                    else if (xMin < 0 && xMax >= 0)
                        step = (Math.Abs(xMax) + Math.Abs(xMin)) / 1000;
                    else
                        step = (Math.Abs(xMin) - Math.Abs(xMax)) / 1000;

                    int j = 0;
  
                    for (int i = 0; i < 1000; i++)
                    {
                        save = x;
                        y = a1 * x * x + b1 * x + c1;


                        if (yMin < 0 && yMax >= 0)
                        {
                            if (y <= 0)
                                y = graph.Height - (((Math.Abs(yMin) - Math.Abs(y)) / (Math.Abs(yMin) + Math.Abs(yMax))) * graph.Height);
                            else
                                y = graph.Height - (((Math.Abs(yMin) + Math.Abs(y)) / (Math.Abs(yMin) + Math.Abs(yMax))) * graph.Height);
                        }
                        else if (yMin >= 0 && yMax >= 0)
                        {
                            y = graph.Height - ((y - yMin) / (yMax - yMin) * graph.Height);
                        }
                        else //yMin <= 0 && yMax <= 0
                        {
                            if (y >= yMax)
                                y = graph.Height - (((Math.Abs(yMin) + Math.Abs(y)) / (Math.Abs(yMin) - Math.Abs(yMax))) * graph.Height);
                            else
                                y = graph.Height - (((Math.Abs(yMin) - Math.Abs(y)) / (Math.Abs(yMin) - Math.Abs(yMax))) * graph.Height);
                        }

                        if (xMin < 0 && xMax >= 0)
                        {
                            if (x <= 0)
                                x = ((Math.Abs(xMin) - Math.Abs(x)) / (Math.Abs(xMax) + Math.Abs(xMin))) * graph.Width;
                            else
                                x = ((Math.Abs(xMin) + x) / (Math.Abs(xMax) + Math.Abs(xMin))) * graph.Width;
                        }
                        else if (xMin >= 0 && xMax >= 0)
                        {
                            x = ((x - xMin) / (xMax + xMin)) * graph.Width;
                        }
                        else
                        {
                            x = ((Math.Abs(xMin) - Math.Abs(x)) / (Math.Abs(xMin) - Math.Abs(xMax))) * graph.Width;
                        }

                        if (x >= 0 && x <= graph.Width && y <= graph.Height && y >= 0)
                            flag = true;

                        points[j] = new PointF(x, y);
                        j++;
                
                        x = save;
                        x += step;
                    }

                    if (flag)
                    {
                        myPen.Color = color2.BackColor;
                        g.DrawCurve(myPen, points);
                    }
                    else
                    {
                        g.DrawString("Equation 'Y = ax^2 + bx + c' out of scope", new Font("Arial", 16), new SolidBrush(Color.Red), 10, 100);
                    }
                
                    
                }


                //quadratic 
                float a2 = 0.0f;
                float b2 = 0.0f;
                float c2 = 0.0f;
                float d2 = 0.0f;
                bool flag2 = false;

                //this is the same as above but with one extra x variable 
                //the only difference is the equation y = a2 * x * x * x + b2 * x * x + c2 * x + d2;
                if (float.TryParse(a2Txt.Text, out a2) && float.TryParse(b2Txt.Text, out b2) && float.TryParse(c2Txt.Text, out c2) && float.TryParse(d2Txt.Text, out d2))
                {
                    PointF[] points = new PointF[1000];
                    float y = 0.0f;
                    float x = xMin;
                    float step = 0.0f;
                    float save = 0.0f;

                    if (xMin >= 0)
                        step = (Math.Abs(xMax) - Math.Abs(xMin)) / 1000;
                    else if (xMin < 0 && xMax >= 0)
                        step = (Math.Abs(xMax) + Math.Abs(xMin)) / 1000;
                    else
                        step = (Math.Abs(xMin) - Math.Abs(xMax)) / 1000;

                    int j = 0;

                    for (int i = 0; i < 1000; i++)
                    {
                        save = x;
                        y = a2 * x * x * x + b2 * x * x + c2 * x + d2;
                     

                        if (yMin < 0 && yMax >= 0)
                        {
                            if (y <= 0)
                                y = graph.Height - (((Math.Abs(yMin) - Math.Abs(y)) / (Math.Abs(yMin) + Math.Abs(yMax))) * graph.Height);
                            else
                                y = graph.Height - (((Math.Abs(yMin) + Math.Abs(y)) / (Math.Abs(yMin) + Math.Abs(yMax))) * graph.Height);
                        }
                        else if (yMin >= 0 && yMax >= 0)
                        {
                            y = graph.Height - ((y - yMin) / (yMax - yMin) * graph.Height);
                        }
                        else //yMin <= 0 && yMax <= 0
                        {
                            if (y >= yMax)
                                y = graph.Height - (((Math.Abs(yMin) + Math.Abs(y)) / (Math.Abs(yMin) - Math.Abs(yMax))) * graph.Height);
                            else
                                y = graph.Height - (((Math.Abs(yMin) - Math.Abs(y)) / (Math.Abs(yMin) - Math.Abs(yMax))) * graph.Height);
                        }

                        if (xMin < 0 && xMax >= 0)
                        {
                            if (x <= 0)
                                x = ((Math.Abs(xMin) - Math.Abs(x)) / (Math.Abs(xMax) + Math.Abs(xMin))) * graph.Width;
                            else
                                x = ((Math.Abs(xMin) + x) / (Math.Abs(xMax) + Math.Abs(xMin))) * graph.Width;
                        }
                        else if (xMin >= 0 && xMax >= 0)
                        {
                            x = ((x - xMin) / (xMax + xMin)) * graph.Width;
                        }
                        else
                        {
                            x = ((Math.Abs(xMin) - Math.Abs(x)) / (Math.Abs(xMin) - Math.Abs(xMax))) * graph.Width;
                        }

                        if (x >= 0 && x <= graph.Width && y <= graph.Height && y >= 0)
                            flag2 = true;

                        points[j] = new PointF(x, y);
                        j++;
                      
                        x = save;
                        x += step;
                    }


                    if (flag2)
                    {
                        myPen.Color = color3.BackColor;
                        g.DrawCurve(myPen, points);
                    }
                    else
                    {
                        g.DrawString("Equation 'Y = ax^3 + bx^2 + cx + d' out of scope", new Font("Arial", 16), new SolidBrush(Color.Red), 10, 150);
                    }
                }
            }
        }


        /***************************************************************
       private void mTxt_Click(object sender, EventArgs e)

        Use: clears the 'placeholder' text in the field once clicked on

        Parameters: 1. object sender - the calling object/button
                    2. EventArgs e - the calling event

        Returns: nothing
        ***************************************************************/
        private void mTxt_Click(object sender, EventArgs e)
        {
            mTxt.Clear();
        }

        //the rest of these are all the same, I do not intend on creating doc boxes for all of them
        private void bTxt_Click(object sender, EventArgs e)
        {
            bTxt.Clear();
        }


        /***************************************************************
       private void color1_Click(object sender, EventArgs e)

        Use: displays a color picker to the user so they may select
        which color to use for the line

        Parameters: 1. object sender - the calling object/button
                    2. EventArgs e - the calling event

        Returns: nothing
        ***************************************************************/
        private void color1_Click(object sender, EventArgs e)
        {
            ColorDialog myDialog = new ColorDialog();

            myDialog.Color = color1.BackColor;

            if (myDialog.ShowDialog() == DialogResult.OK)
                color1.BackColor = myDialog.Color; 
        }

        //more or less all the same below 
        private void color2_Click(object sender, EventArgs e)
        {
            ColorDialog myDialog = new ColorDialog();

            myDialog.Color = color2.BackColor;

            if (myDialog.ShowDialog() == DialogResult.OK)
                color2.BackColor = myDialog.Color;
        }

        private void color3_Click(object sender, EventArgs e)
        {
            ColorDialog myDialog = new ColorDialog();

            myDialog.Color = color3.BackColor;

            if (myDialog.ShowDialog() == DialogResult.OK)
                color3.BackColor = myDialog.Color;
        }

        private void color4_Click(object sender, EventArgs e)
        {
            ColorDialog myDialog = new ColorDialog();

            myDialog.Color = color4.BackColor;

            if (myDialog.ShowDialog() == DialogResult.OK)
                color4.BackColor = myDialog.Color;
        }

        private void mTxt_Enter(object sender, EventArgs e)
        {
            if (mTxt.Text == "m")
                mTxt.Text = "";
        }

        /***************************************************************
       private void mTxt_Leave(object sender, EventArgs e)

       Use: sets back the 'placeholder' text in the field once clicked out of

       Parameters: 1. object sender - the calling object/button
                   2. EventArgs e - the calling event

       Returns: nothing
       ***************************************************************/

        private void mTxt_Leave(object sender, EventArgs e)
        {
            if (mTxt.Text == "")
                mTxt.Text = "m";
        }

        private void bTxt_Enter(object sender, EventArgs e)
        {
            if (bTxt.Text == "b")
                bTxt.Text = "";
        }

        private void bTxt_Leave(object sender, EventArgs e)
        {
            if (bTxt.Text == "")
                bTxt.Text = "b";
        }

        private void a1Txt_Enter(object sender, EventArgs e)
        {
            if (a1Txt.Text == "a")
                a1Txt.Text = "";
        }

        private void a1Txt_Leave(object sender, EventArgs e)
        {
            if (a1Txt.Text == "")
                a1Txt.Text = "a";
        }

        private void b1Txt_Enter(object sender, EventArgs e)
        {
            if (b1Txt.Text == "b")
                b1Txt.Text = "";
        }

        private void b1Txt_Leave(object sender, EventArgs e)
        {
            if (b1Txt.Text == "")
                b1Txt.Text = "b";
        }

        private void c1Txt_Enter(object sender, EventArgs e)
        {
            if (c1Txt.Text == "c")
                c1Txt.Text = "";
        }

        private void c1Txt_Leave(object sender, EventArgs e)
        {
            if (c1Txt.Text == "")
                c1Txt.Text = "c";
        }

        private void a2Txt_Enter(object sender, EventArgs e)
        {
            if (a2Txt.Text == "a")
                a2Txt.Text = "";
        }

        private void a2Txt_Leave(object sender, EventArgs e)
        {
            if (a2Txt.Text == "")
                a2Txt.Text = "a";
        }

        private void b2Txt_Enter(object sender, EventArgs e)
        {
            if (b2Txt.Text == "b")
                b2Txt.Text = "";
        }

        private void b2Txt_Leave(object sender, EventArgs e)
        {
            if (b2Txt.Text == "")
                b2Txt.Text = "b";
        }

        private void c2Txt_Enter(object sender, EventArgs e)
        {
            if (c2Txt.Text == "c")
                c2Txt.Text = "";
        }

        private void c2Txt_Leave(object sender, EventArgs e)
        {
            if (c2Txt.Text == "")
                c2Txt.Text = "c";
        }

        private void d2Txt_Enter(object sender, EventArgs e)
        {
            if (d2Txt.Text == "d")
                d2Txt.Text = "";
        }

        private void d2Txt_Leave(object sender, EventArgs e)
        {
            if (d2Txt.Text == "")
                d2Txt.Text = "d";
        }

        private void hTxt_Enter(object sender, EventArgs e)
        {
            if (hTxt.Text == "h")
                hTxt.Text = "";
        }

        private void hTxt_Leave(object sender, EventArgs e)
        {
            if (hTxt.Text == "")
                hTxt.Text = "h";
        }

        private void kTxt_Enter(object sender, EventArgs e)
        {
            if (kTxt.Text == "k")
                kTxt.Text = "";
        }

        private void kTxt_Leave(object sender, EventArgs e)
        {
            if (kTxt.Text == "")
                kTxt.Text = "k";
        }

        private void rTxt_Enter(object sender, EventArgs e)
        {
            if (rTxt.Text == "r")
                rTxt.Text = "";
        }

        private void rTxt_Leave(object sender, EventArgs e)
        {
            if (rTxt.Text == "")
                rTxt.Text = "r";
        }


        /***************************************************************
       private void Form1_Load(object sender, EventArgs e)

       Use: loads up all the tool tips

       Parameters: 1. object sender - the calling object/button
                   2. EventArgs e - the calling event

       Returns: nothing
       ***************************************************************/
        private void Form1_Load(object sender, EventArgs e)
        {
            ToolTip toolTip1 = new ToolTip();

            toolTip1.AutoPopDelay = 5000;
            toolTip1.InitialDelay = 1000;
            toolTip1.ReshowDelay = 500;
            toolTip1.OwnerDraw = false;
        
            toolTip1.SetToolTip(pb1, "m is the slope of the line and b is the y-intercept of the line");
            toolTip1.SetToolTip(pb2, "a and b are multipliers and c is the y-intercept of the quadratic line");
            toolTip1.SetToolTip(pb3, "a, b, and c are multipliers and d is the y-intercept of the cubic line");
            toolTip1.SetToolTip(pb4, "r is the radius of the circle");
            toolTip1.SetToolTip(pb5, "h is the x-coordinate of the center of the circle");
            toolTip1.SetToolTip(pb6, "k is the y-coordinate of the center of the circle");
        }
    }

   


}

