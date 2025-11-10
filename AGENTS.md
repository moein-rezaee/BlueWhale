Agent Guide (AI-Facing)

Purpose and Scope
- This single document is the authoritative, root-level guide for any AI building or evolving code in this repository.
- It defines complete, opinionated checklists for:
  - Shared extensions (libraries) used by multiple services
  - New microservices (from scratch) with consistent architecture and conventions
- Treat all items as hard requirements unless explicitly stated as optional.

Global Conventions (Applies to Everything)
- Repo layout: per-service top-level folders and shared/ (libraries)
  - Each service lives at <Service>/<Service>.Api | .Application | .Domain | .Infrastructure
  - Do not use a root-level "src" folder for services (legacy layout)
- Configuration isolation: Each service's `appsettings.json` and `.env` must only contain keys used by that service. Do not keep provider secrets or settings of other services in a different service's config. Cross-service URLs live only in the consumer (e.g., `OTPService` holds `Notification:BaseUrl`).
  - No unused keys: If a service does not use a component, do not include its settings (in appsettings.json, `.env`, or compose). Examples:
    - If the service doesn’t use cache: no `Cache:` section in appsettings, no `REDIS_*` in `.env`, and no Redis service in compose.
    - If the service doesn’t send SMS: no `Sms:` section in appsettings, no `KAVENEGAR_*`/`FARAPAYAMAK_*` in `.env`.
  - Prefer per‑service `.env.example` for documentation; real `.env` must contain only keys actually used by that service.
- Target Framework: net9.0 (use multi-target net8.0 if consumers require)
- Microsoft.Extensions.* packages: prefer 9.0.0 to match TFM
- Clean code: no commented/dead code, no XML Swagger comments in controllers or libraries
- Temporary artifacts: never leave behind temp files/folders. If a step creates temporary outputs (e.g., tmp_build_*, tmp_*, temp_*, names starting with ~, or files with .tmp/.temp), delete them before yielding. Keep the repo clean of ephemeral build/test artifacts at all times.
- Namespaces: use using statements; do not inline fully-qualified namespaces
  - Never reference types with fully-qualified names in code (e.g., AuthenticationService.Domain.Interfaces.IOtpClient). Always add the proper using and use the short type name. When you touch files, scan and fix such cases.
- Routing: every public route must begin with the lowercase version prefix `v1/api`; the remaining segments stay PascalCase
  - Example: /v1/api/Otp/Send, /v1/api/Otp/Verify
- Pagination: every paged endpoint validates `pageNumber >= 1` and `1 <= pageSize <= 100`; violations return HTTP 422 with `InvalidPageNumber` or `InvalidPageSize`
- Swagger: configured via shared/Swagger extension; Program.cs calls AddSwaggerExtension/UseSwaggerExtension and service-specific settings live in appsettings.json
- Swagger metadata (title/version/description/auth) must be supplied via each service's appsettings.json; avoid XML documentation files.
- Configuration: app loads `.env` via DotNetEnv, merges with `AddEnvironmentVariables()`, and may use `${ENV_VAR}` placeholders that the shared extensions resolve. 
  - Secrets (passwords, API keys, tokens) must be provided only via environment variables; appsettings.json should contain non-sensitive defaults only and must not include env-style placeholders.
  - Non-sensitive functional settings (timeouts, TTLs, feature toggles, scheduling intervals, pagination defaults, etc.) must live in `appsettings.json` (or typed options bound from it). Do not introduce `.env` keys for these values.
  - Hostnames, ports, base URLs, database names, and credentials are considered sensitive; store them only in environment variables and never in appsettings.json.
  - Do not duplicate configuration keys between appsettings.json and `.env`; choose a single source per key.
  - When reading environment-driven settings (hosts, ports, BaseUrls, credentials), access the env key directly (e.g., `configuration["OTP_BASE_URL"]`) since `Env.Load()` has already merged `.env` into configuration.
  - Env keys: UPPER_CASE with single underscores; concise (1–3 words). Avoid duplication across keys; prefer a single source of truth.
    - Examples: `POSTGRES_HOST`, `POSTGRES_PORT`, `POSTGRES_DB`, `POSTGRES_USER`, `POSTGRES_PASSWORD`, `JWT_SECRET`, `REDIS_HOST`, `REDIS_PORT`, `REDIS_USERNAME`, `REDIS_PASSWORD`, `MINIO_ROOT_USER`, `MINIO_ROOT_PASSWORD`, `PGADMIN_DEFAULT_EMAIL`, `PGADMIN_DEFAULT_PASSWORD`, `CACHE_ENCRYPTION_KEY`, `KAVENEGAR_API_KEY`, `FARAPAYAMAK_USERNAME`, `FARAPAYAMAK_PASSWORD`, `FARAPAYAMAK_FROM`, `OTP_BASE_URL`, `NOTIFICATION_BASE_URL`.
  - Consistent prefix per component: use one canonical prefix for a given context (e.g., always `POSTGRES_*` for Postgres, not a mix of `PG_*` and `POSTGRES_*`). When renaming, update all references in code, appsettings, and compose.
  - Single-source rule: If an infra image already uses a key (e.g., `POSTGRES_DB/USER/PASSWORD`), reuse the same prefix for related keys (e.g., `POSTGRES_HOST`, `POSTGRES_PORT`). Do not introduce parallel aliases.
  - Dedup rule: Never define the same env key twice in a `.env`. Before adding/renaming env keys, grep the repo for similar keys and consolidate usages in code, appsettings, and compose.
  - All environment-dependent settings (hosts, ports, BaseUrls, secrets) come from `.env`. `appsettings.json` must not contain literal secrets or unused keys.
