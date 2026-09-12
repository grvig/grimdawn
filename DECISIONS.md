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

## 2026-09-12 — Put key identity in Core, injection in Output

Section 5 gives `GDPilot.Output` the SendInput wrappers and forbids `Core` from
referencing it. A binding table has to name keys, so the `KeyCode` enumeration
is pure data and lives in `Core`; only the P/Invoke that acts on it lives in
`Output`. The enum's numeric values are the Windows virtual key codes, which
lets the injector cast rather than translate.

## 2026-09-12 — Centralised build settings

Added `Directory.Build.props` at the root rather than repeating properties in
every project file. It turns on nullable reference types, implicit usings and
warnings as errors, and pins every project to x64. Warnings as errors is the
part worth keeping: this project is mostly interop, where a warning is usually
a real marshalling mistake.
