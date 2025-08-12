# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a Unity game development project called "TechCore-EnemyAI" focused on Monster AI implementation using Behavior Trees (BT) and Finite State Machines (FSM). The project is built with Unity 2022.3.62f1.

## Development Commands

### Unity Development
- Open the project in Unity Editor to build, test, and run
- No additional build scripts or custom commands detected
- Standard Unity workflow applies

### Git Operations
- Repository uses standard git workflow
- Recent commits show ongoing development in Chinese, focusing on node implementations and refactoring

## Project Structure

### Core Architecture
```
Assets/Scripts/
├── MonsterBT/                 # Behavior Tree system for monster AI
│   ├── Runtime/              # Runtime BT components (empty currently)
│   ├── Editor/               # Editor extensions (empty currently) 
│   └── Resources/            # UI Toolkit visual editor
│       ├── BTVisualEditor.cs     # Main editor window class
│       ├── BTVisualEditor.uxml   # UI layout definition
│       └── BTVisualEditor.uss    # UI styling
└── MonsterFSM/               # Finite State Machine system
    ├── Core/                 # FSM core logic (empty currently)
    ├── Editor/               # FSM editor tools (empty currently)
    └── Resources/            # FSM resources (empty currently)
```

### Visual Editor System
- Custom UI Toolkit-based behavior tree editor (`BTVisualEditor`)
- Accessible via Unity menu: Window/UI Toolkit/BTVisualEditor
- Uses UXML for layout and USS for styling
- Currently displays placeholder "Hello World" content

### Key Dependencies
- Unity UI Toolkit (UIElements) for visual editing interface
- Standard Unity packages including:
  - Visual Scripting (1.9.4)
  - Timeline (1.7.7)
  - TextMeshPro (3.0.7)
  - Characters Animation feature package

## Development Notes

### Current State
- Project appears to be in early development phase
- Main systems (MonsterBT and MonsterFSM) have directory structure but minimal implementation
- Visual editor framework is set up but needs content implementation
- No test framework currently configured

### Code Patterns
- Follows Unity C# conventions
- Uses Unity's UI Toolkit for editor extensions
- Separates Runtime, Editor, and Resources concerns
- Chinese comments in commit history suggest bilingual development team

### File Organization
- Meta files properly tracked for Unity asset management
- Clear separation between behavior tree and state machine systems
- Editor tools isolated from runtime code