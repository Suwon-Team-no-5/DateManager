using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DateManager
{
    public partial class Form1 : Form
    {
        // 데이터 정제 및 로드를 담당하는 핵심 백엔드 클래스 선언
        private DataProcessor _dataProcessor;

        // 메모리에 로드된 전체 카탈로그 데이터를 담아둘 마스터 리스트
        private List<DonkeyFrame> _masterFrameList;

        private FileRemover _fileRemover;

        private Picture _pictureHandler;

        public Form1()
        {
            InitializeComponent();

            // 프로그램이 켜질 때 객체들을 초기화해 줍니다.
            _dataProcessor = new DataProcessor();
            _fileRemover = new FileRemover();
            _masterFrameList = new List<DonkeyFrame>();
            _pictureHandler = new Picture();
        }

        /// <summary>
        /// 폼이 처음 로드될 때 실행되는 함수입니다. (중복 제거 완료)
        /// </summary>
        private void Form1_Load(object sender, EventArgs e)
        {
            // 필요한 경우 여기에 초기화 코드를 넣습니다.
        }

        /// <summary>
        /// 디자이너 창에서 추가한 테스트 버튼(button1)을 더블클릭했을 때 실행되는 함수입니다.
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            // 💡 진철님 컴퓨터의 실제 WSL Ubuntu 경로 설정 완료!
            string catalogPath = @"\\wsl.localhost\Ubuntu-22.04\home\jinchul04\mycar\data\catalog_0.catalog";
            string imagesFolderPath = @"\\wsl.localhost\Ubuntu-22.04\home\jinchul04\mycar\data\images";

            // 1. 테스트 시작 알림
            MessageBox.Show("진철님의 데이터 엔진으로 파일 읽기를 시작합니다!", "테스트 시작", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // 2. data.cs에 구현해 둔 가공(파싱) 및 자동 분류 함수 호출
            _masterFrameList = _dataProcessor.LoadCatalogData(catalogPath, imagesFolderPath);

            // 3. 데이터가 성공적으로 로드되었다면 신규/기존 데이터 분리 결과 최종 검증
            if (_masterFrameList != null && _masterFrameList.Count > 0)
            {
                // IsNewData 깃발이 true인 새로 수집된 불량 데이터만 골라내기
                List<DonkeyFrame> newDataOnly = _masterFrameList.FindAll(frame => frame.IsNewData == true);

                string resultReport = $"🏁 [검증 결과 최종 리포트]\n\n" +
                                      $"📊 전체 데이터 수: {_masterFrameList.Count}개\n" +
                                      $"❌ 새로 수집된 불량 데이터(정제 대상): {newDataOnly.Count}개\n\n" +
                                      $"자동 구분이 정상적으로 작동합니다. 이제 UI 팀원에게 코드를 넘겨도 좋습니다!";

                MessageBox.Show(resultReport, "엔진 검증 완료", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
            // 라벨 클릭 이벤트가 필요 없다면 비워둡니다.
        }

        private void btnLoadTub_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Donkeycar 데이터(Tub) 폴더를 선택하세요.";
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    string selectedPath = fbd.SelectedPath;

                    // 데이터 폴더 내의 카탈로그와 이미지 경로를 자동 추론
                    string catalogPath = Path.Combine(selectedPath, "catalog_0.catalog");
                    string imagesFolderPath = Path.Combine(selectedPath, "images");

                    // 엔진 가동: 이미지와 데이터를 합쳐서 리스트로 가져옴
                    _masterFrameList = _dataProcessor.LoadCatalogData(catalogPath, imagesFolderPath);

                    // 로드 성공 시 리스트박스 업데이트
                    if (_masterFrameList != null)
                    {
                        lstFrameData.Items.Clear();
                        foreach (var frame in _masterFrameList)
                        {
                            // [이미지] + [조향값]이 합쳐진 형태를 보여줌
                            lstFrameData.Items.Add($"Frame {frame.FrameIndex} | Angle: {frame.Angle:F2}");
                        }
                    }
                }
            }
        }

        private void btnApplyFilter_Click(object sender, EventArgs e)
        {
            if (_masterFrameList == null || _masterFrameList.Count == 0)
            {
                MessageBox.Show("먼저 데이터를 로드해 주세요!", "알림");
                return;
            }

            // 마스터 리스트를 복사해서 필터링을 시작할 임시 리스트 생성
            List<DonkeyFrame> filteredList = new List<DonkeyFrame>(_masterFrameList);

            // 1. Thr > 0 체크박스가 켜져있을 때
            if (chkFilterThr.Checked)
            {
                // Throttle 값이 0보다 큰 것만 남기기 (변수명은 진철님 DonkeyFrame 구조에 맞춤)
                filteredList = filteredList.FindAll(frame => frame.Throttle > 0);
            }

            // 2. Angle == 0 체크박스가 켜져있을 때 (직진 데이터만 필터링)
            if (chkFilterAngleZero.Checked)
            {
                filteredList = filteredList.FindAll(frame => frame.Angle == 0);
            }

            // 3. 필터링된 결과를 우측 리스트박스(lstFrameData)에 다시 업데이트
            lstFrameData.Items.Clear();
            foreach (var frame in filteredList)
            {
                lstFrameData.Items.Add($"[필터됨] Frame - Angle: {frame.Angle}");
            }

            MessageBox.Show($"필터링 완료! {filteredList.Count}개의 데이터가 조건에 맞습니다.", "필터 결과");
        }

        private void btnDeleteData_Click(object sender, EventArgs e)
        {
            // 1. 선택된 데이터가 있는지 확인// 1. 선택된 데이터가 있는지 확인 (리스트박스에서 선택된 항목이 없으면 -1 반환)
            if (lstFrameData.SelectedIndex == -1)
            {
                MessageBox.Show("삭제할 프레임을 리스트에서 선택해주세요!", "선택 필요");
                return;
            }

            // 2. 삭제할 프레임 객체 가져오기
            // 리스트박스의 순서와 _masterFrameList의 순서가 일치한다고 가정합니다.
            int selectedIndex = lstFrameData.SelectedIndex;
            DonkeyFrame targetFrame = _masterFrameList[selectedIndex];

            // 3. 사용자 확인 절차
            DialogResult result = MessageBox.Show($"Frame {targetFrame.FrameIndex}번 데이터를 삭제할까요?\n이 작업은 되돌릴 수 없습니다.",
                                                  "삭제 확인", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.No) return;

            // 4. 새로운 클래스(FileRemover)를 사용하여 파일 및 리스트 삭제 호출
            try
            {
                // 💡 여기서 분리한 클래스의 메서드를 호출합니다.
                // _fileRemover는 Form1 생성자에서 미리 선언 및 초기화해 두어야 합니다.
                _fileRemover.RemoveFrame(_masterFrameList, targetFrame);

                // 5. UI 업데이트: 리스트박스에서 해당 항목 제거
                lstFrameData.Items.RemoveAt(selectedIndex);

                MessageBox.Show("삭제가 성공적으로 완료되었습니다.", "정제 완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                // 삭제 과정에서 발생한 에러 처리
                MessageBox.Show($"삭제 중 오류 발생: {ex.Message}", "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lstFrameData_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = lstFrameData.SelectedIndex;
            if (index >= 0 && index < _masterFrameList.Count)
            {
                DonkeyFrame selectedFrame = _masterFrameList[index];

                // 1. 이미지 연동
                _pictureHandler.LoadImageToPictureBox(pbMainCam, selectedFrame.FullImagePath);

                // 2. 조향값 및 정보 표시 (UI 라벨 이름에 맞춰 수정하세요)
                // 화면 상단에 조향값, 출력값이 같이 나와야 형님이 좋아합니다!
                lblFrameIndex.Text = $"프레임 인덱스 {selectedFrame.FrameIndex}/{_masterFrameList.Count}";
                lblAngle.Text = $"조향값(앵글): {selectedFrame.Angle:F3}";
                lblThrottleTop.Text = $"출력(스레틀): {selectedFrame.Throttle:F3}";
            }
        }
    }
}