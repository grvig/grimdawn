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
| `ReplayGamepadSource` and trace fixtures | Not started |
| Kill switch, release-all, watchdog | Not started, must land before anything presses keys on its own |
| Focus watcher | Not started |
| Resting stick calibration | Not started |
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
- The radial deadzone zeroes drift, preserves direction, rescales to reach full
  magnitude from the deadzone edge, clamps square-gate corners, and still
  reaches full magnitude with a drifted centre.

38 automated tests, all passing, including from a cold rebuild.

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

1. Resting stick calibration in `GDPilot.Core`: the mean of 500 ms of samples,
   rejected if the stick was plainly being held rather than resting.
2. `ReplayGamepadSource` and the JSON trace format from section 6 of the plan,
   so later mapping work can be tested end to end.
3. Evaluate SDL bindings for .NET, pick one that restores cleanly, record the
   choice here and in `DECISIONS.md`.
4. The kill switch, before anything presses a key on its own.

Outstanding in Phase 0, to pick up if HT-1 comes back FAIL:

- The legacy `keybd_event` rung of the ladder.
- The probe does not check that its target stayed in front for the whole run.
