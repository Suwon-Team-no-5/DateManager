using System;
using System.Windows.Forms;
using DateManager.services; // 서비스 네임스페이스 추가

namespace DateManager
{
    public partial class Form1 : Form
    {
        private CatalogFrameController? controller;

        public Form1()
        {
            InitializeComponent();

            // 컨트롤러가 UI 컨트롤들을 찾아서 자동으로 연결합니다.
            controller = CatalogFrameController.TryAttach(this);

            if (controller == null)
            {
                MessageBox.Show("UI 컨트롤 연결 실패! 디자인 창에서 버튼/라벨 이름을 확인하세요.");
            }
        }
    }
}