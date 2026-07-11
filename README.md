# OfficeSystem

사내 임직원 관리 및 전자결재 시스템 (ASP.NET Core Web API)

## 프로젝트 소개

OfficeSystem은 임직원 관리, 역할 기반 접근 제어, 전자결재 워크플로우를 목표로 하는 백엔드 API 프로젝트입니다. 울산 지역 .NET 채용 공고의 요구사항(직원 관리 / 권한 관리 / 전자결재)에 맞춰 직접 설계·구현하고 있습니다.

기존에 Java / Spring Boot / MyBatis 기반으로 개발해왔으며, 이 프로젝트를 통해 C# / ASP.NET Core / EF Core 스택으로 전환하며 학습한 내용을 실제 동작하는 코드로 정리했습니다. 백엔드의 구조와 설계 원리가 언어·프레임워크를 넘어 동일하게 적용된다는 점을 직접 확인하는 것을 목표로 하며, 단순히 기능을 만드는 것을 넘어 **각 설계 결정에 "왜 이렇게 했는가"를 설명할 수 있는 것**을 지향합니다.

## 기술 스택

| 구분 | 사용 기술 |
|---|---|
| 언어 / 프레임워크 | C#, ASP.NET Core Web API (.NET 10) |
| ORM | Entity Framework Core |
| 데이터베이스 | SQL Server (MSSQL) |
| 인증 | JWT (JSON Web Token), BCrypt |
| 시크릿 관리 | .NET User Secrets |
| 버전 관리 | Git / GitHub |
| 개발 환경 | Visual Studio 2026 |

## 주요 기능

### 구현 완료

- **회원가입** — DTO → Service → Controller 레이어 분리, BCrypt 비밀번호 해싱, 응답 시 민감정보(해시) 제외
- **로그인** — 로그인 아이디 조회 + BCrypt 검증, 실패 응답 통일(사용자 열거 방지)
- **JWT 토큰 발급** — 로그인 성공 시 서명된 토큰 반환, 만료 시간 설정, User Secrets로 시크릿 키 분리
- **JWT 토큰 검증** — 인증 미들웨어(`AddAuthentication` + `UseAuthentication`)로 매 요청마다 서명 검증, `[Authorize]`로 보호된 자원 접근 제어
- **아이디 중복 방지** — 애플리케이션 레벨(사전 조회) + DB 레벨(Unique 제약) 2층 방어
- **역할 기반 접근 제어 (RBAC)** — 토큰의 역할 클레임과 `[Authorize(Roles = ...)]`로 자원별 권한 제어
- **전자결재 — 상신** — 문서 생성 + 결재선(순차) 지정. 상신자 신원은 요청 바디가 아닌 토큰 클레임에서 추출하고, 문서 상태는 서버가 강제

### 구현 예정

- **전자결재 — 결재 처리** — 승인/반려, 다음 순서로 넘기기, 문서 종합 상태 계산
- **전자결재 — 조회** — 내가 상신한 문서 / 내가 결재할 문서 목록

## 실행 방법

### 사전 준비

- .NET 10 SDK
- SQL Server (LocalDB 또는 인스턴스)

### 1. 저장소 클론

```bash
git clone https://github.com/park1541/OfficeSystem.git
cd OfficeSystem
```

### 2. User Secrets 설정

민감 정보(DB 연결 문자열, JWT 설정)는 `appsettings.json`이 아니라 User Secrets로 관리합니다.

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<본인 DB 연결 문자열>"
dotnet user-secrets set "Jwt:Key" "<충분히 긴 임의의 서명키>"
dotnet user-secrets set "Jwt:Issuer" "<발급자 식별 문자열>"
dotnet user-secrets set "Jwt:Audience" "<대상자 식별 문자열>"
```

### 3. 데이터베이스 마이그레이션

```bash
dotnet ef database update
```

### 4. 실행

```bash
dotnet run
```

### 5. API 테스트

프로젝트 내 `.http` 파일로 요청을 보낼 수 있습니다. 인증이 필요한 요청은 로그인으로 받은 토큰을 `Authorization: Bearer {{token}}` 헤더에 넣어 호출합니다.

> 참고: 전자결재 상신을 테스트하려면 `DocumentTypes` 테이블에 문서 종류 데이터가 최소 1개 있어야 합니다. (초기 데이터는 추후 시드 데이터로 관리 예정)

## 아키텍처

요청 처리는 계층을 분리하여 관심사를 나눴습니다.

```
Controller  →  Service  →  DbContext (EF Core)  →  MSSQL
  (표현)       (비즈니스 로직)      (데이터 접근)
