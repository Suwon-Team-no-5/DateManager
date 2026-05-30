# 🚗 DonkeyCar Data Manager UI

<p align="center">
  <img src="https://img.shields.io/badge/Platform-Windows%20Forms-blue?style=flat-square&logo=windows" alt="Platform">
  <img src="https://img.shields.io/badge/Language-C%23%20%2F%20.NET-green?style=flat-square&logo=c-sharp" alt="Language">
  <img src="https://img.shields.io/badge/Team-Team%2005%20(Convergence)-orange?style=flat-square" alt="Team">
</p>

## 📝 프로젝트 개요 (Overview)
**DonkeyCar Data Manager UI**는 자율주행 RC카(DonkeyCar)의 주행 데이터(`Tub`/`Catalog`)셋을 시각적으로 확인하고, 효과적인 AI 모델 학습을 위해 데이터를 정제·관리하는 데스크톱 애플리케이션입니다.

운전자가 직접 주행하며 수집한 이미지, 조향각(Angle), 출력(Throttle) 데이터를 프레임 단위로 탐색하고 불필요한 구간을 필터링하여 최적의 자율주행 학습 데이터셋을 구축할 수 있도록 돕습니다.

---

## 📌 목차 (Table of Contents)
1. [주요 기능](#-주요-기능-key-features)
2. [기술 스택](#-기술-스택-tech-stack)
3. [디렉토리 구조](#-디렉토리-구조-project-structure)
4. [프로젝트 개발 계획](#-프로젝트-개발-계획)
5. [시작하기](#-시작하기-getting-started)
6. [팀 구성 및 역할](#-팀-구성-및-역할-team-members)
7. [진행 상황](#-진행-상황)

---

## ✨ 주요 기능 (Key Features)

### 📸 주행 데이터 로드 및 시각화
- `.catalog`, `.jsonl`, `.txt` 포맷의 주행 데이터 파일 로드 및 리스트업 기능.
- 프레임 선택 시 동기화된 메인 카메라 이미지(`pbMainCam`) 실시간 출력.
- **메모리 최적화:** 파일 스트림(`FileStream`) 방식을 적용하여 대량의 주행 이미지 탐색 시에도 메모리 누수(Memory Leak)를 완벽히 차단.

### 📊 직관적인 데이터 모니터링 & UX
- 화면 내 모든 주행 모니터링 라벨(프레임 인덱스, 조향각, 출력, 타임스탬프) 한글 표기.
- 스레틀(Throttle) 데이터를 단순 텍스트가 아닌 **프로그레스 바(ProgressBar)** 와 연동하여 0~100% 게이지로 직관적인 시각화.
- 미디어 플레이어형 UI 흐름을 반영하여 **재생(Play)/정지(Stop)** 버튼의 활성화/비활성화(`Enabled`) 자동 제어.

### 🔍 빠르고 강력한 데이터 필터링
방대한 주행 프레임 중 학습에 필요한 핵심 데이터셋만 추출할 수 있는 필터링 기능.
- `Thr > 0`: 차량이 실제로 앞으로 이동한 프레임만 보기.
- `Angle == 0`: 조향각이 없는 직진 주행 프레임만 보기.
- `Large Angle`: 급커브(조향각이 큰) 구간의 프레임만 보기.

---

## 🛠 기술 스택 (Tech Stack)

| Category | Technology |
| :--- | :--- |
| **Language** | <img src="https://img.shields.io/badge/C%23-239120?style=flat-square&logo=c-sharp&logoColor=white"> |
| **Framework** | <img src="https://img.shields.io/badge/.NET%20Windows%20Forms-5C2D91?style=flat-square&logo=.net&logoColor=white"> |
| **Data Processing** | JSON Lines 파싱 (`System.Text.Json`), LINQ Query |
| **Architecture** | Controller Pattern (UI 컴포넌트와 비즈니스 로직의 완전한 분리) |

---

## 📂 디렉토리 구조 (Project Structure)

```text
DateManager/
 ├── Form1.cs                  # 메인 윈도우 폼 (UI 디자인 및 컴포넌트 배치)
 ├── Form1.Designer.cs         # UI 컴포넌트 자동 생성 파일
 ├── Program.cs                # 애플리케이션 진입점 (Main)
 └── [추가 개발 모듈]/           # AI 학습 연동 및 신규 기능 확장을 위한 베이스라인
```

---

## 📋 프로젝트 개발 계획
- 개발 계획서 PDF 보기
![개발계획서](img/developmentPlan.pdf)

---

## 🚀 시작하기 (Getting Started)
- 요구사항 (Prerequisites)
- Windows OS
- .NET Framework 또는 .NET SDK
- Visual Studio 2022 이상 (.NET 데스크톱 개발 워크로드 포함)

---

## 실행 방법
- Visual Studio에서 DateManager.sln 오픈.
- F5를 눌러 애플리케이션 실행.

---

### 👥 팀 구성 및 역할 (Team Members)
- 김재서(컴퓨터소프트웨어학과)(팀장/UI 총괄): 전체 WinForms 레이아웃 설계, 이미지 표시 및 슬라이더 연동 담당
- 박진철(컴퓨터소프트웨어학과)(데이터 엔지니어): JSON 파싱 및 파일 시스템 삭제 로직 담당
- 윤형규(컴퓨터소프트웨어학과)(로직 개발자): 리스트 선택 및 LINQ 기반 데이터 필터링 알고리즘 구현]
- 이기주(컴퓨터소프트웨어학과)(시스템 통합): Python 프로세스 연동 학습 로그 비동기 출력 구현

---

### 진행 상황
![기초 UI 설계](img/screenshot-1.png)
- 기초 UI 설계


![기능 구현](img/screenshot-2.png)
- 기초 기능 구현


![UI 고도화](img/screenshot-3.png)
- UI 업그레이드 version 1.0


![UI 고도화](img/screenshot-4.png)
- UI 업그레이드 version 2.0