- Security: never log secrets; produce concise, non-sensitive error messages
- Business IDs in API payloads: when clients send resource identifiers that are domain business IDs, accept them under the canonical property name `Id` in request payloads. Do not require a separate `BusinessId` field in request contracts. Examples: address update payloads use `Id` for the business ID.
- Provider IDs stay internal: never expose upstream/provider primary keys in API contracts or DTOs. Every persisted record must surface a deterministic GUID business identifier (the value clients see as `Id`). Compute the GUID from provider data in a reversible/deterministic way so the original provider identifier can be re-derived for commands (create/update/delete) and lookups. Persist provider identifiers separately (e.g., `ProviderId`) and translate between the GUID and provider id inside repositories/infrastructure.
- Enums in Swagger: annotate every enum member with `Display(Name = "...")` (or equivalent) so generated Swagger descriptions list both numeric values and human-friendly labels. Ensure Swagger schemas surface the enum mapping so clients know which numeric value corresponds to which logical option.
- Enums in contracts: prefer enums for categorical fields in Domain. Keep them nullable when absence is meaningful. For wire contracts, expose as strings when interoperability requires, and map string <-> enum in application mappers without defaulting missing values.
- Documentation: keep README files current. With any change to a microservice, shared extension, or cross-cutting behavior, update the relevant README(s): per-service (`<Service>/README.md`), shared extension (`shared/<ExtensionName>/README.md`), and root `README.md`. Treat this as a hard requirement.
- Response payloads: For every GET endpoint, replace foreign-key fields with the fully hydrated related object (or list of objects) and avoid duplicating the raw key alongside the expanded object. Nested relationships must be expressed as tree structures (e.g., Address → City → Province → Country) without repeating the same entity at multiple levels.
  - In-service FK expansion rule: For keys whose target entity exists inside the same microservice, GET responses must embed the related object(s) instead of returning GUID fields. Do not include the raw GUIDs alongside the embedded objects. Create/Update commands and query filters continue to use GUIDs.
  - Example (CatalogService): Item embeds `PrimaryUnit`/`SecondaryUnit` (Unit) instead of `PrimaryUnitId`/`SecondaryUnitId`; Price embeds `SaleType`, `Item`, `Currency`, and `Unit` instead of their `*Id` fields. External references (e.g., `TracingId`, `CustomerCategoryId`) remain GUIDs.
