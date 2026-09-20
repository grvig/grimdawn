# Progress

Read this first in every session, then the plan section for the current phase.

## Current phase

Phase 0 — self-scoring feasibility probe. Waiting on HT-1.

## State

| Phase | Status |
| --- | --- |
| 0. Feasibility probe | Built, waiting on HT-1 |
| 1. Input plumbing and safety | Not started |
| 2. Mode state machine and overlay | Not started |
| 3. Grid navigation by calibration | Not started |
| 4. Occupancy detection | Not started |
| 5. Macros and release | Not started |

## Phase 0 checklist

| Task | Status |
| --- | --- |
| 1. Spike console project | Done |
| 2. Window enumeration by observation | Done |
| 3. Injection wrapper, virtual key and scancode | Done |
| 4. Loopback harness and its test | Done, keyboard and mouse verified |
| 5. Self-scoring game probe | Done, smoke-run without the game |
| 6. Fallback ladder | Two automated rungs done, legacy rung outstanding |

## Verified

- The solution builds clean with warnings as errors on .NET 8.0.425.
- Key injection reaches the operating system in both modes: virtual key 0x57
  and scan code 0x11 for W, recorded by a global low-level hook.
- Mouse injection reaches the operating system: a relative move travels the
  right way, an absolute move lands within a pixel, and both buttons send down
  then up.
- Absolute coordinates are correct on a scaled display. This machine runs
  1920×1080 at 125%, and before the DPI fix an absolute move to x=200 landed
  at 250.
- Screen capture returns real pixels. The loopback window paints itself a known
  colour and a capture of its client area comes back that colour.
- Frame difference scoring and the probe verdict are unit-tested.

25 automated tests, all passing.

The probe was smoke-run end to end with no game present, against a window that
happened to be in front. It captured, injected, scored, wrote `SPIKE_RESULTS.md`
and beeped without crashing. That exercises the plumbing only. It says nothing
about Grim Dawn, which is what HT-1 is for.

## Blocked

**HT-1 is outstanding.** The procedure is in [TESTING.md](TESTING.md). Nothing
in Phase 1 depends on the answer: input reading, calibration, deadzones and the
cursor curve are all unaffected, so that work continues while it waits.

## Human tasks

| Task | Phase | Status |
| --- | --- | --- |
| HT-1 Confirm the game accepts synthesized input | 0 | Raised, waiting |
| HT-2 Capture calibration fixtures | 3 | Not yet raised |
| HT-3 Feel tuning | 5 | Not yet raised |

## Next session starts here

Phase 1, which does not depend on HT-1:

1. Evaluate SDL bindings for .NET, pick one that restores cleanly, record the
   choice in `DECISIONS.md`, and put it behind `IGamepadSource`.
2. The kill switch first, before anything presses a key on its own: global
   hotkey, release-all, watchdog thread.
3. Stick calibration and the radial deadzone, as pure maths in `GDPilot.Core`.

Outstanding in Phase 0, to pick up if HT-1 comes back FAIL:

- The legacy `keybd_event` rung of the ladder.
- The probe does not check that its target stayed in front for the whole run.
  If a run looks wrong, that is the first thing to suspect.
