# 블록 배치 기반 난이도 측정에 따른 Match-3 게임 맵 자동 생성 (2023.03 - 2023.12)


Input으로 목표(파괴할 블록의 종류와 개수)와 원하는 난이도(스왑 횟수)를 지정 시, 특수 블록이 배치된 원하는 난이도의 맵 생성을 위해 Match-3 게임 맵을 유전자로 설정, 특수 블록 배치 정도에 따라 난이도를 측정하고, 원하는 난이도의 맵이 나올 때까지 세대를 반복하여 자동으로 맵을 생성합니다.

<p align="Left">
  <img src="https://github.com/limbsoo/limbsoo.github.io/assets/96706760/d80efd7c-68e0-4390-84e5-548316bd4712" align="center" width="70%">
</p>

유전자 배열을 통해 유전자와 같은 크기의 그리드 형태의 맵을 생성하고, 유전자 값에 따른 블록 배치를 진행하여 맵을 구성합니다.

<p align="Left">
  <img src="https://github.com/limbsoo/limbsoo.github.io/assets/96706760/1f933ce1-88e6-4efb-b643-ae026de73080" align="center" width="100%">
</p>



생성된 맵은 파이프 라인을 따라 원하는 난이도의 맵이 나올 때까지 세대를 반복합니다.

<p align="Left">
  <img src="https://github.com/limbsoo/limbsoo.github.io/assets/96706760/2d0a3c4f-c792-431e-b801-0ad17a92de4c" align="center" width="70%">
</p>


원하는 난이도의 맵을 생성하기 위해 **유효성 검사**와 **종결 조건 충족 검사**를 거칩니다.


**유효성 검사**는 장애물 랜덤 배치로 인해 플레이에 악영향을 주는 빈 셀이 계속 유지되는 경우를 막고, 이를 통과한 맵만이 종결 조건 충족 검사를 진행합니다.

<p align="Left">
  <img src="https://github.com/limbsoo/limbsoo.github.io/assets/96706760/10a3f823-2418-4fd4-8ec5-d7cc0eab9158" align="center" width="70%">
</p>


**종결 조건 충족 검사**는 난이도 측정이라고 할 수 있습니다. 난이도 측정을 위해 맵에서 스왑을 통해 발생시킬 수 있는 최대 매치 발생 경우의 수를 구하고, 특수 블록 배치에 따른 경우의 수의 감소 정도를 측정합니다.

<p align="Left">
  <img src="https://github.com/limbsoo/limbsoo.github.io/assets/96706760/9b321e48-605a-419f-ad26-1a15e9c52fd5" align="center" width="70%">
</p>




이러한 감소 정도를 특수 블록의 종류 별 측정한 표본들의 회귀 분석 기울기에 대입하여 난이도를 측정합니다.

<p align="Left">
  <img src="https://github.com/limbsoo/limbsoo.github.io/assets/96706760/6a46c117-6c21-428a-9061-1d25cfca13f0" align="center" width="70%">
</p>


이를 통해 아래와 같은 MSE, 예측 정도를 얻을 수 있었습니다.

<p align="Left">
  <img src="https://github.com/limbsoo/limbsoo.github.io/assets/96706760/d991de33-6fc2-4474-ad8d-430c94ef7b8e" align="center" width="70%">
</p>
