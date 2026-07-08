# Research: Foundation for Future Integrations

## Decision 1: Use the standard .NET Worker host bootstrap

- **Decision**: Build the baseline around `Host.CreateDefaultBuilder(args)` with explicit `appsettings.json` inclusion and hosted service registration.
- **Rationale**: This matches the requested runtime model, keeps startup conventional, and minimizes custom bootstrapping risk for a service that currently needs only configuration, logging, DI, and a background loop.
- **Alternatives considered**:
  - Minimal custom host wiring: rejected because it adds no value for this baseline and weakens convention-based maintainability.
  - Console app with manual loop: rejected because it bypasses the Worker Service lifecycle the feature explicitly requires.

## Decision 2: Treat the execution interval as validated options

- **Decision**: Represent the recurring execution interval as strongly typed configuration validated at startup, with no fallback default.
- **Rationale**: The clarify phase established that missing interval values must fail clearly instead of being silently defaulted. Strongly typed options keep the requirement testable and isolate schedule semantics from the worker loop.
- **Alternatives considered**:
  - Read raw configuration values directly inside `Worker`: rejected because it spreads validation into runtime flow and makes startup failures less deterministic.
  - Hard-code a daily interval: rejected because clarification explicitly disallowed implicit defaults.

## Decision 3: Keep Infrastructure directories structurally reserved only

- **Decision**: Do not create per-area placeholder services or contracts under `Infrastructure/Storage`, `Infrastructure/Automation`, or `Infrastructure/Email` in this increment.
- **Rationale**: The chosen clarification favors a lean baseline that shows boundaries without implying approved implementations. Central registration comments are enough to communicate where future adapters will be wired.
- **Alternatives considered**:
  - Empty marker classes per area: rejected because they add maintenance noise without current behavior.
  - Placeholder interfaces and service implementations: rejected because they prematurely freeze abstractions before real integration requirements exist.

## Decision 4: Make observability explicit without adding business telemetry pipelines

- **Decision**: The baseline should include clear seams for logs, metrics, and tracing, with immediate logging behavior and reserved wiring points for metrics and tracing expansion.
- **Rationale**: Clarification raised observability from optional guidance to a baseline expectation. The design must therefore make these concerns visible now, even though no domain metrics or distributed trace exporters are needed yet.
- **Alternatives considered**:
  - Logs only: rejected because it no longer satisfies the clarified expectation.
  - Full telemetry stack implementation now: rejected because the feature still forbids expanding into non-structural business integration work.

## Decision 5: Defer active security mechanics

- **Decision**: Avoid authentication, authorization, and secret-management behaviors in this increment unless required for local host startup.
- **Rationale**: The baseline has no active external integrations yet. Introducing security mechanisms now would add speculative scope and produce abstractions disconnected from current behavior.
- **Alternatives considered**:
  - Add secret-provider abstractions now: rejected because no active secrets are consumed in this increment.
  - Add auth middleware or identity concepts: rejected because a Worker Service baseline with no external endpoints does not need them.

## Decision 6: Preserve a root-level single-project layout

- **Decision**: Keep a single project at repository root with architectural folders beneath it, rather than splitting into multiple assemblies in the first increment.
- **Rationale**: The requested structure is organizational, not assembly-driven. A single project compiles faster, reduces boilerplate, and still preserves clean boundaries through folders and DI seams.
- **Alternatives considered**:
  - Separate class libraries per layer: rejected because it increases setup and package references before any real cross-layer complexity exists.
  - Nested `src/` project folder: rejected because the requested tree is already root-oriented and the repository is otherwise empty.
