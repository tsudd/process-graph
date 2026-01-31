# AI Agents Architecture - Canonical Reference

## Executive Summary

### Key principles

1. **2-level hierarchy**: Domain orchestrators are L1, workers are L2.

## Core concepts

An **agent** is a specialized AI assistant with:
- **Isolated context window** (prevents context pollution)
- **Specific domain expertise** (bugs, security, dead code, dependencies)
- **Defined inputs/outputs** (plan files → work → reports)
- **Independent tool access** (Bash, Read, Write, Edit, etc.)

**Location**: `./opencode/agents/{domain}/{orchestrators|workers}/`

## Agent Types

#### 1. Orchestrators (Level 1)

**Purpose**: Coordinate multi-phase workflows

**Responsibilities**:
- Create plan files for each phase
- Signal readiness to user (Return Control)
- Validate worker outputs at quality gates
- Track progress via TodoWrite
- Handle errors with rollback instructions
- Generate final summary reports

**CRITICAL RULES**:
- ❌ **NO Task tool** to invoke subagents
- ❌ **NO implementation work** (delegate to workers)
- ❌ **NO skip quality gate validations**
- ✅ **DO create plan files** before signaling
- ✅ **DO validate worker outputs** at quality gates
- ✅ **DO report status** to user

**Location**: `.opencode/agents/health/orchestrators/`

**Examples**:
- `bug-orchestrator.md`
- `security-orchestrator.md`
- `dead-code-orchestrator.md`
- `dependency-orchestrator.md`

#### 2. Workers (Level 2)

**Purpose**: Execute domain-specific work

**Responsibilities**:
- Read plan file first
- Execute domain work (detection, fixing, verification)
- Validate work internally
- Generate structured report
- Return to main session

**CRITICAL RULES**:
- ❌ **NO invoke other agents**
- ❌ **NO skip report generation**
- ❌ **NO report success without validation**
- ✅ **DO read plan file** first
- ✅ **DO generate changes logs** (for rollback)
- ✅ **DO self-validate** work

**Location**: `.claude/agents/health/workers/`

**Examples**:
- `bug-hunter.md`, `bug-fixer.md`
- `security-scanner.md`, `vulnerability-fixer.md`
- `dead-code-hunter.md`, `dead-code-remover.md`
- `dependency-auditor.md`, `dependency-updater.md`

#### 3. Skills (Utilities)

**Purpose**: Reusable utility functions

**Characteristics**:
- Stateless (no context needed)
- <100 lines logic
- Single responsibility
- No agent invocation

**Location**: `.opencode/skills/{skill-name}/SKILL.md`

**Examples**:
- `validate-plan-file`
- `run-quality-gate`
- `rollback-changes`
- `generate-report-header`

---

## Return Control Pattern

### How It Works

```mermaid
sequenceDiagram
    participant User
    participant Main as Main Claude Session
    participant Orch as Orchestrator
    participant Worker as Worker Agent

    User->>Main: /health bugs
    Main->>Orch: Invoke orchestrator via Task tool
    Orch->>Orch: Create .bug-detection-plan.json
    Orch->>Orch: Validate plan file
    Orch->>Orch: Update TodoWrite (in_progress)
    Orch->>User: Signal readiness, return control
    Orch-->>Main: Exit orchestrator
    Main->>Main: Read .bug-detection-plan.json
    Main->>Worker: Invoke bug-hunter via Task tool
    Worker->>Worker: Read plan file
    Worker->>Worker: Execute detection
    Worker->>Worker: Generate report
    Worker-->>Main: Return control
    Main->>Orch: Resume orchestrator via Task tool
    Orch->>Orch: Validate report (quality gate)
    Orch->>Orch: Create .bug-fixing-plan.json
    Orch->>User: Signal readiness, return control
    Orch-->>Main: Exit orchestrator
    Main->>Main: Read .bug-fixing-plan.json
    Main->>Worker: Invoke bug-fixer via Task tool
    Note: Cycle continues...
```

### Signal Readiness Protocol

Orchestrators must:

