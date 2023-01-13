using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Speech.Recognition;
using System.Threading;
using System.IO;

namespace Speech_Tutorial_2
{
    public partial class Form1 : Form
    {

        public SpeechRecognitionEngine recognizer;
        public Grammar grammar;
        public Thread RecThread;
        public Boolean RecognizerState = true;
        public List<string> rhinoCommands = new List<string>();
        Choices commands = new Choices();

        public Form1()
        {
            InitializeComponent();

            TopMost = true;
            

            String line;
            try
            {
                //Pass the embeded file here to the StreamReader constructor
                //StreamReader sr = new StreamReader(@"D:\Timo\Documents\GSD Yr IV\CS50\final\RhinoCommand.txt");
                StringReader sr = new StringReader(Properties.Resources.RhinoCommand);

                //Read the text
                line = sr.ReadLine();

                //Continue untill end of file
                while (line != null)
                {
                    //make lower case
                    //rhinoCommands.Add(line.ToLower());
                    //commands.Add(line.ToLower());
                    
                    //Add the commands to a list
                    commands.Add(line);

                    //write the lien to console window
                    //richTextBox1.Text += ( line.ToLower() + "\n");

                    //Read the next line
                    line = sr.ReadLine();
                }



                    sr.Close();

                for (int ii = 0; ii < 1000; ii++)
                {
                    commands.Add(ii.ToString());
                }              


                return;
            }
            catch (Exception k)
            {
                richTextBox1.Text += (" " + k + "\n");
            }
            finally
            {
                //indicate its done
                richTextBox1.Text += ("READY\n");
            }
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            //Build the diction            
            GrammarBuilder build = new GrammarBuilder();
            //build.AppendDictation();
            build.Append(commands);

            //add the diction
            grammar = new Grammar(build);
 
            //turn on recognizer 
            recognizer = new SpeechRecognitionEngine();
            recognizer.LoadGrammar(grammar);
            recognizer.SetInputToDefaultAudioDevice();

            //find recognized event
            recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(recognizer_SpeechRecognized);
            //recognizer.SpeechHypothesized += new EventHandler<SpeechHypothesizedEventArgs>(recognizer_SpeechRecognized);

            //initialize the recognizer thread
            RecognizerState = true;
            RecThread = new Thread(new ThreadStart(RecThreadFunction));
            RecThread.Start();
 
        }

        public void recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        //public void recognizer_SpeechRecognized(object sender, SpeechHypothesizedEventArgs e)
        {
            //Recognizer recognizes
            if (!RecognizerState)
                return;
            this.Invoke((MethodInvoker)delegate
            {
                //standarize the result
                String Nresult = e.Result.Text.ToLower();
                Nresult = Nresult.Replace(".", " ");
                Nresult = Nresult.Trim();
                
                //if (rhinoCommands.Contains(e.Result.Text.ToLower()))
                //if (rhinoCommands.Any(o => o.StartsWith(Nresult)))

                //display and output the results
                if (true)
                {
                    richTextBox1.Text += ( Nresult + "\n");
                    //richTextBox1.AppendText(Nresult + "\n");

                    //scroll to bottom
                    richTextBox1.SelectionStart = richTextBox1.Text.Length;
                    richTextBox1.ScrollToCaret();

                    if ( Nresult == "enter")
                    {
                        SendKeys.Send("{ENTER}");
                    }
                                        
                    else
                    {
                        int throwaway;
                        if (Int32.TryParse(Nresult, out throwaway))
                        {
                            SendKeys.Send(Nresult);
                            SendKeys.Send("{ENTER}");
                        }
                        else
                        {
                            SendKeys.Send("{ESC}");
                            SendKeys.Send(Nresult);
                            SendKeys.Send("{ENTER}");
                        }
                        
                    }
                    
                }                  
                
                else
                {
                    richTextBox1.Text += ("invalide:" + Nresult + "\n");
                }
            
            });
        }

        public void RecThreadFunction()
        {
            
            while (true)
            {
                try
                {
                    recognizer.Recognize();
                }
                catch
                {
                    //handles errors
                    //nothing happens
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //when enabled set top and display ready
            RecognizerState = true;
            TopMost = true;
            richTextBox1.Text += ("READY\n");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //when disabled do the opposite
            RecognizerState = false;
            richTextBox1.Text += ("DISABLED\n");
            TopMost = false;

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //unload all memory
            RecThread.Abort();
            RecThread = null;
            recognizer.UnloadAllGrammars();
            recognizer.Dispose();
            grammar = null;
            for (int j = 0; j < rhinoCommands.Count; j++)
            {
                rhinoCommands[j] = null;
            }
            TopMost = false;

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged_1(object sender, EventArgs e)
        {

        }
    }
}
