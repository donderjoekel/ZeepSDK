# ZeepSDK Code Review

Review date: 2026-07-10  
Baseline: `c894ddc`  
Scope: security, correctness, performance, dead code, and code smells

## Coverage

- Inventory: 517 tracked C# and project-support files.
- Full manual review: 13 first-party files covering telemetry, networking, storage, playlists, scripting, and chat commands.
- Targeted static review: remaining first-party code, using searches for filesystem, network, event, reflection, allocation, and lifecycle risks.
- Automated coverage: full solution build, test suite, package vulnerability audit, and diff validation.
- Deferred full-file manual review: 504 files, mainly 299 vendored `External` files plus generated, test, and lower-risk first-party code. These files received compiler, test, dependency-audit, or targeted-search coverage only.

## Fixed findings, ranked

| Rank | Severity | Finding and impact | Fix | Commit |
|---:|---|---|---|---|
| 1 | High | Crash telemetry started without consent. Player data and exceptions could leave process before explicit opt-in. | Default telemetry off; persist explicit consent; veto sends and pause sessions after revocation; harden metadata and teardown paths. | `575ab7b` |
| 2 | High | Playlist names could escape playlist directory through rooted paths or traversal. Existing files could be overwritten outside intended storage. | Canonicalize paths, enforce root containment, reject device names and invalid names, create only contained directory. | `70a1372` |
| 3 | High | `UnityFolder` accepted destination paths outside configured Unity folder. Copy/move APIs could overwrite arbitrary writable files. | Resolve canonical destination, enforce containment, and correct overwrite move behavior. | `b3522a0` |
| 4 | High | Lua scripts could run forever on Unity thread. Infinite loops caused permanent frame hangs. | Execute through instruction-budgeted coroutines; cap slices and script size; apply budget to top-level code and callbacks. | `5972ed9` |
| 5 | Medium | Remote chat commands had no abuse control. Players could trigger unbounded parsing, callbacks, and response traffic. | Add per-player token-bucket throttling, input-size cap, bounded state cleanup, and stop after first command match. | `7636563` |
| 6 | Medium | Version endpoint response was read without time or size bounds. Slow or large responses could retain resources or exhaust memory. | Reuse timeout-bound client; stream with 2 MiB limit; cap JSON depth; add null guards and linear lookup map. | `1a3377c` |
| 7 | Medium | Persistent JSON/blob writes were non-atomic. Crash or interruption could corrupt configuration and saved data. | Write same-directory temporary file, then replace/move atomically with cleanup. | `0ae9a61` |
| 8 | Medium | Playlist discovery loaded every matching file without count, size, or JSON-depth limits. Large libraries could stall startup or exhaust memory. | Cap file count at 1,000, each file at 2 MiB, JSON depth at 64, and ignore invalid/null entries safely. | `00fb45d` |
| 9 | Medium | Failed Lua initialization left event subscriptions active. Repeated failures accumulated callbacks and retained script state. | Unsubscribe partial initialization and make unload cleanup idempotent. | `0d32055` |
| 10 | Medium | Generated Lua events were broadcast across every loaded script. N scripts and M events produced N x M callbacks and cross-script delivery. | Bind generated events to owning `Zua` instance and subscriber. | `aa8ce99` |
| 11 | Low | Chat parser used regex for simple prefix/token parsing and accepted ambiguous delimiter matches. Hot chat traffic paid avoidable regex cost. | Replace regex with ordinal prefix checks and exact whitespace delimiter parsing. | `292fe81` |
| 12 | Low | Source generator interpolated secret text into C# without literal escaping. Quotes or control characters broke generated source. | Emit Roslyn-escaped string literals. | `b037c97` |
| 13 | Low | Communication receiver equality compared incompatible objects, breaking lookup/removal behavior. | Implement receiver-to-receiver value equality. | `403e017` |
| 14 | Low | UI border drawing allocated a new corner array during every `OnGUI` pass. | Reuse one readonly four-element buffer. | `13886c6` |
| 15 | Low | Generator symbol fields declared impossible non-null state and produced nullable warnings. | Preserve symbol nullability until initialization checks complete. | `a4ee1a1` |
| 16 | Informational | Large obsolete commented blocks obscured active behavior and increased maintenance cost. | Remove dead commented implementation. | `7977d82` |

## Remediated follow-up findings, ranked

| Rank | Finding | Fix commit |
|---:|---|---|
| 1 | Version-check lifecycle cancellation | `737b693` |
| 2 | mod.io pagination traversal and bounds | `aa957fb` |
| 3 | Bounded help response | `65d085d` |
| 4 | Subsystem event teardown | `06be7e0` |
| 5 | Chat registry invariants and snapshots | `738e966` |
| 6 | Canonical Lua script index | `a985845` |
| 7 | Cached Lua API descriptors | `5975a60` |
| 8 | Side-effect-free storage reads and typed results | `a485f16` |
| 9 | Indexed playlists and atomic rename | `bed41bb` |
| 10 | Mutation-safe condition ticks | `5e57249` |
| 11 | Vendored source provenance and drift checks | `2bd7938` |
| 12 | Zero-warning build enforcement | `abb379d` |

