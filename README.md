<div align="center">

![그림1](https://github.com/user-attachments/assets/171b0040-33a7-4ea5-b065-9ba28f333bfa)

# BABO IS YOU

</div>
<br/><br/>

# 🎬 Overview
<div align="center">

[![image](https://github.com/user-attachments/assets/ee0fb746-2f13-4f37-b901-349022aedc5c)](https://youtu.be/94JqXuQleCw)

👀 Watch in Youtube!
</div>
<br/>

## 📃 프로젝트 개요 
### C# 2D Console Project

 - 게임 장르 : 퍼즐
 - 개발 환경 : Visual Studio 2019 | .NET Framework 4.7.2
 - 개발 기간 : 2024. 05. 24 ~ 2024. 05. 28  ( 1 주 )
<br/>

> 본 프로젝트는 2D 퍼즐게임 `[ BABA IS YOU ]` 를 C# Console Application으로 모작 구현한 프로젝트입니다.
> 
> 한 스테이지 내에서 플레이어는 `[ YOU ]` 단어가 가르키는 개체를 조작하게 됩니다.
> 
> 예를 들어, 각 스테이지에서는 기본적으로 `[ BABA ]` `[ IS ]` `[ YOU ]` 라는 단어가 서로 붙어있으며
>
> 이 경우 플레이어는 `[ BABA ]`를 조작할 수 있습니다.

<div align="center">
  
![BABOisYOU2024-12-1114-59-24-ezgif com-resize](https://github.com/user-attachments/assets/9af86b17-9827-426a-9e7c-fe6538540d24)
</div>
<br/>

> 스테이지에 존재하는 모든 단어 개체는 `[ IS ]`, `[ HAS ]` 등의 단어를 통해 문장으로 구성할 수 있고,
>
> 문장이 될 경우 고유한 특수 기능을 발동하게 됩니다.
>
> 스테이지를 클리어하기 위해선 각 스테이지에 존재하는 `[ WIN ]` 단어를 문장으로 구성한 뒤,
>
> 그 문장에 맞는 조건을 달성해야 합니다.

<div align="center">
  
![ezgif-4-67c6459262](https://github.com/user-attachments/assets/edb04d4b-90bc-44ad-ac57-05b5488990da)
</div>
<br/>

## 🛠 구현 기능

### Console Window
 - Window Initialize
   > Console Window에 출력되는 폰트는 기본 사이즈가 너무 커서,
   > 도트로 표현되는 Sprite를 디테일하게 화면에 띄우기에는 한계가 있었습니다.
   > 
   > 따라서 게임이 시작되면 Windows API를 사용해 Console Window를 Borderless FullScreen 으로 설정하고,
   > SendInput Method를 활용해 강제로 Wheel Down Event를 발생시켜 화면의 해상도를 높이도록 구현했습니다.
 - Input Control
   > Console 화면에 값을 출력할 때 필연적으로 생기는 Delay 동안, 입력이 발생할 경우 입력 버퍼에 값이 남아
   > 의도하지 않은 작동을 하는 경우가 발생했습니다.
   >
   > 화면에 출력을 시작할 때부터 출력을 마칠 때까지 Delay되는 시간동안 사용자로부터 키 입력을 받지 않도록 처리했습니다.

### Stage
  - File IO : Load Map Data
    > 게임 내 존재하는 스테이지는 스크립트 내부에 저장하지 않고, 외부 파일로 저장한 뒤 스테이지가 시작할 때 불러오도록 구현했습니다.
    >
    > 한 스테이지는 하나의 맵 파일로 저장하여, 맵을 수정하거나 추가해야 할 상황에서 유지보수가 용이하도록 설계했습니다.
  - Stage Print
    > Console에 맵을 출력할 때, 대량의 텍스트를 출력하면서 생길 수 있는 Flickering, Tearing 등 화면 끊김을 방지하고자
    > 화면의 전체 픽셀 수에 대응하는 Screen Buffer를 구현했습니다.
    >
    > 한 프레임 내에서 각 블록에 속성, 위치 등 상태 변화가 있을 때, 각자 화면을 업데이트하지 않고
    > Screen Buffer에 변경되는 부분을 저장하였다가, 이전 프레임과 변경점을 비교한 뒤 일괄적으로 출력하도록 처리했습니다.
    
### Logic
  - `[ IS ]`
    > `[ BABA IS YOU ]` 퍼즐 로직의 핵심 기능을 하는 `[ IS ]` 단어 기능을 구현하기 위해,
    > 
    > `[ IS ]` 단어의 상하좌우에 다른 단어가 위치할 경우를 감지할 경우 우선 문장이 구성될 수 있는지 판별하게 됩니다.
    > 문장이 유효한 경우, 상하좌우의 각 단어에 해당하는 블록을 맵 상에서 찾아서,
    > 해당 블록의 속성 값을 부여하거나 변경하도록 처리했습니다.
    >
    > 각 블록은 자신의 속성이 변경되는 경우 변경된 속성 값에 따라서 적합한 상호작용을 발생시키거나, 블럭 외형이 변경되도록 구현했습니다.
    