- Provider-only flags: do not propagate upstream/provider-specific flags into domain/contracts unless they are confirmed business requirements. If a flag has no current use (e.g., a default marker), omit it entirely from entities, DTOs, and APIs.
- City objects follow a strict hierarchy: `City` embeds only its `Province`, and that `Province` embeds the `Country`. Do not surface redundant country fields (or fields such as postal code prefixes) directly on the city when the data already exists deeper in the tree.
- JSON edits: when adding/removing/editing JSON (e.g., `appsettings.json`, compose fragments), ensure syntax remains valid. Remove trailing commas on the previous/last property when deleting, and verify structure after edits. Do not leave dangling commas or empty sections.
- Validation: every inbound API contract (controllers and MediatR requests) must have strict FluentValidation rules; register validators via AddValidatorsFromAssemblyContaining<...>() and enable AddFluentValidationAutoValidation().
- Error handling: all APIs must reference shared/ErrorHandling, call AddErrorHandling() during service registration, and UseErrorHandling() in the middleware pipeline; application/infrastructure layers throw the shared ServiceException derivatives (BadRequestException, NotFoundException, UnauthorizedException, ExternalServiceException, etc.) so middleware can emit ProblemDetails responses and log errors without exposing sensitive data.
- Logging: all APIs must wire logging via shared/Logging (AddLoggingExtension/UseLoggingExtension) to ensure consistent console output, scopes, and correlation IDs.
- Assembly markers: define a dedicated `AssemblyMarker` class per project and use it for assembly scanning (MediatR, FluentValidation, etc.) instead of referencing random files.
- Assembly markers must be non-static reference types so AddValidatorsFromAssemblyContaining/assembly scanners can instantiate them.
- Request logging records exactly one entry per request (success info, client warning, or server error) with method, path, status, duration, correlation id; do not log sensitive data.
- Shared extensions: whenever a shared extension exists (ErrorHandling, Logging, Cache, Sms, etc.), services must call the provided Add*/Use* methods instead of duplicating configuration.
- Do not register shared extensions or include configuration sections when the service does not use them. Remove unused Cache/Sms/OTP/etc. settings from appsettings and avoid calling Add<ExtensionName>Extension().
- Snapshot fallback: هر زمان داده‌ای از سامانه‌های upstream (مانند Sepidar) کش می‌شود، باید از `ICacheSnapshotService` استفاده کنید و کلید snapshot جداگانه با TTL طولانی‌تر نگه دارید تا در صورت خطا یا timeout، API بتواند داده‌ی سالم قبلی را برگرداند (الگوی stale-while-revalidate). TTL snapshot حداقل برابر TTL اصلی و کانفیگ‌پذیر از appsettings است.
- Options: place feature-specific options inside the owning feature folder (e.g., <Service>.Application/Features/<Module>/Options); use <Service>.Application/Options/Common for cross-feature settings; keep shared extension options inside shared/<Extension>/Core/Options.
- Options classes must live under an `Options` folder (never `Settings`) and use the `*Options` suffix to aid discovery and DI registration.
- appsettings.json should include only non-sensitive defaults; remove keys entirely (do not leave blank strings) when values come from environment variables.
- Options folder structure: if an options class only relates to a specific entity/feature, place it under that feature (e.g., Options inside the feature folder); otherwise keep cross-feature options under <Service>.Application/Options/Common.

Extensions (Libraries) -” Checklist and Rules

Foldering and Naming
- Location: shared/<ExtensionName>
  - Example: shared/SmsProvider, shared/CacheService
- Namespaces (RootNamespace in csproj):
  - SmsExtension.* for SMS, CacheExtension.* for Cache, etc.
- Subfolders (required where applicable):
  - Abstractions: public interfaces (e.g., ISmsProvider, ISmsProviderFactory, ICacheService, ICacheServiceFactory)
  - Core: Options (DefaultProviderOptions), factories (SmsProviderFactory/CacheServiceFactory), default decorators (DefaultSmsProvider), helpers
  - Providers (or Provider.*): concrete implementations per external system
- DependencyInjection.cs: IServiceCollection extension entrypoint (نیازی به قرارگیری داخل پوشه‌ای با همین نام نیست؛ می‌تواند در ریشه پروژه قرار گیرد)

Design and DI
- Single entrypoint per extension:
  - public static IServiceCollection Add<ExtensionName>Extension(this IServiceCollection services, IConfiguration configuration)
  - All wiring, option binding, env placeholder expansion, and validation happen inside this method
- Factories and defaults:
  - Provide DefaultProviderOptions with string DefaultProvider (e.g., "Kavenegar"|"Farapayamak" or "Memory"|"Redis")
  - Provide Factory with Get(kind), GetDefault(), GetAll()
  - Provide DefaultXxxProvider that delegates to the selected concrete provider
