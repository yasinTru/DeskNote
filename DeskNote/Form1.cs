using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;
using System.Media;
using System.Data.SqlClient;

namespace DeskNote
{
    public partial class Form1 : Form
    {
        SqlConnection conn = new SqlConnection("Data Source=DESKTOP-DM8CPLP\\SQLEXPRESS; Database=todo_DB; Integrated Security=True;");
        SqlDataAdapter adapter = new SqlDataAdapter();
        DataSet ds = new DataSet();
        string currentId = "";
        System.Timers.Timer timer;
        string path;
        public Form1()
        {
            InitializeComponent();
        }

        public void GetRecord()
        {
            ds = new DataSet();
            adapter = new SqlDataAdapter("select * from Task_Table", conn);
            adapter.Fill(ds, "Task_Table");
            dataGridView1.DataSource = ds;
            dataGridView1.DataMember = "Task_Table";
        }
      
        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            path = string.Empty;
            textBox.Clear();
            
       
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog fileD= new OpenFileDialog() { Filter="Text Documents |*.txt", ValidateNames=true, Multiselect=false })
            {
                if(fileD.ShowDialog()==DialogResult.OK)
                {

                    using (StreamReader sr = new StreamReader(fileD.FileName))
                    {
                        path = fileD.FileName;
                        Task<string> text = sr.ReadToEndAsync();
                        textBox.Text = text.Result;                       
                    }
                    
                }
            }
        }

        private async void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(path))
            {
                using (SaveFileDialog SaveFD = new SaveFileDialog() { Filter = "Text Documents | *.txt", ValidateNames = true })
                {
                    if (SaveFD.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            path = SaveFD.FileName;
                            using (StreamWriter sw = new StreamWriter(SaveFD.FileName))
                            {
                                await sw.WriteLineAsync(textBox.Text);
                            }

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }

                }
            }
            else
            {
                try
                {
                    using (StreamWriter sw = new StreamWriter(path))
                    {
                        await sw.WriteLineAsync(textBox.Text);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        

        private async void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog SaveFD = new SaveFileDialog() { Filter = "Text Documents | *.txt", ValidateNames = true })
            {
                if (SaveFD.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (StreamWriter sw = new StreamWriter(SaveFD.FileName))
                        {
                            await sw.WriteLineAsync(textBox.Text);
                        }
                    }

                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer = new System.Timers.Timer();
            timer.Interval = 1000;
            timer.Elapsed += Timer_Elapsed;
            GetRecord();
        }

        delegate void UpdateLable(Label lbl, string value);
        void UpdateDataLable(Label lbl,string value)
        {
            lbl.Text = value;
        }
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            DateTime currentTime = DateTime.Now;
            DateTime personTime = dateTimePicker.Value;
            if(currentTime.Hour==personTime.Hour && currentTime.Minute==personTime.Minute && currentTime.Second==personTime.Second)
            {
                timer.Stop();
                try
                {
                    UpdateLable update = UpdateDataLable;
                    if (label1.InvokeRequired)
                        Invoke(update, label1, "Stop");
                    SoundPlayer playing= new SoundPlayer();
                    playing.SoundLocation = @"C:\Windows\Media\Alarm03.wav";
                   
                        playing.PlayLooping();
                    
                   
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
        }

        private void btnSet_Click(object sender, EventArgs e)
        {
            timer.Start();
            label1.Text = "Alexa Will Play Despacito 2...";
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            timer.Stop();
            SoundPlayer playing = new SoundPlayer();
            playing.SoundLocation = @"C:\Windows\Media\Alarm03.wav";
            playing.Stop();
            label1.Text = "Stopped. Set Again";
            
        }

        private void btn_Add_Click(object sender, EventArgs e)
        {
            ds = new DataSet();
            adapter = new SqlDataAdapter("insert into Task_Table (task_content) VALUES ('" + textBox1.Text + "')", conn);
            adapter.Fill(ds, "Task_Table");
            textBox1.Clear();
            GetRecord();

        }

        private void btn_Update_Click(object sender, EventArgs e)
        {
            int i = dataGridView1.CurrentRow.Index;
            currentId = dataGridView1[0, i].Value.ToString();

            ds = new DataSet();
            adapter = new SqlDataAdapter("update Task_Table set task_content= '" + textBox1.Text + "' where task_id=" + currentId, conn);
            adapter.Fill(ds, "Task_Table");
            textBox1.Clear();
            GetRecord();

        }

        private void btn_Edit_Click(object sender, EventArgs e)
        {
            int i = dataGridView1.CurrentRow.Index;
            currentId = dataGridView1[0, i].Value.ToString();
            textBox1.Text = dataGridView1[1, i].Value.ToString(); //bu 1'di

        }

        private void btn_Delete_Click(object sender, EventArgs e)
        {
            try
            {
            int i = dataGridView1.CurrentRow.Index;
            currentId = dataGridView1[0, i].Value.ToString();
            
                ds = new DataSet();
                adapter = new SqlDataAdapter("delete from Task_Table where task_id= " + currentId, conn);
                adapter.Fill(ds, "Task_Table");
                GetRecord();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cant Delete main columns. ", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
        }
    }
}