### 1. Version-check lifecycle had no cancellation

`VersionChecker.CheckVersions` is fire-and-forget, while `WaitForScene` polls and temporarily subscribes to `SceneManager.sceneLoaded`. Plugin unload or a scene that never appears can retain work and handlers.

Fix: pass plugin-destruction `CancellationToken`; replace polling with cancellation-aware scene await; unsubscribe in `finally`; return `UniTask` so caller can observe failures.

### 2. mod.io response model exposed pagination but caller did not traverse it

Only one response page is considered. Installed mods beyond first page can be missed, producing incomplete version results.

Fix: follow `offset`, `limit`, `result_count`, and `result_total`; enforce page/item caps; stop on repeated or empty pages; test multi-page and malformed metadata.

### 3. Help command amplified one request into many network messages

`HelpRemoteChatCommand` sends header plus one message per command. Rate limiting bounds invocations but not responses per accepted invocation.

Fix: build one length-capped response, paginate only when protocol requires it, and impose maximum advertised command count.

### 4. Static event ownership was inconsistent

Several APIs subscribe static or long-lived handlers without a uniform shutdown contract. BepInEx reloads can retain objects or duplicate callbacks.

Fix: make each subsystem `IDisposable`; centralize registration in plugin startup; dispose in reverse order from `OnDestroy`; add reload tests checking one callback per event.

### 5. Chat registry exposed mutable storage and accepted weak registrations

An `IReadOnlyList` backed by `List<T>` remains downcastable. Duplicate names, null commands, and invalid names can make dispatch ambiguous.

Fix: validate command/name/callback; enforce ordinal unique names; expose `ReadOnlyCollection<T>` or immutable snapshot; defer mutation during dispatch.

### 6. Script name lookup recursively scanned plugin tree

`ScriptingApi` uses `Directory.GetFiles(..., SearchOption.AllDirectories)` for load and unload. Each call allocates full result arrays, applies wildcard semantics, and can load same physical file through differently cased paths.

Fix: index scripts once with canonical paths and `OrdinalIgnoreCase`; use literal filenames; invalidate index on explicit refresh; stop enumeration after ambiguity detected.

### 7. Lua API discovery repeated whole-assembly reflection

Each script scans `typeof(Zua).Assembly.GetTypes()` multiple times and uses reflection/activation for event and API discovery.

Fix: build immutable descriptor cache once; preferably generate registry at compile time; surface activation errors per API without rescanning.

### 8. Storage reads created directories and performed blocking I/O

`ModStorage.CreatePath` creates directories even for exists/read/delete operations. APIs perform synchronous disk I/O on caller, commonly Unity main thread, and convert failures to broad fallback values.

Fix: split pure `ResolvePath` from write-only `EnsureDirectory`; add async or scheduled APIs for large blobs; return typed result/error details; lock converter registration or freeze it after startup.

### 9. Playlist queries repeatedly rescanned and exposed mutable state

`Exists` and `GetPlaylist` repeatedly search loaded playlists. Mutable level collections and rename/save behavior can drift from disk state.

Fix: maintain ordinal dictionary keyed by validated name; expose read-only collections; define rename as atomic new-write plus old-delete; derive counts from collection.

### 10. `ConditionTicker` was vulnerable to mutation during iteration

Tick callbacks can add/remove tickables while `foreach` is active. One callback exception can abort remaining tickables for that phase.

Fix: queue mutations until phase completes or iterate stable snapshots; isolate and log callback exceptions; document ordering.

### 11. Vendored source had weak provenance controls

`ZeepSDK/External` contains 299 tracked source files. Package audit cannot verify copied source origin or detect upstream security releases.

Fix: record upstream project, version/commit, license, and local patch set; automate upstream comparison; prefer pinned package references where Unity compatibility permits.

### 12. Warning volume hid new defects

Full build succeeds but emits 53 warnings, mostly missing XML documentation and vendored/generated code. Persistent noise reduces signal from new warnings.

Fix: suppress documentation warnings only for vendor/generated scopes; fix first-party warnings; add CI warning baseline and reject net-new warnings.

## Verification

- `dotnet test ZeepSDK.Tests/ZeepSDK.Tests.csproj --no-restore`
- `dotnet build ZeepSDK.sln --no-restore`
- `dotnet list ZeepSDK/ZeepSDK.csproj package --vulnerable --include-transitive`
- `git diff --check`

Package audit found no known vulnerable direct or transitive NuGet packages in configured sources. This does not cover copied source under `ZeepSDK/External`.

Follow-up verification after remediation: 84 tests pass and full solution build
completes with zero warnings. `Directory.Build.props` treats future compiler
warnings as errors. Vendored source is covered separately by
`scripts/Verify-VendoredSources.ps1` and `vendor-manifest.json`.
