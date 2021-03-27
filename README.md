## StrawberryM
C# Xamarin을 이용한 안드로이드 음악 재생 앱

### 제작 목적
* 주로 유튜브에 있는 피아노 모음집을 즐겨 듣는데, 핸드폰에 파일을 일일이 추가하기 귀찮아 어플을 찾아보았으나 
광고가 실사용에 지장이 있을 정도로 많아서, 광고 없는 클린한 나만의 어플을 만들기로 함.

### 사용된 패키지
*  Google Youtube API ( 노래 검색 ) 
*  YoutubeExplode ( 노래 다운로드, 메모리 누수가 발견되지 않아서 사용하게 됨 )
*  SimpleAudioPlayer ( 노래 플레이, 기존 안드로이드 플레이어가 셋업 과정이 많아서 대신 사용하게 됨 )

### 구현 사항
#### 기능
* 유튜브에서 노래 검색, 다운로드
* 기본적인 노래 재생 관련 기능 ( 재생, 정지, 곡 넘기기, 재생 모드 설정 등 )
* 오디오 포커징 기능, 다른 앱들과의 중복 재생 방지
* 이어폰 분리 시 노래 재생 중지 
* 노티피케이션을 통한 간단한 노래 조작, 어플 재우기
* ForegroundService 등록, 백그라운드에서 종료 방지 - 2020.10.04
* seek 함수가 일부 핸드폰에서 동작하지 않음, 동영상 다운로드 방식 바꿈 (.webm -> .mp4) - 2020.10.13

#### 디자인
* 플라이아웃을 이용한 뷰 전환 및 생성 ( NowPlay는 static 객체, 나머지는 요청 시 생성 )
* 노래 재생 화면에서 LP판이 재생 상태에 맞게 회전
* 재생 모드 바뀔 때마다 토스트 메세지 띄움

#### 수정(2021.03.27)
* 노티피케이션 뷰 수정
* 랭킹 검색 추가
* 랭킹 순위 터치시 바로 검색

### 작동 모습
#### 내 노래 목록, 현재 재생곡, 노래 검색
<div>
  <image width="250" src=https://user-images.githubusercontent.com/59993347/94995037-750f2880-05d6-11eb-8bf1-b469dc226ab8.jpg>
  <image width="250" src=https://user-images.githubusercontent.com/59993347/94995033-72acce80-05d6-11eb-8a37-818ba05b78ac.jpg>
  <image width="250" src=https://user-images.githubusercontent.com/59993347/94995039-75a7bf00-05d6-11eb-9993-6e024e199527.jpg>
</div>

#### 모드 변경 알림, 노티피케이션, 플라이아웃 메뉴
<div>
  <image width="250" src=https://user-images.githubusercontent.com/59993347/94995035-73ddfb80-05d6-11eb-8a45-f76fd4378ca6.jpg>
  <image width="250" src=https://user-images.githubusercontent.com/59993347/94995036-74769200-05d6-11eb-9309-19f858c5ac9e.jpg>
  <image width="250" src=https://user-images.githubusercontent.com/59993347/94995038-75a7bf00-05d6-11eb-8b26-a920283a490d.jpg>
</div>

#### 수정된 뷰(축소), 수정된 뷰(확대), 랭킹
<div>
  <image width="250" src=https://user-images.githubusercontent.com/59993347/112708179-eb407800-8ef3-11eb-96c6-e612a9d5fcec.jpg>
  <image width="250" src=https://user-images.githubusercontent.com/59993347/112708178-ea0f4b00-8ef3-11eb-8095-c0adccc079bc.jpg>
  <image width="250" src=https://user-images.githubusercontent.com/59993347/112708180-ebd90e80-8ef3-11eb-989e-75f5369dc1ae.jpg>
</div>


