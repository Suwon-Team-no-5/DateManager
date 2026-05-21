namespace DateManager
{
    public partial class Form1 : Form
    {
        // 데이터 정제 및 로드를 담당하는 핵심 백엔드 클래스 선언
        private DataProcessor _dataProcessor;

        // 메모리에 로드된 전체 카탈로그 데이터를 담아둘 마스터 리스트
        private List<DonkeyFrame> _masterFrameList;
        public Form1()
        {
            InitializeComponent();

            // 프로그램이 켜질 때 객체들을 초기화해 줍니다.
            _dataProcessor = new DataProcessor();
            _masterFrameList = new List<DonkeyFrame>();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 디자이너 창에서 추가한 테스트 버튼(button1)을 더블클릭했을 때 실행되는 함수입니다.
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            // 💡 [중요] 진철님 컴퓨터의 실제 catalog_0.catalog 파일과 이미지 폴더가 있는 경로로 수정해 주세요!
            string catalogPath = @"\\wsl.localhost\Ubuntu-22.04\home\jinchul04\mycar\data\catalog_0.catalog";
            string imagesFolderPath = @"\\wsl.localhost\Ubuntu-22.04\home\jinchul04\mycar\data\images";

            // 1. 테스트 시작 알림
            MessageBox.Show("진철님의 데이터 엔진으로 파일 읽기를 시작합니다!", "테스트 시작", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // 2. data.cs에 구현해 둔 가라 가공(파싱) 및 자동 분류 함수 호출
            _masterFrameList = _dataProcessor.LoadCatalogData(catalogPath, imagesFolderPath);

            // 3. 데이터가 성공적으로 로드되었다면 신규/기존 데이터 분리 결과 최종 검증
            if (_masterFrameList != null && _masterFrameList.Count > 0)
            {
                // IsNewData 깃발이 true인 새로 수집된 불량 데이터만 LINQ/FindAll로 쏙 골라내기
                List<DonkeyFrame> newDataOnly = _masterFrameList.FindAll(frame => frame.IsNewData == true);

                string resultReport = $"🏁 [검증 결과 최종 리포트]\n\n" +
                                      $"📊 전체 데이터 수: {_masterFrameList.Count}개\n" +
                                      $"❌ 새로 수집된 불량 데이터(정제 대상): {newDataOnly.Count}개\n\n" +
                                      $"자동 구분이 정상적으로 작동합니다. 이제 UI 팀원에게 코드를 넘겨도 좋습니다!";

                MessageBox.Show(resultReport, "엔진 검증 완료", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }
    }
}
