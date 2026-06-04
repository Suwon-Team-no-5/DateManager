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

        // 필수 인수 3개를 받도록 정확히 선언
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
                    string safeWorkingDir = workingDir?.Trim().Replace("\"", "").Replace("'", "") ?? string.Empty;

                    if (safeWorkingDir.StartsWith("~/")) safeWorkingDir = safeWorkingDir.Replace("~/", "/home/jaeseo03/");
                    else if (!safeWorkingDir.StartsWith("/")) safeWorkingDir = "/home/jaeseo03/" + safeWorkingDir;

                    string envPythonPath = "/home/jaeseo03/miniconda3/envs/e2e_env/bin/python";
                    // 3개 인수를 사용
                    psi.Arguments = $"-d Ubuntu-22.04 -e bash -c \"cd '{safeWorkingDir}' && export PYTHONUNBUFFERED=1 && {envPythonPath} train.py --tub={tubPath} --model=./models/mypilot.h5\"";
                }
                else
                {
                    psi.FileName = pythonPath;
                    psi.WorkingDirectory = workingDir;
                    string scriptPath = Path.Combine(workingDir ?? string.Empty, "train.py");
                    psi.Arguments = $"\"{scriptPath}\" --tub={tubPath} --model=./models/mypilot.h5";
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
                        // 백스페이스 문자('\b')와 캐리지 리턴('\r')을 제거합니다.
                        string cleanData = args.Data.Replace("\b", "").Replace("\r", "");
                        if (!string.IsNullOrWhiteSpace(cleanData))
                            LogReceived?.Invoke(cleanData + "\r\n");
                    }
                };
                pythonProcess.ErrorDataReceived += (s, args) => {
                    if (!string.IsNullOrEmpty(args.Data))
                    {
                        string data = args.Data;

                        // 텐서플로우의 단순 알림(CUDA 없음, 라이브러리 로딩 등)은 에러로 치지 않음
                        if (data.Contains("Could not find") || data.Contains("WARNING") || data.Contains("tensorflow"))
                        {
                            LogReceived?.Invoke(data + "\r\n"); // [ERROR] 딱지 없이 그냥 출력
                        }
                        else
                        {
                            LogReceived?.Invoke($"[ERROR] {data}\r\n"); // 진짜 에러만 딱지 붙임
                        }
                    }
                };
                pythonProcess.Exited += (s, args) => { LogReceived?.Invoke("✅ 동키카 AI 학습 완료!\r\n"); TrainingFinished?.Invoke(); pythonProcess?.Dispose(); pythonProcess = null; };

                pythonProcess.Start();
                pythonProcess.BeginOutputReadLine();
                pythonProcess.BeginErrorReadLine();
            }
            catch (Exception) // ex 변수를 사용하지 않으면 그냥 Exception으로 써도 됩니다.
            {
                LogReceived?.Invoke($"❌ 실행 오류 발생\r\n");
            }
        }

        public void KillProcess()
        {
            if (pythonProcess != null && !pythonProcess.HasExited)
            {
                try { pythonProcess.Kill(); pythonProcess.Dispose(); pythonProcess = null; } catch { }
            }
        }
    }
}