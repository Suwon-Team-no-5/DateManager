using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace DateManager // 프로젝트 네임스페이스에 맞게 수정
{
    public class Trainer
    {
        private Process pythonProcess;
        private string condaPath = "/home/jaeseo03/miniconda3/bin/conda"; // WSL2 conda 경로
        private string condaEnvName = "e2e_env"; // conda 환경 이름

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

                    // WSL2에서 conda 환경을 확실하게 활성화하는 방법
                    // 특정 배포판 지정 (-d Ubuntu-22.04)
                    string safeWorkingDir = workingDir?.Replace("\"", "\\\"") ?? string.Empty;
                    psi.Arguments = $"-d Ubuntu-22.04 -e bash -lic \"export PYTHONUNBUFFERED=1; " +
                        $"cd '{safeWorkingDir}' && {condaPath} run -n {condaEnvName} python train.py --tub=./data/ --model=./models/mypilot.h5\"";

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
                    if (!string.IsNullOrEmpty(args.Data))
                    {
                        string data = args.Data;

                        // 1. 특수 제어 문자(백스페이스 등)가 포함되어 있다면 청소
                        if (data.Contains("\b") || data.Contains("\r"))
                        {
                            data = data.Replace("\b", "").Replace("\r", "");
                        }

                        // 2. 텐서플로우 특유의 지저분한 로딩바(>>>>, ====>)가 포함된 줄은 화면 낭비를 막기 위해 패스!
                        if (data.Contains("==========") || data.Contains(">....") || data.Contains("64/64"))
                        {
                            // 단, 실시간 수치가 포함되어 있다면 로딩바 분수(5/64 등)만 깔끔하게 정돈
                            if (data.Contains("loss:"))
                            {
                                // 로딩바 기호(====>....) 부분을 공백으로 변환해서 수치만 남김
                                data = System.Text.RegularExpressions.Regex.Replace(data, @"[=>\s\.]{5,}", " ");
                            }
                            else
                            {
                                return; // 완전히 지저분한 줄은 출력하지 않고 무시
                            }
                        }

                        // 3. 최종 정돈된 예쁜 데이터만 UI로 전송!
                        LogReceived?.Invoke(data.Trim() + "\r\n");
                    }
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