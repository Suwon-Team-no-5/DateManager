using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace DateManager
{
    public partial class LauncherForm : Form
    {
        private const string WslDistro = "Ubuntu-22.04";
        private const int DrivePort = 8887;

        public LauncherForm()
        {
            InitializeComponent();
        }

        private void BtnCollect_Click(object sender, EventArgs e)
        {
            try
            {
                CleanPort8887();

                StartWslDrive("");

                Process.Start(new ProcessStartInfo
                {
                    FileName = $"http://localhost:{DrivePort}",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"수집 모드 구동 실패: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAutoDrive_Click(object sender, EventArgs e)
        {
            string modelsRoot = GetWindowsModelsRoot();

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "자율주행에 사용할 모델 파일(.h5)을 선택하세요";
                ofd.Filter = "DonkeyCar Model (*.h5)|*.h5";

                if (!string.IsNullOrWhiteSpace(modelsRoot) && Directory.Exists(modelsRoot))
                {
                    ofd.InitialDirectory = modelsRoot;
                }

                if (ofd.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                string selectedFileName = Path.GetFileName(ofd.FileName);

                try
                {
                    CleanPort8887();

                    StartWslDrive($"--model=./models/{EscapeBashArg(selectedFileName)} --type=linear");

                    Process.Start(new ProcessStartInfo
                    {
                        FileName = $"http://localhost:{DrivePort}",
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"자율주행 구동 실패: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnOpenMainUi_Click(object sender, EventArgs e)
        {
            Form1 mainForm = new Form1();

            mainForm.StartPosition = FormStartPosition.Manual;

            if (this.WindowState == FormWindowState.Maximized)
            {
                mainForm.Location = this.Location;
                mainForm.WindowState = FormWindowState.Maximized;
            }
            else
            {
                mainForm.WindowState = FormWindowState.Normal;
                mainForm.Bounds = this.Bounds;
            }

            mainForm.FormClosed += (s, args) =>
            {
                if (!this.Visible)
                {
                    this.Close();
                }
            };

            mainForm.Show();
            this.Hide();
        }

        private void CleanPort8887()
        {
            // 1. 기존 fuser 명령어로 8887 포트를 점유 중인 프로세스 종료 (여유 시간 1.5초)
            RunWslHidden($"fuser -k {DrivePort}/tcp || true", waitMilliseconds: 1500);

            // 2. 포트는 닫혔지만 카메라 등을 물고 있는 좀비 manage.py 프로세스가 있다면 일괄 강제 종료 (여유 시간 1초)
            RunWslHidden("pkill -f 'manage.py drive' || true", waitMilliseconds: 1000);
        }

        private void StartWslDrive(string extraArgs)
        {
            // 🚨 핵심: set -e; 를 제거하여 에러가 나도 스크립트가 끝까지 실행되게(exec bash) 만듭니다.
            string command =
                "MYCAR_DIR=\"${DONKEYCAR_MY_CAR_DIR:-$HOME/mycar}\"; " +
                "if [ ! -f \"$MYCAR_DIR/manage.py\" ]; then " +
                "echo \"mycar 폴더를 찾을 수 없습니다. WSL에서 DONKEYCAR_MY_CAR_DIR 환경변수를 설정하거나 $HOME/mycar에 프로젝트를 두세요.\"; " +
                "exec bash; " +
                "fi; " +
                "PYTHON_BIN=\"${DONKEYCAR_PYTHON:-}\"; " +
                "if [ -z \"$PYTHON_BIN\" ]; then " +
                "if command -v conda >/dev/null 2>&1; then " +
                "PYTHON_BIN=\"python\"; " +
                "elif [ -x \"$HOME/miniconda3/envs/e2e_env/bin/python\" ]; then " +
                "PYTHON_BIN=\"$HOME/miniconda3/envs/e2e_env/bin/python\"; " +
                "elif [ -x \"$HOME/anaconda3/envs/e2e_env/bin/python\" ]; then " +
                "PYTHON_BIN=\"$HOME/anaconda3/envs/e2e_env/bin/python\"; " +
                "else " +
                "PYTHON_BIN=\"python3\"; " +
                "fi; " +
                "fi; " +
                "cd \"$MYCAR_DIR\"; " +
                "export PYTHONUNBUFFERED=1; " +
                $"\"$PYTHON_BIN\" manage.py drive {extraArgs}; " +
                "echo \"\"; " +
                "echo \"============================================================\"; " +
                "echo \"[알림] 프로세스가 종료되었습니다. 위 에러 메시지를 확인하세요.\"; " +
                "echo \"창을 닫으려면 exit를 입력하거나 우측 상단 X를 누르세요.\"; " +
                "echo \"============================================================\"; " +
                "exec bash"; // 에러가 나든 정상 종료되든 무조건 bash 쉘을 열어 창을 유지함

            ProcessStartInfo wslInfo = new ProcessStartInfo
            {
                // cmd.exe를 거쳐서 실행하면 wsl.exe 자체가 튕기는 현상도 방어할 수 있습니다.
                FileName = "cmd.exe",
                Arguments = $"/c wsl.exe -d {WslDistro} -e bash -lc \"{EscapeForWslDoubleQuotes(command)}\"",
                CreateNoWindow = false,
                UseShellExecute = true
            };

            Process.Start(wslInfo);
        }

        private void RunWslHidden(string command, int waitMilliseconds)
        {
            ProcessStartInfo info = new ProcessStartInfo
            {
                FileName = "wsl.exe",
                Arguments = $"-d {WslDistro} -e bash -lc \"{EscapeForWslDoubleQuotes(command)}\"",
                CreateNoWindow = true,
                UseShellExecute = false
            };

            using (Process proc = Process.Start(info))
            {
                proc?.WaitForExit(waitMilliseconds);
            }
        }

        private string GetWindowsModelsRoot()
        {
            string linuxHome = RunWslCapture("printf '%s' \"$HOME\"");
            if (string.IsNullOrWhiteSpace(linuxHome))
            {
                return null;
            }

            string userName = linuxHome.Trim().Replace("/home/", "");
            return $@"\\wsl.localhost\{WslDistro}\home\{userName}\mycar\models";
        }

        private string RunWslCapture(string command)
        {
            ProcessStartInfo info = new ProcessStartInfo
            {
                FileName = "wsl.exe",
                Arguments = $"-d {WslDistro} -e bash -lc \"{EscapeForWslDoubleQuotes(command)}\"",
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                StandardOutputEncoding = Encoding.UTF8
            };

            using (Process proc = Process.Start(info))
            {
                string output = proc.StandardOutput.ReadToEnd();
                proc.WaitForExit(3000);
                return output;
            }
        }

        private static string EscapeForWslDoubleQuotes(string value)
        {
            return value.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }

        private static string EscapeBashArg(string value)
        {
            return value.Replace("'", "'\"'\"'");
        }

        private void btnTitle_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. 실행 파일이 있는 폴더에 HTML 파일 경로 지정
                string filePath = System.IO.Path.Combine(Application.StartupPath, "Team5_Development_Plan.html");

                // 2. 어떤 브라우저에서도 100% 작동하는 안전한 토글 로직 적용
                string htmlContent = @"
        <!DOCTYPE html>
        <html lang=""ko"">
        <head>
            <meta charset=""UTF-8"">
            <title>5팀 개발 계획서</title>
            <style>
                /* 기본 테마: 다크 모드 (아무 설정이 없을 때 기본으로 적용) */
                :root {
                    --bg-color: #121212;
                    --container-bg: #1e1e1e;
                    --text-color: #e0e0e0;
                    --title-color: #ffffff;
                    --accent-color: #4da6ff;
                    --card-bg: #2d2d2d;
                    --card-hover: #383838;
                    --role-color: #b0b0b0;
                    --footer-color: #666666;
                }

                /* 라이트 테마: body에 light-mode 클래스가 추가되었을 때 덮어씌워짐 */
                body.light-mode {
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

                body { 
                    font-family: 'Pretendard', 'Malgun Gothic', sans-serif; 
                    background-color: var(--bg-color); 
                    padding: 40px; 
                    color: var(--text-color); 
                    line-height: 1.6;
                    transition: background-color 0.3s ease, color 0.3s ease; 
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
                    transform: scale(1.15); 
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
            <div class=""container"">
                <button class=""theme-toggle"" id=""themeBtn"" onclick=""toggleTheme()"" title=""다크/라이트 모드 전환"">☀️</button>
                
                <h1>🚀 5팀 Donkeycar Data Manager</h1>
                <h3>프로그래밍언어 및 실습 5팀 역할 분담 계획서</h3>
                
                <div class=""member-card"">
                    <div class=""name"">👑 김재서 (팀장)</div>
                    <div class=""role"">▪ 23010114 컴퓨터SW학과<br>▪ 전체 WinForms 레이아웃 설계<br>▪ 미션 2/3 (이미지 표시 및 슬라이더) 연동 담당</div>
                </div>
                
                <div class=""member-card"">
                    <div class=""name"">💻 박진철</div>
                    <div class=""role"">▪ 23017037 컴퓨터SW학과<br>▪ 미션 1 (JSON 파싱)<br>▪ 미션 6 (파일 시스템 삭제 로직) 담당</div>
                </div>
                
                <div class=""member-card"">
                    <div class=""name"">⚙️ 윤형규</div>
                    <div class=""role"">▪ 23017056 컴퓨터SW학과<br>▪ 미션 4/5 (리스트 선택 및 LINQ 기반 데이터 필터링) 알고리즘 구현</div>
                </div>
                
                <div class=""member-card"">
                    <div class=""name"">🔗 이기주</div>
                    <div class=""role"">▪ 23017057 컴퓨터SW학과<br>▪ 미션 7 (Python 프로세스 연동)<br>▪ 학습 로그 비동기 출력 구현</div>
                </div>

                <div class=""footer"">
                    © 2026 프로그래밍언어 및 실습 5팀 | Final Exam Project
                </div>
            </div>

            <script>
                function toggleTheme() {
                    // body 태그에 'light-mode' 클래스를 껐다 켰다 합니다.
                    document.body.classList.toggle('light-mode');
                    
                    const btn = document.getElementById('themeBtn');
                    
                    // 현재 라이트 모드 상태라면 달(🌙) 아이콘으로 변경
                    if (document.body.classList.contains('light-mode')) {
                        btn.textContent = '🌙'; 
                    } else {
                        // 다크 모드 상태라면 해(☀️) 아이콘으로 변경
                        btn.textContent = '☀️'; 
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

        private void btnManual_Click(object sender, EventArgs e)
        {
            pnlManual.Visible = true;
        }

        private void btnCloseManual_Click(object sender, EventArgs e)
        {
            pnlManual.Visible = false;
        }
    }
}