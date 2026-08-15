<#
.SYNOPSIS
    Live view of a Claude Code Workflow run: which subagents are alive, what each is doing right now,
    and what they finally returned.

.DESCRIPTION
    A workflow writes one transcript directory per run:

        <project>\<session-id>\subagents\workflows\<run-id>\
            journal.jsonl                 one line per agent lifecycle event
            agent-<id>.jsonl              the full transcript of that subagent
            agent-<id>.meta.json          agent type / spawn depth

    This script reads those files and renders a refreshing dashboard. It never writes anything and
    opens every file with FileShare::ReadWrite, so it is safe to run against a workflow in flight.

.PARAMETER RunId
    The run id reported when the workflow was launched, e.g. wf_bae7a900-0ac.
    If omitted, the most recently modified run directory is used.

.PARAMETER Root
    The subagents\workflows directory to look in. Defaults to the current session's.

.PARAMETER IntervalSeconds
    Refresh period. Default 5.

.PARAMETER Once
    Render a single snapshot and exit, instead of looping.

.PARAMETER Detail
    Also print the tail of each agent's reasoning/tool stream, not just its last action.

.EXAMPLE
    .\Watch-Workflow.ps1
    .\Watch-Workflow.ps1 -RunId wf_bae7a900-0ac -IntervalSeconds 3
    .\Watch-Workflow.ps1 -Once -Detail
#>

