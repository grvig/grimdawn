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
| Kill switch, release-all, watchdog | Not started, must land before anything presses keys on its own |
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

**The screen capture loopback test is intermittently flaky.** It has failed twice
in roughly thirty runs, and every rerun after a failure passed. Ruled out so far:
a cold start after a rebuild, which reproduced cleanly twice with no failure, and
the window cascading off-screen, since Windows wraps it back to the top left
well before the edge. The two-second retry added on 2026-09-23 did not cure it.
The remaining suspect, with no evidence yet, is another topmost window briefly
drawing over that spot. The NVIDIA overlay is one candidate. The next step is
diagnosis rather than another guess: on failure, report which window
`WindowFromPoint` finds at the client centre and the most common captured
colour.

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

1. Make the capture test diagnose itself on failure, as described above.
2. Evaluate SDL bindings for .NET, pick one that restores cleanly, record the
   choice in `DECISIONS.md`, and put it behind `IGamepadSource`.
3. The kill switch, before anything presses a key on its own.

Outstanding in Phase 0, to pick up if HT-1 comes back FAIL:

- The legacy `keybd_event` rung of the ladder.
- The probe does not check that its target stayed in front for the whole run.
