using ClientUI.Drawing;
using System.Drawing.Drawing2D;

namespace ClientUI
{
    public partial class ClientUIForm : Form
    {
        private Image spriteSheet;
        private int topPadding = 50;
        private string standText = "Client";
        private readonly string spriteMapperFileName;
        private ParseToTile tileParser;
        private readonly string tileDirectory;
        private readonly string exeDir = PathHelper.ExeDir;

        public ClientUIForm()
        {
            // set the directory in which the tilemaps will go
            tileDirectory = Path.Combine(exeDir, @".\TileMaps\");

            // use a factory to load in the data of the current tilemap
            ParseToTileFactory factory = new ParseToTileFactory();
            spriteMapperFileName = factory.Load(tileDirectory);

            // create the parser
            tileParser = factory.Create(spriteMapperFileName);

            InitializeComponent();
            DoubleBuffered = true;
            spriteSheet = Bitmap.FromFile(Path.Combine(exeDir, "sprites.png"));
            this.Text = standText;
        }

        // drawing of the gamefield
        protected override void OnPaint(PaintEventArgs e)
        {
            List<Tile> tiles = new List<Tile>();
            for(int i = 0; i < 80; i++)
            {
                tiles.Add(tileParser.Parse("background", i, 10, 16));
            }
            base.OnPaint(e);
            Graphics g = e.Graphics;
            //make nice pixels
            g.SmoothingMode = SmoothingMode.None;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            foreach (Tile tile in tiles)
            {
                g.DrawImage(spriteSheet, tile.placement, tile.sprite, GraphicsUnit.Pixel);
            }
        }
    }
}