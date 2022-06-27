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
        }

        // drawing of the gamefield
        protected override void OnPaint(PaintEventArgs e)
        {
            List<Tile> tiles = new List<Tile>();
            for(int i = 0; i < 80; i++)
            {
                tiles.Add(tileParser.Parse("background", i, 10, 40));
            }
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
    }
}