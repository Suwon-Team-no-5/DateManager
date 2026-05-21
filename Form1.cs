using System;
using System.Windows.Forms;
using DateManager.services; // 서비스 네임스페이스를 사용합니다.

namespace DateManager
{
    public partial class Form1 : Form
    {
        // develop 브랜치의 명명 규칙을 따르는 것이 좋습니다.
        private CatalogFrameController? catalogFrameController;

        public Form1()
        {
            InitializeComponent();

            // 컨트롤러가 UI 컨트롤들을 찾아서 자동으로 연결합니다.
            catalogFrameController = CatalogFrameController.TryAttach(this);

            if (catalogFrameController == null)
            {
                MessageBox.Show("UI 컨트롤 연결 실패! 디자인 창에서 버튼/라벨 이름을 확인하세요.");
            }
        }
    }
}