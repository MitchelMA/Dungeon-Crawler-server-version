namespace ClientUI
{
    public partial class ClientUIForm : Form
    {
        private Image spriteSheet;
        private int topPadding = 50;
        private string standText = "Client";
        private readonly string exeDir = PathHelper.ExeDir;

        public ClientUIForm()
        {
            InitializeComponent();
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