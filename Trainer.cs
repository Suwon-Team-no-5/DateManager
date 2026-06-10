using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace DateManager
{
    public class Trainer
    {
        private Process pythonProcess;
        public event Action<string> LogReceived;
        public event Action TrainingFinished;

        // Trainer 클래스에서 학습을 시작하는 메인 함수
        public void StartTraining(string pythonPath, string workingDir, string tubPath)
        {
            if (pythonProcess != null && !pythonProcess.HasExited) return;

            try
            {
                ProcessStartInfo psi = new ProcessStartInfo();
                string exeName = Path.GetFileName(pythonPath ?? "").ToLower();

                if (exeName.Contains("wsl"))
                {
                    psi.FileName = pythonPath;

                    // 경로를 리눅스 절대 경로로 완벽하게 변환 (내부 로직 사용)
                    string safeWorkingDir = ToLinuxPath(workingDir);
                    string safeTubPath = ToLinuxPath(tubPath);

                    string envPythonPath = "/home/jaeseo03/miniconda3/envs/e2e_env/bin/python";

                    // WSL 명령어 구성: 윈도우 경로 형식을 완전히 제거하고 리눅스 내부 경로만 사용
                    // bash 내에서 따옴표 처리를 위해 단일 따옴표(') 사용
                    psi.Arguments = $"-d Ubuntu-22.04 -e bash -c \"cd '{safeWorkingDir}' && export PYTHONUNBUFFERED=1 && {envPythonPath} train.py --tubs '{safeTubPath}' --model './models/mypilot.h5'\"";
                }
                else
                {
                    psi.FileName = pythonPath;
                    psi.WorkingDirectory = workingDir;
                    string scriptPath = Path.Combine(workingDir ?? string.Empty, "train.py");
                    psi.Arguments = $"\"{scriptPath}\" --tub=\"{tubPath}\" --model=./models/mypilot.h5";
                }

                psi.UseShellExecute = false;
                psi.CreateNoWindow = true;
                psi.RedirectStandardOutput = true;
                psi.RedirectStandardError = true;
                psi.StandardOutputEncoding = Encoding.UTF8;
                psi.StandardErrorEncoding = Encoding.UTF8;

                pythonProcess = new Process();
                pythonProcess.StartInfo = psi;
                pythonProcess.EnableRaisingEvents = true;

                pythonProcess.OutputDataReceived += (s, args) => {
                    if (!string.IsNullOrEmpty(args.Data))
                    {
                        string cleanData = args.Data.Replace("\b", "").Replace("\r", "");
                        if (!string.IsNullOrWhiteSpace(cleanData))
                            LogReceived?.Invoke(cleanData + "\r\n");
                    }
                };
                pythonProcess.ErrorDataReceived += (s, args) => {
                    if (!string.IsNullOrEmpty(args.Data))
                    {
                        string data = args.Data;
                        // 단순 경고는 로그로만 출력, 진짜 에러만 [ERROR] 표시
                        if (data.Contains("Could not find") || data.Contains("WARNING") || data.Contains("tensorflow"))
                            LogReceived?.Invoke(data + "\r\n");
                        else
                            LogReceived?.Invoke($"[ERROR] {data}\r\n");
                    }
                };
                pythonProcess.Exited += (s, args) => {
                    LogReceived?.Invoke("✅ 동키카 AI 학습 완료!\r\n");
                    TrainingFinished?.Invoke();
                    pythonProcess?.Dispose();
                    pythonProcess = null;
                };

                pythonProcess.Start();
                pythonProcess.BeginOutputReadLine();
                pythonProcess.BeginErrorReadLine();
            }
            catch (Exception ex)
            {
                LogReceived?.Invoke($"❌ 실행 오류 발생: {ex.Message}\r\n");
            }
        }

        public void KillProcess()
        {
            if (pythonProcess != null && !pythonProcess.HasExited)
            {
                try { pythonProcess.Kill(); pythonProcess.Dispose(); pythonProcess = null; } catch { }
            }
        }

        // 윈도우 경로를 리눅스 내부 절대 경로로 강제 변환하는 메서드
        private string ToLinuxPath(string inputPath)
        {
            if (string.IsNullOrEmpty(inputPath)) return "/home/jaeseo03/mycar";

            string path = inputPath.Replace("\\", "/");

            // 1. WSL 네트워크 경로 패턴 제거 (중요!)
            // "//wsl.localhost/Ubuntu-22.04" 형태를 찾아서 루트(/)로 변경
            if (path.Contains("wsl.localhost"))
            {
                int idx = path.IndexOf("Ubuntu-22.04");
                if (idx != -1)
                {
                    path = path.Substring(idx + 12); // "Ubuntu-22.04" 뒤의 경로 추출
                }
            }

            // 2. 만약 C:/ 와 같은 드라이브 문자열이 있다면 /mnt/c/ 로 변환
            if (path.Length > 2 && path[1] == ':')
            {
                string drive = path.Substring(0, 1).ToLower();
                path = "/mnt/" + drive + path.Substring(2);
            }

            // 3. 만약 리눅스 절대 경로(/)로 시작하지 않으면 강제로 홈 디렉토리 붙임
            if (!path.StartsWith("/"))
            {
                path = "/home/jaeseo03/" + path.TrimStart('/');
            }

            return path;
        }
    }
}