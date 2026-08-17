# Third-party source notices

ZeepSDK vendors modified source so it can run inside Unity's/BepInEx's constrained
runtime. `scripts/Verify-VendoredSources.ps1` verifies current vendored trees
against `vendor-manifest.json`; update provenance and expected hash in same review
whenever a tree changes.

| Component | Local source | Upstream base | Local import | License | Local changes |
| --- | --- | --- | --- | --- | --- |
| UniTask | `ZeepSDK/External/UniTask` | [2.3.3 / `b992a061`](https://github.com/Cysharp/UniTask/tree/b992a061fbf68ff1eb475d0837065b0549ed31ba) | `56071e40` | MIT | Namespace relocated under `ZeepSDK.External`; Unity package metadata and unused platform integrations omitted; warning suppressions and project compatibility edits applied. |
| FluentResults | `ZeepSDK/External/FluentResults` | [3.15.2 / `8b675de7`](https://github.com/altmann/FluentResults/tree/8b675de7e8b6e4c0921e95138c99e15fd52cde40) | `36154e06` | MIT | Namespace relocated under `ZeepSDK.External`; logging integration and unused project files omitted; project compatibility edits applied. |
| Newtonsoft.Json for Unity Converters | `ZeepSDK/External/UnityConverters` | [1.5.1 / `024301cc`](https://github.com/applejag/Newtonsoft.Json-for-Unity.Converters/tree/024301ccea513156fcf89c8ce2be21b4137b4028) | `c9de299` | MIT | Namespace relocated under `ZeepSDK.External`; selected converters vendored; `ValuesArray<T>` visibility and project compatibility adjusted. |
| Bugsnag Unity | `ZeepSDK/External/BugSnag` | [8.5.2 / `26497bb8`](https://github.com/bugsnag/bugsnag-unity/tree/26497bb8d921e820efa8af04ac4e093c01c680d0) | `9ec26c0` | MIT | Runtime subset vendored; namespace relocated under `ZeepSDK.External`; external projects consolidated and Unity/BepInEx compatibility and telemetry behavior adjusted. |

Upstream bases were reconstructed by comparing imported source with upstream
tags because original import commits did not record version metadata. Pinned
commit IDs make future comparisons reproducible.

Full license texts live in `THIRD_PARTY_LICENSES`.
