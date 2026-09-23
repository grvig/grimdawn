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

## 2026-09-13 — Loopback swallows what it records

The loopback window's hooks log injected events and then block them from going
further. Without that, every test run would type and click into whatever window
had focus, typically the editor running the tests. Events not flagged as
injected are neither logged nor blocked, so the person at the machine keeps a
working keyboard and mouse while the suite runs.

## 2026-09-13 — Full project reference to the loopback executable

The integration tests first referenced `GDPilot.Loopback` with
`ReferenceOutputAssembly="false"`, only to force build order. That copies the
executable's launcher into the test output but not the assembly it launches, so
the window exited instantly with "application to execute does not exist" and
every test timed out. A plain project reference copies both. The fixture now
launches the executable from beside its own assembly.

## 2026-09-13 — One output layout for every project

Projects added while `Directory.Build.props` declared `<Platforms>x64</Platforms>`
were mapped to `Debug|x64` in the solution, and earlier ones to `Any CPU`, so
build output was split between `bin\Debug` and `bin\x64\Debug`. Removed the
`Platforms` property and mapped every project to `Any CPU`. `PlatformTarget`
still pins the binaries to x64, which is what the P/Invoke layer needs.

## 2026-09-19 — Per-monitor DPI awareness for anything sending absolute input

An absolute mouse move to x=200 landed at x=250 in the loopback test. This
machine runs 1920×1080 at 125% scaling, and a process that has not declared
DPI awareness is shown a virtual 1536×864 screen, so `GetSystemMetrics` and
`GetCursorPos` are off by the scale factor. Grid navigation clicks computed cell
centres, so it would have missed every cell by a quarter. Added
`DpiAwareness.EnablePerMonitor()` in `GDPilot.Output`. Every process that
reads or sends absolute coordinates calls it before creating a window: the test
fixture now, the spike and the app when they do.

## 2026-09-19 — One shared loopback window for all loopback tests

A low-level hook sees injected input from every process on the machine, not
only from its own test. xUnit runs test classes in parallel by default, so a
key test class and a mouse test class each with their own loopback window would
record each other's events. Both classes now belong to one xUnit collection
that owns a single loopback window, which also makes them run one after another.

## 2026-09-20 — The probe takes the foreground window

Section 8 forbids hardcoding the game's window title or executable name. The
probe counts down from five and takes whatever window is in front when it
reaches zero, so the person running it identifies the game by bringing it
forward. The report names the target it measured, which is also the check that
the run is meaningful: a target line naming something else means the alt-tab
missed and the scores describe the wrong window.

## 2026-09-20 — Capture from the screen, not the window

`ScreenCapture.Region` copies from the screen device context rather than the
game window's own. A game drawing through DirectX leaves its window context
empty, so a window capture would return blank frames and every probe would
score zero, which looks identical to input being ignored. The composited screen
always holds the pixels. `CAPTUREBLT` is set so a layered window over the
region is included rather than skipped.

## 2026-09-20 — The ladder automates two rungs, not four

`FeasibilityLadder` climbs the two rungs that are a code change: virtual key
codes, then scan codes. An inconclusive rung is retried once with a doubled
hold before moving on. The remaining rungs in section 8 are not code: running
the game windowed rather than borderless, and running elevated. Both are
printed as guidance when every automated rung fails. The legacy `keybd_event`
rung is not built yet.

## 2026-09-23 — Positive Y is up in every stick value

Hardware disagrees about stick Y: SDL reports down as positive, XInput reports
up as positive. `StickPosition` fixes up as positive, the maths convention, so
angles from `Atan2` mean what they look like and the movement and cursor maths
never need to know which pad produced the value. Flipping is the job of the
source adapter, once.

## 2026-09-23 — Deadzone rescaling assumes the worst-case drift

A drifted centre makes the throw shorter on the side the stick leans towards.
Rescaling uses the whole drift magnitude as lost travel, in every direction. A
full push towards the drift still reaches magnitude 1, but a push away from it
saturates a little early. Per-direction reach would be exact but needs the
stick's real gate shape, which calibration does not measure. Early saturation on
one side is invisible in play. Never reaching full speed on one side is not.

## 2026-09-23 — Capture test waits for the first paint

The capture test failed once on a cold rebuild and passed on every rerun. The
loopback window writes "ready" when it is shown, which is before its first
paint reaches the screen, and a cold start widens that gap enough to capture
whatever was underneath. The test now retries the capture for up to two
seconds until the window's colour appears. It still fails if the colour never
appears, so it still catches a broken capture.
