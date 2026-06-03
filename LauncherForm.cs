using System;
using System.IO;
using System.Windows.Forms;

namespace DateManager
{
    public partial class LauncherForm : Form
    {
        // 기존에 사용하던 리눅스 모델 폴더 기본 경로
        private string _wslModelsRoot = @"\\wsl.localhost\Ubuntu-22.04\home\jaeseo03\mycar\models\";

        public LauncherForm()
        {
            InitializeComponent();
        }

        // ---------------------------------------------------------------------
        // [기능 1] 사진 모으기 (모델 없이 drive 서버를 수동 모드로 구동)
        // ---------------------------------------------------------------------
        private void BtnCollect_Click(object sender, EventArgs e)
        {
            try
            {
                CleanPort8887();

                System.Diagnostics.ProcessStartInfo wslInfo = new System.Diagnostics.ProcessStartInfo();
                wslInfo.FileName = "wsl.exe";
                wslInfo.Arguments = "-d Ubuntu-22.04 -e bash -c \"cd '/home/jaeseo03/mycar' && export PYTHONUNBUFFERED=1 && /home/jaeseo03/miniconda3/envs/e2e_env/bin/python manage.py drive\"";
                wslInfo.CreateNoWindow = false;
                wslInfo.UseShellExecute = true;
                System.Diagnostics.Process.Start(wslInfo);

                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo { FileName = "http://localhost:8887", UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"수집 모드 구동 실패: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ---------------------------------------------------------------------
        // [기능 2] 자율 주행 시키기 (원하는 학습 파일 선택 후 구동)
        // ---------------------------------------------------------------------
        private void BtnAutoDrive_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "자율주행에 사용할 모델 파일(.h5)을 선택하세요";
                ofd.Filter = "DonkeyCar Model (*.h5)|*.h5";

                if (Directory.Exists(_wslModelsRoot))
                {
                    ofd.InitialDirectory = _wslModelsRoot;
                }

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string selectedFileName = Path.GetFileName(ofd.FileName);

                    try
                    {
                        CleanPort8887();

                        System.Diagnostics.ProcessStartInfo wslInfo = new System.Diagnostics.ProcessStartInfo();
                        wslInfo.FileName = "wsl.exe";
                        wslInfo.Arguments = $"-d Ubuntu-22.04 -e bash -c \"cd '/home/jaeseo03/mycar' && export PYTHONUNBUFFERED=1 && /home/jaeseo03/miniconda3/envs/e2e_env/bin/python manage.py drive --model=./models/{selectedFileName} --type=linear\"";
                        wslInfo.CreateNoWindow = false;
                        wslInfo.UseShellExecute = true;
                        System.Diagnostics.Process.Start(wslInfo);

                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo { FileName = "http://localhost:8887", UseShellExecute = true });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"자율주행 구동 실패: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // ---------------------------------------------------------------------
        // [기능 3] 기존 돈키카 UI 실행시키기 (Form1 열기)
        // ---------------------------------------------------------------------
        private void BtnOpenMainUi_Click(object sender, EventArgs e)
        {
            Form1 mainForm = new Form1();

            // 메인 폼이 동일한 위치에 열리도록 설정하여 자연스러운 전환 유도
            mainForm.StartPosition = FormStartPosition.Manual;
            mainForm.Location = this.Location;

            mainForm.FormClosed += (s, args) =>
            {
                // 💡 런처가 이미 화면에 표시(Show)된 상태라면 런처를 닫지 않고, 
                // 사용자가 Form1의 'X' 버튼을 눌러 아예 껐을 때만 프로그램 전체를 종료합니다.
                if (!this.Visible)
                {
                    this.Close();
                }
            };

            mainForm.Show();
            this.Hide(); // 런처 숨기기
        }

        // 포트 청소용 공통 헬퍼 함수
        private void CleanPort8887()
        {
            System.Diagnostics.ProcessStartInfo killInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "wsl.exe",
                Arguments = "-d Ubuntu-22.04 -e bash -c \"fuser -k 8887/tcp || true\"",
                CreateNoWindow = true,
                UseShellExecute = false
            };
            using (var killProc = System.Diagnostics.Process.Start(killInfo))
            {
                killProc.WaitForExit(1500);
            }
        }

        private void btnTitle_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. 실행 파일이 있는 폴더에 HTML 파일 생성 경로 지정
                string filePath = System.IO.Path.Combine(Application.StartupPath, "Team5_Development_Plan.html");

                // 2. 다크모드 토글 기능과 모던 CSS가 적용된 HTML 텍스트 작성
                string htmlContent = @"
        <!DOCTYPE html>
        <html lang='ko'>
        <head>
            <meta charset='UTF-8'>
            <title>5팀 개발 계획서</title>
            <style>
                /* 기본(라이트) 테마 색상 변수 */
                :root {
                    --bg-color: #f7f9fc;
                    --container-bg: #ffffff;
                    --text-color: #333333;
                    --title-color: #1a1a1a;
                    --accent-color: #0078D7;
                    --card-bg: #f8f9fa;
                    --card-hover: #f1f3f5;
                    --role-color: #444444;
                    --footer-color: #aaaaaa;
                }

                /* 다크 테마 색상 변수 */
                [data-theme='dark'] {
                    --bg-color: #121212;
                    --container-bg: #1e1e1e;
                    --text-color: #e0e0e0;
                    --title-color: #ffffff;
                    --accent-color: #4da6ff; /* 다크모드에서는 파란색을 살짝 더 밝게 */
                    --card-bg: #2d2d2d;
                    --card-hover: #383838;
                    --role-color: #b0b0b0;
                    --footer-color: #666666;
                }

                body { 
                    font-family: 'Pretendard', 'Malgun Gothic', sans-serif; 
                    background-color: var(--bg-color); 
                    padding: 40px; 
                    color: var(--text-color); 
                    line-height: 1.6;
                    transition: background-color 0.3s ease, color 0.3s ease; /* 부드러운 전환 효과 */
                }
                .container { 
                    max-width: 800px; 
                    margin: 0 auto; 
                    background: var(--container-bg); 
                    padding: 50px; 
                    border-radius: 16px; 
                    box-shadow: 0 10px 30px rgba(0,0,0,0.1); 
                    position: relative;
                    transition: background-color 0.3s ease;
                }
                .theme-toggle {
                    position: absolute;
                    top: 40px;
                    right: 40px;
                    background: none;
                    border: none;
                    font-size: 1.8em;
                    cursor: pointer;
                    transition: transform 0.2s;
                }
                .theme-toggle:hover {
                    transform: scale(1.15); /* 마우스 올리면 버튼이 살짝 커짐 */
                }
                h1 { 
                    color: var(--title-color); 
                    border-bottom: 3px solid var(--accent-color); 
                    padding-bottom: 15px; 
                    margin-bottom: 10px;
                    margin-top: 0;
                    transition: color 0.3s ease, border-color 0.3s ease;
                }
                h3 {
                    color: var(--role-color);
                    margin-top: 0;
                    margin-bottom: 40px;
                    font-weight: 500;
                    transition: color 0.3s ease;
                }
                .member-card { 
                    background: var(--card-bg); 
                    border-left: 5px solid var(--accent-color); 
                    padding: 20px; 
                    margin-bottom: 20px; 
                    border-radius: 0 8px 8px 0; 
                    transition: transform 0.2s ease, background-color 0.3s ease, border-color 0.3s ease;
                }
                .member-card:hover {
                    transform: translateX(5px);
                    background: var(--card-hover);
                }
                .name { 
                    font-size: 1.3em; 
                    font-weight: bold; 
                    color: var(--accent-color); 
                    margin-bottom: 8px; 
                    transition: color 0.3s ease;
                }
                .role { 
                    font-weight: 500; 
                    color: var(--role-color); 
                    font-size: 1.05em;
                    transition: color 0.3s ease;
                }
                .footer {
                    margin-top: 50px;
                    text-align: center;
                    color: var(--footer-color);
                    font-size: 0.9em;
                    transition: color 0.3s ease;
                }
            </style>
        </head>
        <body>
            <div class='container'>
                <button class='theme-toggle' id='themeBtn' onclick='toggleTheme()' title='다크/라이트 모드 전환'>🌙</button>
                
                <h1>🚀 MoveArt Donkeycar Data Manager</h1>
                <h3>프로그래밍언어 및 실습 5팀 역할 분담 계획서</h3>
                
                <div class='member-card'>
                    <div class='name'>👑 김재서 (팀장)</div>
                    <div class='role'>▪ 전체 WinForms 레이아웃 설계<br>▪ 미션 2/3 (이미지 표시 및 슬라이더) 연동 담당</div>
                </div>
                
                <div class='member-card'>
                    <div class='name'>💻 박진철</div>
                    <div class='role'>▪ 미션 1 (JSON 파싱)<br>▪ 미션 6 (파일 시스템 삭제 로직) 담당</div>
                </div>
                
                <div class='member-card'>
                    <div class='name'>⚙️ 윤형규</div>
                    <div class='role'>▪ 미션 4/5 (리스트 선택 및 LINQ 기반 데이터 필터링) 알고리즘 구현</div>
                </div>
                
                <div class='member-card'>
                    <div class='name'>🔗 이기주</div>
                    <div class='role'>▪ 미션 7 (Python 프로세스 연동)<br>▪ 학습 로그 비동기 출력 구현</div>
                </div>

                <div class='footer'>
                    © 2026 프로그래밍언어 및 실습 5팀 | Final Exam Project
                </div>
            </div>

            <script>
                function toggleTheme() {
                    const htmlDoc = document.documentElement;
                    const btn = document.getElementById('themeBtn');
                    
                    if (htmlDoc.getAttribute('data-theme') === 'dark') {
                        htmlDoc.removeAttribute('data-theme');
                        btn.textContent = '🌙'; // 라이트 모드일 땐 달 아이콘
                    } else {
                        htmlDoc.setAttribute('data-theme', 'dark');
                        btn.textContent = '☀️'; // 다크 모드일 땐 해 아이콘
                    }
                }
            </script>
        </body>
        </html>";

                // 3. 파일로 저장
                System.IO.File.WriteAllText(filePath, htmlContent);

                // 4. 저장된 파일을 기본 웹 브라우저로 즉시 실행
                System.Diagnostics.ProcessStartInfo browserInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true
                };
                System.Diagnostics.Process.Start(browserInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"문서를 여는 중 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}