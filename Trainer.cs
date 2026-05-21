using System;
using System.Diagnostics;
using System.IO;

namespace DateManager
{
    public class Trainer
    {
        private Process pythonProcess;

        // Form1 쪽으로 실시간 로그와 종료 신호를 보내주기 위한 전달자(이벤트) 설정
        public event Action<string> LogReceived;
        public event Action TrainingFinished;

        // AI 학습을 시작하는 핵심 메서드

        public void StartTraining(string pythonPath, string workingDir)
        {
            if (pythonProcess != null && !pythonProcess.HasExited) return;

            try
            {
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = pythonPath; // "wsl.exe" 가 들어옴

                // -u 옵션을 줘서 실시간 로그 출력을 보장함
                psi.Arguments = $"/home/jaeseo03/miniconda3/envs/e2e_env/bin/python -u train.py --tubs=./data/ --model=./models/mypilot.h5";

                // WSL2 상의 mycar 폴더 경로를 작업 디렉토리로 설정
                psi.WorkingDirectory = @"\\wsl$\Ubuntu-22.04" + workingDir.Replace("/", "\\");

                psi.UseShellExecute = false;
                psi.CreateNoWindow = true;
                psi.RedirectStandardOutput = true;
                psi.RedirectStandardError = true;

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
                    LogReceived?.Invoke("✅ W동키카 AI 학습 완료!\r\n");
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

        // 프로그램 강제 종료 시 좀비 프로세스 방지용 메서드
        public void KillProcess()
        {
            if (pythonProcess != null && !pythonProcess.HasExited)
            {
                try { pythonProcess.Kill(); } catch { }
            }
        }
    }
}