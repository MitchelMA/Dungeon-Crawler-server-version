using ClientUI.Drawing;

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

            Tile test = tileParser.Parse("background", 20, 10, 16);
            string positionString = $"[{test.placement.X}; {test.placement.Y}] : [{test.placement.Height}; {test.placement.Width}]";
            string spriteString = $"[{test.sprite.X}; {test.sprite.Y}] : [{test.sprite.Height}; {test.sprite.Width}]";

            InitializeComponent();
            DoubleBuffered = true;
            spriteSheet = Image.FromFile(Path.Combine(exeDir, "sprites.png"));
            this.Text = standText;
        }

        // drawing of the gamefield
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.DrawImage(spriteSheet, new Rectangle(0, topPadding, spriteSheet.Width, spriteSheet.Height));
        }
    }
}