[CmdletBinding()]
param(
    [string]$RunId,
    [string]$Root = "$env:USERPROFILE\.claude\projects\d--CommonLibrary-Net4x-GtkWindowsForms\5aa9d250-b202-4566-8adc-4886e9f3bea9\subagents\workflows",
    [int]$IntervalSeconds = 5,
    [switch]$Once,
    [switch]$Detail
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Resolve-RunDirectory {
    param([string]$Root, [string]$RunId)

    if (-not (Test-Path -LiteralPath $Root)) {
        throw "Workflow root not found: $Root"
    }

    if ($RunId) {
        $dir = Join-Path $Root $RunId
        if (-not (Test-Path -LiteralPath $dir)) { throw "No such run: $dir" }
        return (Get-Item -LiteralPath $dir)
    }

    $latest = Get-ChildItem -LiteralPath $Root -Directory |
        Sort-Object LastWriteTime -Descending |
        Select-Object -First 1

    if (-not $latest) { throw "No workflow runs under $Root" }
    return $latest
}

# The writer still holds these files open, so a plain Get-Content can hit a sharing violation.
function Read-SharedLines {
    param([string]$Path)

    $stream = $null
    $reader = $null
    try {
        $stream = [System.IO.FileStream]::new(
            $Path,
            [System.IO.FileMode]::Open,
            [System.IO.FileAccess]::Read,
            [System.IO.FileShare]::ReadWrite)
        $reader = [System.IO.StreamReader]::new($stream)
        return $reader.ReadToEnd() -split "`r?`n" | Where-Object { $_ }
    }
    catch {
        return @()
    }
    finally {
        if ($reader) { $reader.Dispose() }
        elseif ($stream) { $stream.Dispose() }
    }
}

function ConvertFrom-JsonSafe {
    param([string]$Line)
    try { return $Line | ConvertFrom-Json } catch { return $null }
}

# The workflow's own label for an agent is not stored in the journal, so recover a useful name from the
# opening prompt: these prompts name the difference id and say whether the agent diagnoses or implements.
function Get-AgentLabel {
    param([string]$FirstLine)

    $first = ConvertFrom-JsonSafe $FirstLine
    if (-not $first) { return 'unknown' }

    $text = [string]$first.message.content
    $id = if ($text -match 'difference\s+(D[0-9\-/]+)') { $Matches[1] } else { '?' }
    $kind =
        if ($text -match 'IMPLEMENT AND VERIFY') { 'implement' }
        elseif ($text -match 'DIAGNOSE ONLY') { 'diagnose' }
        else { 'agent' }

    return "$kind`:$id"
}

function Get-Excerpt {
    param([string]$Text, [int]$Max = 96)

    if (-not $Text) { return '' }
    $flat = ($Text -replace '\s+', ' ').Trim()
    if ($flat.Length -le $Max) { return $flat }
    return $flat.Substring(0, $Max - 1) + [char]0x2026
}

# Walk backwards to the newest thing the agent actually did, and describe it in one line.
function Get-LastAction {
    param([string[]]$Lines)

    for ($i = $Lines.Count - 1; $i -ge 0; $i--) {
        $entry = ConvertFrom-JsonSafe $Lines[$i]
        if (-not $entry -or $entry.type -ne 'assistant') { continue }

        $blocks = @($entry.message.content)
        for ($b = $blocks.Count - 1; $b -ge 0; $b--) {
            $block = $blocks[$b]
            if (-not $block -or -not $block.PSObject.Properties['type']) { continue }

            switch ($block.type) {
                'tool_use' {
                    $arg = ''
                    if ($block.PSObject.Properties['input'] -and $block.input) {
                        foreach ($key in 'file_path', 'command', 'pattern', 'prompt') {
                            if ($block.input.PSObject.Properties[$key]) {
                                $arg = Get-Excerpt ([string]$block.input.$key) 70
                                break
                            }
                        }
                    }
                    return [pscustomobject]@{
                        Kind = $block.name
                        Text = $arg
                        When = $entry.timestamp
                    }
                }
                'text' {
                    return [pscustomobject]@{
                        Kind = 'says'
                        Text = Get-Excerpt ([string]$block.text) 70
                        When = $entry.timestamp
                    }
                }
                'thinking' {
                    return [pscustomobject]@{
                        Kind = 'thinking'
                        Text = Get-Excerpt ([string]$block.thinking) 70
                        When = $entry.timestamp
                    }
                }
            }
        }
    }

    return [pscustomobject]@{ Kind = 'starting'; Text = ''; When = $null }
}

function Get-AgentSummary {
    param([System.IO.FileInfo]$File, [hashtable]$Status)

    $lines = Read-SharedLines $File.FullName
    if ($lines.Count -eq 0) { return $null }

    $id = $File.BaseName -replace '^agent-', ''
    $toolCalls = 0
    foreach ($line in $lines) {
        if ($line.Contains('"type":"tool_use"')) { $toolCalls++ }
    }

    $last = Get-LastAction $lines
    $age = ''
    if ($last.When) {
        $age = '{0,5:n0}s' -f ([datetime]::UtcNow - [datetime]$last.When).TotalSeconds
    }

    return [pscustomobject]@{
        Agent   = Get-AgentLabel $lines[0]
        Status  = if ($Status.ContainsKey($id)) { $Status[$id] } else { 'running' }
        Turns   = $lines.Count
        Tools   = $toolCalls
        Idle    = $age
        MB      = '{0,5:n1}' -f ($File.Length / 1MB)
        Doing   = if ($last.Text) { "$($last.Kind) $($last.Text)" } else { $last.Kind }
        Id      = $id
        Updated = $File.LastWriteTime
    }
}

function Show-Dashboard {
    param([System.IO.DirectoryInfo]$RunDir)

    # journal.jsonl is the authority on lifecycle; agent transcripts only tell you about activity.
    $status = @{}
    $results = [System.Collections.Generic.List[object]]::new()
    $journalPath = Join-Path $RunDir.FullName 'journal.jsonl'

    foreach ($line in (Read-SharedLines $journalPath)) {
        $event = ConvertFrom-JsonSafe $line
        if (-not $event -or -not $event.PSObject.Properties['agentId']) { continue }

        $status[$event.agentId] = switch ($event.type) {
            'started'   { 'running' }
            'completed' { 'done' }
            'failed'    { 'FAILED' }
            default     { [string]$event.type }
        }

        if ($event.type -eq 'completed' -and $event.PSObject.Properties['result']) {
            $results.Add([pscustomobject]@{
                Agent  = $event.agentId
                Result = Get-Excerpt ($event.result | ConvertTo-Json -Compress -Depth 6) 160
            })
        }
    }

    $agents = Get-ChildItem -LiteralPath $RunDir.FullName -Filter 'agent-*.jsonl' |
        ForEach-Object { Get-AgentSummary $_ $status } |
        Where-Object { $_ } |
        Sort-Object Agent

    $done = @($agents | Where-Object { $_.Status -ne 'running' }).Count

    # Clear-Host throws when there is no real console (redirected output, CI, an agent shell).
    try { Clear-Host } catch { Write-Host '' }

    Write-Host "workflow $($RunDir.Name)" -ForegroundColor Cyan
    Write-Host ("  {0}  |  {1}/{2} agents finished  |  last write {3:HH:mm:ss}" -f
        (Get-Date -Format 'HH:mm:ss'), $done, $agents.Count, $RunDir.LastWriteTime) -ForegroundColor DarkGray
    Write-Host ''

    $agents |
        Format-Table -Property `
            @{ Label = 'Agent'; Expression = 'Agent'; Width = 20 },
            @{ Label = 'State'; Expression = 'Status'; Width = 8 },
            @{ Label = 'Turns'; Expression = 'Turns'; Width = 6 },
            @{ Label = 'Tools'; Expression = 'Tools'; Width = 6 },
            @{ Label = 'Idle'; Expression = 'Idle'; Width = 7 },
            @{ Label = 'Log MB'; Expression = 'MB'; Width = 7 },
            @{ Label = 'Currently'; Expression = 'Doing' } |
        Out-Host

    if ($results.Count -gt 0) {
        Write-Host 'Returned:' -ForegroundColor Green
        foreach ($r in $results) { Write-Host "  $($r.Agent)  $($r.Result)" -ForegroundColor DarkGray }
        Write-Host ''
    }

    if ($Detail) {
        foreach ($agent in $agents) {
            Write-Host "--- $($agent.Agent)  [$($agent.Id)] ---" -ForegroundColor Yellow
            $lines = Read-SharedLines (Join-Path $RunDir.FullName "agent-$($agent.Id).jsonl")
            $tail = $lines | Select-Object -Last 12
            foreach ($line in $tail) {
                $entry = ConvertFrom-JsonSafe $line
                if (-not $entry -or $entry.type -ne 'assistant') { continue }
                foreach ($block in @($entry.message.content)) {
                    if (-not $block -or -not $block.PSObject.Properties['type']) { continue }
                    switch ($block.type) {
                        'tool_use' { Write-Host ("    -> {0}" -f $block.name) -ForegroundColor DarkCyan }
                        'text'     { Write-Host ("    {0}" -f (Get-Excerpt $block.text 140)) }
                    }
                }
            }
            Write-Host ''
        }
    }

    return ($done -eq $agents.Count -and $agents.Count -gt 0)
}

$runDir = Resolve-RunDirectory -Root $Root -RunId $RunId

if ($Once) {
    [void](Show-Dashboard $runDir)
    return
}

Write-Host "Watching $($runDir.FullName) - Ctrl+C to stop" -ForegroundColor DarkGray
while ($true) {
    $finished = Show-Dashboard $runDir
    Write-Host "refreshing every ${IntervalSeconds}s - Ctrl+C to stop" -ForegroundColor DarkGray

    if ($finished) {
        Write-Host 'All agents have finished.' -ForegroundColor Green
        break
    }

    Start-Sleep -Seconds $IntervalSeconds
    # The run directory rotates its LastWriteTime as agents append; re-stat so the header stays truthful.
    $runDir.Refresh()
}
