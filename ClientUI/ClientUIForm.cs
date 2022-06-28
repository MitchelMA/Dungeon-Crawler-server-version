using System.Threading;
using System.Drawing.Drawing2D;
using ClientUI.Drawing;
using ClientUI.Client;

namespace ClientUI
{
    public partial class ClientUIForm : Form
    {
        internal Image spriteSheet;
        internal int topPadding = 50;
        internal string standText = "Client";
        internal readonly string spriteMapperFileName;
        internal ParseToTile tileParser;

        private readonly string tileDirectory;
        private readonly string exeDir = PathHelper.ExeDir;

        private ClientSocket clientSocket;

        // items to draw the gamefield
        private List<Tile> tiles = new List<Tile>();
        private int fieldWidth = 10;
        private int tileSize = 16;

        // other threads
        Task connectionThread;
        public ClientUIForm()
        {
            InitializeComponent();

            // set the directory in which the tilemaps will go
            tileDirectory = Path.Combine(exeDir, @".\TileMaps\");

            // use a factory to load in the data of the current tilemap
            ParseToTileFactory factory = new ParseToTileFactory();
            spriteMapperFileName = factory.Load(tileDirectory);

            // create the parser
            tileParser = factory.Create(spriteMapperFileName);

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
            clientSocket.Close();
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
            string[] fieldParts = parts.Where((source, index) => index >= 6).ToArray();
            fieldWidth = (fieldParts[0]).Length + 1;
            // clear the tiles
            tiles.Clear();
            string field = String.Join('\0', fieldParts);
             
            // now turn the field int a list of tiles
            for(int i = 0; i < field.Length; i++)
            {
                try
                {
                    Tile tile = tileParser.Parse(field[i], i, fieldWidth, tileSize);
                    tiles.Add(tile);
                } catch { }
            }
            Console.WriteLine("Debug Six");
            // now invalidate the canvas to force it to paint again
            Invalidate();
        }

        private void ConnectBtn_Click(object sender, EventArgs e)
        {
            // try to dispose the client first
            try
            {
                if(clientSocket != null)
                    clientSocket.Close();
            } catch { }
            Console.WriteLine("Clicked!");
            // get the values of the the text-boxes
            string host = "127.0.0.1";
            int port = 80;
            try
            {
                host = IPAddressTSTB.Text;
                port = int.Parse(PortTSTB.Text);
            }
            catch(Exception err) 
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
             if(clientSocket != null)
            {
                connectionThread = new Task(clientSocket.Connect);
                connectionThread.Start();
            }
        }
    }
}