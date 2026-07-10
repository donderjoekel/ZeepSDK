[CmdletBinding()]
param(
    [string] $ManifestPath = (Join-Path $PSScriptRoot "..\vendor-manifest.json")
)

$ErrorActionPreference = "Stop"

function Get-TreeSha256 {
    param([Parameter(Mandatory)][string] $Path)

    $root = (Resolve-Path -LiteralPath $Path).Path
    $lines = Get-ChildItem -LiteralPath $root -Recurse -File |
        Sort-Object { $_.FullName.Substring($root.Length + 1).Replace("\", "/") } |
        ForEach-Object {
            $relativePath = $_.FullName.Substring($root.Length + 1).Replace("\", "/")
            $fileHash = (Get-FileHash -LiteralPath $_.FullName -Algorithm SHA256).Hash.ToLowerInvariant()
            "$relativePath`t$fileHash"
        }

    $payload = [Text.Encoding]::UTF8.GetBytes(($lines -join "`n") + "`n")
    $sha256 = [Security.Cryptography.SHA256]::Create()
    try {
        return ([BitConverter]::ToString($sha256.ComputeHash($payload))).Replace("-", "").ToLowerInvariant()
    }
    finally {
        $sha256.Dispose()
    }
}

$resolvedManifest = (Resolve-Path -LiteralPath $ManifestPath).Path
$repositoryRoot = Split-Path -Parent $resolvedManifest
$manifest = Get-Content -LiteralPath $resolvedManifest -Raw | ConvertFrom-Json
$failed = $false

foreach ($component in $manifest.components) {
    if ($component.revision -notmatch "^[0-9a-f]{40}$") {
        Write-Error "$($component.name): revision must be a full 40-character commit SHA"
    }

    $componentPath = Join-Path $repositoryRoot $component.path
    $actualHash = Get-TreeSha256 -Path $componentPath
    if ($actualHash -ne $component.treeSha256) {
        Write-Error -ErrorAction Continue "$($component.name): vendored tree changed. Expected $($component.treeSha256), got $actualHash. Review against $($component.repository)@$($component.revision), then update provenance and hash together."
        $failed = $true
        continue
    }

    Write-Host "$($component.name): verified $actualHash"
}

if ($failed) {
    exit 1
}
