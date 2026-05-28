using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace DateManager // 프로젝트 네임스페이스에 맞게 수정
{
    public class Trainer
    {
        private Process pythonProcess;

        public event Action<string> LogReceived;
        public event Action TrainingFinished;

        /// <summary>
        /// WSL2 환경의 가상환경을 켜고, 지정된 mycar 폴더로 이동하여 파이썬 학습 스크립트를 백그라운드에서 실행하는 함수입니다.
        /// </summary>
        public void StartTraining(string pythonPath, string workingDir)
        {
            if (pythonProcess != null && !pythonProcess.HasExited) return;

            try
            {
                ProcessStartInfo psi = new ProcessStartInfo();
                // 파라미터로 들어온 pythonPath가 wsl.exe인지 확인
                string exeName = Path.GetFileName(pythonPath ?? "").ToLower();

                if (exeName.Contains("wsl"))
                {
                    psi.FileName = pythonPath; // wsl.exe

                    // WSL 비인터랙티브 셸에서 conda를 사용하려면 shell hook 또는 bashrc를 명시적으로 불러와야 함
                    string safeWorkingDir = workingDir?.Replace("\"", "\\\"") ?? string.Empty;
                    psi.Arguments = $"-e bash -lic \"export PYTHONUNBUFFERED=1; source ~/.bashrc 2>/dev/null || true; eval \\\"$(conda shell.bash hook)\\\" 2>/dev/null || true; conda activate e2e_env && cd '{safeWorkingDir}' && python train.py --tub=./data/ --model=./models/mypilot.h5\"";

                    // 로그로 실행 명령 확인(디버깅용)
                    LogReceived?.Invoke($"[CMD] {psi.FileName} {psi.Arguments}\r\n");
                }
                else
                {
                    // 윈도우 로컬 Python 또는 절대 경로로 직접 실행하는 경우
                    psi.FileName = pythonPath; // ex: "python" 또는 "C:\\Python\\python.exe"
                    psi.WorkingDirectory = workingDir;
                    // train.py 경로를 인수로 전달
                    string scriptPath = Path.Combine(workingDir ?? string.Empty, "train.py");
                    psi.Arguments = $"\"{scriptPath}\" --tub=./data/ --model=./models/mypilot.h5";

                    LogReceived?.Invoke($"[CMD] {psi.FileName} {psi.Arguments} (cwd={psi.WorkingDirectory})\r\n");
                }

                psi.UseShellExecute = false;
                psi.CreateNoWindow = true;
                psi.RedirectStandardOutput = true;
                psi.RedirectStandardError = true;
                psi.StandardOutputEncoding = Encoding.UTF8; // 한글 로그 깨짐 방지
                psi.StandardErrorEncoding = Encoding.UTF8;

                pythonProcess = new Process();
                pythonProcess.StartInfo = psi;
                pythonProcess.EnableRaisingEvents = true;

                pythonProcess.OutputDataReceived += (s, args) => {
                    if (!string.IsNullOrEmpty(args.Data)) LogReceived?.Invoke(args.Data + "\r\n");
                };
                pythonProcess.ErrorDataReceived += (s, args) => {
                    if (!string.IsNullOrEmpty(args.Data)) LogReceived?.Invoke("[ERROR] " + args.Data + "\r\n");
                };

                pythonProcess.Exited += (s, args) => {
                    LogReceived?.Invoke("✅ 동키카 AI 학습 완료!\r\n");
                    TrainingFinished?.Invoke();
                    pythonProcess.Dispose();
                    pythonProcess = null;
                };

                pythonProcess.Start();
                pythonProcess.BeginOutputReadLine();
                pythonProcess.BeginErrorReadLine();
            }
            catch (Exception ex)
            {
                LogReceived?.Invoke($"❌ 실행 오류: {ex.Message}\r\n");
                TrainingFinished?.Invoke();
            }
        }

        /// <summary>
        /// 프로그램 종료 또는 정지 요청 시 백그라운드에 남아있는 파이썬 프로세스를 안전하게 강제 종료하는 안전장치 
        /// </summary>
        public void KillProcess()
        {
            if (pythonProcess != null && !pythonProcess.HasExited)
            {
                try
                {
                    pythonProcess.Kill();
                    pythonProcess.Dispose();
                    pythonProcess = null;
                }
                catch { }
            }
        }
    }
}