- Register only configured providers (validate minimum required options)
  - SMS
    - Kavenegar requires ApiKey + Sender
    - Farapayamak requires Username + Password + From (map Senderâ†’From if only Sender provided)
  - Cache
    - Redis requires Host + Port (Username/Password optional)
- Fallbacks and errors:
  - Choose default provider from configuration; throw a clear exception if no usable provider is configured

Configuration Contracts (required)
- SMS
  - appsettings: Sms:DefaultProvider ("Kavenegar" | "Farapayamak")
  - env: KAVENEGAR_API_KEY, KAVENEGAR_SENDER, FARAPAYAMAK_USERNAME, FARAPAYAMAK_PASSWORD, FARAPAYAMAK_FROM
- Cache
  - appsettings: Cache:DefaultProvider ("Memory" | "Redis")
  - env: REDIS_HOST, REDIS_PORT, REDIS_USERNAME?, REDIS_PASSWORD?, INSTANCE_NAME?, DEFAULT_DATABASE?, CACHE_ENCRYPTION_KEY?

Clean Code and SOLID (extensions)
- SRP: each provider integrates one external system only
- OCP: add new providers by adding classes/registrations; avoid modifying existing behavior
- DIP: consumers depend on abstractions (ISmsProvider/ICacheService), not concretes
- Nullability enabled; implicit usings enabled; no commented code

Logging and Errors (extensions)
- Use ILogger in providers and factories (Debug on invocation; Error on failures)
- Do not log secrets; return concise error messages
- For sync SDKs, use Task.Run sparingly with cancellation tokens

Testing and Packaging (extensions)
- Provide a fake/in-memory provider for dev/test when feasible
- Optional: GeneratePackageOnBuild + package metadata for internal registry publishing

Extensions -” Acceptance Checklist (AI must verify)
- [] Folder at shared/<ExtensionName> created with Abstractions/Core/Providers/DependencyInjection.cs
- [] net9.0 TFM and Microsoft.Extensions.* 9.0.0 in csproj
- [] DefaultProviderOptions + Factory + Default provider implemented
- [] Only validated providers registered; default selection implemented
- [] Add<ExtensionName>Extension expands ${ENV} and binds options
- [] Logging in providers/factory; no secrets in logs
- [] Sample consumer compiles and runs with this extension
- [] Documentation updated: `shared/<ExtensionName>/README.md` exists and reflects current behavior; root `README.md` updated if usage changes

HttpClientRestExtension Notes
- Default provider wraps `IHttpRestClient`/factory pattern; configure base URL and default headers through `HttpClientRest` options.
- `HttpClientRest` options expose `TimeoutSeconds`, `RetryCount`, `RetryBaseDelayMilliseconds`; تنظیم این مقادیر برای سرویس‌هایی که به Sepidar/سرویس‌های کند متصل هستند الزامی است تا retry و timeout یکدست باشند.
- Consumers must resolve `IHttpRestClient` (scoped) or `IHttpRestClientFactory` via DI; avoid manual `new HttpClient`.
- Leverage `DefaultHeaders` for Authorization (e.g., JWT Bearer) to keep Program.cs wiring minimal.


Microservices -” Checklist and Rules

Repository Layout per service
- <Service>/<Service>.Api (Web API)
- <Service>/<Service>.Application (CQRS, handlers, validators, app services)
- <Service>/<Service>.Domain (interfaces/contracts)
- <Service>/<Service>.Infrastructure (infra implementations; cross-cutting under shared)

Program.cs (Api)
- DotNetEnv.Env.Load() at start
- AddControllers(), AddEndpointsApiExplorer()
- AddSwaggerGen(options => options.ConfigureSwagger());
- Register only required shared extensions:
  - AddCacheExtension(builder.Configuration) only if the service uses caching
  - AddSmsExtension(builder.Configuration) only if the service sends SMS
