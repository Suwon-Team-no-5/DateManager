namespace DateManager
{
    public partial class Form1 : Form
    {
        private services.CatalogFrameController? catalogFrameController;

        public Form1()
        {
            InitializeComponent();
            catalogFrameController = services.CatalogFrameController.TryAttach(this);// CatalogFrameController를 Form1에 연결하여 사용할 수 있도록 설정
        }
    }
}