```

- **Controller** — HTTP 요청/응답 처리, DTO 변환, 상태 코드 결정 (얇게 유지)
- **Service** — 인증·검증 등 비즈니스 로직 (인터페이스로 추상화하여 DI)
- **DbContext** — EF Core를 통한 데이터 접근

DTO를 요청용(`LoginRequest`, `RegisterRequest`, `CreateDocumentRequest`)과 응답용(`UserResponse`, `DocumentResponse`)으로 분리하여, 들어오는 데이터와 나가는 데이터의 형태를 독립적으로 관리했습니다.

인증 처리는 미들웨어 파이프라인에서 **인증(Authentication) → 인가(Authorization)** 순서로 구성했습니다. 신원 확인이 끝나야 권한을 판단할 수 있으므로, `UseAuthentication`을 `UseAuthorization`보다 앞에 등록합니다.

## 주요 설계 결정

프로젝트를 진행하며 내린 판단과 그 이유를 기록합니다.

- **비밀번호는 BCrypt로 해싱하고, 검증은 `BCrypt.Verify`에 위임** — 솔트가 해시 문자열 내부에 포함되므로 별도 저장 없이 검증 가능. 로그인 시 평문을 재해싱하지 않고 저장된 해시와 비교합니다.
- **로그인 실패 응답 통일** — "존재하지 않는 아이디"와 "비밀번호 불일치"를 동일하게 응답하여, 공격자가 유효한 계정 목록을 수집하는 사용자 열거(User Enumeration)를 방지했습니다.
- **응답 전용 DTO로 민감정보 차단** — 엔티티를 그대로 응답하지 않고, 필요한 필드만 선별하여 `PasswordHash` 같은 내부 정보가 API 응답에 노출되지 않도록 했습니다.
- **아이디 중복은 2층으로 방어** — 애플리케이션 레벨(사전 조회)로 친절한 안내를 제공하고, DB 레벨(Unique 제약)로 동시 요청까지 확실히 차단했습니다.
- **JWT 검증은 서명 재계산 방식** — 저장된 서명을 꺼내 비교하는 것이 아니라, 서버가 서명 키로 서명을 매 요청마다 재계산하여 토큰에 담긴 서명과 대조합니다. Payload가 위조되면 재계산 값이 달라져 검증에 실패하므로, 서명 검증 하나로 토큰 전체의 무결성이 보장됩니다. (BCrypt 비밀번호 검증과 동일한 재계산 원리)
- **RBAC는 역할 이름 직접 매칭** — 역할이 소수라 `[Authorize(Roles = ...)]`로 직접 나열하는 단순한 방식을 택했습니다. 역할이 늘어 나열이 반복되면 정책 기반 인가나 역할 계층으로 추상화하는 것이 유리하지만, 현재 규모에는 단순 매칭이 명확합니다.
- **상신자 신원은 토큰 클레임에서 추출** — 전자결재 상신 시 상신자 ID를 요청 바디로 받지 않고 JWT 클레임에서 꺼냅니다. 바디로 받으면 타인 명의의 문서를 위조할 수 있고, 검증해서 쓸 값이라면 애초에 받을 이유가 없기 때문입니다. 신원은 항상 서버가 서명을 검증한 토큰에서 가져옵니다.
- **전자결재 상태를 두 층으로 분리** — 결재자 개개인의 판단(결재선 각 행)과 문서 전체의 종합 상태(문서 테이블)를 분리했습니다. 개인 판단들을 종합해 문서 상태를 서버가 계산하는 구조라, "팀장은 승인·부장은 대기" 같은 중간 상태를 정확히 표현할 수 있습니다.
- **얇은 컨트롤러 / 두꺼운 서비스** — 컨트롤러는 HTTP 입출력(요청 바인딩, 인증 정보 추출, 상태 코드 반환)만 담당하고, 업무 규칙은 서비스 계층에 둡니다. 업무 로직이 서비스에 모여야 테스트·재사용이 쉽고 수정 지점이 명확해집니다.

## API 엔드포인트

| 메서드 | 경로 | 인증 | 설명 |
|---|---|---|---|
| POST | `/api/auth/register` | - | 회원가입 |
| POST | `/api/auth/login` | - | 로그인 (성공 시 JWT 반환) |
| GET | `/api/auth/secret` | 필요 | JWT 검증 확인용 보호 엔드포인트 |
| GET | `/api/auth/admin-only` | 관리자 | RBAC 확인용 (관리자 역할 전용) |
| POST | `/api/document` | 필요 | 전자결재 상신 (문서 생성 + 결재선 지정) |

## 설계 문서

각 기능의 도메인 설계와 구현 판단 근거를 Notion에 정리했습니다.

- 📚 [OfficeSystem — 구현 & 학습 정리 (설계 문서 모음)](https://app.notion.com/p/OfficeSystem-390ea3c3663981c8a8caf070c19d4af8)

> 위 링크는 노션 페이지를 웹에 게시(또는 링크 공개) 설정한 뒤에야 외부에서 열립니다. 로그아웃 상태(시크릿 창)에서 실제로 열리는지 확인하세요.
