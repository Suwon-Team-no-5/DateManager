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
                psi.FileName = pythonPath; // "wsl.exe"가 들어옴

                // -u 옵션 대신 파이썬 환경변수(PYTHONUNBUFFERED=1)를 쉘에 주어 실시간 로그를 강제 보장합니다.
                psi.Arguments = $"-e bash -lic \"export PYTHONUNBUFFERED=1 && conda activate e2e_env && cd {workingDir} && python train.py --tub=./data/ --model=./models/mypilot.h5\"";

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