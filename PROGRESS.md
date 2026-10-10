# Progress

Read this first in every session, then the plan section for the current phase.

## Current phase

Phase 1 — input plumbing and safety, started while HT-1 is outstanding. Nothing
in Phase 1 depends on its answer.

## State

| Phase | Status |
| --- | --- |
| 0. Feasibility probe | Built, waiting on HT-1 |
| 1. Input plumbing and safety | In progress |
| 2. Mode state machine and overlay | Not started |
| 3. Grid navigation by calibration | Not started |
| 4. Occupancy detection | Not started |
| 5. Macros and release | Not started |

## Phase 1 checklist

| Task | Status |
| --- | --- |
| Gamepad snapshot and `IGamepadSource` | Done |
| SDL binding chosen, `SdlGamepadSource` | Not started |
| `ReplayGamepadSource` and the JSON trace format | Done |
| Trace fixtures in `tests/fixtures/traces/` | Not started, arrive with the mapping work |
| Output intents, `IInputSink`, `RecordingSink` | Done |
| Kill switch, release-all, watchdog | `GuardedSink` written and tested, parked in a stash; hotkey and watchdog not started |
| Focus watcher | Not started |
| Resting stick calibration | Done |
| Radial deadzone with edge rescaling | Done |
| Left stick to eight-way WASD | Not started |
| Right stick cursor curve | Not started |
| Button table and chord layer | Not started |

## Verified

- The solution builds clean with warnings as errors on .NET 8.0.425.
- Key and mouse injection reach the operating system, in both key modes.
- Absolute coordinates are correct on this 125% scaled display.
- Screen capture returns real pixels from a known-colour window.
- Frame difference scoring and the probe verdict are unit-tested.
- The radial deadzone zeroes drift, keeps direction, and reaches full magnitude.
- Resting calibration finds a drifted centre and refuses a stick that was held
  over or moved while being sampled.
- Traces parse with named or numeric buttons, default omitted axes to rest, and
  reject out-of-order or out-of-range entries. Replay follows its clock exactly.

51 automated tests, all passing.

## Known issues

**The screen capture loopback test is flaky, and the cause is now identified.**
On 2026-10-10 the test reported what was on screen when it failed: the window
at the loopback window's centre belonged to another application in use at the
time, drawn over the loopback window even though only the loopback window is
topmost. The capture
itself is correct. It returns exactly what is on screen.

Established: launched on its own, the loopback window sits on top and a
`SetWindowPos` raise succeeds. The capture test alone, and with the key tests,
always passed. Failures came only in runs that included the mouse tests, at
rates that swung from five in six to none in eight across one afternoon, with
no code change between.

Not established: why another window can rise above a topmost one during a test
run. The test now raises its window before every capture attempt, which is
correct but not proven to cure anything. A failure now also reports whether
each window is topmost and whether the raise succeeded, so the next one should
settle it.

## Blocked

**HT-1 is outstanding.** The procedure is in [TESTING.md](TESTING.md). Phase 1
continues regardless.

## Human tasks

| Task | Phase | Status |
| --- | --- | --- |
| HT-1 Confirm the game accepts synthesized input | 0 | Raised, waiting |
| HT-2 Capture calibration fixtures | 3 | Not yet raised |
| HT-3 Feel tuning | 5 | Not yet raised |

## Next session starts here

1. `git stash pop`. The stash "Guarded sink with kill switch and its tests" holds
   `GuardedSink` in full and its five tests, all passing. It tracks every held
   key and button, checks the enabled flag under a lock immediately before each
   injection, releases everything on `Disable`, and attempts every release even
   if one fails. It is 122 lines and its tests 99, so it lands as several commits.
2. If the capture test fails again, read its message before changing anything.
3. The global kill switch hotkey and the watchdog, on top of `GuardedSink`.
4. Evaluate SDL bindings for .NET and record the choice.

Outstanding in Phase 0, to pick up if HT-1 comes back FAIL:

- The legacy `keybd_event` rung of the ladder.
- The probe does not check that its target stayed in front for the whole run.
