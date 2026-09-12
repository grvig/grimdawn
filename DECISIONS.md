# Decisions

Append an entry whenever something the plan left open gets resolved: a package
choice, a fallback taken, a constraint discovered on the machine. Newest last.

## 2026-09-12 — Installed the .NET 8 SDK

The plan assumes a .NET 8 SDK. The machine had no SDK of any version, so
`dotnet build` could not have run. Installed `Microsoft.DotNet.SDK.8` through
winget, which put 8.0.425 in `C:\Program Files\dotnet`. No change to the plan,
only to the machine.

## 2026-09-12 — Kept spike output untracked

Section 8 has the spike write its verdict to `SPIKE_RESULTS.md`. That file is a
per-run artefact tied to one machine, one resolution and one game build, and it
is rewritten on every run, so it is in `.gitignore`. The verdict that matters
gets copied into `PROGRESS.md` by hand, where it is a durable record rather than
a build output.
