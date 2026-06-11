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

        // 🌟 [수정됨] 학습 파일 이름을 받아올 수 있도록 modelFileName 파라미터 추가
        public void StartTraining(string pythonPath, string workingDir, string tubPath, string modelFileName = "mypilot.h5")
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

                    // 🌟 [수정됨] 하드코딩된 mypilot.h5 대신 입력받은 modelFileName을 사용하도록 변경
                    psi.Arguments = $"-d Ubuntu-22.04 -e bash -c \"cd '{safeWorkingDir}' && export PYTHONUNBUFFERED=1 && {envPythonPath} train.py --tubs '{safeTubPath}' --model './models/{modelFileName}'\"";
                }
                else
                {
                    psi.FileName = pythonPath;
                    psi.WorkingDirectory = workingDir;
                    string scriptPath = Path.Combine(workingDir ?? string.Empty, "train.py");
                    // 🌟 [수정됨] 윈도우 환경에서도 입력받은 modelFileName 사용
                    psi.Arguments = $"\"{scriptPath}\" --tub=\"{tubPath}\" --model=./models/{modelFileName}";
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
                        if (data.Contains("Could not find") || data.Contains("WARNING") || data.Contains("tensorflow"))
                            LogReceived?.Invoke(data + "\r\n");
                        else
                            LogReceived?.Invoke($"[ERROR] {data}\r\n");
                    }
                };
                pythonProcess.Exited += (s, args) => {
                    // 🌟 [수정됨] 완료 로그에도 사용자가 지정한 파일 이름이 출력되도록 변경
                    LogReceived?.Invoke($"✅ 동키카 AI 학습 완료!\r\n모델 위치: /home/jaeseo03/mycar/models\r\n학습 파일 이름: {modelFileName}\r\n");
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

        private string ToLinuxPath(string inputPath)
        {
            if (string.IsNullOrEmpty(inputPath)) return "/home/jaeseo03/mycar";

            string path = inputPath.Replace("\\", "/");

            if (path.Contains("wsl.localhost"))
            {
                int idx = path.IndexOf("Ubuntu-22.04");
                if (idx != -1)
                {
                    path = path.Substring(idx + 12);
                }
            }

            if (path.Length > 2 && path[1] == ':')
            {
                string drive = path.Substring(0, 1).ToLower();
                path = "/mnt/" + drive + path.Substring(2);
            }

            if (!path.StartsWith("/"))
            {
                path = "/home/jaeseo03/" + path.TrimStart('/');
            }

            return path;
        }
    }
}