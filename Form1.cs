using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace DateManager
{
    // 💡 디자이너 오류 방지를 위해 Form1 클래스가 무조건 맨 위에 위치해야 합니다.
    public partial class Form1 : Form
    {
        // UI 내부에서 데이터 관리를 위해 사용할 전역 변수들
        private List<FrameData> frameList = new List<FrameData>();
        private int currentFrameIndex = 0;

        public Form1()
        {
            InitializeComponent();
            LoadDummyData();
        }

        // 프로그램 폼이 처음 켜질 때 실행되는 이벤트
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        // [미션 2] 핵심 함수: 인덱스를 받아서 화면의 텍스트와 이미지 매칭 표시
        private void UpdateFrameDisplay(int index)
        {
            if (frameList == null || frameList.Count == 0 || index < 0 || index >= frameList.Count)
                return;

            currentFrameIndex = index;
            FrameData currentData = frameList[index];

            // 라벨 업데이트
            lblFrameIndex.Text = $"Frame Index {currentData.Index} / {frameList.Count - 1}";
            lblAngle.Text = $"Angle: {currentData.Angle:F2}";
            lblThrottleTop.Text = $"Throttle: {currentData.Throttle:F2}";
            lblThrottleBottom.Text = $"Throttle: {currentData.Throttle:F2}";
            lblTimestamp.Text = $"Timestamp: {currentData.Timestamp}";

            // 파일 존재 여부 확인 후 처리
            if (File.Exists(currentData.ImagePath))
            {
                // 이전 이미지가 있다면 메모리에서 확실하게 제거
                if (pbMainCam.Image != null)
                {
                    pbMainCam.Image.Dispose();
                    pbMainCam.Image = null;
                }

                // 이미지 로드
                using (FileStream fs = new FileStream(currentData.ImagePath, FileMode.Open, FileAccess.Read))
                {
                    pbMainCam.Image = Image.FromStream(fs);
                }
            }
            else
            {
                // ✨ 수정: 파일이 없을 때 이미지를 안전하게 해제하여 빈 화면 출력
                if (pbMainCam.Image != null)
                {
                    pbMainCam.Image.Dispose();
                    pbMainCam.Image = null;
                }
            }
        }

        // 임시 테스트용 데이터 생성 함수
        private void LoadDummyData()
        {
            frameList.Add(new FrameData { Index = 0, ImagePath = "dummy1.jpg", Angle = +0.86f, Throttle = 0.75f, Timestamp = "2024-05-13 14:30:15.88" });
            frameList.Add(new FrameData { Index = 1, ImagePath = "dummy2.jpg", Angle = +0.00f, Throttle = 0.80f, Timestamp = "2024-05-13 14:30:15.90" });
            frameList.Add(new FrameData { Index = 2, ImagePath = "dummy3.jpg", Angle = -0.50f, Throttle = 0.50f, Timestamp = "2024-05-13 14:30:15.95" });

            // ✨ 확정된 트랙바 변수명(trkFrameSlider) 반영 및 최대값 설정
            trkFrameSlider.Maximum = frameList.Count - 1;

            // 데이터 로드 후 가장 첫 번째 프레임(0번)을 화면에 실시간 업데이트
            UpdateFrameDisplay(0);
        }

        // 🕹️ [미션 3] 트랙바(슬라이더) 스크롤 실시간 연동 이벤트
        private void trkFrameSlider_Scroll(object sender, EventArgs e)
        {
            // 슬라이더를 움직일 때마다 현재 슬라이더 위치(Value)의 데이터를 화면에 뿌림
            UpdateFrameDisplay(trkFrameSlider.Value);
        }

        // 기존 label1_Click이 lblThrottleBottom으로 변경됨에 따라 자동 갱신용으로 남겨둔 이벤트
        private void lblThrottleBottom_Click(object sender, EventArgs e)
        {

        }

        // [진철이용] 외부에서 파싱된 데이터를 받아서 화면에 세팅해주는 통로
        public void SetFrameDataList(List<FrameData> parsedData)
        {
            this.frameList = parsedData; // 진철이가 보낸 데이터를 받음
            this.trkFrameSlider.Maximum = frameList.Count - 1;

            // 리스트 박스 초기화 (형규가 맡은 리스트 기능의 기초 작업)
            lstFrameData.Items.Clear();
            foreach (var item in frameList)
            {
                lstFrameData.Items.Add($"Frame {item.Index} | {item.Angle:F2}");
            }

            UpdateFrameDisplay(0); // 첫 번째 프레임 보여주기
        }
    }

    // 📦 데이터 모델 클래스 (Form1 클래스 아래에 두어 디자이너 고장 방지)
    public class FrameData
    {
        public int Index { get; set; }
        public string ImagePath { get; set; } = string.Empty; // Null 경고 방지 기본값 세팅
        public float Angle { get; set; }
        public float Throttle { get; set; }
        public string Timestamp { get; set; } = string.Empty; // Null 경고 방지 기본값 세팅
    }


}