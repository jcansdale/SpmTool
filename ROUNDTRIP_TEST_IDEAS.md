# Round-Trip Test Ideas

This repository previously contained an ad hoc test file with local-path exploratory code.
The implementation details in that file were environment-specific and not suitable for a
public test suite, but a few underlying test ideas are worth preserving.

## 1. Bulk DX8 -> DX9 -> DX8 -> DX9 round-trip validation

Goal:
Validate that converting a corpus of DX8-compatible models to DX9, back to DX8, and then
to DX9 again produces stable DX9 output.

Suggested structure:

- Maintain a sample corpus of `.spm` files in a documented test-data location.
- Skip known-corrupt files explicitly.
- Assert that:
  - the first DX9 output matches the second DX9 output
  - model names remain valid for the target radio

Why it matters:

- Detects unstable or lossy conversion behavior.
- Exercises realistic end-to-end transformations across a broad sample set.

## 2. Filtered DX8 round-trip validation

Goal:
Validate that `FilterDX8` normalizes input in a way that still preserves stable round-trip
behavior.

Suggested structure:

- Apply `SpmConvert.FilterDX8` to each supported Airplane or Helicopter sample.
- Convert filtered DX8 -> DX9 -> DX8 -> DX9.
- Assert that:
  - filtered DX8 matches the round-tripped DX8
  - first DX9 matches second DX9

Why it matters:

- Gives direct coverage to `FilterDX8`, which currently has little explicit test coverage.
- Confirms that normalization does not introduce drift.

## 3. DX7S-specific round-trip validation

Goal:
Validate the subset of conversions involving DX7S source files.

Suggested structure:

- Maintain a DX7S-labeled sample subset in the test corpus.
- Convert DX7S -> DX9 -> DX8 -> DX9.
- Assert stable DX9 output after the second conversion.

Why it matters:

- DX7S is handled through special-case workflow logic.
- This is a likely regression area when conversion rules change.

## 4. Corpus-driven compatibility checks

Goal:
Use a sample corpus to find inputs that should be rejected or specially handled.

Suggested checks:

- Corrupt file detection via `SpmUtilities.IsCorrupt`
- Unsupported model types
- Overlong model names for target radios
- Generator-specific constraints

Why it matters:

- These checks are often only visible when running against real-world files.
- They are good candidates for metadata-driven integration tests.

## 5. Telemetry file coverage

Goal:
Replace one-off local `TLM` experiments with real tests for `TlmUtilities.DecodeFile`.

Suggested structure:

- Add a small set of representative `.TLM` fixtures if they can be shared.
- Assert expected decoded output or key parsed fields.

Why it matters:

- `TlmUtilities` currently has little or no automated coverage.

## Recommended next step

If a sharable fixture set becomes available, add a dedicated integration test suite that:

- loads samples from a repo-owned fixture directory
- tags corpus tests separately from unit tests
- reports fixture-specific failures clearly

Until then, keep round-trip validation ideas documented rather than encoded as local,
environment-specific test code.