- MediatR: RegisterServicesFromAssembly(typeof(AssemblyMarker).Assembly)
- FluentValidation: AddValidatorsFromAssemblyContaining<AnyValidator>() + AddFluentValidationAutoValidation()
- CORS default (loosen or tighten per service)
- app.UseSwagger(); app.UseSwaggerUI(...)
- HTTPS redirection must be conditional: enable only if serving HTTPS (e.g., when `ASPNETCORE_URLS` contains "https"). This ensures Swagger works in Docker (HTTP-only) and locally.
- app.UseHttpsRedirection() when applicable; app.UseCors(); app.UseAuthorization(); app.MapControllers()
- Root GET endpoint at `/` must return a concise plain-text status including: service name, environment, port, Swagger URL (`/swagger/index.html`), base API prefix, and current UTC time.
- app.UseSwagger(); app.UseSwaggerUI(-¦)
- app.UseHttpsRedirection(); app.UseCors(); app.UseAuthorization(); app.MapControllers()

Routing
- Prefix: lowercase "api" only; other segments are PascalCase
- Controllers: [Route("api/[controller]")]
- Actions: PascalCase (e.g., [HttpPost("Send")])
- Do NOT enable global LowercaseUrls

Swagger
- Centralized via shared/Swagger extension (no per-service manual Swagger wiring)
  - Shared extension handles AddSwaggerExtension/UseSwaggerExtension; service-level customization lives in the `Swagger` section of each service's appsettings or via additional filters registered through the extension.
  - Shared Swagger extension exposes default operation filters for null-safe defaults and consistent schema IDs.
- No XML summary/response comments in controllers

Application/Domain/Infrastructure separation
- Application: CQRS handlers thin; validators near DTOs; orchestration only
- Domain: pure contracts/interfaces; no infra dependencies
- Infrastructure: IO/DB integrations if not already in shared
 - Foldering (strict): Under each Feature, group files by operation
   - Commands/Create|Update|Delete → place command + handler + validator for that operation inside its subfolder
   - Queries/GetById|GetPaged|GetAll|<FeatureSpecific> → each query + handler + validator inside its own subfolder
   - Keep DTOs and Mappers under Dtos/ and Mappers/
   - Namespaces remain `...Features.<Feature>.Commands` or `...Queries` to avoid churn; only the physical layout is stricter for discoverability

Using shared extensions
- Always use extension methods in Program.cs (AddSwaggerExtension/AddCacheExtension/AddSmsExtension)
- Default services:
  - ISmsProvider â†’ DefaultSmsProvider (selection via config)
  - ISmsProviderFactory for runtime selection (Get/GetDefault/GetAll)
  - ICacheService â†’ default selection; ICacheServiceFactory available

Service Configuration Contracts
- Cache: appsettings -> Cache:DefaultProvider; env -> REDIS_HOST, REDIS_PORT, REDIS_USERNAME?, REDIS_PASSWORD?, CACHE_ENCRYPTION_KEY?
- SMS: appsettings -> Sms:DefaultProvider; env -> KAVENEGAR_* or FARAPAYAMAK_*
- All environment values are read via IConfiguration; `${ENV_VAR}` placeholders are supported and expanded via the shared extensions.

Clean Code and SOLID (services)
- SRP/OCP/DIP enforced; controllers thin; no DI/config logic outside Program.cs and shared extensions
- No fully-qualified namespaces inline; always use using
- Nullability enabled; no commented code

