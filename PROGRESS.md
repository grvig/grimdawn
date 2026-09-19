# Progress

Read this first in every session, then the plan section for the current phase.

## Current phase

Phase 0 — self-scoring feasibility probe.

## State

| Phase | Status |
| --- | --- |
| 0. Feasibility probe | In progress |
| 1. Input plumbing and safety | Not started |
| 2. Mode state machine and overlay | Not started |
| 3. Grid navigation by calibration | Not started |
| 4. Occupancy detection | Not started |
| 5. Macros and release | Not started |

## Phase 0 checklist

| Task | Status |
| --- | --- |
| 1. Spike console project | Done |
| 2. Window enumeration by observation | Done, smoke-run against a live desktop |
| 3. Injection wrapper, virtual key and scancode | Done |
| 4. Loopback harness and its test | Done, keyboard and mouse verified |
| 5. Self-scoring game probe | Scoring done and tested, capture and probe next |
| 6. Fallback ladder | Not started |

## Verified

- The solution builds clean with warnings as errors on .NET 8.0.425.
- `GDPilot.Spike --windows` lists visible top-level windows with their process
  names. Confirmed by running it. Grim Dawn was not running, so the game window
  itself is still unidentified.
- Key injection reaches the operating system in both modes: virtual key 0x57
  and scan code 0x11 for W, recorded by a global low-level hook.
- Mouse injection reaches the operating system: a relative move travels the
  right way, an absolute move lands within a pixel of its target, and both
  buttons send down then up.
- Absolute coordinates are correct on a scaled display. This machine runs
  1920×1080 at 125%, and before the DPI fix an absolute move to x=200 landed
  at 250.
- Frame difference scoring and the probe verdict are unit-tested.

21 automated tests, all passing.

## Blocked

Nothing. HT-1 is not yet raised because the probe it depends on is not built.

## Human tasks

| Task | Phase | Status |
| --- | --- | --- |
| HT-1 Confirm the game accepts synthesized input | 0 | Not yet raised |
| HT-2 Capture calibration fixtures | 3 | Not yet raised |
| HT-3 Feel tuning | 5 | Not yet raised |

Procedures live in [TESTING.md](TESTING.md).

## Next session starts here

1. Screen capture of a screen rectangle through GDI `BitBlt`, in a new
   `GDPilot.Vision` project, since Phase 4 needs the same capture. Capture from
   the screen rather than the window, because a borderless game draws through
   DirectX and only the composited screen reliably holds its pixels.
2. Test it by giving the loopback window a fixed, distinctive background colour
   and asserting that a capture of its client area comes back that colour.
3. The probe in the spike: locate the game window, idle capture pair, one
   second of forward movement, capture pair, `ProbeVerdict`, cursor delta
   check, write `SPIKE_RESULTS.md`. The spike must call
   `DpiAwareness.EnablePerMonitor()` first.
4. The fallback ladder, then raise HT-1.
