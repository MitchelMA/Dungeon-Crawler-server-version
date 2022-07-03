using System.Threading;
using System.Drawing.Drawing2D;
using ClientUI.Drawing;
using ClientUI.Client;
using System.Diagnostics;
using Shared;

namespace ClientUI
{
    public partial class ClientUIForm : Form
    {
        internal Image spriteSheet;
        internal int topPadding = 32;
        internal string standText = "Client";
        internal readonly string[] spriteMapperFileName;
        internal ParseToTile tileParser;
        private ParseToTileFactory parserFactory;

        private readonly string tileDirectory;
        private readonly string exeDir = PathHelper.ExeDir;

        private ClientSocket clientSocket;

        // items to draw the gamefield
        private List<Tile> tiles = new List<Tile>();
        private int fieldWidth = 10;
        private int tileSize = 16;

        // other tasks
        // connection task
        Task connectionTask;
        CancellationTokenSource connectionCTokenSource = new CancellationTokenSource();
        CancellationToken connectionCToken;
        public ClientUIForm()
        {
            InitializeComponent();

            // set the directory in which the tilemaps will go
            tileDirectory = Path.Combine(exeDir, @".\TileMaps\");

            // use a factory to load in the data of the current tilemap
            parserFactory = new ParseToTileFactory();
            spriteMapperFileName = parserFactory.Load(tileDirectory);

            // now create a dropdown at the theme menu-strip-item
            ThemeTSMI.DropDownItems.Clear();
            foreach(string filename in spriteMapperFileName)
            {
                ToolStripMenuItem dropItem = new ToolStripMenuItem();
                dropItem.Text = filename;
                dropItem.Name = filename + "TSMI";
                dropItem.Size = new Size(102, 22);
                dropItem.Click += new EventHandler(this.DropItem_Click);
                ThemeTSMI.DropDownItems.Add(dropItem);
            }

            // create the parser
            ThemeTSMI.DropDownItems[0].PerformClick();


            // get a cancellation token for the tasks
            connectionCToken = connectionCTokenSource.Token;

            // reading from a spritesheet or "bitmap" is weird: you need uneven, preceding cords for the x-axis
            // for instance, if you want to have sprite with an x-cord of `22`, your input x-cord should be `21`
            // why? I don't know, that's just how it works or something?
            spriteSheet = Bitmap.FromFile(Path.Combine(exeDir, "sprites.png"));
            this.Text = standText;
            DoubleBuffered = true;
            FormClosing += ClientUIForm_FormClosing;
        }

        private void ClientUIForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            // try to formally dispose the task
            try
            {
                connectionCTokenSource.Cancel();
                if(connectionTask != null)
                {
                    connectionTask.Dispose();
                }
                if(clientSocket != null)
                {
                    clientSocket.Close();
                }
            }
            catch { };
        }

        // drawing of the gamefield
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            //make nice pixels
            g.SmoothingMode = SmoothingMode.None;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            foreach (Tile tile in tiles)
            {
                tile.placement.Offset(0, topPadding);
                g.DrawImage(spriteSheet, tile.placement, tile.sprite, GraphicsUnit.Pixel);
            }
        }

        // setup all the tiles of the gamefield
        internal void SetupField(string fieldData)
        {
            // get the width of the gamefield
            string[] parts = fieldData.Split(Environment.NewLine, StringSplitOptions.None);
            // by definition, the gamefield starts at index 6
            // and all the player-info is before that
            string[] playerInfo = parts.Where((source, index) => index < 6).ToArray();
            // display all the playerInfo
            string playerInfoS = String.Join(Environment.NewLine, playerInfo);
            // set the text with the invoke to run it in the main thread (this method gets called by another task)
            Invoke(() =>
            {
                PlayerInfoL.Text = playerInfoS;
            });

            string[] fieldParts = parts.Where((source, index) => index >= 6).ToArray();
            fieldWidth = (fieldParts[0]).Length + 1;
            string field = String.Join('\0', fieldParts);
            // set the new values of the tiles
            tiles = tileParser.ParseTextNew(field, fieldWidth, tileSize);
            // now invalidate the canvas to force it to paint again
            Invalidate();
        }

        private void ConnectBtn_Click(object sender, EventArgs e)
        {
            // try to dispose the client first
            try
            {
                if (clientSocket != null)
                    clientSocket.Close();
            }
            catch { }
            // get the values of the the text-boxes
            string host = "127.0.0.1";
            int port = 80;
            try
            {
                host = IPAddressTSTB.Text;
                port = int.Parse(PortTSTB.Text);
            }
            catch (Exception err)
            {
                port = 80;
            }

            try
            {
                clientSocket = new ClientSocket(host, port, this);
            }
            catch { }

            // now try to connect
            // connecto to the client in a new thread
            if (clientSocket != null)
            {
                if (connectionTask != null)
                {
                    connectionCTokenSource.Cancel();
                }
                // create a new CancellationTokenSource, from which I can get a new token to reset the cancelled status
                connectionCTokenSource = new CancellationTokenSource();
                connectionCToken = connectionCTokenSource.Token;
                try
                {
                    connectionTask = new Task(() => clientSocket.Connect(connectionCToken), connectionCToken);
                    connectionTask.Start();
                }
                catch { }
            }
        }

        private void DropItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem btn = (ToolStripMenuItem)sender;
            if(Array.IndexOf(spriteMapperFileName, btn.Text) != -1)
            {
                foreach(ToolStripMenuItem item in ThemeTSMI.DropDownItems)
                {
                    item.Checked = false;
                }
                btn.Checked = true;
                tileParser = parserFactory.Create(btn.Text);
            }
        }

        // override the ProcessCmdKey so I can take keypress input from the user to play the game
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (clientSocket != null && clientSocket.client.Connected)
            {
                try
                {
                    switch (keyData)
                    {
                        case Keys.Escape:
                            {
                                string inp = Enum.GetName(typeof(Input), Input.quit);
                                string inpE = clientSocket.DataSecurity.EncryptAES(inp);
                                clientSocket.SendMessage(inpE);
                                clientSocket.Close();
                                ClearScreen();
                            }
                            break;
                        case Keys.Q:
                            {
                                string inp = Enum.GetName(typeof(Input), Input.quit);
                                string inpE = clientSocket.DataSecurity.EncryptAES(inp);
                                clientSocket.SendMessage(inpE);
                                clientSocket.Close();
                                ClearScreen();
                            }
                            break;
                        case Keys.Up:
                            {
                                string inp = Enum.GetName(typeof(Input), Input.up);
                                string inpE = clientSocket.DataSecurity.EncryptAES(inp);
                                clientSocket.SendMessage(inpE);
                            }
                            break;
                        case Keys.Right:
                            {
                                string inp = Enum.GetName(typeof(Input), Input.right);
                                string inpE = clientSocket.DataSecurity.EncryptAES(inp);
                                clientSocket.SendMessage(inpE);
                            }
                            break;
                        case Keys.Down:
                            {
                                string inp = Enum.GetName(typeof(Input), Input.down);
                                string inpE = clientSocket.DataSecurity.EncryptAES(inp);
                                clientSocket.SendMessage(inpE);
                            }
                            break;
                        case Keys.Left:
                            {
                                string inp = Enum.GetName(typeof(Input), Input.left);
                                string inpE = clientSocket.DataSecurity.EncryptAES(inp);
                                clientSocket.SendMessage(inpE);
                            }
                            break;
                    }
                    Thread.Sleep(050);
                }
                catch { }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        internal void ClearScreen()
        {
            tiles.Clear();
            PlayerInfoL.Text = "";
            Invalidate();
        }
    }
}