Docker and Compose
- Dockerfile: place under <Service>/<Service>.Api; use sdk:9.0 and aspnet:9.0; restore by copying solution + csproj (<Service>/* and shared/*), then copy source and publish
- Infrastructure environments live under `infrastructure/<Environment>/` (e.g., `infrastructure/prod`). Inside each environment, every component has its own folder (Postgres, Redis, Minio, pgadmin, …) with a dedicated docker-compose.yml and co-located `.env`; microservice compose files must include only the env files they need (../infrastructure/prod/postgres/.env, ../infrastructure/prod/redis/.env, etc.) before their own.
  - When introducing another environment (e.g., staging), duplicate the folder pattern under `infrastructure/<NewEnvironment>/...`; do not place component folders directly under `infrastructure/`.
  - برای محیط‌های بدون دسترسی آزاد، Docker daemon را طوری پیکربندی کنید که از mirror داخلی (مثلاً `https://docker.arvancloud.ir/`) استفاده کند و در صورت نیاز به ریجستری خصوصی، قبل از اجرا `docker login registry.arvancloud.ir` را انجام دهید؛ نام‌های ایمیج در compose همان تگ‌های رسمی باقی می‌مانند.
  - قبل از `docker compose build`, متغیر محیطی `NUGET_SOURCE` را روی mirror در دسترس (مثلاً `https://nuget.arvancloud.ir/v3/index.json`) تنظیم کنید تا Dockerfiles هنگام restore فقط از همان منبع استفاده کنند؛ در صورت خالی بودن متغیر، مقدار پیش‌فرض `https://api.nuget.org/v3/index.json` به کار می‌رود.
- Per-service docker-compose.yml: place under <Service>/ so each microservice runs standalone
  - Only keep settings for the service itself (appsettings/env for that service)
  - Declare depends_on for infra (e.g., redis). If the service needs another microservice, prefer including that service's compose via top-level include rather than duplicating its config
  - Use container hostnames for dependent URLs (e.g., http://notification-service:5260)
  - Cross-service URLs are not secrets; define them in `appsettings.json` of the consumer service (e.g., `"Otp": { "BaseUrl": "http://otp-service:5254" }`). Do not place such URLs in `.env` or remap them via compose `environment`.
  - Always load the service .env via env_file from the Api layer:
    env_file:
      - ./<Service>.Api/.env
  - Do not duplicate variables from .env in environment. Only keep entries in compose `environment:` that are NOT defined in the service `.env` (e.g., `ASPNETCORE_URLS` or truly dynamic overrides).
  - Rule: If a key exists in the service `.env`, do not repeat it under that service's compose `environment:`; rely on `env_file` to provide it. If an infra image (e.g., Postgres/Redis) expects specific env names (like `POSTGRES_DB`), define those exact keys directly in `<Service>.Api/.env` instead of remapping inside compose.
  - All usernames, passwords, tokens, and secrets must live in `.env` only. Never hardcode or redefine them under compose `environment:`.
  - Rule: If a key exists in the service `.env`, do not repeat it under that service's compose `environment:`; rely on `env_file` to provide it.
  - Persistent data must use bind mounts (no named volumes). Keep data under a `./data` folder that sits beside the compose file providing the mount. Examples:
    - Redis: `./redis/data:/data`
    - Postgres: `./postgres/data:/var/lib/postgresql/data`
  - For DB/Redis containers declared in a service compose, load only secrets (user/password and similar) from the Api layer `.env` via `env_file`, and map them to the container's expected env names. Avoid hardcoding secrets in compose. Example:
    - `env_file: ./<Service>.Api/.env`
    - Define directly in `.env`: `POSTGRES_DB=...`, `POSTGRES_USER=...`, `POSTGRES_PASSWORD=...` (do not remap from other keys)
  - Mount appsettings.json و `.env` با `type: bind`; پوشه‌ی `./data` کنار docker-compose به `/app/...` منتقل شود تا داده‌های پایدار سرویس ذخیره شود
  - Shared infra singletons: run only one instance of shared infra (Redis/Postgres) across the stack.
    - Use a single external docker network named `online-shop` and attach all services/infras to it.
    - Choose fixed container names (no product-specific prefixes) for shared infra and reuse them in all services' `.env`.
      - Redis: container_name `redis` → `REDIS_HOST=redis`
      - Postgres: container_name `postgres` → `POSTGRES_HOST=postgres`
      - Minio: container_name `minio` → `MINIO_SERVER_URL=http://minio:9000`
      - pgAdmin: container_name `pgadmin-service` (connect through `http://localhost:5050`)
    - Do not declare the same infra in multiple compose files; define it once and let others connect via the shared network/hostname.
    - Optionally guard infra services with Compose `profiles` (e.g., `profiles: ["infra"]`) to avoid accidental duplicate startup.
    - Distinguish DB instance vs DB name: All services point to the same Postgres instance (host `postgres`) but must use distinct `POSTGRES_DB` values per service (e.g., `authdb`, `otpdb`, ...). Keep credentials in each service’s `.env`; do not hardcode them in compose.
- Root `docker-compose.yml` aggregates the default `prod` stack via `include` (infra + all services) for quick startup.
  - Manual control: you can still target specific subsets with explicit files, e.g., `docker compose -f infrastructure/prod/postgres/docker-compose.yml -f infrastructure/prod/redis/docker-compose.yml -f <Service>/docker-compose.yml up --build`.

Microservice -” Acceptance Checklist (AI must verify)
- [] Four projects created under <Service>/<Service>.* with proper references
- [] Program.cs minimal and calls shared extensions
- [] Routing conforms: /v1/api/<Controller>/<Action> with correct casing
- [] Swagger extension configured via shared AddSwaggerExtension/UseSwaggerExtension with appsettings-driven metadata
- [] appsettings.json contains Swagger section aligning with shared extension contract
- [] Root GET (`/`) responds with service info and Swagger URL
- [] FluentValidation validators exist for every request DTO; services register AddValidatorsFromAssemblyContaining<...>(), AddFluentValidationAutoValidation(), and shared AddErrorHandling()/UseErrorHandling() for consistent ProblemDetails responses
- [] Logging goes through shared Logging extension (builder.AddLoggingExtension(), app.UseLoggingExtension()).
- [] Assembly scanning (MediatR, FluentValidation, etc.) uses the project AssemblyMarker instead of arbitrary types.
- [] Appsettings + .env align with contracts; ${ENV} works via shared extensions
- [] Config isolation enforced: per-service `appsettings.json` and `.env` contain only that service's keys (no unused keys like `REDIS_*` when cache isn’t used); cross-service endpoints only in consumer services
- [] Dockerfile موجود در `<Service>/<Service>.Api/Dockerfile` و از sdk/aspnet نسخه 9.0 استفاده می‌کند (همراه با mirror NUGET_SOURCE)
- [] فایل `docker-compose.yml` در ریشه سرویس (`<Service>/docker-compose.yml`) سرویس را روی شبکه `online-shop` کنار وابستگی‌هایش بالا می‌آورد
- [] Build and run succeeds; Swagger is reachable
- [] Documentation updated: `<Service>/README.md` reflects endpoints, configuration, and run instructions; root `README.md` updated if cross‑cutting behavior changed

Customer Service - Blueprint and Rules
- Purpose: surface customer, category, phone, address, and administrative division data; Sepidar is the upstream source while the service maintains a warm cache.
- Controllers: `Customer`, `CustomerCategory`, `Phone`, `Address`, `City`, `Province`, `Country`; routes follow `/v1/api/<Controller>` with PascalCase segments after the prefix. `Phone` and `Address` exclude the `Customer` prefix.
- Features: CQRS handlers cover CRUD + `GetAll`, `GetPaged`, and entity-specific queries (e.g., `GetByPhone`, `GetCitiesByProvince`). Optional fields validate when supplied (PostalCode, NationalId, etc.).
- Anti-corruption: Infrastructure translates Sepidar DTOs into domain models and back; create/update operations refresh cache after successful API calls; unsupported operations throw `BadRequestException` with clear messages.
- Caching & sync: shared `CacheExtension` + `ICacheSnapshotService` نگهداشت snapshot را مدیریت می‌کند؛ `DataSyncExtension` دو جاب گرم‌کن (مشتری/دسته‌بندی و تقسیمات جغرافیایی) را زمان‌بندی می‌کند تا TTL اصلی منقضی نشود.
- Configuration: `.env` فقط شامل `SEPIDAR_BASE_URL` و `ASPNETCORE_URLS` (به‌علاوه‌ی اسرار مانند `CACHE_ENCRYPTION_KEY` در صورت لزوم) است؛ کلیدهای غیرحساس مثل تایم‌اوت، TTL و بازه‌ی جاب‌ها باید در `appsettings` قرار گیرند. `HttpClientRest` حتماً باید `TimeoutSeconds` و پارامترهای retry (`RetryCount`, `RetryBaseDelayMilliseconds`) را تنظیم کند تا رفتار resiliency یکسان باشد.
- Program.cs wiring: load env, add Logging/ErrorHandling/Swagger/Cache/DataSync/HttpClientRest, register `AddCustomerInfrastructure`, MediatR via `AssemblyMarker`, FluentValidation auto-validation, conditional HTTPS, default CORS, `/` root status endpoint.


AI Routine: Canonical Steps and Prompts
- When creating a new extension
  - Ask: Extension name? Required providers? Default provider? Config keys available? Any env names?
  - Create shared/<ExtensionName>/Abstractions|Core|Providers|DependencyInjection.cs
  - Implement Options, Factory, Default provider, concrete providers; bind options with ${ENV} expansion
  - Add csproj (net9.0; Microsoft.Extensions.* 9.0.0) and verify build
  - Write/update docs: `shared/<ExtensionName>/README.md` and root `README.md` (usage, config, defaults)

- When creating a new microservice
  - Ask: Service name? Endpoints? Default SMS/Cache providers? Any external dependencies?
  - Create 4 projects under <Service>/<Service>.* and wire references
  - Configure shared Swagger extension (AddSwaggerExtension/UseSwaggerExtension) and populate appsettings Swagger section
  - Add controllers with [Route("api/[controller]")] and PascalCase actions
  - Register AddErrorHandling()/UseErrorHandling and wire FluentValidation (AddValidatorsFromAssemblyContaining + AddFluentValidationAutoValidation)
  - Add an AssemblyMarker class in the Application project and use it for MediatR/FluentValidation assembly registration
  - Call AddLoggingExtension()/UseLoggingExtension so logging/correlation stay uniform
  - Wire AddCacheExtension/AddSmsExtension; validate configuration and run
  - Ensure docker-compose env_file entries include only the needed shared files (e.g., ../infrastructure/prod/postgres/.env, ../infrastructure/prod/redis/.env) ahead of the service .env so shared infra keys are available.
  - Before wiring shared extensions, verify the service actually uses them; omit AddCacheExtension/AddSmsExtension and related configuration when not needed.
  - Write/update docs: `<Service>/README.md` (endpoints, config, ports, run) and root `README.md` if needed

Don'ts for Agents
- Don't wire providers directly in Program.cs; always use Add<ExtensionName>Extension
- Don't enable global lowercase URLs; only the "api" prefix is lowercase
- Don't add XML summary comments or commented code
- Don't log sensitive values or hardcode secrets


Authentication Service - Blueprint and Rules
- Purpose: OTP-based login, token issuance, refresh/blacklist, user storage
- Endpoints (routes under /v1/api/Auth):
  - POST Send: body { PhoneNumber }; calls OTPService /v1/api/Otp/Send
  - POST Verify: body { PhoneNumber, Code }; if valid ? FindOrCreateByPhone, issue { accessToken, refreshToken }
  - POST Refresh: body { RefreshToken }; rotates and returns new tokens
  - POST Logout: body { RefreshToken }; revokes refresh token and optionally blacklists current access token (via cache)
- Database (PostgreSQL):
  - Config keys: Database:Postgres: { Host, Port, Database, Username, Password }
  - Support ${ENV_VAR} placeholders; build connection string in Infra
  - Entities: User (Id, PhoneNumber, CreatedAt, UpdatedAt), RefreshToken (Id, UserId, Token, ExpiresAt, RevokedAt?)
  - EF Core with Generic Repository + UnitOfWork; EnsureCreated on startup (migrations optional)
- JWT:
  - Config: Jwt: { Issuer, Audience, Secret, AccessTokenMinutes, RefreshTokenDays }
  - Symmetric HMAC; do not log secrets; store/validate refresh tokens in DB
- OTP Integration:
  - Config: Otp:BaseUrl; call /v1/api/Otp/Send and /v1/api/Otp/Verify via HttpClient
- Cache (optional):
  - Use CacheExtension for blacklist/rate-limit; prefer Redis provider
- Program.cs wiring:
  - Env.Load(); Controllers/Swagger; AddCacheExtension if needed; register DbContext (Npgsql), repositories/UoW, OTP HTTP client; MediatR/FluentValidation; CORS; Swagger UI
- Compose:
  - AuthenticationService/docker-compose.yml declares postgres and (if used) redis; include ../OTPService/docker-compose.yml for OTP dependency
- Acceptance (Authentication):
  - [] 4 projects under AuthenticationService/* created and referenced
  - [] DbContext + IRepository<T> + IUnitOfWork implemented
  - [] JWT issuing + refresh rotate + logout implemented
  - [] OTP client calls Send/Verify
  - [] Postgres via env; compose up works
  - [] Swagger reachable; routes casing rules respected
